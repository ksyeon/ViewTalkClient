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
        public ObservableCollection<ChattingData> Chatting { get; set; }
        public ObservableCollection<int> Participant { get; set; }

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
            this.Participant = new ObservableCollection<int>();
            this.Chatting = new ObservableCollection<ChattingData>();

            this.ChattingMessage = string.Empty;

            App.TcpClient.ExecuteMessage = ExecuteMessage;
            App.TcpClient.SendConnect();
        }

        public void ExecuteMessage(MessageData message)
        {
            string notice = string.Empty;

            switch (message.Command)
            {
                case Command.Connect:
                    notice = message.Number + " 님이 입장하셨습니다.";

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Participant.Add(message.Number);
                        Chatting.Add(new ChattingData(message.Number, notice));
                    });

                    break;

                case Command.Close:
                    notice = message.Number + " 님이 퇴장하셨습니다.";

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Participant.Remove(message.Number);
                        Chatting.Add(new ChattingData(message.Number, notice));
                    });

                    break;

                case Command.Message:
                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Chatting.Add(new ChattingData(message.Number, message.Message));
                    });

                    break;

                case Command.Update:

                    break;
            }
        }

        private void SendMessage()
        {
            if (ChattingMessage.Length > 0)
            {
                Chatting.Add(new ChattingData(App.TcpClient.UserNumber, ChattingMessage));

                App.TcpClient.SendChatting(ChattingMessage);

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
