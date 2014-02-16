//=============================================================================
// Copyright © 2006 Heddway, All Rights Reserved
// $Id: WeatherData.cs 43 2010-12-08 04:17:45Z jheddings $
//=============================================================================
using System;
using System.Globalization;

///////////////////////////////////////////////////////////////////////////////
namespace Tiempo.Service.Weather {
    public class WeatherData {

        public String Location { get; set; }
        public String Name { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime ExpiresAt { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }

        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public decimal Pressure { get; set; }

        public bool Expired {
            get { return (this.ExpiresAt <= DateTime.Now); }
        }

        ///////////////////////////////////////////////////////////////////////
        public WeatherData() {
            this.LastUpdated = DateTime.MinValue;
            this.Sunrise = DateTime.MaxValue;
            this.Sunset = DateTime.MaxValue;
            this.Latitude = decimal.MinValue;
            this.Longitude = decimal.MinValue;
        }
    }
}