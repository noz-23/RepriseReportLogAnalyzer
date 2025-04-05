/*
 * Reprise Report Log Analyzer
 * Copyright (c) 2025 noz-23
 *  https://github.com/noz-23/
 * 
 * Licensed under the MIT License 
 * 
 */
using RepriseReportLogAnalyzer.Analyses;
using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventCheckIn = Regist("IN", (l_) => new LogEventCheckIn(l_));
}

/// <summary>
/// check-in
/// </summary>
[Sort(12)]
[Table("TbCheckIn")]
internal sealed class LogEventCheckIn : LogEventBase, ILogEventUserHost, ILogEventWhy, ILicenseCount
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventCheckIn(string[] list_) : base()
    {
        //if (list_.Count() < 4)
        if (list_.Length <= _INDEX_SML_TIME)
        {
            _initSmall(list_);
        }else
        {
            _initStandardDetail(list_);
        }

        _checkOut = null;
    }
    private void _initSmall(string[] list_)
    {
        // small
        Why = (StatusValue)int.Parse(list_[_INDEX_WHY], CultureInfo.InvariantCulture);
        Count = int.Parse(list_[_INDEX_SML_COUNT], CultureInfo.InvariantCulture);
        HandleServer = list_[_INDEX_SML_SERVER_HANDLE];

        //
        //EventDateTime = DateTime.Parse(_NowDate + " " + list_[_INDEX_SML_TIME], CultureInfo.InvariantCulture);
        EventDateTime = _GetDateTime(list_[_INDEX_SML_TIME]);
        LogFormat = LogFormat.SMALL;
    }

    private void _initStandardDetail(string[] list_)
    {
        // std
        // detailed
        //Why = int.Parse(list_[1]);
        Why = (StatusValue)int.Parse(list_[_INDEX_WHY], CultureInfo.InvariantCulture);
        Product = list_[_INDEX_STD_PRODUCT];
        Version = list_[_INDEX_STD_VERSION];
        User = list_[_INDEX_STD_USER];
        Host = list_[_INDEX_STD_HOST];
        IsvDef = list_[_INDEX_STD_ISV_DEF];
        //
        Count = int.Parse(list_[_INDEX_STD_COUNT], CultureInfo.InvariantCulture);
        CountCurrent = int.Parse(list_[_INDEX_STD_COUNT_CURRENT], CultureInfo.InvariantCulture);
        ResuseCurrent = int.Parse(list_[_INDEX_STD_RESUSE_CURRENT], CultureInfo.InvariantCulture);
        //
        HandleServer = list_[_INDEX_STD_SERVER_HANDLE];
        //
        EventDateTime = _GetDateTime(list_[_INDEX_STD_DATE], list_[_INDEX_STD_TIME]);
        LogFormat = (list_[_INDEX_STD_TIME].Contains('.') == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
    }


    //public const LogEventType LogType = LogEventType.CheckIn;
    //check-in
    //IN why count   server_handle hh:mm
    //IN why product version       user  host “isv_def” count cur_use cur_resuse server_handle mm/dd hh:mm:ss
    //IN why product version       user  host “isv_def” count cur_use cur_resuse server_handle mm/dd hh:mm:ss.tenths_of_msec
    //0  1   2       3             4     5     6          7     8       9          10            11    12
    private const int _INDEX_WHY = 1;
    //
    private const int _INDEX_SML_COUNT = 2;
    private const int _INDEX_SML_SERVER_HANDLE = 3;
    private const int _INDEX_SML_TIME = 4;
    //
    private const int _INDEX_STD_PRODUCT = 2;
    private const int _INDEX_STD_VERSION = 3;
    private const int _INDEX_STD_USER = 4;
    private const int _INDEX_STD_HOST = 5;
    private const int _INDEX_STD_ISV_DEF = 6;
    private const int _INDEX_STD_COUNT = 7;
    private const int _INDEX_STD_COUNT_CURRENT = 8;
    private const int _INDEX_STD_RESUSE_CURRENT = 9;
    private const int _INDEX_STD_SERVER_HANDLE = 10;
    private const int _INDEX_STD_DATE = 11;
    private const int _INDEX_STD_TIME = 12;
    //

    [Column("Product", Order = 11)]
    public string Product { get; private set; } = string.Empty;

    [Column("Version", Order = 12)]
    public string Version { get; private set; } = string.Empty;

    [Column("Product Version", Order = 13)]
    public string ProductVersion { get => Product + " " + Version; }

    [Column("User", Order = 21)]
    public string User { get; private set; } = string.Empty;

    [Column("Host", Order = 22)]
    public string Host { get; private set; } = string.Empty;

    [Column("User@Host", Order = 23)]
    public string UserHost { get => User + "@" + Host; }
    //
    [Column("Why", Order = 101)]
    public StatusValue Why { get; private set; } = StatusValue.Success;
    //public int Why { get; private set; } = 0;

    [Column("Isv Def", Order = 102)]
    public string IsvDef { get; private set; } = string.Empty;
    //
    [Column("Count", Order = 103)]
    public int Count { get; private set; } = -1;

    [Column("Current Count", Order = 104)]
    public int CountCurrent { get; private set; } = -1;

    [Column("Current Resuse", Order = 105)]
    public int ResuseCurrent { get; private set; } = -1;

    [Column("Server Handle", Order = 106)]
    public string HandleServer { get; private set; } = string.Empty;
    //
    //public int HandleServerNum() => _handleServerNum;
    //private int _handleServerNum = 1;
    //public int HandleServerNum { get; private set; } = -1;

    private LogEventCheckOut? _checkOut;
    public void SetLogEventCheckOut(LogEventCheckOut checkOut_) => _checkOut = checkOut_;

    //public bool SetCount(IDictionary<string, int> listCount_, IDictionary<string, int> listHave_, IDictionary<string, int> listOutIn_)
    public bool SetCount(IDictionary<string, AnalysisLicenseCount.LicenseCount> listCount_)
    {
        string product = (string.IsNullOrEmpty(Product) == false) ? Product : (_checkOut != null) ? _checkOut.Product : string.Empty;

        if (string.IsNullOrEmpty(product) == true)
        {
            return false;
        }
        if (listCount_[product].CheckOutInCurrent == CountCurrent)
        {
            // 重複チェック
            return false;
        }

        //listCount_[product].Count--;
        //listCount_[product].CheckOutInCurrent = CountCurrent;
        var data = listCount_[product];
        data.Count--;
        data.CheckOutInCurrent = CountCurrent;
        listCount_[product] = data;


        return true;
    }
}
