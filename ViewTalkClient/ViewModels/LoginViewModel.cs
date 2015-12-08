using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;
using ViewTalkClient.Services;

namespace ViewTalkClient.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public MessangerClient Messanger { get; set; }

        private string _id;
        public string ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged("ID"); }
        }

        public LoginViewModel(IMessangerService messangerService)
        {
            Messanger = messangerService.GetMessanger(ResponseMessage);

            Messanger.User.Reset();

            ID = string.Empty;
        }

        public void ResponseMessage(TcpMessage message)
        {
            switch (message.Command)
            {
                case Command.Login:
                    ValidateLogin(message.Check, message.UserNumber, message.Message);
                    break;
            }
        }

        public void ValidateLogin(int check, int userNumber, string nickname)
        {
            switch (check)
            {
                case 0:
                    Messanger.User.Number = userNumber;
                    Messanger.User.Nickname = nickname;

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        SettingWindow settingWindow = new SettingWindow();
                        settingWindow.Show();

                        CloseWindow();
                    });

                    break;

                case 1:
                    MessageBox.Show("이미 로그인되어 있습니다.", AppConst.AppName);
                    break;

                case 2:
                    MessageBox.Show("아이디/비밀번호가 일치하지 않습니다.", AppConst.AppName);
                    break;
            }
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

        public ICommand LoginCommand
        {
            get { return new RelayCommand<PasswordBox>(ExcuteLogin); }
        }

        public void ExcuteLogin(PasswordBox passwordBox)
        {
            var pwdPassword = passwordBox as PasswordBox;
            string password = pwdPassword.Password;

            if (string.IsNullOrEmpty(ID))
            {
                MessageBox.Show("아이디를 입력하세요.", AppConst.AppName);
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("비밀번호를 입력하세요.", AppConst.AppName);
            }
            else
            {
                if (!Messanger.RequestLogin(ID, password))
                {
                    MessageBox.Show("서버와의 연결이 끊겼습니다.", AppConst.AppName);
                    CloseWindow();
                }
            }
        }
    }
}
