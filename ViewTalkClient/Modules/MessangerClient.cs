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

        private JsonHelper json;

        public UserData User { get; set; }
        public int ChatNumber { get; set; }

        public delegate void MessageDelegate(TcpMessage message);
        public MessageDelegate ExecuteMessage { get; set; }        

        public MessangerClient() : base(ServerIP, ServerPort)
        {
            this.json = new JsonHelper();
            this.User = new UserData();
        }

        public bool RequestLogin(string id, string password)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.Login;
            message.Message = json.SetLoginInfo(id, password);

            return SendMessage(message);
        }

        public void RequestLogout()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.Logout;
            message.UserNumber = User.Number;

            SendMessage(message);
        }

        public void RequestCreateChatting()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.CreateChatting;
            message.UserNumber = User.Number;

            SendMessage(message);

        }

        public void RequestJoinChatting(string nickname)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.JoinChatting;
            message.UserNumber = User.Number;
            message.Message = nickname;

            SendMessage(message);

        }

        public void RequestCloseChatting()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.CloseChatting;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            SendMessage(message);
        }

        public void RequestJoinUser()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.JoinUser;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            SendMessage(message);
        }

        public void RequestExistUser()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.ExitUser;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            SendMessage(message);
        }

        public void RequestSendChat(string chatMessage)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.SendChat ;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;
            message.Message = chatMessage;

            SendMessage(message);

        }

        public void RequestSendPPT(PPTData ppt)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.SendPPT;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;
            message.PPT = ppt;

            SendMessage(message);
        }

        public void RequestClosePPT()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.ClosePPT;
            message.UserNumber = User.Number;
            message.ChatNumber = ChatNumber;

            SendMessage(message);
        }

        public override void ResponseMessage(TcpMessage message)
        {
            ExecuteMessage(message);
        }
    }
}
