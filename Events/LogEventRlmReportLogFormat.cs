using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventRlmReportLogFormat = Regist("RLM", (l_) => new LogEventRlmReportLogFormat(l_));
}

/// <summary>
/// log file start
/// </summary>
[Sort(82)]
internal sealed class LogEventRlmReportLogFormat : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventRlmReportLogFormat(string[] list_) : base()
    {
        Version = list_[6];

        EventDateTime = NowDateTime;
    }

    //RLM Report Log Format d, version x.y authentication flag
    //0   1      2   3      4  5       6
    [Sort(101)]
    public string Version { get; private set; } = string.Empty;
}
