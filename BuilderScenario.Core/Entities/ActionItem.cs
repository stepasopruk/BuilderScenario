using System;

namespace BuilderScenario.Core.Entities
{
    public class ActionItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public int Order { get; set; }

        public Guid StepItemId { get; set; }
    }
}
