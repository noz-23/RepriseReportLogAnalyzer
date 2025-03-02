using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Enums;
using RepriseReportLogAnalyzer.Events;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RepriseReportLogAnalyzer.Analyses
{
    class AnalysisCheckOutIn
    {
        public AnalysisCheckOutIn(LogEventCheckOut checkOut_, LogEventBase? checkIn_)
        {
            _checkOut = checkOut_;
            _checkIn = checkIn_;

            _joinEvent = new JoinEventCheckOutIn(checkOut_, checkIn_);
        }

        public const string HEADER = "CheckOut Date Time,CheckIn Date Time,Duration,Product,Version,Product Version,User,Host,User@Host";

        [ColumnSort(101)]
        public DateTime CheckOutDateTime { get => _checkOut.EventDateTime; }

        [ColumnSort(102)]
        public DateTime CheckInDateTime { get => _checkIn?.EventDateTime?? LogEventBase.NowDateTime; }

        [ColumnSort(103)]
        public TimeSpan Duration { get => CheckInDateTime - CheckOutDateTime; }

        //
        [ColumnSort(111)]
        public string Product { get => _checkOut.Product; }
        [ColumnSort(112)]
        public string Version { get => _checkOut.Version; }
        [ColumnSort(113)]
        public string ProductVersion { get => _checkOut.ProductVersion; }
        //
        [ColumnSort(121)]
        public string User { get => _checkOut.User; }
        [ColumnSort(121)]
        public string Host { get => _checkOut.Host; }
        [ColumnSort(121)]
        public string UserHost { get => _checkOut.UserHost; }

        private readonly LogEventCheckOut _checkOut;
        private readonly LogEventBase? _checkIn=null;
        private readonly JoinEventCheckOutIn _joinEvent;

        public LogEventCheckOut CheckOut() => _checkOut;
        public LogEventBase? CheckIn() => _checkIn;
        public JoinEventCheckOutIn JoinEvent() => _joinEvent;

        public long CheckOutNumber() => _checkOut.EventNumber;
        public long CheckInNumber() => _checkIn?.EventNumber ?? LogEventBase.NowEventNumber;

        public DateTime JointDateTime() => _joinEvent.CheckIn()?.EventDateTime ?? LogEventBase.NowDateTime;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSame(LogEventCheckIn checkIn_)
        {
            return _checkIn == checkIn_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWithInRange(long number_)
        {
            return number_ > CheckOutNumber() && number_ < CheckInNumber();
        }


        public TimeSpan DurationDuplication() 
        {
            return JointDateTime() - CheckOutDateTime;
        }

        public string GroupName(ANALYSIS_GROUP group_)
        {
            switch (group_)
            {
                case ANALYSIS_GROUP.USER:return User;
                case ANALYSIS_GROUP.HOST:return Host;
                case ANALYSIS_GROUP.USER_HOST:return UserHost;
                default:
                    break;
            }return string.Empty;
        }
        public string ToString( bool duplication_)
        {
            if (duplication_ == true)
            {
                return $"{CheckOutDateTime.ToString()},{JointDateTime().ToString()},{DurationDuplication().ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
            }

            return ToString();
        }

        public override string ToString()
        {
            return $"{CheckOutDateTime.ToString()},{CheckInDateTime.ToString()},{Duration.ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
        }


    }
}
