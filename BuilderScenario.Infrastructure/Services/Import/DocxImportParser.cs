using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using BuilderScenario.Core.Entities;
using System.Text.RegularExpressions;
using System.Text;

namespace BuilderScenario.Infrastructure.Services.Import
{
    public class DocxImportParser : IImportParser
    {
        public bool CanParse(string fileName)
            => fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase);

        public Scenario Parse(string filePath)
        {
            var scenario = new Scenario();
            var lines = new List<string>();

            try
            {
                // Открываем документ
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    var body = wordDoc.MainDocumentPart?.Document.Body;
                    if (body == null)
                        throw new Exception("Не удалось прочитать документ");

                    // Проходим по каждому параграфу отдельно
                    foreach (var paragraph in body.Elements<Paragraph>())
                    {
                        // Получаем текст из параграфа
                        string text = GetParagraphText(paragraph);

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            lines.Add(text.Trim());
                            Console.WriteLine($"Найден параграф: {text.Trim()}"); // Для отладки
                        }
                    }

                    // Если не нашли через параграфы, пробуем другой способ
                    if (!lines.Any())
                    {
                        // Получаем все текстовые элементы
                        var texts = body.Descendants<Text>();
                        foreach (var text in texts)
                        {
                            if (!string.IsNullOrWhiteSpace(text.Text))
                            {
                                lines.Add(text.Text.Trim());
                            }
                        }
                    }
                }

                // Выводим количество найденных строк для отладки
                Console.WriteLine($"Найдено строк: {lines.Count}");
                foreach (var line in lines)
                {
                    Console.WriteLine($"Строка: {line}");
                }

                if (!lines.Any())
                {
                    throw new Exception("Документ не содержит текста. Убедитесь, что в документе есть текст и он сохранен в правильном формате.");
                }

                // Первая строка - название сценария
                scenario.Name = lines.First();

                ActionGroup? currentGroup = null;
                StepItem? currentStep = null;

                // Обрабатываем остальные строки
                for (int i = 1; i < lines.Count; i++)
                {
                    var line = lines[i].Trim();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    Console.WriteLine($"Обработка строки {i}: {line}");

                    // Группа ($$$)
                    if (line.StartsWith("$$$"))
                    {
                        string groupName = line.Substring(3).Trim();
                        if (string.IsNullOrEmpty(groupName))
                            groupName = $"Группа {scenario.Groups.Count + 1}";

                        currentGroup = new ActionGroup
                        {
                            Name = groupName,
                            Order = scenario.Groups.Count
                        };
                        scenario.Groups.Add(currentGroup);
                        currentStep = null;
                        Console.WriteLine($"Добавлена группа: {groupName}");
                    }
                    // Шаг ($$)
                    else if (line.StartsWith("$$"))
                    {
                        if (currentGroup == null)
                        {
                            currentGroup = new ActionGroup
                            {
                                Name = "Основная группа",
                                Order = scenario.Groups.Count
                            };
                            scenario.Groups.Add(currentGroup);
                            Console.WriteLine($"Создана группа по умолчанию");
                        }

                        string stepName = line.Substring(2).Trim();
                        if (string.IsNullOrEmpty(stepName))
                            stepName = $"Шаг {currentGroup.Steps.Count + 1}";

                        currentStep = new StepItem
                        {
                            Name = stepName,
                            Order = currentGroup.Steps.Count
                        };
                        currentGroup.Steps.Add(currentStep);
                        Console.WriteLine($"Добавлен шаг: {stepName}");
                    }
                    // Действие ($)
                    else if (line.StartsWith("$"))
                    {
                        if (currentGroup == null)
                        {
                            currentGroup = new ActionGroup
                            {
                                Name = "Основная группа",
                                Order = scenario.Groups.Count
                            };
                            scenario.Groups.Add(currentGroup);
                            Console.WriteLine($"Создана группа по умолчанию");
                        }

                        if (currentStep == null)
                        {
                            currentStep = new StepItem
                            {
                                Name = $"Шаг {currentGroup.Steps.Count + 1}",
                                Order = currentGroup.Steps.Count
                            };
                            currentGroup.Steps.Add(currentStep);
                            Console.WriteLine($"Создан шаг по умолчанию");
                        }

                        string actionName = line.Substring(1).Trim();
                        // Удаляем номера в начале (1., 2., и т.д.)
                        actionName = Regex.Replace(actionName, @"^\d+\.\s*", "");

                        if (string.IsNullOrEmpty(actionName))
                            actionName = $"Действие {currentStep.Actions.Count + 1}";

                        var action = new ActionItem
                        {
                            Name = actionName,
                            Order = currentStep.Actions.Count
                        };
                        currentStep.Actions.Add(action);
                        Console.WriteLine($"Добавлено действие: {actionName}");
                    }
                    else
                    {
                        // Если строка не начинается с $, это может быть продолжение предыдущей строки
                        // или обычный текст. Игнорируем или добавляем как комментарий
                        Console.WriteLine($"Игнорируется обычный текст: {line}");
                    }
                }

                // Если ничего не создалось, создаем тестовые данные
                if (!scenario.Groups.Any())
                {
                    Console.WriteLine("Не найдено групп, создаем тестовые данные");
                    return CreateTestScenario();
                }

                return scenario;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при парсинге документа: {ex.Message}", ex);
            }
        }

        private string GetParagraphText(Paragraph paragraph)
        {
            var text = new StringBuilder();

            foreach (var element in paragraph.Elements())
            {
                if (element is Run run)
                {
                    text.Append(run.InnerText);
                }
                else if (element is Hyperlink hyperlink)
                {
                    foreach (var hyperlinkRun in hyperlink.Elements<Run>())
                    {
                        text.Append(hyperlinkRun.InnerText);
                    }
                }
            }

            return text.ToString();
        }

        private Scenario CreateTestScenario()
        {
            return new Scenario
            {
                Name = "Тестовый сценарий",
                Groups = new List<ActionGroup>
                {
                    new ActionGroup
                    {
                        Name = "Группа 1",
                        Order = 0,
                        Steps = new List<StepItem>
                        {
                            new StepItem
                            {
                                Name = "Шаг 1",
                                Order = 0,
                                Actions = new List<ActionItem>
                                {
                                    new ActionItem { Name = "Действие 1", Order = 0 },
                                    new ActionItem { Name = "Действие 2", Order = 1 }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}