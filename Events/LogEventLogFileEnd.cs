namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// 文字列 と イベントの紐づけ登録
/// </summary>
internal sealed partial class LogEventRegist
{
    private bool _logEventLogFileEnd = Regist("END", (l_) => new LogEventLogFileEnd(l_));
}

/// <summary>
/// log file end
/// </summary>
internal sealed class LogEventLogFileEnd : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventLogFileEnd(string[] list_) : base()
    {
        EventDateTime = DateTime.Parse(list_[1] + " " + list_[2]);
    }

    //END mm/dd/yyyy hh:mm
    //0   1          2
}
