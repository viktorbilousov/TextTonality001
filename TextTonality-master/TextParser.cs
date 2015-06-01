using System.Collections.Generic;
using System.Linq;

namespace TextTonality
{
    /// <summary>
    /// Разделяет текст на слова
    /// </summary>
    static class TextParser
    {
        private static char[] unreadableSymbols = ".,\"?\':()1234567890«»[]-\r\t\n=+*;!|—".ToArray();
        private static string EndTextString = @"<\end>";
        public static int numberOfText = 0;

        public static bool Parse(string input, ref List<Text> output)
        {
            if (input == "")
                return false;

            var word = "";
            var needIncrementNumber = false;

            input = input.ToLower();
            numberOfText++;

            for (int i = 0; i < input.Length; i++)
            {
                if (unreadableSymbols.Contains(input[i]))
                    continue;
                
                if (input[i] == ' ')
                {
                    if (word == "")
                        continue;

                    if (word == EndTextString)
                    {
                        needIncrementNumber = true;
                        word = "";
                        continue;
                    }

                    output.Add(new Text(word, numberOfText));

                    word = "";
                    continue;
                }
                word += input[i];
                if (needIncrementNumber) // иначе может прибавить из-за слов, состоящие только из знаков изключеня "1232425"
                {
                    needIncrementNumber = false;
                    numberOfText++;
                }
            }
            if (word != "")
            {
                if (needIncrementNumber)
                    numberOfText++;

                output.Add(new Text(word, numberOfText));
            }
            
            return true;
        }
    }
}
