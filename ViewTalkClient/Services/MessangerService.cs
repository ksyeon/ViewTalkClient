using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;

namespace ViewTalkClient.Services
{
    public class MessangerService : IMessangerService
    {
        private MessangerClient messanger;

        public MessangerService()
        {
            messanger = new MessangerClient();
        }

        public MessangerClient GetMessanger(Action<TcpMessage> execute)
        {
            messanger.ExecuteMessage += execute;

            return messanger;
        }
    }
}
