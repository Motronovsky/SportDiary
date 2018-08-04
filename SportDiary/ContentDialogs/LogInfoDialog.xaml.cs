using SportDiary.ContentDialogs.LogDialogEx;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SportDiary.ContentDialogs
{
    /// <summary>
    /// Диалог показывающий информацию с отчетом
    /// </summary>
    public sealed partial class LogInfoDialog : ContentDialog
    {
        StorageFile logFile;

        /// <param name="file">Файл отчета</param>
        /// <param name="mode">Экспорт, импорт</param>
        public LogInfoDialog(StorageFile file, Mode mode)
        {
            this.InitializeComponent();
            logFile = file;

            switch (mode)
            {
                case Mode.Export:
                    Title = ResourceLoader.GetForCurrentView().GetString("ExportText");
                    InfoTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("CompleteExportText");
                    break;
                case Mode.Import:
                    Title = ResourceLoader.GetForCurrentView().GetString("ImportText");
                    InfoTextBlock.Text = ResourceLoader.GetForCurrentView().GetString("CompleteImportText");
                    break;
                default:
                    break;
            }
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFileAsync(logFile);
        }
    }

    namespace LogDialogEx
    {
        public enum Mode
        {
            Export, Import
        }
    }
}
