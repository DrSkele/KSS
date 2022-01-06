using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public abstract class CommandBlock
{
    public enum CommandType { AsSibling, MakeChild, ToParent }
    public abstract CommandType GetCommand();
    public abstract XElement Element();
}

public class ContainerBlock : CommandBlock
{
    public override CommandType GetCommand() => CommandType.AsSibling;
    public override XElement Element()
    {
        return new XElement("Item", 1);
    }

}

public class LoopStarterBlock : CommandBlock
{
    public override CommandType GetCommand() => CommandType.MakeChild;

    public override XElement Element()
    {
        return new XElement("LoopStart", 0);
    }
}

public class LoopFinishBlock : CommandBlock
{
    public override CommandType GetCommand() => CommandType.ToParent;

    public override XElement Element()
    {
        return new XElement("Item", 1);
    }
}

