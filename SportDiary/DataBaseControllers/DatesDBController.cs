using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using SportDiary.Models;
using System.Threading.Tasks;

namespace SportDiary.DataBaseControllers
{
    public class DatesDBController
    {
        #region Fields
        private SqliteConnection sqliteConnection;
        #endregion

        #region Constructors
        public DatesDBController(SqliteConnection connection)
        {
            sqliteConnection = connection;
        }
        #endregion

        #region Methods
        public async void SelectDatesAsync(ObservableCollection<Date> datesCollection)
        {
            if (datesCollection.Count > 0)
            {
                datesCollection.Clear();
            }

            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Dates ORDER BY [When] DESC", db);

                using (SqliteDataReader dr = await command.ExecuteReaderAsync())
                {
                    while (dr.Read())
                    {
                        datesCollection.Add(new Date(dr.GetInt64(0), dr.GetDateTime(1)));
                    }
                }
            }
        }

        public void InsertDate(Date newDate)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("INSERT INTO Dates ([When]) VALUES(@nDate)", db);
                SqliteParameter parameter = new SqliteParameter("nDate", newDate.When.ToString("yyyy-MM-dd"));
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();

                command.CommandText = "SELECT IdDate FROM Dates WHERE [When] = @nDate";

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    long IdDate = dr.GetInt64(0);
                    newDate.IdDate = IdDate;
                }
            }
        }

        public void EditDate(Date date)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();
                SqliteCommand command = new SqliteCommand("UPDATE Dates SET [When] = @NewDate WHERE IdDate = @Id", db);
                command.Parameters.Add(new SqliteParameter("NewDate", date.When.ToString("yyyy-MM-dd")));
                command.Parameters.Add(new SqliteParameter("Id", date.IdDate));
                command.ExecuteNonQuery();
            }
        }

        public void RemoveDate(long idDate)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();

                SqliteParameter parameter = new SqliteParameter("Id", idDate);
                SqliteCommand command = new SqliteCommand("DELETE FROM Exercises WHERE IdDate = @Id", db);
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Dates WHERE IdDate = @Id";
                command.ExecuteNonQuery();
            }
        }

        public int GetCountDates(DateTime date)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();
                SqliteCommand sqliteCommand = new SqliteCommand("SELECT COUNT(*) FROM Dates WHERE [When] = @Date", db);
                sqliteCommand.Parameters.Add(new SqliteParameter("Date", date.Date.ToString("yyyy-MM-dd")));

                using (SqliteDataReader dr = sqliteCommand.ExecuteReader())
                {
                    dr.Read();
                    return dr.GetInt32(0);
                }
            }
        } 
        #endregion
    }
}
