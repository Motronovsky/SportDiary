using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SportDiary.Converters
{
    class DateToCultureFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (DateTime.Parse(value.ToString())).ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new Exception();
        }
    }
}
