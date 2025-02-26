namespace RepriseReportLogAnalyzer.Events
{
    internal class LogEventTimeStamp: LogEventBase
    {
        //periodic timestamp
        //mm/dd/yyyy hh:mm
        //0          1
        public LogEventTimeStamp(string[] list_):base()
        {
            EventDateTime = DateTime.Parse(list_[0]+" "+ list_[1]);
        }
    }
}
