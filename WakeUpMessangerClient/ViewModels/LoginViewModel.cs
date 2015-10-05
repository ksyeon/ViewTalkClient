using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls; // PasswordBox 추가

namespace WakeUpMessangerClient.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _id;
        public string ID
        {
            get { return _id; }
            set { _id = value; OnNotifyPropertyChanged("ID"); }
        }

        public ICommand ClickLogin
        {
            get { return new DelegateCommand2(Login); }
        }

        public ICommand ClickJoin
        {
            get { return new DelegateCommand(Join); }
        }

        public LoginViewModel()
        {
            this.ID = string.Empty;
        }

        private void Login(string password)
        {
            if (string.IsNullOrEmpty(ID))
            {
                MessageBox.Show("아이디를 입력하세요.", "WakeUp! Messanger");
            }
            else if(string.IsNullOrEmpty(password))
            {
                MessageBox.Show( "비밀번호를 입력하세요.", "WakeUp! Messanger");
            }
            else
            {

            }
        }

        private void Join()
        {
            JoinWindow joinWindow = new JoinWindow();
            joinWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class DelegateCommand2 : ICommand
    {
        private readonly Action<string> _action;

        public DelegateCommand2(Action<string> action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox.Password;

            _action(password);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
