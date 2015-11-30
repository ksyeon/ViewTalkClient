using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
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

        private PPTData _ppt;
        public PPTData PPT
        {
            get { return _ppt; }
            set { _ppt = value; OnNotifyPropertyChanged("PPT"); }
        }

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

        public ICommand ClickOpenPPT
        {
            get { return new DelegateCommand(param => CommandOpenPPT()); }
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
            // Close();
        }

        public void CommandOpenPPT()
        {
            if (tcpClient.User.IsTeacher)
            {
                if(PPT.LastPage == 0)
                {
                    OpenFileDialog openFile = new OpenFileDialog();

                    openFile.DefaultExt = "pptx";
                    openFile.Filter = "PowerPoint 프레젠테이션 (*.pptx;*.ppt)|*.pptx;*.ppt";

                    openFile.ShowDialog();

                    if (openFile.FileName.Length > 0)
                    {
                        PowerPoint powerPoint = new PowerPoint();

                        List<byte[]> bytePPT = powerPoint.ConvertPPT(openFile.FileName); // 비동기 처리 필요
                        PPT.OpenPPT(bytePPT);

                        tcpClient.RequestSendPPT(PPT);
                    }
                }
                else
                {
                    PPT.ResetPPT();

                    tcpClient.RequestClosePPT();
                }
            }
            else
            {
                MessageBox.Show("강사만 클릭할 수 있습니다.", AppConst.AppName);
            }
        }

        public void CommandMovePPT(int direction)
        {
            if (tcpClient.User.IsTeacher)
            {
                switch (direction)
                {
                    case 0: // Left
                        if (PPT.CurrentPage > 1)
                        {
                            PPT.CurrentPPT = PPT.BytePPT[(--PPT.CurrentPage) - 1];

                            tcpClient.RequestSendPPT(PPT);
                        }
                        break;

                    case 1: // Right
                        if (PPT.CurrentPage < PPT.LastPage)
                        {
                            PPT.CurrentPPT = PPT.BytePPT[(++PPT.CurrentPage) - 1];

                            tcpClient.RequestSendPPT(PPT);
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("강사만 클릭할 수 있습니다.", AppConst.AppName);
            }
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
                            UpdateChatting(message.Message);
                            break;
                    }
                    break;

                case Command.ExitUser:
                    break;

                case Command.SendChat:
                    AddChatMessage(message.UserNumber, message.Message);
                    break;

                case Command.SendPPT:
                    LoadPPT(message.PPT);
                    break;

                case Command.ClosePPT:
                    ClosePPT();
                    break;
            }
        }

        private void UpdateChatting(string message)
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
                UserData SendChatUser = Users.First(x => (x.Number == userNumber)); // ArgumentNullException

                UserChat.Add(new ChatMessage(false, SendChatUser.Nickname, message));

                if (SendChatUser.IsTeacher)
                {
                    TeacherChat.Add(new ChatMessage(false, SendChatUser.Nickname, message));
                }
            });
        }

        private void LoadPPT(PPTData ppt)
        {
            PPT = ppt;
        }

        private void ClosePPT()
        {
            PPT.ResetPPT();
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