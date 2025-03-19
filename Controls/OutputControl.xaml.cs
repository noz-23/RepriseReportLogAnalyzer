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

    /// <summary>
    /// 総合ファイルの出力
    /// </summary>
    public bool IsSaveSummy { get; set; } = false;

    public List<OutputView> ListEvent { get; private set; } = new();
    private Dictionary<string, bool> _listSavetEvent = new();

    public List<OutputView> ListAnalysis { get;private set; } = new();
    private Dictionary<string, bool> _listSavetAnalysis = new();

    /// <summary>
    /// 各イベント項目の追加
    /// </summary>
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
                        var view = new OutputView(t, t.Name.Replace(_CLASS_NAME_EVENT, string.Empty));
                        ListEvent.Add(view);

                        _listSavetEvent[view.Name] = view.IsChecked;
                        LogFile.Instance.WriteLine($"{t.Name}");
                    }
                }
            }

            if (t.Namespace == _NAME_SPACE_ANALYSES)
            {
                if (t.GetInterfaces().Where(t_ => t_.Name == typeof(IAnalysisOutputFile).Name).Count() > 0)
                {
                    var find = ListAnalysis.Where(x_ => x_.ClassType.Name == t.Name);
                    if (find.Count() == 0)
                    {
                        var listPropetyInfo = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

                        foreach (var p in listPropetyInfo)
                        {
                            if (p.GetValue(null) is ListStringLongPair list)
                            {
                                var view = new OutputView(t, t.Name.Replace(_CLASS_NAME_ANALYSIS, string.Empty), list);
                                ListAnalysis.Add(view);

                                _listSavetAnalysis[view.Name] = view.IsChecked;
                                LogFile.Instance.WriteLine($"{t.Name}");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void _loaded(object sender_, RoutedEventArgs e_)
    {
        foreach (var v in ListAnalysis)
        {
            // なんかうまくバインドされないため
            v.SelectedIndex = 0;
        }
    }

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
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == true)
        {
            if (_selectOutFolder()==true) { return; }
        }
        string outputFolder = _textBoxFolder.Text ;

        _saveCsv.IsEnabled = false;

        if (IsSaveSummy == true)
        {
            string outPath = outputFolder + @"\Summy.txt";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteSummy(outPath);
        }

        foreach (var view in ListEvent)
        {
            if (view.IsChecked == false)
            {
                continue;
            }
            var outPath = outputFolder + @"\" + view.Name + @".csv";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteText(outPath, view.ClassType);
        }

        foreach (var view in ListAnalysis)
        {
            if (view.IsChecked == false)
            {
                continue;
            }

            string outPath = outputFolder + @"\" + view.Name + @".csv";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteText(outPath, view.ClassType, view.SelectedValue);
        }
        _saveCsv.IsEnabled = true;

    }

    private async void _saveSqliteClick(object sender_, RoutedEventArgs e_)
    {
        LogFile.Instance.WriteLine($"Write : {sender_}");
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == true)
        {
            if (_selectOutFolder() == true) { return; }
        }

        _saveSql.IsEnabled = false;
        _textBoxFolder.IsEnabled = false;
        string output = _textBoxFolder.Text + @"\ReportLog.db";
        LogFile.Instance.WriteLine($"Write : {output}");

        await Task.Run(() =>
        {
            var sql = new SQLiteManager(output);

            foreach (var view in ListEvent)
            {
                if (view.IsChecked == false)
                {
                    continue;
                }
                sql.Create(view.ClassType);
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
    private void _changeEvent(object sender_, RoutedEventArgs e_)
    {
 
    }

    private void _changeAllEvent(object sender_, RoutedEventArgs e_)
    {
        //var flg = _isCheckedAllEvent();
        if (sender_ is CheckBox checkbox)
        {

            LogFile.Instance.WriteLine($"{checkbox.Content} {checkbox.IsChecked}");
            switch (checkbox.IsChecked)
            {
                case true:
                    ListEvent.ForEach(v_ => v_.IsChecked = checkbox.IsChecked ?? false);
                    break;
                case false:
                    ListEvent.ForEach(v_ => _listSavetEvent[v_.Name] = v_.IsChecked);
                    ListEvent.ForEach(v_ => v_.IsChecked = checkbox.IsChecked ?? false);
                    break;
                default:
                    ListEvent.ForEach(v_ => v_.IsChecked = _listSavetEvent[v_.Name]);
                    break;
            }
        }
    }


    private void _changeAnalysis(object sender_, RoutedEventArgs e_)
    {
    }

    private void _changeAllAnalysis(object sender_, RoutedEventArgs e_)
    {
        if (sender_ is CheckBox checkbox)
        {
            LogFile.Instance.WriteLine($"{checkbox.Name} {checkbox.IsChecked}");
            switch (checkbox.IsChecked)
            {
                case true:
                    ListAnalysis.ForEach(v_ => v_.IsChecked = checkbox.IsChecked ?? false);
                    break;
                case false:
                    ListAnalysis.ForEach(v_ => _listSavetAnalysis[v_.Name] = v_.IsChecked);
                    ListAnalysis.ForEach(v_ => v_.IsChecked = checkbox.IsChecked ?? false);
                    break;
                default:
                    ListAnalysis.ForEach(v_ => v_.IsChecked = _listSavetAnalysis[v_.Name]);
                    break;
            }
        }
    }
}
