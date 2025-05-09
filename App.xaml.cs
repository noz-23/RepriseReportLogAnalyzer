﻿using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using System.Globalization;
using System.Windows;

namespace RepriseReportLogAnalyzer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// スタート
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _startup(object sender_, StartupEventArgs e_)
    {
        LogFile.Instance.Create();
        LogEventRegist.Instance.Create();
        AnalysisManager.Instance.Create();
        //SQLiteManager.Instance.Create();
    }
}

