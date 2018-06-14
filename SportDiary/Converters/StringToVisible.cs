using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SportDiary.Converters
{
    class StringToVisible : IValueConverter
    {
        public object Visible { get; private set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(Int64.Parse(value.ToString()) > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
