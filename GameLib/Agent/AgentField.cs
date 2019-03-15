using System;

namespace GameLib
{
    public struct AgentField
    {
        private int distance;

        public int Distance
        {
            get => distance;
            set
            {
                distance = value;
                Timestamp = DateTime.UtcNow;
            }
        }

        public DateTime Timestamp { get; set; }
        public FieldState IsGoal { get; set; }
    }
}