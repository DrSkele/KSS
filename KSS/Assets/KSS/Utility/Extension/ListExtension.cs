using System.Collections.Generic;
using System;

namespace KSS.Utility
{
    public static class ListExtension
    {
        public static void Swap<T>(this List<T> list, int from, int to)
        {
            if(from < 0 || to < 0)
            {
                throw new ArgumentException("Index should not be negative");
            }

            if (list.Count < from || list.Count < to)
            {
                throw new ArgumentOutOfRangeException("Index larger than list");
            }

            (list[from], list[to]) = (list[to], list[from]);
        }

        public static List<T> Shuffle<T>(this List<T> list, int seed = 0)
        {
            var randomSeed = seed == 0 ? new Random() : new Random(seed);
            int length = list.Count;
            int randomPick = 0;

            for (int i = 0; i < length; i++)
            {
                randomPick = randomSeed.Next(i, length);
                //var temp = list[i];
                //list[i] = list[randomPick];
                //list[randomPick] = temp;
                (list[i], list[randomPick]) = (list[randomPick], list[i]);
            }
            return list;
        }
    }
}