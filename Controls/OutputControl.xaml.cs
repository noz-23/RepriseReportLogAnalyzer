/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Microsoft.Win32;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace RepriseReportLogAnalyzer.Controls;

/// <summary>
/// OutputControl.xaml の相互作用ロジック
/// </summary>
public partial class OutputControl : UserControl
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OutputControl()
    {
        InitializeComponent();
        _init();
    }

    /// <summary>
    /// ログ イベント クラスの先頭文字
    /// </summary>
    private const string _CLASS_NAME_EVENT = "LogEvent";
    private const string _CLASS_NAME_ANALYSIS = "ListAnalysis";

    private const string _NAME_SPACE_EVENT = "RepriseReportLogAnalyzer.Events";
    private const string _NAME_SPACE_ANALYSES = "RepriseReportLogAnalyzer.Analyses";

    public List<OutputView> ListEvent { get; private set; } = new();
    public List<OutputView> ListAnalysis { get;private set; } = new();

    /// <summary>
    /// 出力フォルダ 選択
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _selectClick(object sender_, RoutedEventArgs e_) 
    {
        _selectOutFolder();
    }

    private bool _selectOutFolder()
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

        return (string.IsNullOrEmpty(_textBoxFolder.Text));
    }

    /// <summary>
    /// Csv 出力 開始
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private async void _saveCsvClick(object sender_, RoutedEventArgs e_)
    {
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == false)
        {
            if (_selectOutFolder()==true) { return; }
        }

        _saveCsv.IsEnabled = false;
        await Task.Run(() =>
        {

            foreach (var view in ListEvent)
            {
                if (view.IsChecked == false)
                {
                    continue;
                }
                string output = _textBoxFolder.Text + @"\" + view.Name + @".csv";
                LogFile.Instance.WriteLine($"Write : {output}");

                AnalysisManager.Instance.WriteText(output, view.ClassType);
            }

            foreach (var view in ListAnalysis)
            {
                if (view.IsChecked == false)
                {
                    continue;
                }

                string output = _textBoxFolder.Text + @"\" + view.Name + @".csv";
                LogFile.Instance.WriteLine($"Write : {output}");

                AnalysisManager.Instance.WriteText(output, view.ClassType, view.SelectedValue);
            }
        });

        _saveCsv.IsEnabled = true;

    }

    private async void _saveSqliteClick(object sender_, RoutedEventArgs e_)
    {
        LogFile.Instance.WriteLine($"Write : {sender_}");
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == false)
        {
            if (_selectOutFolder() == true) { return; }
        }

        _saveSql.IsEnabled = false;
        await Task.Run(() =>
        {
            _textBoxFolder.IsEnabled = false;
            string output = _textBoxFolder.Text + @"\ReportLog.db";
            LogFile.Instance.WriteLine($"Write : {output}");

            var sql = new SQLiteManager(output);

            foreach (var view in ListEvent)
            {
                if (view.IsChecked == false)
                {
                    continue;
                }
                sql.Create(view.ClassType);
                //if (view.ClassType==typeof(LogEventStart)) 
                //{
                //    sql.Insert(AnalysisManager.Instance.ListStart);

                //}
                //sql.Insert(view.ClassType,AnalysisManager.Instance.ListEvent(view.ClassType).ToList());
                //sql.Insert(AnalysisManager.Instance.ListEvent(view.ClassType).ToList());

                sql.Insert(view.ClassType, ToDataBase.Header( view.ClassType), AnalysisManager.Instance.ListEventValue(view.ClassType));
            }
            foreach (var view in ListAnalysis)
            {
                if (view.IsChecked == false)
                {
                    continue;
                }
                sql.Create(view.ClassType, AnalysisManager.Instance.ListEventHeader(view.ClassType, view.SelectedValue));
                sql.Insert(view.ClassType, AnalysisManager.Instance.EventHeader(view.ClassType, view.SelectedValue), AnalysisManager.Instance.ListEventValue(view.ClassType, view.SelectedValue));


            }
            sql.Close();

        });
        _saveSql.IsEnabled = true;


    }
    private void _init()
    { 
        var _assembly = Assembly.GetExecutingAssembly();

        var tyepInNamespace = _assembly.GetTypes().Where(t_ => t_.IsClass).Distinct().OrderBy(t_ => (Attribute.GetCustomAttribute(t_, typeof(SortAttribute)) as SortAttribute)?.Sort);
        foreach (var t in tyepInNamespace)
        {
            if (t.Namespace == _NAME_SPACE_EVENT)
            {
                if (t.IsSubclassOf(typeof(LogEventBase)) == true)
                {
                    var find = ListEvent.Where(x_ => x_.ClassType.Name == t.Name);
                    if (find.Count() == 0)
                    {
                        ListEvent.Add(new(t, t.Name.Replace(_CLASS_NAME_EVENT, string.Empty)));
                        LogFile.Instance.WriteLine($"{t.Name}");
                    }

                }
            }

            if (t.Namespace == _NAME_SPACE_ANALYSES)
            {
                //if (t.GetInterfaces().Where(t_=>t_.IsConstructedGenericType ==true && t_.GetGenericTypeDefinition() ==typeof(IAnalysisOutputFile)).Count()>0)
                //var list = t.GetInterfaces();
                //if(list.Count()>0)
                //{
                if (t.GetInterfaces().Where(t_ => t_.Name == typeof(IAnalysisOutputFile).Name).Count() > 0)
                {
                    var find = ListAnalysis.Where(x_ => x_.ClassType.Name == t.Name);
                    if (find.Count() == 0)
                    {
                        var listPropetyInfo = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static );

                        foreach (var p in listPropetyInfo)
                        {
                            if (p.GetValue(null) is ListStringLongPair list)
                            {
                                ListAnalysis.Add(new(t, t.Name.Replace(_CLASS_NAME_ANALYSIS, string.Empty),list));
                                LogFile.Instance.WriteLine($"{t.Name}");
                                break;
                            }
                        }
                    }
                }
                //}
            }
        }  
    }

    private void _loaded(object sender, RoutedEventArgs e)
    {
        foreach(var v in ListAnalysis)
        {
            v.SelectedIndex = 0;
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
