using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

using GalaSoft.MvvmLight;

namespace ViewTalkClient.Models
{
    public class UserData : ViewModelBase
    {
        public int Number { get; set; }
        public string Nickname { get; set; }
        public bool IsTeacher { get; set; }

        public UserData() : this(0, string.Empty, false)
        {

        }

        public UserData(int number, string nickname, bool isTeacher)
        {
            Number = number;
            Nickname = nickname;
            IsTeacher = IsTeacher;
        }
    }
}
