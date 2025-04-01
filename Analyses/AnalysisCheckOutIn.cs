/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Data;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Interfaces;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses;

/// <summary>
/// チェックアウトとチェックインを結合
/// </summary>
internal sealed class AnalysisCheckOutIn : ToDataBase, IComparer, IComparable
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="checkOut_">チェックアウト イベント</param>
    /// <param name="checkIn_">チェックイン or シャットダウン イベント</param>
    public AnalysisCheckOutIn(LogEventCheckOut checkOut_, LogEventBase checkIn_)
    {
        _checkOut = checkOut_;
        _checkIn = checkIn_;

        _joinEvent = new JoinEventCheckOutIn(checkOut_, checkIn_);
    }

    /// <summary>
    /// チェックアウト時間
    /// </summary>
    [Column("CheckOut Date Time", Order = 101)]
    public DateTime CheckOutDateTime { get => _checkOut.EventDateTime; }

    /// <summary>
    /// チェックイン時間
    /// </summary>
    [Column("CheckIn Date Time", Order = 102)]
    public DateTime CheckInDateTime { get => _checkIn?.EventDateTime ?? LogEventBase.NowDateTime; }

    /// <summary>
    /// 利用時間
    /// </summary>
    [Column("Duration", Order = 103)]
    public TimeSpan Duration { get => CheckInDateTime - CheckOutDateTime; }

    /// <summary>
    /// プロダクト
    /// </summary>
    [Column("Product", Order = 111)]
    public string Product { get => _checkOut.Product; }
    /// <summary>
    /// バージョン
    /// </summary>
    [Column("Version", Order = 112)]
    public string Version { get => _checkOut.Version; }
    /// <summary>
    /// プロダクト バージョン
    /// </summary>
    [Column("Product Version", Order = 113)]
    public string ProductVersion { get => _checkOut.ProductVersion; }
    /// <summary>
    /// ユーザー
    /// </summary>
    [Column("User", Order = 121)]
    public string User { get => _checkOut.User; }
    /// <summary>
    /// ホスト
    /// </summary>
    [Column("Host", Order = 121)]
    public string Host { get => _checkOut.Host; }
    /// <summary>
    /// ユーザー@ホスト
    /// </summary>
    [Column("User@Host", Order = 121)]
    public string UserHost { get => _checkOut.UserHost; }

    /// <summary>
    /// チェックアウト イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogEventCheckOut CheckOut() => _checkOut;
    private readonly LogEventCheckOut _checkOut;

    /// <summary>
    /// チェックイン イベント(リフレクションで呼び出さないため関数化)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogEventBase CheckIn() => _checkIn;
    private readonly LogEventBase _checkIn;

    /// <summary>
    /// 結合情報(リフレクションで呼び出さないため関数化)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JoinEventCheckOutIn JoinEvent() => _joinEvent;
    private readonly JoinEventCheckOutIn _joinEvent;

    /// <summary>
    /// チェックアウトのイベント番号
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long CheckOutNumber() => _checkOut.EventNumber;

    /// <summary>
    /// チェックインのイベント番号
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long CheckInNumber() => _checkIn?.EventNumber ?? LogEventBase.NowEventNumber;

    /// <summary>
    /// チェックアウトとチェックインの間のイベントか？
    /// </summary>
    /// <param name="number_">イベント番号</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithInRange(long number_) => (number_ > CheckOutNumber()) && (number_ < CheckInNumber());

    /// <summary>
    /// チェックアウトとチェックインの間のイベントか？
    /// </summary>
    /// <param name="dateTime_">イベント時間</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsWithInRange(DateTime dateTime_) => (dateTime_ > CheckOutDateTime) && (dateTime_ < CheckInDateTime);

    /// <summary>
    /// 重複を取り除いたチェックインの時間
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DateTime JointDateTime() => _joinEvent.CheckIn()?.EventDateTime ?? LogEventBase.NowDateTime;

    /// <summary>
    /// 重複を取り除いた利用時間
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeSpan DurationDuplication() => (JointDateTime() - CheckOutDateTime);

    /// <summary>
    /// グループ集計する名称
    /// </summary>
    /// <param name="group_"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GroupName(AnalysisGroup group_)
    {
        switch (group_)
        {
            case AnalysisGroup.USER: return User;
            case AnalysisGroup.HOST: return Host;
            case AnalysisGroup.USER_HOST: return UserHost;
            default:
                break;
        }
        return string.Empty;
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    /// <returns></returns>

    public static string Header() => ToDataBase.Header(typeof(AnalysisCheckOutIn));

    /// <summary>
    /// リスト化したヘッダー項目
    /// </summary>
    /// <returns></returns>
    public static ListStringStringPair ListHeader() => ToDataBase.ListHeader(typeof(AnalysisCheckOutIn));

    /// <summary>
    /// リスト化したデータ(重複除去)
    /// </summary>
    /// <returns></returns>
    public List<string> ListDuplicationValue()
    {
        return new()
        {
            $"{CheckOutDateTime.ToString(Properties.Settings.Default.FORMAT_DATE_TIME, CultureInfo.InvariantCulture)}",
            $"{JointDateTime().ToString(Properties.Settings.Default.FORMAT_DATE_TIME, CultureInfo.InvariantCulture)}",
            $"{DurationDuplication().ToString(Properties.Settings.Default.FORMAT_TIME_SPAN, CultureInfo.InvariantCulture)}",
            $"{Product}",
            $"{Version}",
            $"{ProductVersion}",
            $"{User}",
            $"{Host}",
            $"{UserHost}"
        };
    }

    /// <summary>
    /// 比較処理
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <returns></returns>
    public int Compare(object? a_, object? b_) => (a_ is AnalysisCheckOutIn a) ? a.CompareTo(b_) : -1;

    /// <summary>
    /// 比較処理
    /// </summary>
    /// <param name="b_"></param>
    /// <returns></returns>
    public int CompareTo(object? b_) => (b_ is AnalysisCheckOutIn b) ? (int)(CheckOutNumber() - b.CheckOutNumber()) : -1;
}
