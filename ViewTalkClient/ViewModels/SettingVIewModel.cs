using System;
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
    public class SettingViewModel : INotifyPropertyChanged
    {
        private TcpClientHelper tcpClient;

        private string _teacherNickName;
        public string TecherNickName
        {
            get { return _teacherNickName; }
            set { _teacherNickName = value; OnNotifyPropertyChanged("TecherNickName"); }
        }

        public ICommand ClickCreateChatting
        {
            get { return new DelegateCommand(param => CommandCreateChatting()); }
        }

        public ICommand ClickJoinChatting
        {
            get { return new DelegateCommand(param =>  CommandJoinChatting()); }
        }

        public ICommand ClickLogout
        {
            get { return new DelegateCommand(param => CommandLogout()); }
        }

        public SettingViewModel(TcpClientHelper tcpClient)
        {
            this.tcpClient = tcpClient;
            tcpClient.ExecuteMessage = ResponseMessage;
        }

        public void CommandCreateChatting()
        {
            tcpClient.RequestCreateChatting();
        }

        public void CommandJoinChatting()
        {
            tcpClient.RequestJoinChatting(TecherNickName);
        }

        public void CommandLogout()
        {
            tcpClient.RequestLogout();
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
            tcpClient.User.IsTeacher = true;

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                ChattingWindow chattingWindow = new ChattingWindow();
                chattingWindow.ShowDialog();

                // Close();
            });

        }

        private void JoinChatting(int check, int chatNumber)
        {
            switch (check)
            {
                case 0:
                    tcpClient.ChatNumber = chatNumber;

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ChattingWindow chattingWindow = new ChattingWindow();
                        chattingWindow.ShowDialog();

                        // Close();
                    });

                    break;

                case 1:
                    MessageBox.Show("개설되지 않은 채팅방입니다.", AppConst.AppName);
                    break;

                case 2:
                    MessageBox.Show("존재하지 않는 닉네임입니다.", AppConst.AppName);
                    break;
            }

        }

        private void Logout()
        {
            // Close();
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
