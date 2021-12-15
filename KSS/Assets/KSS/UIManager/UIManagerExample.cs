using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KSS.UIBind; 
public class UIManagerExample : MonoBehaviour
{
    UIBinder binder = new UIBinder();

    private void Test()
    {
        if(binder.GetBindedComponent("key", typeof(Text), out Text text))
        {

        }
    }
}
