//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: Daemon.cs 41 2010-12-07 05:24:02Z jheddings $
//=============================================================================
using System;
using System.ServiceProcess;

namespace Tiempo.Service {
    internal class Daemon : ServiceBase {

        private static readonly Logger _logger =
            Logger.Get(typeof(Daemon));

        private Tiempo _tiempo;

        ///////////////////////////////////////////////////////////////////////
        public Daemon(Tiempo tiempo) {
            InitializeComponent();

            _tiempo = tiempo;
        }

        ///////////////////////////////////////////////////////////////////////
        protected override void OnStart(String[] args) {
            _logger.Debug("OnStart(args:{0})", args.Length);
            _tiempo.Enabled = true;
        }

        ///////////////////////////////////////////////////////////////////////
        protected override void OnStop() {
            _logger.Debug("OnStop()");
            _tiempo.Dispose();
        }

        ///////////////////////////////////////////////////////////////////////
        protected override void OnPause() {
            _logger.Debug("OnPause()");
            _tiempo.Enabled = false;
        }

        ///////////////////////////////////////////////////////////////////////
        protected override void OnContinue() {
            _logger.Debug("OnContinue()");
            _tiempo.Enabled = true;
        }

        ///////////////////////////////////////////////////////////////////////
        private void InitializeComponent() {
            this.ServiceName = "Tiempo";
            this.AutoLog = true;
            this.CanStop = true;
            this.CanShutdown = false;
            this.CanPauseAndContinue = true;
        }

        ///////////////////////////////////////////////////////////////////////
#if (false)
        public static void Main(String[] args) {
            _logger.Info("Application START");

            // TODO parse args and configure _tiempo
            Tiempo tiempo = new Tiempo();

            _logger.Debug("Starting daemon");
            ServiceBase.Run(new Daemon(tiempo));

            _logger.Info("Application EXIT");
        }
#endif
    }
}