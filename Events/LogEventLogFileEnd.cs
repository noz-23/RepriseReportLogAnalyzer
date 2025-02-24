using System;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventLogFileEnd = LogEventBase.Regist("END", (l_) => new LogEventLogFileEnd(l_));
    }

    internal class LogEventLogFileEnd: LogEventBase
    {
        //END mm/dd/yyyy hh:mm
        //0   1          2

        public LogEventLogFileEnd(string[] list_):base()
        {
            EventDateTime = DateTime.Parse(list_[1] + " " + list_[2]);
        }
    }
}
