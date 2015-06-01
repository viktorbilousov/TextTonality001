using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTonality
{

    /// <summary>
    /// Разделяет текст на слова
    /// </summary>
    static class ParserText
    {

        private static string unreadableSymbols = ".,\"?\':()1234567890«»[]-\r\t\n=+*;!|—";
        private static string EndTextString = @"<\end>";
        public static int numberOfText = 0;

        public static bool Parse(string input, ref List<Text> output)
        {

            string word = "";
            bool lastText = false;
            if (input == "")
                return false;

            bool NeedIncrementNumber = false;

            input = input.ToLower();
            numberOfText++;

            for (int i = 0; i < input.Length; i++)
            {
                
                bool flag = false;
                foreach (char symbol in unreadableSymbols)
                {
                    char s = input[i];
                    if (input[i] == symbol)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    continue;



                if (input[i] == ' ')
                {
                    if (word == "")
                        continue;

                    if (word == EndTextString)
                    {
                        //numberOfText ++;
                        NeedIncrementNumber = true;
                        word = "";
                        continue;
                    }

                    output.Add(new Text(word, numberOfText));

                    word = "";
                    continue;
                }
                word += input[i];
                if (NeedIncrementNumber) // иначе может прибавить из-за слов, состоящие только из знаков изключеня "1232425"
                {
                    NeedIncrementNumber = false;
                    numberOfText++;
                }
            }
            if (word != "")
            {
                if (NeedIncrementNumber)
                    numberOfText++;

                output.Add(new Text(word, numberOfText));
            }




            return true;
        }
    }
}
