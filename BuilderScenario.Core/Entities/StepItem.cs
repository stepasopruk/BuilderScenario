using System;
using System.Collections.Generic;

namespace BuilderScenario.Core.Entities
{
    public class StepItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public int Order { get; set; }

        public Guid ActionGroupId { get; set; }

        public List<ActionItem> Actions { get; set; } = new();
    }
}
