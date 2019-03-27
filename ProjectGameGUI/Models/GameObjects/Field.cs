using GameLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectGameGUI.Models
{
    public class Field : GameObject
    {
        public string Name { get; set; }
        public Team? Team { get; set; }
        public bool IsUndiscoveredGoal { get; set; }
    }
}
