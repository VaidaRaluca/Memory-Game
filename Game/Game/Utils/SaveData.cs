using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Utils
{
    public class SaveData
    {
        public class TokenSaveData
        {
            public string ImageName { get; set; }
            public bool IsFlipped { get; set; }
            public bool IsMatched { get; set; }
        }

        public class GameSaveData
        {
            public string Username { get; set; }
            public uint WonGames { get; set; }
            public uint PlayedGames { get; set; }

            public int Rows { get; set; }
            public int Columns { get; set; }
            public string SelectedCategory { get; set; }
            public string SelectedMode { get; set; }
            public string TimeRemaining { get; set; }
            public bool IsGameOver { get; set; }
            public bool IsGameStarted { get; set; }

            public List<TokenSaveData> Tokens { get; set; }
        }

    }
}
