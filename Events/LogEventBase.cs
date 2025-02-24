using RepriseReportLogAnalyzer.Attributes;
using RepriseReportLogAnalyzer.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RepriseReportLogAnalyzer.Events
{
    internal partial class EventRegist
    {
        public static EventRegist Instance = new EventRegist();
        private EventRegist()
        {
        }

        public void Create()
        {
            LogFile.Instance.WriteLine($"{LogEventBase.ListEventDataCount()}");
        }
    }

    internal partial class LogEventBase
    {
        public delegate LogEventBase NewEvent(string [] list);
        protected static Dictionary<string, NewEvent> _ListEventData =new Dictionary<string, NewEvent>();

        public static bool Regist(string key_, NewEvent event_)
        {
            _ListEventData.Add(key_, event_);
            return true;
        }
        public static int ListEventDataCount() => _ListEventData.Count;

        public static DateTime NowDateTime =DateTime.Now;
        public static long NowEventNumber = 0;

        protected static long _NowYear { get => NowDateTime.Year; }
        protected static long _NowMonth { get => NowDateTime.Month; }
        protected static string _NowDate { get => NowDateTime.ToString("MM/dd/yyyy"); }
        //
        [ColumnSort(1)]
        public long EventNumber { get; protected set; } = 0;
        //
        [ColumnSort(2)]
        public DateTime EventDateTime
        {
            get => _eventDateTime;
            set
            {
                _eventDateTime = value;
                NowDateTime = value;
            }
        }
        private DateTime _eventDateTime;

        public DateTime EventDate()=> EventDateTimeUnit(TimeSpan.TicksPerDay);

        public DateTime EventDateTimeUnit(long timeSpan_)
        {
            return new DateTime(EventDateTime.Ticks - (EventDateTime.Ticks % timeSpan_));
        }

        public static LogEventBase? EventData(string str_)
        {
            var list = _splitSpace(str_);

            //switch (list[0])
            //{
            //    //case "START": return new LogEventStart(list);
            //    //case "AUTH:": return new LogEventAuthentication(list);
            //    //case "IN": return new LogEventCheckIn(list);
            //    //case "OUT": return new LogEventCheckOut(list);
            //    //case "DEQUE": return new LogEventDequeue(list);
            //    //case "DYNRES": return new LogEventDynamicReservation(list);
            //    //case "log": return new LogEventAuthentication(list);
            //    //case "END": return new LogEventLogFileEnd(list);
            //    //case "METER_DEC": return new LogEventMeterDecrement(list);
            //    //case "QUE": return new LogEventQueue(list);
            //    //case "REPROCESSED": return new LogEventRepProcessed(list);
            //    //case "RLM": return new LogEventRlmReportLogFormat(list);
            //    //case "ROAM_EXTEND": return new LogEventRoamExtend(list);
            //    //case "SHUTDOWN": return new LogEventShutdown(list);
            //    //case "SWITCH": return new LogEventSwitch(list);
            //    //case "TIMEJUMP": return new LogEventTimeJump(list);
            //    //case "TIMEZONE": return new LogEventTimeZone(list);
            //    //case "DENY": return new LogEventLicenseDenial(list);
            //    //case "LICENSE": return new LogEventLicenseFile(list);
            //    //case "INUSE": return new LogEventLicenseInUse(list);
            //    //case "REREAD": return new LogEventLicenseReread(list);
            //    //case "TEMP": return new LogEventLicenseTemporary(list);
            //    //case "PRODUCT": return new LogEventProduct(list);

            //    default:
            //        if (list.Count() == 2 && list[0].Contains("/") == true && list[1].Contains(":") == true)
            //        {
            //            return new LogEventTimeStamp(list);
            //        }
            //        break;
            //}

            NewEvent? newEvent =null;
            if (_ListEventData.TryGetValue(list[0], out newEvent) == false)
            {
                if (list.Count() == 2 && list[0].Contains("/") == true && list[1].Contains(":") == true)
                {
                    return new LogEventTimeStamp(list);
                }
                return null;
            }

            return newEvent(list);
        }

        public LogEventBase()
        {
            //
            EventNumber = ++NowEventNumber;
        }

        public static void Clear()
        {
            EventRegist.Instance.Create();

            NowEventNumber = 0;
            NowDateTime = DateTime.Now;

        }

        // ｢a "b c" d｣→｢a,"b c",d｣
        private static string[] _splitSpace(string str_)
        {
            var list = str_.Split(' ');// ｢a "b c" d｣→｢a,"b,c",d｣
            var rtn = new List<string>();

            string tmp = string.Empty;
            foreach (var s in list)
            {
                if (string.IsNullOrEmpty(tmp) == true)
                {
                    if (s.Contains("\"") == true)
                    {
                        if (s.IndexOf("\"") == s.LastIndexOf("\""))
                        {
                            tmp += s;
                            continue;
                        }
                    }
                }
                else
                {
                    if (s.Contains("\"") == true)
                    {
                        tmp += s;
                        rtn.Add(tmp);
                        tmp = string.Empty;
                    }
                    else
                    {
                        tmp += s;
                    }
                    continue;
                }
                rtn.Add(s);
            }
            // ｢a,"b,c",d｣→｢a,"b c",d｣
            return rtn.ToArray();
        }

        protected static DateTime _GetDateTime(string date_, string time_)
        {
            var listDate = date_.Split('/');
            var month = int.Parse(listDate[0]);
            // New Year
            var year = (month < _NowMonth) ? (_NowYear + 1) : (_NowYear);
            return DateTime.Parse(date_ + "/" + year + " " + time_);
        }

        public static string HEADER { get => "Number,Date Time"; }
        public override string ToString()
        {
            return $"{EventNumber},{EventDateTime.ToString()}";
        }
    }
}
