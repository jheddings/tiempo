//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: NullService.cs 40 2010-11-26 22:08:16Z jheddings $
//=============================================================================
using System;
using System.Collections;

namespace Tiempo.Service.Weather {
    internal class NullService : WeatherService {

        ///////////////////////////////////////////////////////////////////////
        public override string ProviderURL {
            get { return null; }
        }

        ///////////////////////////////////////////////////////////////////////
        public override Hashtable SearchForLocation(String search) {
            return null;
        }

        ///////////////////////////////////////////////////////////////////////
        protected override WeatherData FetchCurrentWeather(String location) {
            return null;
        }
    }
}