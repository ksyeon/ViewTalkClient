using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using WakeUpMessangerClient.Modules;

namespace WakeUpMessangerClient
{
    public partial class App : Application
    {
        public static TcpClientHelper TcpClient = new TcpClientHelper();
    }
}
