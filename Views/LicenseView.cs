using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Views
{
    class LicenseView : INotifyPropertyChanged
    {
        public LicenseView()
        {
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => _setValue(ref _isChecked, value);
        }
        private bool _isChecked = true;

        //public DateTime Date
        //{
        //    get => _date;
        //    set => _setValue(ref _date, value);
        //}
        //private DateTime _date = DateTime.Now;

        public string Name 
        { 
            get=> _name; 
            set=> _setValue(ref _name,value); 
        }
        public string _name = string.Empty;

        public int Count
        {
            get => _count;
            set => _setValue(ref _count, value);
        }
        public int _count = 0;

        public int Max
        {
            get => _max;
            set => _setValue(ref _max, value);
        }
        public int _max = 0;
        public TimeSpan Duration 
        {
            get => _duration;
            set => _setValue(ref _duration, value);
        }
        public TimeSpan _duration = TimeSpan.Zero;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void _notifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// プロパティの値を設定する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field_"></param>
        /// <param name="value_"></param>
        /// <param name="propertyName_"></param>
        /// <returns></returns>
        private bool _setValue<T>(ref T field_, T value_, [CallerMemberName] string propertyName_ = "")
        {
            if (EqualityComparer<T>.Default.Equals(field_, value_))
            {
                return false;
            }
            field_ = value_;
            _notifyPropertyChanged(propertyName_);
            return true;
        }
    }
}
