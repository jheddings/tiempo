//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: Tiempo.cs 41 2010-12-07 05:24:02Z jheddings $
//=============================================================================
using System;
using Tiempo.Service.Properties;

namespace Tiempo.Service {
    internal class Tiempo {

        private static readonly Logger _logger =
            Logger.Get(typeof(Tiempo));

        private Scheduler _sched = new Scheduler();

        ///////////////////////////////////////////////////////////////////////
        private bool _enabled = false;
        public bool Enabled {
            get { return _enabled; }

            set {
                _sched.Enabled = value;
                _enabled = value;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public Tiempo() {
            Settings settings = Settings.Default;

            if (settings.Schedule != String.Empty) {
                _sched.Load(settings.Schedule);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public void Dispose() {
            _logger.Debug("Disposing Tiempo...");
            _sched.Dispose();
            _logger.Debug("Tiempo Disposed");
        }
    }
}