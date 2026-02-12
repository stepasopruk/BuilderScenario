using System;
using System.Collections.Generic;

namespace BuilderScenario.Core.Entities
{
    public class Scenario
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public List<ActionGroup> Groups { get; set; } = new();
    }
}
