//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: XoapService.cs 43 2010-12-08 04:17:45Z jheddings $
//=============================================================================
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Tiempo.Service.Properties;

namespace Tiempo.Service.Weather {
    internal class XoapService : WeatherService {

        private static readonly Logger _logger =
            Logger.Get(typeof(XoapService));

        ///////////////////////////////////////////////////////////////////////
        private static readonly Regex _lsupRegex =
            new Regex(@"^([0-9/]*\s+[0-9:]*\s+[A-Za-z]*)\s+(.*)$");

        ///////////////////////////////////////////////////////////////////////
        public override String ProviderURL {
            get {
                String id = Settings.Default.XoapPartnerID;
                return "http://www.weather.com/?prod=xoap&par=" + id;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public override Hashtable SearchForLocation(String search) {
            Settings conf = Settings.Default;

            StringBuilder url = new StringBuilder();
            url.Append("http://xoap.weather.com/search/search");
            url.Append("?where=").Append(search);
            url.Append("&par=").Append(conf.XoapPartnerID);
            url.Append("&key=").Append(conf.XoapPartnerKey);
            XDocument doc = XDocument.Load(url.ToString());

            // TODO load the XML from the weather service
            Hashtable locations = new Hashtable();

            throw new NotImplementedException();
        }

        ///////////////////////////////////////////////////////////////////////
        protected override WeatherData FetchCurrentWeather(String location) {
            Settings conf = Settings.Default;

            // build the URL for the weather service API
            StringBuilder url = new StringBuilder();
            url.Append("http://xoap.weather.com/weather/local/");
            url.Append(location).Append("?unit=m&cc&link=xoap&prod=xoap");
            url.Append("&par=").Append(conf.XoapPartnerID);
            url.Append("&key=").Append(conf.XoapPartnerKey);
            _logger.Debug("Fetch: {0}", url.ToString());

            XDocument doc = XDocument.Load(url.ToString());

            // TODO this is a little messy to read...
            WeatherData data = (
                from n in doc.Descendants("weather") select new WeatherData {
                    Location = n.Element("loc").Attribute("id").Value,
                    Name = n.Element("loc").Element("dnam").Value,
                    Latitude = decimal.Parse(n.Element("loc").Element("lat").Value),
                    Longitude = decimal.Parse(n.Element("loc").Element("lon").Value),
                    Sunrise = DateTime.Parse(n.Element("loc").Element("sunr").Value),
                    Sunset = DateTime.Parse(n.Element("loc").Element("suns").Value),
                    Temperature = int.Parse(n.Element("cc").Element("tmp").Value),
                    Humidity = int.Parse(n.Element("cc").Element("hmid").Value),
                    WindSpeed = int.Parse(n.Element("cc").Element("wind").Element("s").Value),
                    //TODO LastUpdated = n.Element("cc").Element("lsup").Value,
                }
            ).FirstOrDefault();

            // FIXME this should be parsed from the the return data
            data.LastUpdated = DateTime.Now;

            return data;
        }
    }
}