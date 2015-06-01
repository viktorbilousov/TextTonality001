using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTonality
{
    public class FileReader
    {
        /*
         *  считывает побайтно фаил и возвращает считываемую одну строку. 
         *  запоминает индекс байта и при следующем считыванием возвращает следующую строку
         */

        public static bool EndOfStream = false;
        private string[] textStrings;
        private static char divideSymbol = '.';
        // private static string[] notReadingTags;
        private static char[] notReadingSpecSymb;
        private static long index = 0;
        private static bool isInit = false;
        private static FileStream file;

        public static void Init()
        {

            notReadingSpecSymb = new char[]
            {
                '\0', '\a', '\b', '\t', '\n', '\v', '\f', '\r'
            };

            isInit = true;
        }

        public static bool ReadFile(out string line, string path)
        {
            if (!isInit)
                Init();

            line = "";
            FileStream file = null;
            byte[] b = new byte[1];
            string symbol = "";

            try
            {
                file = new FileStream(path, FileMode.Open, FileAccess.Read);
                file.Seek(index, SeekOrigin.Begin);
                long lenghtFile = file.Length;
                while (lenghtFile > index)
                {

                    file.Read(b, 0, 1);
                    index++;


                    symbol = Encoding.Default.GetString(b);
                    bool nextSymb = false;
                    foreach (char notReadSmb in notReadingSpecSymb)
                    {
                        if (notReadSmb.ToString() == symbol)
                            nextSymb = true;
                    }
                    if (nextSymb)
                        continue;

                    line += symbol;
                    if (symbol == divideSymbol.ToString())
                    {
                        // System.Console.WriteLine(line);

                        file.Close();
                        System.Console.WriteLine(index + "\\" + lenghtFile);
                        return true;
                    }

                }

                if (lenghtFile <= index)
                    return false;

                // System.Console.WriteLine(line);
                System.Console.WriteLine(index + "\\" + lenghtFile);

                index++;
                EndOfStream = true;

            }
            catch (Exception exception)
            {
                //  Console.WriteLine(exception.Message.ToString());
                if (file != null)
                    file.Close();

                return false;
            }

            file.Close();
            return true;
        }
    }
   }

