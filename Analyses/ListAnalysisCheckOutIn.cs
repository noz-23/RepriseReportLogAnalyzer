using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using RepriseReportLogAnalyzer.Windows;
using System.IO;
using System.Text;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// チェックアウトとチェックイン結合情報のリスト化 
/// </summary>
internal sealed class ListAnalysisCheckOutIn : List<AnalysisCheckOutIn>
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

    /// <summary>
    /// 解析処理
    /// </summary>
    /// <param name="log_">イベント ログ情報</param>
    /// <param name="listStartShutdown_">スタートとシャットダウンの時間帯リスト</param>
    public void Analysis(AnalysisReportLog log_, IEnumerable<AnalysisStartShutdown> listStartShutdown_)
    {
        int count = 0;
        int max = 0;
        var listCheckOutIn = new Dictionary<string, List<AnalysisCheckOutIn>>();
        foreach (var startShutdown in listStartShutdown_)
        {
            var listCheckOut = log_.GetListEvent<LogEventCheckOut>(startShutdown);
            var listCheckIn = log_.GetListEvent<LogEventCheckIn>(startShutdown);

            LogFile.Instance.WriteLine($"{startShutdown.StartDateTime.ToString()} - {startShutdown.ShutdownDateTime.ToString()} : {listCheckOut.Count}");

            count = 0;
            max = listCheckOut.Count;
            ProgressCount?.Invoke(0, max, _ANALYSIS + "Join");
            foreach (var checkOut in listCheckOut)
            {
                AnalysisCheckOutIn data;
                try
                {
                    // 対応するチェックインを探す
                    var checkIn = listCheckIn.AsParallel().AsOrdered().First(f_ => checkOut.IsFindCheckIn(f_));
                    data = new AnalysisCheckOutIn(checkOut, checkIn);

                    if (checkIn != null)
                    {
                        listCheckIn.Remove(checkIn);
                    }
                }
                catch
                {
                    data = new AnalysisCheckOutIn(checkOut, startShutdown?.EventShutdown());
                    LogFile.Instance.WriteLine($"Check In Not Found");
                }
                this.Add(data);

                // 重複のチェック(同一のプロダクト ユーザー ホスト)一覧
                var key = $"{data.Product} {data.UserHost}";
                if (listCheckOutIn.ContainsKey(key) == false)
                {
                    listCheckOutIn[key] = new List<AnalysisCheckOutIn>();
                }
                listCheckOutIn[key].Add(data);

                ProgressCount?.Invoke(++count, max);
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

            // 時間内に含まれるデータは除外
            foreach (var data in listKeyData)
            {
                if (listNoCheck.Contains(data) == true)
                {
                    continue;
                }
                var list = listKeyData.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && data.IsWithInRange(x_.CheckInNumber()));
                list.ForAll(x_ => x_.JoinEvent().SetDuplication());

                listNoCheck.AddRange(list);
            }

            //
            foreach (var data in listKeyData)
            {
                if (listNoCheck.Contains(data) == true)
                {
                    continue;
                }

                // チェックアウト時間のみが範囲
                var list = listKeyData.AsParallel().Where(x_ => data.IsWithInRange(x_.CheckOutNumber()) && (listNoCheck.Contains(x_) == false)).OrderBy(x_ => x_.CheckInNumber());
                if (list.Count() > 0)
                {
                    // 時間(最後)を更新して追加
                    var renew = list.Last();
                    data.JoinEvent().SetDuplication(renew.CheckIn());

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
    /// 文字列リスト化
    /// </summary>
    /// <param name="duplication_">ture:重複なし</param>
    private List<string> _listToString(bool duplication_)
    {
        var rtn = new List<string>();
        var list = (duplication_ == false) ? this : ListDuplication();
        foreach (var data in list)
        {
            rtn.Add(data.ToString(duplication_));
        }

        return rtn;
    }

    /// <summary>
    /// ファイル保存(チェックアウト-チェックイン)
    /// </summary>
    /// <param name="path_">パス</param>
    /// <param name="duplication_">ture:重複なし</param>
    public void WriteText(string path_, bool duplication_ = false)
    {
        var list = new List<string>();
        list.Add(AnalysisCheckOutIn.HEADER);
        list.AddRange(_listToString(duplication_));
        File.WriteAllLines(path_, list, Encoding.UTF8);
    }

    /// <summary>
    /// 重複なしのデータリスト
    /// </summary>
    public IEnumerable<AnalysisCheckOutIn> ListDuplication() => this.Where(x_ => x_.JoinEvent().DuplicationNumber != JoinEventCheckOutIn.DUPLICATION);

    /// <summary>
    /// 結合情報リスト
    /// </summary>

    public IEnumerable<JoinEventCheckOutIn> ListJointEvetn() => this.Select(x_ => x_.JoinEvent());

    /// <summary>
    /// ファイル保存(結合情報)
    /// </summary>
    /// <param name="path_">パス</param>
    public void WriteDuplicationText(string path_)
    {
        var list = new List<string>();
        list.Add(JoinEventCheckOutIn.HEADER);
        list.AddRange(ListJointEvetn().Select(x_ => x_.ToString()));
        File.WriteAllLines(path_, list, Encoding.UTF8);
    }


}
