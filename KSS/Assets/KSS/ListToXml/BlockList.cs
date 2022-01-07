using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSS.Utility;
using System.Xml.Linq;

public class BlockList<T>
{
    List<CommandBlock<T>> blocks = new List<CommandBlock<T>>();

    public void AddBlock(CommandBlock<T> block)
    {
        blocks.Add(block);
    }

    public void RemoveBlock(CommandBlock<T> block)
    {
        blocks.Remove(block);
    }

    public void MoveBlock(int originIndex, int destinationIndex)
    {
        blocks.Swap(originIndex, destinationIndex);

    }

    public XElement ConvertAll()
    {
        var root = new XElement("root");
        var parent = root;
        foreach (var block in blocks)
        {
            switch (block.GetCommand())
            {
                case CommandBlock<T>.CommandType.AsSibling:
                    parent.Add(block.ConvertToElement());
                    break;
                case CommandBlock<T>.CommandType.MakeChild:
                    var element = block.ConvertToElement();
                    parent.Add(element);
                    parent = element;
                    break;
                case CommandBlock<T>.CommandType.ToParent:
                    parent = parent.Parent;
                    break;
                default:
                    break;
            }
        }
        return root;
    }

    public List<CommandBlock<T>> RevertAll(XElement root)
    {
        var list = new List<CommandBlock<T>>();
        Revert(list, root);
        return list;
    }

    private static void Revert(List<CommandBlock<T>> list, XElement root)
    {
        list.Add(CommandBlock<T>.RevertToBlock(root.Name.LocalName, root.Value));
        foreach (var element in root.Elements())
        {
            list.Add(CommandBlock<T>.RevertToBlock(element.Name.LocalName, element.Value));
            if (element.HasElements)
            {
                Revert(list, element);
            }
        }
    }
}