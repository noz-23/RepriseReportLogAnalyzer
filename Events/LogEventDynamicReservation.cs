using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Interfaces;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class LogEventRegist
    {
        private bool _logEventDynamicReservation = Regist("DYNRES", (l_) => new LogEventDynamicReservation(l_));
    }

    internal class LogEventDynamicReservation : LogEventBase, ILogEventUserHost
    {


        //dynamic reservation
        //DYNRES [create | remove] user host license-pool count “string” mm/dd hh:mm:ss
        //0      1                 2    3    4            5      6         7     8
        //
        [ColumnSort(11)]
        public string User { get; private set; } = string.Empty;
        [ColumnSort(12)]
        public string Host { get; private set; } = string.Empty;
        [ColumnSort(13)]
        public string UserHost { get => User + "@" + Host; }
        [ColumnSort(101)]
        public ReservationType Reservation { get; private set; } = ReservationType.CREATE;
        [ColumnSort(102)]
        public int LicensePool { get; private set; } =-1;
        [ColumnSort(103)]
        public int Count { get; private set; } = -1;
        [ColumnSort(104)]
        public String StringData{ get; private set; } = string.Empty;
        //
        public LogEventDynamicReservation(string[] list_):base()
        {
            Reservation = (list_[1] =="create") ? ReservationType.CREATE:ReservationType.REMOVE;
            User = list_[2];
            Host = list_[3];
            //
            LicensePool = int.Parse(list_[4]);
            Count = int.Parse(list_[5]);
            //
            StringData = list_[6];
            EventDateTime = _GetDateTime(list_[7], list_[8]);
        }
    }
}
