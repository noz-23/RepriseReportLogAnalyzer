using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;
using RepriseReportLogAnalyzer.Enums;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventLicenseTemporary = LogEventBase.Regist("TEMP", (l_) => new LogEventLicenseTemporary(l_));
    }

    internal class LogEventLicenseTemporary: LogEventBase, ILogEventUserHost, ILogEventProduct
    {


        //Temporary license creation/removal
        //TEMP[create | remove | restart | expired] product version license-pool user host “isv_def” expdate exptime server_handle mm/dd hh:mm:ss
        //0    1                                    2       3       4            5    6     7          8       9       10            11    12
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
        public LicenseTemporaryType LicenseTemporary { get; private set; } = LicenseTemporaryType.CREATE;
        [ColumnSort(102)]
        public string LicensePool { get; private set; } = string.Empty;
        [ColumnSort(103)]
        public string IsvDef { get; private set; } = string.Empty;
        //
        [ColumnSort(104)]
        public string ExpiredDate { get; private set; } = string.Empty;
        [ColumnSort(105)]
        public string ExpiredTime { get; private set; } = string.Empty;
        [ColumnSort(106)]
        public string HandleServer { get; private set; } = string.Empty;
        //
        public LogEventLicenseTemporary(string[] list_):base()
        {
            switch(list_[1])
            {
                default:
                case "create": LicenseTemporary = LicenseTemporaryType.CREATE;break;
                case "remove": LicenseTemporary = LicenseTemporaryType.REMOVE;break;
                case "restart": LicenseTemporary = LicenseTemporaryType.RESTART;break;
                case "expired": LicenseTemporary = LicenseTemporaryType.EXPIRED;break;
            }

            Product = list_[2];
            Version = list_[3];
            LicensePool= list_[4];
            User = list_[5];
            Host = list_[6];
            IsvDef = list_[7];
            //
            ExpiredDate = list_[8];
            ExpiredTime = list_[9];
            HandleServer = list_[10];

            EventDateTime = _GetDateTime(list_[11], list_[12]);
        }
    }
}
