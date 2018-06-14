using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SportDiary.Models
{
    public class Exercise : MVVMLib.NotifyPropertyChanged, INotifyPropertyChanged
    {
        #region Fields
        private string name;
        private long? repetition;
        private long? approaches;
        private string description; 
        #endregion

        #region Properties
        public long IdExercise { get; set; }
        public long IdDate { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        public long? Repetition
        {
            get { return repetition; }
            set { repetition = value; OnPropertyChanged(); }
        }

        public long? Approaches
        {
            get { return approaches; }
            set { approaches = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }
        #endregion

        #region Constructors
        public Exercise(long aIdExercise, long aIdDate, string aName, long? aRepetition, long? aApproaches, string aDescription)
        {
            IdExercise = aIdExercise;
            IdDate = aIdDate;
            Name = aName;
            Repetition = aRepetition;
            Approaches = aApproaches;
            Description = aDescription;
        }

        public Exercise() { } 
        #endregion
    }
}
