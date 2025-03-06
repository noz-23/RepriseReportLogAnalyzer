using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// グループ毎の集計
///  Key:対応グループ
///  Value:一致するチェックアウト チェックイン結合情報リスト
/// </summary>
internal sealed class ListAnalysisLicenseGroupDuration : Dictionary<string, ListAnalysisCheckOutIn>
{

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="group_"></param>
    public ListAnalysisLicenseGroupDuration(ANALYSIS_GROUP group_)
    {
        _group = group_;
    }

    /// <summary>
    /// プロット用の文字列
    /// </summary>
    private const string _GROUP_FORMAT = "{0}.{1}";

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

    /// <summary>
    /// 解析内容
    /// </summary>
    private const string _ANALYSIS = "[License Group Duration]";

    /// <summary>
    /// グループ内容
    /// </summary>
    private ANALYSIS_GROUP _group = ANALYSIS_GROUP.NONE;

    /// <summary>
    /// 文字列化のヘッダー
    /// </summary>
    public string Header { get => _group.Description() + ",Duration,Days,Count"; }

    /// <summary>
    /// 対応するグループリスト
    /// </summary>
    private SortedSet<string> _listGroup =new();

    public void Analysis(IEnumerable<string> listGroup_, ListAnalysisCheckOutIn listCheckOutIn_)
    {
        _listGroup.UnionWith(listGroup_);

        var minDate = listCheckOutIn_.Select(x => x.CheckOut().EventDate()).Min();
        var maxDate = listCheckOutIn_.Select(x => x.CheckOut().EventDate()).Max();
        //for (var date = minDate; date < maxDate.AddTicks(TimeSpan.TicksPerDay); date = date.AddTicks(TimeSpan.TicksPerDay))
        //{
        //    _listDayToGroup[date]= new List<LicenseView>();
        //}

        int count = 0;
        int max = listGroup_.Count();

        ProgressCount?.Invoke(0, max, _ANALYSIS + _group.Description());
        foreach (var group in listGroup_)
        {
            var list = new ListAnalysisCheckOutIn(listCheckOutIn_.ListDuplication().Where(x_ => x_.GroupName(_group) == group));
            this[group] = list;

            //_addDayToGroup(minDate, maxDate, group, list);
            ProgressCount?.Invoke(++count, max);
        }
    }

    //private void _addDayToGroup(DateTime minDate_, DateTime maxDate_, string group_,ListAnalysisCheckOutIn list_)
    //{
    //    for (var date = minDate_; date < maxDate_.AddTicks(TimeSpan.TicksPerDay); date = date.AddTicks(TimeSpan.TicksPerDay))
    //    {
    //        var listDay = list_.ListDuplication().Where(x_ => x_.CheckOut().EventDate() == date.Date);

    //        if (listDay.Any() == false)
    //        {
    //            continue;
    //        }

    //        var view = new LicenseView()
    //        {
    //            Name = group_,
    //            Duration = new TimeSpan(listDay.Sum(x_ => x_.Duration.Ticks)),
    //        };

    //        _listDayToGroup[date].Add(view);
    //    }
    //}

    //public IEnumerable<LicenseView> ListDayToGroup()
    //{
    //    var rtn = new List<LicenseView>();

    //    var list = new List<LicenseView>();
    //    _listDayToGroup.ToList().ForEach(x_ => list.AddRange(x_.Value));

    //    foreach (var group in _listGroup)
    //    {
    //        var view = new LicenseView()
    //        {
    //            Name = group,
    //            Count = list.Count(x_ => x_.Name == group),
    //            Max = list.Where(x_=> x_.Name == group).Max(x_ => x_.Count),
    //            Duration = new TimeSpan(list.Where(x_ => x_.Name == group).Sum(x_ => x_.Duration.Ticks))
    //        };
    //        rtn.Add(view);
    //    }

    //    return rtn;
    //}

    //public IEnumerable<LicenseView> ListDayToGroup(DateTime date_)
    //{
    //    if (_listDayToGroup.TryGetValue(date_, out var rtn) == true)
    //    {
    //        return rtn;
    //    }
    //    return new List<LicenseView>();
    //}
    //private string GetName(ANALYSIS_GROUP src_)
    //{
    //    var gm = src_.GetType().GetMember(src_.ToString());
    //    var attributes = gm[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
    //    var description = ((DescriptionAttribute)attributes[0]).Description;
    //    return description;
    //}

    /// <summary>
    /// リスト表示するグループ情報
    /// </summary>
    /// <param name="date_">指定日付(null:一覧)</param>
    /// <returns></returns>

    public List<LicenseView> ListView(DateTime? date_)
    {
        var rtn = new List<LicenseView>();

        var flg = (date_ == null);

        foreach (var group in this.Keys)
        {
            var list = this[group]?.Where(x_ => ((x_.CheckOut().EventDate() == date_) && AnalysisManager.Instance.IsChecked(x_.Product) == true) || flg);

            if (list?.Count() > 0)
            {
                var view = new LicenseView()
                {
                    Name = group,
                    Count = list.Count(),
                    Duration = new TimeSpan(list.Sum(x_ => x_.Duration.Ticks)),
                };
                rtn.Add(view);
            }
        }

        return rtn.OrderByDescending(x_ => x_.Duration).ToList();
    }

    /// <summary>
    /// プロットするグループ情報
    /// </summary>
    /// <param name="listX_">対応する時間リスト</param>
    /// <param name="timeSpan_">時間間隔</param>
    /// <returns>Key:データ内容/Value:対応するデータ</returns>
    public Dictionary<string, List<double>> ListPlot(List<DateTime> listX_, long timeSpan_)
    {
        var rtn = new Dictionary<string, List<double>>();



        // 期間順にするため
        var listGroup = AnalysisManager.Instance.ListResultGroup.Select(x_ => x_.Name).Take(25);
        int count = 1;
        foreach (var group in listGroup)
        {
            
            rtn[string.Format(_GROUP_FORMAT,count, group)] = new();
            count++;
        }
        foreach (var time in listX_)
        {
            //var listView = ListView(time, timeSpan_);
            count = 1;
            foreach (var group in listGroup)
            {
                //var list = listView.Where(x_ => x_.Name == group);
                var list =( timeSpan_ != TimeSpan.TicksPerDay) ?this[group].Where(x_=>x_.IsWithInRange(time)==true): this[group].Where(x_ => (x_.CheckOut().EventDate() == time));

                if (list.Count() > 0)
                {
                    rtn[string.Format(_GROUP_FORMAT, count, group)].Add(count);
                }
                else
                {
                    rtn[string.Format(_GROUP_FORMAT, count, group)].Add(double.NaN);
                }
                count++;
            }
        }

        return rtn;
    }


    /// <summary>
    /// 文字列化のリスト
    /// </summary>
    private List<string> _listToString()
    {
        var rtn = new List<string>();

        int count = 0;
        int max = this.Keys.Count;
        ProgressCount?.Invoke(0, max, _ANALYSIS);
        foreach (var key in this.Keys)
        {
            var list = this[key];
            var sum = new TimeSpan(list.Sum(x => x.DurationDuplication().Ticks));
            var days = new HashSet<DateTime>(list.Select(x => x.CheckOutDateTime.Date));
            rtn.Add($"{key},{sum.ToString(@"d\.hh\:mm\:ss")},{days.Count},{list.Count}");
            ProgressCount?.Invoke(++count, max);
        }
        return rtn;
    }

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    public void WriteText(string path_)
    {
        var list = new List<string>();
        list.Add(Header);
        list.AddRange(_listToString());
        File.WriteAllLines(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }
}