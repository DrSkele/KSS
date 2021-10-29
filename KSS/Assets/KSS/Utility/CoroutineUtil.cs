using System;
using System.Collections;
using UnityEngine;

namespace KSS
{
    public class WaitForSecondsUntil : IEnumerator
    {
        Func<bool> waitUntil;
        float timeOut;
        float current = 0;
        public WaitForSecondsUntil(Func<bool> predicate, float seconds)
        {
            waitUntil = predicate;
            timeOut = seconds;
            current = 0;
        }

        public object Current
        {
            get
            {
                current += Time.deltaTime;
                return current;
            }
        }

        public bool MoveNext()
        {
            return !(waitUntil() || current > timeOut);
        }

        public void Reset()
        {
            current = 0;
        }
    }
}