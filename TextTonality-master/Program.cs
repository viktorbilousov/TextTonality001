using System;
using SVM;
using System.Windows.Forms;

namespace TextTonality
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // <- вот тут
            
        }
    }
}
