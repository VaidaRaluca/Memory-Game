using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Game.Commands;

namespace Game.ViewModels
{
    public class SignUpVM : BaseVM
    {
        private UserModel newUser;
        private List<ImageModel> availableImages;
        private int selectedImageIndex;

        public event Action<UserModel> UserAdded = delegate { };
        public event Action OnSignUpComplete = delegate { };

        public UserModel NewUser
        {
            get => newUser;
            set
            {
                newUser = value;
                OnPropertyChanged();
            }
        }

        public List<ImageModel> AvailableImages
        {
            get => availableImages;
            set
            {
                availableImages = value;
                OnPropertyChanged();
            }
        }

        public SignUpVM()
        {
            NewUser = new UserModel();

            AvailableImages = new List<ImageModel>
            {
                new ImageModel("quokka1.jpg", "quokka1"),
                new ImageModel("quokka2.jpg", "quokka2"),
                new ImageModel("quokka3.jpg", "quokka3"),
                new ImageModel("quokka4.jpg", "quokka4"),
                new ImageModel("quokka5.jpg", "quokka5")
            };

            selectedImageIndex = 2;
            NewUser.Avatar = AvailableImages[selectedImageIndex];

            PreviousImageCommand = new RelayCommand(ShowPreviousImage, CanShowPreviousImage);
            NextImageCommand = new RelayCommand(ShowNextImage, CanShowNextImage);
        }

        #region AddUser

        private ICommand addUserCommand;
        public ICommand AddUserCommand
        {
            get
            {
                if (addUserCommand == null)
                {
                    addUserCommand = new RelayCommand(ExecuteAddUser, CanExecuteAddUser);
                }
                return addUserCommand;
            }
        }
        private bool CanExecuteAddUser()
        {
            return !string.IsNullOrWhiteSpace(NewUser.Username);
        }
        private void ExecuteAddUser()
        {
            Console.WriteLine($"New user {NewUser.Username} has been added with avatar {NewUser.Avatar.ImagePath}.");
            UserAdded?.Invoke(NewUser);
            OnSignUpComplete?.Invoke();
        }
        #endregion

        #region ShowImages
        public ICommand PreviousImageCommand { get; set; }
        public ICommand NextImageCommand { get; set; }

        private void ShowPreviousImage()
        {
            if (selectedImageIndex > 0)
            {
                selectedImageIndex--;
            }
            NewUser.Avatar = AvailableImages[selectedImageIndex];
            UpdateCommandStates();
        }

        private void ShowNextImage()
        {
            if (selectedImageIndex < AvailableImages.Count - 1)
            {
                selectedImageIndex++;
            }
            NewUser.Avatar = AvailableImages[selectedImageIndex];
            UpdateCommandStates();
        }

        private bool CanShowPreviousImage()
        {
            return selectedImageIndex > 0; 
        }

        private bool CanShowNextImage()
        {
            return selectedImageIndex < AvailableImages.Count - 1;
        }
        #endregion

        private void UpdateCommandStates()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
