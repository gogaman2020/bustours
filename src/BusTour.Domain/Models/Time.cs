using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Models
{
    public struct Time
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public Time(int hours, int minutes = 0, int seconds = 0)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        public DateTime AddToDate(DateTime date)
        {
            return date.Date.AddHours(Hours).AddMinutes(Minutes).AddSeconds(Seconds);
        }
    }
}
