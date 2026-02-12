using System;
using System.Collections.Generic;

namespace BuilderScenario.Core.Entities
{
    public class ActionGroup
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public int Order { get; set; }

        public Guid ScenarioId { get; set; }

        public List<StepItem> Steps { get; set; } = new();
    }
}
