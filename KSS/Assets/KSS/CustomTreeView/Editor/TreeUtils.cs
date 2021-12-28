using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace KSS.CustomTreeView
{
    public static class TreeUtils
    {
        /// <summary>
        /// Returns ordered list of nodes by their depth.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns>List ordered by depth</returns>
        public static IList<T> ReorderNodes<T>(IList<T> nodes) where T : TreeNode
        {
            return nodes.OrderBy(x => x.depth).ToList();
        }
        /// <summary>
        /// Assemble new Tree with given ordered nodes.<br/>
        /// If nodes are not ordered, use <see cref="ReorderNodes{T}(IList{T})"/>
        /// </summary>
        /// <param name="nodes">List of nodes. Must be ordered by depth.</param>
        /// <returns>Root node of assembled tree</returns>
        public static T AssembleTree<T>(IList<T> nodes) where T : TreeNode
        {
            ValidateNodes(nodes);

            foreach (var node in nodes)
            {
                node.ReleaseRelation();
            }

            for (int currentIdx = 0; currentIdx < nodes.Count; currentIdx++)
            {
                var currentNode = nodes[currentIdx];
                int currentDepth = currentNode.depth;
                int childCount = 0;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].depth == currentDepth + 1)
                        childCount++;
                    else if (nodes[i].depth <= currentDepth)
                        break;
                }

                List<TreeNode> childList = null;
                if(childCount != 0)
                {
                    childList = new List<TreeNode>(childCount);

                    for (int i = currentIdx + 1; i < nodes.Count; i++)
                    {
                        if(nodes[i].depth == currentDepth + 1)
                        {
                            nodes[i].parent = currentNode;
                            childList.Add(nodes[i]);
                        }
                        else if (nodes[i].depth <= currentDepth)
                            break;

                    }
                    currentNode.children = childList;
                }
            }
            return nodes[0];
        }

        public static void ValidateNodes<T>(IList<T> nodes) where T : TreeNode
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes), "Input nodes are null.");
            if (nodes.Count == 0)
                throw new ArgumentException("Input nodes are empty.", nameof(nodes));
            if (nodes[0].depth != -1)
                throw new ArgumentException("First node must have depth of -1 in order to be a root node.", nameof(nodes));
            if (nodes.Where(x => x.depth == -1).Count() > 1)
                throw new ArgumentException("Multiple nodes have depth value of -1. Root node with depth value -1 must be one.", nameof(nodes));
            
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                int currentDepth = nodes[i].depth;
                int nextDepth = nodes[i + 1].depth;
                if (nextDepth > currentDepth + 1)
                    throw new ArgumentException($"Depth value missing between {currentDepth} and {nextDepth}.", nameof(nodes));
            }
        }
    }
}