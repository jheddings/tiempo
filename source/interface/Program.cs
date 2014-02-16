using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

// this may provide a graphical interface for interacting with the service

namespace Tiempo.GUI {
    static class Program {

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainFrame());
        }
    }
}