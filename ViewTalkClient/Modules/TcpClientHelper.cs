using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewTalkClient.Models;

namespace ViewTalkClient.Modules
{
    public class TcpClientHelper : TcpClient
    {
        private const string ServerIP = "127.0.0.1";
        private const int ServerPort = 8080;

        public int UserNumber { get; set; }

        public delegate void MessageDelegate(TcpMessage message);
        public MessageDelegate ExecuteMessage { get; set; }        

        public TcpClientHelper() : base(ServerIP, ServerPort)
        {

        }

        public void RequestLogin(string id, string password)
        {
            TcpMessage message = new TcpMessage();

            JsonHelper json = new JsonHelper();

            message.Command = Command.Login;
            message.Message = json.GetLoginInfo(id, password);

            SendMessage(message);
        }

        public void RequestConnect()
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.Connect;
            message.Number = UserNumber;

            SendMessage(message);
        }

        public void RequestChatting(string chatting)
        {
            TcpMessage message = new TcpMessage();

            message.Command = Command.Message;
            message.Number = UserNumber;
            message.Message = chatting;

            SendMessage(message);
        }

        public override void ResponseMessage(TcpMessage message)
        {
            ExecuteMessage(message);
        }
    }
}
