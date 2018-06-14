using Windows.UI.Xaml.Controls;
using SportDiary.DataBaseControllers;
using Windows.Storage;
using Windows.ApplicationModel.Resources;
using SportDiary.Models;

namespace SportDiary.ContentDialogs
{
    public sealed partial class DeleteDialog : ContentDialog
    {
        public string TextDialog { get; set; }

        public DeleteDialog(Date date)
        {
            TextDialog = ResourceLoader.GetForCurrentView().GetString("DeleteDateQestion") + '\n' + date.When.ToString("dd.MM.yyyy") + "?";
            this.InitializeComponent();
        }

        public DeleteDialog(Exercise exercise)
        {
            TextDialog = ResourceLoader.GetForCurrentView().GetString("DeleteExerciseQestion") + '\n' + exercise.Name + "?";
            this.InitializeComponent();
        }

        public DeleteDialog(StorageFile dataBaseFile)
        {
            TextDialog = ResourceLoader.GetForCurrentView().GetString("DeleteDataBaseQestion") + '\n' + dataBaseFile.DisplayName + "?";
            this.InitializeComponent();
        }
    }
}
