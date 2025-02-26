namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventLogFileEnd = Regist("END", (l_) => new LogEventLogFileEnd(l_));
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
