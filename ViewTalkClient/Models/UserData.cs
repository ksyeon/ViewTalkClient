using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace ViewTalkClient.Models
{
    public class UserData : INotifyPropertyChanged
    {
        public int Number { get; set; }

        private string _nickname;
        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; OnNotifyPropertyChanged("Nickname"); }
        }

        public bool IsTeacher { get; set; }

        public UserData() : this(0, string.Empty, false)
        {

        }

        public UserData(int number, string nickname, bool isTeacher)
        {
            this.Number = number;
            this.Nickname = nickname;
            this.IsTeacher = IsTeacher;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
