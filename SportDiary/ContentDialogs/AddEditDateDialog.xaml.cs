using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using SportDiary.DataBaseControllers;
using SportDiary.Models;

namespace SportDiary.ContentDialogs
{
    public sealed partial class AddEditDateDialog : ContentDialog
    {
        #region Fields
        private Date editDate;
        private DatesDBController datesDBController;
        #endregion

        #region Properties
        public bool OkButtonEnabled
        {
            get { return IsPrimaryButtonEnabled; }
            set { IsPrimaryButtonEnabled = value;}
        }

        public Visibility ErrorTextVisibility
        {
            get { return ErrorDateTextBlock.Visibility; }
            set { ErrorDateTextBlock.Visibility = value;}
        }
        #endregion

        #region Constructors
        public AddEditDateDialog(Date date, DatesDBController dBController, bool isAdd = false)
        {
            editDate = date;
            datesDBController = dBController;
            this.InitializeComponent();

            if (isAdd)
            {
                Title = ResourceLoader.GetForCurrentView().GetString("AddingText");
            }
            else
            {
                Title = ResourceLoader.GetForCurrentView().GetString("EditingText");
            }

            NewDate.Date = editDate.When;
            CheckDate();
        }
        #endregion

        #region Methods
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            editDate.When = DateTime.Parse(NewDate.Date.ToString());
        }

        private void NewDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            CheckDate();
        }

        private void CheckDate()
        {
            if (datesDBController.GetCountDates(Convert.ToDateTime(NewDate.Date.ToString())) > 0)
            {
                ErrorTextVisibility = Visibility.Visible;
                OkButtonEnabled = false;
            }
            else
            {
                ErrorTextVisibility = Visibility.Collapsed;
                OkButtonEnabled = true;
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //} 
        #endregion
    }
}
