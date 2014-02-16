//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: Scheduler.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using Tiempo.Service.Properties;

// TODO handle a signal/command to dump the current schedule to a file

namespace Tiempo.Service {
    internal class Scheduler : IDisposable {

        private static readonly Logger _logger =
            Logger.Get(typeof(Scheduler));

        private Timer _timer;  // the timer used to service the schedule
        private String _filename;  // the schedule configuration file
        private List<Task> _tasks;  // all tasks in the active schedule

        private const uint TimerTickRateMs = 60 * 1000;

        ///////////////////////////////////////////////////////////////////////
        public bool Enabled { get; set; }

        ///////////////////////////////////////////////////////////////////////
        public Scheduler() {
            _tasks = new List<Task>();
            this.Enabled = false;

            // setup the timer to fire every minute, start at the next minute
            _timer = new Timer(new TimerCallback(TimerTick));
            AlignTimerToNextMinute(DateTime.Now);

            _logger.Info("Scheduler started");
        }

        ///////////////////////////////////////////////////////////////////////
        public void Dispose() {
            _timer.Dispose();
            _tasks.Clear();
            _filename = null;

            _logger.Info("Scheduler disposed");
        }

        ///////////////////////////////////////////////////////////////////////
        public void Load(String file) {
            _logger.Info("Load: {0}", file);
            _filename = null;

            try {
                UnsafeLoadFile(file);
                _filename = file;

            } catch (XmlSchemaValidationException xsve) {
                _logger.Warn("Schema violation; line {0}: {1}",
                             xsve.LineNumber, xsve.Message);

            } catch (XmlException xe) {
                _logger.Warn("Invalid XML input; line {0}: {1}",
                             xe.LineNumber, xe.Message);

            } catch (Exception e) {
                _logger.Warn(e);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public void Reload() {
            if (_filename == null) {
                throw new Exception("no file to reload");
            }

            Load(_filename);
        }

        ///////////////////////////////////////////////////////////////////////
        private void AlignTimerToNextMinute(DateTime time) {
            // TODO only make changes if we need to...  we can't rely completely
            // on time.Second being zero, because this method may be called from
            // the constructor, at which point the timer has never been started
            // it's possible that time.Second is zero, but the timer is inactive

            // TODO do we need to compensate for a timer that drifts backwards?
            // that would only happen if the tick was called early, which isn't
            // supposed to happen according to the documentation -- I hope not

            if (time.Second != 0) {
                _logger.Debug("Drift detected: {0} sec", time.Second);
            }

            int due = (60 - time.Second) * 1000;
            _timer.Change(due, TimerTickRateMs);
        }

        ///////////////////////////////////////////////////////////////////////
        private void TimerTick(Object data) {
            DateTime now = DateTime.Now;

            // keep the timer aligned at the start of each minute
            AlignTimerToNextMinute(now);

            if (this.Enabled) {
                _logger.Debug("Tick: {0}", now);
                ExecuteTasksIfNeeded();
            }
        }

        ///////////////////////////////////////////////////////////////////////
        private void ExecuteTasksIfNeeded() {
            lock (_tasks) {
                foreach (Task task in _tasks) {
                    if (task.IsTime()) {
                        task.Execute();
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        private void UnsafeLoadFile(String file) {
            // XXX maybe we only need to do this once
            XmlReaderSettings sett = GetReaderSettings();

            // we'll use a document to make parsing a little easier later
            XmlDocument doc = new XmlDocument();

            // do I/O operations before loading the task list
            using (FileStream stream = File.OpenRead(file)) {
                using (XmlReader reader = XmlReader.Create(stream, sett)) {
                    doc.Load(reader);
                }
            }

            // now we can use the document to load the schedule
            lock (_tasks) {
                // UnsafeLoadConfig(doc); ??
                UnsafeLoadTasks(doc);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        private void UnsafeLoadTasks(XmlDocument doc) {
            _tasks.Clear();

            XmlNodeList nodes = doc.GetElementsByTagName("task");
            _logger.Debug("Parsing {0} tasks from document", nodes.Count);

            foreach (XmlNode node in nodes) {
                Task task = Task.Load(node);

                if (task == null) {
                    _logger.Warn("Invalid task");

                } else {
                    _tasks.Add(task);
                    _logger.Debug("Added task: {0}", task);
                }
            }

            _logger.Debug("Loaded {0} tasks into scheduler", _tasks.Count);
        }

        ///////////////////////////////////////////////////////////////////////
        private XmlReaderSettings GetReaderSettings() {
            XmlReaderSettings sett = new XmlReaderSettings();
            sett.IgnoreWhitespace = true;
            sett.IgnoreComments = true;

            // load the validating schema (*should* never fail)
            using (TextReader reader = new StringReader(Resources.TiempoXSD)) {
                XmlSchema schema = XmlSchema.Read(reader, null);

                if (schema != null) {
                    sett.ValidationType = ValidationType.Schema;
                    sett.Schemas.Add(schema);
                }
            }

            return sett;
        }
    }
}