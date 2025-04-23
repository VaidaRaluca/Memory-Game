using Game.Commands;
using Game.Models;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Game.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        private UserControl currentView;
        public ICommand SwitchToSignInViewCommand { get; set; }
        public ICommand SwitchToSignUpViewCommand { get; set; }
        public ICommand SwitchToGameViewCommand { get; set; }

        private SignInVM signInVM;

        public UserControl CurrentView
        {
            get
            {
                return currentView;
            }
            set
            {
                if (value != currentView)
                {
                    currentView = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindowVM()
        {
            signInVM = new SignInVM();
            signInVM.NavigateToSignUpRequested += OnNavigateToSignUpRequested;
            CurrentView = new SignInControl
            {
                DataContext = signInVM
            };

            signInVM.NavigateToGameRequested += OnNavigateToGameRequested;

            SwitchToSignInViewCommand = new RelayCommand(SwitchToSignInView);
            SwitchToSignUpViewCommand = new RelayCommand(SwitchToSignUpView);
            SwitchToGameViewCommand = new RelayCommand(SwitchToGameView);
        }

        #region SignInUpViews
        private void OnNavigateToSignUpRequested()
        {
            var signUpVM = new SignUpVM();
            signUpVM.UserAdded += (newUser) =>
            {
                if (!string.IsNullOrWhiteSpace(newUser.Username) && 
             !signInVM.Users.Any(user => user.Username == newUser.Username))
                {
                    signInVM.AddUser(newUser);
                    SwitchToSignInView(); 
                }
            };
            CurrentView = new SignUpControl
            {
                DataContext = signUpVM
            };
        }

        private void SwitchToSignInView()
        {
            CurrentView = new SignInControl
            {
                DataContext = signInVM
            };
        }

        private void SwitchToSignUpView()
        {
            CurrentView = new SignUpControl
            {
                DataContext = new SignUpVM()
            };
        }
        #endregion

        #region GameView
        private void SwitchToGameView()
        {
            var gameVM = new GameVM();
            gameVM.ExitRequested += OnExitRequested;
            CurrentView = new GameControl
            {
                DataContext = gameVM
            };
        }

        private void OnNavigateToGameRequested(UserModel selectedUser)
        {
            var gameVM = new GameVM(selectedUser);
            gameVM.ExitRequested += OnExitRequested;
            CurrentView = new GameControl
            {
                DataContext = gameVM
            };
        }

        private void OnExitRequested()
        {
            SwitchToSignInView(); 
        }
        #endregion
    }
}
