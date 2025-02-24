using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventRoamExtend = LogEventBase.Regist("ROAM_EXTEND", (l_) => new LogEventRoamExtend(l_));
    }

    internal class LogEventRoamExtend : LogEventBase, ILogEventUserHost, ILogEventProduct
    {
        //roam extend
        //ROAM_EXTEND product version pool# user host “isv_def” #days_extended server_handle process_id mm/dd hh:mm:ss
        //0           1       2       3     4    5     6          7              8             9          10    11
        [ColumnSort(11)]
        public string Product { get; private set; } = string.Empty;
        [ColumnSort(12)]
        public string Version { get; private set; } = string.Empty;
        [ColumnSort(13)]
        public string ProductVersion { get => Product + " " + Version; }
        [ColumnSort(21)]
        public string User { get; private set; } = string.Empty;
        [ColumnSort(22)]
        public string Host { get; private set; } = string.Empty;
        [ColumnSort(23)]
        public string UserHost { get => User + "@" + Host; }
        //
        [ColumnSort(101)]
        public string Pool { get; private set; } = string.Empty;
        //
        [ColumnSort(102)]
        public string IsvDef { get; private set; } = string.Empty;
        [ColumnSort(103)]
        public string DaysExtended { get; private set; } = string.Empty;
        [ColumnSort(104)]
        public string HandleServer { get; private set; } =string.Empty;
        [ColumnSort(105)]
        public string ProcessId { get; private set; } = string.Empty;
        //
        public LogEventRoamExtend(string[] list_):base()
        {
            Product = list_[1];
            Version = list_[2];
            Pool = list_[3];
            User = list_[4];
            Host = list_[5];
            IsvDef = list_[6];
            //
            DaysExtended = list_[7];
            HandleServer = list_[8];
            ProcessId = list_[9];
            //
            EventDateTime = _GetDateTime(list_[10], list_[11]);
        }

        new public static string HEADER { get => "Number,Date Time,Product,Version,Product Version,User,Host,User@Host,Pool,IsvDef,DaysExtended,HandleServer,ProcessId"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()},{Product},{Version},{ProductVersion},{User},{Host},{UserHost},{Pool},{IsvDef},{DaysExtended},{HandleServer},{ProcessId}";
        }
    }
}
