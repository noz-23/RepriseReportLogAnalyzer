using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventDequeue = Regist("DEQUE", (l_) => new LogEventDequeue(l_));
    }

    internal class LogEventDequeue : LogEventBase, ILogEventUserHost, ILogEventProduct
    {
        //dequeue
        //DEQUE why product version       user  host “isv_def” count server_handle mm/dd hh:mm:ss
        //DEQUE why product version       user  host “isv_def” count server_handle mm/dd hh:mm:ss.tenths_of_msec
        //DEQUE why count   server_handle hh:mm
        //0     1   2       3             4     5     6          7     8             9     10
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

        [ColumnSort(101)]
        public string Why { get; private set; } = string.Empty;
        [ColumnSort(102)]
        public string IsvDef { get; private set; } = string.Empty;
        [ColumnSort(103)]
        public int Count { get; private set; } = -1;
        [ColumnSort(104)]
        public string HandleServer { get; private set; } =string.Empty;
        //
        public LogEventDequeue(string[] list_):base()
        {
            if (list_.Count() < 4)
            {
                Why = list_[1];
                Count = int.Parse(list_[2]);
                HandleServer = list_[3];
                //
                EventDateTime = DateTime.Parse(_NowDate + " " + list_[4]);
            }
            else
            {
                Why = list_[1];
                Product = list_[2];
                Version = list_[3];
                User = list_[4];
                Host = list_[5];
                IsvDef = list_[6];
                //
                Count = int.Parse(list_[7]);
                //
                HandleServer = list_[8];
                //
                EventDateTime = _GetDateTime(list_[9], list_[10]);
            }
        }
    }
}
