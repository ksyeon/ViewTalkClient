using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls; // PasswordBox 추가

using ViewTalkClient.Models;

namespace ViewTalkClient.ViewModels
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

        public LoginViewModel()
        {
            this.ID = string.Empty;

            App.TcpClient.ExecuteMessage = ExecuteMessage;
        }

        public void ExecuteMessage(MessageData message)
        {
            switch (message.Command)
            {
                case Command.Login:
                    bool isExistUser = Convert.ToBoolean(message.Auth);

                    if (isExistUser)
                    {
                        App.TcpClient.UserNumber = message.Number;
                        App.TcpClient.ExecuteMessage = null;

                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.ShowDialog();
                        });

                        // Close();
                    }
                    else
                    {
                        MessageBox.Show("아이디/비밀번호가 일치하지 않습니다.", "WakeUp! Messanger");
                    }

                    break;
            }
        }

        private void Login(string password)
        {
            if (string.IsNullOrEmpty(ID))
            {
                MessageBox.Show("아이디를 입력하세요.", "WakeUp! Messanger");
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("비밀번호를 입력하세요.", "WakeUp! Messanger");
            }
            else
            {
                App.TcpClient.SendLogin(ID, password); // 일정 시간 후 연결 종료(MessageBox 띄우기)
            }
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
