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
        public string UserNickname { get; set; }
        public string Message { get; set; }

        public ChatMessage(bool isNotice, string UserNickname, string message)
        {
            this.IsNotice = isNotice;
            this.UserNickname = UserNickname;
            this.Message = message;
        }
    }
}
