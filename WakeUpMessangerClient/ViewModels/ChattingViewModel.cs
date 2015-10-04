using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;

using WakeUpMessangerClient.Models;
using WakeUpMessangerClient.Modules;
using System.Windows.Input;

namespace WakeUpMessangerClient.ViewModels
{
    class ChattingViewModel : INotifyPropertyChanged
    {
        private TcpClientHelper tcpClient;

        public ObservableCollection<ulong> Participant { get; set; }
        public ObservableCollection<ChattingData> Chatting { get; set; }

        private string _chattingMessage;
        public string ChattingMessage
        {
            get { return _chattingMessage; }
            set { _chattingMessage = value; OnNotifyPropertyChanged("ChattingMessage"); }
        }

        public ICommand ClickSendMessage
        {
            get { return new DelegateCommand(SendMessage); }
        }

        public ChattingViewModel()
        {
            this.tcpClient = new TcpClientHelper(1111, GetMessage);

            this.Participant = new ObservableCollection<ulong>();
            this.Chatting = new ObservableCollection<ChattingData>();

            this.ChattingMessage = "";
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

        private void SendMessage()
        {
            if(ChattingMessage.Length > 0)
            { 
                Chatting.Add(new ChattingData(1234, ChattingMessage));

                tcpClient.SendChattingMessage(ChattingMessage);

                ChattingMessage = "";
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

    public class DelegateCommand : ICommand
    {
        private readonly Action _action;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
