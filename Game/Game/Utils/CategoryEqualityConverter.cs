using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Game.Models.GameModel;
using System.Windows.Data;

namespace Game.Utils
{
    public class CategoryEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Category selectedCategory && parameter is string categoryString)
            {
                return selectedCategory.ToString() == categoryString;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is string categoryString)
            {
                return Enum.TryParse<Category>(categoryString, out var result) ? result : Category.Koala;
            }
            return Category.Koala;
        }
    }

}
