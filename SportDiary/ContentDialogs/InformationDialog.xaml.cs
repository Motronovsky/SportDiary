using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SportDiary.ContentDialogs
{
    public sealed partial class InformationDialog : ContentDialog
    {
        public InformationDialog(string title, string mainText)
        {
            this.InitializeComponent();
            Title = title;
            MainText.Text = mainText;
        }

        public InformationDialog(string mainText)
        {
            this.InitializeComponent();
            MainText.Text = mainText;
        }
    }
}
