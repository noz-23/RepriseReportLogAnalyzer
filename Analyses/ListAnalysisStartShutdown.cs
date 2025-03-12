/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Reflection;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// スタート シャットダウン結合情報のリスト化 
/// </summary>
internal sealed class ListAnalysisStartShutdown : List<AnalysisStartShutdown>, IAnalysisOutputFile
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ListAnalysisStartShutdown()
    {
    }


    public static ListStringLongPair ListSelect
    {
        get => new()
        {
            //new( "スキップあり", JoinEventStartShutdown.SKIP_DATA),
            new( "スキップあり", (long)SelectData.ECLUSION),
            //new ("全データ", JoinEventStartShutdown.USE_DATA )
            new ("全データ", (long)SelectData.ALL )
        };
    }

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;


    /// <summary>
    /// 文字列化のヘッダー
    /// </summary>
    //public string Header(long selected_)
    //{
    //    var listColunm = new List<string>();
    //    var listPropetyInfo = typeof(AnalysisStartShutdown).GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(SortAttribute)) as SortAttribute)?.Sort);

    //    listPropetyInfo?.ToList().ForEach(prop =>
    //    {
    //        listColunm.Add($"{prop.Name}");
    //    });

    //    return string.Join(",", listColunm);
    //}

    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    public void Analysis(ConvertReportLog log_)
    {
        var listSkipNumber = new SortedSet<long>();
        //foreach (var end in log_.ListEnd)
        //{
        //    // ログのの切り替えの次のスタートはスキップ
        //    var start = log_.ListStart.Find(x_ => x_.EventNumber > end.EventNumber);
        //    if (start != null)
        //    {
        //        listSkipNumber.Add(start.EventNumber);
        //    }
        //}

        AnalysisStartShutdown? last = null;
        foreach (var start in log_.ListStart)
        {
            LogEventBase? shutdown = log_.ListShutdown.ToList().Find(down_ => down_.EventNumber > start.EventNumber);

            var startShutdown = new AnalysisStartShutdown(start, shutdown);
            this.Add(startShutdown);
            //
            if (listSkipNumber.Contains(start.EventNumber) == true)
            {
                startShutdown.JoinEvent().SetSkip();
                last = startShutdown;
                continue;
            }

            if (last != null)
            {
                // スタートが2回続いた場合(シャットダウンログ等がない)
                if (shutdown?.EventNumber == last.ShutdownNumber)
                {
                    startShutdown.JoinEvent().SetSkip();
                }
            }
            last = startShutdown;
        }
    }

    /// <summary>
    /// スタート シャットダウン結合情報のリスト(スキップ内容を含まない)
    /// </summary>

    //public IEnumerable<AnalysisStartShutdown> ListWithoutSkip()=> this.Where(x_ => x_.JoinEvent().IsSkip != JoinEventStartShutdown.SKIP_DATA);
    public IEnumerable<AnalysisStartShutdown> ListWithoutSkip() => this.Where(x_ => x_.JoinEvent().IsSkip != (long)SelectData.ECLUSION);

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="withoutSkip_">スキップ内容ありか</param>
    //public void WriteText(string path_, long withoutSkip_ = JoinEventStartShutdown.SKIP_DATA)
    public void WriteText(string path_, long withoutSkip_ = (long)SelectData.ECLUSION)
    {
        var list = new List<string>();
        list.Add(Header(withoutSkip_));

        //var listData = (withoutSkip_ == (long)SelectData.ALL) ? this : ListWithoutSkip();

        //list.AddRange(listData.Select(x_ => x_.ToString()));
        list.AddRange(ListValue(withoutSkip_).Select(x_ => string.Join(",",x_)));
        File.WriteAllLines(path_, list, Encoding.UTF8);
    }


    public string Header(long withoutSkip_) => AnalysisStartShutdown.Header();

    public ListStringStringPair ListHeader(long withoutSkip_) => AnalysisStartShutdown.ListHeader();


    public IEnumerable<List<string>> ListValue(long withoutSkip_)
    {
        var list = (withoutSkip_ == (long)(SelectData.ALL)) ? this : ListWithoutSkip();
        return list.Select(x_ =>  x_.ListValue());
    }
}
