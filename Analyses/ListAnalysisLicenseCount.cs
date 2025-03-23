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
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// ライセンスの利用状況集計
/// 　EventBase   :基本となるイベント(時間)
/// 　CountProduct:プロダクト-集計処理でのカウント
/// 　MaxProduct  :プロダクト-最大数
/// 　OutInProduct:プロダクト-ログの数値
/// </summary>
[Sort(2)][Table("TbAnalysisLicenseCount")]
internal sealed class ListAnalysisLicenseCount : List<AnalysisLicenseCount>, IAnalysisOutputFile
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ListAnalysisLicenseCount()
    {
    }

    /// <summary>
    /// コンボボックスの項目
    /// </summary>
    public static ListStringLongPair ListSelect { get => _listSelect; }
    private static ListStringLongPair _listSelect= new ()
    {
        new("Interval Event", _NO_TIME_STAMP),
        new("Interval 1 Day", TimeSpan.TicksPerDay),
        new("Interval 1 Hour", TimeSpan.TicksPerHour),
        new("Interval 30 Minute", 30*TimeSpan.TicksPerMinute),
    };


    public int Max 
    {
        get
        {
            //return this.SelectMany(x_ => x_.MaxProduct).Where(x_ => AnalysisManager.Instance.IsProductChecked(x_.Key) == true).Max(x_ => x_.Value);
            int rtn = 0;
            foreach (var list in this)
            {
                foreach (var product in list.MaxProduct.Keys)
                {
                    if (AnalysisManager.Instance.IsProductChecked(product) == false)
                    {
                        continue;
                    }
                    rtn = Math.Max(rtn, list.MaxProduct[product]);
                }
            }
            return rtn;
        }

    }

    /// <summary>
    /// 解析内容
    /// </summary>
    private const string _ANALYSIS = "[License Count]";

    /// <summary>
    /// プロット用カウント数文字
    /// </summary>
    private const string _PLOT_COUNT = "[Count]";

    /// <summary>
    /// プロット用最大数文字
    /// </summary>
    private const string _PLOT_MAX = "[ Max ]";

    /// <summary>
    /// 基本的な ログ イベント
    /// </summary>
    private const long _NO_TIME_STAMP = -1;

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

    /// <summary>
    /// プロダクト リスト
    /// </summary>
    private SortedSet<string> _listProduct = new();

    /// <summary>
    /// 集計処理でのカウント
    /// </summary>
    private SortedDictionary<string, int> _listCount = new();

    /// <summary>
    /// 最大数
    /// </summary>
    private SortedDictionary<string, int> _listHave = new();

    /// <summary>
    /// ログの数値(=_listCount)
    /// </summary>
    private SortedDictionary<string, int> _listCountOutIn = new();


    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    /// <param name="listCheckOutIn_">チェックアウト チェックイン 結合情報</param>
    public void Analysis(ConvertReportLog log_, ListAnalysisCheckOutIn listCheckOutIn_)
    {
        if (listCheckOutIn_.Any() == false)
        {
            return;
        }

        // プロダクトのコピー
        _listProduct.UnionWith(log_.ListProduct);

        _clearCount();

        var listBase = log_.ListEvent<LogEventBase>();

        int count = 0;
        int max = listBase.Count();

        ProgressCount?.Invoke(0, max, _ANALYSIS);
        foreach (var ev in listBase)
        {
            if (ev.EventDateTime ==LogEventBase.NotAnalysisEventTime)
            {
                // 時間が定まってないため処理しない
                continue;
            } else
            if (ev is LogEventProduct eventProduct)
            {
                // プロダクト の場合 プロダクトの最大数を更新
                if (_setHave(eventProduct.Product, eventProduct.Count) == true)
                {
                    _add(eventProduct);
                }
            } else
            if (ev is LogEventCheckOut eventCheckOut)
            {
                // チェックアウトの場合、カウント増加
                if (_setCountUp(eventCheckOut.Product, eventCheckOut.CountCurrent) == true)
                {
                    _add(eventCheckOut);
                }
            }else
            if (ev is LogEventCheckIn eventCheckIn)
            {
                // チェックインの場合、カウント減少
                var flg = true;
                if (string.IsNullOrEmpty(eventCheckIn.Product) == false)    
                {
                    flg = _setCountDown(eventCheckIn.Product, eventCheckIn.CountCurrent);
                }
                else
                {
                    // チェックインにはプロダクトの情報がない場合があるため
                    //LogFile.Instance.WriteLine($"CheckIn: {eventCheckIn.EventNumber} {eventCheckIn.EventDateTime} {eventCheckIn.Product}");

                    var checkOut = listCheckOutIn_.Find(eventCheckIn);
                    if (checkOut != null)
                    {
                        LogFile.Instance.WriteLine($"CheckOut: {checkOut.ToString()}");
                        flg = _setCountDown(checkOut.Product, eventCheckIn.CountCurrent);
                    }
                    else
                    {
                        LogFile.Instance.WriteLine($"Not Fount: {eventCheckIn.EventNumber} {eventCheckIn.EventDateTime} {eventCheckIn.Product}");
                    }

                }
                if (flg == true)
                {
                    _add(eventCheckIn);
                }
            }else
            if (ev is LogEventShutdown eventShutdown)
            {
                // シャットダウン時に、一旦クリア
                _clearCount();
                _add(eventShutdown);
            }else
            if (ev is LogEventTimeStamp eventTimeStamp)
            {
                _add(eventTimeStamp);
            }
            ProgressCount?.Invoke(++count, max);
        }
    }

    /// <summary>
    /// 各種カウンタのクリア
    /// </summary>
    private void _clearCount()
    {
        foreach (var product in _listProduct)
        {
            _listCount[product] = 0;
            _listHave[product] = 0;
            _listCountOutIn[product] = 0;
        }
    }

    /// <summary>
    /// 最大数の更新
    /// </summary>
    /// <param name="product_">プロダクト</param>
    /// <param name="count_">数値</param>
    /// <returns></returns>
    private bool _setHave(string product_, int count_)
    {
        if (string.IsNullOrEmpty(product_) == true)
        {
            return false;
        }
        _listHave[product_] = count_;
        return true;
    }

    /// <summary>
    /// カウンタの増加
    /// </summary>
    /// <param name="product_">プロダクト</param>
    /// <param name="count_">サーバでの数値</param>
    /// <returns></returns>
    private bool _setCountUp(string product_, int count_)
    {
        if (string.IsNullOrEmpty(product_) == true)
        {
            return false;
        }
        if (_listCountOutIn[product_] == count_)
        {
            // 重複チェック
            return false;
        }

        _listCount[product_]++;
        _listCountOutIn[product_] = count_;
        return true;
    }

    /// <summary>
    /// カウンタの減少
    /// </summary>
    /// <param name="product_">プロダクト</param>
    /// <param name="count_">サーバでの数値</param>
    /// <returns></returns>
    private bool _setCountDown(string product_, int count_)
    {
        if (string.IsNullOrEmpty(product_) == true)
        {
            return false;
        }
        if (_listCountOutIn[product_] == count_)
        {
            // 重複チェック
            return false;
        }
        _listCount[product_]--;
        _listCountOutIn[product_] = count_;
        return true;
    }

    /// <summary>
    /// 結果の追加
    /// </summary>
    /// <param name="logEventBase_">ログ イベント</param>
    public void _add(LogEventBase logEventBase_) => this.Add(new AnalysisLicenseCount(logEventBase_, _listCount, _listHave, _listCountOutIn));

    /// <summary>
    /// リスト表示するライセンス数
    /// </summary>
    /// <param name="date_">指定日付(null:一覧)</param>
    /// <param name="timeSpan_">表示間隔</param>
    /// <returns></returns>
    public List<LicenseView> ListView(DateTime? date_, long timeSpan_ = TimeSpan.TicksPerDay)
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
                Max = 0,
            };

            if (list.Count() > 0)
            {
                // ない場合は0入れ
                view.Count = list.Select(x_ => x_.CountProduct[product]).Max();
                view.Max = list.Select(x_ => x_.MaxProduct[product]).Max();
            }

            rtn.Add(view);
        }

        return rtn;
    }

    /// <summary>
    /// プロットするライセンス数
    /// </summary>
    /// <param name="listX_">対応する時間リスト</param>
    /// <param name="timeSpan_">時間間隔</param>
    /// <returns>Key:データ内容/Value:対応するデータ</returns>
    public SortedDictionary<string, List<double>> ListPlot(List<DateTime> listX_, long timeSpan_)
    {
        var rtn = new SortedDictionary<string, List<double>>();

        // 初期化
        foreach (var product in _listProduct)
        {
            if (AnalysisManager.Instance.IsProductChecked(product) == false)
            {
                continue;
            }
            rtn[product + _PLOT_MAX] = new();
            rtn[product + _PLOT_COUNT] = new();
        }

        // データ入れ
        foreach (var time in listX_)
        {
            var listView = ListView(time, timeSpan_);

            foreach (var product in _listProduct)
            {
                if (AnalysisManager.Instance.IsProductChecked(product) == false)
                {
                    continue;
                }

                rtn[product + _PLOT_MAX].Add( (listView.Count == 0) ? double.NaN : listView.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Max).Max());
                rtn[product + _PLOT_COUNT].Add( (listView.Count == 0) ? double.NaN : listView.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count).Max());
            }
        }

        return rtn;
    }

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="timeSpan_">間隔時間</param>
    public async Task WriteText(string path_, long timeSpan_= _NO_TIME_STAMP)
    {
        var list = new List<string>();
        // ヘッダー
        list.Add(Header(timeSpan_));
        // データ
        list.AddRange(ListValue(timeSpan_).Select(x_=>string.Join(",",x_)));
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

        var minDate = this.Select(x => x.EventBase.EventDate()).Min();
        var maxDate = this.Select(x => x.EventBase.EventDate()).Max();

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

    public string Header(long timeSpan_ ) => "'"+string.Join("','", ListHeader(timeSpan_).Select(x_=>x_.Key))+"'";

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
            return this.Select(x_ =>x_.ListValue());
        }

        var rtn = new List<List<string>>();
        //
        var listNowMax = new SortedDictionary<string, int>();
        _listProduct.ToList().ForEach(product => listNowMax[product] = 0);
        //
        foreach (var time in _getListTimeSpan(timeSpan_))
        {
            var add = new List<string>();
            add.Add(time.Date.ToShortDateString());
            add.Add(time.TimeOfDay.ToString());
            // 分割した時間内のデータを追加
            var listTime = this.Where(d_ => d_.EventBase.EventDateTimeUnit(timeSpan_) == time);
            foreach (var product in _listProduct)
            {
                if ((listTime?.Count() ?? 0) == 0)
                {
                    add.Add("0");
                    add.Add("0");
                    //add.Add("0");
                    continue;
                }
                var countMax = listTime?.Select(x_ => x_.CountProduct[product]).Max() ?? 0;
                var haveMax = listTime?.Select(x_ => x_.MaxProduct[product]).Max() ?? 0;
                var outInMax = listTime?.Select(x_ => x_.OutInProduct[product]).Max() ?? 0;

                add.Add(countMax.ToString());
                add.Add(haveMax.ToString());
                //add.Add(outInMax.ToString());
            }
            rtn.Add(add);
        }

        return rtn;
    }

}
