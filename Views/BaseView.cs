/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Views
{
    /// <summary>
    /// INotifyPropertyChangedの処理をまとめたクラス
    /// </summary>
    public class BaseView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティの変更通知を一時無効化
        /// </summary>
        public bool SuspendedPropertyChanged { get; set; } = false;
        /// <summary>
        /// プロパティの変更を通知する
        /// </summary>
        /// <param name="propertyName_"></param>
        protected void _NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            if (SuspendedPropertyChanged == true)
            {
                return;
            }

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
