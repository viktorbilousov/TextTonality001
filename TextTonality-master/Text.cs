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

        public Text(string text, int number)
        {
            if (number < 0)
            {
                this.text = "";
                numberOfText = -1;
                return;
            }

            this.text = text;
            numberOfText = number;
        }
    }
}
