using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventLicenseReread = Regist("REREAD", (l_) => new LogEventLicenseReread(l_));
    }

    internal class LogEventLicenseReread: LogEventBase, ILogEventUserHost
    {
        //server reread of license/option file
        //REREAD user host mm/dd hh:mm:ss
        //0      1    2    3     4
        //
        [ColumnSort(21)]
        public string User { get; private set; } = string.Empty;
        [ColumnSort(22)]
        public string Host { get; private set; } = string.Empty;
        [ColumnSort(23)]
        public string UserHost { get => User + "@" + Host; }
        //
        public LogEventLicenseReread(string[] list_):base()
        {
            User = list_[1];
            Host = list_[2];

            EventDateTime = _GetDateTime(list_[3], list_[4]);
        }
    }
}
