using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;

namespace ViewTalkClient.Models
{
    public class ChatMessage
    {
        public ChatType Type { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }

        private SolidColorBrush _color;

        public SolidColorBrush Color
        {
            get
            {
                SolidColorBrush color = _color;

                switch (Type)
                {
                    case ChatType.Notice:
                        color = Application.Current.Resources["NoticeColor"] as SolidColorBrush;
                        break;

                    case ChatType.Teacher:
                        color = Application.Current.Resources["TeacherColor"] as SolidColorBrush;
                        break;

                    case ChatType.Student:
                        color = Application.Current.Resources["StudentColor"] as SolidColorBrush;
                        break;

                    case ChatType.User:
                        color = Application.Current.Resources["UserColor"] as SolidColorBrush;
                        break;
                }

                return color;
            }

            set { _color = value; }
        }


        public ChatMessage(ChatType type, string nickname, string message)
        {
            Type = type;
            Nickname = nickname;
            Message = message;
            _color = new SolidColorBrush(Colors.Black);
        }
    }

    public enum ChatType
    {
        Notice,
        Teacher,
        Student,
        User
    }
}
