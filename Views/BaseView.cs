using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Views
{
    public class BaseView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void _NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
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
        protected bool _SetValue<T>(ref T field_, T value_, [CallerMemberName] string propertyName_ = "")
        {
            if (EqualityComparer<T>.Default.Equals(field_, value_))
            {
                return false;
            }
            field_ = value_;
            _NotifyPropertyChanged(propertyName_);
            return true;
        }
    }
}
