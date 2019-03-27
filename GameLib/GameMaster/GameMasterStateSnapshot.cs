using System.Collections.Generic;

namespace GameLib
{
    public class GameMasterStateSnapshot
    {
        public readonly Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();

        public readonly GameMasterBoard Board;

        public GameMasterStateSnapshot(GameMasterState state)
        {
            Dictionary<int, PlayerState> playerStates = new Dictionary<int, PlayerState>(state.PlayerStates);

            foreach (var (id, ps) in playerStates)
            {
                this.PlayerStates.Add(id, (PlayerState)ps.Clone());
            }

            this.Board = (GameMasterBoard)state.Board.Clone();
        }
    }
}