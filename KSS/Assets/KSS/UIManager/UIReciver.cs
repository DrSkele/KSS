using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIReciver : UIComponent, UIMessageReciever
{
    public List<UISender> connected = new List<UISender>();

    public void AddSender(UISender sender)
    {
        if (!connected.Find(x => x.ComponentID == sender.ComponentID))
            connected.Add(sender);
    }

    public abstract string[] GetEnumArray();
}
