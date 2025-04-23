using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Models;
using System.Collections.ObjectModel;
using static Game.Utils.SaveData;
using System.IO;
using System.Text.Json;

namespace Game.ViewModels
{   
    namespace Game.ViewModels
    {
        public class StatsVM : BaseVM
        {
            public ObservableCollection<StatsModel> Users { get; set; }

            public StatsVM(ObservableCollection<UserModel> users)
            {
                Users = new ObservableCollection<StatsModel>();

                foreach (var user in users)
                {
                    var stats = LoadUserStats(user.Username);
                    if (stats != null)
                        Users.Add(stats);
                }
            }

            private StatsModel LoadUserStats(string username)
            {
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Data\Saves", username);
                if (!Directory.Exists(basePath))
                    return null;

                var files = Directory.GetFiles(basePath, "game*.json");
                uint played = 0;
                uint won = 0;

                foreach (var file in files)
                {
                        var json = File.ReadAllText(file);
                        var saveData = JsonSerializer.Deserialize<GameSaveData>(json);

                        if (saveData != null)
                        {
                            played++;
                            if (saveData.Tokens.All(t => t.IsMatched)) 
                                won++;
                        }
                }

                return new StatsModel
                {
                    Username = username,
                    PlayedGames = played,
                    WonGames = won
                };
            }
        }
    }

}
