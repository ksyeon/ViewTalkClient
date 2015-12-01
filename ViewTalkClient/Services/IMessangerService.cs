﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewTalkClient.Models;
using ViewTalkClient.Modules;

namespace ViewTalkClient.Services
{
    public interface IMessangerService
    {
        MessangerClient GetMessanger(Action<TcpMessage> excute);
    }
}
