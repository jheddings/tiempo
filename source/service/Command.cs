//=============================================================================
// Copyright © 2009 Heddway, All Rights Reserved
// $Id: Command.cs 46 2011-01-24 03:43:14Z jheddings $
//=============================================================================
using System;
using System.Diagnostics;
using System.Text;

// TODO this class should have better cmdline-parsing
// (e.g. handle escaped and quoted strings)

// XXX will we ever have other kinds of commands?

namespace Tiempo.Service {
    internal class Command {

        // XXX should we make this a configurable option?
        public const int MaxExecTimeMs = 5 * 1000;

        private static readonly Logger _logger =
            Logger.Get(typeof(Command));

        private ProcessStartInfo _proc;

        ///////////////////////////////////////////////////////////////////////
        private Command() {
            _proc = new ProcessStartInfo();
            _proc.CreateNoWindow = true;
            _proc.UseShellExecute = true;
        }

        ///////////////////////////////////////////////////////////////////////
        public Command(String cmdline) : this() {
            _proc.FileName = GetCommand(cmdline);
            _proc.Arguments = GetArguments(cmdline);
        }

        ///////////////////////////////////////////////////////////////////////
        public Command(String cmd, String args) : this() {
            _proc.FileName = cmd;
            _proc.Arguments = args;
        }

        ///////////////////////////////////////////////////////////////////////
        public void Execute() {
            _logger.Debug("Execute: {0}", ToString());

            try {
                Process proc = Process.Start(_proc);

                if (proc == null) {
                    _logger.Warn("Could not start process");
                } else {
                    CompleteProcess(proc);
                }
            } catch (Exception e) {
                _logger.Warn(e);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public override String ToString() {
            StringBuilder str = new StringBuilder();
            str.Append('{').Append(_proc.FileName);

            String args = _proc.Arguments;
            if ((args != null) && (args.Length > 0)) {
                str.Append(':').Append(args);
            }

            str.Append('}');
            return str.ToString();
        }

        ///////////////////////////////////////////////////////////////////////
        private String GetCommand(String cmdline) {
            // TODO handle quoted or escaped command strings
            int space = cmdline.IndexOf(' ');
            return (space < 0) ? cmdline : cmdline.Substring(0, space);
        }

        ///////////////////////////////////////////////////////////////////////
        private String GetArguments(String cmdline) {
            // TODO handle quoted or escaped command strings
            int space = cmdline.IndexOf(' ');
            return (space < 0) ? null : cmdline.Substring(space + 1);
        }

        ///////////////////////////////////////////////////////////////////////
        private void CompleteProcess(Process proc) {
            _logger.Debug("Process started");

            if (proc.WaitForExit(MaxExecTimeMs)) {
                if (proc.ExitCode == 0) {
                    _logger.Debug("Process completed successfully");
                } else {
                    _logger.Warn("Process completed with errors: {0}", proc.ExitCode);
                }

            } else {
                _logger.Warn("Process did not complete; killing");
                try { proc.Kill(); } catch { /* toss any exceptions */ }
            }
        }
    }
}