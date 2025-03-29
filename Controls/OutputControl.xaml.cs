/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using Microsoft.Win32;
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using System.Collections.ObjectModel;
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
        IsSaveSummy = false;
        IsSaveJoinStartShutdown = false;
        IsSaveJoinCheckOutIn = false;

        InitializeComponent();
        //
        _init();
        //
    }

    /// <summary>
    /// ログ イベント クラスの先頭文字
    /// </summary>
    private readonly string _CLASS_NAME_EVENT = "LogEvent";
    private readonly string _CLASS_NAME_ANALYSIS = "ListAnalysis";

    private readonly string _NAME_SPACE_EVENT = "RepriseReportLogAnalyzer.Events";
    private readonly string _NAME_SPACE_ANALYSES = "RepriseReportLogAnalyzer.Analyses";

    /// <summary>
    /// 総合ファイルの出力
    /// </summary>
    public bool IsSaveSummy { get; set; }

    /// <summary>
    /// 結合情報ファイル(Start Shutdown)の出力
    /// </summary>
    public bool IsSaveJoinStartShutdown { get; set; }
    /// <summary>
    /// 結合情報ファイル(CheckOut CheckIn)の出力
    /// </summary>
    public bool IsSaveJoinCheckOutIn { get; set; }
    /// <summary>
    /// 各イベントの出力確認
    /// </summary>
    public List<OutputView> ListEvent { get; private set; } = new();
    private Dictionary<string, bool> _listSavetEvent = new();

    /// <summary>
    /// 各解析の出力確認
    /// </summary>
    public List<OutputView> ListAnalysis { get; private set; } = new();
    private Dictionary<string, bool> _listSavetAnalysis = new();

    /// <summary>
    /// 各イベント項目の追加
    /// </summary>
    private void _init()
    {
        var _assembly = Assembly.GetExecutingAssembly();

        // イベントの追加
        //var listEventClass = _assembly.GetTypes().Where(t_ => t_.IsClass && t_.Namespace == _NAME_SPACE_EVENT && t_.IsSubclassOf(typeof(LogEventBase)) == true).Distinct().OrderBy(t_ => t_.GetAttribute<SortAttribute>()?.Sort);
        var listEventClass = _assembly.GetTypes().Where(t_ => t_.IsSubclassOf(typeof(LogEventBase)) == true).Distinct().OrderBy(t_ => t_.GetAttribute<SortAttribute>()?.Sort);
        foreach (var t in listEventClass)
        {
            //var find = ListEvent.Where(x_ => (x_.ClassType == null) ? false : x_.ClassType.Name == t.Name);
            //if (find.Any() == false)
            //{
                var view = new OutputView(t, t.Name.Replace(_CLASS_NAME_EVENT, string.Empty));
                ListEvent.Add(view);

                _listSavetEvent[view.Name] = view.IsChecked;
                LogFile.Instance.WriteLine($"{t.Name}");
            //}
        }
        // 解析の追加
        var listAnalysisClass = _assembly.GetTypes().Where(t_ => t_.GetInterfaces().Where(t_ => t_.Name == typeof(IAnalysisOutputFile).Name).Any() == true).Distinct().OrderBy(t_ => t_.GetAttribute<SortAttribute>()?.Sort);
        foreach (var t in listAnalysisClass)
        {
            //var find = ListAnalysis.Where(x_ => (x_.ClassType == null) ? false : x_.ClassType.Name == t.Name);
            //if (find.Any() == false)
            //{
                //var listPropetyInfo = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

                //foreach (var p in listPropetyInfo)
                //{
                //    // コンボボックスのリストを取得
                //    if (p.GetValue(null) is ListStringLongPair list)
                //    {
                //        var view = new OutputView(t, t.Name.Replace(_CLASS_NAME_ANALYSIS, string.Empty), list);
                //        ListAnalysis.Add(view);

                //        _listSavetAnalysis[view.Name] = view.IsChecked;
                //        LogFile.Instance.WriteLine($"{t.Name}");
                //        break;
                //    }
                //}
                var listCombo = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Select(p_=>p_.GetValue(null)).OfType<ListStringLongPair>();
                if (listCombo.Any())
                {
                    var view = new OutputView(t, t.Name.Replace(_CLASS_NAME_ANALYSIS, string.Empty), listCombo.First());
                    ListAnalysis.Add(view);

                    _listSavetAnalysis[view.Name] = view.IsChecked;
                    LogFile.Instance.WriteLine($"{t.Name}");
                }

            //}
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

    /// <summary>
    /// 出力フォルダ 選択
    /// </summary>
    /// <returns></returns>
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
            LogFile.Instance.WriteLine($"Selected {dlg.FolderName}");
        }

        return (string.IsNullOrEmpty(_textBoxFolder.Text));
    }

    /// <summary>
    /// Csv 出力 開始
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _saveCsvClick(object sender_, RoutedEventArgs e_)
    {
        // フォルダ選択
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == true)
        {
            if (_selectOutFolder() == true) { return; }
        }

        _saveCsv.IsEnabled = false;

        var outputFolder = _textBoxFolder.Text;
        LogFile.Instance.WriteLine($"output {outputFolder}");

        var win = new WaitWindow()
        {
            Run = async () => await _saveCsvFile(outputFolder),
            Owner = Application.Current.MainWindow
        };
        win.ShowDialog();
        _saveCsv.IsEnabled = true;

    }

    /// <summary>
    /// Sqlite 出力 開始
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _saveSqliteClick(object sender_, RoutedEventArgs e_)
    {
        LogFile.Instance.WriteLine($"Write : {sender_}");
        if (string.IsNullOrEmpty(_textBoxFolder.Text) == true)
        {
            if (_selectOutFolder() == true) { return; }
        }

        _saveSql.IsEnabled = false;
        _textBoxFolder.IsEnabled = false;
        var outputFolder = _textBoxFolder.Text;
        LogFile.Instance.WriteLine($"output {outputFolder}");

        var win = new WaitWindow()
        {
            Run = async () => await _saveSqlFile(outputFolder + @"\ReportLog.db"),
            Owner = Application.Current.MainWindow
        };
        win.ShowDialog();

        _saveSql.IsEnabled = true;
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

    /// <summary>
    /// Csv 出力
    /// </summary>
    /// <param name="outputFoloder_"></param>
    /// <returns></returns>
    private async Task _saveCsvFile(string outputFoloder_)
    {
        // 総合情報
        if (IsSaveSummy == true)
        {
            string outPath = outputFoloder_ + @"\Summy.txt";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteSummy(outPath);
        }

        // 各イベント
        foreach (var view in ListEvent)
        {
            if (view.IsChecked == false)
            {
                continue;
            }
            if (view.ClassType == null)
            {
                continue;
            }

            var outPath = outputFoloder_ + @"\" + view.Name + @".csv";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteText(outPath, view.ClassType);
        }

        // 結合系
        if (IsSaveJoinStartShutdown == true)
        {
            string outPath = outputFoloder_ + @"\JoinStartShutdown.txt";
            LogFile.Instance.WriteLine($"Write : {outPath}");
            await AnalysisManager.Instance.WriteJoinStartShutdownText(outPath);
        }

        if (IsSaveJoinCheckOutIn == true)
        {
            string outPath = outputFoloder_ + @"\JoinCheckOutIn.txt";
            LogFile.Instance.WriteLine($"Write : {outPath}");
            await AnalysisManager.Instance.WriteJoinCheckOutInText(outPath);

        }

        // 解析情報
        foreach (var view in ListAnalysis)
        {
            if (view.IsChecked == false)
            {
                continue;
            }
            if (view.ClassType == null)
            {
                continue;
            }

            string outPath = outputFoloder_ + @"\" + view.Name + @".csv";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteText(outPath, view.ClassType, view.SelectedValue);
        }
    }

    /// <summary>
    /// Sqlite 出力
    /// </summary>
    /// <param name="outputFile"></param>
    /// <returns></returns>
    private async Task _saveSqlFile(string outputFile)
    {
        LogFile.Instance.WriteLine($"Write : {outputFile}");

        using (var sql = new SQLiteManager(outputFile))
        {
            // 総合情報
            if (IsSaveSummy == true)
            {
                sql.CreateTable("Product");
                sql.Insert("Product", AnalysisManager.Instance.ListProduct);

                sql.CreateTable(AnalysisGroup.USER.Description());
                sql.Insert(AnalysisGroup.USER.Description(), AnalysisManager.Instance.ListUser);

                sql.CreateTable(AnalysisGroup.HOST.Description());
                sql.Insert(AnalysisGroup.HOST.Description(), AnalysisManager.Instance.ListHost);

                sql.CreateTable(AnalysisGroup.USER_HOST.Description());
                sql.Insert(AnalysisGroup.USER_HOST.Description(), AnalysisManager.Instance.ListUserHost);
            }

            // 各イベント
            foreach (var view in ListEvent)
            {
                if (view.IsChecked == false)
                {
                    continue;
                }
                if (view.ClassType == null)
                {
                    continue;
                }
                sql.CreateTable(view.ClassType);
                sql.Insert(view.ClassType, ToDataBase.Header(view.ClassType), AnalysisManager.Instance.ListEventValue(view.ClassType));
            }

            // 結合系
            if (IsSaveJoinStartShutdown == true)
            {
                sql.CreateTable(typeof(JoinEventStartShutdown));
                sql.Insert(typeof(JoinEventStartShutdown), ToDataBase.Header(typeof(JoinEventStartShutdown)), AnalysisManager.Instance.ListJoinStartShutdownValeu());
            }

            if (IsSaveJoinCheckOutIn == true)
            {
                sql.CreateTable(typeof(JoinEventCheckOutIn));
                sql.Insert(typeof(JoinEventCheckOutIn), ToDataBase.Header(typeof(JoinEventCheckOutIn)), AnalysisManager.Instance.ListJoinCheckOutInValeu());
            }

            // 解析情報
            foreach (var view in ListAnalysis)
            {
                if (view.IsChecked == false)
                {
                    continue;
                }
                if (view.ClassType == null)
                {
                    continue;
                }
                sql.CreateTable(view.ClassType, AnalysisManager.Instance.ListEventHeader(view.ClassType, view.SelectedValue));
                sql.Insert(view.ClassType, AnalysisManager.Instance.EventHeader(view.ClassType, view.SelectedValue), AnalysisManager.Instance.ListEventValue(view.ClassType, view.SelectedValue));
            }
            sql.Close();
        }
        await Task.Delay(0);
    }
}