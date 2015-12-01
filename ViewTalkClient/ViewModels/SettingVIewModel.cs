using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;
using ViewTalkClient.Services;

namespace ViewTalkClient.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private MessangerClient messanger;

        private string _teacherNickname;
        public string TecherNickname
        {
            get { return _teacherNickname; }
            set { _teacherNickname = value; RaisePropertyChanged("TecherNickName"); }
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

        public SettingViewModel(IMessangerService MessangerService)
        {
            this.messanger = MessangerService.GetMessanger();
            messanger.ExecuteMessage = ResponseMessage;
        }

        public void CommandCreateChatting()
        {
            messanger.RequestCreateChatting();
        }

        public void CommandJoinChatting()
        {
            if(string.IsNullOrEmpty(TecherNickname))
            {
                MessageBox.Show("강사의 닉네임을 입력하세요.", AppConst.AppName);
            }
            else
            {
                messanger.RequestJoinChatting(TecherNickname);
            }
        }

        public void CommandLogout()
        {
            messanger.RequestLogout();
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
            messanger.ChatNumber = messanger.User.Number;
            messanger.User.IsTeacher = true;

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
                    messanger.ChatNumber = chatNumber;

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ChattingWindow chattingWindow = new ChattingWindow();
                        chattingWindow.ShowDialog();

                        // Close();
                    });

                    TecherNickname = string.Empty;

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

        private void Logout()
        {
            // Close();
        }
    }
}
