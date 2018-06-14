using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SportDiary.Views.SettingPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalizationSetting : Page
    {
        List<CultureInfo> cultureInfoList = new List<CultureInfo>();

        public PersonalizationSetting()
        {
            this.InitializeComponent();
            switch (ApplicationData.Current.LocalSettings.Values["CurrentTheme"])
            {
                case "Dark":
                    DarkThemeRadioButton.IsChecked = true;
                    break;
                case "Light":
                    LightThemeRadioButton.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in ApplicationLanguages.Languages)
            {
                CultureInfo cultureInfo = new CultureInfo(item);
                LanguagesList.Items.Add(cultureInfo.DisplayName);
                cultureInfoList.Add(cultureInfo);
            }
            LanguagesList.SelectedIndex = 0;
        }

        private void ThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            switch (radioButton.Tag)
            {
                case "Dark":
                    ApplicationData.Current.LocalSettings.Values["CurrentTheme"] = "Dark";
                    break;
                case "Light":
                    ApplicationData.Current.LocalSettings.Values["CurrentTheme"] = "Light";
                    break;
                default:
                    break;
            }
        }

        private void LanguagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string language = (sender as ComboBox).SelectedItem.ToString();
            CultureInfo languageCultureInfo = cultureInfoList.Where(obj => obj.DisplayName == language).FirstOrDefault();
            ApplicationLanguages.PrimaryLanguageOverride = cultureInfoList.Where(obj => obj.DisplayName == language).FirstOrDefault().Name;
        }
    }
}
