using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockListExample : MonoBehaviour
{
    class ExampleItem
    {

    }

    BlockList<ExampleItem> blockList = new BlockList<ExampleItem>();

    private void Start()
    {
        blockList.AddBlock(new ContainerBlock<ExampleItem>(new ExampleItem()));
    }
}
