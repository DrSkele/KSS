using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public interface bindedUI<out T> 
{
    T GetUI();
    Type GetUIType();
    
}
public class InterfaceTest : MonoBehaviour
{
    public bindedUI<UIBehaviour> uis;

    private void Start()
    {
        Debug.Log(uis.GetUIType());
    }
}
