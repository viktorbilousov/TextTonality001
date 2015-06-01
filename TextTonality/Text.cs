using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTonality
{
    class Text
    {
        public string text { get; set; }
        public int numberOfText { get; set; }

        public Text()
        {
            text = "";
            numberOfText = -1;
        }

        public Text(string _text, int number)
        {
            if (number < 0)
            {
                text = "";
                numberOfText = -1;
                return;
            }

            text = _text;
            numberOfText = number;
        }


    }
}
