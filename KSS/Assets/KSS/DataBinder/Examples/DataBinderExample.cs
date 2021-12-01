using UnityEngine;
using KSS.DataBind;

public class DataBinderExample : MonoBehaviour
{
    public Sprite testSprite;
    public Texture testTexture;
    DataBinder binder => DataBinder.Instance;

    private void OnEnable()
    {
        //bindable activator
        binder["BindableUI"] = true;
        //text
        binder["Name"] = "Data Binder Example";
        //inputfield
        binder["InputField"] = "Data Binded Input";
        //image
        binder["Sprite"] = testSprite;
        binder["Fill"] = 0.5f;
        //raw image
        binder["Texture"] = testTexture;
        //toggle group
        binder["Group"] = 1;
        //toggle
        binder["Toggle"] = false;
        //slider
        binder["Slider"] = 0.55f;
        //dropdown
        binder["Dropdown"] = new string[] { "New", "options", "are here" };
        binder["Index"] = 1;
        binder["Value"] = "options";
        
        //bindable activator
        binder["TurnOff"] = false;
        binder["TurnOn"] = false;

        binder.GetKeyEvent("Toggle").AddListener(ToggleAction);
        binder.GetKeyEvent("Slider").AddListener(DoAction);
        binder.GetKeyEvent("Index").AddListener(DoAction);
        binder.GetKeyEvent("Value").AddListener(DoAction);
        binder.GetKeyEvent("Group").AddListener(DoAction);
        binder.GetKeyEvent("InputField").AddListener(DoAction);
    }
    private void Start()
    {
        //binder["Index"] = 1;
    }
    private void OnDestroy()
    {
        //if (DataBinder.IsQuit) return;
        binder?.GetKeyEvent("Toggle").RemoveListener(ToggleAction);
        binder?.GetKeyEvent("Slider").RemoveListener(DoAction);
        binder?.GetKeyEvent("Index").RemoveListener(DoAction);
        binder?.GetKeyEvent("Value").RemoveListener(DoAction);
        binder?.GetKeyEvent("InputField").RemoveListener(DoAction);
    }
    private void ToggleAction(object obj)
    {
        binder["TurnOn"] = (bool)obj;
        binder["Interactable"] = (bool)obj;
    }
    private void DoAction(object obj)
    {
        Debug.LogError(obj.ToString());
    }
}
