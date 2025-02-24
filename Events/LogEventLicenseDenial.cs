using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventLicenseDenial = LogEventBase.Regist("DENY", (l_) => new LogEventLicenseDenial(l_));
    }

    internal class LogEventLicenseDenial : LogEventBase, ILogEventUserHost, ILogEventProduct
    {
        //license denial
        //DENY product version user host “isv_def” count why last_attempt pid mm/dd hh:mm
        //DENY product version user host “isv_def” count why last_attempt pid mm/dd hh:mm:ss.tenths_of_msec
        //0    1       2       3    4     5          6     7   8            9   10    11
        [ColumnSort(11)]
        public string Product { get; private set; } = string.Empty;
        [ColumnSort(12)]
        public string Version { get; private set; } = string.Empty;
        [ColumnSort(13)]
        public string ProductVersion { get => Product + " " + Version; }
        //
        [ColumnSort(21)]
        public string User { get; private set; } = string.Empty;
        [ColumnSort(22)]
        public string Host { get; private set; } = string.Empty;
        [ColumnSort(23)]
        public string UserHost { get => User + "@" + Host; }
        //
        [ColumnSort(101)]
        public string IsvDef { get; private set; } = string.Empty;
        [ColumnSort(102)]
        public int Count { get; private set; } = -1;
        [ColumnSort(103)]
        public string Why { get; private set; } = string.Empty;
        [ColumnSort(104)]
        public string LastAttempt{ get; private set; } = string.Empty;
        [ColumnSort(105)]
        public string ProcessId{ get; private set; } = string.Empty;
        //
        public LogEventLicenseDenial (string[] list_):base()
        {
            Product = list_[1];
            Version = list_[2];
            User = list_[3];
            Host = list_[4];
            IsvDef = list_[5];
            //
            Count = int.Parse(list_[6]);
            Why = list_[7];
            LastAttempt = list_[8];
            ProcessId = list_[9];

            EventDateTime = _GetDateTime(list_[10], list_[11]);
        }

        new public static string HEADER { get => "Number,Date Time,Product,Version,Product Version,User,Host,UserHost,IsvDef,Count,Why,LastAttempt,ProcessId"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()},{Product},{Version},{ProductVersion},{User},{Host},{UserHost},{IsvDef},{Count},{Why},{LastAttempt},{ProcessId}";
        }
    }
}
