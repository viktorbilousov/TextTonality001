using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTonality
{
    class Program
    {
        static void Main(string[] args)
        {
            DictinaryAttribute dictinary = new DictinaryAttribute();


            dictinary.BildNewDictinary(
                    new[]
                    {
                        @"C:\001\002\bad3.txt",
                        @"C:\001\002\good3.txt"
                    },
                    new[]
                    {
                        ClassTypeValue.Negative, 
                        ClassTypeValue.Positive, 
                    }
                );


            // dictinary.LoadDictinaryFrom(@"C:\001\002\");

            // dictinary.SaveAllTo(@"C:\001\002", true);

            SVM svm = new SVM();
            svm.LoadVectorsFromAttribures(dictinary);
            svm.DoWork();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
