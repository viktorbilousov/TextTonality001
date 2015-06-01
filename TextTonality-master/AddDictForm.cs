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
    public partial class AddDictForm : Form
    {
        public AddDictForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            PathToFile.PathToNegativeText = dialog.FileName;
            ConsoleText.WriteLine("Negative text: " + dialog.FileName);
            checkBox1.CheckState = CheckState.Checked;

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            PathToFile.PathToPositiveText = dialog.FileName;
            ConsoleText.WriteLine("Positive text: " + dialog.FileName);
            checkBox2.CheckState = CheckState.Checked;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (PathToFile.PathToNegativeText == null || PathToFile.PathToPositiveText == null ||
                PathToFile.PathToSaveDictinary == null)
            {
                ConsoleText.WriteLine("Error path");
                return;
            }
            Analis.StartCulcDic(PathToFile.PathToPositiveText, PathToFile.PathToNegativeText);
            Analis.SaveDic(PathToFile.PathToSaveDictinary);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            PathToFile.PathToSaveDictinary = dialog.SelectedPath;
            ConsoleText.WriteLine("Seve dictionary to: " + dialog.SelectedPath);
            checkBox3.CheckState = CheckState.Checked;


        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            PathToFile.PathToDictionary = dialog.FileName;
            ConsoleText.WriteLine("Dictionary: " + dialog.FileName);
            Analis.LoadDict(PathToFile.PathToDictionary);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
