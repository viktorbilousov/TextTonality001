using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SVM;

namespace TextTonality
{
   
    public partial class MainForm : Form
    {
        private AddDictForm addDictForm;
        private LoadText loadText;
        private MashineForm mashineForm;
        
        public MainForm()
        {
            InitializeComponent();
            
         //   ConsoleText.Set(ref this.ConsoleBox);
        }

        //public void  StartProgram()
        //{
            
        //    var dictinary = new DictinaryAttribute(true);
        //    var Text = new DictinaryAttribute(false);

        //  //  dictinary.LoadDictinaryFromFile(@"C:\001\002\Dictinary.txt");
        //    // dictinary.SaveAllTo(@"C:\001\002", true);

        //    Text.LoadTextToRecognition(
        //        new[]
        //        {
        //            @"C:\001\002\text.txt"
        //        });


        //    dictinary.BildNewDictinary(
        //        new[]
        //        {
        //            @"C:\001\002\bad2.txt",
        //            @"C:\001\002\good2.txt"
        //        },
        //        new[]
        //        {
        //            ClassTypeValue.Negative,
        //            ClassTypeValue.Positive,
        //        });

        // //   Text.PrepareTextForRecognition(dictinary);

        //    var svm = new SVM();
        //    SVM_Vectors train = new SVM_Vectors();
        //    SVM_Vectors test = new SVM_Vectors();
         

        //    train.LoadVectorsFromAttribures(dictinary);
        //    test.LoadVectorsFromAttribures(Text);



        //    var model = svm.TrainModel(train);
        //    double res =  svm.Test(test.VectorList[0]);

        //    ConsoleText.WriteLine(ClassType.ClassTypeStringList[(int)res]);
        //    Model.Write(@"..\..\input\model.txt", model);

            
        //    //var model2 = Model.Read(@"..\..\input\model.txt");
        //    //Model.Write(@"..\..\input\model2.txt", model2);

            
        //    ConsoleText.WriteLine("All is done");
        //  //  Console.ReadKey();
        //}

       

        private void button1_Click(object sender, EventArgs e)
        {
          //  StartProgram();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           addDictForm = new AddDictForm();
           addDictForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            mashineForm = new MashineForm();
            mashineForm.Show();
           

        }

        private void button4_Click(object sender, EventArgs e)
        {
            loadText = new LoadText();
            loadText.Show();
        }

        
        
    }
    public static class ConsoleText
    {
        private static ListBox _listBox;

        public static void Set(ref ListBox list)
        {
            _listBox = list;
        }

        public static void WriteLine(string Line)
        {

            if (_listBox == null)
            {
                Console.WriteLine(Line);
                return;
            }
            _listBox.Items.Add(Line);
            _listBox.Update();

        }
        
    }

    public static class PathToFile
    {
        public static string PathToNegativeText;
        public static string PathToPositiveText;
        public static string PathToTestText;
        public static string PathToSaveDictinary;
        public static string PathToDictionary;
        public static string PathToSaveSVM;

        
    }
}
