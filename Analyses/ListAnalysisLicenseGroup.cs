﻿/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Data;
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

/// <summary>
/// 出力のためグループを分けクラス化(User)
/// </summary>
[Sort(11)]

internal sealed class ListAnalysisLicenseUser : ListAnalysisLicenseGroup, IAnalysisOutputFile
{
    public ListAnalysisLicenseUser() : base(AnalysisGroup.USER)
    { 
    }

    /// <summary>
    /// コンボボックスの項目(リフレクションのため別だし)
    /// </summary>
    public static ListStringLongPair ListSelect { get => _ListSelect; }
}

/// <summary>
/// 出力のためグループを分けクラス化(Host)
/// </summary>
[Sort(12)]
internal sealed class ListAnalysisLicenseHost : ListAnalysisLicenseGroup, IAnalysisOutputFile
{
    public ListAnalysisLicenseHost() : base(AnalysisGroup.HOST)
    {
    }
    /// <summary>
    /// コンボボックスの項目(リフレクションのため別だし)
    /// </summary>
    public static ListStringLongPair ListSelect { get => _ListSelect; }
}

/// <summary>
/// 出力のためグループを分けクラス化(User@Host)
/// </summary>
[Sort(13)]
internal sealed class ListAnalysisLicenseUserHost : ListAnalysisLicenseGroup, IAnalysisOutputFile
{
    public ListAnalysisLicenseUserHost() : base(AnalysisGroup.USER_HOST)
    {
    }
    /// <summary>
    /// コンボボックスの項目(リフレクションのため別だし)
    /// </summary>
    public static ListStringLongPair ListSelect { get => _ListSelect; }
}



/// <summary>
/// グループ毎の集計
///  Key:対応グループ
///  Value:一致するチェックアウト チェックイン結合情報リスト
/// </summary>
[Sort(10)]
internal class ListAnalysisLicenseGroup : Dictionary<string, ListAnalysisCheckOutIn>
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

    protected static ListStringLongPair _ListSelect
    {
        get => new()
        {
            new("All In One", (long)SelectData.ALL),
            new("By Product",(long)SelectData.ECLUSION),
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

        int count = 0;
        int max = listGroup_.Count();

        ProgressCount?.Invoke(0, max, _ANALYSIS + _group.Description());
        foreach (var group in listGroup_)
        {
            var list = new ListAnalysisCheckOutIn(listCheckOutIn_.ListNoDuplication().Where(x_ => x_.GroupName(_group) == group));
            this[group] = list;

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
    public SortedDictionary<string, List<double>> ListPlot(List<DateTime> listX_, long timeSpan_)
    {
        var rtn = new SortedDictionary<string, List<double>>();

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
            count = 1;
            foreach (var group in listGroup)
            {
                var list =( timeSpan_ != TimeSpan.TicksPerDay) ?this[group].Where(x_=>x_.IsWithInRange(time)==true): this[group].Where(x_ => (x_.CheckOut().EventDate() == time));
                rtn[string.Format(_GROUP_FORMAT, count, group)].Add((list.Count() == 0) ? double.NaN : count);

                count++;
            }
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
        // ヘッダー
        list.Add(Header(product_));
        // データ
        list.AddRange(ListValue(product_).Select(x_=>string.Join(",",x_)));
        File.WriteAllLines(path_, list, Encoding.UTF8);

        LogFile.Instance.WriteLine($"Write:{path_}");
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    public string Header(long product_) => string.Join(",", ListHeader(product_).Select(x_ => x_.Key));

    /// <summary>
    /// リスト化したヘッダー
    /// </summary>
    /// <param name="product_"></param>
    /// <returns></returns>
    
    public ListStringStringPair ListHeader(long product_)
    {
        var rtn = new ListStringStringPair();
        rtn.Add(new(_group.Description(), ToDataBase.GetDatabaseType(typeof(string))));

        if (product_ == (long)SelectData.ALL)
        {
            // 全て
            rtn.Add(new("Duration", ToDataBase.GetDatabaseType(typeof(TimeSpan))));
            rtn.Add(new("Days", ToDataBase.GetDatabaseType(typeof(long))));
            rtn.Add(new("Count", ToDataBase.GetDatabaseType(typeof(long))));

            return rtn;
        }

        // プロダクト毎
        foreach (var product in _listProduct)
        {
            rtn.Add(new($"{product}_Duration", ToDataBase.GetDatabaseType(typeof(TimeSpan))));
            rtn.Add(new($"{product}_Days", ToDataBase.GetDatabaseType(typeof(long))));
            rtn.Add(new($"{product}_Count", ToDataBase.GetDatabaseType(typeof(long))));
        }
        return rtn;
    }

    /// <summary>
    /// リストしたデータ
    /// </summary>
    /// <param name="product_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListValue(long product_)
    {
        var rtn = new List<List<string>>();

        foreach (var key in _listGroup)
        {
            var listCount = this[key];

            var add = new List<string>();
            add.Add($"{key}");
            if (product_ == (long)SelectData.ALL)
            {
                // 全て
                var sum = new TimeSpan(listCount.Sum(x => x.DurationDuplication().Ticks));
                var days = new HashSet<DateTime>(listCount.Select(x => x.CheckOutDateTime.Date));

                add.Add($"{sum.ToString(@"d\.hh\:mm\:ss")}");
                add.Add($"{days.Count}");
                add.Add($"{listCount.Count}");
                rtn.Add(add);

                continue;
            }
            // プロダクト毎
            foreach (var product in _listProduct)
            {
                var listProduct = listCount.Where(x_ => x_.Product == product);
                var sum = new TimeSpan(listProduct.Sum(x => x.DurationDuplication().Ticks));
                var days = new HashSet<DateTime>(listProduct.Select(x => x.CheckOutDateTime.Date));

                add.Add($"{sum.ToString(@"d\.hh\:mm\:ss")}");
                add.Add($"{days.Count}");
                add.Add($"{listProduct.Count()}");
            }
            rtn.Add(add);

        }
        return rtn;
    }
}