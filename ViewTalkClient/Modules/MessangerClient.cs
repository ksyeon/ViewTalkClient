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
        private const string ServerIP = "183.109.83.66";
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

        public bool RequestLogout(int userNumbr)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.Logout;
            message.UserNumber = userNumbr;

            return SendMessage(message);
        }

        public bool RequestCreateChatting(int userNumbr)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.CreateChatting;
            message.UserNumber = userNumbr;

            return SendMessage(message);

        }

        public bool RequestJoinChatting(int userNumbr, string nickname)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.JoinChatting;
            message.UserNumber = userNumbr;
            message.Message = nickname;

            return SendMessage(message);

        }

        public bool RequestCloseChatting(int userNumbr, int chatNumber)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.CloseChatting;
            message.UserNumber = userNumbr;
            message.ChatNumber = chatNumber;

            return SendMessage(message);
        }

        public bool RequestJoinUser(int userNumbr, int chatNumber)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.JoinUser;
            message.UserNumber = userNumbr;
            message.ChatNumber = chatNumber;

            return SendMessage(message);
        }

        public bool RequestExistUser(int userNumbr, int chatNumber)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.ExitUser;
            message.UserNumber = userNumbr;
            message.ChatNumber = chatNumber;

            return SendMessage(message);
        }

        public bool RequestSendChat(int userNumbr, int chatNumber, string chatMessage)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.SendChat ;
            message.UserNumber = userNumbr;
            message.ChatNumber = chatNumber;
            message.Message = chatMessage;

            return SendMessage(message);

        }

        public bool RequestSendPPT(int userNumbr, int chatNumber, PPTData ppt)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.SendPPT;
            message.UserNumber = userNumbr;
            message.ChatNumber = chatNumber;
            message.PPT = ppt;

            return SendMessage(message);
        }

        public bool RequestClosePPT(int userNumbr, int chatNumber)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.ClosePPT;
            message.UserNumber = userNumbr;
            message.ChatNumber = chatNumber;

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
