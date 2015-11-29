using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewTalkClient.Models
{
    public class ChatMessage
    {
        public bool IsNotice { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }

        public ChatMessage(bool isNotice, string nickname, string message)
        {
            this.IsNotice = isNotice;
            this.Nickname = nickname;
            this.Message = message;
        }
    }
}
