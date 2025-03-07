/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Microsoft.Win32;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Controls;

/// <summary>
/// OutputControl.xaml の相互作用ロジック
/// </summary>
public partial class OutputControl : UserControl
{
    public OutputControl()
    {
        InitializeComponent();

    }

    private const string _CLASS_NAME = "LogEvent";

    public ObservableCollection<OutputView> ListEvent { get;  set; } = new();

    /// <summary>
    /// 出力フォルダ 選択
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _selectClick(object sender_, RoutedEventArgs e_)
    {
        var dlg = new OpenFolderDialog()
        {
            Title = "Please Select Output Folder",
            Multiselect = false
        };
        if (dlg.ShowDialog() == true)
        {
            _textBoxFolder.Text = dlg.FolderName;
        }
    }

    /// <summary>
    /// 出力 開始
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _outputClick(object sender_, RoutedEventArgs e_)
    {
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == false)
        {
            _textBoxFolder.IsEnabled = false;
            //AnalysisManager.Instance.WriteText(_textBoxFolder.Text);

            foreach (var view in ListEvent)
            {
                //AnalysisManager.Instance.WriteText
                if (view.IsChecked == false)
                {
                    continue;
                }
                AnalysisManager.Instance.WriteText(_textBoxFolder.Text + @"\" + view.Name + @".csv", view.ClassType);


            }

            _textBoxFolder.IsEnabled = true;
        }

    }

    private void _loaded(object sender_, RoutedEventArgs e_)
    {
        var _assembly = Assembly.GetExecutingAssembly();

        var tyepInNamespace = _assembly.GetTypes().Where(t_ => t_.IsClass && t_.Namespace == "RepriseReportLogAnalyzer.Events").Distinct().OrderBy(t_ => (Attribute.GetCustomAttribute(t_, typeof(SortAttribute)) as SortAttribute)?.Sort);
        foreach (var t in tyepInNamespace)
        {
            if (t.Name.Contains("LogEvent") == true)
            {
                var find = ListEvent.Where( x_=>x_.ClassType.Name ==t.Name);
                if (find.Count()==0)
                {
                    ListEvent.Add(new(t, t.Name.Replace(_CLASS_NAME, string.Empty)));

                    LogFile.Instance.WriteLine($"{t.Name}");

                }

            }
        }
    }

    //private class CompareOutputView : IEqualityComparer<OutputView>
    //{
    //    public bool Equals(OutputView? a_, OutputView? b_)
    //    {
    //        if (a_ == null)
    //        {
    //            return false;
    //        }
    //        if (b_ == null)
    //        {
    //            return false;
    //        }

    //        if (a_.Name != b_.Name)
    //        {
    //            return false;
    //        }

    //        return true;
    //    }
    //    public int GetHashCode(OutputView codeh_)
    //    {
    //        return codeh_.Name.GetHashCode() ^ codeh_.Name.GetHashCode();
    //    }
    //}
}
