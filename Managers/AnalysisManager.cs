/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using ScottPlot;
using ScottPlot.WPF;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace RepriseReportLogAnalyzer.Managers;

/// <summary>
/// 解析処理管理
/// </summary>
class AnalysisManager : INotifyPropertyChanged
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public static AnalysisManager Instance = new AnalysisManager();
    private AnalysisManager()
    {
        _listAnalysis.Add(_listCheckOutIn);
        _listAnalysis.Add(_listLicenseCount);
        _listAnalysis.Add(_listStartShutdown);
        _listAnalysis.Add(_listUserDuration);
        _listAnalysis.Add(_listHostDuration);
        _listAnalysis.Add(_listUserHostDuration);     
    }

    public void Create()
    {
        LogFile.Instance.WriteLine("Create");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void _notifyPropertyChanged([CallerMemberName] string propertyName_ = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
    }

    /// <summary>
    /// プロダクトの画面表示
    /// </summary>
    public ObservableCollection<LicenseView> ListResultProduct { get; private set; } = new();

    /// <summary>
    /// グループのデータ表示
    /// </summary>

    public ObservableCollection<LicenseView> ListResultGroup { get; private set; } = new();

    /// <summary>
    /// カレンダーの開始
    /// </summary>
    public DateTime? StartDate { get => (ListDate.Count() == 0) ? null : ListDate.FirstOrDefault(); }

    /// <summary>
    /// カレンダーの終了
    /// </summary>
    public DateTime? EndDate { get => (ListDate.Count() == 0) ? null : ListDate.LastOrDefault(); }


    public IEnumerable<LogEventBase> ListEvent(Type classType_) => _convertReportLog.ListEvent(classType_);


    /// <summary>
    /// 出力用の管理
    /// </summary>
    private List<IAnalysisOutputFile> _listAnalysis = new();

    /// <summary>
    /// 解析利用
    /// </summary>
    private ConvertReportLog _convertReportLog = new();
    private readonly ListAnalysisStartShutdown _listStartShutdown = new();
    private readonly ListAnalysisCheckOutIn _listCheckOutIn = new();
    private readonly ListAnalysisLicenseCount _listLicenseCount = new();
    //
    private readonly ListAnalysisLicenseUser _listUserDuration = new();
    private readonly ListAnalysisLicenseHost _listHostDuration = new();
    private readonly ListAnalysisLicenseUserHost _listUserHostDuration = new();

    /// <summary>
    /// 読み込んだファイル
    /// </summary>
    private List<string> _listFile = new();

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    private ProgressCountDelegate? _progressCount = null;
    //
    // _convertReportLog があるが確実にデータがはいっているタイミングで更新する
    public SortedSet<string> ListProduct { get; private set; } = new();
    public SortedSet<(string Product,string Version)> ListProductVersion { get; private set; } = new();
    public SortedSet<string> ListUser { get; private set; } = new();
    public SortedSet<string> ListHost { get; private set; } = new();
    public SortedSet<string> ListUserHost { get; private set; } = new();
    public SortedSet<DateTime> ListDate { get; private set; } = new();

    /// <summary>
    /// 解析処理開始時間
    /// </summary>
    private DateTime? _runStartTime =null ;
    /// <summary>
    /// 解析処理終了時間
    /// </summary>
    private DateTime? _runEndTime = null;

    /// <summary>
    /// 各処理のプログレスバー設定
    /// </summary>
    /// <param name="progressCount_"></param>
    public void SetProgressCount(ProgressCountDelegate progressCount_)
    {
        _progressCount = progressCount_;
        _convertReportLog.ProgressCount = progressCount_;
        _listStartShutdown.ProgressCount = progressCount_;
        _listCheckOutIn.ProgressCount = progressCount_;
        _listLicenseCount.ProgressCount = progressCount_;
        _listUserDuration.ProgressCount = progressCount_;
        _listHostDuration.ProgressCount = progressCount_;
        _listUserHostDuration.ProgressCount = progressCount_;

    }

    /// <summary>
    /// 処理クリア
    /// </summary>
    private void _clear()
    {
        ListProduct.Clear();
        ListProductVersion.Clear();
        ListUser.Clear();
        ListHost.Clear();
        ListUserHost.Clear();
        ListDate.Clear();

        //
        _listStartShutdown.Clear();
        _listCheckOutIn.Clear();
        _listLicenseCount.Clear();
        _listUserDuration.Clear();
        _listHostDuration.Clear();
        _listUserHostDuration.Clear();
        //
        _runStartTime = null;
        _runEndTime = null;
    }

    /// <summary>
    /// ファイルの変換解析
    /// </summary>
    /// <param name="listFile"></param>
    public void Analysis(IEnumerable<string> listFile)
    {
        _runStartTime ??= DateTime.MinValue;

        _listFile.AddRange(listFile);
        //
        _clear();
        //
        _convert(); // 変換
        _analysis(); // 解析

        _runEndTime ??= DateTime.MinValue;

    }

    /// <summary>
    /// 変換処理
    /// </summary>
    private void _convert() 
    {
        int max = _listFile.Count();
        foreach (string path_ in _listFile)
        {
            LogFile.Instance.WriteLine($"LogAnalysis: {path_}");
            _convertReportLog.Start(path_);
        }
        _convertReportLog.End();
    }

    /// <summary>
    /// 解析処理
    /// </summary>
    private void _analysis()
    {
        ListProduct.AddRange(_convertReportLog.ListProduct);
        ListProductVersion.AddRange(_convertReportLog.ListProductEvent.Select(x_=>(x_.Product,x_.Version)));

        ListUser.AddRange(_convertReportLog.ListUser);
        ListHost.AddRange(_convertReportLog.ListHost);
        ListUserHost.AddRange(_convertReportLog.ListUserHost);
        ListDate.AddRange(_convertReportLog.ListDate);
        //
        _listStartShutdown.Analysis(_convertReportLog);
        _listCheckOutIn.Analysis(_convertReportLog, _listStartShutdown.ListNoIncludeSkip());
        _listLicenseCount.Analysis(_convertReportLog, _listCheckOutIn);
        //
        _listUserDuration.Analysis(ListUser, _listCheckOutIn);
        _listHostDuration.Analysis(ListHost, _listCheckOutIn);
        _listUserHostDuration.Analysis(ListUserHost, _listCheckOutIn);

        //
        _notifyPropertyChanged("StartDate");
        _notifyPropertyChanged("EndDate");
    }

    /// <summary>
    /// 総合データの書き出し
    /// </summary>
    /// <param name="path_"></param>
    public void WriteSummy(string path_)
    {
        var list = new List<string>();

        list.Add($"Analsis File Count:{_listFile.Count}");
        list.AddRange(_listFile.Select(x_ => Path.GetFileName(x_)));
        list.Add("\n");

        list.Add($"Analsis Time : {_runEndTime - _runStartTime}");
        list.Add($"Start   Time : {_runStartTime}");
        list.Add($"End     Time : {_runEndTime}");
        list.Add("\n");
        //
        list.Add($"License Count : {ListProduct.Count()}");
        list.AddRange(ListProductVersion.Select(x_ => $"{x_.Product},{x_.Version}"));
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListProduct:{ListProduct.Count()}");

        list.Add($"User Count : {ListUser.Count()}");
        list.AddRange(ListUser);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListUser:{ListUser.Count()}");

        list.Add($"Host Count : {ListHost.Count()}");
        list.AddRange(ListHost);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListHost:{ListHost.Count()}");

        list.Add($"User@Host Count : {ListUserHost.Count()}");
        list.AddRange(ListUserHost);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListUserHost:{ListUserHost.Count()}");

        File.WriteAllLines(path_, list, Encoding.UTF8);
        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    /// <summary>
    /// 変換系のテキストファイル出力
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="classType_"></param>

    public void WriteText(string path_, Type classType_)
    {
        _convertReportLog.WriteEventText(path_, classType_);
    }
    /// <summary>
    /// 解析系のテキストファイル出力
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    public void WriteText(string path_, Type classType_, long selected_)
    {
        var find = _listAnalysis.Find(f_ => f_.GetType() == classType_);
        find?.WriteText(path_, selected_);

    }

    /// <summary>
    /// 変換系のリスト化したデータ
    /// </summary>
    /// <param name="classType_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListEventValue(Type classType_)=> _convertReportLog.ListEvent(classType_).Select(x => x.ListValue(classType_));

    /// <summary>
    /// 解析系のヘッダー
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    /// <returns></returns>
    public string EventHeader(Type classType_, long selected_) => _listAnalysis.Find(f_ => f_.GetType() == classType_)?.Header(selected_) ?? default;

    /// <summary>
    /// 解析系のリスト化したヘッダー
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    /// <returns></returns>
    public ListStringStringPair ListEventHeader(Type classType_, long selected_)=> _listAnalysis.Find(f_ => f_.GetType() == classType_)?.ListHeader(selected_) ?? default;


    /// <summary>
    /// 解析系のリスト化したデータ
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListEventValue(Type classType_, long selected_)
    {
        var find = _listAnalysis.Find(f_ => f_.GetType() == classType_);
        return find?.ListValue(selected_) ?? default;
    }

    /// <summary>
    /// 画面のプロダクト表示のチェック状態
    /// </summary>
    /// <param name="product_"></param>
    /// <returns></returns>
    public bool IsProductChecked(string product_) => ListResultProduct.Where(x_ => x_.Name == product_).Select(x_ => x_.IsChecked).FirstOrDefault();

    /// <summary>
    /// 画面のデータ表示更新
    /// </summary>
    /// <param name="date_"></param>
    /// <param name="group_"></param>

    public void SetData(DateTime? date_, AnalysisGroup group_)
    {
        // Product は使いまわし(チェックのため)
        foreach (var view in _listLicenseCount.ListView(date_))
        {
            ListResultProduct.SetView(view);
        }

        // Groupはいったんクリア
        ListResultGroup.Clear();
        switch (group_)
        {
            case AnalysisGroup.USER: ListResultGroup.AddRange(_listUserDuration.ListView(date_)); break;
            case AnalysisGroup.HOST: ListResultGroup.AddRange(_listHostDuration.ListView(date_)); break;
            case AnalysisGroup.USER_HOST: ListResultGroup.AddRange(_listUserHostDuration.ListView(date_)); break;
            default: break;
        }
        //
        _notifyPropertyChanged("ListResultProduct");
        _notifyPropertyChanged("ListResultGroup");
    }

    /// <summary>
    /// 表の表示更新
    /// </summary>
    /// <param name="plot_"></param>
    /// <param name="date_"></param>
    /// <param name="group_"></param>
    public void SetPlot(WpfPlot plot_, DateTime? date_, AnalysisGroup group_)
    {
        plot_.Plot.Clear();

        var title = string.Empty;
        var xlabel = (date_ ==null) ? $"Date":$"[{date_.GetValueOrDefault().ToShortDateString()}] Time (30 Minute)";
        var ylabel = string.Empty;
        var pos = Alignment.LowerRight;
        switch (group_)
        {
            case AnalysisGroup.USER: 
            case AnalysisGroup.HOST: 
            case AnalysisGroup.USER_HOST:
                title = (date_ == null) ? $"Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}" : $"[{date_.GetValueOrDefault().ToShortDateString()}] Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}";
                ylabel = $"Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}";
                plot_.Plot.Axes.SetLimitsY(bottom: ListAnalysisLicenseGroup.TOP_PLOT_USE, top: 1);
                pos = Alignment.LowerLeft;
                break;
            default:
            case AnalysisGroup.NONE:
                title = (date_ == null) ? $"Product - License Count": $"[{ date_.GetValueOrDefault().ToShortDateString()}] Product - License Count";
                ylabel = $"Count";
                pos = Alignment.UpperLeft;
                break;
        }
        plot_.Plot.Title(title);


        plot_.Plot.XLabel(xlabel);
        plot_.Plot.YLabel(ylabel);
        //plot_.Plot.ShowLegend(Edge.Right); // なぜか二重に表示されるので見合わせ
        plot_.Plot.ShowLegend(pos);


        long timeSpan = TimeSpan.TicksPerDay;
        var listX = new List<DateTime>();
        if (date_ == null)
        {
            listX.AddRange(ListDate);
        }
        else
        {
            timeSpan = 30 * TimeSpan.TicksPerMinute;
            DateTime date = date_ ?? DateTime.Now;
            for (var time = date; time < date.AddTicks(TimeSpan.TicksPerDay); time = time.AddTicks(timeSpan))
            {
                listX.Add(time);
            }
        }

        var listY = new SortedDictionary<string, List<double>>();
        switch (group_)
        {
            case AnalysisGroup.USER: listY = _listUserDuration.ListPlot(listX, timeSpan); break;
            case AnalysisGroup.HOST: listY = _listHostDuration.ListPlot(listX, timeSpan); break;
            case AnalysisGroup.USER_HOST: listY = _listUserHostDuration.ListPlot(listX, timeSpan); break;
            default:
            case AnalysisGroup.NONE: listY = _listLicenseCount.ListPlot(listX, timeSpan); break;
        }

        foreach (var key in listY.Keys)
        {
            var plotCount = plot_.Plot.Add.Scatter(listX.ToArray(), listY[key].ToArray());
            plotCount.LegendText = key;
        }


        plot_.Plot.Axes.DateTimeTicksBottom();
        plot_.Refresh();

    }

}
