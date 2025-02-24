using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        private bool _logEventMeterDecrement = LogEventBase.Regist("METER_DEC", (l_) => new LogEventMeterDecrement(l_));
    }

    internal class LogEventMeterDecrement: LogEventBase
    {
        //meter decrement
        //METER_DEC license_handle meter_counter amount_decremented mm/dd hh:mm:ss[.tenths_of_msec]
        //0         1              2             3                  4     5
        [ColumnSort(101)]
        public string HandleLicense { get; private set; } = string.Empty;
        [ColumnSort(102)]
        public string CounterMeter { get; private set; } = string.Empty;
        [ColumnSort(103)]
        public string AmountDecremented { get; private set; } = string.Empty;

        public LogEventMeterDecrement(string[] list_):base()
        {
            HandleLicense = list_[1];
            CounterMeter  = list_[2];
            AmountDecremented = list_[3];
            //
            EventDateTime = _GetDateTime(list_[4], list_[5]);
        }

        new public static string HEADER { get => "Number,Date Time,HandleLicense,CounterMeter,AmountDecremented"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()},{HandleLicense},{CounterMeter},{AmountDecremented}";
        }
    }
}
