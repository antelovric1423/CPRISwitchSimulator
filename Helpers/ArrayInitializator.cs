using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPRISwitchSimulator.Helpers
{
    public class ArrayInitializator
    {
        public static T[] InitializeArray<T>(uint length) where T : new()
        {
            T[] array = new T[length];
            for (uint i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }
    }
}
