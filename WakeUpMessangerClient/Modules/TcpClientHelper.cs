using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using WakeUpMessangerClient.Models;

namespace WakeUpMessangerClient.Modules
{
    class TcpClientHelper : TcpClient
    {
        private const string ServerIP = "127.0.0.1";
        private const int ServerPort = 8080;

        private int userNumber;
        private MessageDelegate SetMessage;

        public delegate void MessageDelegate(MessageData message);

        public TcpClientHelper(int userNumber, MessageDelegate getMessage) : base(ServerIP, ServerPort)
        {
            this.userNumber = userNumber;
            this.SetMessage = getMessage;
        }

        public override MessageData GetConnectMessage()
        {
            MessageData sendMessage = new MessageData();

            sendMessage.Command = Command.Login;
            
            // ID, Password Json 후 메시지에 저장

            return sendMessage;
        }

        public void SendChattingMessage(string chattingMessage)
        {
            MessageData sendMessage = new MessageData();

            sendMessage.Command = Command.Message;
            sendMessage.UserNumber = userNumber;
            sendMessage.Message = chattingMessage;

            SendMessage(sendMessage);
        }

        public override void CheckMessage(MessageData message)
        {
            SetMessage(message);
        }
    }
}
