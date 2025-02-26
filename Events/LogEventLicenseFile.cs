using RepriseReportLogAnalyzer.Attributes;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventLicenseFile = Regist("LICENSE", (l_) => new LogEventLicenseFile(l_));
    }

    internal class LogEventLicenseFile : LogEventBase
    {
        //LICENSE FILE filename
        //0       1    2
        [ColumnSort(101)]
        public string FileName{ get; private set; } = string.Empty;

        public LogEventLicenseFile (string[] list_):base()
        {
            FileName=list_[2];
            EventDateTime =NowDateTime;
        }
    }
}
