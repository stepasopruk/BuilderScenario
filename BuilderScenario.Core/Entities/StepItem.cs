using System;
using System.Collections.Generic;

namespace BuilderScenario.Core.Entities
{
    public class StepItem
    {
        public int Id { get; set; }
        public int ActionGroupId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<ActionItem> Actions { get; set; } = new();
    }
}
