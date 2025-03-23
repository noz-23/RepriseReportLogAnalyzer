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
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Windows;
using ScottPlot;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// チェックアウトとチェックイン結合情報のリスト化 
/// </summary>
[Sort(1)]
[Table("TbAnalysisCheckOutCheckIn")]
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
            // SortedList>SortedDictionary>SortedSet の順位に早い
            var listCheckIn = new SortedList<long, LogEventCheckIn>(log_.ListEvent<LogEventCheckIn>(startShutdown).ToDictionary(x_ => x_.EventNumber));
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
                    listCheckIn.Remove(delIn.EventNumber);
                }

                var data = new AnalysisCheckOutIn(checkOut, checkIn);
                // 重複のチェック(同一のプロダクト ユーザー ホスト)一覧

                listCheckOutIn.Add(data);
                ProgressCount?.Invoke(++count, max);
            }

            // 重複のチェック
            var divCheckOutIn =listCheckOutIn.GroupBy(x_=>string.Format(_KEY_PRODUCT_USER_HOST, x_.Product, x_.UserHost));
            _setDuplication(divCheckOutIn);
            this.AddRange(listCheckOutIn);
        }
    }
    private void _setDuplication(IEnumerable<IGrouping<string, AnalysisCheckOutIn>> listOutIn_)
    {
        int count = 0;
        int max = listOutIn_.Count();
        ProgressCount?.Invoke(0, max, _ANALYSIS + "Duplication");
        Parallel.ForEach(listOutIn_, new() { MaxDegreeOfParallelism = 4 }, dataGroup_ =>
        {
            var listNoCheck = new List<long>();
            var listValue = dataGroup_.OrderBy(x_=>x_.CheckOutNumber());

            if (listValue.Any() ==false)
            {
                Interlocked.Increment(ref count);
                return;
            }
            foreach (var data in listValue)
            {
                if (listNoCheck.Contains(data.CheckOutNumber()) == true)
                {
                    continue;
                }
                // 時間内に含まれるデータは除外
                var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                list.ForAll(x_ =>
                {
                    x_.JoinEvent().SetDuplication();
                    listNoCheck.Add(x_.CheckOutNumber()); 
                });
            }
            //
            foreach (var data in listValue)
            {
                if (listNoCheck.Contains(data.CheckOutNumber()) == true)
                {
                    //チェック対象外は見ない
                    continue;
                }

                // チェックアウト時間のみが範囲
                var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_.CheckOutNumber()) == false)).OrderBy(x_ => x_.CheckInNumber());
                if (list.Any() ==true)
                {
                    // 時間(最後)を更新して追加
                    var renew = list.Last();
                    data.JoinEvent().SetDuplication(renew.CheckIn());

                    list.ForAll(x_ =>
                    {
                        x_.JoinEvent().SetDuplication();
                        listNoCheck.Add(x_.CheckOutNumber());

                    });
                }
            }
            Interlocked.Increment(ref count);
            ProgressCount?.Invoke(count, max);
        });
    }

    /// <summary>
    /// チェックインに対応するチェックアウトの検索
    /// </summary>
    /// <param name="checkIn_">チェックイン</param>
    /// <returns></returns>
    public LogEventCheckOut? Find(LogEventCheckIn checkIn_) => this.ToList().Find(x_ => x_.IsSame(checkIn_))?.CheckOut() ?? null;

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

}
