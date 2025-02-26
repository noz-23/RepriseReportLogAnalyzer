using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventQueue = Regist("QUE", (l_) => new LogEventQueue(l_));
    }

    internal class LogEventQueue: LogEventBase, ILogEventUserHost, ILogEventProduct
    {
        //queue
        //QUE product version user host “isv_def” count server_handle “project” “requested product” “requested version” mm/dd hh:mm:ss
        //QUE product version user host “isv_def” count server_handle “project” “requested product” “requested version” mm/dd hh:mm:ss.tenths_of_msec
        //QUE product version user host “isv_def” count server_handle hh:mm
        //0   1       2       3    4     5          6     7             8            9                     10                   11    12
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
        [ColumnSort(101)]
        public string IsvDef { get; private set; } = string.Empty;
        [ColumnSort(102)]
        public int Count { get; private set; } = -1;
        [ColumnSort(103)]
        public string HandleServer { get; private set; } = string.Empty;
        [ColumnSort(104)]
        public string Project { get; private set; } = string.Empty;
        [ColumnSort(105)]
        public string RequestedProduct { get; private set; } = string.Empty;
        [ColumnSort(106)]
        public string RequestedVersion { get; private set; } = string.Empty;
        //
        public LogEventQueue(string[] list_):base()
        {
            if (list_.Count() < 12)
            {
                Product = list_[1];
                Version = list_[2];
                User = list_[3];
                Host = list_[4];
                IsvDef = list_[5];
                Count = int.Parse(list_[6]);
                //
                HandleServer = list_[7];
                //
                EventDateTime = DateTime.Parse(_NowDate + " " + list_[8]);
            }else
            {
                Product = list_[1];
                Version = list_[2];
                User = list_[3];
                Host = list_[4];
                IsvDef = list_[5];
                //
                Count = int.Parse(list_[6]);
                //
                HandleServer = list_[7];
                //
                Project = list_[8];
                RequestedProduct = list_[9];
                RequestedVersion = list_[10];

                //
                EventDateTime = _GetDateTime(list_[11], list_[12]);
            }
        }
    }
}
