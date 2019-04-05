using System;

namespace GameLib
{
    public struct AgentField
    {
        public AgentFieldState IsGoal { get; set; }

        public int Distance { get; private set; }
        public int Timestamp { get; private set; }

        public void SetDistance(int distance, int timestamp)
        {
            this.Distance = distance;
            this.Timestamp = timestamp;
        }

        public AgentField(int distance, AgentFieldState fieldState, int timestamp)
        {
            this.Distance = distance;
            this.IsGoal = fieldState;
            this.Timestamp = timestamp;
        }
    }
}