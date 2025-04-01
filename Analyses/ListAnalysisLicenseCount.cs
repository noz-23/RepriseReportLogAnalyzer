/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// ライセンスの利用状況集計
/// 　EventBase   :基本となるイベント(時間)
/// 　CountProduct:プロダクト-集計処理でのカウント
/// 　MaxProduct  :プロダクト-最大数
/// 　OutInProduct:プロダクト-ログの数値
/// </summary>
[Sort(2)]
[Table("TbAnalysisLicenseCount")]
[Description("Product License Usage(Count)"), Category("Analyses")]
internal sealed class ListAnalysisLicenseCount : List<AnalysisLicenseCount>, IAnalysisOutputFile
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ListAnalysisLicenseCount()
    {
        ProgressCount = null;
    }

    /// <summary>
    /// コンボボックスの項目
    /// </summary>
    public static ListStringLongPair ListSelect { get => _listSelect; }
    private static ListStringLongPair _listSelect = new()
    {
        new("Interval Event", _NO_TIME_STAMP),
        new("Interval 1 Day", TimeSpan.TicksPerDay),
        new("Interval 1 Hour", TimeSpan.TicksPerHour),
        new("Interval 30 Minute", 30*TimeSpan.TicksPerMinute),
    };

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountCallBack? ProgressCount;

    /// <summary>
    /// 全体の最大カウント(プロット用)
    /// </summary>
    public int Max { get => (this.Any() == true) ? (this.SelectMany(x_ => x_.ListCount).Where(x_ => AnalysisManager.Instance.IsProductChecked(x_.Key) == true).Max(x_ => x_.Value.ServerHave)) : (0); }

    /// <summary>
    /// 解析内容
    /// </summary>
    private readonly string _ANALYSIS = "[License Count]";

    /// <summary>
    /// プロット用カウント数文字
    /// </summary>
    private readonly string _PLOT_COUNT = "[Count]";

    /// <summary>
    /// プロット用最大数文字
    /// </summary>
    private readonly string _PLOT_HAVE = "[Have ]";

    /// <summary>
    /// 基本的な ログ イベント
    /// </summary>
    private const long _NO_TIME_STAMP = -1;

    /// <summary>
    /// プロダクト リスト
    /// </summary>
    private SortedSet<string> _listProduct = new();


    /// <summary>
    /// 解析
    /// </summary>
    /// <param name="log_"></param>
    public void Analysis(ConvertReportLog log_)
    {
        var listBase = log_.ListEvent<ILicenseCount>();
        if (listBase.Any() == false)
        {
            return;
        }
        // プロダクトのコピー
        _listProduct.UnionWith(log_.ListProduct);

        // ログの数値
        var listCount = new SortedList<string, AnalysisLicenseCount.LicenseCount>();

        _clearCount(listCount);


        int count = 0;
        int max = listBase.Count();

        ProgressCount?.Invoke(0, max, _ANALYSIS);
        foreach (var ev in listBase)
        {
            if (ev.SetCount(listCount) == true)
            {
                _add(ev as LogEventBase, listCount);
            }
            ProgressCount?.Invoke(++count, max);
        }
        Sort((a_, b_) => (int)(a_.EventBase.EventNumber - b_.EventBase.EventNumber));
    }

    /// <summary>
    /// 各種カウンタのクリア
    /// </summary>
    private void _clearCount(SortedList<string, AnalysisLicenseCount.LicenseCount> listCount_)
    {
        foreach (var product in _listProduct)
        {
            listCount_[product] = new();
        }
    }

    /// <summary>
    /// 結果の追加
    /// </summary>
    /// <param name="logEventBase_">ログ イベント</param>
    public void _add(LogEventBase? logEventBase_, IDictionary<string, AnalysisLicenseCount.LicenseCount> listCount_)
    {
        if (logEventBase_ == null)
        {
            return;
        }

        Add(new AnalysisLicenseCount(logEventBase_, listCount_));
    }

    /// <summary>
    /// リスト表示するライセンス数
    /// </summary>
    /// <param name="date_">指定日付(null:一覧)</param>
    /// <param name="timeSpan_">表示間隔</param>
    /// <returns></returns>
    public async Task<List<LicenseView>> ListView(DateTime? date_, long timeSpan_ = TimeSpan.TicksPerDay)
    {
        var rtn = new List<LicenseView>();

        var flg = (date_ == null);
        var list = this.Where(x_ => (x_.EventBase.EventDateTimeUnit(timeSpan_) == date_) || flg);

        foreach (var product in _listProduct)
        {
            var view = new LicenseView()
            {
                Name = product,
                Count = 0,
                Have = 0,
            };

            if (list.Any() == true)
            {
                // ない場合は0入れ
                view.Count = list.Select(x_ => x_.ListCount[product].Count).Max();
                view.Have = list.Select(x_ => x_.ListCount[product].ServerHave).Max();
            }

            rtn.Add(view);
        }
        await Task.Delay(0);

        return rtn;
    }

    /// <summary>
    /// プロットするライセンス数
    /// </summary>
    /// <param name="listX_">対応する時間リスト</param>
    /// <param name="timeSpan_">時間間隔</param>
    /// <returns>Key:データ内容/Value:対応するデータ</returns>
    public async Task<SortedList<string, List<double>>> ListPlot(List<DateTime> listX_, long timeSpan_)
    {
        var rtn = new SortedList<string, List<double>>();

        // 初期化
        foreach (var product in _listProduct)
        {
            if (AnalysisManager.Instance.IsProductChecked(product) == false)
            {
                continue;
            }
            rtn[product + _PLOT_HAVE] = new();
            rtn[product + _PLOT_COUNT] = new();
        }

        // データ入れ
        foreach (var time in listX_)
        {
            var listView = await ListView(time, timeSpan_);

            foreach (var product in _listProduct)
            {
                if (AnalysisManager.Instance.IsProductChecked(product) == false)
                {
                    continue;
                }

                rtn[product + _PLOT_HAVE].Add((listView.Count == 0) ? double.NaN : listView.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Have).Max());
                rtn[product + _PLOT_COUNT].Add((listView.Count == 0) ? double.NaN : listView.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count).Max());
            }
        }

        return rtn;
    }

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="timeSpan_">間隔時間</param>
    public async Task WriteText(string path_, long timeSpan_ = _NO_TIME_STAMP)
    {
        var list = new List<string>();
        // ヘッダー
        list.Add(Header(timeSpan_));
        // データ
        list.AddRange(ListValue(timeSpan_).Select(x_ => string.Join(",", x_)));
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    /// <summary>
    /// 時間分割
    /// </summary>
    /// <param name="timeSpan_">間隔時間</param>
    /// <returns></returns>
    private SortedSet<DateTime> _getListTimeSpan(long timeSpan_)
    {
        var rtn = new SortedSet<DateTime>();

        var minDate = this.First().EventBase.EventDate();
        var maxDate = this.Last().EventBase.EventDate();

        for (var date = minDate; date < maxDate.AddTicks(TimeSpan.TicksPerDay); date = date.AddTicks(timeSpan_))
        {
            rtn.Add(date);
        }

        return rtn;
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    /// <param name="timeSpan_"></param>
    /// <returns></returns>
    public string Header(long timeSpan_) => "'" + string.Join("','", ListHeader(timeSpan_).Select(x_ => x_.Key)) + "'";

    /// <summary>
    /// リスト化したヘッダー
    /// </summary>
    /// <param name="timeSpan_"></param>
    /// <returns></returns>
    public ListStringStringPair ListHeader(long timeSpan_)
    {
        var rtn = new ListStringStringPair();
        rtn.Add(new("Date", ToDataBase.GetDatabaseType(typeof(DateTime))));
        rtn.Add(new("Time", ToDataBase.GetDatabaseType(typeof(DateTime))));
        //
        foreach (var product in _listProduct)
        {
            rtn.Add(new($"{product}[Use]", ToDataBase.GetDatabaseType(typeof(long))));
            rtn.Add(new($"{product}[Have]", ToDataBase.GetDatabaseType(typeof(long))));
            //rtn.Add(new($"{product}[OutIn]", ToDataBase.GetDatabaseType(typeof(long))));
        }

        return rtn;
    }

    /// <summary>
    /// リスト化したデータ
    /// </summary>
    /// <param name="timeSpan_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListValue(long timeSpan_)
    {
        if (timeSpan_ == _NO_TIME_STAMP)
        {
            // 集計なしに出力
            return this.Select(x_ => x_.ListValue());
        }

        var rtn = new List<List<string>>();
        //
        var listNowMax = new SortedList<string, int>();
        _listProduct.ToList().ForEach(product => listNowMax[product] = 0);
        //
        foreach (var time in _getListTimeSpan(timeSpan_))
        {
            var list = new List<string>();
            list.Add(time.Date.ToShortDateString());
            list.Add(time.TimeOfDay.ToString());
            // 分割した時間内のデータを追加
            var listTime = this.Where(d_ => d_.EventBase.EventDateTimeUnit(timeSpan_) == time);
            foreach (var product in _listProduct)
            {
                if ((listTime?.Count() ?? 0) == 0)
                {
                    list.Add("0");
                    list.Add("0");
                    //add.Add("0");
                    continue;
                }
                var countMax = listTime?.Select(x_ => x_.ListCount[product].Count).Max() ?? 0;
                var haveMax = listTime?.Select(x_ => x_.ListCount[product].ServerHave).Max() ?? 0;
                var outInMax = listTime?.Select(x_ => x_.ListCount[product].CheckOutInCurrent).Max() ?? 0;

                list.Add(countMax.ToString(CultureInfo.InvariantCulture));
                list.Add(haveMax.ToString(CultureInfo.InvariantCulture));
                //add.Add(outInMax.ToString(CultureInfo.InvariantCulture));
            }
            rtn.Add(list);
        }

        return rtn;
    }
}
