using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace SportDiary.DataBaseControllers
{
    class NamesExercisesDBController
    {
        #region Fields
        private SqliteConnection sqliteConnection;
        #endregion

        #region Constructors
        public NamesExercisesDBController(SqliteConnection connection)
        {
            sqliteConnection = connection;
        } 
        #endregion

        #region Methods
        public void SelectNamesExercises(ObservableCollection<string> collection)
        {
            if (collection.Count > 0)
            {
                collection.Clear();
            }

            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();
                SqliteCommand command = new SqliteCommand("SELECT Name FROM NamesExercises ORDER BY Name", db);

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        collection.Add(dr.GetString(0));
                    }
                }
            }
        }

        public void InsertNamesExercises(string newName)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM NamesExercises WHERE Name = @Name", db);
                command.Parameters.Add(new SqliteParameter("Name", newName));

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    int count = dr.GetInt32(0);

                    if (count > 0)
                    {
                        return;
                    }
                }

                command.CommandText = "INSERT INTO NamesExercises(Name) VALUES(@Name)";
                command.ExecuteNonQuery();
            }
        }

        public void RemoveNamesExercises(string name)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();
                SqliteCommand command = new SqliteCommand("DELETE FROM NamesExercises WHERE Name = @Name", db);
                command.Parameters.Add(new SqliteParameter("Name", name));
                command.ExecuteNonQuery();
            }
        } 
        #endregion
    }
}
