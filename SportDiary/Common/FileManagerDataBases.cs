using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace SportDiary.Common
{
    class FileManagerDataBases
    {
        #region Methods
        public async Task SelectDataBasesAsync(ObservableCollection<StorageFile> dataBaseCollection)
        {
            if(dataBaseCollection.Count > 0)
            {
                dataBaseCollection.Clear();
            }

            StorageFolder dataBasesFolder = await GetDataBaseFolderAsync();
            IReadOnlyList<StorageFile> dataBasesFiles = await dataBasesFolder.GetFilesAsync();

            foreach (var item in dataBasesFiles)
            {
                if(item.FileType == ".db")
                {
                    dataBaseCollection.Add(item);
                }
            }
        }

        public async Task<bool> ExistDataBaseAsync(string nameDB)
        {
            try
            {
                StorageFolder databasesFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("DataBases");
                StorageFile file = await databasesFolder.GetFileAsync(nameDB + ".db");
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
            return true;
        }

        public async Task<StorageFolder> GetDataBaseFolderAsync()
        {
            StorageFolder folder;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("DataBases");
            }
            catch (System.IO.FileNotFoundException)
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("DataBases");
            }
            return folder;
        }
        #endregion
    }
}
