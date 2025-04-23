using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Models
{
    public class TokenModel : BaseModel
    {
        private ImageModel image;
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        public ImageModel Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private bool isFlipped;
        public bool IsFlipped
        {
            get => isFlipped;
            set
            {
                if (isFlipped != value)
                {
                    isFlipped = value;
                    DisplayedImage = isFlipped || isMatched ? Image?.ImagePath : "bg1.png";
                    OnPropertyChanged(nameof(IsFlipped));
                    OnPropertyChanged(nameof(DisplayedImage));
                }
            }
        }

        private bool isMatched;
        public bool IsMatched
        {
            get => isMatched;
            set
            {
                if (isMatched != value)
                {
                    isMatched = value;
                    OnPropertyChanged(nameof(IsMatched));
                    OnPropertyChanged(nameof(DisplayedImage));
                }
            }
        }

        private string displayedImage;
        public string DisplayedImage
        {
            get => displayedImage;
            set
            {
                if (displayedImage != value)
                {
                    displayedImage = value;
                    OnPropertyChanged(nameof(DisplayedImage));  
                }
            }
        }



        public TokenModel(ImageModel image)
        {
            Image = image;
            IsFlipped = false;
            IsMatched = false;
        }

        public TokenModel(ImageModel image, bool isMathced, bool isFlipped)
        {
            Image = image;
            IsMatched = isMatched;
            IsFlipped = isFlipped;
        }

        public TokenModel() { }
    }

}
