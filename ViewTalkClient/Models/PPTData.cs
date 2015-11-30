using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace ViewTalkClient.Models
{
    public class PPTData : INotifyPropertyChanged
    {
        public List<byte[]> BytePPT { get; set; }

        private byte[] _currentPPT;
        public byte[] CurrentPPT
        {
            get { return _currentPPT; }
            set { _currentPPT = value; OnNotifyPropertyChanged("CurrentPPT"); }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnNotifyPropertyChanged("CurrentPage"); }
        }

        private int _lastPage;
        public int LastPage
        {
            get { return _lastPage; }
            set { _lastPage = value; OnNotifyPropertyChanged("LastPage"); }
        }

        public PPTData()
        {
            ResetPPT();
        }

        public void OpenPPT(List<byte[]> bytePPT)
        {
            BytePPT = bytePPT;
            CurrentPPT = bytePPT[0];
            CurrentPage = 1;
            LastPage = bytePPT.Count;
        }

        public void LoadPPT(List<byte[]> bytePPT, int currentPage)
        {
            BytePPT = bytePPT;
            CurrentPPT = bytePPT[currentPage - 1];
            CurrentPage = currentPage;
            LastPage = bytePPT.Count;
        }

        public void ResetPPT()
        {
            BytePPT = new List<byte[]>();
            CurrentPPT = new byte[0];
            CurrentPage = 0;
            LastPage = 0;
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
}
