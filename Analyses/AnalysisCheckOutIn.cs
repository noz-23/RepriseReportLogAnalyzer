using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Events;
using RepriseReportLogAnalyzer.Files;
using System;
using System.Runtime.CompilerServices;

namespace RepriseReportLogAnalyzer.Analyses
{
    class AnalysisCheckOutIn
    {
        //public AnalysisCheckOutIn(AnalysisStartShutdown startShutdown_, LogEventCheckOut checkOut_, LogEventCheckIn? checkIn_)

        public AnalysisCheckOutIn(LogEventCheckOut checkOut_, LogEventBase checkIn_)
        {
            _checkOut = checkOut_;
            _checkIn = checkIn_;

            //if (checkIn_ is LogEventCheckIn checkIn)
            //{
            //    _joinCheckOutIn = new JoinEventCheckOutIn(checkOut_, checkIn);
            //}
            //else if (checkIn_ is LogEventShutdown shutdown)
            //{
            //    _joinCheckOutIn = new JoinEventCheckOutIn(checkOut_, shutdown);
            //}
            //else
            //{
            //    LogFile.Instance.WriteLine($"{checkIn_.EventNumber} {checkIn_.GetType()}");
            //}
            _joinCheckOutIn = new JoinEventCheckOutIn(checkOut_, checkIn_);
        }

        //public AnalysisCheckOutIn(LogEventCheckOut checkOut_, LogEventCheckIn checkIn_)
        //{
        //    _checkOut = checkOut_;
        //    _checkIn = checkIn_;

        //    _joinCheckOutIn = new JoinEventCheckOutIn(checkOut_, checkIn_);
        //}

        //public AnalysisCheckOutIn(LogEventCheckOut checkOut_, LogEventShutdown checkIn_)
        //{
        //    _checkOut = checkOut_;
        //    _checkIn = checkIn_;

        //    _joinCheckOutIn = new JoinEventCheckOutIn(checkOut_, checkIn_);
        //}

        //[Sort(101)]
        //public long StartNumber { get => _startShutdown.StartNumber; }
        [ColumnSort(102)]
        public DateTime CheckOutDateTime { get => _checkOut.EventDateTime; }

        [ColumnSort(103)]
        public DateTime CheckInDateTime { get => _checkIn.EventDateTime; }

        [ColumnSort(104)]
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
        private readonly LogEventBase _checkIn;
        private readonly JoinEventCheckOutIn _joinCheckOutIn;


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

        public LogEventCheckOut CheckOut() => _checkOut;
        public LogEventBase CheckIn() => _checkIn;
        public JoinEventCheckOutIn JoinEvent() => _joinCheckOutIn;

        public long CheckOutNumber()  => _checkOut.EventNumber;
        public long CheckInNumber() => _checkIn.EventNumber;

        public TimeSpan DurationDuplication() 
        {
            return _joinCheckOutIn.CheckIn().EventDateTime - CheckOutDateTime;
        }

        public static string HEADER { get => "CheckOut Date Time,CheckIn Date Time,Duration,Product,Version,Product Version,User,Host,User@Host"; }


        public string ToString( bool duplication_)
        {
            if (duplication_ == true)
            {
                return $"{CheckOutDateTime.ToString()},{_joinCheckOutIn.CheckIn().EventDateTime.ToString()},{DurationDuplication().ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
            }

            return ToString();
        }

        public override string ToString()
        {
            return $"{CheckOutDateTime.ToString()},{CheckInDateTime.ToString()},{Duration.ToString(@"d\.hh\:mm\:ss")},{Product},{Version},{ProductVersion},{User},{Host},{UserHost}";
        }


    }
}
