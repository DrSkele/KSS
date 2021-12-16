using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KSS.UIFind; 
public class UIManagerExample : MonoBehaviour
{
    UIFinder binder = new UIFinder();

    private void Test()
    {
        binder.FindComponent("key", out Text text);
    }
}
