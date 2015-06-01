using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTonality
{

    // todo: переписать enum (на список) и класс, что бы можно было добавлять любое количество классов
    public enum ClassTypeValue
    {
        Unknown =  -1, 
        Negative = 0,
        Positive = 1
    };

    static class ClassType
    {
        public static readonly int cnt = 2; // количество типов как таковых без "unknown"

        public static readonly string[] ClassTypeStringList = new[]
        {
            //"Unkonow",
            "Negative",
            "Positive"
        };

        //public static readonly int Unknown = -1;
        //public static readonly int Negative = 0;
        //public static readonly int Positive = 1;


        public static string ToString(int value)
        {


            if (value == (int)ClassTypeValue.Negative)
                return ClassTypeStringList[(int)ClassTypeValue.Negative];

            if (value == (int)ClassTypeValue.Positive)
                return ClassTypeStringList[(int)ClassTypeValue.Positive];

            return "Unknow";

        }

        public static string ToString(ClassTypeValue type)
        {
            return ClassTypeStringList[(int)type];

        }
    }



}
