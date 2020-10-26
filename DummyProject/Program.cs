using LogSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DummyProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Use the LoginForm with the parameter of the form we want to show after a successful login
            Application.Run(new LoginForm(new Form1()));
        }
    }
}
