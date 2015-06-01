using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextTonality
{
    /// <summary>
    /// Словарь признаков
    /// </summary>
    class DictinaryAttribute
    {

        private List<Attribute> _attributeslList;  // масив, j-й елемент - список признаков сост из словосочитаний 
                                                     // (которые состоят из j-1 слов)
        private List<ClassTypeValue> _TextType;

        private bool _isTraning = true;


        public int CountList
        {
            get { return _attributeslList.Count; }
        }
        public bool isTraning
        {
            get { return _isTraning; }
        }

        public List<Attribute> AttributesList
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

        private readonly string outputPath = @"C:\001\002";


        // далее признаки = атрибуты // 


        public DictinaryAttribute(bool isTraningDict)
        {
            _attributeslList = new List<Attribute>();
            _TextType = new List<ClassTypeValue>();
            _isTraning = isTraningDict;

        }

        /// <summary>
        /// Добавляет текст в список признаков
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="classType">Класс текста</param>
        /// <returns></returns>
        public bool AddText(string text, ClassTypeValue classType)
        {
            ConsoleText.WriteLine("Start add words to the dictionary");

            var wordList = new List<Text>();

            if (!TextParser.Parse(text, ref wordList))
                ConsoleText.WriteLine(this + "Error AddText : Empty input File");

            var cntNewText = wordList.Last().numberOfText; // сумарное количество текстов 
            TextParser.numberOfText = cntNewText;

            if (!isTraning)
                cntNewText = 1;
            
            while (cntNewText - _TextType.Count > 0)
                _TextType.Add(classType);

            // добавляем все слова в список
            ConsoleText.WriteLine("add words [1]");
            for (int i = 0; i < wordList.Count; i++)
            {
                if ((i + 1) % 10000 == 0)
                    ConsoleText.WriteLine((i + 1).ToString() + "\\" + wordList.Count);

                AddWord(wordList[i], 0, classType);
            }

            ConsoleText.WriteLine(wordList.Count + "\\" + wordList.Count);

            //составляем все словосочитания определенной размерности и заносим их в список

            var exit = false;
            var word = "";

            for (var i = 1; i < MAX_LEIGHT_OF_WORDS_IN_COLLOCATION; i++)
            {
                ConsoleText.WriteLine("add words [" + (i + 1) + "]");

                for (int j = 0; j < wordList.Count; j++)
                {
                    if ((j + 1) % 10000 == 0)
                        ConsoleText.WriteLine((j + 1) +"\\"+ wordList.Count);

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
                ConsoleText.WriteLine( wordList.Count +"\\"+ wordList.Count);

            }

            if(isTraning)
                 foreach (var att in _attributeslList)
                {
                    att.setLeightIsInText(_TextType.Count);
                }

            ConsoleText.WriteLine("Finish add words to the dictionary");
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
            
            var indexList = leightOfCollation;
            var word = text.text;

            if (word != "я" && word.Length == 1)
                return;

            var index = _attributeslList.FindIndex(x => x.word == word);

            if (index < 0)
            {
                Attribute attr;
                if( classType == ClassTypeValue.Unknown)
                    attr = new Attribute(word);
                else 
                    attr = new Attribute(word, classType);

                 attr.SetNumberText(text.numberOfText);
                _attributeslList.Add(attr);

                //    ConsoleText.WriteLine( "добавлено " + word );
            }
            else
            {
                _attributeslList[index].incrementCnt(classType);
                _attributeslList[index].SetNumberText(text.numberOfText);
            }
        }

        /// <summary>
        /// Сохраняет список признаков определенной длинны в фаил
        /// </summary>
        /// <param name="path">Путь к ФАЙЛУ, где будет сознданы фаилы</param>
        /// <param name="LeightOfCollaction">длина словосочитания (1 - слово)</param>
        /// <param name="writeNew">если True - то записывает новый</param>
        /// <returns></returns>
        private bool SaveDictinaryToFile(string path,  bool writeNew)
        {
            //todo : проверить работу с writeNew = false. Он должен дописывать слова в фаил, а не переписывать его 

           
            using (var writer = new StreamWriter(path, !writeNew))
            {
                var line = "";
                if (writeNew)
                {
                    var firstLine = "# number of words: " + _attributeslList.Count;
                    //  writer.WriteLine(firstLine);
                    firstLine = "# Words\t\t";  //'#' - не убирать !

                    for (int i = 0; i < ClassType.cnt; i++)
                        firstLine += ClassType.ClassTypeStringList[i] + "\t";

                    writer.WriteLine(firstLine);
                }

                line = "%";
                foreach (ClassTypeValue typeValue in _TextType)
                {
                    line += (int) typeValue;
                }

                writer.WriteLine(line);
                
                foreach (var attribute in _attributeslList)
                {
                    line = attribute.word + "\t";
                    if (line.Length < 10) // что бы ровненько
                        line += "\t";
                    for (var i = 0; i < attribute.weight.Length; i++)
                    {
                        //   line += "\t" + attribute.count[i] + ": ";
                        line += attribute.weight[i].ToString("F5") + "\t\t";
                    }

                    for (int i = 0; i < attribute.isInText.Count; i++)
                    {
                        if (attribute.isInText[i])
                            line += "1";
                        else
                            line += "0";
                    }
                    line += "\t";
                    
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
        //private bool LoadDictinaryFrom(string path)
        //{
        //    ConsoleText.WriteLine("Start Load Dictinary");
        //    string[] pathToDic = new string[MAX_LEIGHT_OF_WORDS_IN_COLLOCATION];
        //    if (path[path.Length - 1] != '\\')
        //        path += '\\';

        //    for (int i = 0; i < pathToDic.Length; i++)
        //        pathToDic[i] = path + nameOfDictinaryFile + (i + 1).ToString() + ".txt";

        //    bool flag = true;

        //    for (int i = 0; i < MAX_LEIGHT_OF_WORDS_IN_COLLOCATION; i++)
        //    {
        //        if (!LoadDictinaryFromFile(pathToDic[i], i + 1))
        //            flag = false;
        //    }

        //    ConsoleText.WriteLine("End Load Dictinary");

        //    return flag;
        //}

        /// <summary>
        /// Загружает фаил списка 
        /// </summary>
        /// <param name="path">Путь к ФАЙЛУ</param>
        /// <param name="leightOFCallaction">Длинна загружаемых словосочитаний (1 - слово)</param>
        /// <returns></returns>
        public bool LoadDictinaryFromFile(string path)
        {
            // если строка начиначется с '#', то не считает ее

      
            var word = "";
            var flag = false;
            float[] weight;
            List<bool> isInText;

            try
            {
                var reader = new StreamReader(path);

                while (true)
                {

                    string line = reader.ReadLine();

                    if (line == null)
                        break;

                    if (ReadStreamParse(line, out word, out weight, out isInText))
                    {
                        _attributeslList.Add(new Attribute(word, weight, isInText));
                       flag = true;
                    }
                }
                reader.Close();
            }
            catch (Exception e)
            {
                ConsoleText.WriteLine(e.ToString());
            }



            int cnt = _attributeslList.Count;

            ConsoleText.WriteLine("Dictinary" + ":" + cnt);


            return flag;
        }

        /// <summary>
        /// Выделяет из строки параметры признаков
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <param name="word">Текст признака</param>
        /// <param name="weight">Веса признаков по классам</param>
        /// <returns></returns>
        private bool ReadStreamParse(string input, out string word, out float[] weight, out List<bool> isInText  )
        {

            string[]  result = new string[ClassType.cnt + 2]; // +1  на слово и +1 на вектор .
            string TypeOfText = "";
            weight = new float[ClassType.cnt];
            isInText = new List<bool>();
            bool FirstLine = true;

            string cutLine = "";
            char symb;
            word = "";

            if (input.Length == 0)
                return false;

            if (input[0] == '#')
                return false;
            if (input[0] == '%')
            {
                TypeOfText = input.Replace("%","");
               
                int type = -1; 
                for (int i = 0; i < TypeOfText.Length; i++)
                {
                    if (int.TryParse(TypeOfText[i].ToString(), out type))
                    {
                        _TextType.Add((ClassTypeValue)type);
                    }
                    else 
                        ConsoleText.WriteLine("Error read TextType");
                }
                return false;

            }
           


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
            for (int i = 0; i < result[result.Length-1].Length; i++)
            {
                if (result[result.Length - 1][i] == '0')
                    isInText.Add(false);
                else 
                    isInText.Add(true);

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
            ConsoleText.WriteLine("Start safe to file");

            
                var dicName = nameOfDictinaryFile + ".txt";
                var dictionary = Path.Combine(path, dicName);

                SaveDictinaryToFile(dictionary, writeNew);
            

            ConsoleText.WriteLine("Finish safe to file");
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
            ConsoleText.WriteLine("Start delete scare words");
            
                for (int i = 0; i < _attributeslList.Count; i++)
                {
                    bool flag = true;
                    for (int j = 0; j < _attributeslList[i].count.Length; j++)
                    {

                        if (_attributeslList[i].count[j] >= minLevel)
                            flag = false;

                    }

                    if (flag)
                    {
                        _attributeslList.RemoveAt(i);
                        i--;
                    }
                }
            

            ConsoleText.WriteLine("Finish delete scare words");

            return;

        }
        /// <summary>
        /// Сортирует список
        /// </summary>
        public void SortList()
        {
            ConsoleText.WriteLine("Start sort words");

            AttComporatorByText att = new AttComporatorByText();

          
                this.AttributesList.Sort(att);
            

            ConsoleText.WriteLine("Finish sort words");

        }

        /// <summary>
        /// Считает глобальные веса всех атрибутов
        /// </summary>
        public void CalcAllWeigth()
        {
          
              foreach (Attribute attribute in _attributeslList)
                attribute.CaclWeight();

            
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
                    var reader = new StreamReader(TextPath[i], Encoding.GetEncoding(1251));
                    text = reader.ReadToEnd();
                    reader.Close();
                }
                catch (Exception e)
                {
                    ConsoleText.WriteLine(e.ToString());
                }

                AddText(text, Type[i]);
            }
            if (!isTraning) 
            {
                // если выборка, класс которой мы ищем имеет мало слов
                if(this.AttributesList.Count < 100)
                    DeleteScareWord(1);
                else 
                    for (int i = 1; i < DELETE_MIN_LEVEL_DEF; i++)
                    {
                        if (this.AttributesList.Count < 1000*i)
                        {
                            DeleteScareWord(i);
                            break;
                        }
                    }
              
            }
            else 
                 DeleteScareWord();

            SortList();
            CalcAllWeigth();

            if(isTraning)
                SaveAllTo(outputPath, true);
        }

        

        public void ClearAllWeight()
        {
            
                foreach (Attribute attribute in AttributesList)
                {
                    attribute.clearAllWeight();
                }
            
        }

        public void ResetAllCnt()
        {
            foreach (Attribute attribute in _attributeslList)
            {
                attribute.resetAllCnt();
            }
        }

        public void PrepareTextForRecognition(DictinaryAttribute dictinary)
        {
            int indexUndefClass = 0;
            DictinaryAttribute result = new DictinaryAttribute(false);
            result.CopyWordsOnlyAsUndefClass(dictinary);
            
            foreach (Attribute Attr in result.AttributesList)
            {
                  Attr.weight[indexUndefClass] = 1;
            }

            for (int i = 0; i < result.AttributesList.Count; i++)
            {
                string word = result.AttributesList[i].word;
                var index = _attributeslList.FindIndex(x => x.word == word);
                if (index >= 0)
                {
                  //  ConsoleText.WriteLine(index.ToString());
                    result.AttributesList[i].weight[indexUndefClass] =
                        _attributeslList[index].weight[indexUndefClass];

                    result.AttributesList[i].isInText[0] = true;

                }
            }
            Copy(result);
            
        }

        public void LoadTextToRecognition(string[] path)
        {
            
            ClassTypeValue[] unknowType = new ClassTypeValue[path.Length];
            for (int i = 0; i < unknowType.Length; i++)
            {
                unknowType[i] = ClassTypeValue.Unknown; 
            }
            
            BildNewDictinary(path, unknowType);
        }

        public void Copy(DictinaryAttribute dictinary)
        {
            _attributeslList.Clear();

            foreach (Attribute attribute in dictinary.AttributesList)
            {
                _attributeslList.Add(attribute);
            }
        }
        /// <summary>
        /// Копирует все слова атрибутов как слово с неизвестным классом
        /// </summary>
        /// <param name="dictinary">откуда копирывать</param>
        public void CopyWordsOnlyAsUndefClass(DictinaryAttribute dictinary)
        {
            this._attributeslList.Clear();

            for (int i = 0; i < dictinary.CountList; i++)
            {
                Attribute tmpAtr = new Attribute(dictinary._attributeslList[i].word);
                tmpAtr.isInText[0] = false;
               _attributeslList.Add(tmpAtr);
                
            }
            

        }
        
    }

}
