using SportDiary.Common;
using System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SportDiary.Views
{
    public sealed partial class MainPageView : Page
    {
        #region Properties
        public ViewModels.MainPageViewModel ViewModel
        {
            get { return DataContext as ViewModels.MainPageViewModel; }
        } 
        #endregion

        #region Constructors
        public MainPageView()
        {
            this.InitializeComponent();
        }
        #endregion

        #region EventHundlers
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                SystemNavigationManager navigationManager = SystemNavigationManager.GetForCurrentView();
                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                navigationManager.BackRequested += Page_BackRequested;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SystemNavigationManager navigationManager = SystemNavigationManager.GetForCurrentView();
            navigationManager.BackRequested -= Page_BackRequested;
            GC.Collect();
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
            Windows.UI.Xaml.Input.Pointer ptr = e.Pointer;
            if (ptr.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint(this);
                if (ptrPt.Properties.IsXButton1Pressed)
                {
                    Page_BackRequested(null, null);
                }
            }
        }

        private async void CommandBar_Opening(object sender, object e)
        {
            CommandBar commandBar = sender as CommandBar;

            AppBarButton appBarButton = commandBar.SecondaryCommands[0] as AppBarButton;

            if (ExercisesList.SelectedItems.Count > 0)
            {
                appBarButton.IsEnabled = true;
            }
            else
            {
                appBarButton.IsEnabled = false;
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => 
            {
                appBarButton = commandBar.SecondaryCommands[1] as AppBarButton;
                object tmp = await Clipboard.DeserializeAsync();

                if (tmp != null && DatesList.SelectedItem != null)
                {
                    appBarButton.IsEnabled = true;
                }
                else
                {
                    appBarButton.IsEnabled = false;
                }
            });
        }
        #endregion
    }
}
