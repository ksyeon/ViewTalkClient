using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;

using WakeUpMessangerClient.Models;
using WakeUpMessangerClient.Modules;

namespace WakeUpMessangerClient.ViewModels
{
    class ChattingViewModel : INotifyPropertyChanged
    {
        private TcpClient tcpClient;

        public ObservableCollection<ulong> Participant { get; set; }
        public ObservableCollection<ChattingData> Chatting { get; set; }

        public ChattingViewModel()
        {
            this.tcpClient = new TcpClientHelper(1234, GetMessage);
            this.Chatting = new ObservableCollection<ChattingData>();
        }

        public void GetMessage(MessageData message)
        {
            string notice = string.Empty;

            switch (message.Command)
            {
                case Command.Login:
                    Participant.Add(message.UserNumber);

                    notice = message.UserNumber + " 님이 입장하셨습니다.";
                    Chatting.Add(new ChattingData(0, notice));

                    break;

                case Command.Logout:
                    Participant.Remove(message.UserNumber);

                    notice = message.UserNumber + " 님이 퇴장하셨습니다.";
                    Chatting.Add(new ChattingData(0, notice));

                    break;

                case Command.Message:
                    Chatting.Add(new ChattingData(message.UserNumber, message.Message));

                    break;

                case Command.Update:

                    break;
            }
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
