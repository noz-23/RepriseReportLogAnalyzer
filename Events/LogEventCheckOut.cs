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
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventCheckOut = Regist("OUT", (l_) => new LogEventCheckOut(l_));
}

/// <summary>
/// checkout
/// </summary>
[Sort(11)][Table("TbCheckOut")]
internal sealed class LogEventCheckOut : LogEventBase, ILogEventUserHost, ILogEventCountCurrent
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventCheckOut(string[] list_) : base()
    {
        if (list_.Count() < 10)
        {
            // smail
            Product = list_[1];
            Version = list_[2];
            User = list_[3];
            Host = list_[4];
            IsvDef = list_[5];
            Count = int.Parse(list_[6]);
            //
            HandleServer = list_[7];
            HandleShare = list_[8];
            //
            EventDateTime = DateTime.Parse(_NowDate + " " + list_[9]);
            LogFormat = LogFormat.SMALL;
        }
        else
        {
            // std
            // detailed
            Product = list_[1];
            Version = list_[2];
            Pool = list_[3];
            User = list_[4];
            Host = list_[5];
            IsvDef = list_[6];
            //
            Count = int.Parse(list_[7]);
            CountCurrent = int.Parse(list_[8]);
            ResuseCurrent = int.Parse(list_[9]);
            //
            HandleServer = list_[10];
            HandleShare = list_[11];
            //
            ProcessId = list_[12];
            Project = list_[13];
            RequestedProduct = list_[14];
            RequestedVersion = list_[15];

            //
            EventDateTime = _GetDateTime(list_[16], list_[17]);
            LogFormat = (list_[17].Contains(".") == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
        }
    }

    //checkout
    //OUT product version pool# user  host      “isv_def” count         cur_use      cur_resuse server_handle share_handle process_id “project” “requested product” “requested version” mm/dd hh:mm:ss
    //OUT product version pool# user  host      “isv_def” count         cur_use      cur_resuse server_handle share_handle process_id “project” “requested product” “requested version” mm/dd hh:mm:ss.tenths_of_msec “client_machine_os_info” “application argv0” roam_days roam_handle client-ip-address
    //OUT product version user  host “isv_def” count      server_handle share_handle hh:mm
    //0   1       2       3     4     5          6          7             8            9          10            11           12          13          14                    15                   16    17                       18                         19                   20        21          22
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
    [Sort(21)]
    [Column("User")]
    public string User { get; private set; } = string.Empty;

    [Sort(22)]
    [Column("Host")]
    public string Host { get; private set; } = string.Empty;

    [Sort(23)]
    [Column("User@Host")]
    public string UserHost { get => User + "@" + Host; }
    //
    [Sort(101)]
    [Column("Isv Def")]
    public string IsvDef { get; private set; } = string.Empty;

    [Sort(102)]
    [Column("Pool")]
    public string Pool { get; private set; } = string.Empty;
    //
    [Sort(103)]
    [Column("Count")]
    public int Count { get; private set; } = -1;

    [Sort(104)]
    [Column("Current Count")]
    public int CountCurrent { get; private set; } = -1;
 
    [Sort(105)]
    [Column("Current Resuse")]
    public int ResuseCurrent { get; private set; } = -1;
    //
    [Sort(106)]
    [Column("Server Handle")]
    public string HandleServer { get; private set; } = string.Empty;

    [Sort(107)]
    [Column("Share Handle")]
    public string HandleShare { get; private set; } = string.Empty;

    [Sort(108)]
    [Column("Process ID")]
    public string ProcessId { get; private set; } = string.Empty;
    //
    [Sort(109)]
    [Column("Project")]
    public string Project { get; private set; } = string.Empty;

    [Sort(110)]
    [Column("Requested Product")]
    public string RequestedProduct { get; private set; } = string.Empty;

    [Sort(111)]
    [Column("Requested Version")]
    public string RequestedVersion { get; private set; } = string.Empty;
    //

    /// <summary>
    /// チェックアウト に対応するチェックイン情報か
    /// </summary>
    /// <param name="checkIn_"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFindCheckIn(LogEventCheckIn checkIn_)=> IsFindCheckIn(checkIn_.HandleServer, checkIn_.EventNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFindCheckIn(string hande_,long number_) => (hande_ == HandleServer) && (number_ > EventNumber);
}
