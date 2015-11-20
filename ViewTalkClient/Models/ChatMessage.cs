using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewTalkClient.Models
{
    public class ChatMessage
    {
        public int UserNumber { get; set; }
        public string Message { get; set; }

        public ChatMessage(int userNumber, string message)
        {
            this.UserNumber = userNumber;
            this.Message = message;
        }
    }
}
