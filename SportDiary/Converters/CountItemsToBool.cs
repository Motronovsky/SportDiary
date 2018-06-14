using SportDiary.DataBaseControllers;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SportDiary.Converters
{
    class CountItemsToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if((value as ItemCollection).Count == 0)
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
