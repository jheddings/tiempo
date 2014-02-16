//=============================================================================
// Copyright © 2006 Heddway, All Rights Reserved
// $Id: TimeCond.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;

namespace Tiempo.Service.Conditions {
    internal class TimeCond : Condition {

        private static readonly Regex TimeRE =
            new Regex(@"^(?<h>\d{1,2}):(?<m>\d{2})");

        private int _hour;
        private int _minute;

        ///////////////////////////////////////////////////////////////////////
        public override DateTime NextTime {
            get {
                DateTime next = DateTime.Now;
                next = next.SetHour(_hour);
                next = next.SetMinute(_minute);
                next = next.SetSecond(0);

                while (next <= DateTime.Now) {
                    next = next.AddDays(1);
                }

                return next;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        private TimeCond(String time) {
            Match match = TimeRE.Match(time);

            if (match.Success) {
                _hour = int.Parse(match.Groups["h"].Value);
                _minute = int.Parse(match.Groups["m"].Value);

            } else {
                throw new FormatException("Invalid time format");
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public TimeCond(int hours, int minutes) {
            _hour = hours;
            _minute = minutes;
        }

        ///////////////////////////////////////////////////////////////////////
        public static new TimeCond Load(XmlNode node) {
            return new TimeCond(node.InnerText);
        }

        ///////////////////////////////////////////////////////////////////////
        public override String ToString() {
            StringBuilder str = new StringBuilder();

            str.Append('{');
            str.AppendFormat("{0:00}:{1:00}", _minute, _hour);
            str.Append('}');

            return str.ToString();
        }
    }
}