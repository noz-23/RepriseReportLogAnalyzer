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
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// スタート シャットダウン結合情報のリスト化 
/// </summary>
internal sealed class ListAnalysisStartShutdown : List<AnalysisStartShutdown>
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ListAnalysisStartShutdown()
    {
    }

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    public void Analysis(AnalysisReportLog log_)
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

    public IEnumerable<AnalysisStartShutdown> ListWithoutSkip()=> this.Where(x_ => x_.JoinEvent().IsSkip != JoinEventStartShutdown.SKIP);

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="withoutSkip_">スキップ内容ありか</param>
    public void WriteText(string path_, bool withoutSkip_ = false)
    {
        var list = new List<string>();
        list.Add(AnalysisStartShutdown.HEADER);

        var listData = (withoutSkip_ == false) ? this : ListWithoutSkip();

        list.AddRange(listData.Select(x_ => x_.ToString()));
        File.WriteAllLines(path_, list, Encoding.UTF8);
    }
}
