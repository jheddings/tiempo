//=============================================================================
// Copyright © 2006 Heddway, All Rights Reserved
// $Id: Extensions.cs 40 2010-11-26 22:08:16Z jheddings $
//=============================================================================
using System;

namespace Tiempo {
    public static class Extensions {

        ///////////////////////////////////////////////////////////////////////
        public static DateTime SetSecond(this DateTime dt, int second) {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, second);
        }

        ///////////////////////////////////////////////////////////////////////
        public static DateTime SetMinute(this DateTime dt, int minute) {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, minute, dt.Second);
        }

        ///////////////////////////////////////////////////////////////////////
        public static DateTime SetHour(this DateTime dt, int hour) {
            return new DateTime(dt.Year, dt.Month, dt.Day, hour, dt.Minute, dt.Second);
        }

        ///////////////////////////////////////////////////////////////////////
        public static DateTime SetMonth(this DateTime dt, int month) {
            return new DateTime(dt.Year, month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        ///////////////////////////////////////////////////////////////////////
        public static DateTime SetDay(this DateTime dt, int day) {
            return new DateTime(dt.Year, dt.Month, day, dt.Hour, dt.Minute, dt.Second);
        }

        ///////////////////////////////////////////////////////////////////////
        public static DateTime SetYear(this DateTime dt, int year) {
            return new DateTime(year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
    }
}