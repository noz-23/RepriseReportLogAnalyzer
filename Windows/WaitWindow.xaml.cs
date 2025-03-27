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

public delegate Task RunDelegate();

/// <summary>
/// WaitWindow.xaml の相互作用ロジック
/// </summary>
public partial class WaitWindow : Window
{
    public RunDelegate? Run = null;

    public WaitWindow()
    {
        InitializeComponent();
    }

    private async void _loaded(object sender, RoutedEventArgs e)
    {
        await Task.Run( async() => {await Run?.Invoke(); });

        Close();
    }
}
