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

/// <summary>
/// Wiatで実行する処理のデリゲート
/// </summary>
/// <returns></returns>
public delegate Task RunCallBack();

/// <summary>
/// WaitWindow.xaml の相互作用ロジック
/// </summary>
public partial class WaitWindow : Window
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public WaitWindow()
    {
        InitializeComponent();
        Run = null;
    }

    /// <summary>
    /// 実行する処理
    /// </summary>
    public RunCallBack? Run { get; set; }

    /// <summary>
    /// ロード処理
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private async void _loaded(object sender_, RoutedEventArgs e_)
    {
        // 実行して
        await Task.Run(async () =>
        {
            if (Run != null) { await Run.Invoke(); }

        });
        // 閉じる
        Close();
    }
}
