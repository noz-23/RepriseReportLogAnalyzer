using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventSwitch = LogEventBase.Regist("SWITCH", (l_) => new LogEventSwitch(l_));
    }

    internal class LogEventSwitch : LogEventBase
    {
        //log file end
        //SWITCH to filename(if an rlmswitch was done)
        //0      1  2

        //SWITCH from old-reportlog-name (new in v14.0, not authenticated)
        //0      1    2                  4
        [ColumnSort(101)]
        public SwitchType Switch { get; private set; } = SwitchType.FROM;

        [ColumnSort(102)]
        public string OldReportLogName { get; private set; } = string.Empty;

        public LogEventSwitch(string[] list_):base()
        {
            Switch =(list_[1] =="to")? SwitchType.TO:SwitchType.FROM;
            OldReportLogName =list_[2];
            EventDateTime =NowDateTime;
        }

        new public static string HEADER { get => "Number,Date Time,Switch,OldReportLogName"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()},{Switch},{OldReportLogName}";
        }
    }
}
