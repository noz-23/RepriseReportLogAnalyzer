using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventAuthentication = LogEventBase.Regist("AUTH", (l_) => new LogEventAuthentication(l_));
    }

    internal class LogEventAuthentication: LogEventBase
    {
        //AUTH section signature
        //0    1       2
        [ColumnSort(101)]
        public string Section { get; private set; } = string.Empty;
        [ColumnSort(102)]
        public string Signature { get; private set; } = string.Empty;

        public LogEventAuthentication(string[] list_):base()
        {
            Section = list_[1];
            Signature = list_[2];

            EventDateTime =NowDateTime;
        }

        new public static string HEADER { get => "Number,Date Time,Section,Signature"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()},{Section},{Signature}";
        }
    }
}
