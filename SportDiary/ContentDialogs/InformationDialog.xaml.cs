using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SportDiary.ContentDialogs
{
    /// <summary>
    /// Диалог показывающий информацию
    /// </summary>
    public sealed partial class InformationDialog : ContentDialog
    {
        /// <param name="title">Заголовок</param>
        /// <param name="mainText">Информация</param>
        public InformationDialog(string title, string mainText)
        {
            this.InitializeComponent();
            Title = title;
            MainText.Text = mainText;
        }

        /// <param name="mainText">Информация</param>
        public InformationDialog(string mainText)
        {
            this.InitializeComponent();
            MainText.Text = mainText;
        }
    }
}
