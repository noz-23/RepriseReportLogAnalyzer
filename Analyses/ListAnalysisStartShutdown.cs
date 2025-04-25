/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Controls;
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// スタート シャットダウン結合情報のリスト化 
/// </summary>
[Sort(1)]
[Table("TbAnalysisStartShutdown")]
[Description("Join Start And Shutdown"), Category("Analyses")]
internal sealed class ListAnalysisStartShutdown : List<AnalysisStartShutdown>, IAnalysisOutputFile
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ListAnalysisStartShutdown()
    {
        ProgressCount = null;
    }

    /// <summary>
    /// コンボボックスの項目
    /// </summary>
    public static ListStringLongPair ListSelect { get => _listSelect; }
    private static ListStringLongPair _listSelect = new()
    {
        new( "No Include Skip", (long)SelectData.ECLUSION),
        new ("All Data", (long)SelectData.ALL )
    };


    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountCallBack? ProgressCount;

    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    public void Analysis(ConvertReportLog log_)
    {
        var listSkipNumber = new SortedSet<long>();

        var listStart = log_.ListEvent<LogEventStart>();
        var listShutdown = log_.ListEvent<LogEventShutdown>();

        AnalysisStartShutdown? last = null;
        foreach (var start in listStart)
        {
            // 同じ終了データが続いた場合、二つ目はスキップ(利用しない)
            var shutdown = listShutdown.ToList().Find(down_ => down_.EventNumber > start.EventNumber) ?? new LogEventShutdown();

            var startShutdown = new AnalysisStartShutdown(start, shutdown);
            Add(startShutdown);
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
    public async Task WriteText(string path_, long skip_ = (long)SelectData.ECLUSION)
    {
        var list = new List<string>();
        // ヘッダー
        //list.Add(Header(skip_));
        list.Add(CsvHeader(skip_));
        // データ
        list.AddRange(ListValue(skip_).Select(x_ => string.Join(",", x_)));
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    /// <param name="skip_"></param>
    /// <returns></returns>
    //public string Header(long skip_) => AnalysisStartShutdown.Header();
    public string CsvHeader(long skip_) => AnalysisStartShutdown.CsvHeader();

    /// <summary>
    /// リスト化したヘッダー
    /// </summary>
    /// <param name="skip_"></param>
    /// <returns></returns>
    //public ListStringStringPair ListHeader(long skip_) => AnalysisStartShutdown.ListHeader();
    public List<string> ListCreateHeader(long skip_) => AnalysisStartShutdown.ListCreateHeader();

    public List<string> ListInsertHeader(long skip_) => AnalysisStartShutdown.ListInsertHeader();

    /// <summary>
    /// リスト化したデータ
    /// </summary>
    /// <param name="skip_"></param>
    /// <returns></returns>
    public IEnumerable<List<string>> ListValue(long skip_)
    {
        var list = (skip_ == (long)(SelectData.ALL)) ? this : ListNoIncludeSkip();
        return list.Select(x_ => x_.ListValue());
    }

    /// <summary>
    /// ファイル保存(結合情報)
    /// </summary>
    /// <param name="path_"></param>
    /// <returns></returns>
    public async Task WriteJoinText(string path_)
    {
        var list = new List<string>();
        // ヘッダー
        //list.Add(JoinHeader());
        list.Add(JoinCsvHeader());
        // データ
        list.AddRange(ListJoinValue().Select(x_ => string.Join(",", x_)));
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);
    }
    /// <summary>
    /// 結合情報ヘッダー項目
    /// </summary>
    /// <returns></returns>
    //public static string JoinHeader() => ToDataBase.Header(typeof(JoinEventStartShutdown));
    public static string JoinCsvHeader() => BaseToData.CsvHeader(typeof(JoinEventStartShutdown));

    /// <summary>
    /// 結合情報データ項目
    /// </summary>
    /// <returns></returns>
    public IEnumerable<List<string>> ListJoinValue() => this.Select(x_ => x_.JoinEvent().ListValue());
}
