using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KSS.UIFind
{
    public class UIFinder : Singleton<UIFinder>
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();

        public void RegisterComponent(string key, object obj)
        {
            dic.Add(key, obj);
        }
        public void UnregisterComponent(string key)
        {
            dic.Remove(key);
        }
        public bool FindComponent<T>(string key, out T component) where T : Component
        {
            if (dic.ContainsKey(key))
            {
                if (dic[key].GetType() == typeof(T))
                {
                    component = (T)dic[key];
                    return true;
                }
            }
            component = default(T);
            return false;
        }


    }
}
