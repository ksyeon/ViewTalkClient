using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

using ViewTalkClient.Services;

namespace ViewTalkClient.ViewModels
{
    public class ViewModelLocator
    {
        private static string _currentKey = System.Guid.NewGuid().ToString();
        public static string CurrentKey
        {
            get { return _currentKey; }
            private set { _currentKey = value; }
        }

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<IMessangerService, MessangerService>();

            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
            SimpleIoc.Default.Register<ChattingViewModel>();
        }

        public LoginViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>(Guid.NewGuid().ToString());
            }
        }

        public SettingViewModel Setting
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingViewModel>(Guid.NewGuid().ToString());
            }
        }

        public ChattingViewModel Chatting
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChattingViewModel>(Guid.NewGuid().ToString());
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}