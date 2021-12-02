using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDGenerator
{
    static ulong IDStack = 0;

    public static ulong GenerateID()
    {
        if (IDStack == ulong.MaxValue)
        {
            throw new System.OverflowException("IDStack Overflow");
        }
        return ++IDStack;
    }
}
public enum UserInteraction { ButtonClick }
public class UIMessage
{
    public ulong sender;
    public UserInteraction interaction;
}
public class UIComponent : MonoBehaviour
{
    ulong componentID = 0;
    public ulong ComponentID
    {
        get
        {
            if (componentID == 0)
                componentID = IDGenerator.GenerateID();
            return componentID;
        }
    }
}

public interface UIMessageSender
{
    UIReciver GetSendTarget();
}
public interface UIMessageReciever
{
    string[] GetEnumArray();
    void AddSender(UISender sender);
}
public interface UIMessageReciever<in EnumType> : UIMessageReciever
{
    void GetEnumType(EnumType type);
    void OnMessageRecive(UIMessage message);
}