using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TextTonality
{
    /// <summary>
    /// Словарь признаков
    /// </summary>
    class DictinaryAttribute
    {

        private List<Attribute>[] _attributeslList;  // масив, j-й елемент - список признаков сост из словосочитаний 
        // (которые состоят из j-1 слов)
        private List<ClassTypeValue> _TextType;

        public List<Attribute>[] AttributesListArrow
        {
            get { return _attributeslList; }
        }

        public List<ClassTypeValue> TextTypeList
        {
            get { return _TextType; }
        }

        private readonly int DELETE_MIN_LEVEL_DEF = 5; // Минимальное количество вхождение признака в глобальный текст 
        private readonly int MAX_LEIGHT_OF_WORDS_IN_COLLOCATION = 3; // максимальная количество слов в словосочитаниях
        private readonly string nameOfDictinaryFile = "Dictinary";
        private readonly string EndOfText = @"<\end>"; // символ окончания текста

        // далее признаки = атрибуты // 


        public DictinaryAttribute()
        {
            _attributeslList = new List<Attribute>[MAX_LEIGHT_OF_WORDS_IN_COLLOCATION];
            _TextType = new List<ClassTypeValue>();
            //   _svm = new SVM();

            for (int i = 0; i < _attributeslList.Length; i++)
                _attributeslList[i] = new List<Attribute>();


        }

        /// <summary>
        /// Добавляет текст в список признаков
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="classType">Класс текста</param>
        /// <returns></returns>
        public bool AddText(string text, ClassTypeValue classType)
        {
            Console.WriteLine("Start add words to the dictionary");


            List<Text> wordList = new List<Text>();

            if (!ParserText.Parse(text, ref wordList))
                Console.WriteLine(this.ToString() + "Error AddText : Empty input File");


            int cntNewText = wordList.Last().numberOfText; // сумарное количество текстов 
            ParserText.numberOfText = cntNewText;

            while (cntNewText - _TextType.Count > 0)
                _TextType.Add(classType);


            // добавляем все слова в список
            Console.WriteLine("add words [1]");
            for (int i = 0; i < wordList.Count; i++)
            {
                if ((i + 1) % 10000 == 0)
                    Console.WriteLine("{0}\\{1}", (i + 1), wordList.Count);

                AddWord(wordList[i], 0, classType);
            }

            Console.WriteLine("{0}\\{1}", wordList.Count, wordList.Count);


            //составляем все словосочитания определенной размерности и заносим их в список

            bool exit = false;
            string word = "";

            for (int i = 1; i < MAX_LEIGHT_OF_WORDS_IN_COLLOCATION; i++)
            {
                Console.WriteLine("add words [{0}]", (i + 1));

                for (int j = 0; j < wordList.Count; j++)
                {
                    if ((j + 1) % 10000 == 0)
                        Console.WriteLine("{0}\\{1}", (j + 1), wordList.Count);

                    int tmpNum = wordList[j].numberOfText;

                    for (int k = 0; k <= i; k++)
                    {
                        if (j + k >= wordList.Count)
                        {
                            exit = true;
                            break;
                        }

                        if (wordList[j + k].numberOfText == tmpNum)
                            word += wordList[j + k].text;
                    }

                    AddWord(new Text(word, tmpNum), i, classType);
                    word = "";

                }
                Console.WriteLine("{0}\\{1}", wordList.Count, wordList.Count);

            }

            Console.WriteLine("Finish add words to the dictionary");
            return true;
        }

        /// <summary>
        /// Добавляет слово или словосочитанием, который станет одним атрибутом
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="leightOfCollation">Длина словосочитание (1 - слово)</param>
        /// <param name="classType">Класс текста</param>
        public void AddWord(Text text, int leightOfCollation, ClassTypeValue classType)
        {
            //todo : оптимизировать поиск. Возможно удалять слова, которые повторяются
            // хотя можно и забить. так оно выглядит только круче 
            

            int indexList = leightOfCollation;
            string word = text.text;

            if (word != "я" && word.Length == 1)
                return;

            int index = _attributeslList[indexList].FindIndex(x => x.word == word);

            if (index < 0)
            {
                Attribute attr = new Attribute(word, classType);
                attr.SetNumberText(text.numberOfText);
                _attributeslList[indexList].Add(attr);

                //    Console.WriteLine( "добавлено " + word );
            }
            else
            {
                _attributeslList[indexList][index].incrementCnt(classType);
                _attributeslList[indexList][index].SetNumberText(text.numberOfText);
            }
            return; ;

        }


        /// <summary>
        /// Сохраняет список признаков определенной длинны в фаил
        /// </summary>
        /// <param name="path">Путь к ФАЙЛУ, где будет сознданы фаилы</param>
        /// <param name="LeightOfCollaction">длина словосочитания (1 - слово)</param>
        /// <param name="writeNew">если True - то записывает новый</param>
        /// <returns></returns>
        private bool SaveDictinaryToFile(string path, int LeightOfCollaction, bool writeNew)
        {
            //todo : проверить работу с writeNew = false. Он должен дописывать слова в фаил, а не переписывать его 


            if (LeightOfCollaction > MAX_LEIGHT_OF_WORDS_IN_COLLOCATION || LeightOfCollaction < 1)
                return false;

            LeightOfCollaction--;

            int index = LeightOfCollaction;
            using (StreamWriter writer = new StreamWriter(path, !writeNew))
            {
                string line = "";
                if (writeNew)
                {
                    string firstLine = "# number of words: " + _attributeslList[index].Count;
                    //  writer.WriteLine(firstLine);
                    firstLine = "# Words\t\t";  //'#' - не убирать !

                    for (int i = 0; i < ClassType.cnt; i++)
                        firstLine += ClassType.ClassTypeStringList[i] + "\t";

                    writer.WriteLine(firstLine);
                }


                foreach (Attribute attribute in _attributeslList[index])
                {
                    line = attribute.word + "\t";
                    if (line.Length < 10) // что бы ровненько
                        line += "\t";
                    for (int i = 0; i < ClassType.cnt; i++)
                    {
                        //   line += "\t" + attribute.count[i] + ": ";
                        line += attribute.weight[i].ToString("F5") + "\t\t";
                    }
                    writer.WriteLine(line);
                }
            }

            return true;
        }
        /// <summary>
        /// Загружает фаилы списка и записывает их в класс
        /// </summary>
        /// <param name="path">Путь к ПАПКЕ и файлами</param>
        /// <returns></returns>
        public bool LoadDictinaryFrom(string path)
        {
            Console.WriteLine("Start Load Dictinary");
            string[] pathToDic = new string[MAX_LEIGHT_OF_WORDS_IN_COLLOCATION];
            if (path[path.Length - 1] != '\\')
                path += '\\';

            for (int i = 0; i < pathToDic.Length; i++)
                pathToDic[i] = path + nameOfDictinaryFile + (i + 1).ToString() + ".txt";

            bool flag = true;

            for (int i = 0; i < MAX_LEIGHT_OF_WORDS_IN_COLLOCATION; i++)
            {
                if (!LoadDictinaryFromFile(pathToDic[i], i + 1))
                    flag = false;
            }

            Console.WriteLine("End Load Dictinary");

            return flag;

        }

        /// <summary>
        /// Загружает фаил списка 
        /// </summary>
        /// <param name="path">Путь к ФАЙЛУ</param>
        /// <param name="leightOFCallaction">Длинна загружаемых словосочитаний (1 - слово)</param>
        /// <returns></returns>
        private bool LoadDictinaryFromFile(string path, int leightOFCallaction)
        {
            // если строка начиначется с '#', то не считает ее

            Console.WriteLine("Load Dictinary[{0}]", leightOFCallaction);
            if (leightOFCallaction > MAX_LEIGHT_OF_WORDS_IN_COLLOCATION || leightOFCallaction < 1)
                return false;

            leightOFCallaction--;

            string word = "";
            float[] weight;
            bool flag = false;


            try
            {
                StreamReader Reader = new StreamReader(path);

                while (true)
                {

                    string line = Reader.ReadLine();

                    if (line == null)
                        break;

                    if (ReadStreamParse(line, out word, out weight))
                    {
                        _attributeslList[leightOFCallaction].Add(new Attribute(word, weight));
                        flag = true;
                    }
                }
                Reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

            int cnt = _attributeslList[leightOFCallaction].Count;

            Console.WriteLine("Dictinary[{0}] : {1}", leightOFCallaction + 1, cnt);


            return flag;
        }

        /// <summary>
        /// Выделяет из строки параметры признаков
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <param name="word">Текст признака</param>
        /// <param name="weight">Веса признаков по классам</param>
        /// <returns></returns>
        private bool ReadStreamParse(string input, out string word, out float[] weight)
        {

            string[]  result = new string[ClassType.cnt + 1]; // +1  на слово .
            weight = new float[ClassType.cnt];

            string cutLine = "";
            char symb;
            word = "";

            if (input.Length == 0)
                return false;

            if (input[0] == '#')
                return false;


            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] = float.NaN;
            }


            int index = 0;

            for (int i = 0; i < input.Length; i++)
            {
                symb = input[i];
                if (symb == '\t')
                {
                    if (cutLine == "")
                        continue;
                    else
                    {
                        result[index++] = cutLine;
                        cutLine = "";
                        continue;
                    }
                }
                cutLine += symb;
            }

            word = result[0];
            bool flag = true;

            for (int i = 0; i < weight.Length; i++)
            {
                if (!float.TryParse(result[i + 1], out weight[i]))
                    flag = false;
            }

            return flag;
        }
        /// <summary>
        /// Сохраняет все списки признаков в файлы
        /// </summary>
        /// <param name="path">Путь к ПАПКЕ</param>
        /// <param name="writeNew">true - перезаписать заново</param>
        public void SaveAllTo(string path, bool writeNew)
        {
            Console.WriteLine("Start safe to file");

            string newPathToFile = path + @"\" + nameOfDictinaryFile;
            for (int i = 0; i < _attributeslList.Length; i++)
            {
                SaveDictinaryToFile(
                    newPathToFile + (i + 1).ToString() + ".txt",
                    i + 1,
                    writeNew
                    );
            }

            Console.WriteLine("Finish safe to file");

        }


        /// <summary>
        /// Удаляет слова встерчающие меньше 
        /// минимального значения по умолчанию
        /// </summary>
        public void DeleteScareWord()
        {
            DeleteScareWord(DELETE_MIN_LEVEL_DEF);
        }
        /// <summary>
        ///  Удаляет редко встречающие слова 
        /// </summary>
        /// <param name="minLevel">Минимальный порош</param>
        private void DeleteScareWord(int minLevel) // ранее был public
        {
            Console.WriteLine("Start delete scare words");

            for (int k = 0; k < _attributeslList.Length; k++)
            {

                for (int i = 0; i < _attributeslList[k].Count; i++)
                {
                    bool flag = true;
                    for (int j = 0; j < ClassType.cnt; j++)
                    {

                        if (_attributeslList[k][i].count[j] >= minLevel)
                            flag = false;

                    }

                    if (flag)
                    {
                        _attributeslList[k].RemoveAt(i);
                        i--;
                    }
                }
            }

            Console.WriteLine("Finish delete scare words");

            return;

        }
        /// <summary>
        /// Сортирует список
        /// </summary>
        public void SortList()
        {
            Console.WriteLine("Start sort words");

            AttComporatorByText att = new AttComporatorByText();

            foreach (List<Attribute> attribute in _attributeslList)
            {
                attribute.Sort(att);
            }

            Console.WriteLine("Finish sort words");

        }

        /// <summary>
        /// Считает глобальные веса всех атрибутов
        /// </summary>
        public void CalcAllWeigth()
        {
            for (int i = 0; i < _attributeslList.Length; i++)
            {
                foreach (Attribute attribute in _attributeslList[i])
                    attribute.CaclWeight();

            }
            return;
        }

        /// <summary>
        /// Создает словарь из файлов со списками
        /// </summary>
        /// <param name="TextPath">Пути к файлам с файлами</param>
        /// <param name="Type">Классы файлов</param>
        public void BildNewDictinary(string[] TextPath, ClassTypeValue[] Type)
        {
            string text = "";
            for (int i = 0; i < TextPath.Length; i++)
            {
                try
                {
                    StreamReader Text = new StreamReader(TextPath[i], Encoding.GetEncoding(1251));
                    text = Text.ReadToEnd();
                    Text.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                AddText(text, Type[i]);
            }


            DeleteScareWord();
            SortList();
            CalcAllWeigth();
            SaveAllTo(@"C:\001\002", true);
        }

    }

}
