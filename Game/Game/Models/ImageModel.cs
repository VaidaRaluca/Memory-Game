using System.ComponentModel;

namespace Game.Models
{
    public class ImageModel : BaseModel
    {
        private string imagePath;
        private string imageName;

        public string ImagePath
        {
            get => imagePath;
            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public string ImageName
        {
            get => imageName;
            set
            {
                if (imageName != value)
                {
                    imageName = value;
                    OnPropertyChanged(nameof(ImageName));
                }
            }
        }

        public ImageModel(string imagePath, string imageName)
        {
            ImagePath = imagePath;
            ImageName = imageName;
        }

        public ImageModel() { }
    }
}
