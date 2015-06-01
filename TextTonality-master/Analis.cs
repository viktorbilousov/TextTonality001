using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVM;

namespace TextTonality
{
    class Analis
    {

        private static DictinaryAttribute Dictinary = new DictinaryAttribute(true);
        private static DictinaryAttribute Text = new DictinaryAttribute(false);
        private static SVM_Vectors train = new SVM_Vectors();
        private static SVM_Vectors test = new SVM_Vectors();
        private static SVM svm = new SVM();
        private static Model model;

        private static bool isReadyToTrain = false;
        private static bool isReadyToTest = false;
        private static bool isTran = false;

        public static bool IsReadyToTrain
        {
            get { return isReadyToTrain; }
        }

        public static bool IsReadyToTest
        {
            get { return isReadyToTest; }
        }

        public static bool IsTrain
        {
            get { return isTran; }
        }
        public static void StartCulcDic(string pos, string neg)
        {
            //Dictinary = new DictinaryAttribute(true);
            Dictinary.BildNewDictinary(
               new[]
                {
                    neg,
                    pos
                },
               new[]
                {
                    ClassTypeValue.Negative,
                    ClassTypeValue.Positive,
                });

            isReadyToTrain = true;

        }

        public static void LoadAndCulcTextDict(string text)
        {
           // Text = new DictinaryAttribute(false);
            Text.LoadTextToRecognition(
              new[]
                {
                    text
                });
            test.LoadVectorsFromAttribures(Text);
            // тестить
            //if(Dictinary != null)
            //      Text.PrepareTextForRecognition(Dictinary);
            isReadyToTest = true;

        }

        public static void LoadDict(string pathToDic)
        {
            if( Dictinary == null)
                Dictinary = new DictinaryAttribute(true);

            Dictinary.LoadDictinaryFromFile(pathToDic);
            isReadyToTrain = true;
        }

        public static void SaveDic(string pathToSave)
        {
            Dictinary.SaveAllTo(pathToSave, true);
        }

        public static void SaveModelSVM(string pathSaveModel)
        {
            Model.Write(pathSaveModel, model);

        }

        public static int StarTest()
        {
            double res =  svm.Test(test.VectorList[0]);
            ConsoleText.WriteLine(ClassType.ClassTypeStringList[(int)res]);
            return (int) res;

        }


        public static void LoadText(string text)
        {
            Text.LoadText(text);
            test.LoadVectorsFromAttribures(Text);
            isReadyToTest = true;
        }
        public static void StartTrain()
        {
            train.LoadVectorsFromAttribures(Dictinary);
            model = svm.TrainModel(train);
            isTran = true;
            //SVM_Vectors train = new SVM_Vectors();
            //SVM_Vectors test = new SVM_Vectors();



            //var model2 = Model.Read(@"..\..\input\model.txt");
            //Model.Write(@"..\..\input\model2.txt", model2);

            //  Console.ReadKey();
        }

    }
}
