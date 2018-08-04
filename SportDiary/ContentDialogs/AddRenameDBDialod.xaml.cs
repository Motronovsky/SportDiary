using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SportDiary.ContentDialogs
{
    /// <summary>
    /// Диалог переименования файла БД
    /// </summary>
    public sealed partial class AddRenameDBDialod : ContentDialog
    {
        #region Fields
        //private readonly string currentName;

        /// <summary>
        /// Коллекция файлов БД
        /// </summary>
        private ObservableCollection<StorageFile> dataBaseCollection;
        #endregion

        #region Properties
        public string NameDataBase
        {
            get { return NameDB_TextBox.Text; }
            set { NameDB_TextBox.Text = value; }
        }
        #endregion

        #region Constructors
        public AddRenameDBDialod(ObservableCollection<StorageFile> dataBaseFiles, string nameDB = null)
        {
            dataBaseCollection = dataBaseFiles;
            this.InitializeComponent();
            if (nameDB == null)
            {
                Title = ResourceLoader.GetForCurrentView().GetString("AddingText");
            }
            else
            {
                Title = ResourceLoader.GetForCurrentView().GetString("RenameText");
                NameDataBase = nameDB;
                NameDB_TextBox.Focus(FocusState.Keyboard);
                NameDB_TextBox.SelectAll();
            }
            CheckNameDataBase();
        }
        #endregion

        #region Event Hundlers
        private void NameDB_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckNameDataBase();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Проверка имени БД
        /// </summary>
        private void CheckNameDataBase()
        {
            if (NameDataBase == string.Empty)
            {
                if (IsPrimaryButtonEnabled) { IsPrimaryButtonEnabled = false; }
                ErrorText.Visibility = Visibility.Collapsed;
            }

            else if (dataBaseCollection.Where(obj => obj.DisplayName == NameDataBase).Count() > 0)
            {
                if (IsPrimaryButtonEnabled) { IsPrimaryButtonEnabled = false; }
                ErrorText.Visibility = Visibility.Visible;
            }
            else
            {
                if (!IsPrimaryButtonEnabled) { IsPrimaryButtonEnabled = true; }
                ErrorText.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}