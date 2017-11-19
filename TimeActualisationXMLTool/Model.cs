using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace TimeActualisationXMLTool
{
    class Model : INotifyPropertyChanged
    {
        private string _path; //a folder that must must be processed
        private DateTime _datetime; //datetime to filter files

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        public DateTime DateTime
        {
            get { return _datetime; }
            set
            {
                _datetime = value;
                OnPropertyChanged("DateTime");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
