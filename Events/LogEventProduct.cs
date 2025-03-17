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
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventProduct = Regist("PRODUCT", (l_) => new LogEventProduct(l_));
}

/// <summary>
/// support for a product
/// </summary>
[Sort(30)][Table("TbProduct")]
internal sealed class LogEventProduct : LogEventBase, ILogEventProduct
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>

    public LogEventProduct(string[] list_) : base()
    {
        // small
        // std
        // detailed
        Product = list_[1];
        Version = list_[2];
        Pool = list_[3];
        //
        Count = int.Parse(list_[4]);
        Reservations = int.Parse(list_[5]);
        LimitSoft = int.Parse(list_[6]);
        //
        HostId = list_[7];
        Contract = list_[8];
        Customer = list_[9];
        Issuer = list_[10];
        LineItem = list_[11];
        Options = list_[12];
        //
        Share = int.Parse(list_[13]);
        ShareMax = int.Parse(list_[14]);
        ShareType = int.Parse(list_[15]);
        CountNamedUser = int.Parse(list_[16]);
        MeterType = int.Parse(list_[17]);
        MeterCounter = int.Parse(list_[18]);
        MeterInitialDecrement = int.Parse(list_[19]);
        MeterPeriod = int.Parse(list_[20]);
        MeterPeriodDecrement = int.Parse(list_[21]);

        EventDateTime = NowDateTime;
        LogFormat = LogFormat.NONE;
    }

    //support for a product
    //PRODUCT name     version pool# count #reservations soft_limit “hostid” “contract” “customer” “issuer” “line_item” “options”        share max_share type named_user_count meter_type meter_counter meter_initial_decrement meter_period meter_period_decrement
    //0       1        2       3     4     5             6           7          8            9            10         11            12                13    14        15   16               17         18            19                      20           21
    //PRODUCT AbcName  25      1     1     0             1           ""         "A-B-C-D-E"  ""           ""         ""            "LA:xx_XX TY:XXX" 3     0         0    0                0          0             0                       0            0
    [Sort(11)]
    [Column("Product")]
    public string Product { get; private set; } = string.Empty;

    [Sort(12)]
    [Column("Version")]
    public string Version { get; private set; } = string.Empty;

    [Sort(13)]
    [Column("Product Version")]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Sort(101)]
    [Column("Pool")]
    public string Pool { get; private set; } = string.Empty;
    //
    [Sort(102)]
    [Column("Count")]
    public int Count { get; private set; } = -1;

    [Sort(103)]
    [Column("Reservations")]
    public int Reservations { get; private set; } = -1;

    [Sort(104)]
    [Column("Limit Soft")]
    public int LimitSoft { get; private set; } = -1;
    //
    [Sort(105)]
    [Column("Host ID")]
    public string HostId { get; private set; } = string.Empty;
    //
    [Sort(106)]
    [Column("Contract")]
    public string Contract { get; private set; } = string.Empty;

    [Sort(107)]
    [Column("Customer")]
    public string Customer { get; private set; } = string.Empty;

    [Sort(108)]
    [Column("Issuer")]
    public string Issuer { get; private set; } = string.Empty;

    [Sort(109)]
    [Column("Line Item")]
    public string LineItem { get; private set; } = string.Empty;

    [Sort(110)]
    [Column("Options")]
    public string Options { get; private set; } = string.Empty;

    [Sort(111)]
    [Column("Share")]
    public int Share { get; private set; } = -1;

    [Sort(112)]
    [Column("Max Share")]
    public int ShareMax { get; private set; } = -1;

    [Sort(113)]
    [Column("Type Share")]
    public int ShareType { get; private set; } = -1;

    [Sort(114)]
    [Column("Count Named User")]
    public int CountNamedUser { get; private set; } = -1;

    [Sort(115)]
    [Column("Meter Type")]
    public int MeterType { get; private set; } = -1;

    [Sort(116)]
    [Column("Counter Counter")]
    public int MeterCounter { get; private set; } = -1;

    [Sort(117)]
    [Column("Meter Initial Decrement")]
    public int MeterInitialDecrement { get; private set; } = -1;

    [Sort(118)]
    [Column("Meter Period")]
    public int MeterPeriod { get; private set; } = -1;

    [Sort(119)]
    [Column("Meter Period Decrement")]
    public int MeterPeriodDecrement { get; private set; } = -1;
    //
}
