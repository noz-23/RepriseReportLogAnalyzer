﻿/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

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
[Sort(30)]
[Table("TbProduct")]
internal sealed class LogEventProduct : LogEventBase, ILogEventProduct, ILicenseCount
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
        Count = int.Parse(list_[4], CultureInfo.InvariantCulture);
        Reservations = int.Parse(list_[5], CultureInfo.InvariantCulture);
        LimitSoft = int.Parse(list_[6], CultureInfo.InvariantCulture);
        //
        HostId = list_[7];
        Contract = list_[8];
        Customer = list_[9];
        Issuer = list_[10];
        LineItem = list_[11];
        Options = list_[12];
        //
        Share = int.Parse(list_[13], CultureInfo.InvariantCulture);
        ShareMax = int.Parse(list_[14], CultureInfo.InvariantCulture);
        ShareType = int.Parse(list_[15], CultureInfo.InvariantCulture);
        CountNamedUser = int.Parse(list_[16], CultureInfo.InvariantCulture);
        MeterType = int.Parse(list_[17], CultureInfo.InvariantCulture);
        MeterCounter = int.Parse(list_[18], CultureInfo.InvariantCulture);
        MeterInitialDecrement = int.Parse(list_[19], CultureInfo.InvariantCulture);
        MeterPeriod = int.Parse(list_[20], CultureInfo.InvariantCulture);
        MeterPeriodDecrement = int.Parse(list_[21], CultureInfo.InvariantCulture);
    }

    //support for a product
    //PRODUCT name     version pool# count #reservations soft_limit “hostid” “contract” “customer” “issuer” “line_item” “options”        share max_share type named_user_count meter_type meter_counter meter_initial_decrement meter_period meter_period_decrement
    //0       1        2       3     4     5             6           7          8            9            10         11            12                13    14        15   16               17         18            19                      20           21
    //PRODUCT AbcName  XY      1     1     0             1           ""         "A-B-C-D-E"  ""           ""         ""            "LA:xx_XX TY:XXX" 3     0         0    0                0          0             0                       0            0
    [Column("Product", Order = 11, TypeName = "TEXT")]
    public string Product { get; private set; } = string.Empty;

    [Column("Version", Order = 12, TypeName = "TEXT")]
    public string Version { get; private set; } = string.Empty;

    [Column("Product Version", Order = 13, TypeName = "TEXT")]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Column("Pool", Order = 101, TypeName = "TEXT")]
    public string Pool { get; private set; } = string.Empty;
    //
    [Column("Count", Order = 102, TypeName = "INTEGER")]
    public int Count { get; private set; } = -1;

    [Column("Reservations", Order = 103, TypeName = "INTEGER")]
    public int Reservations { get; private set; } = -1;

    [Column("Limit Soft", Order = 104, TypeName = "INTEGER")]
    public int LimitSoft { get; private set; } = -1;
    //
    [Column("Host ID", Order = 105, TypeName = "TEXT")]
    public string HostId { get; private set; } = string.Empty;
    //
    [Column("Contract", Order = 106, TypeName = "TEXT")]
    public string Contract { get; private set; } = string.Empty;

    [Column("Customer", Order = 107, TypeName = "TEXT")]
    public string Customer { get; private set; } = string.Empty;

    [Column("Issuer", Order = 108, TypeName = "TEXT")]
    public string Issuer { get; private set; } = string.Empty;

    [Column("Line Item", Order = 109, TypeName = "TEXT")]
    public string LineItem { get; private set; } = string.Empty;

    [Column("Options", Order = 110, TypeName = "TEXT")]
    public string Options { get; private set; } = string.Empty;

    [Column("Share", Order = 111, TypeName = "INTEGER")]
    public int Share { get; private set; } = -1;

    [Column("Max Share", Order = 112, TypeName = "INTEGER")]
    public int ShareMax { get; private set; } = -1;

    [Column("Type Share", Order = 113, TypeName = "INTEGER")]
    public int ShareType { get; private set; } = -1;

    [Column("Count Named User", Order = 114, TypeName = "INTEGER")]
    public int CountNamedUser { get; private set; } = -1;

    [Column("Meter Type", Order = 115, TypeName = "INTEGER")]
    public int MeterType { get; private set; } = -1;

    [Column("Counter Counter", Order = 116, TypeName = "INTEGER")]
    public int MeterCounter { get; private set; } = -1;

    [Column("Meter Initial Decrement", Order = 117, TypeName = "INTEGER")]
    public int MeterInitialDecrement { get; private set; } = -1;

    [Column("Meter Period", Order = 118, TypeName = "INTEGER")]
    public int MeterPeriod { get; private set; } = -1;

    [Column("Meter Period Decrement", Order = 119, TypeName = "INTEGER")]
    public int MeterPeriodDecrement { get; private set; } = -1;
    //
    /// <summary>
    /// ライセンスカウント処理
    /// </summary>
    /// <param name="listCount_"></param>
    /// <returns></returns>
    public bool SetCount(IDictionary<string, AnalysisLicenseCount.LicenseCount> listCount_)
    {
        if (string.IsNullOrEmpty(Product) == true)
        {
            return false;
        }
        var data = listCount_[Product];
        data.ServerHave = Count;
        listCount_[Product] = data;

        return true;
    }

}
