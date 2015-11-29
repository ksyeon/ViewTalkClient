using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;
using System.Windows.Input;

namespace ViewTalkClient.ViewModels
{
    public class ChattingViewModel : INotifyPropertyChanged
    {
        private TcpClientHelper tcpClient;

        private int chatNumber;
        private bool isTeacher;

        public ObservableCollection<UserData> Users { get; set; }
        public ObservableCollection<ChatMessage> UserChat { get; set; }
        public ObservableCollection<ChatMessage> TeacherChat { get; set; }
        // PPTData

        private string _chatMessage;
        public string ChatMessage
        {
            get { return _chatMessage; }
            set { _chatMessage = value; OnNotifyPropertyChanged("ChatMessage"); }
        }

        public ICommand ClickSendChat
        {
            get { return new DelegateCommand(param => CommandSendChat()); }
        }

        public ChattingViewModel(TcpClientHelper tcpClient)
        {
            this.tcpClient = tcpClient;
            tcpClient.ExecuteMessage = ResponseMessage;

            this.chatNumber = tcpClient.ChatNumber;
            this.isTeacher = tcpClient.User.IsTeacher;

            this.Users = new ObservableCollection<UserData>();
            this.UserChat = new ObservableCollection<ChatMessage>();
            this.TeacherChat = new ObservableCollection<ChatMessage>();

            this.ChatMessage = string.Empty;

            InitializeChatting();
        }

        public void CommandSendChat()
        {
            if (ChatMessage.Length > 0)
            {
                AddChatMessage(tcpClient.User.Number, ChatMessage);

                tcpClient.RequestSendChat(chatNumber, ChatMessage);

                ChatMessage = "";
            }
        }

        public void CommandCloseChatting()
        {

        }

        public void CommandLoadPPT()
        {

        }

        public void CommandMovePPT()
        {

        }

        public void CommandClosePPT()
        {

        }

        public void InitializeChatting()
        {
            // 방장/참여자 정보 가져오기
            // PPT 가져오기

            if (tcpClient.User.IsTeacher)
            {
                chatNumber = tcpClient.User.Number;
            }
            else
            {
                tcpClient.RequestJoinUser();
            }
        }

        public void ResponseMessage(TcpMessage message)
        {
            switch (message.Command)
            {
                case Command.JoinUser:
                    AddUser(new UserData(message.UserNumber, message.Message, false));
                    break;

                case Command.ExitUser:
                    break;

                case Command.SendChat:
                    AddChatMessage(message.UserNumber, message.Message);
                    break;

                case Command.LoadPPT:
                    break;

                case Command.MovePPT:
                    break;

                case Command.ClosePPT:
                    break;
            }
        }

        private void AddUser(UserData user)
        {
            string notice = user.Nickname + " 님이 입장하셨습니다.";

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Users.Add(user);
                UserChat.Add(new ChatMessage(true, user.Nickname, notice));
            });
        }

        private void Delete(UserData user)
        {
            string notice = user.Nickname + " 님이 퇴장하셨습니다.";

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Users.Remove(user);
                UserChat.Add(new ChatMessage(true, user.Nickname, notice));
            });
        }

        private void AddChatMessage(int userNumber, string message)
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                UserData SendChatUser = Users.First(x => (x.Number == userNumber));

                if (SendChatUser != null)
                {
                    UserChat.Add(new ChatMessage(false, SendChatUser.Nickname, message));

                    if (SendChatUser.IsTeacher)
                    {
                        TeacherChat.Add(new ChatMessage(false, SendChatUser.Nickname, message));
                    }
                }
            });
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