using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events;

/// <summary>
/// periodic timestamp
/// </summary>
[Sort(98)]
internal sealed class LogEventTimeStamp : LogEventBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list_">スペースで分割した文字列リスト</param>
    public LogEventTimeStamp(string[] list_) : base()
    {
        EventDateTime = DateTime.Parse(list_[0] + " " + list_[1]);
    }

    //periodic timestamp
    //mm/dd/yyyy hh:mm
    //0          1
}
