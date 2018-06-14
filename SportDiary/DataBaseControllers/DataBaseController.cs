using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using Microsoft.Data.Sqlite;

namespace SportDiary.DataBaseControllers
{
    static class DataBaseController
    {
        #region Methods
        public static SqliteConnection CheckDataBaseFile(string connectionString = null)
        {
            string pathToDataBasesFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, "DataBases");
            if (!Directory.Exists(pathToDataBasesFolder))
            {
                Directory.CreateDirectory(pathToDataBasesFolder);
            }

            //if (connectionString == null || !connectionString.Contains(@"Filename=DataBases\"))
            //{
            //    string tmpConnectionString = ApplicationData.Current.LocalSettings.Values["ConnectionString"]?.ToString();
            //    if (tmpConnectionString == null || !tmpConnectionString.Contains(@"Filename=DataBases\"))
            //    {
            //        ApplicationData.Current.LocalSettings.Values["ConnectionString"] = @"Filename=DataBases\SportDiary.db";
            //    }
            //    //connectionString = ApplicationData.Current.LocalSettings.Values["ConnectionString"]?.ToString();
            //}

            CreateDataBase(connectionString);

            return new SqliteConnection(connectionString);
        }

        public static void CreateDataBase(string connectionString)
        {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    List<string> tables = GetTablesList(connection);

                    if (tables.Select(obj => obj).Where(obj => obj == "Dates").Count() < 1)
                    {
                        CreateDatesTable(connection);
                    }

                    if (tables.Select(obj => obj).Where(obj => obj == "Exercises").Count() < 1)
                    {
                        CreateExercisesTable(connection);
                    }

                    if (tables.Select(obj => obj).Where(obj => obj == "NamesExercises").Count() < 1)
                    {
                        CreateNamesExercisesTable(connection);
                    }
                    new SqliteCommand("PRAGMA foreign_keys = 1;", connection).ExecuteNonQuery();
                }
        }

        private static List<string> GetTablesList(SqliteConnection connection)
        {
            List<string> tables = new List<string>();
            SqliteCommand command = new SqliteCommand("SELECT name FROM sqlite_master WHERE TYPE = 'table'", connection);
            using (SqliteDataReader dataReader = command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    tables.Add(dataReader[0].ToString());
                }
            }
            return tables;
        }

        private static void CreateDatesTable(SqliteConnection connection)
        {
            new SqliteCommand("CREATE TABLE Dates (IdDate INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [When] DATE UNIQUE NOT NULL)", connection).ExecuteNonQuery();
        }

        private static void CreateExercisesTable(SqliteConnection connection)
        {
            new SqliteCommand("CREATE TABLE Exercises" +
                " (IdExercise INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, IdDate INTEGER REFERENCES Dates (IdDate) NOT NULL," +
                " Name TEXT (250) NOT NULL, Repetition INTEGER, Approaches INTEGER, Description TEXT (500))", connection).ExecuteNonQuery();
        }

        private static void CreateNamesExercisesTable(SqliteConnection connection)
        {
            new SqliteCommand("CREATE TABLE NamesExercises (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Name STRING UNIQUE NOT NULL)", connection).ExecuteNonQuery(); ;
        }
        #endregion
    }
}
