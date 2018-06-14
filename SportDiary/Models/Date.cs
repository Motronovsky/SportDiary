using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SportDiary.Models
{
    public class Date : MVVMLib.NotifyPropertyChanged, INotifyPropertyChanged
    {
        #region Fields
        private DateTime when;
        #endregion

        #region Properties
        public long IdDate { get; set; }

        public DateTime When
        {
            get { return when; }
            set { when = value; OnPropertyChanged(); }
        } 
        #endregion

        #region Constructors
        public Date(long idDate, DateTime aWhen)
        {
            IdDate = idDate;
            When = aWhen;
        } 
        #endregion
    }
}
