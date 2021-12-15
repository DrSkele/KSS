using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KSS.UIBind
{
    public class UIBinder : Singleton<UIBinder>
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
        public bool GetBindedComponent<T>(string key, Type type, out T component) where T : Component
        {
            if (dic.ContainsKey(key))
            {
                if (dic[key].GetType() == type)
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
