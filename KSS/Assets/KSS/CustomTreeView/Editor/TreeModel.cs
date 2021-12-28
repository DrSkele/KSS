using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSS.CustomTreeView
{
    public class TreeModel<T> where T : TreeNode
    {
        IList<T> _nodes;
        T _root;

        int _registered = 0;

        public T root { get => _root; set => _root = value; }
        
        public event Action OnModelChanged
        {
            add
            {
                OnModelChanged += value;
                _registered++;
            }
            remove
            {
                OnModelChanged -= value;
                _registered--;
            }
        }

        public int Count => _nodes.Count;

        public TreeModel(IList<T> nodes)
        {

        }

        public void SetDatas(IList<T> nodes)
        {
            _root = TreeUtils.AssembleTree(nodes);

            _nodes = nodes;
        }
    }
}

