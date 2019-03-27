using GameLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectGameGUI.Models
{
    public class Player : GameObject
    {
        public Piece HeldPiece { get; set; }
        public string Name { get; set; }
        public Team Team { get; set; }
        public bool IsActive { get; set; }
    }
}
