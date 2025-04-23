using Game.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.Commands;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace Game.ViewModels
{
    public class SignInVM : BaseVM
    {
        private UserModel selectedUser;
        private ObservableCollection<UserModel> users;
        private string usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"..\..\..\Data\Users.json");
        public UserModel SelectedUser
        {
            get => selectedUser;
            set
            {
                selectedUser = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserModel> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        public SignInVM()
        {
            LoadUsersFromJson();
            SwitchToSignUpViewCommand = new RelayCommand(SwitchToSignUpView);
            DeleteUserCommand = new RelayCommand(DeleteSelectedUser, CanExecuteDeleteUser);
            SwitchToGameCommand = new RelayCommand(SwitchToGameView, CanExecuteSwitchToGameView);
        }

        #region SignUp
        public event Action NavigateToSignUpRequested = delegate { };
        public ICommand SwitchToSignUpViewCommand { get; set; }

        private void SwitchToSignUpView()
        {
            NavigateToSignUpRequested?.Invoke();
        }
        #endregion

        #region Game
        public event Action<UserModel> NavigateToGameRequested = delegate { }; //here
        public ICommand SwitchToGameCommand { get; set; }
        private void SwitchToGameView()
        {
            NavigateToGameRequested?.Invoke(SelectedUser);
        }
        private bool CanExecuteSwitchToGameView()
        {
            return SelectedUser != null;
        }
        #endregion

        #region DeleteUser
        public ICommand DeleteUserCommand { get; }
        private void DeleteSelectedUser()
        {
            if (SelectedUser != null && Users.Contains(SelectedUser))
            {
                var userFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Data\Saves", SelectedUser.Username);

                if (Directory.Exists(userFolderPath))
                {
                    Directory.Delete(userFolderPath, true);
                }

                Users.Remove(SelectedUser);
                SelectedUser = null;
                SaveUsersToJson();
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private bool CanExecuteDeleteUser()
        {
            return SelectedUser != null;
        }
        #endregion

        #region AddUser
        public void AddUser(UserModel newUser)
        {
            Users.Add(newUser);
            SaveUsersToJson();
            OnPropertyChanged(nameof(Users));
        }
        #endregion

        #region Json
        private void SaveUsersToJson()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(usersFilePath));
            var json = JsonConvert.SerializeObject(Users);
            File.WriteAllText(usersFilePath, json);
        }

        private void LoadUsersFromJson()
        {
            if (File.Exists(usersFilePath))
            {
                var json = File.ReadAllText(usersFilePath);
                var loadedUsers = JsonConvert.DeserializeObject<ObservableCollection<UserModel>>(json);
                if (loadedUsers != null && loadedUsers.Any())
                {
                    Users = loadedUsers;
                }
                else
                {
                    Users = new ObservableCollection<UserModel>();
                }
            }
            else
            {
                Users = new ObservableCollection<UserModel>();
            }
        }
        #endregion
    }
}
