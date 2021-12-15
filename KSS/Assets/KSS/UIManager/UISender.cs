using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UISender : UIComponent, UIMessageSender
{
    public UIReciver sendTarget;
    public string key;
    public int index;
    public UIReciver GetSendTarget() => sendTarget;
}