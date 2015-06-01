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
    public partial class LoadText : Form
    {
        public LoadText()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            PathToFile.PathToTestText = dialog.FileName;
            ConsoleText.WriteLine("Test text: " + dialog.FileName);
            Analis.LoadAndCulcTextDict(PathToFile.PathToTestText);   

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "") 
                Analis.LoadText(richTextBox1.Text);
            else 
                ConsoleText.WriteLine("Empty text");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Analis.IsReadyToTest && Analis.IsTrain)
                Analis.StarTest();
            else if (!Analis.IsTrain)
            {
                ConsoleText.WriteLine("Error Train");
                return;
            }
            else 
            {
                ConsoleText.WriteLine("Error Text");
                return;
                
            }

        }
    }
}
