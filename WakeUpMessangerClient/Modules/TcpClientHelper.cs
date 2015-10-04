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

        private ulong userNumber;
        private MessageDelegate SetMessage;

        public delegate void MessageDelegate(MessageData message);

        public TcpClientHelper(ulong userNumber, MessageDelegate getMessage) : base(ServerIP, ServerPort)
        {
            this.userNumber = userNumber;
            this.SetMessage = getMessage;
        }

        public override MessageData GetConnectInfo()
        {
            MessageData sendMessage = new MessageData();
            sendMessage.Command = Command.Login;
            sendMessage.UserNumber = userNumber;

            return sendMessage;
        }

        public override void CheckMessage(MessageData message)
        {
            SetMessage(message);
        }
    }
}
