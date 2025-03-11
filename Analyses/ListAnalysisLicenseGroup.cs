﻿/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Managers;
using RepriseReportLogAnalyzer.Views;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

internal sealed class ListAnalysisLicenseUser : ListAnalysisLicenseGroup
{
    public ListAnalysisLicenseUser() : base(AnalysisGroup.USER)
    { 
    }
}

internal sealed class ListAnalysisLicenseHost : ListAnalysisLicenseGroup
{
    public ListAnalysisLicenseHost() : base(AnalysisGroup.HOST)
    {
    }
}

internal sealed class ListAnalysisLicenseUserHost : ListAnalysisLicenseGroup
{
    public ListAnalysisLicenseUserHost() : base(AnalysisGroup.USER_HOST)
    {
    }
}



/// <summary>
/// グループ毎の集計
///  Key:対応グループ
///  Value:一致するチェックアウト チェックイン結合情報リスト
/// </summary>
[Sort(3)]
internal class ListAnalysisLicenseGroup : Dictionary<string, ListAnalysisCheckOutIn>, IAnalysisTextWrite
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="group_"></param>
    public ListAnalysisLicenseGroup(AnalysisGroup group_)
    {
        _group = group_;
    }

    /// <summary>
    /// プロット表示数
    /// </summary>
    public const int TOP_PLOT_USE =25;

    /// <summary>
    /// プロット用の文字列
    /// </summary>
    private const string _GROUP_FORMAT = "{0}.{1}";

    /// <summary>
    /// 解析内容
    /// </summary>
    private const string _ANALYSIS = "[License Group Duration]";

    //private const long _ALL = 0;
    //private const long _PRODUCT = 0;

    public static ListKeyPair ListSelect
    {
        get => new()
        {
            new("全てまとめて", (long)SelectData.ALL),
            new("プロダクト毎",(long)SelectData.ECLUSION),
        };
    }

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

    /// <summary>
    /// グループ内容
    /// </summary>
    private AnalysisGroup _group = AnalysisGroup.NONE;

    /// <summary>
    /// プロダクト リスト
    /// </summary>
    private SortedSet<string> _listProduct = new();

    /// <summary>
    /// 文字列化のヘッダー
    /// </summary>
    public string Header { get => _group.Description() + ",Duration,Days,Count"; }

    private string _header(long product_)
    {
        var list = new List<string>();
        list.Add(_group.Description());
        foreach (var product in _listProduct)
        {
            list.Add($"{product}[Duration]");
            list.Add($"{product}[Days]");
            list.Add($"{product}[Count]");
        }
        return string.Join(",", list);
    }
    /// <summary>
    /// 対応するグループリスト
    /// </summary>
    private SortedSet<string> _listGroup =new();

    public void Analysis(IEnumerable<string> listGroup_, ListAnalysisCheckOutIn listCheckOutIn_)
    {
        // プロダクトのコピー
        _listProduct.UnionWith(listCheckOutIn_.Select(x_=>x_.Product));
        // グループのコピー
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
            var list = new ListAnalysisCheckOutIn(listCheckOutIn_.ListNoDuplication().Where(x_ => x_.GroupName(_group) == group));
            this[group] = list;

            //_addDayToGroup(minDate, maxDate, group, list);
            ProgressCount?.Invoke(++count, max);
        }
    }

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
            var list = this[group]?.Where(x_ => ((x_.CheckOut().EventDate() == date_) && AnalysisManager.Instance.IsProductChecked(x_.Product) == true) || flg);

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
        var listGroup = AnalysisManager.Instance.ListResultGroup.Select(x_ => x_.Name).Take(TOP_PLOT_USE);
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
    private List<string> _listToString(long product_)
    {
        var rtn = new List<string>();

        int count = 0;
        int max = this.Keys.Count;
        ProgressCount?.Invoke(0, max, _ANALYSIS);
        foreach (var key in this.Keys)
        {
            var listCount = this[key];

            if (product_ == (long)SelectData.ECLUSION)
            {
                var sum = new TimeSpan(listCount.Sum(x => x.DurationDuplication().Ticks));
                var days = new HashSet<DateTime>(listCount.Select(x => x.CheckOutDateTime.Date));

                rtn.Add($"{key},{sum.ToString(@"d\.hh\:mm\:ss")},{days.Count},{listCount.Count}");
            }
            else
            {
                foreach (var product in _listProduct)
                {
                    var list = new List<string>();
                    var listProduct = listCount.Where(x_ => x_.Product == product);
                    var sum = new TimeSpan(listProduct.Sum(x => x.DurationDuplication().Ticks));
                    var days = new HashSet<DateTime>(listProduct.Select(x => x.CheckOutDateTime.Date));

                    list.Add($"{sum.ToString(@"d\.hh\:mm\:ss")}");
                    list.Add($"{days.Count}");
                    list.Add($"{listProduct.Count()}");
                    rtn.Add($"{key},{sum.ToString(@"d\.hh\:mm\:ss")},{string.Join(",", list)}");
                }
            }

            ProgressCount?.Invoke(++count, max);
        }
        return rtn;
    }

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    public void WriteText(string path_, long product_)
    {
        var list = new List<string>();
        list.Add(_header(product_));
        list.AddRange(_listToString( product_));
        File.WriteAllLines(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }
}