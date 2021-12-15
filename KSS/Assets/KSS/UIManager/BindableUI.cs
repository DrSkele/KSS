using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSS.UIBind
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
            UIBinder.Instance.RegisterComponent(key, comp);
        }
        private void OnDisable()
        {
            UIBinder.Instance.UnregisterComponent(key);
        }
    }
}