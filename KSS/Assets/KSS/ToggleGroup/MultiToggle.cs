using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using KSS.Utility;
using System.Linq;

public class MultiToggleEvent : UnityEvent<int, bool> { }
public class MultiToggle : MonoBehaviour
{
    [SerializeField] [Range(0, int.MaxValue)]int minToggleOn;
    [SerializeField] [Range(1, int.MaxValue)]int maxToggleOn;
    [SerializeField] List<Toggle> toggles = new List<Toggle>();

    Queue<Toggle> activeToggles = new Queue<Toggle>();

    public MultiToggleEvent OnValueChanged = new MultiToggleEvent();
    public List<Toggle> AllToggles => toggles;
    public int ToggleCount => toggles.Count;
    public int ActiveCount => activeToggles.Count;
    private void Start()
    {
        Initialize();
        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(isOn => ManipulateToggle(index, isOn));
        }
    }
    private void Initialize()
    {
        if (toggles.Count == 0)
            toggles.AddRange(GetComponentsInChildren<Toggle>());

        if(ActiveCount < minToggleOn)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].isOn)
                    continue;

                toggles[i].isOn = true;
                activeToggles.Enqueue(toggles[i]);

                if (ActiveCount == minToggleOn)
                    break;
            }
        }
        if(maxToggleOn < ActiveCount)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (!toggles[i].isOn)
                    continue;

                toggles[i].isOn = false;

                if (ActiveCount == maxToggleOn)
                    break;
            }
        }
    }
    private void ManipulateToggle(int index, bool isOn)
    {
        if(isOn)
        {
            if(maxToggleOn == minToggleOn && ActiveCount > 0)
            {
                activeToggles.Enqueue(toggles[index]);
                OnValueChanged.Invoke(index, isOn);
                activeToggles.Dequeue().isOn = false;
            }
            if(ActiveCount <= maxToggleOn)
            {
                activeToggles.Enqueue(toggles[index]);
                OnValueChanged.Invoke(index, isOn);
            }
            else
            {
                toggles[index].isOn = false;
            }
        }
        else
        {
            if(minToggleOn <= ActiveCount)
            {
                OnValueChanged.Invoke(index, isOn);
            }
            else
            {
                toggles[index].isOn = true;
            }
        }
    }
    public void AddToggle(Toggle toggle)
    {
        toggles.Add(toggle);
    }
    public void AddToggleAt(Toggle toggle, int index)
    {
        if (toggles.Count < index)
            throw new ArgumentOutOfRangeException($"Index : {index} is larger than number of toggles : {toggles.Count}");
        else if(toggles.Count == index)
            toggles.Add(toggle);
        else
            toggles.Insert(index, toggle);
    }
    public void RemoveToggle(Toggle toggle)
    {
        toggles.Remove(toggle);
    }
    public void RemoveToggleAt(int index)
    {
        toggles.RemoveAt(index);
    }
}
