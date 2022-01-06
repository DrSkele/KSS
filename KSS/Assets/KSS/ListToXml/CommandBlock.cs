using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CommandBlock<T>
{
    public enum CommandType { AsSibling, MakeChild, ToParent }

    T item;
    CommandType type;
    public CommandBlock(CommandType type, T item)
    {
        this.type = type;
        this.item = item;
    }
    public T GetT() => item;
    public virtual CommandType GetCommand() => type;
    public virtual XElement ConvertToElement()
    {
        return new XElement(type.ToString(), JsonUtility.ToJson(item));
    }

    public static CommandBlock<T> RevertToBlock(string type, string json)
    {
        return new CommandBlock<T>((CommandType)Enum.Parse(typeof(CommandType), type), JsonUtility.FromJson<T>(json));
    }

   
}

