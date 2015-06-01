using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextTonality
{
    public partial class MashineForm : Form
    {
        public MashineForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            string Load = dialog.FileName;
            Analis.LoadSVM(Load);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            PathToFile.PathToSaveSVM = dialog.SelectedPath;
            ConsoleText.WriteLine("Save SVM: " + dialog.SelectedPath);
             


            ConsoleText.WriteLine("Start Train");

            if (Analis.IsReadyToTrain)
                Analis.StartTrain();
            else
            {
                ConsoleText.WriteLine("Error Dict");
                return;
            }
            ConsoleText.WriteLine("Train Ok");

            Analis.SaveModelSVM(PathToFile.PathToSaveSVM);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
