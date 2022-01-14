using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class QueueableList<T> : List<T>
{
    public void Enqueue(T item)
    {
        this.Add(item);
    }
    public T Dequeue()
    {
        if (this.Count == 0)
            throw new InvalidOperationException("QueueableList empty.");

        var dequeueItem = this[0];
        this.RemoveAt(0);
        return dequeueItem;
    }
}
