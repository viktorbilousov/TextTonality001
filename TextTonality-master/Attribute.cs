using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTonality
{
    internal class Attribute
    {
        private bool needCalcWeight = true;
        private bool needRecognizeClass = false;

        public string word;
        public float[] weight; // вектор веса текста в j-м классе текста
        public int[] count; // количество раз, которое встречается слово в j-м классе текста

        public List<bool> isInText; // есть ли слово в j-м тексте 


        /// <summary>Устанавливает отметку присутствия слова в тексте </summary>
        /// <param name = "numberOfText">Номер текста начиная с 1 </param>
        /// <param name = "type">Класс текста</param>
        public void SetNumberText(int numberOfText)
        {


            if (numberOfText > isInText.Count)
            {
                while (numberOfText != isInText.Count)
                    isInText.Add(false);
            }
            numberOfText--;

            isInText[numberOfText] = true;

            return;

        }

        /// <summary>Создает атрибут </summary>
        /// <param name = "Word">Текст слова</param>
        /// <param name = "Weight">Вес слова в классе <para>Value</para> </param>
        /// <param name="value">Класс атрибута(текста)</param>
        public Attribute(string Word, float Weight, ClassTypeValue value)
        {

            Init();

            this.word = Word;
            this.weight[(int)value] = Weight;
            count[(int)value] = 1;


            needCalcWeight = false;
        }

        /// <summary>Создает атрибут с пустым весом </summary>
        /// <param name = "Word">Текст слова</param>
        /// <param name="value">Класс атрибута(текста)</param>
        public Attribute(string Word, ClassTypeValue value)
        {
            Init();

           this.word = Word;
            count[(int)value] = 1;
            weight[(int)value] = float.NaN;
        }

        /// <summary>Создает атрибут </summary>
        /// <param name = "Word">Текст слова</param>
        /// <param name = "_Weight">Вес слова во всех класах</para> </param>
        /// <param name = "_isInText">вектор соответствия слов в тексте</para> </param>
        public Attribute(string Word, float[] _Weight, List<bool> _isInText )
        {
            Init();

            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] = _Weight[i];
                count[i] = 1;
            }

            this.word = Word;
            this.isInText = _isInText;
            needCalcWeight = false;
        }
        /// <summary>
        /// Создает Атрибут, класс которого надо определить
        /// </summary>
        /// <param name="Word">Текст слова</param>
        public Attribute(string Word)
        {

            int UndefCnt = 1;
            
            weight = new float[UndefCnt];
            count = new int[UndefCnt];
            isInText = new List<bool>();
            isInText.Add(true);

            for (int i = 0; i < UndefCnt; i++)
            {
                weight[i] = float.NaN;
                count[i] = 0;

            }
            
            word = Word;
            needRecognizeClass = true;
            needCalcWeight = true;

        }


        private void Init()
        {
            weight = new float[ClassType.cnt];
            count = new int[ClassType.cnt];
            isInText = new List<bool>();

            for (int i = 0; i < ClassType.cnt; i++)
            {
                weight[i] = float.NaN;
                count[i] = 0;

            }
        }

        /// <summary>Увеличивает счетчик </summary>
        /// <param name="value">Класс атрибута(текста)</param>
        public void incrementCnt(ClassTypeValue value)
        {
            if (value == ClassTypeValue.Unknown)
                count[0]++;
            else
                count[(int)value]++;
        }

        /// <summary>Калькулирует глобальный вес слова в классах</summary>
        public void CaclWeight()
        {

            /* вообще эту штуку считают не RF методом , а другими. Однако для 
             * для комбинированного метода (SVM + идекс парам) использование RF оправданно 
             * (но надо попробывать другие )
             */

            if (needRecognizeClass)
            {
                CalcUndefClass();
                return;
            }


            const int CoefNorm = 1;

            // todo : сделать конусиносную нормализацию 
            // todo : http://habrahabr.ru/post/149605/ взять оттуда вес для бинарнго класса
            
            int sumCnt = 0;
            foreach (int cnt in count)
                sumCnt += cnt;

            for (int i = 0; i < ClassType.cnt; i++)
            {
                float tmp = 0;
                if (sumCnt < 1 || sumCnt - count[i] == 0)
                    tmp = count[i];
                else
                    tmp = count[i] / (float)(sumCnt - count[i]);

                weight[i] = (float)Math.Log(2 + tmp, 2) * CoefNorm; // RF метод  
            }
        }

        private void CalcUndefClass()
        {
            weight[0] = (float)(Math.Log(2 + count[0],2));
        }


        /// <summary>Устанавливает длину списка индикаторов присутствия признаков в тексте</summary>
        /// <param name = "leight">количество текстов</param>
        /// <param name = "type">Класс текста</param>
        public void setLeightIsInText(int leinght)
        {
            while (leinght != isInText.Count)
                isInText.Add(false);
        }

        public override string ToString()
        {
            string output = word;
            for (int i = 0; i < ClassType.cnt; i++)
            {
                if (count[i] == 0)
                    continue;

                if (word.Length < 8)
                {
                    output += "\t";
                    if (word.Length < 3)
                        output += "\t";
                }


                output += "\t";
                output += ClassType.ClassTypeStringList[i];
                output += " cnt = " + count[i];
                output += "  W = " + weight[i].ToString("##,###");

            }
            return output;
        }

        public override bool Equals(object obj)
        {
            Attribute Atr = obj as Attribute;
            if (Atr.word == this.word)
                return true;

            return false;
        }

        public void clearAllWeight()
        {
            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] = float.NaN;
            }
        }

        public void resetAllCnt()
        {
            for (int i = 0; i < count.Length; i++)
            {
                count[i] = 1;
            }
        }
    }


    class AttComporatorByText : IComparer<Attribute>
    {
        public int Compare(Attribute X, Attribute Y)
        {
            if (X == null || Y == null)
            {
                if (X == null && Y == null)
                    return 0;

                if (X == null)
                    return -1;
                else
                    return 1;
            }

            if (X.word.Equals(Y.word))
                return 0;

            if (X.word.CompareTo(Y.word) > 0)
                return 1;

            return -1;

        }
    }

}
