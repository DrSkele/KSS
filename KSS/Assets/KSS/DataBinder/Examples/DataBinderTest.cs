using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBinderTest : MonoBehaviour
{
    public Sprite testSprite;
    public Texture testTexture;
    DataBinder binder;

    private void Awake()
    {
        binder = DataBinder.Init(this.gameObject);
    }

    private void OnEnable()
    {
        binder["Name"] = "Data Binder Example";
        binder["Sprite"] = testSprite;
        binder["Texture"] = testTexture;
        binder["Toggle"] = false;
        binder["Slider"] = 0.5f;
        binder["Dropdown"] = new string[] { "New", "options", "are here" };
        binder["Index"] = 0;

        binder.GetKeyEvent("Toggle").AddListener(DoAction);
        binder.GetKeyEvent("Slider").AddListener(DoAction);
        binder.GetKeyEvent("Index").AddListener(DoAction);
    }

    private void OnDisable()
    {
        binder.GetKeyEvent("Toggle").RemoveListener(DoAction);
        binder.GetKeyEvent("Slider").RemoveListener(DoAction);
        binder.GetKeyEvent("Index").RemoveListener(DoAction);
    }

    private void DoAction(object obj)
    {
        Debug.LogError(obj.ToString());
    }
}
