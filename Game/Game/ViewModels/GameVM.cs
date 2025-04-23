using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.Commands;
using Game.Models;
using System.Timers;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Threading;
using static Game.Models.GameModel;
using System.Printing;
using System.Text.Json;
using static Game.Utils.SaveData;
using Game.ViewModels.Game.ViewModels;
using Game.Views;
using System.Windows;

namespace Game.ViewModels
{
    public class GameVM : BaseVM
    {
        private GameModel game;
        private System.Timers.Timer gameTimer;
        private TokenModel firstFlippedToken;
        private TokenModel secondFlippedToken;
        private ICommand exitCommand;
        private ICommand newGameCommand;

        private bool isLoadingGame = false;

        public ICommand FlipTokenCommand { get; }

        public GameModel Game
        {
            get => game;
            set
            {
                game = value;
                OnPropertyChanged();
            }
        }

        public int Rows
        {
            get => Game.Rows;
            set
            {
                Game.Rows = value;
                OnPropertyChanged();
                if (!isLoadingGame)
                    RegenerateTokens();
            }
        }

        public int Columns
        {
            get => Game.Columns;
            set
            {
                Game.Columns = value;
                OnPropertyChanged();
                if (!isLoadingGame)
                    RegenerateTokens();
            }
        }

        public ObservableCollection<TokenModel> Tokens
        {
            get => Game.Tokens;
            set
            {
                Game.Tokens = value;
                OnPropertyChanged();
            }
        }

        public Category SelectedCategory
        {
            get => Game.SelectedCategory;
            set
            {
                Game.SelectedCategory = value;
                OnPropertyChanged();
                if (!isLoadingGame)
                    RegenerateTokens();
            }
        }

        #region Modes

        public GameMode SelectedMode
        {
            get => Game.SelectedMode;
            set
            {
                if (Game.SelectedMode != value)
                {
                    Game.SelectedMode = value;
                    OnPropertyChanged();
                    if (Game.SelectedMode == GameMode.Standard)
                    {
                        Rows = 4;
                        Columns = 4;
                    }
                    RegenerateTokens();
                }
            }
        }

        public bool IsStandardMode
        {
            get => SelectedMode == GameMode.Standard;
            set
            {
                if (value && SelectedMode != GameMode.Standard)
                {
                    SelectedMode = GameMode.Standard;
                    OnPropertyChanged(nameof(IsStandardMode));
                    OnPropertyChanged(nameof(IsCustomMode));
                }
                else if (!value && SelectedMode == GameMode.Standard)
                {
                    SelectedMode = GameMode.Custom;
                    OnPropertyChanged(nameof(IsCustomMode));
                    OnPropertyChanged(nameof(IsStandardMode));
                }
            }
        }

        public bool IsCustomMode
        {
            get => SelectedMode == GameMode.Custom;
            set
            {
                if (value && SelectedMode != GameMode.Custom)
                {
                    SelectedMode = GameMode.Custom;
                    OnPropertyChanged(nameof(IsStandardMode));
                    OnPropertyChanged(nameof(IsCustomMode));
                }
                else if (!value && SelectedMode == GameMode.Custom)
                {
                    SelectedMode = GameMode.Standard;
                    OnPropertyChanged(nameof(IsCustomMode));
                    OnPropertyChanged(nameof(IsStandardMode));
                }
            }
        }

        #endregion
        public ICommand SetRowsCommand { get; private set; }
        public ICommand SetColumnsCommand { get; private set; }

        public UserModel CurrentUser { get; private set; }

        public GameVM(UserModel user) : this()
        {
            CurrentUser = user;
            OnPropertyChanged(nameof(CurrentUser));
        }

        public GameVM()
        {
            Game = new GameModel();

            OpenUserStatsCommand = new RelayCommand(param => OpenUserStats());

            AboutCommand = new RelayCommand(param => About());

            SetRowsCommand = new RelayCommand(param =>
            {
                if (int.TryParse(param.ToString(), out int rows))
                {
                    Rows = rows;
                }
            });

            SetColumnsCommand = new RelayCommand(param =>
            {
                if (int.TryParse(param.ToString(), out int columns))
                {
                    Columns = columns;
                }
            });

            Tokens = new ObservableCollection<TokenModel>();
            SelectedMode = GameMode.Custom;
            SelectedCategory = Category.Koala;
            GenerateTokens();
            FlipTokenCommand = new RelayCommand(
                 param => FlipToken(param as TokenModel),
                 param => CanFlipToken(param as TokenModel));
        }

        #region SaveLoad
        public ObservableCollection<string> SavedGames { get; set; }
        public ICommand SaveGameCommand => new RelayCommand(SaveGame);
        public void SaveGame()
        {
            var saveData = new GameSaveData
            {
                Username = CurrentUser.Username,
                WonGames = CurrentUser.WonGames,
                PlayedGames = CurrentUser.PlayedGames,
                Rows = Rows,
                Columns = Columns,
                SelectedCategory = SelectedCategory.ToString(),
                SelectedMode = SelectedMode.ToString(),
                TimeRemaining = TimeRemaining.ToString(),
                IsGameOver = Game.IsGameOver,
                IsGameStarted = Game.IsGameStarted,
                Tokens = Tokens.Select(t => new TokenSaveData
                {
                    ImageName = t.Image.ImageName,
                    IsFlipped = t.IsFlipped,
                    IsMatched = t.IsMatched
                }).ToList()
            };

            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Data\Saves");

            var userFolderPath = Path.Combine(basePath, CurrentUser.Username);
            Directory.CreateDirectory(userFolderPath);

            var existingFiles = Directory.GetFiles(userFolderPath, "game*.json");

            int gameNumber = existingFiles.Length + 1;

            var savePath = Path.Combine(userFolderPath, $"game{gameNumber}.json");

            var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(savePath, json);

        }

        public ICommand LoadGameCommand => new RelayCommand(LoadGame);

        private string GetImagePath(string imageName)
        {
            return $"{imageName}.jpg";
        }

        public void LoadGame()
        {
            isLoadingGame = true;
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Data\Saves");
            var userFolderPath = Path.Combine(basePath, CurrentUser.Username);
            var existingFiles = Directory.GetFiles(userFolderPath, "game*.json");

            var latestGameFile = existingFiles
                .Select(file => new
                 {
                      FilePath = file,
                      FileNumber = int.Parse(Path.GetFileNameWithoutExtension(file).Substring(4))
                 })
                .OrderByDescending(x => x.FileNumber)
                .FirstOrDefault();

            var savePath = latestGameFile.FilePath;

            var json = File.ReadAllText(savePath);
            var saveData = JsonSerializer.Deserialize<GameSaveData>(json);

            Rows = saveData.Rows;
            Columns = saveData.Columns;
            SelectedCategory = Enum.Parse<Category>(saveData.SelectedCategory);
            SelectedMode = Enum.Parse<GameMode>(saveData.SelectedMode);
            TimeRemaining = TimeSpan.Parse(saveData.TimeRemaining);
            Game.IsGameOver = saveData.IsGameOver;
            Game.IsGameStarted = saveData.IsGameStarted;

            Tokens.Clear();
            foreach (var tokenSave in saveData.Tokens)
            {
                var image = new ImageModel
                {
                    ImageName = tokenSave.ImageName,
                    ImagePath = GetImagePath(tokenSave.ImageName)
                };

                var token = new TokenModel(image, tokenSave.IsMatched, tokenSave.IsFlipped)
                {
                    IsMatched = tokenSave.IsMatched,
                    IsFlipped = tokenSave.IsFlipped
                };

                Tokens.Add(token);
            }
            isLoadingGame = false;
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(SelectedMode));
            OnPropertyChanged(nameof(TimeRemaining));
            OnPropertyChanged(nameof(Tokens));
        }

        #endregion


        #region TimerHandling

        private bool isSliderEnabled = true;

        public bool IsSliderEnabled
        {
            get => isSliderEnabled;
            set
            {
                isSliderEnabled = value;
                OnPropertyChanged();
            }
        }

        private double sliderValue;

        public double SliderValue
        {
            get => sliderValue;
            set
            {
                sliderValue = value;
                TotalTime = TimeSpan.FromMinutes(sliderValue);
                TimeRemaining = TotalTime;
                OnPropertyChanged();
            }
        }

        private DispatcherTimer gameCountdownTimer;
        public TimeSpan TotalTime
        {
            get => Game.TotalTime;
            set
            {
                Game.TotalTime = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan TimeRemaining
        {
            get => Game.TimeRemaining;
            set
            {
                Game.TimeRemaining = value;
                OnPropertyChanged();
            }
        }

        private void StartGameTimer()
        {
            gameCountdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            gameCountdownTimer.Tick += (sender, e) =>
            {
                if (TimeRemaining > TimeSpan.Zero)
                {
                    TimeRemaining = TimeRemaining.Subtract(TimeSpan.FromSeconds(1));
                }
                else
                {
                    gameCountdownTimer.Stop();
                    MessageBox.Show("Better Luck Next Time!");
                    Game.IsGameOver = true;
                }
            };
            gameCountdownTimer.Start();
        }

        private void StopGameTimer()
        {
            if (gameCountdownTimer != null)
            {
                gameCountdownTimer.Stop();
                CurrentUser.PlayedGames++;
                gameCountdownTimer = null;
            }
        }
        #endregion

        #region NewGame
        public ICommand NewGameCommand
        {
            get
            {
                if (newGameCommand == null)
                {
                    newGameCommand = new RelayCommand(ExecuteNewGame);
                }
                return newGameCommand;
            }
        }
        private void ExecuteNewGame()
        {
            RegenerateTokens();
            Game.IsGameOver = false;
            Game.IsGameStarted = false;
            Game.TimeRemaining = TotalTime;
            IsSliderEnabled = true;
        }
        #endregion

        #region Exit
        public event Action ExitRequested = delegate { };
        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                {
                    exitCommand = new RelayCommand(ExecuteExit);
                }
                return exitCommand;
            }
        }

        private void ExecuteExit()
        {
            ExitRequested?.Invoke();
        }
        #endregion

        #region TokenHandling

        private void RegenerateTokens()
        {
            StopGameTimer();
            Tokens.Clear();
            GenerateTokens();
            firstFlippedToken = null;
            secondFlippedToken = null;
            Game.IsGameOver = false;
            Game.IsGameStarted = false;
            TimeRemaining = TotalTime;
        }

        private void GenerateTokens()
        {
            var category = SelectedCategory;
            var random = new Random();
            int number = Rows * Columns;
            if (number % 2 != 0) number--;
            var imageList = GetImagesForCategory(category);
            var pairedImages = imageList.Take(number / 2)
                                        .SelectMany(img => new[] { img, img })
                                        .OrderBy(_ => random.Next())
                                        .ToList();

            foreach (var img in pairedImages)
            {
                var token = new TokenModel(img)
                {
                    IsFlipped = false,
                    IsMatched = false
                };
                Tokens.Add(token);
            }

        }

        private void FlipToken(TokenModel token)
        {
            if (!CanFlipToken(token))
                return;

            if (!Game.IsGameStarted)
            {
                Game.IsGameStarted = true;
                IsSliderEnabled = false;
                StartGameTimer();
            }

            token.IsFlipped = true;

            if (firstFlippedToken == null)
            {
                firstFlippedToken = token;
            }
            else if (secondFlippedToken == null)
            {
                secondFlippedToken = token;

                if (firstFlippedToken.Image.ImageName == secondFlippedToken.Image.ImageName)
                {
                    firstFlippedToken.IsMatched = true;
                    secondFlippedToken.IsMatched = true;

                    firstFlippedToken = null;
                    secondFlippedToken = null;

                    if (Tokens.All(t => t.IsMatched))
                    {
                        Game.IsGameOver = true;
                        MessageBox.Show("Congratulations!");
                        StopGameTimer();
                        CurrentUser.WonGames++;
                    }
                }
                else
                {
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(0.5)
                    };
                    timer.Tick += (s, e) =>
                    {
                        firstFlippedToken.IsFlipped = false;
                        secondFlippedToken.IsFlipped = false;

                        firstFlippedToken = null;
                        secondFlippedToken = null;
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
        }

        private bool CanFlipToken(TokenModel token)
        {
            if (Game.IsGameOver)
            {
                return false;
            }
            if (token.IsFlipped)
            {
                return false;
            }
            if (firstFlippedToken != null && secondFlippedToken != null)
            {
                return false;
            }
            return true;
        }

        private List<ImageModel> GetImagesForCategory(GameModel.Category category)
        {
            string prefix = category.ToString().ToLower();
            var allImageNames = new List<string>
    {
        "koala1.jpg",
        "koala2.jpg",
        "koala3.jpg",
        "koala4.jpg",
        "koala5.jpg",
        "koala6.jpg",
        "koala7.jpg",
        "koala8.jpg",
        "koala9.jpg",
        "koala10.jpg",
        "koala11.jpg",
        "koala12.jpg",
        "koala13.jpg",
        "koala14.jpg",
        "koala15.jpg",
        "koala16.jpg",
        "koala17.jpg",
        "koala18.jpg",
        "dolphin1.jpg",
        "dolphin2.jpg",
        "dolphin3.jpg",
        "dolphin4.jpg",
        "dolphin5.jpg",
        "dolphin6.jpg",
        "dolphin7.jpg",
        "dolphin8.jpg",
        "dolphin9.jpg",
        "dolphin10.jpg",
        "dolphin11.jpg",
        "dolphin12.jpg",
        "dolphin13.jpg",
        "dolphin14.jpg",
        "dolphin15.jpg",
        "dolphin16.jpg",
        "dolphin17.jpg",
        "dolphin18.jpg",
        "penguin1.jpg",
        "penguin2.jpg",
        "penguin3.jpg",
        "penguin4.jpg",
        "penguin5.jpg",
        "penguin6.jpg",
        "penguin7.jpg",
        "penguin8.jpg",
        "penguin9.jpg",
        "penguin10.jpg",
        "penguin11.jpg",
        "penguin12.jpg",
        "penguin13.jpg",
        "penguin14.jpg",
        "penguin15.jpg",
        "penguin16.jpg",
        "penguin17.jpg",
        "penguin18.jpg"
    };

            var filtered = allImageNames
                .Where(name => name.StartsWith(prefix))
                .ToList();

            return filtered.Select(file =>
            {
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                return new ImageModel(file, nameWithoutExtension);
            }).ToList();
        }
        #endregion

        #region Stats
        public ICommand OpenUserStatsCommand { get; }

        private void OpenUserStats()
        {
            var signInVM = new SignInVM();
            var statsWindow = new Stats
            {
                DataContext = new StatsVM(signInVM.Users)
            };
            statsWindow.ShowDialog();
        }
        #endregion

        #region About
        public ICommand AboutCommand { get; }

        private void About()
        {
            string email = "example@com";
            string mesaj = $"Nume: Vaida Raluca\n" +
                   $"Email: {email}\n";

            MessageBoxResult result = MessageBox.Show(mesaj, "Do you want to access the address?", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($"mailto:{email}")
                {
                    UseShellExecute = true
                });
            }
        }
        #endregion
    }
}

