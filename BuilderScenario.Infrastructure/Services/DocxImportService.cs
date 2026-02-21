using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using BuilderScenario.Core.Entities;

public class DocxImportService
{
    public Scenario Import(string filePath)
    {
        var scenario = new Scenario();

        using var doc = WordprocessingDocument.Open(filePath, false);
        var body = doc.MainDocumentPart.Document.Body;

        var paragraphs = body.Elements<Paragraph>()
                             .Select(p => p.InnerText.Trim())
                             .Where(t => !string.IsNullOrWhiteSpace(t))
                             .ToList();

        ActionGroup? currentGroup = null;
        StepItem? currentStep = null;

        foreach (var line in paragraphs)
        {
            // GROUP
            if (line.StartsWith("$$$"))
            {
                var groupName = line.Replace("$$$", "").Trim();

                currentGroup = new ActionGroup
                {
                    Name = groupName
                };

                scenario.Groups.Add(currentGroup);
                continue;
            }

            // STEP
            if (line.StartsWith("$$"))
            {
                if (currentGroup == null) continue;

                var stepName = line.Replace("$$", "").Trim();

                currentStep = new StepItem
                {
                    Name = stepName
                };

                currentGroup.Steps.Add(currentStep);
                continue;
            }

            // SUBSTEP
            if (line.StartsWith("$"))
            {
                if (currentStep == null) continue;

                var clean = line.Replace("$", "").Trim();

                // Удаляем номер "1." если есть
                clean = System.Text.RegularExpressions.Regex
                        .Replace(clean, @"^\d+\.\s*", "");

                var action = new ActionItem
                {
                    Name = clean
                };

                currentStep.Actions.Add(action);
            }
        }

        // Если у шага нет подшагов — добавляем один с именем шага
        foreach (var group in scenario.Groups)
        {
            foreach (var step in group.Steps)
            {
                if (!step.Actions.Any())
                {
                    step.Actions.Add(new ActionItem
                    {
                        Name = step.Name
                    });
                }
            }
        }

        return scenario;
    }
}