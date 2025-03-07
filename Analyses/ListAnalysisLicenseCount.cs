/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
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
internal sealed class ListAnalysisLicenseCount : List<AnalysisLicenseCount>
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ListAnalysisLicenseCount()
    {
    }

    /// <summary>
    /// 文字列化のヘッダー
    /// </summary>
    public string Header
    {
        get
        {
            var list = new List<string>();
            list.Add("Date");
            list.Add("Time");
            foreach (var product in _listProduct)
            {
                list.Add($"{product}[Use]");
                list.Add($"{product}[Have]");
                //list.Add($"{product}[OutIn]");
            }
            return string.Join(",", list);
        }
    }

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

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
    /// プロダクト リスト
    /// </summary>
    private SortedSet<string> _listProduct = new();

    /// <summary>
    /// 集計処理でのカウント
    /// </summary>
    private Dictionary<string, int> _listCount = new();

    /// <summary>
    /// 最大数
    /// </summary>
    private Dictionary<string, int> _listHave = new();

    /// <summary>
    /// ログの数値(=_listCount)
    /// </summary>
    private Dictionary<string, int> _listCountOutIn = new();


    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    /// <param name="listCheckOutIn_">チェックアウト チェックイン 結合情報</param>
    public void Analysis(AnalysisReportLog log_, ListAnalysisCheckOutIn listCheckOutIn_)
    {
        _listProduct.UnionWith(log_.ListProduct);

        _clearCount();

        int count = 0;
        int max = log_.ListEvent.Count;

        ProgressCount?.Invoke(0, max, _ANALYSIS);
        foreach (var ev in log_.ListEvent)
        {
            if (ev.EventDateTime ==LogEventBase.NotAnalysisEventTime)
            {
                // 時間が定まってないため処理しない
                continue;
            }

            if (ev is LogEventProduct eventProduct)
            {
                // プロダクト の場合 プロダクトの最大数を更新
                if (_setHave(eventProduct.Product, eventProduct.Count) == true)
                {
                    _add(eventProduct);
                }
            }

            if (ev is LogEventCheckOut eventCheckOut)
            {
                // チェックアウトの場合、カウント増加
                if (_setCountUp(eventCheckOut.Product, eventCheckOut.CountCurrent) == true)
                {
                    _add(eventCheckOut);
                }
            }
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
                    LogFile.Instance.WriteLine($"CheckIn: {eventCheckIn.EventNumber} {eventCheckIn.EventDateTime} {eventCheckIn.Product}");

                    var checkOut = listCheckOutIn_.Find(eventCheckIn);
                    if (checkOut != null)
                    {
                        LogFile.Instance.WriteLine($"CheckOut: {checkOut.EventNumber} {checkOut.EventDateTime} {checkOut.Product}");
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
            }
            if (ev is LogEventShutdown eventShutdown)
            {
                // シャットダウン時に、一旦クリア
                _clearCount();
                _add(eventShutdown);
            }
            if (ev is LogEventTimeStamp eventTimeStamp)
            {
                _add(eventTimeStamp);

            }
            ProgressCount?.Invoke(++count, max);
        }

        //var minDate = this.Select(x => x.EventBase.EventDate()).Min();
        //var maxDate = this.Select(x => x.EventBase.EventDate()).Max();

        //count = 0;
        //max = (maxDate - minDate).Days + 1;

        //ProgressCount?.Invoke(0, max, _ANALYSIS + "Days");
        //for (var date = minDate; date < maxDate.AddTicks(TimeSpan.TicksPerDay); date = date.AddTicks(TimeSpan.TicksPerDay))
        //{
        //    _listDayToProduct[date] = _getDayToProduct(date);
        //    ProgressCount?.Invoke(++count, max);
        //}

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

    //private IEnumerable<LicenseView> _getDayToProduct(DateTime date_)
    //{
    //    var rtn = new List<LicenseView>();

    //    var listSelectDay = this.Where(x_ => x_.EventBase.EventDate() == date_);
    //    foreach (var product in _listProduct)
    //    {
    //        var view = new LicenseView()
    //        {
    //            Name = product,
    //            Date = date_,
    //            Count =  0,
    //            Max = 0,
    //        };

    //        if (listSelectDay?.Any() == true)
    //        {
    //            view.Count = listSelectDay?.Select(x_ => x_.CountProduct[product]).Max() ?? 0;
    //            view.Max = listSelectDay?.Select(x_ => x_.MaxProduct[product]).Max() ?? 0;
    //        }
    //        //var minTime = listSelectDay?.Select(x_ => x_.EventBase.EventDateTime).Min() ?? DateTime.Now;
    //        //var maxTime = listSelectDay?.Select(x_ => x_.EventBase.EventDateTime).Max() ?? DateTime.Now;

    //        rtn.Add(view);
    //    }
    //    return rtn;
    //}


    //public IEnumerable<LicenseView> ListDayToProduct(DateTime date_)
    //{
    //    if (_listDayToProduct.TryGetValue(date_, out var rtn) == true)
    //    {
    //        return rtn;
    //    }
    //    return new List<LicenseView>();
    //}

    //public List<LicenseView> GetCount(DateTime date_, long timeSpan_, string product_)
    //{
    //    var rtn = new List<LicenseView>();

    //    //TimeSpan.TicksPerDay
    //    var listSelectDay = this.Where(x_ => x_.EventBase.EventDateTime.Date == date_);
    //    for(var time = date_;time<date_.AddDays(1); time = time.AddTicks(timeSpan_))
    //    {
    //        var view = new LicenseView()
    //        {
    //            Date = time,
    //            Count = listSelectDay?.Select(x_ => x_.CountProduct[product_]).Max() ?? 0,
    //            Max = listSelectDay?.Select(x_ => x_.MaxProduct[product_]).Max() ?? 0
    //        };

    //        rtn.Add(view);
    //    }

    //    return rtn;
    //}

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
        //var list = (timeSpan_ ==-1) ? this.Where(x_ => (x_.EventBase.EventDateTime.Date == date_) || flg): this.Where(x_ => (x_.EventBase.EventDateTimeUnit(timeSpan_) == date_));
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
    public Dictionary<string, List<double>> ListPlot(List<DateTime> listX_, long timeSpan_)
    {
        var rtn = new Dictionary<string, List<double>>();

        // 初期化
        foreach (var product in _listProduct)
        {
            if (AnalysisManager.Instance.IsChecked(product) == false)
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
                if (AnalysisManager.Instance.IsChecked(product) == false)
                {
                    continue;
                }

                if (listView.Count > 0)
                {
                    rtn[product + _PLOT_MAX].Add(listView.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Max).Max());
                    rtn[product + _PLOT_COUNT].Add(listView.Where(x_ => x_.Name == product).Select(x_ => (double)x_.Count).Max());
                }
                else
                {
                    /// ない場合は非表示
                    rtn[product + _PLOT_MAX].Add(double.NaN);
                    rtn[product + _PLOT_COUNT].Add(double.NaN);
                }
            }
        }

        return rtn;
    }


    /// <summary>
    /// 文字列のリスト化
    /// </summary>
    private List<string> _listToString()
    {
        var rtn = new List<string>();
        foreach (var d in this)
        {
            var add = new List<string>();
            var dateTime = d.EventBase.EventDateTime.ToString().Split(" ");
            add.Add(dateTime[0]);
            add.Add(dateTime[1]);
            foreach (var product in _listProduct)
            {
                add.Add(d.CountProduct[product].ToString());
                add.Add(d.MaxProduct[product].ToString());
                //add.Add(d.OutInProduct[product].ToString());
            }
            rtn.Add(string.Join(",", add));
        }
        return rtn;
    }

    /// <summary>
    /// 指定間隔での結果リスト
    /// </summary>
    /// <param name="timeSpan_"></param>
    /// <returns></returns>
    private List<string> _listTimeSpanString(long timeSpan_ = TimeSpan.TicksPerDay)
    {
        var rtn = new List<string>();

        var listNowMax = new Dictionary<string, int>();
        _listProduct.ToList().ForEach(product => listNowMax[product] = 0);

        var listTimeSpan = _getListTimeSpan(timeSpan_);
        foreach (var dateTimeSpan in listTimeSpan)
        {
            var add = new List<string>();
            add.Add(dateTimeSpan.Date.ToShortDateString());
            add.Add(dateTimeSpan.TimeOfDay.ToString());
            //
            var listData = this.Where(d_ => d_.EventBase.EventDateTimeUnit(timeSpan_) == dateTimeSpan);
            foreach (var product in _listProduct)
            {
                if ((listData?.Count() ?? 0) == 0)
                {
                    add.Add("0");
                    add.Add(listNowMax[product].ToString());
                    continue;
                }
                var countMax = listData?.Select(x_ => x_.CountProduct[product]).Max() ?? 0;
                var haveMax = listData?.Select(x_ => x_.MaxProduct[product]).Max() ?? 0;
                var outIn = listData?.Select(x_ => x_.OutInProduct[product]).Max() ?? 0;

                add.Add(countMax.ToString());
                add.Add(haveMax.ToString());
                //add.Add(outIn.ToString());

                listNowMax[product] = haveMax;
            }

            rtn.Add(string.Join(",", add));
        }

        return rtn;
    }

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="timeSpan_">間隔時間</param>
    public void WriteText(string path_, long timeSpan_=-1)
    {
        var list = new List<string>();
        list.Add(Header);
        list.AddRange((timeSpan_ ==-1) ?_listToString(): _listTimeSpanString(timeSpan_));
        File.WriteAllLines(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    /// <summary>
    /// 時間分割
    /// </summary>
    /// <param name="timeSpan_">間隔時間</param>
    /// <returns></returns>
    private List<DateTime> _getListTimeSpan(long timeSpan_)
    {
        var rtn = new List<DateTime>();

        var minDate = this.Select(x => x.EventBase.EventDate()).Min();
        var maxDate = this.Select(x => x.EventBase.EventDate()).Max();

        for (var date = minDate; date < maxDate.AddTicks(TimeSpan.TicksPerDay); date = date.AddTicks(timeSpan_))
        {
            rtn.Add(date);
        }

        return rtn;
    }
}
