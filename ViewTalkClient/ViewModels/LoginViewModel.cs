﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;

namespace ViewTalkClient.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private TcpClientHelper tcpClient;

        private string _id;
        public string ID
        {
            get { return _id; }
            set { _id = value; OnNotifyPropertyChanged("ID"); }
        }

        public ICommand ClickLogin
        {
            get { return new DelegateCommand(param => CommandLogin("1234")); }
        }

        public LoginViewModel(TcpClientHelper tcpClient)
        {
            this.tcpClient = tcpClient;
            this.tcpClient.ExecuteMessage = ResponseMessage;

            this.ID = string.Empty;
        }

        private void CommandLogin(string password)
        {
            if (string.IsNullOrEmpty(ID))
            {
                MessageBox.Show("아이디를 입력하세요.;", AppConst.AppName);
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("비밀번호를 입력하세요.", AppConst.AppName);
            }
            else
            {
                tcpClient.RequestLogin(ID, password);
            }
        }

        private void ValidateLogin(int check, int userNumber)
        {
            switch (check)
            {
                case 0:
                    tcpClient.User.Number = userNumber;

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        SettingWindow settingWindow = new SettingWindow();
                        settingWindow.ShowDialog();
                        
                        // Close();
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

        public void ResponseMessage(TcpMessage message)
        {
            switch (message.Command)
            {
                case Command.Login:
                    ValidateLogin(message.Check, message.UserNumber);
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
