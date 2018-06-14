using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SportDiary.ContentDialogs;
//using SportDiary.Data;

namespace SportDiary.Models
{
    class MainPageModel
    {
        private string connectionString = "Filename=SportDiary.db";

        public void SelectDates(ObservableCollection<Date> datesCollection)
        {
            if (datesCollection.Count > 0)
            {
                datesCollection.Clear();
            }

            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Dates ORDER BY [When] DESC", db);

                SqliteDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    datesCollection.Add(new Date(dr.GetInt64(0), dr.GetDateTime(1)));
                }
            }
        }

        public void InsertDate(Date newDate)
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
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
            using (SqliteConnection db = new SqliteConnection(connectionString))
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
            using (SqliteConnection db = new SqliteConnection(connectionString))
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

        public void SelectExercises(long idDate, ObservableCollection<Exercise> exercisesCollection)
        {
            if (exercisesCollection.Count > 0)
            {
                exercisesCollection.Clear();
            }

            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Dates WHERE [When] = @NewDate", db);
                command.Parameters.Add(new SqliteParameter("CurIdDate", idDate));

                SqliteDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    exercisesCollection.Add(new Exercise(dr.GetInt64(0), dr.GetInt64(1), dr.GetString(2), dr.GetValue(3) as long?, dr.GetValue(4) as long?, dr.GetValue(5) as string));
                }
            }
        }

        public void InsertExercise(Exercise newExercise)
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("INSERT INTO Exercises (IdDate, Name, Repetition, Approaches, Description) VALUES(@IdDate, @Name, @Repetition, @Approaches, @Description)", db);
                command.Parameters.Add(new SqliteParameter("IdDate", newExercise.IdDate));
                command.Parameters.Add(new SqliteParameter("Name", newExercise.Name));
                command.Parameters.Add(new SqliteParameter("Repetition", newExercise.Repetition));
                command.Parameters.Add(new SqliteParameter("Approaches", newExercise.Approaches));
                command.Parameters.Add(new SqliteParameter("Description", newExercise.Description));
                command.ExecuteNonQuery();

                command.CommandText = "SELECT IdExercise FROM Exercises WHERE IdExercise = @Name AND IdDate = @IdDate";
                command.Parameters.Clear();
                command.Parameters.Add(new SqliteParameter("IdDate", newExercise.IdDate));
                command.Parameters.Add(new SqliteParameter("Name", newExercise.Name));

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    long IdExercise = dr.GetInt64(0);
                    newExercise.IdExercise = IdExercise;
                }
            }
        }

        public void EditExercise(Exercise Exercise)
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("UPDATE Exercises SET Name = @Name, Repetition = @Repetition, Approaches = @Approaches, Description = @Description", db);
                command.Parameters.Add(new SqliteParameter("Name", Exercise.Name));
                command.Parameters.Add(new SqliteParameter("Repetition", Exercise.Repetition));
                command.Parameters.Add(new SqliteParameter("Approaches", Exercise.Approaches));
                command.Parameters.Add(new SqliteParameter("Description", Exercise.Description));
                command.ExecuteNonQuery();
            }
        }

        public void RemoveExercise(long idExercise)
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("DELETE FROM Exercises WHERE IdExercise = @Id", db);
                SqliteParameter parameter = new SqliteParameter("Id", idExercise);
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
            }
        }

        public void InsertToNamesExercises()
        {

        }

        private bool IsExistsNameExercise(string Name)
        {
            using (SqliteConnection db = new SqliteConnection(connectionString))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM NamesExercises WHERE Name = @Name", db);
                command.Parameters.Add(new SqliteParameter("Name", Name));
                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    int count = dr.GetInt32(0);
                    if(count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}

