using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;
using ViewTalkClient.Services;
using System.Windows.Media;

namespace ViewTalkClient.ViewModels
{
    public class ChattingViewModel : ViewModelBase
    {
        public MessangerClient Messanger { get; set; }

        public ObservableCollection<UserData> Users { get; set; }
        public ObservableCollection<ChatMessage> UserChat { get; set; }
        public ObservableCollection<ChatMessage> TeacherChat { get; set; }

        private string _chatMessage;
        public string ChatMessage
        {
            get { return _chatMessage; }
            set { _chatMessage = value; RaisePropertyChanged("ChatMessage"); }
        }

        private PPTData _ppt;
        public PPTData PPT
        {
            get { return _ppt; }
            set { _ppt = value; RaisePropertyChanged("PPT"); }
        }        

        public ChattingViewModel(IMessangerService messangerService)
        {
            Messanger = messangerService.GetMessanger(ResponseMessage);

            Users = new ObservableCollection<UserData>();
            UserChat = new ObservableCollection<ChatMessage>();
            TeacherChat = new ObservableCollection<ChatMessage>();

            PPT = new PPTData();
            ChatMessage = string.Empty;

            InitializeChatting();
        }

        public void InitializeChatting()
        {
            if (Messanger.User.IsTeacher)
            {
                Users.Add(Messanger.User);
            }
            else
            {
                if (!Messanger.RequestJoinUser(Messanger.User.Number, Messanger.ChatNumber))
                {
                    MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                    CloseWindow();
                }
            }
        }

        public void ResponseMessage(TcpMessage message)
        {
            switch (message.Command)
            {
                case Command.CloseChatting:
                    CloseChatting();
                    break;

                case Command.JoinUser:
                    switch (message.Check)
                    {
                        case 0:
                            AddUser(new UserData(message.UserNumber, message.Message, false));
                            break;

                        case 1:
                            UpdateChatting(message.ChatNumber, message.Message, message.PPT);
                            break;
                    }
                    break;

                case Command.ExitUser:
                    DeleteUser(message.UserNumber);
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
        

        private void CloseChatting()
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show("채팅방이 종료되었습니다.", AppConst.AppName); // 오류 수정해야 함 (Modal)

                CloseWindow();
            });
        }

        private void UpdateChatting(int chatNumbet, string message, PPTData ppt)
        {
            JsonHelper json = new JsonHelper();

            List<UserData> userList = json.GetChattingInfo(chatNumbet, message);
            
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Users.Clear();

                foreach (UserData user in userList)
                {
                    Users.Add(user);
                }
            });

            LoadPPT(ppt);
        }

        private void AddUser(UserData user)
        {
            string notice = $"'{user.Nickname}'님이 입장하셨습니다.";

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Users.Add(user);
                UserChat.Add(new ChatMessage(ChatType.Notice, user.Nickname, notice));
            });
        }

        private void DeleteUser(int userNumber)
        {
            UserData user = Users.First(x => (x.Number == userNumber)); // ArgumentNullException

            string notice = $"'{user.Nickname}'님이 퇴장하셨습니다.";

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Users.Remove(user);
                UserChat.Add(new ChatMessage(ChatType.Notice, user.Nickname, notice));
            });
        }


        private void AddChatMessage(int userNumber, string message)
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                UserData user = Users.First(x => (x.Number == userNumber)); // ArgumentNullException

                if (Messanger.User.Number == userNumber)
                {
                    UserChat.Add(new ChatMessage(ChatType.User, user.Nickname, message));
                }
                else if (user.IsTeacher)
                {
                    UserChat.Add(new ChatMessage(ChatType.Teacher, user.Nickname, message));
                }
                else
                {
                    UserChat.Add(new ChatMessage(ChatType.Student, user.Nickname, message));
                }

                if (user.IsTeacher)
                {
                    TeacherChat.Add(new ChatMessage(ChatType.Teacher, user.Nickname, message));
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

        public void CloseWindow()
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                    }
                }
            });
        }

        public ICommand SendChatCommand
        {
            get { return new RelayCommand(ExcuteSendChat); }
        }

        public ICommand OpenPPTCommand
        {
            get { return new RelayCommand(ExcuteOpenPPT); }
        }

        public ICommand OpenImageCommand
        {
            get { return new RelayCommand(ExcuteOpenImage); }
        }

        public ICommand MoveLeftPPTCommand
        {
            get { return new RelayCommand(ExcuteMoveLeftPPT); }
        }

        public ICommand MoveRightPPTCommand
        {
            get { return new RelayCommand(ExcuteMoveRightPPT); }
        }

        public ICommand CloseWindowCommand
        {
            get { return new RelayCommand(ExcuteCloseWindow); }
        }

        public void ExcuteSendChat()
        {
            if (!string.IsNullOrEmpty(ChatMessage))
            {
                AddChatMessage(Messanger.User.Number, ChatMessage);

                if (!Messanger.RequestSendChat(Messanger.User.Number, Messanger.ChatNumber, ChatMessage))
                {
                    MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                    CloseWindow();
                }

                ChatMessage = string.Empty;
            }
        }

        public void ExcuteOpenPPT()
        {
            if (Messanger.User.IsTeacher)
            {
                if (PPT.LastPage == 0)
                {
                    OpenFileDialog openFile = new OpenFileDialog();

                    openFile.DefaultExt = "pptx";
                    openFile.Filter = "PowerPoint 프레젠테이션 (*.pptx;*.ppt)|*.pptx;*.ppt";

                    openFile.ShowDialog();

                    if (openFile.FileName.Length > 0)
                    {
                        PPTManager powerPoint = new PPTManager();

                        List<byte[]> bytePPT = powerPoint.ConvertPPT(openFile.FileName); // 비동기 처리 필요

                        if (bytePPT.Count > 0)
                        {
                            PPT.OpenPPT(bytePPT);

                            if (!Messanger.RequestSendPPT(Messanger.User.Number, Messanger.ChatNumber, PPT))
                            {
                                MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                                CloseWindow();
                            }
                        }
                        else
                        {
                            MessageBox.Show("PPT를 불러오는 데 실패했습니다.", AppConst.AppName);
                        }
                    }
                }
                else
                {
                    PPT.ResetPPT();

                    if (!Messanger.RequestClosePPT(Messanger.User.Number, Messanger.ChatNumber))
                    {
                        MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                        CloseWindow();
                    }
                }
            }
            else
            {
                MessageBox.Show("강사만 사용할 수 있습니다.", AppConst.AppName);
            }
        }

        public void ExcuteOpenImage()
        {
            if (Messanger.User.IsTeacher)
            {
                if (PPT.LastPage == 0)
                {
                    OpenFileDialog openFile = new OpenFileDialog();

                    openFile.DefaultExt = "jpg";
                    openFile.Filter = "그림 파일 (*.jpg;)|*.jpg;";

                    openFile.ShowDialog();

                    if (openFile.FileName.Length > 0)
                    {
                        PPTManager powerPoint = new PPTManager();

                        byte[] byteImage = powerPoint.ConvertImageToByte(openFile.FileName); // 비동기 처리 필요

                        List<byte[]> bytePPT = new List<byte[]>();
                        bytePPT.Add(byteImage);

                        if (bytePPT.Count > 0)
                        {
                            PPT.OpenPPT(bytePPT);

                            if (!Messanger.RequestSendPPT(Messanger.User.Number, Messanger.ChatNumber, PPT))
                            {
                                MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                                CloseWindow();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Image를 불러오는 데 실패했습니다.", AppConst.AppName);
                        }
                    }
                }
                else
                {
                    PPT.ResetPPT();

                    if (!Messanger.RequestClosePPT(Messanger.User.Number, Messanger.ChatNumber))
                    {
                        MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                        CloseWindow();
                    }
                }
            }
            else
            {
                MessageBox.Show("강사만 사용할 수 있습니다.", AppConst.AppName);
            }
        }

        public void ExcuteMoveLeftPPT()
        {
            if (Messanger.User.IsTeacher)
            {
                if (PPT.CurrentPage > 1)
                {
                    PPT.CurrentPPT = PPT.BytePPT[(--PPT.CurrentPage) - 1];

                    if (!Messanger.RequestSendPPT(Messanger.User.Number, Messanger.ChatNumber, PPT))
                    {
                        MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                        CloseWindow();
                    }
                }
            }
            else
            {
                MessageBox.Show("강사만 사용할 수 있습니다.", AppConst.AppName);
            }
        }

        public void ExcuteMoveRightPPT()
        {
            if (Messanger.User.IsTeacher)
            {
                if (PPT.CurrentPage < PPT.LastPage)
                {
                    PPT.CurrentPPT = PPT.BytePPT[(++PPT.CurrentPage) - 1];

                    if (!Messanger.RequestSendPPT(Messanger.User.Number, Messanger.ChatNumber, PPT))
                    {
                        MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                        CloseWindow();
                    }
                }
            }
            else
            {
                MessageBox.Show("강사만 사용할 수 있습니다.", AppConst.AppName);
            }
        }

        public void ExcuteCloseWindow()
        {
            /*
            if (Messanger.ChatNumber != 0)
            {
                Messanger.ChatNumber = 0;

                SettingWindow settingWindow = new SettingWindow();
                settingWindow.Show();
            }*/

            if (Messanger.User.IsTeacher)
            {
                if (!Messanger.RequestCloseChatting(Messanger.User.Number, Messanger.ChatNumber))
                {
                    MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                    CloseWindow();
                }
            }
            else
            {
                if (!Messanger.RequestExistUser(Messanger.User.Number, Messanger.ChatNumber))
                {
                    MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                    CloseWindow();
                }
            }

            // CloseWindow();
        }
    }
}