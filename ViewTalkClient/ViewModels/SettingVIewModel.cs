using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;
using ViewTalkClient.Services;

namespace ViewTalkClient.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private MessangerClient Messanger { get; set; }

        private string _teacherNickname;
        public string TecherNickname
        {
            get { return _teacherNickname; }
            set { _teacherNickname = value; RaisePropertyChanged("TecherNickName"); }
        }

        public SettingViewModel(IMessangerService messangerService)
        {
            Messanger = messangerService.GetMessanger(ResponseMessage);

            Messanger.ChatNumber = 0;
        }

        public void ResponseMessage(TcpMessage message)
        {
            switch (message.Command)
            {
                case Command.CreateChatting:
                    CreateChatting();
                    break;

                case Command.JoinChatting:
                    JoinChatting(message.Check, message.ChatNumber);
                    break;

                case Command.Logout:
                    Logout();
                    break;
            }
        }

        private void CreateChatting()
        {
            Messanger.ChatNumber = Messanger.User.Number;
            Messanger.User.IsTeacher = true;

            ShowChattingWindow();
        }

        private void JoinChatting(int check, int chatNumber)
        {
            switch (check)
            {
                case 0:
                    Messanger.ChatNumber = chatNumber;
                    ShowChattingWindow();
                    break;

                case 1:
                    TecherNickname = string.Empty;
                    MessageBox.Show("개설되지 않은 채팅방입니다.", AppConst.AppName);
                    break;

                case 2:
                    TecherNickname = string.Empty;
                    MessageBox.Show("존재하지 않는 닉네임입니다.", AppConst.AppName);
                    break;
            }
        }

        public void Logout()
        {
            Messanger.User.Reset();

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
            });

            CloseWindow();
        }

        public void ShowChattingWindow()
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                ChattingWindow chattingWindow = new ChattingWindow();
                chattingWindow.Show();
            });

            CloseWindow();
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

        public ICommand CreateChattingCommand
        {
            get { return new RelayCommand(ExcuteCreateChatting); }
        }

        public ICommand JoinChattingCommand
        {
            get { return new RelayCommand(ExcuteJoinChatting); }
        }

        public ICommand LogoutCommand
        {
            get { return new RelayCommand(ExcuteLogout); }
        }

        public void ExcuteCreateChatting()
        {
            if (!Messanger.RequestCreateChatting(Messanger.User.Number))
            {
                MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                CloseWindow();
            }
        }

        public void ExcuteJoinChatting()
        {
            if (string.IsNullOrEmpty(TecherNickname))
            {
                MessageBox.Show("강사의 닉네임을 입력하세요.", AppConst.AppName);
            }
            else
            {
                if (!Messanger.RequestJoinChatting(Messanger.User.Number, TecherNickname))
                {
                    MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                    CloseWindow();
                }
            }
        }

        public void ExcuteLogout()
        {
            if (!Messanger.RequestLogout(Messanger.User.Number))
            {
                MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                CloseWindow();
            }
        }
    }
}
