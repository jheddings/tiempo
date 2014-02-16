//=============================================================================
// Copyright © 2006 Heddway, All Rights Reserved
// $Id: CronCond.cs 40 2010-11-26 22:08:16Z jheddings $
//=============================================================================
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Tiempo.Service.Conditions {
    internal class CronCond : Condition {

        private String _orig;

        private CronSet _minutes;
        private CronSet _hours;
        private CronSet _days;
        private CronSet _months;
        private CronSet _dow;

        ///////////////////////////////////////////////////////////////////////
        public override DateTime NextTime {
            get { return CalcNextTime(DateTime.Now); }
        }

        ///////////////////////////////////////////////////////////////////
        public CronCond() {
        }

        ///////////////////////////////////////////////////////////////////////
        public override String ToString() {
            return _orig;
        }

        ///////////////////////////////////////////////////////////////////////
        // http://tinyurl.com/canztx (loopy version)
        private DateTime CalcNextTime(DateTime start) {
            DateTime next = start.AddMinutes(1).SetSecond(0);

            for (int loop = 0; loop < 256; loop++) {
                if (! _months[next.Month]) {
                    next = next.AddMonths(1).SetDay(1).SetHour(0).SetMinute(0);
                    continue;
                }

                if (! _days[next.Day]) {
                    next = next.AddDays(1).SetHour(0).SetMinute(0);
                    continue;
                }

                if (! _hours[next.Hour]) {
                    next = next.AddHours(1).SetMinute(0);
                    continue;
                }

                if (! _minutes[next.Minute]) {
                    next = next.AddMinutes(1);
                    continue;
                }

                // TODO handle day of week

                return next;
            }

            return DateTime.MaxValue;
        }

        ///////////////////////////////////////////////////////////////////////
        public static CronCond Parse(String str) {
            String[] fields = str.Split(' ');
            if (fields.Length != 5) { return null; }

            CronCond cond = new CronCond();
            cond._minutes = CronSet.Parse(fields[0], 0, 59);
            cond._hours = CronSet.Parse(fields[1], 0, 23);
            cond._days = CronSet.Parse(fields[2], 1, 31);
            cond._months = CronSet.Parse(fields[3], 1, 12);
            cond._dow = CronSet.Parse(fields[4], 0, 6);

            cond._orig = str;
            return cond;
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    internal class CronSet {

        // the named groups of this expression:
        // - m : the matched expr (w/out step)
        // - r : a range specifier
        //     - rb : the start of the range
        //     - re : the end of the range
        // - n : a single matched number
        // - s : an optional step
        private static Regex CronPartRE =
            new Regex(@"(?<m>\*|(?<r>(?<rb>\d+)-(?<re>\d+))|(?<n>\d+))(/(?<s>\d+))?");

        private int _min;
        private int _max;

        private BitArray _bits;
        private String _orig;

        ///////////////////////////////////////////////////////////////////
        public bool this[int val] {
            get { return _bits[val - _min]; }
            set { _bits[val - _min] = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        public CronSet(int min, int max) {
            // range is inclusive (+1)
            int range = max - min + 1;
            _bits = new BitArray(range);

            _min = min;
            _max = max;
        }

        ///////////////////////////////////////////////////////////////////////
        public override String ToString() {
            return _orig;
        }

        ///////////////////////////////////////////////////////////////////////
        public static CronSet Parse(String str, int min, int max) {
            CronSet set = new CronSet(min, max);
            Expand(str, set);
            set._orig = str;
            return set;
        }

        ///////////////////////////////////////////////////////////////////////
        // based loosely on perl's Set::Crontab parsing routine
        private static void Expand(String str, CronSet set) {
            foreach (String part in str.Split(',')) {
                Match match = CronPartRE.Match(part);

                int step = 1;
                if (match.Groups["s"].Success) {
                    String val = match.Groups["s"].Value;
                    step = int.Parse(val);
                }

                if (match.Groups["n"].Success) {
                    String val = match.Groups["n"].Value;
                    set[int.Parse(val)] = true;

                } else if (match.Groups["m"].Value == "*") {
                    for (int idx = set._min; idx <= set._max; idx++) {
                        if (idx % step == 0) { set[idx] = true; }
                    }

                } else if (match.Groups["r"].Success) {
                    int start = int.Parse(match.Groups["rb"].Value);
                    int stop = int.Parse(match.Groups["re"].Value);
                    for (int idx = start; idx <= stop; idx++) {
                        if (idx % step == 0) { set[idx] = true; }
                    }
                }
            }
        }
    }
}