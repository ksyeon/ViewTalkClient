﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ViewTalkClient.Modules;

namespace ViewTalkClient
{
    public partial class App : Application
    {
        public static TcpClientHelper TcpClient = new TcpClientHelper();
    }
}