using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal class LogEventIsvSpecificData : LogEventBase
    {
        //isv-specific data
        //log mm/dd hh:mm:ss isv-specific-data-here
        //0   1     2        3
        [ColumnSort(101)]
        public string IsvSpecificData { get; private set; } = string.Empty;

        public LogEventIsvSpecificData(string[] list_):base()
        {
            EventDateTime = _GetDateTime(list_[1], list_[2]);
            IsvSpecificData = list_[3];
        }

        new public static string HEADER { get => "Number,Date Time,IsvSpecificData"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()},{IsvSpecificData}";
        }
    }
}
