using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeUpMessangerClient.Models
{
    class ChattingData
    {
        public ulong UserNumber { get; set; }
        public string Message { get; set; }

        public ChattingData(ulong userNumber, string message)
        {
            this.UserNumber = userNumber;
            this.Message = message;
        }
    }
}
