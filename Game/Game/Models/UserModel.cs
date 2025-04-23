using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models
{
    public class UserModel : BaseModel
    {
        private string username;
        private ImageModel avatar;
        private uint playedGames;
        private uint wonGames;

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public ImageModel Avatar
        {
            get => avatar;
            set
            {
                avatar = value;
                OnPropertyChanged();
            }
        }

        public uint PlayedGames
        { 
            get => playedGames;
            set
            {
                playedGames = value;
                OnPropertyChanged();
            }
        }

        public uint WonGames
        {
            get => wonGames;
            set
            {
                wonGames = value;
                OnPropertyChanged();
            }
        }


        public UserModel(string username, ImageModel avatar, uint playedGames, uint wonGames)
        {
            this.username = username;
            this.avatar = avatar;
            this.playedGames = playedGames;
            this.wonGames = wonGames;
        }

       public UserModel() { }
    }

}
