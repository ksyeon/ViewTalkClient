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
    public abstract class TcpClient
    {
        protected Socket clientSocket;
        private string serverIP;
        private int serverPort;

        private byte[] byteData;

        public TcpClient(string serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;

            this.byteData = new byte[1024];

            Initialize();
        }

        protected void Initialize()
        {
            try
            {
                /* [1] Socket */
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* [2] Connect */
                IPAddress ipAddress = IPAddress.Parse(serverIP);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, serverPort);
                
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                MessageData sendMessage = GetConnectInfo();
                SendMessage(sendMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendMessage(MessageData message)
        {
            try
            {
                byte[] byteMessage = message.ToByteData();

                /* [3] Connect */
                clientSocket.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnSend(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnReceive(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndReceive(ar);

                MessageData receiveMessage = new MessageData(byteData);
                CheckMessage(receiveMessage);

                /* [4] Connect */
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void CloseSocket()
        {
            try
            {
                if (clientSocket != null)
                {
                    /* [5] Close */
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public abstract MessageData GetConnectInfo();

        public abstract void CheckMessage(MessageData receiveMessage);
    }
}
