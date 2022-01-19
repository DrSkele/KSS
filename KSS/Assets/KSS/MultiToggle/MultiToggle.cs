using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KSS.MultiToggle
{
    public class MultiToggleEvent : UnityEvent<int, bool> { }
    public class MultiToggle : MonoBehaviour
    {
        [SerializeField] [Range(0, 100)] int minToggleOn;
        [SerializeField] [Range(1, 100)] int maxToggleOn;
        [SerializeField] List<Toggle> toggles = new List<Toggle>();

        QueueableList<Toggle> activeToggles = new QueueableList<Toggle>();
        Toggle invalidToggle;

        public MultiToggleEvent OnValueChanged = new MultiToggleEvent();
        public List<Toggle> AllToggles => toggles;
        public int ToggleCount => toggles.Count;
        public int ActiveCount => activeToggles.Count;
        private void Awake()
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

            foreach (var toggle in toggles)
            {
                if (toggle.isOn)
                    activeToggles.Enqueue(toggle);
            }

            if (ActiveCount < minToggleOn)
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
            if (maxToggleOn < ActiveCount)
            {
                for (int i = toggles.Count - 1; 0 <= i; i--)
                {
                    if (!toggles[i].isOn)
                        continue;

                    toggles[i].isOn = false;
                    activeToggles.Remove(toggles[i]);

                    if (ActiveCount == maxToggleOn)
                        break;
                }
            }

        }
        private void ManipulateToggle(int index, bool isOn)
        {
            if (invalidToggle == toggles[index])
            {
                invalidToggle = null;
                return;
            }

            if (isOn)
            {
                if (maxToggleOn == minToggleOn)
                {
                    activeToggles.Enqueue(toggles[index]);
                    OnValueChanged.Invoke(index, isOn);
                    invalidToggle = activeToggles.Dequeue();
                    invalidToggle.isOn = false;
                }
                else if (ActiveCount < maxToggleOn)
                {
                    activeToggles.Enqueue(toggles[index]);
                    OnValueChanged.Invoke(index, isOn);
                }
                else
                {
                    invalidToggle = toggles[index];
                    invalidToggle.isOn = false;
                }
            }
            else
            {
                if (minToggleOn < ActiveCount)
                {
                    activeToggles.Remove(toggles[index]);
                    OnValueChanged.Invoke(index, isOn);
                }
                else
                {
                    invalidToggle = toggles[index];
                    invalidToggle.isOn = true;
                }
            }
        }
        public void IsOn(int index, bool isOn)
        {
            if (index > toggles.Count)
                throw new ArgumentOutOfRangeException($"Index : {index} is larger than number of toggles : {toggles.Count}");
            toggles[index].isOn = isOn;
        }
        public void AddToggle(Toggle toggle)
        {
            toggles.Add(toggle);
        }
        public void AddToggleAt(Toggle toggle, int index)
        {
            if (toggles.Count < index)
                throw new ArgumentOutOfRangeException($"Index : {index} is larger than number of toggles : {toggles.Count}");
            else if (toggles.Count == index)
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
}