//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: YahooService.cs 43 2010-12-08 04:17:45Z jheddings $
//=============================================================================
using System;
using System.Collections;
using System.Text;
using System.Xml;
using Tiempo.Service.Properties;

// FIXME the location used by this implementation has been deprecated
// this uses the same location as the XOAP service, but Yahoo! wants WOEID's
// archived documentation: http://developer.yahoo.com/weather/archive.html

namespace Tiempo.Service.Weather {
    internal class YahooService : WeatherService {

        private static readonly Logger _logger =
            Logger.Get(typeof(YahooService));

        ///////////////////////////////////////////////////////////////////////
        public override String ProviderURL {
            get { return "http://developer.yahoo.com/weather/"; }
        }

        ///////////////////////////////////////////////////////////////////////
        public override Hashtable SearchForLocation(String search) {
            throw new NotImplementedException();
        }

        ///////////////////////////////////////////////////////////////////////
        protected override WeatherData FetchCurrentWeather(String location) {
            Settings conf = Settings.Default;

            // build the URL for the weather service API
            StringBuilder url = new StringBuilder();
            url.Append("http://weather.yahooapis.com/forecastrss?");
            url.Append("u=c&w=").Append(conf.Location);  // WOEID
            _logger.Debug("Fetch: {0}", url.ToString());

            XmlReaderSettings sett = new XmlReaderSettings();
            sett.IgnoreWhitespace = true;
            sett.IgnoreComments = true;

            WeatherData data = new WeatherData {
                Location = location,
                LastUpdated = DateTime.Now,
            };

            XmlReader reader = XmlReader.Create(url.ToString(), sett);
            while (reader.Read()) {
                if (reader.Name == "lastBuildDate") {
                    // TODO make this work...
                    //data.LastUpdated = reader.ReadElementContentAsDateTime();
                }

                if (reader.Name == "yweather:location") {
                    data.Name = reader["city"];
                }

                if (reader.Name == "yweather:condition") {
                    data.Temperature = int.Parse(reader["temp"]);
                }

                // FIXME this should look at tomorrow if the sunrise or sunset are old
                if (reader.Name == "yweather:astronomy") {
                    data.Sunrise = DateTime.Parse(reader["sunrise"]);
                    data.Sunset = DateTime.Parse(reader["sunset"]);
                }

                if (reader.Name == "yweather:atmosphere") {
                    data.Humidity = int.Parse(reader["humidity"]);
                    data.Pressure = decimal.Parse(reader["pressure"]);
                }

                if (reader.Name == "yweather:wind") {
                    data.WindSpeed = (int) decimal.Parse(reader["speed"]);
                }

                if (reader.Name == "geo:lat") {
                    data.Latitude = reader.ReadElementContentAsDecimal();
                }

                if (reader.Name == "geo:long") {
                    data.Longitude = reader.ReadElementContentAsDecimal();
                }
            }

            return data;
        }
    }
}