/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using System.Windows;

namespace RepriseReportLogAnalyzer.Windows;

public delegate Task RunCallBack();

/// <summary>
/// WaitWindow.xaml の相互作用ロジック
/// </summary>
public partial class WaitWindow : Window
{
    public WaitWindow()
    {
        InitializeComponent();
        Run = null;
    }

    public RunCallBack? Run { get; set; }

    private async void _loaded(object sender, RoutedEventArgs e)
    {
        await Task.Run(async () =>
        {
            if (Run != null) { await Run.Invoke(); }

        });

        Close();
    }
}
