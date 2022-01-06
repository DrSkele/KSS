using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSS.Utility;
using System.Xml.Linq;

public class BlockList : MonoBehaviour
{
    List<CommandBlock> blocks = new List<CommandBlock>();

    private void Start()
    {
        AddBlock(new ContainerBlock());
        AddBlock(new LoopStarterBlock());
        AddBlock(new ContainerBlock());
        AddBlock(new LoopFinishBlock());
        ConvertAll();
    }

    public void AddBlock(CommandBlock block)
    {
        blocks.Add(block);
    }

    public void RemoveBlock(CommandBlock block)
    {
        blocks.Remove(block);
    }

    public void MoveBlock(CommandBlock block, int destinationIndex)
    {
        int originIndex = blocks.IndexOf(block);
        blocks.Swap(originIndex, destinationIndex);
    }

    public void ConvertAll()
    {
        var root = new XElement("root");
        var parent = root;
        foreach (var block in blocks)
        {
            switch (block.GetCommand())
            {
                case CommandBlock.CommandType.AsSibling:
                    parent.Add(block.Element());
                    break;
                case CommandBlock.CommandType.MakeChild:
                    var element = block.Element();
                    parent.Add(element);
                    parent = element;
                    break;
                case CommandBlock.CommandType.ToParent:
                    parent = parent.Parent;
                    break;
                default:
                    break;
            }
        }
        Debug.Log(root);
    }
}