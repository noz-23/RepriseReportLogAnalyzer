using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Views
{
    internal class LicenseView :BaseView
    {
        public LicenseView()
        {
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => _SetValue(ref _isChecked, value);
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
            get => _name;
            set => _SetValue(ref _name, value);
        }
        public string _name = string.Empty;

        public int Count
        {
            get => _count;
            set => _SetValue(ref _count, value);
        }
        public int _count = 0;

        public int Max
        {
            get => _max;
            set => _SetValue(ref _max, value);
        }
        public int _max = 0;
        public TimeSpan Duration
        {
            get => _duration;
            set => _SetValue(ref _duration, value);
        }
        public TimeSpan _duration = TimeSpan.Zero;

    }
}
