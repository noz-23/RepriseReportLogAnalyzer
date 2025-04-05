/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using System.Reflection;
using System.Windows.Controls;

namespace RepriseReportLogAnalyzer.Controls;

/// <summary>
/// EventControl.xaml の相互作用ロジック
/// </summary>
public partial class EventControl : UserControl
{
    public EventControl()
    {
        InitializeComponent();

        _init();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void _init()
    {
        _comboBox.Items.Add(new OutputView(null, "NONE"));
        var _assembly = Assembly.GetExecutingAssembly();

        // Why を持っているイベントの抽出
        var tyepInNamespace = _assembly.GetTypes().Where(t_ => t_.IsClass).Distinct().OrderBy(t_ => t_.Sort());
        foreach (var t in tyepInNamespace)
        {
            if (t.GetInterfaces().Where(t_ => t_.Name == typeof(ILogEventWhy).Name).Any() == true)
            {
                _comboBox.Items.Add(new OutputView(t));
                LogFile.Instance.WriteLine($"{t.Name}");

            }
        }
        _comboBox.SelectedIndex = 0;

    }

    /// <summary>
    /// コンボボックス変更処理
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _selectionChanged(object sender_, SelectionChangedEventArgs e_)
    {
        if (_comboBox.SelectedValue is OutputView selected)
        {
            LogFile.Instance.WriteLine($"{selected.Name}");

            if (selected.ClassType == null)
            {
                return;
            }
            _dataGrid.ItemsSource = null;

            var listEvent = AnalysisManager.Instance.ListEvent(selected.ClassType);
            _dataGrid.ItemsSource = listEvent;

            LogFile.Instance.WriteLine($"{listEvent.Count()}");
        }
    }
}
