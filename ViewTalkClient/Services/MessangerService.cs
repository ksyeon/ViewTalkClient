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

        public delegate void MessageDelegate(TcpMessage message);

        public MessangerService()
        {
            this.messanger = new MessangerClient();
        }

        public MessangerClient GetMessanger()
        {
            return messanger;
        }
    }
}
