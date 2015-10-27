using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeUpMessangerClient.Models
{
    class ChattingData
    {
        public int UserNumber { get; set; }
        public string Message { get; set; }

        public ChattingData(int userNumber, string message)
        {
            this.UserNumber = userNumber;
            this.Message = message;
        }
    }
}
