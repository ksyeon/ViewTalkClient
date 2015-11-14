﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using ViewTalkClient.Models;

namespace ViewTalkClient.Modules
{
    public abstract class TcpClient
    {
        private Socket clientSocket;
        private string serverIP;
        private int serverPort;

        private byte[] byteData;

        public TcpClient(string serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;

            this.byteData = new byte[1024];

            ConnectSocket();
        }

        public void ConnectSocket()
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

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                /* [4] Receive */
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void SendMessage(MessageData message)
        {
            try
            {
                byte[] byteMessage = message.ToByteData();

                /* [3] Send */
                clientSocket.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnSend(IAsyncResult ar)
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

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndReceive(ar);

                MessageData receiveMessage = new MessageData(byteData);
                CheckMessage(receiveMessage);

                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CloseSocket()
        {
            try
            {
                /* [5] Close */
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public abstract void CheckMessage(MessageData receiveMessage);
    }
}