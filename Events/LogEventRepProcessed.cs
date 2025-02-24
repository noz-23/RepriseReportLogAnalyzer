using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventRepProcessed = LogEventBase.Regist("REPROCESSED", (l_) => new LogEventRepProcessed(l_));
    }

    internal class LogEventRepProcessed : LogEventBase
    {
        //REPROCESSED with rlmanon vx.y
        //0           1    2       3
        [ColumnSort(101)]
        public string Version  { get; private set; } = string.Empty;
        
        public LogEventRepProcessed(string[] list_):base()
        {
            Version = list_[3];

            EventDateTime = NowDateTime;
        }

        new public static string HEADER { get => "Number,Date Time,Version"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()},{Version}";
        }
    }
}
