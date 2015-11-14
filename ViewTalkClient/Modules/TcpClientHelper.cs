using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using ViewTalkClient.Models;

namespace ViewTalkClient.Modules
{
    public class TcpClientHelper : TcpClient
    {
        private const string ServerIP = "127.0.0.1";
        private const int ServerPort = 8080;

        public int UserNumber { get; set; }

        public delegate void MessageDelegate(MessageData message);
        public MessageDelegate ExecuteMessage { get; set; }        

        public TcpClientHelper() : base(ServerIP, ServerPort)
        {

        }

        public void SendLogin(string id, string password)
        {
            MessageData message = new MessageData();

            JsonHelper json = new JsonHelper();

            message.Command = Command.Login;
            message.Message = json.GetLoginInfo(id, password);

            SendMessage(message);
        }

        public void SendConnect()
        {
            MessageData message = new MessageData();

            message.Command = Command.Connect;
            message.Number = UserNumber;

            SendMessage(message);
        }

        public void SendChatting(string chatting)
        {
            MessageData message = new MessageData();

            message.Command = Command.Message;
            message.Number = UserNumber;
            message.Message = chatting;

            SendMessage(message);
        }

        public override void CheckMessage(MessageData message)
        {
            ExecuteMessage(message);
        }
    }
}
