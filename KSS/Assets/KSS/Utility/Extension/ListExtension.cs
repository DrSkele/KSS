using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }
}