using UnityEngine;

public class DataBinderExample : MonoBehaviour
{
    public Sprite testSprite;
    public Texture testTexture;
    DataBinder binder => DataBinder.Instance;

    private void Awake()
    {
        binder["Name"] = "Data Binder Example";
        binder["Sprite"] = testSprite;
        binder["Texture"] = testTexture;
        binder["Toggle"] = false;
        binder["Slider"] = 0.5f;
        binder["Dropdown"] = new string[] { "New", "options", "are here" };
        binder["Index"] = 0;

        binder["TurnOff"] = false;
        binder["TurnOn"] = true;

        binder.GetKeyEvent("Toggle").AddListener(DoAction);
        binder.GetKeyEvent("Slider").AddListener(DoAction);
        binder.GetKeyEvent("Index").AddListener(DoAction);
    }

    private void OnDestroy()
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
