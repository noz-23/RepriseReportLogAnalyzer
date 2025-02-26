using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventRlmReportLogFormat = Regist("RLM", (l_) => new LogEventRlmReportLogFormat(l_));
    }

    internal class LogEventRlmReportLogFormat : LogEventBase
    {
        //RLM Report Log Format d, version x.y authentication flag
        //0   1      2   3      4  5       6
        [ColumnSort(101)]
        public string Version  { get; private set; } = string.Empty;

        public LogEventRlmReportLogFormat(string[] list_):base()
        {
            Version = list_[6];

            EventDateTime = NowDateTime;
        }
    }
}
