/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
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
            new( "No Include Skip", (long)SelectData.ECLUSION),
            new ("All Data", (long)SelectData.ALL )
        };
    }

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    public void Analysis(ConvertReportLog log_)
    {
        var listSkipNumber = new SortedSet<long>();

        AnalysisStartShutdown? last = null;
        foreach (var start in log_.ListStart)
        {
            // 同じ終了データが続いた場合、二つ目はスキップ(利用しない)
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
    /// スキップを含まないデータ
    /// </summary>
    /// <returns></returns>
    public IEnumerable<AnalysisStartShutdown> ListNoIncludeSkip() => this.Where(x_ => x_.JoinEvent().IsSkip != (long)SelectData.ECLUSION);

    /// <summary>
    /// ファイル保存
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="withoutSkip_">スキップ内容ありか</param>
    public void WriteText(string path_, long skip_ = (long)SelectData.ECLUSION)
    {
        var list = new List<string>();
        // ヘッダー
        list.Add(Header(skip_));
        // データ
        list.AddRange(ListValue(skip_).Select(x_ => string.Join(",",x_)));
        File.WriteAllLines(path_, list, Encoding.UTF8);
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    /// <param name="skip_"></param>
    /// <returns></returns>
    public string Header(long skip_) => AnalysisStartShutdown.Header();

    /// <summary>
    /// リスト化したヘッダー
    /// </summary>
    /// <param name="skip_"></param>
    /// <returns></returns>
    public ListStringStringPair ListHeader(long skip_) => AnalysisStartShutdown.ListHeader();

    /// <summary>
    /// リスト化したデータ
    /// </summary>
    /// <param name="skip_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListValue(long skip_)
    {
        var list = (skip_ == (long)(SelectData.ALL)) ? this : ListNoIncludeSkip();
        return list.Select(x_ =>  x_.ListValue());
    }
}
