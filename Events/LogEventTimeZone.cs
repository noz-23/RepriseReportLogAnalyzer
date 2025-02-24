using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventTimeZone = LogEventBase.Regist("TIMEZONE", (l_) => new LogEventTimeZone(l_));
    }

    internal class LogEventTimeZone : LogEventBase
    {
        //TIMEZONE minutes-west-of-UTC daylight rules # readable version of data
        //0        1                   2        3     
        [ColumnSort(101)]
        public string MinutesWestOfUTC { get; private set; } = string.Empty;
        [ColumnSort(102)]
        public string DayLight  { get; private set; } = string.Empty;
        [ColumnSort(103)]
        public string Rules  { get; private set; } = string.Empty;

        public LogEventTimeZone(string[] list_):base()
        {
            MinutesWestOfUTC =list_[1];
            DayLight  =list_[2];
            Rules  =list_[3];
            //
            EventDateTime =NowDateTime;
        }
    }
}
