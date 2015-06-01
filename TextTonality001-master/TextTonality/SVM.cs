using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVM;

namespace TextTonality
{
    class SVM
    {
        private List<Node[]> VectorList;    //обучающие вектора 
        //node[] - вектор весов слов для k-го текста

        private List<double> TypeElementVectorList; // тип елементов вектора


        private RangeTransform range;
        private Model model;


        public SVM()
        {
            TypeElementVectorList = new List<double>();
            VectorList = new List<Node[]>();

        }


        /// <summary>
        /// Создает обучающие вектора из библиотеки
        /// </summary>
        /// <param name="dictinary">Библиотека</param>
        public void LoadVectorsFromAttribures(DictinaryAttribute dictinary)
        {

            List<Attribute>[] arrowOFAttrList = dictinary.AttributesListArrow;
            List<ClassTypeValue> textType = dictinary.TextTypeList;

            int cntText = dictinary.TextTypeList.Count; // сумарное количество текстов
            int sumLeightOfVectors = 0; // сумарное количество всех признаков


            foreach (List<Attribute> attributesList in arrowOFAttrList)
            {
                sumLeightOfVectors += attributesList.Count;

                foreach (Attribute att in attributesList)
                {
                    att.setLeightIsInText(cntText);
                }
            }


            for (int i=0; VectorList.Count != cntText; i++)
            {
                VectorList.Add(new Node[sumLeightOfVectors]);
                TypeElementVectorList.Add((double)textType[i]);
            }


            int tmpCnt = 0;
            foreach (List<Attribute> attributeList in arrowOFAttrList)
            {
                for (int attrIndex = 0; attrIndex < attributeList.Count; attrIndex++)
                {
                    for (int textIndex = 0; textIndex < cntText; textIndex++)
                    {


                        int typeOfText = (int)textType[textIndex];
                        Node node = new Node();
                        node.Index = attrIndex + tmpCnt + 1;

                        if (attributeList[attrIndex].isInText[textIndex])
                            node.Value = attributeList[attrIndex].weight[typeOfText];
                        else
                            node.Value = 0;

                        VectorList[textIndex][attrIndex + tmpCnt] = node;


                    }
                }

                tmpCnt += attributeList.Count;
            }

            return;

        }
        /// <summary>
        /// Очистить обучающие вектора
        /// </summary>
        public void ClearAllVectors()
        {
            VectorList.Clear();
            TypeElementVectorList.Clear();
        }

        /// <summary>
        /// Начать процес обучения
        /// </summary>
        public void DoWork()
        {
            Problem problem = new Problem(VectorList.Count, TypeElementVectorList.ToArray(), VectorList.ToArray(), VectorList[0].Length);
            range = RangeTransform.Compute(problem);
            problem = range.Scale(problem);

            // константы подобраны методом тыка. работают оптимально 
            Parameter param = new Parameter();
            param.C = 1;
            param.Gamma = .5;


            model = Training.Train(problem, param);
            
            // todo : разобраться и написать как сохранить обученую модель в фаил (model)
            // todo : написать метод загрузки обученой модели из фаила
           


        }
        /// <summary>
        /// Возвращает тип класса к которому относится вектор 
        /// </summary>
        /// <param name="testVector">Вектор</param>
        /// <returns></returns>
        public double Test(Node[] testVector)
        {

            //todo : дописать метод для проверки большого текста (включая приведения в векторному типу)
            double assignment = Prediction.Predict(model, testVector);
            return assignment;
        }

        


    }
}
