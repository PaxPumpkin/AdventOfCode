using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class DateTimeFun : AoCodeModule
    {
        public DateTimeFun()
        {
        }
        
        public override void DoProcess()
        {
            DateTime x = DateTime.Now;
            DateTime y = new DateTime(DateTime.UtcNow.Ticks);
            DateTimeOffset z = new DateTimeOffset(y, new TimeSpan(-5, 0, 0));
            TimeZoneInfo.GetSystemTimeZones().OrderBy(a => a.Id).ToList().ForEach(a => Console.WriteLine(a.Id + " ---   " + a.DisplayName));
            double aNumber=5.5;
            List<TimeZoneInfo> findit = TimeZoneInfo.GetSystemTimeZones().Where(a=>a.BaseUtcOffset.Hours==Math.Floor(aNumber) && a.BaseUtcOffset.Minutes==((aNumber%1)*60)).ToList();
           var p = Convert.ToDateTime("1/12/2018");

            foreach (TimeZoneInfo tz in TimeZoneInfo.GetSystemTimeZones())
            {
                Console.WriteLine(tz.Id);
            }
            /*
            int q = 0;
            tz.BaseUtcOffset
{-12:00:00}
    _ticks: -432000000000
    Days: 0
    Hours: -12
    Milliseconds: 0
    Minutes: 0
    Seconds: 0
    Ticks: -432000000000
    TotalDays: -0.5
    TotalHours: -12.0
    TotalMilliseconds: -43200000.0
    TotalMinutes: -720.0
    TotalSeconds: -43200.0
             * */
        }
    }
}
