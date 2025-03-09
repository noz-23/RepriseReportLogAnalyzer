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
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using RLMLogReader.Extensions;
using ScottPlot;
using ScottPlot.Panels;
using ScottPlot.WPF;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Managers;

class AnalysisManager : INotifyPropertyChanged
{
    public static AnalysisManager Instance = new AnalysisManager();
    private AnalysisManager()
    {
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
    /// 解析内容
    /// </summary>
    private const string _ANALYSIS = "[File Read]";

    public ObservableCollection<LicenseView> ListResultProduct { get; private set; } = new();

    public ObservableCollection<LicenseView> ListResultGroup { get; private set; } = new();

    private readonly ListAnalysisStartShutdown _listStartShutdown = new();
    private readonly ListAnalysisCheckOutIn _listCheckOutIn = new();
    private readonly ListAnalysisLicenseCount _listLicenseCount = new();
    //
    private readonly ListAnalysisLicenseGroup _listUserDuration = new(ANALYSIS_GROUP.USER);
    private readonly ListAnalysisLicenseGroup _listHostDuration = new(ANALYSIS_GROUP.HOST);
    private readonly ListAnalysisLicenseGroup _listUserHostDuration = new(ANALYSIS_GROUP.USER_HOST);

    private AnalysisReportLog _analysisReortLog =new AnalysisReportLog();

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    private ProgressCountDelegate? _progressCount = null;
    //
    //private readonly Dictionary<DateTime, IEnumerable<LicenseView>> _listDateToView = new Dictionary<DateTime, IEnumerable<LicenseView>>();
    //private Dictionary<DateTime, IEnumerable<LicenseView>> ListDayToProduct() => _listLicenseCount.ListDayToProduct();

    //private readonly Dictionary<string, bool> _listProductChecked = new ();


    // _analysisReortLog があるが確実にデータがはいっているタイミングで更新する
    public SortedSet<string> ListProduct { get; private set; } = new();
    public SortedSet<string> ListUser { get; private set; } = new();
    public SortedSet<string> ListHost { get; private set; } = new();
    public SortedSet<string> ListUserHost { get; private set; } = new();
    public SortedSet<DateTime> ListDate { get; private set; } = new();

    public DateTime? StartDate { get => (ListDate.Count() == 0) ? null : ListDate.FirstOrDefault(); }
    public DateTime? EndDate { get => (ListDate.Count() == 0) ? null : ListDate.LastOrDefault(); }

    public void SetProgressCount(ProgressCountDelegate progressCount_)
    {
        _progressCount = progressCount_;
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
    }

    public void Analysis(IEnumerable<string> listFile)
    {
        int count = 0;
        int max = listFile.Count();
        _progressCount?.Invoke(0, max, _ANALYSIS);

        //var analysis = new AnalysisReportLog();
        foreach (string path_ in listFile)
        {
            LogFile.Instance.WriteLine($"LogAnalysis: {path_}");
            _analysisReortLog.StartAnalysis(path_);
            _progressCount?.Invoke(++count, max);
        }
        _analysisReortLog.EndAnalysis();
        //_calendarShow(analysis.ListDate);

        //AnalysisManager.Instance.Analysis(analysis);
        _analysis();
    }

    //public void Analysis(AnalysisReportLog analysis_)
    private void _analysis()

    {
        //_analysis = analysis_;

        Clear();
        //
        //ListProduct.AddRange(_analysisReortLog.ListProductEvent.Select(x_ => x_.Product));
        ListProduct.AddRange(_analysisReortLog.ListProduct);
        ListUser.AddRange(_analysisReortLog.ListUser);
        ListHost.AddRange(_analysisReortLog.ListHost);
        ListUserHost.AddRange(_analysisReortLog.ListUserHost);
        ListDate.AddRange(_analysisReortLog.ListDate);
        //
        //foreach (var product in ListProduct)
        //{
        //    _listProductChecked[product]=true;
        //}
        //
        _listStartShutdown.Analysis(_analysisReortLog);
        _listCheckOutIn.Analysis(_analysisReortLog, _listStartShutdown.ListWithoutSkip());
        _listLicenseCount.Analysis(_analysisReortLog, _listCheckOutIn);
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

    public void WriteText(string path_, Type classType_)
    {
        _analysisReortLog.WriteEventText(path_, classType_);
    }

    public void WriteText(string outFolder_)
    {
        _analysisReortLog.WriteSummy(outFolder_ + @"\Analysis.txt");

        _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdown.csv", true);
        _listStartShutdown.WriteText(outFolder_ + @"\ListAnalysisStartShutdownAll.csv");
        //
        _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutIn.csv");
        _listCheckOutIn.WriteText(outFolder_ + @"\ListAnalysisCheckOutInDuplication.csv", JoinEventCheckOutIn.NO_DUPLICATION);
        _listCheckOutIn.WriteDuplicationText(outFolder_ + @"\ListJoinEventCheckOutIn.csv");
        //
        _listLicenseCount.WriteText(outFolder_ + @"\ListAnalysisLicenseCount.csv");
        _listLicenseCount.WriteText(outFolder_ + @"\LicenseCountDate.csv", TimeSpan.TicksPerDay);
        _listLicenseCount.WriteText(outFolder_ + @"\LicenseCountHour.csv", TimeSpan.TicksPerHour);
        _listLicenseCount.WriteText(outFolder_ + @"\LicenseCount30Minute.csv", TimeSpan.TicksPerMinute * 30);
        //
        _listUserDuration.WriteText(outFolder_ + @"\DurationUser.csv");
        _listHostDuration.WriteText(outFolder_ + @"\DurationHost.csv");
        _listUserHostDuration.WriteText(outFolder_ + @"\DurationUserHost.csv");

    }

    public bool IsChecked(string product_) => ListResultProduct.Where(x_ => x_.Name == product_).Select(x_ => x_.IsChecked).FirstOrDefault();
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


    public void SetData(DateTime? date_, ANALYSIS_GROUP group_)
    {
        //if (_analysisReortLog.ListEvent.Count() == 0)
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
            case ANALYSIS_GROUP.USER: ListResultGroup.AddRange(_listUserDuration.ListView(date_)); break;
            case ANALYSIS_GROUP.HOST: ListResultGroup.AddRange(_listHostDuration.ListView(date_)); break;
            case ANALYSIS_GROUP.USER_HOST: ListResultGroup.AddRange(_listUserHostDuration.ListView(date_)); break;
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

    public void SetPlot(WpfPlot plot_, DateTime? date_, ANALYSIS_GROUP group_)
    {
        plot_.Plot.Clear();
        //if (_analysisReortLog.ListEvent.Count() == 0)
        //{
        //    return;
        //}
        var title = string.Empty;
        var xlabel = (date_ ==null) ? $"Date":$"[{date_.GetValueOrDefault().ToShortDateString()}] Time (30 Minute)";
        var ylabel = string.Empty;
        var pos = Alignment.LowerRight;
        switch (group_)
        {
            case ANALYSIS_GROUP.USER: 
            case ANALYSIS_GROUP.HOST: 
            case ANALYSIS_GROUP.USER_HOST:
                title = (date_ == null) ? $"Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}" : $"[{date_.GetValueOrDefault().ToShortDateString()}] Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}";
                ylabel = $"Top {ListAnalysisLicenseGroup.TOP_PLOT_USE} - {group_.Description()}";
                plot_.Plot.Axes.SetLimitsY(bottom: ListAnalysisLicenseGroup.TOP_PLOT_USE, top: 1);
                pos = Alignment.LowerLeft;
                break;
            default:
            case ANALYSIS_GROUP.NONE:
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
        switch ((ANALYSIS_GROUP)group_)
        {
            case ANALYSIS_GROUP.USER: listY = _listUserDuration.ListPlot(listX, timeSpan); break;
            case ANALYSIS_GROUP.HOST: listY = _listHostDuration.ListPlot(listX, timeSpan); break;
            case ANALYSIS_GROUP.USER_HOST: listY = _listUserHostDuration.ListPlot(listX, timeSpan); break;
            default:
            case ANALYSIS_GROUP.NONE: listY = _listLicenseCount.ListPlot(listX, timeSpan); break;
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
