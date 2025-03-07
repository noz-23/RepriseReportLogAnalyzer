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
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// チェックアウトとチェックインを結合
/// </summary>
internal sealed class AnalysisCheckOutIn
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="checkOut_">チェックアウト イベント</param>
    /// <param name="checkIn_">チェックイン or シャットダウン イベント</param>
    public AnalysisCheckOutIn(LogEventCheckOut checkOut_, LogEventBase? checkIn_)
    {
        _checkOut = checkOut_;
        _checkIn = checkIn_;

        _joinEvent = new JoinEventCheckOutIn(checkOut_, checkIn_);
    }

    /// <summary>
    /// 文字列化のヘッダー
    /// </summary>
    public const string HEADER = "CheckOut Date Time,CheckIn Date Time,Duration,Product,Version,Product Version,User,Host,User@Host";

    /// <summary>
    /// チェックアウト時間
    /// </summary>
    [Sort(101)]
    public DateTime CheckOutDateTime { get => _checkOut.EventDateTime; }

    /// <summary>
    /// チェックイン時間
    /// </summary>
    [Sort(102)]
    public DateTime CheckInDateTime { get => _checkIn?.EventDateTime ?? LogEventBase.NowDateTime; }

    /// <summary>
    /// 利用時間
    /// </summary>
    [Sort(103)]
    public TimeSpan Duration { get => CheckInDateTime - CheckOutDateTime; }

    /// <summary>
    /// プロダクト
    /// </summary>
    [Sort(111)]
    public string Product { get => _checkOut.Product; }
    /// <summary>
    /// バージョン
    /// </summary>
    [Sort(112)]
    public string Version { get => _checkOut.Version; }
    /// <summary>
    /// プロダクト バージョン
    /// </summary>
    [Sort(113)]
    public string ProductVersion { get => _checkOut.ProductVersion; }
    /// <summary>
    /// ユーザー
    /// </summary>
    [Sort(121)]
    public string User { get => _checkOut.User; }
    /// <summary>
    /// ホスト
    /// </summary>
    [Sort(121)]
    public string Host { get => _checkOut.Host; }
    /// <summary>
    /// ユーザー@ホスト
    /// </summary>
    [Sort(121)]
    public string UserHost { get => _checkOut.UserHost; }

    /// <summary>
    /// チェックアウト イベント
    /// </summary>
    private readonly LogEventCheckOut _checkOut;
    /// <summary>
    /// チェックイン(シャットダウン) イベント
    /// </summary>
    private readonly LogEventBase? _checkIn = null;
    /// <summary>
    /// 結合情報
    /// </summary>
    private readonly JoinEventCheckOutIn _joinEvent;

    /// <summary>
    /// チェックアウト イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    public LogEventCheckOut CheckOut() => _checkOut;

    /// <summary>
    /// チェックイン イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    public LogEventBase? CheckIn() => _checkIn;

    /// <summary>
    /// 結合情報(リフレクションで呼び出さないため関数化)
    /// </summary>
    public JoinEventCheckOutIn JoinEvent() => _joinEvent;

    /// <summary>
    /// チェックアウトのイベント番号
    /// </summary>
    public long CheckOutNumber() => _checkOut.EventNumber;

    /// <summary>
    /// チェックインのイベント番号
    /// </summary>
    public long CheckInNumber() => _checkIn?.EventNumber ?? LogEventBase.NowEventNumber;

    /// <summary>
    /// 同一のチェックインのか？
    /// </summary>
    /// <param name="checkIn_">チェックイン イベント</param>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSame(LogEventCheckIn checkIn_)
    {
        return _checkIn == checkIn_;
    }

    /// <summary>
    /// チェックアウトとチェックインの間のイベントか？
    /// </summary>
    /// <param name="number_">イベント番号</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithInRange(long number_)
    {
        return (number_ > CheckOutNumber()) && (number_ < CheckInNumber());
    }

    /// <summary>
    /// チェックアウトとチェックインの間のイベントか？
    /// </summary>
    /// <param name="dateTime_">イベント時間</param>
    public bool IsWithInRange(DateTime dateTime_)
    {
        return (dateTime_ > CheckOutDateTime) && (dateTime_ < CheckInDateTime);
    }

    /// <summary>
    /// 重複を取り除いたチェックインの時間
    /// </summary>
    public DateTime JointDateTime() => _joinEvent.CheckIn()?.EventDateTime ?? LogEventBase.NowDateTime;

    /// <summary>
    /// 重複を取り除いた利用時間
    /// </summary>
    public TimeSpan DurationDuplication()
    {
        return JointDateTime() - CheckOutDateTime;
    }

    /// <summary>
    /// グループ集計する名称
    /// </summary>
    /// <param name="group_"></param>
    public string GroupName(ANALYSIS_GROUP group_)
    {
        switch (group_)
        {
            case ANALYSIS_GROUP.USER: return User;
            case ANALYSIS_GROUP.HOST: return Host;
            case ANALYSIS_GROUP.USER_HOST: return UserHost;
            default:
                break;
        }
        return string.Empty;
    }

    /// <summary>
    /// 文字列化
    /// </summary>
    /// <param name="duplication_">重複除去</param>
    public string ToString(bool duplication_)
    {
        if (duplication_ == true)
        {
            return $"{CheckOutDateTime.ToString()},{JointDateTime().ToString()},{DurationDuplication().ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
        }

        return ToString();
    }

    /// <summary>
    /// 文字列化
    /// </summary>

    public override string ToString()
    {
        return $"{CheckOutDateTime.ToString()},{CheckInDateTime.ToString()},{Duration.ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
    }


}
