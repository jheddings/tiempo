//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: WeatherService.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using Tiempo.Service.Properties;

// TODO persist the weather data cache on disk

namespace Tiempo.Service.Weather {
    internal abstract class WeatherService {

        private static readonly Logger _logger =
            Logger.Get(typeof(WeatherService));

        private Settings _conf = Settings.Default;

        private Dictionary<String, WeatherData> _cache;

        ///////////////////////////////////////////////////////////////////////
        public abstract String ProviderURL { get; }

        ///////////////////////////////////////////////////////////////////////
        private static WeatherService _instance;
        public static WeatherService Instance {
            get {
                if (_instance == null) {
                    _instance = new YahooService();
                }
                return _instance;
            }
        }
        
        ///////////////////////////////////////////////////////////////////////
        protected WeatherService() {
            _cache = new Dictionary<String, WeatherData>();
        }

        ///////////////////////////////////////////////////////////////////////
        // Return the current weather conditions.
        public WeatherData GetCurrentWeather() {
            return GetCurrentWeather(Settings.Default.Location);
        }

        ///////////////////////////////////////////////////////////////////////
        // Return the current weather conditions.
        public WeatherData GetCurrentWeather(String location) {
            WeatherData weather = null;
            DateTime now = DateTime.Now;

            lock (_cache) {
                if (_conf.WeatherCacheEnabled) {
                    weather = GetCachedData(location);
                }

                if (weather == null) {
                    _logger.Info("Retrieving current weather from provider");
                    weather = FetchCurrentWeather(location);
                    // TODO should we sanity-check the result?

                    // force the data to expire based on config
                    uint exp = _conf.WeatherCacheTimeout;
                    weather.ExpiresAt = now.AddMinutes(exp);
                }

                if (weather != null) {
                    _cache[location] = weather;
                }
            }

            if (weather == null) {
                _logger.Warn("Update failed: {0}", location);
            } else {
                _logger.Debug("Current data: {0}", weather.LastUpdated);
            }

            return weather;
        }

        ///////////////////////////////////////////////////////////////////////
        // Return a map of the possible locations as "id" => "label"
        public abstract Hashtable SearchForLocation(String search);

        ///////////////////////////////////////////////////////////////////////
        // Return the current weather data from the provider.
        protected abstract WeatherData FetchCurrentWeather(String location);

        ///////////////////////////////////////////////////////////////////////
        private WeatherData GetCachedData(String location) {
            WeatherData weather = null;

            while (_cache.Count >= _conf.WeatherCacheSize) {
                // TODO remove any expired items
                // TODO remove the oldest entry
                _cache.Clear();
            }

            // look for a cached version of the data
            if (_cache.ContainsKey(location)) {
                WeatherData cached = _cache[location];
                if (cached.Expired) {
                    _logger.Debug("Cache expired: {0}", location);
                    _cache.Remove(location);
                } else {
                    _logger.Debug("Cache hit: {0}", location);
                    weather = cached;
                }
            } else {
                _logger.Debug("Cache miss: {0}", location);
            }

            return weather;
        }
    }
}