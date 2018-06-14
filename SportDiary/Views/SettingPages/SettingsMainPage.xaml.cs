using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SportDiary.Views.SettingPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsMainPage : Page
    {
        public SettingsMainPage()
        {
            this.InitializeComponent();
        }

        private void SettingsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Models.Setting settingItem = e.ClickedItem as Models.Setting;
            if (SettingPage.CurrentSourcePageType != settingItem.ContentType)
            {
                SettingPage.Navigate(settingItem.ContentType);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                SystemNavigationManager navigationManager = SystemNavigationManager.GetForCurrentView();
                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                navigationManager.BackRequested += Page_BackRequested;
            }
        }

        private void Page_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                ApplicationView.GetForCurrentView().Title = string.Empty;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                Frame.GoBack();
            }
        }

        private void Page_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Pointer ptr = e.Pointer;
            if (ptr.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint(this);
                if (ptrPt.Properties.IsXButton1Pressed)
                {
                    Page_BackRequested(null, null);
                }
            }
        }
    }
}
