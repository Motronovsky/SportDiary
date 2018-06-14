using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SportDiary.ContentDialogs;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using SportDiary.DataBaseControllers;
using MVVMLib;
using System.IO;
using Windows.UI.Popups;

namespace SportDiary.ViewModels
{
    public class SelectDataBaseViewModel : NotifyPropertyChanged, IDisposable
    {
        #region Fields
        //private StorageFile dataBasesListSelectedItem;
        private ObservableCollection<StorageFile> dataBasesItems = new ObservableCollection<StorageFile>();
        private Common.FileManagerDataBases FileManagerDB = new Common.FileManagerDataBases();
        private Visibility emtyTextVisibility = Visibility.Collapsed;
        private bool waitMode;
        private bool disableClick;
        #endregion

        #region Properties
        //public StorageFile DataBasesListSelectedItem
        //{
        //    get { return dataBasesListSelectedItem; }
        //    set { dataBasesListSelectedItem = value; OnPropertyChanged(); }
        //}

        public ObservableCollection<StorageFile> DataBasesItems
        {
            get { return dataBasesItems; }
            set { dataBasesItems = value; OnPropertyChanged(); }
        }

        public Visibility EmtyTextVisibility
        {
            get { return emtyTextVisibility; }
            set
            {
                if (emtyTextVisibility != value)
                {
                    emtyTextVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool WaitMode
        {
            get { return waitMode; }
            set { waitMode = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        #region DataBaseCommands
        private Command addDataBaseClick;

        public Command AddDataBaseClick
        {
            get
            {
                return addDataBaseClick ?? (addDataBaseClick = new Command(async obj =>
                {
                    if (!disableClick)
                    {
                        disableClick = true;
                        try
                        {
                            AddRenameDBDialod addDBDialod = new AddRenameDBDialod(DataBasesItems);
                            addDBDialod.Closing += delegate { disableClick = false; };
                            if (await addDBDialod.ShowAsync() == ContentDialogResult.None)
                            {
                                return;
                            }
                            DataBaseController.CreateDataBase(new StringBuilder().Append("Filename=DataBases\\").Append(addDBDialod.NameDataBase).Append(".db").ToString());

                            disableClick = false;
                            RefreshClick.Execute(null);

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

        private Command renameDataBaseClick;

        public Command RenameDataBaseClick
        {
            get
            {
                return renameDataBaseClick ?? (renameDataBaseClick = new Command(async obj =>
                {
                    try
                    {
                        StorageFile dataBaseFile = (obj as MenuFlyoutItem).DataContext as StorageFile;
                        AddRenameDBDialod renameDBDialod = new AddRenameDBDialod(DataBasesItems, dataBaseFile.DisplayName);
                        int index = DataBasesItems.IndexOf(dataBaseFile);

                        if (await renameDBDialod.ShowAsync() == ContentDialogResult.None)
                        {
                            return;
                        }

                        try
                        {
                            DataBasesItems.Remove(dataBaseFile);
                            await dataBaseFile.RenameAsync(renameDBDialod.NameDataBase + ".db");
                            DataBasesItems.Insert(index, dataBaseFile);
                        }
                        catch (Exception)
                        {
                            DataBasesItems.Insert(index, dataBaseFile);
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }


        private Command deleteDataBaseClick;

        public Command DeleteDataBaseClick
        {
            get
            {
                return deleteDataBaseClick ?? (deleteDataBaseClick = new Command(async obj =>
                {
                    try
                    {
                        StorageFile dataBaseFile = (obj as MenuFlyoutItem).DataContext as StorageFile;
                        DeleteDialog deleteDialog = new DeleteDialog(dataBaseFile);
                        int index = DataBasesItems.IndexOf(dataBaseFile);

                        if (await deleteDialog.ShowAsync() == ContentDialogResult.None)
                        {
                            return;
                        }

                        DataBasesItems.Remove(dataBaseFile);
                        try
                        {
                            await dataBaseFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                        catch (Exception)
                        {
                            DataBasesItems.Insert(index, dataBaseFile);
                            throw;
                        }
                        CheckEmtyDataBasesList();
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }

        private Command exportDataBaseClick;

        public Command ExportDataBaseClick
        {
            get
            {
                return exportDataBaseClick ?? (exportDataBaseClick = new Command(async obj =>
                {
                    if (!disableClick)
                    {
                        disableClick = true;
                        try
                        {
                            StorageFile file = (obj as MenuFlyoutItem).DataContext as StorageFile;
                            FileSavePicker savePicker = new FileSavePicker();
                            savePicker.FileTypeChoices.Add("Data Base File", new List<string>() { ".db" });
                            savePicker.DefaultFileExtension = ".db";
                            savePicker.SuggestedFileName = file.DisplayName + " " + DateTime.Now;
                            StorageFile saveFile = await savePicker.PickSaveFileAsync();

                            if (saveFile == null)
                            {
                                return;
                            }

                            WaitMode = true;

                            try
                            {
                                await file.CopyAndReplaceAsync(saveFile);
                            }
                            catch (Exception)
                            {
                                await saveFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                throw;
                            }

                            await new InformationDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ExportButton/Text"), Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CompleteExportText")).ShowAsync();
                        }
                        catch (Exception ex)
                        {
                            await new ErrorDialog(ex.Message).ShowAsync();
                        }
                        finally
                        {
                            WaitMode = false;
                            disableClick = false;
                        }
                    }
                }));
            }
        }

        private Command exportAllDataBasesFilesClick;

        public Command ExportAllDataBasesFilesClick
        {
            get
            {
                return exportAllDataBasesFilesClick ?? (exportAllDataBasesFilesClick = new Command(async obj =>
                {
                    if (!disableClick)
                    {
                        disableClick = true;
                        try
                        {
                            if (DataBasesItems.Count == 0)
                            {
                                await new InformationDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("EmptyDBList")).ShowAsync();
                                return;
                            }

                            FolderPicker folderPicker = new FolderPicker();
                            folderPicker.FileTypeFilter.Add("*");
                            StorageFolder exportFolder = await folderPicker.PickSingleFolderAsync();
                            StringBuilder result = new StringBuilder();
                            const string format = "dd-MM-yyyy";

                            if (exportFolder == null)
                            {
                                return;
                            }

                            WaitMode = true;
                            exportFolder = await exportFolder.CreateFolderAsync(DateTime.Now.ToString(format), CreationCollisionOption.GenerateUniqueName);

                            foreach (var item in DataBasesItems)
                            {
                                result.Append(item.DisplayName).Append(" - ");
                                try
                                {
                                    var a = await item.CopyAsync(exportFolder);
                                    result.Append(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("SuccessfullyText"));
                                }
                                catch (Exception)
                                {
                                    result.Append(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("FailedText"));
                                }
                                result.AppendLine();
                            }

                            StorageFile logFile = await ToLogAsync("ExportLog.txt", result.ToString());

                            await new LogInfoDialog(logFile, ContentDialogs.LogDialogEx.Mode.Export).ShowAsync();
                        }
                        catch (Exception ex)
                        {
                            await new ErrorDialog(ex.Message).ShowAsync();
                        }
                        finally
                        {
                            WaitMode = false;
                            disableClick = false;
                        }
                    }
                }));
            }
        }


        private Command importDataBaseFileClick;

        public Command ImportDataBaseFileClick
        {
            get
            {
                return importDataBaseFileClick ?? (importDataBaseFileClick = new Command(async obj =>
                {
                    if (!disableClick)
                    {
                        disableClick = true;

                        try
                        {
                            FileOpenPicker fileOpenPicker = new FileOpenPicker();
                            fileOpenPicker.FileTypeFilter.Add(".db");
                            IReadOnlyList<StorageFile> files = await fileOpenPicker.PickMultipleFilesAsync();
                            StringBuilder result = new StringBuilder();

                            if (files.Count == 0)
                            {
                                return;
                            }

                            WaitMode = true;
                            foreach (var file in files)
                            {
                                try
                                {
                                    StorageFile newFile = await file.CopyAsync(await FileManagerDB.GetDataBaseFolderAsync(), file.Name, NameCollisionOption.GenerateUniqueName);
                                    DataBasesItems.Add(newFile);
                                    result.Append(newFile.DisplayName).Append(" - ").Append(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("SuccessfullyText")).AppendLine();
                                }
                                catch (Exception ex)
                                {
                                    result.Append(file.DisplayName).Append(" - ").Append(ex.Message).AppendLine();
                                }
                            }
                            CheckEmtyDataBasesList();

                            StorageFile logFile = await ToLogAsync("ImportLog.txt", result.ToString());
                            await new LogInfoDialog(logFile, ContentDialogs.LogDialogEx.Mode.Import).ShowAsync();
                            //await new InformationDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ImportText"), ResourceLoader.GetForCurrentView().GetString("CompleteImportText")).ShowAsync();
                        }
                        catch (Exception ex)
                        {
                            await new ErrorDialog(ex.Message).ShowAsync();
                        }
                        finally
                        {
                            WaitMode = false;
                            disableClick = false;
                        }
                    }
                }));
            }
        }
        #endregion

        #region OtherCommands
        private Command onLoaded;

        public Command OnLoaded
        {
            get
            {
                return onLoaded ?? (onLoaded = new Command(obj =>
                {
                    RefreshClick.Execute(null);
                }));
            }
        }


        private Command dataBaseItemClick;

        public Command DataBaseItemClick
        {
            get
            {
                return dataBaseItemClick ?? (dataBaseItemClick = new Command(async obj =>
                {
                    try
                    {
                        string connectionString = @"Filename=DataBases\" + ((obj as ItemClickEventArgs).ClickedItem as StorageFile).Name;
                        MainPageViewModel.SqliteConnectionTemp = DataBaseController.CheckDataBaseFile(connectionString);

                        Frame rootFrame = Window.Current.Content as Frame;
                        rootFrame.Navigate(typeof(Views.MainPageView));
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }

        private Command refreshClick;

        public Command RefreshClick
        {
            get
            {
                return refreshClick ?? (refreshClick = new Command(async obj =>
                {
                    if (!disableClick)
                    {
                        disableClick = true;
                        try
                        {
                            await FileManagerDB.SelectDataBasesAsync(DataBasesItems);
                            CheckEmtyDataBasesList();
                        }
                        catch (Exception ex)
                        {
                            await new ErrorDialog(ex.Message).ShowAsync();
                        }
                        finally
                        {
                            disableClick = false;
                        }
                    }
                }));
            }
        }

        private Command settingsClick;

        public Command SettingsClick
        {
            get
            {
                return settingsClick ?? (settingsClick = new Command(async obj =>
                {
                    try
                    {
                        Frame rootFrame = Window.Current.Content as Frame;
                        rootFrame.Navigate(typeof(Views.SettingPages.SettingsMainPage));
                    }
                    catch (Exception ex)
                    {
                        await new ErrorDialog(ex.Message).ShowAsync();
                    }
                }));
            }
        }

        private Command openInNewWindowClick;

        public Command OpenInNewWindowClick
        {
            get
            {
                return openInNewWindowClick ?? (openInNewWindowClick = new Command(async obj =>
                {
                    MainPageViewModel.SqliteConnectionTemp = new Microsoft.Data.Sqlite.SqliteConnection(@"Filename=DataBases\" + ((obj as MenuFlyoutItem).DataContext as StorageFile).Name);

                    CoreApplicationView newView = CoreApplication.CreateNewView();

                    int newViewId = 0;
                    await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Frame frame = new Frame();
                        frame.Navigate(typeof(Views.MainPageView));
                        frame.Loaded += (s, a) => { System.Diagnostics.Debug.WriteLine("Loaded"); };
                        Window.Current.Content = frame;
                        var v = frame.Content as Views.MainPageView;
                        v.Unloaded += (s, a) => { System.Diagnostics.Debug.WriteLine("Unloaded"); };
                        Window.Current.Activate();

                        newViewId = ApplicationView.GetForCurrentView().Id;

                    });
                    await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
                }));
            }
        }
        #endregion
        #endregion

        #region Methods
        private void CheckEmtyDataBasesList()
        {
            if (DataBasesItems.Count == 0)
            {
                EmtyTextVisibility = Visibility.Visible;
            }
            else
            {
                EmtyTextVisibility = Visibility.Collapsed;
            }
        }

        public void Dispose()
        {
            //dataBasesItems = null;
            FileManagerDB = null;

        }

        private async System.Threading.Tasks.Task<StorageFile> ToLogAsync(string nameLogFile, string textLogFile)
        {
            StorageFile log = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(nameLogFile, CreationCollisionOption.OpenIfExists);
            await File.WriteAllTextAsync(log.Path, textLogFile);
            return log;
        }
        #endregion
    }
}