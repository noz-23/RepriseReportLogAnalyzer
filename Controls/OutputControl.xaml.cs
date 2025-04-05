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
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
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
        _initEventLog(_assembly);
        _initAnalysis(_assembly);
    }

    /// <summary>
    /// イベントログクラスの表示
    /// </summary>
    /// <param name="asm_"></param>
    private void _initEventLog(Assembly asm_)
    {
        // イベントの追加
        var listEventClass = asm_.GetTypes()
                                　.Where(t_ => t_.IsSubclassOf(typeof(LogEventBase)) == true)
                                　.Distinct();

        listEventClass.ToList().ForEach(t_ => _addEvent(t_));

        ListEvent.Sort((a_, b_) => a_.Sort -b_.Sort);
    }

    /// <summary>
    /// イベントログクラスの追加
    /// </summary>
    /// <param name="classType_"></param>
    private void _addEvent(Type classType_)
    {
        var view = new OutputView(classType_, classType_.Name.Replace(_CLASS_NAME_EVENT, string.Empty));
        ListEvent.Add(view);

        _listSavetEvent[view.Name] = view.IsChecked;
        LogFile.Instance.WriteLine($"{classType_.Name}");
    }

    /// <summary>
    /// 解析クラスの表示
    /// </summary>
    /// <param name="asm_"></param>
    private void _initAnalysis(Assembly asm_)
    {
        // 解析の追加
        var listAnalysisClass = asm_.GetTypes()
                                    .Where(t_ => t_.GetInterfaces().Any(t_ => t_ == typeof(IAnalysisOutputFile)) == true)
                                    .Distinct();

        listAnalysisClass.ToList().ForEach(t_ => _addAnalysis(t_));

        ListAnalysis.Sort((a_, b_) => a_.Sort - b_.Sort);
    }
    /// <summary>
    /// 解析クラスの追加
    /// </summary>
    /// <param name="classType_"></param>
    private void _addAnalysis(Type classType_)
    {
        var listCombo = classType_.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
                        .Select(p_ => p_.GetValue(null)).OfType<ListStringLongPair>();
        if (listCombo.Any())
        {
            var view = new OutputView(classType_, classType_.Name.Replace(_CLASS_NAME_ANALYSIS, string.Empty), listCombo.First());
            ListAnalysis.Add(view);

            _listSavetAnalysis[view.Name] = view.IsChecked;
            LogFile.Instance.WriteLine($"{classType_.Name}");
        }
    }

    /// <summary>
    /// 表示ごとのコンボボックス位置初期化
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
    private void _loaded(object sender_, RoutedEventArgs e_)
    {
        //foreach (var v in ListAnalysis)
        //{
        //    v.SelectedIndex = 0;
        //}
        // なんかうまくバインドされないため
        ListAnalysis.ForEach(v_ => v_.SelectedIndex = 0);
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
    /// <summary>
    /// 全選択イベントの変更
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>
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

    /// <summary>
    /// 全選択解析の変更
    /// </summary>
    /// <param name="sender_"></param>
    /// <param name="e_"></param>

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
    /// <param name="outputFolder_"></param>
    /// <returns></returns>
    private async Task _saveCsvFile(string outputFolder_)
    {
        // 総合情報
        await _saveCsvSummy(outputFolder_);
        // 各イベント
        await _saveCsvEvent(outputFolder_);
        // 結合系
        await _saveCsvJoin(outputFolder_);
        // 解析情報
        await _saveCsvAnalysis(outputFolder_);
    }

    /// <summary>
    /// Csv 出力(総合情報)
    /// </summary>
    /// <param name="outputFolder_"></param>
    /// <returns></returns>

    private async Task _saveCsvSummy(string outputFolder_)
    {
        if (IsSaveSummy == true)
        {
            string outPath = outputFolder_ + @"\Summy.txt";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteSummy(outPath);
        }
    }
    /// <summary>
    /// Csv 出力(各イベント)
    /// </summary>
    /// <param name="outputFolder_"></param>
    /// <returns></returns>
    private async Task _saveCsvEvent(string outputFolder_)
    {
        foreach (var view in ListEvent.Where(v_ => v_.IsChecked == true && v_.ClassType != null))
        {
            var outPath = outputFolder_ + @"\" + view.Name + @".csv";
            LogFile.Instance.WriteLine($"Write : {outPath}");

            await AnalysisManager.Instance.WriteText(outPath, view.ClassType);
        }
    }
    /// <summary>
    /// Csv 出力(結合系)
    /// </summary>
    /// <param name="outputFolder_"></param>
    /// <returns></returns>
    private async Task _saveCsvJoin(string outputFolder_)
    {
        if (IsSaveJoinStartShutdown == true)
        {
            string outPath = outputFolder_ + @"\JoinStartShutdown.txt";
            LogFile.Instance.WriteLine($"Write : {outPath}");
            await AnalysisManager.Instance.WriteJoinStartShutdownText(outPath);
        }

        if (IsSaveJoinCheckOutIn == true)
        {
            string outPath = outputFolder_ + @"\JoinCheckOutIn.txt";
            LogFile.Instance.WriteLine($"Write : {outPath}");
            await AnalysisManager.Instance.WriteJoinCheckOutInText(outPath);

        }
    }

    /// <summary>
    /// Csv 出力(解析情報)
    /// </summary>
    /// <param name="outputFolder_"></param>
    /// <returns></returns>
    private async Task _saveCsvAnalysis(string outputFolder_) 
    {
        foreach (var view in ListAnalysis.Where(v_=> v_.IsChecked == true && v_.ClassType != null))
        {
            string outPath = outputFolder_ + @"\" + view.Name + @".csv";
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
            _saveSqlSummy(sql);
            // 各イベント
            _saveSqlEvent(sql);
            // 結合系
            _saveSqlJoin(sql);
            // 解析情報
            _saveSqlAnalysis(sql);

            sql.Close();
        }
        await Task.Delay(0);
    }

    /// <summary>
    /// Sqlite 出力(総合情報)
    /// </summary>
    /// <param name="sql_"></param>

    private void _saveSqlSummy(SQLiteManager sql_)
    {
        if (IsSaveSummy == true)
        {
            sql_.CreateTableAndInsert("Product", AnalysisManager.Instance.ListProductVersion.Select(x_ => x_.Product + " " + x_.Version));
            sql_.CreateTableAndInsert(AnalysisGroup.USER.Description(), AnalysisManager.Instance.ListUser);
            sql_.CreateTableAndInsert(AnalysisGroup.HOST.Description(), AnalysisManager.Instance.ListHost);
            sql_.CreateTableAndInsert(AnalysisGroup.USER_HOST.Description(), AnalysisManager.Instance.ListUserHost);
        }

    }

    /// <summary>
    /// Sqlite 出力(各イベント)
    /// </summary>
    /// <param name="sql_"></param>
    private void _saveSqlEvent(SQLiteManager sql_)
    {
        foreach (var view in ListEvent.Where(v_=> v_.IsChecked==true && v_.ClassType !=null))
        {
            sql_.CreateTableAndInsert(view.ClassType, AnalysisManager.Instance.ListEventValue(view.ClassType));
        }

    }
    /// <summary>
    /// Sqlite 出力(結合系)
    /// </summary>
    /// <param name="sql_"></param>
    private void _saveSqlJoin(SQLiteManager sql_)
    {
        if (IsSaveJoinStartShutdown == true)
        {
            sql_.CreateTableAndInsert(typeof(JoinEventStartShutdown), AnalysisManager.Instance.ListJoinStartShutdownValue());
        }

        if (IsSaveJoinCheckOutIn == true)
        {
            sql_.CreateTableAndInsert(typeof(JoinEventCheckOutIn), AnalysisManager.Instance.ListJoinCheckOutInValue());
        }
    }

    /// <summary>
    /// Sqlite 出力(解析情報)
    /// </summary>
    /// <param name="sql_"></param>
    private void _saveSqlAnalysis(SQLiteManager sql_)
    {
        foreach (var view in ListAnalysis.Where(v_ => v_.IsChecked == true && v_.ClassType != null))
        {
            sql_.CreateTableAndInsert(view.ClassType, view.SelectedValue);
        }
    }
}