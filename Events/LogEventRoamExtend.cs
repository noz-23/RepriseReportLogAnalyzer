using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventRoamExtend = Regist("ROAM_EXTEND", (l_) => new LogEventRoamExtend(l_));
}

/// <summary>
/// roam extend
/// </summary>
[Sort(73)]
internal sealed class LogEventRoamExtend : LogEventBase, ILogEventUserHost, ILogEventProduct
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventRoamExtend(string[] list_) : base()
    {
        Product = list_[1];
        Version = list_[2];
        Pool = list_[3];
        User = list_[4];
        Host = list_[5];
        IsvDef = list_[6];
        //
        DaysExtended = list_[7];
        HandleServer = list_[8];
        ProcessId = list_[9];
        //
        EventDateTime = _GetDateTime(list_[10], list_[11]);
    }

    //roam extend
    //ROAM_EXTEND product version pool# user host “isv_def” #days_extended server_handle process_id mm/dd hh:mm:ss
    //0           1       2       3     4    5     6          7              8             9          10    11
    [Sort(11)]
    public string Product { get; private set; } = string.Empty;
    [Sort(12)]
    public string Version { get; private set; } = string.Empty;
    [Sort(13)]
    public string ProductVersion { get => Product + " " + Version; }
    [Sort(21)]
    public string User { get; private set; } = string.Empty;
    [Sort(22)]
    public string Host { get; private set; } = string.Empty;
    [Sort(23)]
    public string UserHost { get => User + "@" + Host; }
    //
    [Sort(101)]
    public string Pool { get; private set; } = string.Empty;
    //
    [Sort(102)]
    public string IsvDef { get; private set; } = string.Empty;
    [Sort(103)]
    public string DaysExtended { get; private set; } = string.Empty;
    [Sort(104)]
    public string HandleServer { get; private set; } = string.Empty;
    [Sort(105)]
    public string ProcessId { get; private set; } = string.Empty;
    //
}
