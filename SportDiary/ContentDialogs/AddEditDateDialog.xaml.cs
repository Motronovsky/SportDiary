using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using SportDiary.DataBaseControllers;
using SportDiary.Models;

namespace SportDiary.ContentDialogs
{
    /// <summary>
    /// Диалоговое окно для добавления или редактирования поля таблицы Dates
    /// </summary>
    public sealed partial class AddEditDateDialog : ContentDialog
    {
        #region Fields

        /// <summary>
        /// Ссылка на редактируемую сущность
        /// </summary>
        private Date editDate;

        /// <summary>
        /// Ссылка на контроллер таблицы Dates
        /// </summary>
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
        /// <param name="date">Ссылка на редактируемую сущность</param>
        /// <param name="dBController">Ссылка на контроллер таблицы Dates</param>
        /// <param name="isAdd">true = добавление, false = редактирование</param>
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

        #region Event Hundlers
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            editDate.When = DateTime.Parse(NewDate.Date.ToString());
        }

        private void NewDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            CheckDate();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Проверка введенной даты на доступность
        /// </summary>
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
        #endregion
    }
}
