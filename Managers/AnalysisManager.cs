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
internal sealed class AnalysisManager : INotifyPropertyChanged
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
        _runStartTime = null;
        _runEndTime = null;
        _progressCount = null;
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
    public DateTime? StartDate { get => (ListDate.Count == 0) ? null : ListDate.FirstOrDefault(); }

    /// <summary>
    /// カレンダーの終了
    /// </summary>
    public DateTime? EndDate { get => (ListDate.Count == 0) ? null : ListDate.LastOrDefault(); }


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
    private ProgressCountCallBack? _progressCount;
    //
    // _convertReportLog があるが確実にデータがはいっているタイミングで更新する
    public SortedSet<string> ListProduct { get; private set; } = new();
    public SortedSet<(string Product, string Version)> ListProductVersion { get; private set; } = new();
    public SortedSet<string> ListUser { get; private set; } = new();
    public SortedSet<string> ListHost { get; private set; } = new();
    public SortedSet<string> ListUserHost { get; private set; } = new();
    public SortedSet<DateTime> ListDate { get; private set; } = new();
    public SortedSet<DateTime> ListDateTime { get; private set; } = new();

    /// <summary>
    /// 解析処理開始時間
    /// </summary>
    private DateTime? _runStartTime;
    /// <summary>
    /// 解析処理終了時間
    /// </summary>
    private DateTime? _runEndTime;

    /// <summary>
    /// 各処理のプログレスバー設定
    /// </summary>
    /// <param name="progressCount_"></param>
    public void SetProgressCount(ProgressCountCallBack progressCount_)
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
        ListDateTime.Clear();
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

        ConvertReportLog.Clear();
    }

    /// <summary>
    /// ファイルの変換解析
    /// </summary>
    /// <param name="listFile"></param>
    public async Task Analysis(IEnumerable<string> listFile)
    {
        //
        _clear();
        //
        _runStartTime = DateTime.Now;
        // 処理ファイル
        _listFile.AddRange(listFile);
        //
        await _convert(); // 変換
        await _analysis(); // 解析

        _runEndTime = DateTime.Now;


        _progressCount?.Invoke(0, 0, $"Analysis End");
    }

    /// <summary>
    /// 変換処理
    /// </summary>
    private async Task _convert()
    {
        int max = _listFile.Count;
        foreach (string path_ in _listFile)
        {
            LogFile.Instance.WriteLine($"LogAnalysis: {path_}");
            await _convertReportLog.Start(path_);
        }
        _convertReportLog.End();
    }

    /// <summary>
    /// 解析処理
    /// </summary>
    private async Task _analysis()
    {
        ListProduct.AddRange(_convertReportLog.ListProduct);
        ListProductVersion.AddRange(_convertReportLog.ListProductEvent.Select(x_ => (x_.Product, x_.Version)));

        ListUser.AddRange(_convertReportLog.ListUser);
        ListHost.AddRange(_convertReportLog.ListHost);
        ListUserHost.AddRange(_convertReportLog.ListUserHost);
        ListDate.AddRange(_convertReportLog.ListDate);
        ListDateTime.AddRange(_convertReportLog.ListDateTime);
        //
        _listStartShutdown.Analysis(_convertReportLog);
        _listCheckOutIn.Analysis(_convertReportLog, _listStartShutdown.ListNoIncludeSkip()); //
        //_listLicenseCount.Analysis(_convertReportLog, _listCheckOutIn);
        _listLicenseCount.Analysis(_convertReportLog);
        //
        //_listUserDuration.Analysis(ListUser, _listCheckOutIn);
        //_listHostDuration.Analysis(ListHost, _listCheckOutIn);
        //_listUserHostDuration.Analysis(ListUserHost, _listCheckOutIn);
        _listUserDuration.Analysis(_listCheckOutIn);
        _listHostDuration.Analysis(_listCheckOutIn);
        _listUserHostDuration.Analysis(_listCheckOutIn);

        //
        _notifyPropertyChanged("StartDate");
        _notifyPropertyChanged("EndDate");

        await Task.Delay(0);
    }

    /// <summary>
    /// 総合データの書き出し
    /// </summary>
    /// <param name="path_"></param>
    public async Task WriteSummy(string path_)
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
        list.Add($"Replort Log Period");
        list.Add($"Start   Time : {ListDateTime.FirstOrDefault()}");
        list.Add($"End     Time : {ListDateTime.LastOrDefault()}");
        list.Add("\n");


        list.Add($"License Count : {ListProduct.Count}");
        list.AddRange(ListProductVersion.Select(x_ => $"{x_.Product},{x_.Version}"));
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListProduct:{ListProduct.Count}");

        list.Add($"User Count : {ListUser.Count}");
        list.AddRange(ListUser);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListUser:{ListUser.Count}");

        list.Add($"Host Count : {ListHost.Count}");
        list.AddRange(ListHost);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListHost:{ListHost.Count}");

        list.Add($"User@Host Count : {ListUserHost.Count}");
        list.AddRange(ListUserHost);
        list.Add("\n");
        LogFile.Instance.WriteLine($"ListUserHost:{ListUserHost.Count}");

        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);
        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    /// <summary>
    /// 変換系のテキストファイル出力
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="classType_"></param>
    public async Task WriteText(string path_, Type classType_) => await _convertReportLog.WriteEventText(path_, classType_);

    /// <summary>
    /// 結合情報(Start Shutdown)のテキストファイル出力
    /// </summary>
    /// <param name="path_"></param>
    /// <returns></returns>
    public async Task WriteJoinStartShutdownText(string path_) => await _listStartShutdown.WriteJoinText(path_);

    /// <summary>
    /// 結合情報(CheckOut CheckIn)のテキストファイル出力
    /// </summary>
    /// <param name="path_"></param>
    /// <returns></returns>
    public async Task WriteJoinCheckOutInText(string path_) => await _listCheckOutIn.WriteJoinText(path_);

    /// <summary>
    /// 解析系のテキストファイル出力
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    public async Task WriteText(string path_, Type classType_, long selected_)
    {
        var find = _listAnalysis.Find(f_ => f_.GetType() == classType_);
        if (find != null)
        {
            await find.WriteText(path_, selected_);
        }
    }

    /// <summary>
    /// 変換系のリスト化したデータ
    /// </summary>
    /// <param name="classType_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListEventValue(Type classType_) => _convertReportLog.ListEvent(classType_).Select(x => x.ListValue(classType_));

    /// <summary>
    /// 結合情報(Start Shutdown)のリスト化したデータ
    /// </summary>
    public IEnumerable<List<string>> ListJoinStartShutdownValue() => _listStartShutdown.ListJoinValue();

    /// <summary>
    /// 結合情報(CheckOut CheckIn)のリスト化したデータ
    /// </summary>
    public IEnumerable<List<string>> ListJoinCheckOutInValue() => _listCheckOutIn.ListJoinValue();

    /// <summary>
    /// 解析系のヘッダー
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    /// <returns></returns>
    public string EventHeader(Type classType_, long selected_) => _listAnalysis.Find(f_ => f_.GetType() == classType_)?.Header(selected_) ?? string.Empty;

    /// <summary>
    /// 解析系のリスト化したヘッダー
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    /// <returns></returns>
    public ListStringStringPair ListEventHeader(Type classType_, long selected_) => _listAnalysis.Find(f_ => f_.GetType() == classType_)?.ListHeader(selected_) ?? new();


    /// <summary>
    /// 解析系のリスト化したデータ
    /// </summary>
    /// <param name="classType_"></param>
    /// <param name="selected_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListEventValue(Type classType_, long selected_)
    {
        var find = _listAnalysis.Find(f_ => f_.GetType() == classType_);

        return find?.ListValue(selected_) ?? new List<List<string>>();
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
    public async Task SetData(DateTime? date_, AnalysisGroup group_)
    {
        // Product は使いまわし(チェックのため)

        var list = await _listLicenseCount.ListView(date_);
        foreach (var view in list)
        {
            ListResultProduct.SetView(view);
        }

        // Groupはいったんクリア
        ListResultGroup.Clear();
        switch (group_)
        {
            case AnalysisGroup.USER: ListResultGroup.AddRange(await _listUserDuration.ListView(date_)); break;
            case AnalysisGroup.HOST: ListResultGroup.AddRange(await _listHostDuration.ListView(date_)); break;
            case AnalysisGroup.USER_HOST: ListResultGroup.AddRange(await _listUserHostDuration.ListView(date_)); break;
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
    public async Task SetPlot(WpfPlot plot_, DateTime? date_, AnalysisGroup group_)
    {
        plot_.Plot.Clear();
        _setPlotLabel(plot_, date_, group_);

        long timeSpan = (date_ == null) ? TimeSpan.TicksPerDay : 30 * TimeSpan.TicksPerMinute;
        // x 軸の値
        var listX = _listX(date_);

        // y 軸の値
        var listY = new SortedList<string, List<double>>();
        switch (group_)
        {
            case AnalysisGroup.USER: listY = await _listUserDuration.ListPlot(listX, timeSpan); break;
            case AnalysisGroup.HOST: listY = await _listHostDuration.ListPlot(listX, timeSpan); break;
            case AnalysisGroup.USER_HOST: listY = await _listUserHostDuration.ListPlot(listX, timeSpan); break;
            default:
            case AnalysisGroup.NONE: listY = await _listLicenseCount.ListPlot(listX, timeSpan); break;
        }

        // Key にプロットタイトル
        foreach (var key in listY.Keys)
        {
            var plotCount = plot_.Plot.Add.Scatter(listX.ToArray(), listY[key].ToArray());
            plotCount.LegendText = key;
        }

        plot_.Plot.Axes.DateTimeTicksBottom();
        plot_.Refresh();
    }

    /// <summary>
    /// プロットのx軸
    /// </summary>
    /// <param name="date_"></param>
    /// <returns></returns>
    private List<DateTime> _listX(DateTime? date_)
    {
        if (date_ == null)
        {
            return new List<DateTime>(ListDate);
        }
        var rtn = new List<DateTime>();

        var date = date_ ?? DateTime.Now;
        for (var time = date; time < date.AddTicks(TimeSpan.TicksPerDay); time = time.AddTicks(30 * TimeSpan.TicksPerMinute))
        {
            rtn.Add(time);
        }
        return rtn;
    }

    /// <summary>
    /// プロットのラベル
    /// </summary>
    /// <param name="plot_"></param>
    /// <param name="date_"></param>
    /// <param name="group_"></param>
    private void _setPlotLabel(WpfPlot plot_, DateTime? date_, AnalysisGroup group_)
    {
        var title = string.Empty;
        var xlabel = (date_ == null) ? $"Date" : $"[{date_.GetValueOrDefault().ToShortDateString()}] Time (30 Minute)";
        var ylabel = string.Empty;
        //
        var pos = Alignment.LowerRight;
        switch (group_)
        {
            case AnalysisGroup.USER:
            case AnalysisGroup.HOST:
            case AnalysisGroup.USER_HOST:
                title = (date_ == null) ? $"Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}" : $"[{date_.GetValueOrDefault().ToShortDateString()}] Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}";
                ylabel = $"Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}";
                plot_.Plot.Axes.SetLimitsY(ListAnalysisLicenseGroup.TOP_PLOT_USE, 1);
                pos = Alignment.LowerLeft;
                break;
            default:
            case AnalysisGroup.NONE:
                title = (date_ == null) ? $"Product - License Count" : $"[{date_.GetValueOrDefault().ToShortDateString()}] Product - License Count";
                ylabel = $"Count";
                pos = Alignment.UpperLeft;
                plot_.Plot.Axes.SetLimitsY(0, _listLicenseCount.Max);
                break;
        }

        title += (date_ == null) ? "" : $" [{date_.GetValueOrDefault().ToShortDateString()}]";
        plot_.Plot.Title(title);

        plot_.Plot.XLabel(xlabel);
        plot_.Plot.YLabel(ylabel);
        //plot_.Plot.ShowLegend(Edge.Right); // なぜか二重に表示されるので見合わせ
        plot_.Plot.ShowLegend(pos);


        LogFile.Instance.WriteLine($"Title [{title}] Y Label{ylabel}");


    }

}
