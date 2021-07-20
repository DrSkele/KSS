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
    }
}
