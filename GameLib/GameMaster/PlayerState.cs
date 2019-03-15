﻿using System;

namespace GameLib
{
    public struct PlayerState
    {
        public (int X, int Y) Position { get; set; }
        public bool IsLeader { get; }
        public Team Team { get; }
        public Piece Piece { get; set; }

        public DateTime LastRequestTimestamp { get; set; }
        public int LastActionDelay { get; set; }

        public bool IsEligibleForAction => DateTime.UtcNow >= LastRequestTimestamp.AddMilliseconds(LastActionDelay);

        public PlayerState(int x, int y, Team team = Team.Blue, bool isLeader = false)
        {
            Position = (x, y);
            IsLeader = isLeader;
            Team = team;

            Piece = null;
            LastRequestTimestamp = DateTime.MinValue;
            LastActionDelay = 0;
        }
    }
}