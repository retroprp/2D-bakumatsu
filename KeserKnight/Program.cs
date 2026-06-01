using System;
using System.Windows.Forms;

namespace KeserKnight
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
            // Form1 doğrudan KeserKnight namespace'i altında olduğu için burası jilet gibi uyuşmalı
            Application.Run(new Form1());
        }
    }
}