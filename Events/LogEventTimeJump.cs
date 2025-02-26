using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventTimeJump = Regist("TIMEJUMP", (l_) => new LogEventTimeJump(l_));
    }

    internal class LogEventTimeJump: LogEventBase
    {
        //server time jump
        //TIMEJUMP[+ | -]minutes mm/dd hh:mm:ss
        //0        1             2     3
        [ColumnSort(101)]
        public string Minutes { get; private set; } = string.Empty;

        public LogEventTimeJump(string[] list_):base()
        {
            Minutes = list_[1];
            EventDateTime = DateTime.Parse(list_[2]+" "+ list_[3]);
        }
    }
}
