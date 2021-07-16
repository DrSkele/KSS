using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBinderTest : MonoBehaviour
{
    public Sprite testSprite;
    DataBinder binder;

    private void Awake()
    {
        binder = GetComponent<DataBinder>();
    }

    private void OnEnable()
    {
        binder["Name"] = "Data Binder Example";
        binder["Sprite"] = testSprite;
    }
}
