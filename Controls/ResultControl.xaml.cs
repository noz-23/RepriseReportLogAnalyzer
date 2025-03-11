/*
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

        foreach (AnalysisGroup group in Enum.GetValues(typeof(AnalysisGroup)))
        {
            _comboBox.Items.Add(group.Description());
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
        SetDate();
    }

    /// <summary>
    /// カレンダーダブルクリック処理
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    //private void _mouseDoubleClick(object sender_, MouseButtonEventArgs e_)
    //{
    //    //_calendar.SelectedDate = null;
    //    _label.Content = "Selected : ";

    //    SetDate();
    //}

    /// <summary>
    /// グループ変更処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _selectionChanged(object sender_, SelectionChangedEventArgs e_)
    {
        LogFile.Instance.WriteLine($"Selected [{_comboBox.SelectedIndex}]");
        SetDate();
    }
    
    /// <summary>
    /// プロダクト表 チェック変更処理
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _sourceUpdated(object sender_, System.Windows.Data.DataTransferEventArgs e_)
    {
        LogFile.Instance.WriteLine($"DataGrid Updated");
        SetDate();
    }

    /// <summary>
    /// データ表示処理
    /// </summary>
    public void SetDate()
    {
        //var date = _calendar.SelectedDate;
        var date = _dataPicker.SelectedDate;
        var index = _comboBox.SelectedIndex;

        LogFile.Instance.WriteLine($"[{date}] [{index}]");

        AnalysisManager.Instance.SetData(date, (AnalysisGroup)index);

        //if (date == null)
        //{
        //    AnalysisManager.Instance.SetAllPlot(_scottPlot);
        //}
        //else
        //{
        //    AnalysisManager.Instance.SetDatePlot(_scottPlot, date ?? DateTime.Now, index);
        //}
        AnalysisManager.Instance.SetPlot(_scottPlot, date, (AnalysisGroup)index);

    }

}
