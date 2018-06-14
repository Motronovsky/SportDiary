using Microsoft.Data.Sqlite;
using SportDiary.DataBaseControllers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using SportDiary.Models;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SportDiary.ContentDialogs
{
    public sealed partial class AddEditExerciseDialog : ContentDialog, INotifyPropertyChanged
    {
        #region Fields
        private bool isEdit;
        private bool isFirsStart;
        private Exercise currentExercise;
        private ObservableCollection<string> names = new ObservableCollection<string>();
        private ObservableCollection<Exercise> exercises;

        NamesExercisesDBController namesExercisesModel;

        private Visibility error = Visibility.Collapsed;
        #endregion

        #region Properties
        public Visibility Error
        {
            get { return error; }
            set { error = value; OnPropertyChanged(); }
        }
        #endregion

        #region Constructors        
        public AddEditExerciseDialog(Exercise exercise, ObservableCollection<Exercise> collection, SqliteConnection connection, bool isAdd = false)
        {
            currentExercise = exercise;

            this.InitializeComponent();

            if (isAdd)
            {
                Title = ResourceLoader.GetForCurrentView().GetString("AddingText");
            }
            else
            {
                Title = ResourceLoader.GetForCurrentView().GetString("EditingText");
                NameTextBox.Text = exercise.Name;
                RepetitionTextBox.Text = exercise.Repetition.ToString();
                ApproachesTextBox.Text = exercise.Approaches.ToString();
                DescriptionTextBox.Text = exercise.Description.ToString();
                RepetitionTextBox.TextChanged += TextChanged;
                ApproachesTextBox.TextChanged += TextChanged;
                DescriptionTextBox.TextChanged += TextChanged;
                isEdit = true;
                isFirsStart = true;
            }

            exercises = collection;
            IsPrimaryButtonEnabled = false;
            namesExercisesModel = new NamesExercisesDBController(connection);
            namesExercisesModel.SelectNamesExercises(names);
        }
        #endregion

        #region Event Handlers
        private void OnlyDigitFilter_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            int numKey = Convert.ToInt32(e.Key);

            if ((numKey >= 96 && numKey <= 105) || (numKey >= 48 && numKey <= 57)
                || e.Key == VirtualKey.Back || e.Key == VirtualKey.Delete
                || (numKey >= 37 && numKey <= 40 || e.Key == VirtualKey.Tab))
            {
                return;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            namesExercisesModel.InsertNamesExercises(NameTextBox.Text);

            currentExercise.Name = NameTextBox.Text;

            if (RepetitionTextBox.Text != string.Empty)
            {
                currentExercise.Repetition = Convert.ToInt64(RepetitionTextBox.Text);
            }
            else
            {
                currentExercise.Repetition = null;
            }

            if (ApproachesTextBox.Text != string.Empty)
            {
                currentExercise.Approaches = Convert.ToInt64(ApproachesTextBox.Text);
            }
            else
            {
                currentExercise.Approaches = null;
            }

            currentExercise.Description = DescriptionTextBox.Text;
        }

        private void Name_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (isFirsStart)
            {
                isFirsStart = false;
                return;
            }

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                NameTextBox.ItemsSource = names.Where(i => i.ToLower().Contains(NameTextBox.Text.ToLower())).ToArray();
            }

            if (currentExercise.Name == NameTextBox.Text && isEdit)
            {
                ErrorModeOn();
                return;
            }

            if (sender.Text.Length == 0)
            {
                IsPrimaryButtonEnabled = false;
            }
            else if (exercises.Where(obj => obj.Name == NameTextBox.Text).Count() > 0)
            {
                ErrorModeOn();
            }
            else
            {
                ErrorModeOff();
            }
        }

        private async void TextBox_PasteAsync(object sender, TextControlPasteEventArgs e)
        {
            foreach (var item in await Clipboard.GetContent().GetTextAsync())
            {
                if (char.IsDigit(Convert.ToChar(item)))
                {
                    continue;
                }
                e.Handled = true;
                return;
            }
        }

        private void TextChanged(object sender,TextChangedEventArgs args)
        {
            if (DescriptionTextBox.Text != currentExercise.Description || ApproachesTextBox.Text != currentExercise.Approaches.ToString() || RepetitionTextBox.Text != currentExercise.Repetition.ToString() && isEdit)
            {
                ErrorModeOff();
            }
            else
            {
                ErrorModeOn();
            }
        }

        private void NameTextBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            namesExercisesModel.RemoveNamesExercises(sender.Text);
            sender.Text = "";
        }

        private void RemoveSuggestionButton_Click(object sender, RoutedEventArgs e)
        {
            string suggestionName = (sender as Button).DataContext.ToString();
            namesExercisesModel.RemoveNamesExercises(suggestionName);
            names.Remove(suggestionName);
            //MessageDialog messageDialog = new MessageDialog("Удалено. Закрыть диалог?", "Внимание!");
            //messageDialog.Commands.Add(new UICommand("Да") { Id = 0 });
            //messageDialog.Commands.Add(new UICommand("Нет") { Id = 1 });

            //IUICommand result = await messageDialog.ShowAsync();

            //if (Convert.ToInt32(result.Id) == 0)
            //{
            //    Hide();
            //}
        }
        #endregion

        #region Methods
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ErrorModeOn()
        {
            if (IsPrimaryButtonEnabled)
            {
                IsPrimaryButtonEnabled = !IsPrimaryButtonEnabled;
            }
            if (Error == Visibility.Collapsed)
            {
                Error = Visibility.Visible;
            }
        }

        private void ErrorModeOff()
        {
            if (!IsPrimaryButtonEnabled)
            {
                IsPrimaryButtonEnabled = !IsPrimaryButtonEnabled;
            }
            if (Error == Visibility.Visible)
            {
                Error = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
