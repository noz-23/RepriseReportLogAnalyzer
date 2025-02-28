﻿using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RepriseReportLogAnalyzer.Controls
{
    /// <summary>
    /// ResultControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ResultControl : UserControl
    {
        public ResultControl()
        {
            InitializeComponent();

            foreach (ANALYSIS_GROUP group in Enum.GetValues(typeof(ANALYSIS_GROUP)))
            {
                _comboBox.Items.Add(group.Description());
            }
        }


        private void _calendarSelected(object sender_, System.Windows.Controls.SelectionChangedEventArgs e_)
        {
            //if( sender_ is Calendar calendar )
            //{
            _label.Content = "Select:"+(_calendar.SelectedDate.ToString());

            LogFile.Instance.WriteLine($"Selected {_calendar.SelectedDate}");
                AnalysisManager.Instance.SetDate(_calendar.SelectedDate, _comboBox.SelectedIndex);
            //}
        }

        //public void CalendarShow(IEnumerable<DateTime> list_)
        //{
        //    App.Current.Dispatcher.Invoke(() =>
        //    {
        //        _calendar.DisplayDateStart = list_.First();
        //        _calendar.DisplayDateEnd = list_.Last();

        //        //_calendar.SelectedDates.Clear();
        //        //list_.ToList().ForEach(date_ => _calendar.SelectedDates.Add(date_));
        //    });

        //}

        private void _mouseDoubleClick(object sender_, MouseButtonEventArgs e_)
        {
            _calendar.SelectedDate = null;
            _label.Content = "Select:";
        }
    }
}
