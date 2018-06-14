using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SportDiary.Converters
{
    public class SelectIndexToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(System.Convert.ToInt32(value) >= 0)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
