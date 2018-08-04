using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace SportDiary.ContentDialogs
{
    /// <summary>
    /// Диалог показывающий ошибку
    /// </summary>
    public sealed partial class ErrorDialog : ContentDialog
    {
        public string ErrorString { get; set; }

        /// <param name="errorString">Текст ошибки</param>
        public ErrorDialog(string errorString)
        {
            ErrorString = errorString;
            this.InitializeComponent();
        }

        private void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(ErrorString);
            Clipboard.SetContent(dataPackage);

            //Вывод уведомления
            ToastContent toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = ResourceLoader.GetForCurrentView().GetString("CopiedText")
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = "LargeTile.scale-100.png",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                }
            };
            ToastNotification toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddSeconds(1);
            toast.SuppressPopup = false;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
