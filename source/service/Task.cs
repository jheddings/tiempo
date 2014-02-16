//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: Task.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

// represents a single task in the schedule, composed of
// a condition and one or more commands to be executed

// XXX the way NextTime is cached between here and the condition feels a
// little funny...  especially with null conditions and the use of MaxValue

namespace Tiempo.Service {
    internal class Task {

        private static readonly Logger _logger =
            Logger.Get(typeof(Task));

        ///////////////////////////////////////////////////////////////////////
        private DateTime _lastTime = DateTime.MaxValue;
        public DateTime LastTime {
            get { return _lastTime; }
        }

        ///////////////////////////////////////////////////////////////////////
        private DateTime _nextTime = DateTime.MaxValue;
        public DateTime NextTime {
            get {
                if (this.Condition == null) {
                    return DateTime.MaxValue;
                } else if (_nextTime < DateTime.MaxValue) {
                    return _nextTime;
                }
                return this.Condition.NextTime;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        private List<Command> _commands = new List<Command>();
        public List<Command> Commands {
            get { return _commands; }
        }

        ///////////////////////////////////////////////////////////////////////
        public Condition Condition { get; set; }

        ///////////////////////////////////////////////////////////////////////
        public Task() {
        }

        ///////////////////////////////////////////////////////////////////////
        public bool IsTime() {
            return (NextTime <= DateTime.Now);
        }

        ///////////////////////////////////////////////////////////////////////
        public void Execute() {
            // TODO wrap this with exception handling ??
            // XXX maybe it would be better to log an ID instead of the command
            _logger.Info("Execute: {0}", "TODO");

            DateTime now = DateTime.Now;
            foreach (Command cmd in _commands) {
                cmd.Execute();
            }

            _lastTime = now;

            if (this.Condition != null) {
                _nextTime = this.Condition.NextTime;
            }

            _logger.Debug("Next time: {0}", this.NextTime);
        }

        ///////////////////////////////////////////////////////////////////////
        public override String ToString() {
            StringBuilder str = new StringBuilder("{Task ");

            if (this.Condition != null) {
                str.Append("; Condition:").Append(this.Condition);
            }

            str.Append("; Commands:").Append(_commands.Count);
            str.Append("; LastTime:").Append(this.LastTime);
            str.Append("; NextTime:").Append(this.NextTime);

            str.Append('}');
            return str.ToString();
        }

        ///////////////////////////////////////////////////////////////////////
        // this method assumes the node passed the schema validator
        public static Task Load(XmlNode node) {
            if (node.Name != "task") {
                return null;
            }

            Task task = new Task();

            // XXX this approach feels a bit brute-force...  not very elegant
            foreach (XmlNode child in node.ChildNodes) {
                if (child.Name == "condition") {
                    LoadCondition(child, task);
                } else if (child.Name == "commands") {
                    LoadCommands(child, task);
                }
            }

            return task;
        }

        ///////////////////////////////////////////////////////////////////////
        private static void LoadCondition(XmlNode node, Task task) {
            Condition cond = Condition.Load(node);
            if (cond != null) {
                task.Condition = cond;
                task._nextTime = cond.NextTime;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        private static void LoadCommands(XmlNode node, Task task) {
            foreach (XmlNode child in node.ChildNodes) {
                if (child.Name == "exec") {
                    Command cmd = new Command(child.InnerText);
                    task.Commands.Add(cmd);
                }

                // XXX will we support other kinds of commands?
            }
        }
    }
}