using System;
using System.Collections.ObjectModel;
using System.Linq;
using SportDiary.DataBaseControllers;
using SportDiary.ContentDialogs;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Microsoft.Data.Sqlite;
using MVVMLib;
using SportDiary.Models;
using System.Collections.Generic;
using SportDiary.Common;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Windows.UI.Core;


namespace SportDiary.ViewModels
{
    public class MainPageViewModel : NotifyPropertyChanged, IDisposable
    {
        #region Fields
        private DatesDBController datesDBController;
        private ExercisesDBController exercisesDBController;
        private int dateSelectedIndex = -1;
        private Visibility emptyTextVisibility = Visibility.Collapsed;
        private int emptyTextGridSpan;
        private int emptyTextGridColumn;
        private bool disableClick;
        private static SqliteConnection sqliteConnectionTemp;
        #endregion

        #region Properties
        public static SqliteConnection SqliteConnectionTemp
        {
            private get => sqliteConnectionTemp;
            set
            {
                if (sqliteConnectionTemp?.DataSource != value.DataSource)
                {
                    sqliteConnectionTemp = value;
                }
            }
        }

        public SqliteConnection Connection { get; set; }

        public ObservableCollection<Date> DatesCollection { get; private set; } = new ObservableCollection<Date>();

        public ObservableCollection<Exercise> ExercisesCollection { get; private set; } = new ObservableCollection<Exercise>();

        public Visibility EmptyTextVisibility
        {
            get { return emptyTextVisibility; }
            set
            {
                if (emptyTextVisibility != value)
                {
                    emptyTextVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public int DateSelectedIndex
        {
            get { return dateSelectedIndex; }
            set
            {
                dateSelectedIndex = value;
                OnPropertyChanged();
            }
        }

        public int EmptyTextGridSpan
        {
            get { return emptyTextGridSpan; }
            set
            {
                if (emptyTextGridSpan != value)
                {
                    emptyTextGridSpan = value;
                    OnPropertyChanged();
                }
            }
        }

        public int EmptyTextGridColumn
        {
            get { return emptyTextGridColumn; }
            set
            {
                if (emptyTextGridColumn != value)
                {
                    emptyTextGridColumn = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructors
        public MainPageViewModel()
        {
            Connection = SqliteConnectionTemp;
            datesDBController = new DatesDBController(Connection);
            exercisesDBController = new ExercisesDBController(Connection);
        }
        #endregion

        #region Commands
        #region DateCommands
        private Command addDateClick;

        public Command AddDateClick
        {
            get
            {
                return addDateClick ?? (addDateClick = new Command(async obj =>
                {
                    if (!disableClick)
                    {
                        disableClick = true;
                        try
                        {
                            Date newDate = new Date(0, DateTime.Today);

                            AddEditDateDialog editDateDialog = new AddEditDateDialog(newDate, dBController: datesDBController, isAdd: true);
                            editDateDialog.Closing += delegate { disableClick = false; };

                            if (await editDateDialog.ShowAsync() != ContentDialogResult.None)
                            {
                                datesDBController.InsertDate(newDate);
                                DatesCollection.Insert(0, newDate);
                                CheckEmptyDatesList();

                                if (DateSelectedIndex != -1)
                                {
                                    CheckEmptyExercisesList();
                                }

                                DateSelectedIndex = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            await new ErrorDialog(ex.Message).ShowAsync();
                        }
                        finally
                        {
                            if (disableClick)
                            {
                                disableClick = false;
                            }
                        }
                    }
                }));
            }
        }

        private Command removeDateClick;

        public Command RemoveDateClick
        {
            get
            {
                return removeDateClick ?? (removeDateClick = new Command(async IdDate =>
                {
                    try
                    {
                        Date removeDate = DatesCollection.Where(item => item.IdDate == Convert.ToInt64(IdDate)).FirstOrDefault();

                        DeleteDialog deleteDialog = new DeleteDialog(removeDate);

                        if (await deleteDialog.ShowAsync() != ContentDialogResult.None)
                        {
                            datesDBController.RemoveDate(removeDate.IdDate);
                            DatesCollection.Remove(removeDate);

                            Date selectedDate = DateSelectedIndex == -1 ? null : DatesCollection[DateSelectedIndex];

                            if (removeDate != selectedDate)
                            {
                                CheckEmptyDatesList();
                            }
                            else
                            {
                                EmptyTextVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }

                }));
            }
        }

        private Command editDateClick;

        public Command EditDateClick
        {
            get
            {
                return editDateClick ?? (editDateClick = new Command(async IdDate =>
                {
                    try
                    {
                        Date selectedDate = DatesCollection.Where(item => item.IdDate == Convert.ToInt32(IdDate)).FirstOrDefault();

                        AddEditDateDialog editDateDialog = new AddEditDateDialog(selectedDate, datesDBController);

                        if (await editDateDialog.ShowAsync() != ContentDialogResult.None)
                        {
                            datesDBController.EditDate(selectedDate);
                        }
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }
        #endregion

        #region ExerciseCommands
        private Command addExerciseClick;

        public Command AddExerciseClick
        {
            get
            {
                return addExerciseClick ?? (addExerciseClick = new Command(async obj =>
                {
                    if (!disableClick)
                    {
                        disableClick = true;
                        try
                        {
                            Date selectedDate = ((obj as ListView).SelectedItem as Date);

                            if (DatesCollection.Count == 0)
                            {
                                disableClick = false;
                                AddDateClick.Execute(null);
                                return;
                            }

                            if (selectedDate == null)
                            {
                                selectedDate = ((obj as ListView).Items[0] as Date);
                                DateSelectedIndex = 0;
                            }

                            Exercise newExercise = new Exercise() { IdDate = selectedDate.IdDate };

                            AddEditExerciseDialog addExerciseDialog = new AddEditExerciseDialog(newExercise, ExercisesCollection, Connection, true);
                            addExerciseDialog.Closing += delegate { disableClick = false; };

                            if (await addExerciseDialog.ShowAsync() != ContentDialogResult.None)
                            {
                                exercisesDBController.InsertExercise(newExercise);
                                ExercisesCollection.Add(newExercise);
                            }
                            CheckEmptyExercisesList();
                        }
                        catch (Exception ex)
                        {
                            await new ErrorDialog(ex.Message).ShowAsync();
                        }
                        finally
                        {
                            if (disableClick)
                            {
                                disableClick = false;
                            }
                        }
                    }
                }));
            }
        }

        private Command editExerciseClick;

        public Command EditExerciseClick
        {
            get
            {
                return editExerciseClick ?? (editExerciseClick = new Command(async idExericise =>
                {
                    try
                    {
                        Exercise Exercise = ExercisesCollection.Where(exercise => exercise.IdExercise == Convert.ToInt64(idExericise)).FirstOrDefault();

                        if (Exercise == null)
                        {
                            throw new NullReferenceException();
                        }

                        AddEditExerciseDialog editExerciseDialog = new AddEditExerciseDialog(Exercise, ExercisesCollection, Connection, false);
                        ContentDialogResult result = await editExerciseDialog.ShowAsync();

                        if (result != ContentDialogResult.None)
                        {
                            exercisesDBController.EditExercise(Exercise);
                        }
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }


        private Command removeExerciseClick;

        public Command RemoveExerciseClick
        {
            get
            {
                return removeExerciseClick ?? (removeExerciseClick = new Command(async idExercise =>
                {
                    try
                    {
                        long idExerciseLong = Convert.ToInt64(idExercise);
                        Exercise selectExercise = ExercisesCollection.Where(item => item.IdExercise == idExerciseLong).FirstOrDefault();

                        DeleteDialog deleteDialog = new DeleteDialog(selectExercise);
                        ContentDialogResult result = await deleteDialog.ShowAsync();
                        if (result == ContentDialogResult.None)
                        {
                            return;
                        }

                        exercisesDBController.RemoveExercise(idExerciseLong);
                        ExercisesCollection.Remove(selectExercise);
                        CheckEmptyExercisesList();
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }
        #endregion

        #region OtherCommands
        private Command refreshClick;

        public Command RefreshClick
        {
            get
            {
                return refreshClick ?? (refreshClick = new Command(async obj =>
                {
                    try
                    {
                        datesDBController.SelectDatesAsync(DatesCollection);
                        CheckEmptyDatesList();
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }

        private Command switchingDate;

        public Command SwitchingDate
        {
            get
            {
                return switchingDate ?? (switchingDate = new Command(async obj =>
                {
                    try
                    {
                        if (obj == null)
                        {
                            ExercisesCollection.Clear();
                            return;
                        }

                        exercisesDBController.SelectExercises((obj as Date).IdDate, ExercisesCollection);
                        CheckEmptyExercisesList();
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }

        private Command onLoaded;

        public Command OnLoaded
        {
            get
            {
                return onLoaded ?? (onLoaded = new Command(obj =>
                {
                    ApplicationView.GetForCurrentView().Title = SqliteConnectionTemp.ConnectionString.Replace("Filename=DataBases\\", string.Empty);
                    RefreshClick.Execute(null);
                }));
            }
        }
        #endregion

        #region ClipboardCommands
        private Command copyExercisesClick;

        public Command CopyExercisesClick
        {
            get
            {
                return copyExercisesClick ?? (copyExercisesClick = new Command(obj =>
                {
                    var a = (obj as ListView).SelectedItems.OfType<Exercise>().ToList();

                    if (a.Count == 0)
                    {
                        return;
                    }

                    if (a.Count == 1)
                    {
                        Clipboard.SerializeAsync(a[0]);
                    }
                    else
                    {
                        Clipboard.SerializeAsync(a);
                    }
                }));
            }
        }

        private Command pasteExercisesClick;

        public Command PasteExercisesClick
        {
            get
            {
                return pasteExercisesClick ?? (pasteExercisesClick = new Command(async obj =>
                {

                    Date date = (obj as ListView).SelectedItem as Date;
                    if (date == null)
                    {
                        return;
                    }

                    object buf = await Clipboard.DeserializeAsync();

                    Type type = buf?.GetType();

                    if (type == typeof(Exercise))
                    {
                        Exercise exercise = buf as Exercise;
                        if (ExercisesCollection.Where(x => x.Name == exercise.Name).Count() == 0)
                        {
                            exercise.IdDate = date.IdDate;
                            ExercisesCollection.Add(exercise);
                            exercisesDBController.InsertExercise(exercise);
                        }
                    }
                    else if (type == typeof(List<Exercise>))
                    {
                        foreach (var item in buf as List<Exercise>)
                        {
                            if (ExercisesCollection.Where(x => x.Name == item.Name).Count() == 0)
                            {
                                item.IdDate = date.IdDate;
                                ExercisesCollection.Add(item);
                                exercisesDBController.InsertExercise(item);
                            }
                        }
                    }

                    CheckEmptyExercisesList();
                }));
            }
        }


        //private Command checkCopy;

        //public Command CheckCopy
        //{
        //    get
        //    {
        //        return checkCopy ?? (checkCopy = new Command(obj =>
        //        {
        //            obj = false;
        //        }));
        //    }
        //}

        //private Command checkPaste;

        //public Command CheckPaste
        //{
        //    get
        //    {
        //        return checkPaste ?? (checkPaste = new Command(obj =>
        //        {

        //        }));
        //    }
        //}
        #endregion
        #endregion

        #region Methods
        private void CheckEmptyExercisesList()
        {
            if (ExercisesCollection.Count == 0)
            {
                EmptyTextVisibility = Visibility.Visible;
                EmptyTextGridSpan = 1;
                EmptyTextGridColumn = 1;
            }
            else
            {
                EmptyTextVisibility = Visibility.Collapsed;
            }
        }

        private void CheckEmptyDatesList()
        {
            if (DatesCollection.Count == 0)
            {
                EmptyTextVisibility = Visibility.Visible;
                EmptyTextGridColumn = 0;
                EmptyTextGridSpan = 2;
            }
            else
            {
                EmptyTextVisibility = Visibility.Collapsed;
            }
        }

        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
            Connection = null;
            DatesCollection = null;
            ExercisesCollection = null;
            datesDBController = null;
            exercisesDBController = null;
        }
        #endregion
    }
}