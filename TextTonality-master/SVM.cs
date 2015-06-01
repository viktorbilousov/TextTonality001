using System.Collections.Generic;
using SVM;

namespace TextTonality
{
    class SVM
    {

        private SVM_Vectors TraningList;

        private RangeTransform range;
        private Model model;

        public SVM()
        {
            TraningList = new SVM_Vectors();
        }

        /// <summary>
        /// Начать процес обучения
        /// </summary>
        public Model TrainModel(SVM_Vectors traningVectors)
        {

            List<Node[]> VectorList = traningVectors.VectorList;
            List<double> TypeElementVectorList = traningVectors.TypeElementVectorList;

            var problem = new Problem(VectorList.Count, TypeElementVectorList.ToArray(), VectorList.ToArray(), VectorList[0].Length);
            range = RangeTransform.Compute(problem);
            problem = range.Scale(problem);

            // константы подобраны методом тыка. работают оптимально 
            var param = new Parameter {C = 1, Gamma = .5};

            model = Training.Train(problem, param);
            return model;
        }

        /// <summary>
        /// Возвращает тип класса к которому относится вектор 
        /// </summary>
        /// <param name="testVector">Вектор</param>
        /// <returns></returns>
        public double Test(Node[] testVector)
        {
            return Prediction.Predict(model, testVector);
        }
        
    }

     class SVM_Vectors
    {
        private List<Node[]> _vectorList; //обучающие вектора 
        //node[] - вектор весов слов для k-го текста

        private List<double> _typeElementVectorList; // тип елементов вектора


        public SVM_Vectors()
        {
            _typeElementVectorList = new List<double>();
            _vectorList = new List<Node[]>();
        }

        public List<Node[]> VectorList
        {
            get { return _vectorList; }
        }

        public List<double> TypeElementVectorList
        {
            get { return _typeElementVectorList; }
        }


        /// <summary>
        /// Создает обучающие вектора из библиотеки
        /// </summary>
        /// <param name="dictinary">Библиотека</param>
        public void LoadVectorsFromAttribures(DictinaryAttribute dictinary)
        {
            var arrowOFAttrList = dictinary.AttributesList;
            var textTypes = dictinary.TextTypeList;

            var cntText = dictinary.TextTypeList.Count; // сумарное количество текстов
            var sumLeightOfVectors = arrowOFAttrList.Count; // сумарное количество всех признаков
            

          
            for (var i = 0; _vectorList.Count != cntText; i++)
            {
                _vectorList.Add(new Node[sumLeightOfVectors]);
                _typeElementVectorList.Add((double)textTypes[i]);
            }


            for (int attrIndex = 0; attrIndex < arrowOFAttrList.Count; attrIndex++)
            {
                for (int textIndex = 0; textIndex < cntText; textIndex++)
                {
                    var typeOfText = (int)textTypes[textIndex];
                    if (typeOfText == -1)
                        typeOfText = 0;
                    var node = new Node { Index = attrIndex  + 1 };

                    if (arrowOFAttrList[attrIndex].isInText[textIndex])
                        node.Value = arrowOFAttrList[attrIndex].weight[typeOfText];
                    else
                        node.Value = 0;

                    _vectorList[textIndex][attrIndex] = node;
                }
            }


        }

        /// <summary>
        /// Очистить обучающие вектора
        /// </summary>
        public void ClearAllVectors()
        {
            _vectorList.Clear();
            _typeElementVectorList.Clear();
        }
    }



}
