using System;

namespace BuilderScenario.Core.Entities
{
    public class ActionItem
    {
        public int Id { get; set; }
        public int StepItemId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
