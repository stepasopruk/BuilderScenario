using System;
using System.Collections.Generic;

namespace BuilderScenario.Core.Entities
{
    public class Scenario
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ActionGroup> Groups { get; set; } = new();
    }
}
