using System;

namespace GameLib
{
    public struct AgentField
    {
        public int Distance { get; }
        public DateTime Timestamp { get; }
        public FieldState IsGoal { get; }
    }

    public enum FieldState: byte
    {
        DiscoveredGoal,
        DiscoveredNotGoal,
        Unknown,
        NA
    }
}