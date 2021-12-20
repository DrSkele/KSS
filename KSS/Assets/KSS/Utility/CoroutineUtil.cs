using System;
using System.Collections;
using UnityEngine;

namespace KSS.Utility.Coroutine
{
    /// <summary>
    /// Wait until given condition is met, or given seconds has passed.
    /// </summary>
    public class WaitForSecondsUntil : IEnumerator
    {
        Func<bool> waitUntil;
        float timeout;
        float current = 0;
        public WaitForSecondsUntil(Func<bool> predicate, float timeoutSeconds)
        {
            waitUntil = predicate;
            timeout = timeoutSeconds;
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
            return !(waitUntil() || current > timeout);
        }

        public void Reset()
        {
            current = 0;
        }
    }

    public class WaitUntilWithMinimumSeconds : IEnumerator
    {
        Func<bool> waitUntil;
        float minimum;
        float current = 0;
        public WaitUntilWithMinimumSeconds(Func<bool> predicate, float minimumSeconds)
        {
            waitUntil = predicate;
            minimum = minimumSeconds;
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
            return !(waitUntil() && current > minimum);
        }

        public void Reset()
        {
            current = 0;
        }
    }
}