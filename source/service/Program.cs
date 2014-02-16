//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: Program.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Threading;

// TODO need a lock mechanism for this entry point

namespace Tiempo.Service {
    internal static class Program {

        private static readonly Logger _logger =
            Logger.Get(typeof(Program));

        private static bool _apprun = true;

        private static Tiempo _tiempo;

        ///////////////////////////////////////////////////////////////////////
        public static void Main(String[] args) {
            _logger.Info("Application START");

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            // TODO parse args and configure
            _tiempo = new Tiempo();
            _tiempo.Enabled = true;

            _logger.Debug("Starting main application loop");

            while (_apprun) {
                Thread.Sleep(100);
            }

            _logger.Info("Application EXIT");
        }

        ///////////////////////////////////////////////////////////////////////
        private static void Console_CancelKeyPress(Object sender, ConsoleCancelEventArgs args) {
            _logger.Info("Program canceled by user");

            args.Cancel = true;

            _tiempo.Dispose();
            _apprun = false;
        }
    }
}