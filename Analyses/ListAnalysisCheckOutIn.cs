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
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Windows;
using ScottPlot;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// チェックアウトとチェックイン結合情報のリスト化 
/// </summary>
[Sort(2)]
[Table("TbAnalysisCheckOutCheckIn")]
[Description("Join Check-Out And Check-In"),Category("Analyses")]
internal sealed class ListAnalysisCheckOutIn : SortedSet<AnalysisCheckOutIn>, IAnalysisOutputFile
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ListAnalysisCheckOutIn()
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">グループ集計</param>
    public ListAnalysisCheckOutIn(IEnumerable<AnalysisCheckOutIn> list_)
    { 
        this.AddRange(list_);
    }

    /// <summary>
    /// プログレスバー 解析処理 更新デリゲート
    /// </summary>
    public ProgressCountDelegate? ProgressCount = null;

    /// <summary>
    /// 解析内容
    /// </summary>
    private const string _ANALYSIS = "[CheckOut - CheckIn]";

    /// <summary>
    /// 重複検出用(product user@host)
    /// </summary>
    private const string _KEY_PRODUCT_USER_HOST = "{0} {1}";

    /// <summary>
    /// コンボボックスの項目
    /// </summary>
    public static ListStringLongPair ListSelect { get => _listSelect; }
    private static ListStringLongPair _listSelect = new()
    {
        new( "No Duplicate", (long)SelectData.ECLUSION),
        new ("Have Duplicate", (long)SelectData.ALL)
    };

    /// <summary>
    /// 重複なしのデータリスト
    /// </summary>
    public IEnumerable<AnalysisCheckOutIn> ListNoDuplication() => this.Where(x_ => x_.JoinEvent().DuplicationNumber != (long)SelectData.ECLUSION);

    /// <summary>
    /// 結合情報リスト
    /// </summary>
    public IEnumerable<JoinEventCheckOutIn> ListJointEvetn() => this.Select(x_ => x_.JoinEvent());

    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    /// <param name="listStartShutdown_">スタートとシャットダウンの時間帯リスト</param>
    public void Analysis(ConvertReportLog log_, IEnumerable<AnalysisStartShutdown> listStartShutdown_)
    {
        if (listStartShutdown_.Any() == false)
        {
            return;
        }

        foreach (var startShutdown in listStartShutdown_)
        {
            // Start と Shutdown で区切る
            //// 逆順の方がかなり早い(_ _;)
            var listCheckOut = log_.ListEvent<LogEventCheckOut>(startShutdown).OrderByDescending(x_ => x_.EventNumber);
            var listCheckIn = new SortedList<long, LogEventCheckIn>(log_.ListEvent<LogEventCheckIn>(startShutdown).ToDictionary(x_ => x_.EventNumber));
            //
            LogFile.Instance.WriteLine($"{startShutdown.StartDateTime.ToString()} - {startShutdown.ShutdownDateTime.ToString()} : {listCheckOut.Count()}");
            var listCheckOutIn=new List<AnalysisCheckOutIn>();

            int count = 0;
            int max = listCheckOut.Count();

            ProgressCount?.Invoke(0, max, _ANALYSIS + "Join " + startShutdown.StartDateTime.ToShortDateString());
            foreach (var checkOut in listCheckOut)
            {
                // 対応するチェックインを探す
                var checkIn = listCheckIn.Where(x_ => x_.Key > checkOut.EventNumber).Select(x_ => x_.Value).ToList().Find(f_ => checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber)) ?? startShutdown.EventShutdown();

                if (checkIn is LogEventCheckIn delIn)
                {
                    // 対のチェックアウト情報の登録
                    delIn.SetLogEventCheckOut(checkOut);
                    // スピードアップのため検索リストから検出したデータを削除
                    listCheckIn.Remove(delIn.EventNumber);
                }

                listCheckOutIn.Add(new AnalysisCheckOutIn(checkOut, checkIn));
                //
                ProgressCount?.Invoke(++count, max);
            }

            // グループ分け
            var divCheckOutIn =listCheckOutIn.GroupBy(x_=>string.Format(_KEY_PRODUCT_USER_HOST, x_.Product, x_.UserHost));
            // 重複のチェック
            _setDuplication(divCheckOutIn);
            // リストに追加
            this.AddRange(listCheckOutIn);
        }
    }

    /// <summary>
    /// 重複チェック
    /// </summary>
    /// <param name="listOutIn_"></param>
    private void _setDuplication(IEnumerable<IGrouping<string, AnalysisCheckOutIn>> listOutIn_)
    {
        int count = 0;
        int max = listOutIn_.Count();
        ProgressCount?.Invoke(0, max, _ANALYSIS + "Duplication");
        Parallel.ForEach(listOutIn_, new() { MaxDegreeOfParallelism = 4 }, dataGroup_ =>
        {
            // long より AnalysisCheckOutIn の方が速い
            var listNoCheck = new List<AnalysisCheckOutIn>();
            //
            var listValue = new SortedSet<AnalysisCheckOutIn>();
            listValue.AddRange(dataGroup_);

            if (listValue.Any() == false)
            {
                Interlocked.Increment(ref count);
                return;
            }
            foreach (var data in listValue)
            {
                if(listNoCheck.Contains(data) == true)
                {
                    continue;
                }
                // 時間内に含まれるデータは除外
                var list = listValue.Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                list.ToList().ForEach(x_ =>
                {
                    x_.JoinEvent().SetDuplication();
                    listNoCheck.Add(x_); 
                });
            }
            //
            foreach (var data in listValue)
            {
                if (listNoCheck.Contains(data) == true)
                {
                    //チェック対象外は見ない
                    continue;
                }

                // チェックアウト時間のみが範囲
                var list = listValue.Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());

                if (list.Any() ==true)
                {
                    // 時間(最後)を更新して追加
                    var renew = list.Last();
                    data.JoinEvent().SetDuplication(renew.CheckIn());
                    //
                    list.ToList().ForEach(x_ =>
                    {
                        x_.JoinEvent().SetDuplication();
                        listNoCheck.Add(x_);
                    });
                }
            }
            Interlocked.Increment(ref count);
            ProgressCount?.Invoke(count, max);
        });
    }

    /// <summary>
    /// ファイル保存(チェックアウト-チェックイン)
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="duplication_">ture:重複なし</param>
    public async Task WriteText(string path_, long duplication_ = 0)
    {
        var list = new List<string>();
        // ヘッダー
        list.Add(Header(duplication_));
        // データ
        list.AddRange(ListValue(duplication_).Select(x_ => string.Join(",", x_)));
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    /// <param name="duplication_"></param>
    /// <returns></returns>
    public string Header(long duplication_) => AnalysisCheckOutIn.Header();

    /// <summary>
    /// リスト化したヘッダー項目
    /// </summary>
    /// <param name="duplication_"></param>
    /// <returns></returns>
    public ListStringStringPair ListHeader(long duplication_) => AnalysisCheckOutIn.ListHeader();

    /// <summary>
    /// リスト化したデータ項目
    /// </summary>
    /// <param name="duplication_"></param>
    /// <returns></returns>

    public IEnumerable<List<string>> ListValue(long duplication_)
    {
        var list = (duplication_ == (long)(SelectData.ALL)) ? this : ListNoDuplication();
        return list.Select(x_ => (duplication_ == (long)(SelectData.ALL)) ? x_.ListValue() : x_.ListDuplicationValue());
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
        list.Add(JoinHeader());
        // データ
        list.AddRange(ListJoinValue().Select(x_ => string.Join(",", x_)));
        await File.WriteAllLinesAsync(path_, list, Encoding.UTF8);
    }

    /// <summary>
    /// 結合情報ヘッダー項目
    /// </summary>
    /// <returns></returns>
    public string JoinHeader() => ToDataBase.Header(typeof(JoinEventCheckOutIn));
    /// <summary>
    /// 結合所法データ項目
    /// </summary>
    /// <returns></returns>
    public IEnumerable<List<string>> ListJoinValue()
    {
        return this.Select(x_ => x_.JoinEvent().ListValue());
    }
}
