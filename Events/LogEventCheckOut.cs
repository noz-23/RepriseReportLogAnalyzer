/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using OpenTK.Compute.OpenCL;
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Net;
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
[Sort(11)]
[Table("TbCheckOut")]
internal sealed class LogEventCheckOut : LogEventBase, ILogEventUserHost, ILicenseCount
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventCheckOut(string[] list_) : base()
    {
        //if (list_.Count() < 10)
        if (list_.Length < 10)
        {
            _initSmall(list_);
        }else
        {
            _initStandard(list_);
        }
    }

    private void _initSmall(string[] list_)
    {
        // smail
        Product = list_[_INDEX_PRODUCT];
        Version = list_[_INDEX_VERSION];
        User = list_[_INDEX_SML_USER];
        Host = list_[_INDEX_SML_HOST];
        IsvDef = list_[_INDEX_SML_ISV_DEF];
        Count = int.Parse(list_[_INDEX_SML_COUNT], CultureInfo.InvariantCulture);
        //
        HandleServer = list_[_INDEX_SML_SERVER_HANDLE];
        HandleShare = list_[_INDEX_SML_SHARE_HANDLE];
        //
        //EventDateTime = DateTime.Parse(_NowDate + " " + list_[_INDEX_SML_TIME], CultureInfo.InvariantCulture);
        EventDateTime = _GetDateTime(list_[_INDEX_SML_TIME]);

        LogFormat = LogFormat.SMALL;
    }

    private void _initStandard(string[] list_)
    {
        // std
        // detailed
        Product = list_[_INDEX_PRODUCT];
        Version = list_[_INDEX_VERSION];
        Pool = list_[_INDEX_STD_POOL];
        User = list_[_INDEX_STD_USER];
        Host = list_[_INDEX_STD_HOST];
        IsvDef = list_[_INDEX_STD_ISV_DEF];
        //
        Count = int.Parse(list_[_INDEX_STD_COUNT], CultureInfo.InvariantCulture);
        CountCurrent = int.Parse(list_[_INDEX_STD_USE_CURRENT], CultureInfo.InvariantCulture);
        ResuseCurrent = int.Parse(list_[_INDEX_STD_RESUSE_CURRENT], CultureInfo.InvariantCulture);
        //
        HandleServer = list_[_INDEX_STD_SERVER_HANDLE];
        HandleShare = list_[_INDEX_STD_SHARE_HANDLE];
        //
        ProcessId = list_[_INDEX_STD_PROCESS_ID];
        Project = list_[_INDEX_STD_PROJECT];
        RequestedProduct = list_[_INDEX_STD_REQUEST_PRODUCT];
        RequestedVersion = list_[_INDEX_STD_REQUEST_VERSION];

        //
        EventDateTime = _GetDateTime(list_[_INDEX_STD_DATE], list_[_INDEX_STD_TIME]);
        LogFormat = (list_[17].Contains('.') == true) ? LogFormat.DETAILED : LogFormat.STANDARD;

        if (LogFormat == LogFormat.STANDARD)
        {
            _initDetail(list_);
        }
    }

    private void _initDetail(string[] list)
    {
        ClientMachineOsInfo = list[_INDEX_STD_CLIENT_MACHINE_OS_INFO];
        ApplicationArgv0 = list[_INDEX_STD_APPLICATION_ARGV0];
        RoamDays = list[_INDEX_STD_ROAM_DAYS];
        RoamHandle = list[_INDEX_STD_ROAM_HANDLE];
        ClientIpAddress = list[_INDEX_STD_CLIENT_IP_ADDRESS];
    }

    //checkout
    //OUT product version user  host “isv_def” count      server_handle share_handle hh:mm
    //OUT product version pool# user  host      “isv_def” count         cur_use      cur_resuse server_handle share_handle process_id “project” “requested product” “requested version” mm/dd hh:mm:ss
    //OUT product version pool# user  host      “isv_def” count         cur_use      cur_resuse server_handle share_handle process_id “project” “requested product” “requested version” mm/dd hh:mm:ss.tenths_of_msec “client_machine_os_info” “application argv0” roam_days roam_handle client-ip-address
    //0   1       2       3     4     5          6          7             8            9          10            11           12          13          14                    15                   16    17                       18                         19                   20        21          22
    private const int _INDEX_PRODUCT = 1;
    private const int _INDEX_VERSION = 2;
    //
    private const int _INDEX_SML_USER = 3;
    private const int _INDEX_SML_HOST = 4;
    private const int _INDEX_SML_ISV_DEF = 5;
    private const int _INDEX_SML_COUNT = 6;
    private const int _INDEX_SML_SERVER_HANDLE = 7;
    private const int _INDEX_SML_SHARE_HANDLE = 8;
    private const int _INDEX_SML_TIME = 9;
    //
    private const int _INDEX_STD_POOL = 3;
    private const int _INDEX_STD_USER = 4;
    private const int _INDEX_STD_HOST = 5;
    private const int _INDEX_STD_ISV_DEF = 6;
    private const int _INDEX_STD_COUNT = 7;
    private const int _INDEX_STD_USE_CURRENT = 8;
    private const int _INDEX_STD_RESUSE_CURRENT = 9;
    private const int _INDEX_STD_SERVER_HANDLE = 10;
    private const int _INDEX_STD_SHARE_HANDLE = 11;
    private const int _INDEX_STD_PROCESS_ID = 12;
    private const int _INDEX_STD_PROJECT = 13;

    private const int _INDEX_STD_REQUEST_PRODUCT = 14;
    private const int _INDEX_STD_REQUEST_VERSION = 15;

    private const int _INDEX_STD_DATE = 16;
    private const int _INDEX_STD_TIME = 17;
    //
    private const int _INDEX_STD_CLIENT_MACHINE_OS_INFO = 18;
    private const int _INDEX_STD_APPLICATION_ARGV0 = 19;
    private const int _INDEX_STD_ROAM_DAYS = 20;
    private const int _INDEX_STD_ROAM_HANDLE = 21;
    private const int _INDEX_STD_CLIENT_IP_ADDRESS = 22;
    //
    [Column("Product", Order = 11)]
    public string Product { get; private set; } = string.Empty;

    [Column("Version", Order = 12)]
    public string Version { get; private set; } = string.Empty;

    [Column("Product Version", Order = 13)]
    public string ProductVersion { get => Product + " " + Version; }
    //
    [Column("User", Order = 21)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 22)]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order = 23)]
    public string UserHost { get => User + "@" + Host; }
    //
    [Column("Isv Def", Order = 101)]
    public string IsvDef { get; private set; } = string.Empty;

    [Column("Pool", Order = 102)]
    public string Pool { get; private set; } = string.Empty;
    //
    [Column("Count", Order = 103)]
    public int Count { get; private set; } = -1;

    [Column("Current Count", Order = 104)]
    public int CountCurrent { get; private set; } = -1;

    [Column("Current Resuse", Order = 105)]
    public int ResuseCurrent { get; private set; } = -1;
    //
    [Column("Server Handle", Order = 106)]
    public string HandleServer { get; private set; } = string.Empty;

    [Column("Share Handle", Order = 107)]
    public string HandleShare { get; private set; } = string.Empty;

    [Column("Process ID", Order = 108)]
    public string ProcessId { get; private set; } = string.Empty;
    //
    [Column("Project", Order = 109)]
    public string Project { get; private set; } = string.Empty;

    [Column("Requested Product", Order = 110)]
    public string RequestedProduct { get; private set; } = string.Empty;

    [Column("Requested Version", Order = 111)]
    public string RequestedVersion { get; private set; } = string.Empty;
    //
    [Column("Client Machine OS Info", Order = 201)]
    public string ClientMachineOsInfo { get; private set; } = string.Empty;

    [Column("Application Argv0", Order = 202)]
    public string ApplicationArgv0 { get; private set; } = string.Empty;

    [Column("Roam Days", Order = 203)]
    public string RoamDays { get; private set; } = string.Empty;

    [Column("Roam Handle", Order = 204)]
    public string RoamHandle { get; private set; } = string.Empty;

    [Column("Client IP Address", Order = 205)]
    public string ClientIpAddress { get; private set; } = string.Empty;
    //
    /// <summary>
    /// チェックアウト に対応するチェックイン情報か
    /// </summary>
    /// <param name="checkIn_"></param>
    /// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public bool IsFindCheckIn(LogEventCheckIn checkIn_) => IsFindCheckIn(checkIn_.HandleServer, checkIn_.EventNumber);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public bool IsFindCheckIn(string hande_, long number_) => (hande_ == HandleServer) && (number_ > EventNumber);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public bool IsFindCheckIn(int hande_, long number_) => (hande_ == HandleServerNum) && (number_ > EventNumber);

    //public bool SetCount(IDictionary<string, int> listCount_, IDictionary<string, int> listHave_, IDictionary<string, int> listOutIn_)
    public bool SetCount(IDictionary<string, AnalysisLicenseCount.LicenseCount> listCount_)
    {
        if (string.IsNullOrEmpty(Product) == true)
        {
            return false;
        }
        //if (listOutIn_[Product] == CountCurrent)
        if (listCount_[Product].CheckOutInCurrent == CountCurrent)
        {
            // 重複チェック
            return false;
        }

        //listCount_[Product]++;
        //listOutIn_[Product] = CountCurrent;
        listCount_[Product].Count++;
        listCount_[Product].CheckOutInCurrent = CountCurrent;

        return true;
    }

}
