/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepriseReportLogAnalyzer.Views
{
    public class OutputView:BaseView
    {

        public OutputView(Type type_, string name_="", ListStringLongPair? list_=null)
        {
            Name =(string.IsNullOrEmpty(name_)==true) ? type_.Name:name_;
            ClassType=type_;

            list_?.ToList().ForEach(x_ => ListSelect.Add(x_));

            _NotifyPropertyChanged("SelectedIndex");
        }


        public bool IsChecked
        {
            get => _isChecked;
            set => _SetValue(ref _isChecked, value);
        }
        private bool _isChecked = false;

        public string Name
        {
            get => _name;
            set => _SetValue(ref _name, value);
        }
        public string _name = string.Empty;

        public Type ClassType { get; private set; }


        public ListStringLongPair ListSelect { get; private set; } = new();

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => _SetValue(ref _selectedIndex, value);
        }
        private int _selectedIndex = -1;

        public long SelectedValue
        {
            get => _selectedValue;
            set => _SetValue(ref _selectedValue, value);
        }
        private long _selectedValue = -1;

    }
}
