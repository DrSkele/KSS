using UnityEngine;

public class DataBinderExample : MonoBehaviour
{
    public Sprite testSprite;
    public Texture testTexture;
    DataBinder binder => DataBinder.Instance;

    private void Awake()
    {
        binder["Name"] = "Data Binder Example";
        binder["InputField"] = "Data Binded Input";
        binder["Sprite"] = testSprite;
        binder["Texture"] = testTexture;
        binder["Toggle"] = false;
        binder["Slider"] = 0.5f;
        binder["Dropdown"] = new string[] { "New", "options", "are here" };
        binder["Index"] = 1;
        binder["Value"] = "options";

        binder["TurnOff"] = false;
        binder["TurnOn"] = false;
        binder["BindableUI"] = true;

        binder.GetKeyEvent("Toggle").AddListener(ToggleAction);
        binder.GetKeyEvent("Slider").AddListener(DoAction);
        binder.GetKeyEvent("Index").AddListener(DoAction);
        binder.GetKeyEvent("Value").AddListener(DoAction);
        binder.GetKeyEvent("InputField").AddListener(DoAction);
        
    }
    private void Start()
    {
        //binder["Index"] = 1;
    }
    private void OnDestroy()
    {
        binder.GetKeyEvent("Toggle").RemoveListener(ToggleAction);
        binder.GetKeyEvent("Slider").RemoveListener(DoAction);
        binder.GetKeyEvent("Index").RemoveListener(DoAction);
        binder.GetKeyEvent("Value").RemoveListener(DoAction);
        binder.GetKeyEvent("InputField").RemoveListener(DoAction);
    }
    private void ToggleAction(object obj)
    {
        binder["TurnOn"] = (bool)obj;
    }
    private void DoAction(object obj)
    {
        Debug.LogError(obj.ToString());
    }
}
