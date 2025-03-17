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
using RepriseReportLogAnalyzer.Extensions;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Windows;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// チェックアウトとチェックイン結合情報のリスト化 
/// </summary>
[Sort(1)]
internal sealed class ListAnalysisCheckOutIn : List<AnalysisCheckOutIn>, IAnalysisOutputFile
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
        list_?.ToList().ForEach(item => this.Add(item));
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
    public static ListStringLongPair ListSelect
    {
        get => new()
        {
            new( "No Duplicate", (long)SelectData.ECLUSION),
            new ("Have Duplicate", (long)SelectData.ALL)
        };
    }

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
        foreach (var startShutdown in listStartShutdown_)
        {
            // Start と Shutdown で区切る
            int count = 0;
            int max = 1;
            var listCheckOut = log_.ListEvent<LogEventCheckOut>(startShutdown);
            var listCheckIn = new SortedSet<LogEventCheckIn>(log_.ListEvent<LogEventCheckIn>(startShutdown));
            LogFile.Instance.WriteLine($"{startShutdown.StartDateTime.ToString()} - {startShutdown.ShutdownDateTime.ToString()} : {listCheckOut.Count()}");
            //
            //int div = 1;
            var listCheckOutIn = new SortedDictionary<string, List<AnalysisCheckOutIn>>();
            var countProductUserHost = listCheckOut.Select(x_ => string.Format(_KEY_PRODUCT_USER_HOST, x_.Product, x_.UserHost)).Distinct();

            //ProgressCount?.Invoke(0, max, _ANALYSIS + "Div " + startShutdown.StartDateTime.ToShortDateString());
            foreach (var key in countProductUserHost)
            {
                var listOut = listCheckOut.Where(x_ => string.Format(_KEY_PRODUCT_USER_HOST, x_.Product, x_.UserHost) == key);

                if (listOut.Count() > 0)
                {
                    listCheckOutIn[key] = new();
                }
                //ProgressCount?.Invoke(++count, max);
            }
            var listEnd = new SortedSet<long>();
            //var listEnd = new ConcurrentBag<long>();

            count = 0;
            max = listCheckOut.Count();
            //div = (max / 100) + 1;

            ProgressCount?.Invoke(0, max, _ANALYSIS + "Join " + startShutdown.StartDateTime.ToShortDateString());
            foreach (var checkOut in listCheckOut)
            //Parallel.ForEach(listCheckOut,new ParallelOptions(){MaxDegreeOfParallelism=8}, checkOut =>
            {
                // 対応するチェックインを探す
                var checkIn = listCheckIn.FirstOrDefault(f_ => listEnd.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                //var checkIn = listCheckIn.AsParallel().AsOrdered().FirstOrDefault(f_ => listEnd.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                var data = new AnalysisCheckOutIn(checkOut, checkIn);
                //this.Add(data);

                //listCheckIn.Remove(checkIn);
                listEnd.Add(checkIn?.EventNumber ?? 0);


                // 重複のチェック(同一のプロダクト ユーザー ホスト)一覧
                //var key = $"{data.Product} {data.UserHost}";
                var key = string.Format(_KEY_PRODUCT_USER_HOST, data.Product, data.UserHost);

                //if (listCheckOutIn.ContainsKey(key) == false)
                //{
                //    listCheckOutIn[key] = new ();
                //}
                listCheckOutIn[key].Add(data);
                //Interlocked.Increment(ref count);
                //if ((count % div) == 0)
                //{ 
                ProgressCount?.Invoke(++count, max);
                //}
                //await Task.Delay(10);
                //});
            }

            // 重複のチェック
            count = 0;
            max = listCheckOutIn.Keys.Count;
            ProgressCount?.Invoke(0, max, _ANALYSIS + "Duplication " + startShutdown.StartDateTime.ToShortDateString());
            foreach (var key in listCheckOutIn.Keys)
            {
                var listNoCheck = new List<AnalysisCheckOutIn>();
                var listValue = listCheckOutIn[key].OrderBy(x_ => x_.CheckOutNumber());

                if (listValue.Count() <= 1)
                {
                    count++;
                    continue;
                }
                foreach (var data in listValue)
                {
                    if (listNoCheck.Contains(data) == true)
                    {
                        continue;
                    }
                    // 時間内に含まれるデータは除外
                    //var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                    var list = listValue.Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                    list.ToList().ForEach(x_ => x_.JoinEvent().SetDuplication());
                    listNoCheck.AddRange(list);
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
                    var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                    if (list.Count() > 0)
                    {
                        // 時間(最後)を更新して追加
                        var renew = list.Last();
                        data.JoinEvent().SetDuplication(renew.CheckIn());

                        list.ForAll(x_ => x_.JoinEvent().SetDuplication());
                        listNoCheck.AddRange(list);

                        //LogFile.Instance.WriteLine($"{data.CheckOutNumber()} : {data.CheckInNumber()} -> {renew.CheckInNumber()}");
                        continue;
                    }
                }
                ProgressCount?.Invoke(++count, max);
            }
            foreach (var data in listCheckOutIn.Values)
            {
                this.AddRange(data);
            }
        }
    }


    public void AnalysisEx(ConvertReportLog log_, IEnumerable<AnalysisStartShutdown> listStartShutdown_)
    {
        foreach (var startShutdown in listStartShutdown_)
        {
            // Start と Shutdown で区切る
            int count = 0;
            int max = 1;
            var listCheckOut = log_.ListEvent<LogEventCheckOut>(startShutdown);
            var listCheckIn = new SortedSet<LogEventCheckIn>(log_.ListEvent<LogEventCheckIn>(startShutdown));
            LogFile.Instance.WriteLine($"{startShutdown.StartDateTime.ToString()} - {startShutdown.ShutdownDateTime.ToString()} : {listCheckOut.Count()}");
            //
            //int div = 1;
            var divCheckOut = new SortedDictionary<string, SortedSet<LogEventCheckOut>>();
            var divCheckIn = new SortedDictionary<string, SortedSet<LogEventCheckIn>>();
            var listCheckOutIn = new SortedDictionary<string, List<AnalysisCheckOutIn>>();

            var countProductUserHost = listCheckOut.Select(x_=> string.Format(_KEY_PRODUCT_USER_HOST, x_.Product, x_.UserHost)).Distinct();
            //var listAdd=new ConcurrentBag<AnalysisCheckOutIn>();
            count = 0;
            max = countProductUserHost.Count();

            ProgressCount?.Invoke(0, max, _ANALYSIS + "Div " + startShutdown.StartDateTime.ToShortDateString());
            foreach (var key in countProductUserHost)
            {
                var listOut = listCheckOut.Where(x_ => string.Format(_KEY_PRODUCT_USER_HOST, x_.Product,  x_.UserHost) == key);

                if (listOut.Count() > 0)
                {
                    listCheckOutIn[key] = new();
                    //
                    divCheckOut[key] = new(listOut);

                    var checkOut = listOut.First();
                    divCheckIn[key] = new(listCheckIn.Where(x_ => x_.EventNumber > checkOut.EventNumber));
                }
                ProgressCount?.Invoke(++count, max);
            }

           // var listEnd = new SortedSet<long>();
            var listEnd = new ConcurrentBag<long>();
            
            count = 0;
            max = divCheckOut.Keys.Count();
            //div = (max / 100) + 1;

            ProgressCount?.Invoke(0, max, _ANALYSIS + "Join " + startShutdown.StartDateTime.ToShortDateString());

            Parallel.ForEach(divCheckOut.Keys, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, div => 
            {
                var listOut = divCheckOut[div];
                var listIn= divCheckIn[div];

                foreach (var checkOut in listOut)
                {
                    // 対応するチェックインを探す
                    //var checkIn = listCheckIn.FirstOrDefault(f_ => listEnd.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                    //var checkIn = listIn.AsParallel().AsOrdered().FirstOrDefault(f_ => listEnd.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                    var checkIn = listIn.FirstOrDefault(f_ => listEnd.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                    var data = new AnalysisCheckOutIn(checkOut, checkIn);
                    //listAdd.Add(data);

                    listEnd.Add(checkIn?.EventNumber ?? 0);

                    listCheckOutIn[div].Add(data);
                }
                listOut.Clear();
                listIn.Clear();

                // 重複のチェック
                foreach (var key in listCheckOutIn.Keys)
                {
                    var listNoCheck = new List<AnalysisCheckOutIn>();
                    var listValue = listCheckOutIn[key].OrderBy(x_ => x_.CheckOutNumber());

                    if (listValue.Count() <= 1)
                    {
                        //count++;
                        continue;
                    }


                    foreach (var data in listValue)
                    {
                        if (listNoCheck.Contains(data) == true)
                        {
                            continue;
                        }
                        // 時間内に含まれるデータは除外
                        //var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                        var list = listValue.Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                        list.ToList().ForEach(x_ => x_.JoinEvent().SetDuplication());
                        listNoCheck.AddRange(list);
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
                        //var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                        var list = listValue.Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                        if (list.Count() > 0)
                        {
                            // 時間(最後)を更新して追加
                            var renew = list.Last();
                            data.JoinEvent().SetDuplication(renew.CheckIn());

                            list.ToList().ForEach(x_ => x_.JoinEvent().SetDuplication());
                            listNoCheck.AddRange(list);

                            //LogFile.Instance.WriteLine($"{data.CheckOutNumber()} : {data.CheckInNumber()} -> {renew.CheckInNumber()}");
                            continue;
                        }
                    }
                }
                Interlocked.Increment(ref count); 
                ProgressCount?.Invoke(count, max);
            });
            foreach (var data in listCheckOutIn.Values)
            {
                this.AddRange(data);
            }
        }
    }

    public void AnalysisExEx(ConvertReportLog log_, IEnumerable<AnalysisStartShutdown> listStartShutdown_)
    {
        foreach (var startShutdown in listStartShutdown_)
        {
            // Start と Shutdown で区切る
            int count = 0;
            int max = 1;
            var listCheckOut = log_.ListEvent<LogEventCheckOut>(startShutdown);
            var listCheckIn = new SortedSet<LogEventCheckIn>(log_.ListEvent<LogEventCheckIn>(startShutdown));
            LogFile.Instance.WriteLine($"{startShutdown.StartDateTime.ToString()} - {startShutdown.ShutdownDateTime.ToString()} : {listCheckOut.Count()}");
            //
            //int div = 1;
            var divCheckOut = new SortedDictionary<string, SortedSet<LogEventCheckOut>>();
            var divCheckIn = new SortedDictionary<string, SortedSet<LogEventCheckIn>>();
            var listCheckOutIn = new SortedDictionary<string, List<AnalysisCheckOutIn>>();

            var countHandle = listCheckOut.Select(x_ => x_.HandleServer).Distinct();
            var listAdd = new ConcurrentBag<List<AnalysisCheckOutIn>>();
            count = 0;
            max = countHandle.Count();

            ProgressCount?.Invoke(0, max, _ANALYSIS + "Div " + startShutdown.StartDateTime.ToShortDateString());
            foreach (var key in countHandle)
            {
                var listOut = listCheckOut.Where(x_ => x_.HandleServer == key);

                if (listOut.Count() > 0)
                {
                    listCheckOutIn[key] = new();
                    //
                    divCheckOut[key] = new(listOut);
                    divCheckIn[key] = new(listCheckIn.Where(x_ => x_.HandleServer == key));
                }
            }

            // var listEnd = new SortedSet<long>();
            //var listEnd = new ConcurrentBag<long>();

            count = 0;
            max = divCheckOut.Keys.Count();
            //div = (max / 100) + 1;

            ProgressCount?.Invoke(0, max, _ANALYSIS + "Join " + startShutdown.StartDateTime.ToShortDateString());

            Parallel.ForEach(divCheckOut.Keys, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, div =>
            {
                //var listEnd = new SortedSet<long>();
                var listOut = divCheckOut[div];
                var listIn = divCheckIn[div];

                foreach (var checkOut in listOut)
                {
                    // 対応するチェックインを探す
                    //var checkIn = listIn.AsParallel().AsOrdered().FirstOrDefault(f_ => listEnd.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                    var checkIn = listIn.FirstOrDefault(f_ => f_.EventNumber> checkOut.EventNumber);
                    var data = new AnalysisCheckOutIn(checkOut, checkIn);
                    //listAdd.Add(data);

                    //listEnd.Add(checkIn?.EventNumber ?? 0);

                    listCheckOutIn[div].Add(data);
                }
                listOut.Clear();
                listIn.Clear();

                // 重複のチェック
                foreach (var key in listCheckOutIn.Keys)
                {
                    var listNoCheck = new List<AnalysisCheckOutIn>();
                    var listValue = listCheckOutIn[key].OrderBy(x_ => x_.CheckOutNumber());

                    if (listValue.Count() <= 1)
                    {
                        //count++;
                        continue;
                    }

                    foreach (var data in listValue)
                    {
                        if (listNoCheck.Contains(data) == true)
                        {
                            continue;
                        }
                        // 時間内に含まれるデータは除外
                        //var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                        var list = listValue.Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                        list.ToList().ForEach(x_ => x_.JoinEvent().SetDuplication());
                        listNoCheck.AddRange(list);
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
                        //var list = listValue.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                        var list = listValue.Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                        if (list.Count() > 0)
                        {
                            // 時間(最後)を更新して追加
                            var renew = list.Last();
                            data.JoinEvent().SetDuplication(renew.CheckIn());

                            //list.ForAll(x_ => x_.JoinEvent().SetDuplication());
                            list.ToList().ForEach(x_ => x_.JoinEvent().SetDuplication());
                            listNoCheck.AddRange(list);

                            //LogFile.Instance.WriteLine($"{data.CheckOutNumber()} : {data.CheckInNumber()} -> {renew.CheckInNumber()}");
                            continue;
                        }
                    }
                }
                Interlocked.Increment(ref count);
                ProgressCount?.Invoke(count, max);
            });
            foreach (var data in listCheckOutIn.Values)
            {
                this.AddRange(data);
            }
        }
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
    public void WriteText(string path_, long duplication_ = 0)
    {
        var list = new List<string>();
        // ヘッダー
        list.Add(Header(duplication_));
        // データ
        list.AddRange(ListValue(duplication_).Select(x_ => string.Join(",", x_)));
        File.WriteAllLines(path_, list, Encoding.UTF8);
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
