//=============================================================================
// Copyright © 2006 Heddway, All Rights Reserved
// $Id: Logger.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using log4net;
using log4net.Config;

// hide the underlying logging system

namespace Tiempo {
    public class Logger {

        private ILog _impl;

        ///////////////////////////////////////////////////////////////////////
        public static Logger Get(Type type) {
            Logger logger = new Logger();
            logger._impl = LogManager.GetLogger(type);

            return logger;
        }

        ///////////////////////////////////////////////////////////////////////
        static Logger() {
            BasicConfigurator.Configure();
        }

        ///////////////////////////////////////////////////////////////////////
        private Logger() {
        }

        ///////////////////////////////////////////////////////////////////////
        public void Debug(String msg) {
            _impl.Debug(msg);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Debug(String msg, params Object[] args) {
            _impl.DebugFormat(msg, args);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Debug(Exception e) {
            _impl.Debug(e.ToString());
        }

        ///////////////////////////////////////////////////////////////////////
        public void Info(String msg) {
            _impl.Info(msg);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Info(String msg, params Object[] args) {
            _impl.InfoFormat(msg, args);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Info(Exception e) {
            _impl.Info(e.ToString());
        }

        ///////////////////////////////////////////////////////////////////////
        public void Warn(String msg) {
            _impl.Warn(msg);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Warn(String msg, params Object[] args) {
            _impl.WarnFormat(msg, args);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Warn(Exception e) {
            _impl.Warn(e.ToString());
        }

        ///////////////////////////////////////////////////////////////////////
        public void Fatal(String msg) {
            _impl.Fatal(msg);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Fatal(String msg, params Object[] args) {
            _impl.FatalFormat(msg, args);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Fatal(Exception e) {
            _impl.Fatal(e.ToString());
        }
    }
}
