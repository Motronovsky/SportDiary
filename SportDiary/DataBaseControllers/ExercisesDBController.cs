using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using SportDiary.Models;

namespace SportDiary.DataBaseControllers
{
    class ExercisesDBController
    {
        #region Fields
        private SqliteConnection sqliteConnection;
        #endregion

        #region Constructors
        public ExercisesDBController(SqliteConnection connection)
        {
            sqliteConnection = connection;
        }
        #endregion

        #region Methods
        public void SelectExercises(long idDate, ObservableCollection<Exercise> exercisesCollection)
        {
            if (exercisesCollection.Count > 0)
            {
                exercisesCollection.Clear();
            }

            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("SELECT IdExercise, IdDate, Name, Repetition, Approaches, Description FROM Exercises WHERE IdDate = @CurIdDate", db);
                command.Parameters.Add(new SqliteParameter("CurIdDate", idDate));

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        exercisesCollection.Add(new Exercise(dr.GetInt64(0), dr.GetInt64(1), dr.GetString(2), dr.GetValue(3) as long?, dr.GetValue(4) as long?, dr.GetValue(5) as string));
                    }
                }
            }
        }

        public void InsertExercise(Exercise newExercise)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("INSERT INTO Exercises (IdDate, Name, Repetition, Approaches, Description) VALUES(@IdDate, @Name, @Repetition, @Approaches, @Description)", db);

                command.Parameters.Add(new SqliteParameter("IdDate", newExercise.IdDate));
                command.Parameters.Add(new SqliteParameter("Name", newExercise.Name));

                if (newExercise.Repetition == null)
                {
                    command.Parameters.Add(new SqliteParameter("Repetition", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqliteParameter("Repetition", newExercise.Repetition));
                }

                if (newExercise.Approaches == null)
                {
                    command.Parameters.Add(new SqliteParameter("Approaches", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqliteParameter("Approaches", newExercise.Approaches));
                }

                command.Parameters.Add(new SqliteParameter("Description", newExercise.Description));
                command.ExecuteNonQuery();

                command.CommandText = "SELECT IdExercise FROM Exercises WHERE Name = @Name AND IdDate = @IdDate";
                command.Parameters.Clear();
                command.Parameters.Add(new SqliteParameter("IdDate", newExercise.IdDate));
                command.Parameters.Add(new SqliteParameter("Name", newExercise.Name));

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    int IdExercise = dr.GetInt32(0);
                    newExercise.IdExercise = IdExercise;
                }
            }
        }

        public void EditExercise(Exercise Exercise)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("UPDATE Exercises SET Name = @Name, Repetition = @Repetition, Approaches = @Approaches, Description = @Description WHERE IdExercise = @IdExercise", db);

                command.Parameters.Add(new SqliteParameter("IdExercise", Exercise.IdExercise));
                command.Parameters.Add(new SqliteParameter("Name", Exercise.Name));

                if (Exercise.Repetition == null)
                {
                    command.Parameters.Add(new SqliteParameter("Repetition", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqliteParameter("Repetition", Exercise.Repetition));
                }

                if (Exercise.Approaches == null)
                {
                    command.Parameters.Add(new SqliteParameter("Approaches", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqliteParameter("Approaches", Exercise.Approaches));
                }

                command.Parameters.Add(new SqliteParameter("Description", Exercise.Description));
                command.ExecuteNonQuery();
            }
        }

        public void RemoveExercise(long idExercise)
        {
            using (SqliteConnection db = sqliteConnection)
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("DELETE FROM Exercises WHERE IdExercise = @Id", db);
                SqliteParameter parameter = new SqliteParameter("Id", idExercise);
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
            }
        }

        public int CountExercisesInDay(long idDate, string name)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=SportDiary.db"))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Exercises WHERE Name = @Name AND IdDate = @IdDate", db);
                command.Parameters.Add(new SqliteParameter("Name", name));
                command.Parameters.Add(new SqliteParameter("IdDate", idDate));

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    return dr.GetInt32(0);
                }
            }
        }

        public static bool IsExistExercisesInDay(long idDate, string name)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=SportDiary.db"))
            {
                db.Open();

                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Exercises WHERE Name = @Name AND IdDate = @IdDate", db);
                command.Parameters.Add(new SqliteParameter("Name", name));
                command.Parameters.Add(new SqliteParameter("IdDate", idDate));

                using (SqliteDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    return dr.GetInt32(0) > 0;
                }
            }
        } 
        #endregion
    }
}
