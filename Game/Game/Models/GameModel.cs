using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models
{
    public class GameModel : BaseModel
    {
        public enum Category
        {
            Koala,
            Penguin,
            Dolphin
        }

        public enum GameMode
        {
            Standard,
            Custom
        }

        private bool isGameStarted;
        public bool IsGameStarted
        {
            get { return isGameStarted; }
            set
            {
                if (isGameStarted != value)
                {
                    isGameStarted = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isGameOver;

        public bool IsGameOver
        {
            get => isGameOver;
            set
            {
                isGameOver = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan totalTime;
        public TimeSpan TotalTime
        {
            get => totalTime;
            set
            {
                totalTime = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan timeRemaining;
        public TimeSpan TimeRemaining
        {
            get => timeRemaining;
            set
            {
                timeRemaining = value;
                OnPropertyChanged();
            }
        }

        private int rows;
        private int columns;
        private ObservableCollection<TokenModel> tokens;


        public int Rows
        {
            get => rows;
            set
            {
                rows = value;
                OnPropertyChanged();
            }
        }

        public int Columns
        {
            get => columns;
            set
            {
                columns = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TokenModel> Tokens
        {
            get => tokens;
            set
            {
                tokens = value;
                OnPropertyChanged();
            }
        }

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }

        private GameMode selectedMode;
        public GameMode SelectedMode
        {
            get => selectedMode;
            set
            {
                if (selectedMode != value)
                {
                    selectedMode = value;
                    OnPropertyChanged();
                    if (selectedMode == GameMode.Standard)
                    {
                        Rows = 4;
                        Columns = 4;
                    }
                }
            }
        }
    }
}

