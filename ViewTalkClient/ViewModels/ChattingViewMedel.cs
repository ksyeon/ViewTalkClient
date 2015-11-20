using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;

using ViewTalkClient.Models;
using System.Windows.Input;

namespace ViewTalkClient.ViewModels
{
    public class ChattingViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatMessage> Chatting { get; set; }
        public ObservableCollection<int> Participant { get; set; }

        private string _chattingMessage;
        public string ChattingMessage
        {
            get { return _chattingMessage; }
            set { _chattingMessage = value; OnNotifyPropertyChanged("ChattingMessage"); }
        }

        public ICommand ClickSendChat
        {
            get { return new DelegateCommand(new Action(CommandSendChat), null); }
        }

        public ChattingViewModel()
        {
            this.Participant = new ObservableCollection<int>();
            this.Chatting = new ObservableCollection<ChatMessage>();

            this.ChattingMessage = string.Empty;

            App.TcpClient.ExecuteMessage = ResponseMessage;
            App.TcpClient.RequestConnect();
        }

        public void ResponseMessage(TcpMessage message)
        {
            string notice = string.Empty;

            switch (message.Command)
            {
                case Command.Connect:
                    notice = message.Number + " 님이 입장하셨습니다.";

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Participant.Add(message.Number);
                        Chatting.Add(new ChatMessage(message.Number, notice));
                    });

                    break;

                case Command.Close:
                    notice = message.Number + " 님이 퇴장하셨습니다.";

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Participant.Remove(message.Number);
                        Chatting.Add(new ChatMessage(message.Number, notice));
                    });

                    break;

                case Command.Message:
                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Chatting.Add(new ChatMessage(message.Number, message.Message));
                    });

                    break;

                case Command.Update:

                    break;
            }
        }

        private void CommandSendChat()
        {
            if (ChattingMessage.Length > 0)
            {
                Chatting.Add(new ChatMessage(App.TcpClient.UserNumber, ChattingMessage));

                App.TcpClient.RequestChatting(ChattingMessage);

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
}