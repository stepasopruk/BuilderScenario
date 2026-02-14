using System;
using System.Collections.Generic;

namespace BuilderScenario.Core.Entities
{
    public class ActionGroup
    {
        public int Id { get; set; }
        public int ScenarioId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<StepItem> Steps { get; set; } = new();
    }
}
