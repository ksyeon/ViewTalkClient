using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewTalkClient.Models;

namespace ViewTalkClient.Modules
{
    public class MessangerClient : TcpClient
    {
        private const string ServerIP = "127.0.0.1";
        private const int ServerPort = 8080;

        public Action<TcpMessage> ExcuteMessage { get; set; }

        public UserData User { get; set; }
        public int ChatNumber { get; set; }

        public MessangerClient() : base(ServerIP, ServerPort)
        {
            ExcuteMessage = null;

            User = new UserData();
            ChatNumber = 0;
        }

        public bool RequestLogin(string id, string password)
        {
            TcpMessage message = new TcpMessage();
            JsonHelper json = new JsonHelper();

            message.Command = Command.Login;
            message.Message = json.SetLoginInfo(id, password);

            return SendMessage(message);
        }

        public bool RequestLogout()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.Logout;
            message.UserNumber = User.Number;

            return SendMessage(message);
        }

        public bool RequestCreateChatting()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.CreateChatting;
            message.UserNumber = User.Number;

            return SendMessage(message);

        }

        public bool RequestJoinChatting(string nickname)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.JoinChatting;
            message.UserNumber = User.Number;
            message.Message = nickname;

            return SendMessage(message);

        }

        public bool RequestCloseChatting()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.CloseChatting;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            return SendMessage(message);
        }

        public bool RequestJoinUser()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.JoinUser;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            return SendMessage(message);
        }

        public bool RequestExistUser()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.ExitUser;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            return SendMessage(message);
        }

        public bool RequestSendChat(string chatMessage)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.SendChat ;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;
            message.Message = chatMessage;

            return SendMessage(message);

        }

        public bool RequestSendPPT(PPTData ppt)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.SendPPT;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;
            message.PPT = ppt;

            return SendMessage(message);
        }

        public bool RequestClosePPT()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.ClosePPT;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            return SendMessage(message);
        }

        public override void ResponseMessage(TcpMessage message)
        {
            if (ExcuteMessage != null)
            {
                ExcuteMessage(message);
            }
        }
    }
}
