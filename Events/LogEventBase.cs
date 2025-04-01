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
using RepriseReportLogAnalyzer.Files;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録クラス
/// </summary>
internal sealed partial class LogEventRegist
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public static LogEventRegist Instance = new LogEventRegist();
    private LogEventRegist()
    {
    }

    /// <summary>
    /// 作成した時点で各値が入り登録
    /// </summary>
    public void Create()
    {
        LogFile.Instance.WriteLine($"Create {typeof(LogEventRegist)}");
    }

    /// <summary>
    /// 登録処理
    /// </summary>
    /// <param name="key_"></param>
    /// <param name="event_"></param>
    /// <returns></returns>
    public static bool Regist(string key_, LogEventBase.NewLogEvent event_) => LogEventBase.Regist(key_, event_);
}

/// <summary>
/// ログ イベント(ベース)
/// </summary>
[Sort(99)]
internal partial class LogEventBase : ToDataBase, IComparer, IComparable
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LogEventBase()
    {
        EventNumber = ++NowEventNumber;
    }

    /// <summary>
    /// 各イベントのコンストラクタ(new)を登録するためのデリゲート
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public delegate LogEventBase NewLogEvent(string[] list);

    /// <summary>
    /// 各イベントの文字とデリゲートを紐づけるリスト
    /// </summary>
    private static SortedList<string, NewLogEvent> _listEventData = new();

    /// <summary>
    /// 登録関数
    /// </summary>
    /// <param name="key_">ログの先頭文字列</param>
    /// <param name="event_">登録関数デリゲート</param>
    /// <returns></returns>
    public static bool Regist(string key_, NewLogEvent event_)
    {
        _listEventData.Add(key_, event_);
        return true;
    }

    /// <summary>
    /// 現在時間を取り扱わない時間として利用する
    /// </summary>
    public static readonly DateTime NotAnalysisEventTime = DateTime.Now;


    /// <summary>
    /// 現在のイベント時間
    /// </summary>
    public static DateTime NowDateTime = NotAnalysisEventTime;
    /// <summary>
    /// 現在のイベント番号(重複なし)
    /// </summary>
    public static long NowEventNumber;

    /// <summary>
    /// 日付設定に利用
    /// </summary>
    protected static long _NowYear { get => NowDateTime.Year; }
    protected static long _NowMonth { get => NowDateTime.Month; }
    protected static string _NowDate { get => NowDateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture); }

    /// <summary>
    /// イベント番号
    /// </summary>
    [Column("No", Order = 1)]
    public long EventNumber { get; protected set; } = 0;

    /// <summary>
    /// イベント時間
    /// </summary>
    [Column("DateTime", Order = 2)]
    public DateTime EventDateTime
    {
        get => _eventDateTime;
        set
        {
            _eventDateTime = value;
            NowDateTime = value;
        }
    }
    private DateTime _eventDateTime;


    [Column("Format", Order = ToDataBase.NO_OUPUT_DATA)]
    public LogFormat LogFormat { get; set; } = LogFormat.NONE;

    /// <summary>
    /// イベント日付
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime EventDate() => EventDateTimeUnit(TimeSpan.TicksPerDay);

    /// <summary>
    /// 日時を単位時間に分けた時の時間帯
    /// 15分単位なら 16:13 →16:00
    /// </summary>
    /// <param name="timeSpan_">時間帯</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime EventDateTimeUnit(long timeSpan_) => new DateTime(EventDateTime.Ticks - (EventDateTime.Ticks % timeSpan_));

    /// <summary>
    /// 登録してある
    /// </summary>
    /// <param name="str_"></param>
    /// <returns></returns>
    public static LogEventBase? EventData(string str_)
    {
        // スペース区切りの文字列を配列に分割("hoge hoge"にも考慮)
        var list = _splitSpace(str_);

        if (_listEventData.TryGetValue(list[0], out var newEvent) == true)
        {
            // 一致している場合
            return newEvent?.Invoke(list);
        }

        //if (list.Count() == 2 && list[0].Contains("/") == true && list[1].Contains(":") == true)
        if (list.Length == 2 && list[0].Contains('/') == true && list[1].Contains(':') == true)
        {
            // タイムスタンプ
            return new LogEventTimeStamp(list);
        }
        return null;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public static void Clear()
    {
        NowEventNumber = 0;
        NowDateTime = NotAnalysisEventTime;
    }

    /// <summary>
    /// スペースで分割するが "" で括られている場合はしない
    /// ｢a "b c" d｣→｢a,"b c",d｣ 
    /// </summary>
    /// <param name="str_"></param>
    /// <returns></returns>
    private static string[] _splitSpace(string str_)
    {
        var list = str_.Split(' ');// ｢a "b c" d｣→｢a,"b,c",d｣
        var rtn = new List<string>();

        string tmp = string.Empty;
        foreach (var s in list)
        {
            if (string.IsNullOrEmpty(tmp) == true)
            {
                if (s.Contains('\"') == true)
                {
                    if (s.IndexOf('\"') == s.LastIndexOf('\"'))
                    {
                        tmp += s;
                        continue;
                    }
                }
            }
            else
            {
                if (s.Contains('\"') == true)
                {
                    tmp += s;
                    rtn.Add(tmp);
                    tmp = string.Empty;
                }
                else
                {
                    tmp += s;
                }
                continue;
            }
            rtn.Add(s);
        }
        // ｢a,"b,c",d｣→｢a,"b c",d｣
        return rtn.ToArray();
    }

    /// <summary>
    /// 月日だけの場合は年を追加する(翌年1/1を考慮)
    /// </summary>
    /// <param name="date_"></param>
    /// <param name="time_"></param>
    /// <returns></returns>
    protected static DateTime _GetDateTime(string date_, string time_)
    {
        var listDate = date_.Split('/');
        var month = int.Parse(listDate[0], CultureInfo.InvariantCulture);
        // New Year
        var year = (month < _NowMonth) ? (_NowYear + 1) : (_NowYear);
        return DateTime.Parse(date_ + "/" + year + " " + time_, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 比較処理
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <returns></returns>
    public int Compare(object? a_, object? b_) => (a_ is LogEventBase a) ? a.CompareTo(b_) : -1;
    //{
    //    if (a_ is LogEventBase a)
    //    {
    //        //if (b_ is LogEventBase b)
    //        //{
    //        //    return (int)(a.EventNumber - b.EventNumber);
    //        //}
    //        return a.CompareTo(b_);
    //    }
    //    return -1;
    //}

    /// <summary>
    /// 比較処理
    /// </summary>
    /// <param name="b_"></param>
    /// <returns></returns>
    public int CompareTo(object? b_) => (b_ is LogEventBase b) ? (int)(EventNumber - b.EventNumber) : -1;
}
