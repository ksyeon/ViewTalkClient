﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;
using ViewTalkClient.Services;

namespace ViewTalkClient.ViewModels
{
    public class ChattingViewModel : ViewModelBase
    {
        private MessangerClient messanger;

        public ObservableCollection<UserData> Users { get; set; }
        public ObservableCollection<ChatMessage> UserChat { get; set; }
        public ObservableCollection<ChatMessage> TeacherChat { get; set; }

        private PPTData _ppt;
        public PPTData PPT
        {
            get { return _ppt; }
            set { _ppt = value; RaisePropertyChanged("PPT"); }
        }

        private string _chatMessage;
        public string ChatMessage
        {
            get { return _chatMessage; }
            set { _chatMessage = value; RaisePropertyChanged("ChatMessage"); }
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

        public ICommand CloseCommand
        {
            get { return new RelayCommand(CloseWindow); }
        }

        public ChattingViewModel(IMessangerService MessangerService)
        {
            this.messanger = MessangerService.GetMessanger();
            messanger.ExecuteMessage = ResponseMessage;

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
                AddChatMessage(messanger.User.Number, ChatMessage);

                messanger.RequestSendChat(ChatMessage);

                ChatMessage = string.Empty;
            }
        }

        public void CommandOpenPPT()
        {
            if (messanger.User.IsTeacher)
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

                        if(bytePPT.Count > 0)
                        {
                            PPT.OpenPPT(bytePPT);

                            messanger.RequestSendPPT(PPT);
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

                    messanger.RequestClosePPT();
                }
            }
            else
            {
                MessageBox.Show("강사만 사용할 수 있습니다.", AppConst.AppName);
            }
        }

        public void CommandMovePPT(int direction)
        {
            if (messanger.User.IsTeacher)
            {
                switch (direction)
                {
                    case 0: // Left
                        if (PPT.CurrentPage > 1)
                        {
                            PPT.CurrentPPT = PPT.BytePPT[(--PPT.CurrentPage) - 1];

                            messanger.RequestSendPPT(PPT);
                        }
                        break;

                    case 1: // Right
                        if (PPT.CurrentPage < PPT.LastPage)
                        {
                            PPT.CurrentPPT = PPT.BytePPT[(++PPT.CurrentPage) - 1];

                            messanger.RequestSendPPT(PPT);
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("강사만 사용할 수 있습니다.", AppConst.AppName);
            }
        }

        public void InitializeChatting()
        {
            if (messanger.User.IsTeacher)
            {
                Users.Add(messanger.User);
            }
            else
            {
                messanger.RequestJoinUser();
            }
        }

        public void CloseWindow()
        {
            if (messanger.User.IsTeacher)
            {
                messanger.RequestCloseChatting();
            }
            else
            {
                messanger.RequestExistUser();
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
            MessageBox.Show("채팅방이 종료되었습니다.");

            // Close();
        }

        private void UpdateChatting(int chatNumbet, string message, PPTData ppt)
        {
            JsonHelper json = new JsonHelper();

            List<UserData> userList = json.GetChattingInfo(chatNumbet, message);
            Users = new ObservableCollection<UserData>(userList);

            LoadPPT(ppt);
        }

        private void AddUser(UserData user)
        {
            string notice = $"'{user.Nickname}'님이 입장하셨습니다.";

            App.Current.Dispatcher.InvokeAsync(() =>
            {
                Users.Add(user);

                UserChat.Add(new ChatMessage(true, user.Nickname, notice));
            });
        }

        private void DeleteUser(int userNumber)
        {
            UserData user = Users.First(x => (x.Number == userNumber)); // ArgumentNullException

            string notice = $"'{user.Nickname}'님이 퇴장하셨습니다.";

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
                UserData user = Users.First(x => (x.Number == userNumber)); // ArgumentNullException

                UserChat.Add(new ChatMessage(false, user.Nickname, message));

                if (user.IsTeacher)
                {
                    TeacherChat.Add(new ChatMessage(false, user.Nickname, message));
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
    }
}