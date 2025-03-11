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
/// チェックアウトとチェックイン結合情報のリスト化 
/// </summary>
[Sort(1)]
internal sealed class ListAnalysisCheckOutIn : List<AnalysisCheckOutIn>, IAnalysisTextWrite
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

    public static ListKeyPair ListSelect
    {
        get => new()
        {
            //new( "重複なし", JoinEventCheckOutIn.NO_DUPLICATION),
            //new ("重複あり", JoinEventCheckOutIn.HAVE_DUPLICATION )
            new( "重複なし", (long)SelectData.ECLUSION),
            new ("重複あり", (long)SelectData.ALL)
        };
    }


    public string Header
    {
        get
        {
            var listColunm = new List<string>();
            var listPropetyInfo = typeof(AnalysisCheckOutIn).GetProperties(BindingFlags.Instance | BindingFlags.Public)?.OrderBy(s_ => (Attribute.GetCustomAttribute(s_, typeof(SortAttribute)) as SortAttribute)?.Sort);

            listPropetyInfo?.ToList().ForEach(prop =>
            {
                listColunm.Add($"{prop.Name}");
            });

            return string.Join(",", listColunm);
        }
    }

    /// <summary>
    /// 重複なしのデータリスト
    /// </summary>
    //public IEnumerable<AnalysisCheckOutIn> ListNoDuplication() => this.Where(x_ => x_.JoinEvent().DuplicationNumber == JoinEventCheckOutIn.NO_DUPLICATION);
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
        int count = 0;
        int max = 0;
        int div = 0;
        var listCheckOutIn = new Dictionary<string, List<AnalysisCheckOutIn>>();
        foreach (var startShutdown in listStartShutdown_)
        {
            // Start と Shutdown で区切る
            var listCheckOut =log_.ListEvent<LogEventCheckOut>(startShutdown).ToList();
            var listCheckIn = new SortedSet<LogEventCheckIn>( log_.ListEvent<LogEventCheckIn>(startShutdown));
            var listEnd= new SortedSet<long>();
            //var list = new BlockingCollection<long>();

            LogFile.Instance.WriteLine($"{startShutdown.StartDateTime.ToString()} - {startShutdown.ShutdownDateTime.ToString()} : {listCheckOut.Count()}");

            count = 0;
            max = listCheckOut.Count();
            div = (max / 1000)+1;
            //ProgressCount?.Invoke(0, max, _ANALYSIS + "Join");
            foreach (var checkOut in listCheckOut)
            //Parallel.ForEach(listCheckOut,async checkOut =>
            {
                // 対応するチェックインを探す
                var checkIn = listCheckIn.FirstOrDefault(f_ => listEnd.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                //var checkIn = listCheckIn.FirstOrDefault(f_ => list.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                //var checkIn = listCheckIn.AsParallel().AsOrdered().WithDegreeOfParallelism(4).FirstOrDefault(f_ => list.Contains(f_.EventNumber) == false && checkOut.IsFindCheckIn(f_.HandleServer, f_.EventNumber));
                //var checkIn = listCheckIn.AsParallel().AsOrdered().WithDegreeOfParallelism(4).FirstOrDefault(f_ => list.Contains(f_.EventNumber)==false&& checkOut.IsFindCheckIn(f_));
                //var checkIn = listCheckIn.AsParallel().AsOrdered().Where(f_=>f_.HandleServer == checkOut.HandleServer).FirstOrDefault(f_ =>f_.EventNumber> checkOut.EventNumber);
                var data = new AnalysisCheckOutIn(checkOut, checkIn);
                this.Add(data);

                //listCheckIn.Remove(checkIn);
                listEnd.Add(checkIn?.EventNumber ?? 0);


                // 重複のチェック(同一のプロダクト ユーザー ホスト)一覧
                var key = $"{data.Product} {data.UserHost}";
                if (listCheckOutIn.ContainsKey(key) == false)
                {
                    listCheckOutIn[key] = new ();
                }
                listCheckOutIn[key].Add(data);
                Interlocked.Increment(ref count);
                if ((count % div) == 0)
                { 
                    ProgressCount?.Invoke(count, max);
                }
                //await Task.Delay(10);
            //});
            }
        }

        // 重複のチェック
        count = 0;
        max = listCheckOutIn.Keys.Count;
        ProgressCount?.Invoke(0, max, _ANALYSIS + "Duplication");
        foreach (var key in listCheckOutIn.Keys)
        {
            var listNoCheck = new List<AnalysisCheckOutIn>();
            var listKeyData = listCheckOutIn[key].OrderBy(x_ => x_.CheckOutNumber());

            foreach (var data in listKeyData)
            {
               if (listNoCheck.Contains(data) == true)
                {
                    continue;
                }
                // 時間内に含まれるデータは除外
                var list = listKeyData.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                list.ForAll(x_ => x_.JoinEvent().SetDuplication());
                listNoCheck.AddRange(list);
            }

            //
            foreach (var data in listKeyData)
            {
                if (listNoCheck.Contains(data) == true)
                {
                    //チェック対象外は見ない
                    continue;
                }

                // チェックアウト時間のみが範囲
                var list = listKeyData.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                if (list.Count() > 0)
                {
                    // 時間(最後)を更新して追加
                    var renew = list.Last();
                    data.JoinEvent().SetDuplication(renew.CheckIn());

                    list.ForAll(x_ => x_.JoinEvent().SetDuplication());
                    listNoCheck.AddRange(list);

                    LogFile.Instance.WriteLine($"{data.CheckOutNumber()} : {data.CheckInNumber()} -> {renew.CheckInNumber()}");
                    continue;
                }
            }
            ProgressCount?.Invoke(++count, max);
        }
    }

    /// <summary>
    /// チェックインに対応するチェックアウトの検索
    /// </summary>
    /// <param name="checkIn_">チェックイン</param>
    /// <returns></returns>
    public LogEventCheckOut? Find(LogEventCheckIn checkIn_)=> this.ToList().Find(x_ => x_.IsSame(checkIn_))?.CheckOut() ?? null;

    /// <summary>
    /// ファイル保存(チェックアウト-チェックイン)
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="duplication_">ture:重複なし</param>
    public void WriteText(string path_, long duplication_ = 0)
    {
        var list = new List<string>();
        list.Add(AnalysisCheckOutIn.HEADER);
        list.AddRange(_listToString(duplication_!=0));
        File.WriteAllLines(path_, list, Encoding.UTF8);
    }


    /// <summary>
    /// ファイル保存(結合情報)
    /// </summary>
    /// <param name="path_">パス</param>
    public void WriteDuplicationText(string path_)
    {
        var list = new List<string>();
        list.Add(Header);
        list.AddRange(ListJointEvetn().Select(x_ => x_.ToString()));
        File.WriteAllLines(path_, list, Encoding.UTF8);
    }

    /// <summary>
    /// 文字列リスト化
    /// </summary>
    /// <param name="duplication_">ture:重複なし</param>
    private List<string> _listToString(bool duplication_)
    {
        var rtn = new List<string>();
        var list = (duplication_ == false) ? this : ListNoDuplication();
        foreach (var data in list)
        {
            rtn.Add(data.ToString(duplication_));
        }

        return rtn;
    }


}
