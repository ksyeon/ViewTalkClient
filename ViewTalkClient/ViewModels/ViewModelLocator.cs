using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewTalkClient.Modules;

namespace ViewTalkClient.ViewModels
{
    public class ViewModelLocator
    {
        private TcpClientHelper tcpClient;

        public ViewModelLocator()
        {
            this.tcpClient = new TcpClientHelper();
        }

        private LoginViewModel _loginViewModel;
        public LoginViewModel LoginViewModel
        {
            get
            {
                return _loginViewModel ?? (_loginViewModel = new LoginViewModel(tcpClient));
            }
        }

        private SettingViewModel _settingViewModel;
        public SettingViewModel SettingViewModel
        {
            get
            {
                return _settingViewModel ?? (_settingViewModel = new SettingViewModel(tcpClient));
            }
        }

        private ChattingViewModel _chattingViewModel;
        public ChattingViewModel ChattingViewModel
        {
            get
            {
                return _chattingViewModel ?? (_chattingViewModel = new ChattingViewModel(tcpClient));
            }
        }
    }
}
