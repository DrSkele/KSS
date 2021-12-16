using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSS.UIFind
{
    public interface IBindableUI
    {

    }
    
    public class BindableUI : MonoBehaviour, IBindableUI
    {
        string key;
        Component comp;

        private void OnEnable()
        {
            UIFinder.Instance.RegisterComponent(key, comp);
        }
        private void OnDisable()
        {
            UIFinder.Instance.UnregisterComponent(key);
        }
    }
}