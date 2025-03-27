/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
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

    private const string _NAME_SPACE_EVENT = "RepriseReportLogAnalyzer.Events";

    private void _init()
    {
        _comboBox.Items.Add(new OutputView(null, "NONE"));
        var _assembly = Assembly.GetExecutingAssembly();

        // Why を持っているイベントの抽出
        var tyepInNamespace = _assembly.GetTypes().Where(t_ => t_.IsClass && t_.Namespace == _NAME_SPACE_EVENT).Distinct().OrderBy(t_ => t_.GetAttribute<SortAttribute>()?.Sort);
        foreach (var t in tyepInNamespace)
        {
            if (t.GetInterfaces().Where(t_ => t_.Name == typeof(ILogEventWhy).Name).Count() > 0)
            {
                _comboBox.Items.Add(new OutputView(t));
                LogFile.Instance.WriteLine($"{t.Name}");

            }
        }
        _comboBox.SelectedIndex = 0;

    }

    private void _selectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_comboBox.SelectedValue is OutputView selected)
        {
            LogFile.Instance.WriteLine($"{selected.Name}");

            if (selected.ClassType == null)
            {
                return;
            }
            _dataGrid.ItemsSource = null;

            var listEvent =AnalysisManager.Instance.ListEvent(selected.ClassType);
            _dataGrid.ItemsSource = listEvent;

            LogFile.Instance.WriteLine($"{listEvent.Count()}");
        }
    }
}
