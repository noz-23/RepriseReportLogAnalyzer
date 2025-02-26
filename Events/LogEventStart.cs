using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventStart = Regist("START", (l_) => new LogEventStart(l_));
    }

    internal class LogEventStart : LogEventBase
    {

        //public const LogEventType LogType = LogEventType.Start;
        //START hostname mm/dd/yyyy hh:mm
        //0     1        2          3
        [ColumnSort(101)]
        public string HostName { get; private set; } = string.Empty;

        public LogEventStart(string[] list_):base()
        {
            //LogEventBase.ListEventData.Add("START", (l_) => new LogEventStart(l_));
            //var start= LogEventBase.ListEventData["START"] = (l_) => new LogEventStart(l_);

            HostName = list_[1];
            EventDateTime = DateTime.Parse(list_[2]+" "+ list_[3]);
        }
    }
}
