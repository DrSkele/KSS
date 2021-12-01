using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BindedButton : MonoBehaviour, bindedUI<Text>
{

    [SerializeField] InterfaceTest test;

    [SerializeField] Text text;

    private void Awake()
    {
        test.uis = this;
    }

    public Text GetUI()
    {
        return text;
    }
    public Type GetUIType()
    {
        return typeof(Text);
    }
}