﻿/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Windows;
using System.Windows;
using System.Windows.Controls;

namespace RepriseReportLogAnalyzer.Controls;

/// <summary>
/// ResultControl.xaml の相互作用ロジック
/// </summary>
public partial class ResultControl : UserControl
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ResultControl()
    {
        InitializeComponent();

        //foreach (AnalysisGroup group in Enum.GetValues(typeof(AnalysisGroup)))
        foreach (AnalysisGroup group in Enum.GetValues<AnalysisGroup>())
        {
            var item = group.Description();
            LogFile.Instance.WriteLine($"Selected [{item}]");
            _comboBox.Items.Add(item);
        }
        _comboBox.SelectedIndex = 0;
    }

    /// <summary>
    /// カレンダー選択時
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _datePickerSelected(object sender_, System.Windows.Controls.SelectionChangedEventArgs e_)
    {
        LogFile.Instance.WriteLine($"Selected [{_dataPicker.SelectedDate}]");
        if (AnalysisManager.Instance.StartDate == null)
        {
            return;
        }

        var win = new WaitWindow()
        {
            Run = async () => await SetDate(),
            Owner = Application.Current.MainWindow
        };
        win.ShowDialog();
        //Task.Run(async () => await SetDate());
    }

    /// <summary>
    /// グループ変更処理
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _selectionChanged(object sender_, SelectionChangedEventArgs e_)
    {
        LogFile.Instance.WriteLine($"Selected [{_comboBox.SelectedIndex}]");
        if (AnalysisManager.Instance.StartDate == null)
        {
            return;
        }

        var win = new WaitWindow()
        {
            Run = async () => await SetDate(),
            Owner = Application.Current.MainWindow
        };
        win.ShowDialog();

        //Task.Run(async () => await SetDate());
    }

    /// <summary>
    /// プロダクト表 チェック変更処理
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _sourceUpdated(object sender_, System.Windows.Data.DataTransferEventArgs e_)
    {
        LogFile.Instance.WriteLine($"DataGrid Updated");
        if (AnalysisManager.Instance.StartDate == null)
        {
            return;
        }

        var win = new WaitWindow()
        {
            Run = async () => await SetDate(),
            Owner = Application.Current.MainWindow
        };
        win.ShowDialog();
        //Task.Run(async () => await SetDate());
    }

    /// <summary>
    /// データ表示処理
    /// </summary>
    public async Task SetDate()
    {
        await App.Current.Dispatcher.Invoke(async () =>
        {
            var date = _dataPicker.SelectedDate;
            var index = _comboBox.SelectedIndex;

            LogFile.Instance.WriteLine($"[{date}] [{index}]");

            await AnalysisManager.Instance.SetData(date, (AnalysisGroup)index);
            await AnalysisManager.Instance.SetPlot(_scottPlot, date, (AnalysisGroup)index);
        });
    }
}
