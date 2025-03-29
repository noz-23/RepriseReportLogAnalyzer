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
        if (list_.Length < 4)
        {
            // small
            //Why = int.Parse(list_[1]);
            Why = (StatusValue)int.Parse(list_[1]);
            Count = int.Parse(list_[2]);
            HandleServer = list_[3];
            //HandleServerNum = Convert.ToInt32(HandleServer, 16);

            //
            EventDateTime = DateTime.Parse(_NowDate + " " + list_[4]);
            LogFormat = LogFormat.SMALL;

        }
        else
        {
            // std
            // detailed
            //Why = int.Parse(list_[1]);
            Why = (StatusValue)int.Parse(list_[1]);
            Product = list_[2];
            Version = list_[3];
            User = list_[4];
            Host = list_[5];
            IsvDef = list_[6];
            //
            Count = int.Parse(list_[7]);
            CountCurrent = int.Parse(list_[8]);
            ResuseCurrent = int.Parse(list_[9]);
            //
            HandleServer = list_[10];
            //
            EventDateTime = _GetDateTime(list_[11], list_[12]);

            LogFormat = (list_[12].Contains('.') == true) ? LogFormat.DETAILED : LogFormat.STANDARD;
        }

        _checkOut = null;
    }


    //public const LogEventType LogType = LogEventType.CheckIn;
    //check-in
    //IN why product version       user  host “isv_def” count cur_use cur_resuse server_handle mm/dd hh:mm:ss
    //IN why product version       user  host “isv_def” count cur_use cur_resuse server_handle mm/dd hh:mm:ss.tenths_of_msec
    //IN why count   server_handle hh:mm
    //0  1   2       3             4     5     6          7     8       9          10            11    12
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
        //var data = listCount_[product];

        //if (listOutIn_[product] == CountCurrent)
        if (listCount_[product].CheckOutInCurrent == CountCurrent)
        {
            // 重複チェック
            return false;
        }

        //listCount_[product]--;
        //listOutIn_[product] = CountCurrent;
        listCount_[product].Count--;
        listCount_[product].CheckOutInCurrent = CountCurrent;

        return true;
    }
}
