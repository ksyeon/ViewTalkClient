using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using Microsoft.Win32;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;

namespace ViewTalkClient.ViewModels
{
    public class ChattingViewModel : INotifyPropertyChanged
    {
        private TcpClientHelper tcpClient;

        public ObservableCollection<UserData> Users { get; set; }
        public ObservableCollection<ChatMessage> UserChat { get; set; }
        public ObservableCollection<ChatMessage> TeacherChat { get; set; }

        public PPTData PPT { get; set; }

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

        public ICommand ClickLoadPPT
        {
            get { return new DelegateCommand(param => CommandLoadPPT()); }
        }

        public ICommand ClickLeftPPT
        {
            get { return new DelegateCommand(param => CommandMovePPT(0)); }
        }

        public ICommand ClickRightPPT
        {
            get { return new DelegateCommand(param => CommandMovePPT(1)); }
        }

        public ChattingViewModel(TcpClientHelper tcpClient)
        {
            this.tcpClient = tcpClient;
            tcpClient.ExecuteMessage = ResponseMessage;

            this.Users = new ObservableCollection<UserData>();
            this.UserChat = new ObservableCollection<ChatMessage>();
            this.TeacherChat = new ObservableCollection<ChatMessage>();

            this.PPT = new PPTData();

            this.ChatMessage = string.Empty;

            InitializeChatting();
        }

        public void CommandSendChat()
        {
            if (!string.IsNullOrEmpty(ChatMessage))
            {
                AddChatMessage(tcpClient.User.Number, ChatMessage);

                tcpClient.RequestSendChat(ChatMessage);

                ChatMessage = "";
            }
        }

        public void CommandCloseChatting()
        {
            // 이전 윈도우
        }

        public void CommandLoadPPT()
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.DefaultExt = "pptx";
            openFile.Filter = "PowerPoint 프레젠테이션 (*.pptx;*.ppt)|*.pptx;*.ppt";

            openFile.ShowDialog();

            if (openFile.FileName.Length > 0)
            {
                PowerPoint powerPoint = new PowerPoint();

                List<byte[]> bytePPT = powerPoint.ConvertPPT(openFile.FileName); // 비동기 처리 필요
                PPT.LoadPPT(bytePPT);
            }
        }

        public void CommandMovePPT(int direction)
        {
            switch (direction)
            {
                case 0: // Left
                    if (PPT.CurrentPage - 1 >= 0)
                    {
                        PPT.CurrentPPT = PPT.BytePPT[--PPT.CurrentPage];
                    }
                    break;

                case 1: // Right
                    if (PPT.CurrentPage + 1 <= PPT.LastPage)
                    {
                        PPT.CurrentPPT = PPT.BytePPT[++PPT.CurrentPage];
                    }
                    break;
            }
        }

        public void CommandClosePPT()
        {
            PPT.ResetPPT();
        }

        public void InitializeChatting()
        {
            if (tcpClient.User.IsTeacher)
            {
                Users.Add(tcpClient.User);
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
                    switch (message.Check)
                    {
                        case 0:
                            AddUser(new UserData(message.UserNumber, message.Message, false));
                            break;

                        case 1:
                            updateChatting(message.Message);
                            break;
                    }
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

        private void updateChatting(string message)
        {
            JsonHelper json = new JsonHelper();

            List<UserData> userList = json.GetChattingInfo(message);

            Users = new ObservableCollection<UserData>(userList);

            // PPT 불러오기
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

        private void DeleteUser(UserData user)
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
                UserData SendChatUser = Users.First(x => (x.Number == userNumber)); // 예외

                UserChat.Add(new ChatMessage(false, SendChatUser.Nickname, message));

                if (SendChatUser.IsTeacher)
                {
                    TeacherChat.Add(new ChatMessage(false, SendChatUser.Nickname, message));
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