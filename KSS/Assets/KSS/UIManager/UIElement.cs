using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDGenerator
{
    static ulong IDStack = 0;

    public static ulong GenerateID()
    {
        if(IDStack == ulong.MaxValue)
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
public interface UIMessageSender
{

}

public interface UIMessageReciever
{
    string[] GetEnumArray();
}
public interface UIMessageReciever<in EnumType> : UIMessageReciever
{
    void GetEnumType(EnumType type);
    void OnMessageRecive(UIMessage message);
}
public class UIElement : MonoBehaviour
{
    private ulong _elementID = 0;
    public ulong elementID
    {
        get
        {
            if (_elementID == 0)
                _elementID = IDGenerator.GenerateID();
            return _elementID;
        }
    }
}
public class TestIt
{
    List<string> keys = new List<string>();

    private void SwitchTest(string key)
    {
        if(keys.Contains(key))
        {

        }

    }
}

public class EnumTypeTest : UIMessageReciever<EnumTypeTest.TestEnum>
{
    public enum TestEnum { }
    public void GetEnumType(TestEnum type)
    {
        switch (type)
        {
            default:
                break;
        }
    }

    public string[] GetEnumArray()
    {
        return Enum.GetNames(typeof(TestEnum));
    }

    public void OnMessageRecive(UIMessage message)
    {
        throw new NotImplementedException();
    }
}