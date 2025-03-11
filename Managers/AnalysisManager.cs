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
using RLMLogReader.Extensions;
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

    public ObservableCollection<LicenseView> ListResultProduct { get; private set; } = new();

    public ObservableCollection<LicenseView> ListResultGroup { get; private set; } = new();

    private List<IAnalysisTextWrite> _listAnalysis = new();

    private readonly ListAnalysisStartShutdown _listStartShutdown = new();
    private readonly ListAnalysisCheckOutIn _listCheckOutIn = new();
    private readonly ListAnalysisLicenseCount _listLicenseCount = new();
    //
    private readonly ListAnalysisLicenseUser _listUserDuration = new();
    private readonly ListAnalysisLicenseHost _listHostDuration = new();
    private readonly ListAnalysisLicenseUserHost _listUserHostDuration = new();

    private ConvertReportLog _convertReportLog =new ();
    private List<string> _listFile = new();

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    private ProgressCountDelegate? _progressCount = null;
    //
    //private readonly Dictionary<DateTime, IEnumerable<LicenseView>> _listDateToView = new Dictionary<DateTime, IEnumerable<LicenseView>>();
    //private Dictionary<DateTime, IEnumerable<LicenseView>> ListDayToProduct() => _listLicenseCount.ListDayToProduct();

    //private readonly Dictionary<string, bool> _listProductChecked = new ();


    // _convertReportLog があるが確実にデータがはいっているタイミングで更新する
    public SortedSet<string> ListProduct { get; private set; } = new();

    public SortedSet<(string Product,string Version)> ListProductVersion { get; private set; } = new();

    public SortedSet<string> ListUser { get; private set; } = new();
    public SortedSet<string> ListHost { get; private set; } = new();
    public SortedSet<string> ListUserHost { get; private set; } = new();
    public SortedSet<DateTime> ListDate { get; private set; } = new();

    public DateTime? StartDate { get => (ListDate.Count() == 0) ? null : ListDate.FirstOrDefault(); }
    public DateTime? EndDate { get => (ListDate.Count() == 0) ? null : ListDate.LastOrDefault(); }


    private DateTime? _startTime =null ;
    private DateTime? _endTime = null;
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

    public void Clear()
    {
        ListProduct.Clear();
        ListProductVersion.Clear();
        ListUser.Clear();
        ListHost.Clear();
        ListUserHost.Clear();
        ListDate.Clear();
        //
        //_listProductChecked.Clear();

        //
        _listStartShutdown.Clear();
        _listCheckOutIn.Clear();
        _listLicenseCount.Clear();
        _listUserDuration.Clear();
        _listHostDuration.Clear();
        _listUserHostDuration.Clear();

        _startTime = null;
        _endTime = null;
    }

    public void Analysis(IEnumerable<string> listFile)
    {
        _startTime??= DateTime.MinValue;

        _listFile.AddRange(listFile);

        _convert();
        //_calendarShow(analysis.ListDate);

        //AnalysisManager.Instance.Analysis(analysis);
        _analysis();

        _endTime ??= DateTime.MinValue;

    }

    private void _convert() 
    {
        //int count = 0;
        int max = _listFile.Count();
        //_progressCount?.Invoke(0, max, _ANALYSIS);

        //var analysis = new AnalysisReportLog();
        foreach (string path_ in _listFile)
        {
            LogFile.Instance.WriteLine($"LogAnalysis: {path_}");
            _convertReportLog.Start(path_);
            //_progressCount?.Invoke(++count, max);
        }
        _convertReportLog.End();
    }
    //public void Analysis(AnalysisReportLog analysis_)
    private void _analysis()
    {
        //_analysis = analysis_;

        Clear();
        //
        //ListProduct.AddRange(_convertReportLog.ListProductEvent.Select(x_ => x_.Product));
        ListProduct.AddRange(_convertReportLog.ListProduct);
        ListProductVersion.AddRange(_convertReportLog.ListProductEvent.Select(x_=>(x_.Product,x_.Version)));

        ListUser.AddRange(_convertReportLog.ListUser);
        ListHost.AddRange(_convertReportLog.ListHost);
        ListUserHost.AddRange(_convertReportLog.ListUserHost);
        ListDate.AddRange(_convertReportLog.ListDate);
        //
        //foreach (var product in ListProduct)
        //{
        //    _listProductChecked[product]=true;
        //}
        //
        _listStartShutdown.Analysis(_convertReportLog);
        _listCheckOutIn.Analysis(_convertReportLog, _listStartShutdown.ListWithoutSkip());
        _listLicenseCount.Analysis(_convertReportLog, _listCheckOutIn);
        //
        _listUserDuration.Analysis(ListUser, _listCheckOutIn);
        _listHostDuration.Analysis(ListHost, _listCheckOutIn);
        _listUserHostDuration.Analysis(ListUserHost, _listCheckOutIn);

        //
        //foreach (var date in ListDate)
        //{
        //    _listDateToView[date] = _listLicenseCount.ListDayToProduct(date);
        //}

        //
        _notifyPropertyChanged("StartDate");
        _notifyPropertyChanged("EndDate");
    }
    public void WriteSummy(string path_)
    {
        var list = new List<string>();

        list.Add($"Analsis File Count:{_listFile.Count}");
        list.AddRange(_listFile.Select(x_ => Path.GetFileName(x_)));
        list.Add("\n");

        list.Add($"Analsis Time : {_endTime - _startTime}");
        list.Add($"Start   Time : {_startTime}");
        list.Add($"End     Time : {_endTime}");
        list.Add("\n");


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

    public void WriteText(string path_, Type classType_)
    {
        _convertReportLog.WriteEventText(path_, classType_);
    }
    public void WriteText(string path_, Type classType_, long selected_)
    {
        var find = _listAnalysis.Find(f_ => f_.GetType() == classType_);
        find?.WriteText(path_, selected_);

    }


    public IEnumerable<T> ListEvent<T>() where T : LogEventBase => _convertReportLog.ListEvent<T>();
    public IEnumerable<object> ListEvent(Type classType_)=>
    //{ 
        _convertReportLog.ListEvent(classType_);
    //}

    //public void WriteText(string outFolder_)
    //{
    //    WriteSummy(outFolder_ + @"\Analysis.txt");

    //    _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdown.csv", (long)SelectData.ECLUSION);
    //    _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdownAll.csv");
    //    //
    //    _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutIn.csv");
    //    _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutInDuplication.csv", (long)SelectData.ECLUSION);
    //    _listCheckOutIn.WriteDuplicationText(outFolder_ + @"\ListJoinEventCheckOutIn.csv");
    //    //
    //    _listLicenseCount.WriteText(outFolder_ + @"\ListAnalysisLicenseCount.csv");
    //    _listLicenseCount.WriteText(outFolder_ + @"\LicenseCountDate.csv", TimeSpan.TicksPerDay);
    //    _listLicenseCount.WriteText(outFolder_ + @"\LicenseCountHour.csv", TimeSpan.TicksPerHour);
    //    _listLicenseCount.WriteText(outFolder_ + @"\LicenseCount30Minute.csv", TimeSpan.TicksPerMinute * 30);
    //    //
    //    _listUserDuration.WriteText(outFolder_ + @"\DurationUser.csv",(long) SelectData.ALL);
    //    _listHostDuration.WriteText(outFolder_ + @"\DurationHost.csv", (long)SelectData.ALL);
    //    _listUserHostDuration.WriteText(outFolder_ + @"\DurationUserHost.csv", (long)SelectData.ALL);

    //}

    public bool IsProductChecked(string product_) => ListResultProduct.Where(x_ => x_.Name == product_).Select(x_ => x_.IsChecked).FirstOrDefault();
    //{
    //foreach (var view in ListResultProduct)
    //{
    //    if (view.Name == product_)
    //    {
    //        return view.IsChecked;
    //    }
    //}
    //return true;
    //}


    public void SetData(DateTime? date_, AnalysisGroup group_)
    {
        //if (_convertReportLog.ListEvent.Count() == 0)
        //{
        //    return;
        //}

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

    //public void SetAllPlot(WpfPlot plot_)
    //{
    //    plot_.Plot.Clear();
    //    plot_.Plot.Title("Product Date - Count");
    //    plot_.Plot.XLabel("Date");
    //    plot_.Plot.YLabel("Count");
    //    //var listX = new List<double>();
    //    var listYProduct = new List<LicenseView>();

    //    foreach (var date in ListDate) 
    //    {
    //        listYProduct.AddRange(_listLicenseCount.ListLicenseProduct(date));
    //    }

    //    foreach (var product in ListProduct)
    //    {
    //        if (IsChecked(product) == false)
    //        {
    //            continue;
    //        }

    //        var listYCount = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count);
    //        var listYMax = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Max);

    //        var plotCount = plot_.Plot.Add.Scatter(ListDate.ToArray(), listYCount.ToArray());
    //        plotCount.LegendText= product + " Count";
    //        var plotMax = plot_.Plot.Add.Scatter(ListDate.ToArray(), listYMax.ToArray());
    //        plotMax.LegendText = product + " Max";
    //    }

    //    plot_.Plot.Axes.DateTimeTicksBottom();
    //    plot_.Refresh();
    //}

    //public void SetDatePlot(WpfPlot plot_, DateTime date_, int group_)
    //{
    //    plot_.Plot.Clear();
    //    plot_.Plot.Title($"Product {date_.ToShortDateString()} - Count");
    //    plot_.Plot.XLabel("Time(30 Minitue)");
    //    plot_.Plot.YLabel("Count");

    //    long timeSpan = 30 * TimeSpan.TicksPerMinute;

    //    var listX = new List<DateTime>();
    //    var listYProduct = new List<LicenseView>();

    //    for (var time = date_; time < date_.AddTicks(TimeSpan.TicksPerDay); time = time.AddTicks(timeSpan))
    //    {
    //        listX.Add(time);

    //        switch ((ANALYSIS_GROUP)group_)
    //        {
    //            case ANALYSIS_GROUP.USER:
    //                break;
    //            case ANALYSIS_GROUP.HOST:
    //                break;
    //            case ANALYSIS_GROUP.USER_HOST:
    //                break;
    //            case ANALYSIS_GROUP.NONE:
    //            default:
    //                listYProduct.AddRange(_listLicenseCount.ListLicenseProduct(time, timeSpan));
    //                break;
    //        }
    //    }

    //    foreach (var product in ListProduct)
    //    {
    //        if (IsChecked(product) == false)
    //        {
    //            continue;
    //        }

    //        var listYCount = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count);
    //        var listYMax = listYProduct.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Max);

    //        var plotCount = plot_.Plot.Add.Scatter(listX.ToArray(), listYCount.ToArray());
    //        plotCount.LegendText = product + " Count";
    //        var plotMax = plot_.Plot.Add.Scatter(listX.ToArray(), listYMax.ToArray());
    //        plotMax.LegendText = product + " Max";
    //    }

    //    plot_.Plot.Axes.DateTimeTicksBottom();
    //    plot_.Refresh();

    //}

    public void SetPlot(WpfPlot plot_, DateTime? date_, AnalysisGroup group_)
    {
        plot_.Plot.Clear();
        //if (_convertReportLog.ListEvent.Count() == 0)
        //{
        //    return;
        //}
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

        var listY = new Dictionary<string, List<double>>();
        switch ((AnalysisGroup)group_)
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
