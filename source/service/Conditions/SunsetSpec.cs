//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: SunsetSpec.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Text;
using System.Xml;
using Tiempo.Service.Weather;

namespace Tiempo.Service.Conditions {

    public enum SolarTime {
        Sunrise, Sunset
    }

    internal class SunSpec : SpecCond {

        private static WeatherService _wx = WeatherService.Instance;

        ///////////////////////////////////////////////////////////////////////
        private SolarTime _type;
        public SolarTime Type {
            get { return _type; }
        }

        ///////////////////////////////////////////////////////////////////////
        public override DateTime SpecTime {
            get {
                WeatherData data = _wx.GetCurrentWeather();
                if (data == null) { return DateTime.MaxValue; }

                DateTime time = DateTime.MaxValue;
                switch (_type) {
                    case SolarTime.Sunrise:
                        time = data.Sunrise;
                        break;

                    case SolarTime.Sunset:
                        time = data.Sunset;
                        break;
                }

                // the weather data is reporting times for today's sunset,
                // so we'll just force it to report the same time tomorrow
                while (time <= DateTime.Now) {
                    time = time.AddDays(1);
                }

                return time;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public SunSpec(SolarTime type) {
            _type = type;
        }

        ///////////////////////////////////////////////////////////////////////
        public static new SunSpec Load(XmlNode node) {
            SunSpec spec = null;

            switch (node.Name) {
                case "sunset":
                    spec = new SunSpec(SolarTime.Sunset);
                    break;

                case "sunrise":
                    spec = new SunSpec(SolarTime.Sunrise);
                    break;
            }

            XmlAttribute attr = node.Attributes["offset"];
            if (attr != null) {
                spec.Offset = int.Parse(attr.Value);
            }

            return spec;
        }

        ///////////////////////////////////////////////////////////////////////
        public override String ToString() {
            StringBuilder str = new StringBuilder("{");

            switch (_type) {
                case SolarTime.Sunrise:
                    str.Append("sunrise");
                    break;

                case SolarTime.Sunset:
                    str.Append("sunset");
                    break;
            }

            if (this.Offset < 0) {
                str.Append(this.Offset);
            } else if (this.Offset > 0) {
                str.Append('+').Append(this.Offset);
            }

            str.Append('}');
            return str.ToString();
        }
    }
}