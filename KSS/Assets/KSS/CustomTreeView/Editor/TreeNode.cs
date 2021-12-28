using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSS.CustomTreeView
{
    public class TreeNode
    {
        [SerializeField] int _id;
        [SerializeField] string _name;
        [SerializeField] int _depth;
        [NonSerialized] TreeNode _parent;
        [NonSerialized] List<TreeNode> _children;

        public int id { get => _id; set => _id = value; }
        public string name { get => _name; set => _name = value; }
        public int depth { get => _depth; set => _depth = value; }
        public TreeNode parent { get => _parent; set => _parent = value; }
        public List<TreeNode> children { get => _children; set => _children = value; }

        public bool isRoot => _depth == -1;
        public bool hasParent => _parent != null;
        public bool hasChildren => _children != null && _children.Count > 0;

        public TreeNode() { }

        public TreeNode(int ID, string Name, int Depth)
        {
            _id = ID;
            _name = Name;
            _depth = Depth;
        }
        public void ReleaseRelation()
        {
            _parent = null;
            _children = null;
        }
    }
}