using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TreeNode
{
    public enum TreeNodeType{
        DIRECTORY_NODE,
        FILE_NODE
    }

    private string name;
    private string fullPath;
    private TreeNode parent;
    private List<TreeNode> childs;
    private static TreeNode _isntanec;
    private bool isOpen = false;
    private TreeNodeType _nodeType = TreeNodeType.FILE_NODE;
    private int height = 0;

    public TreeNode() {
        childs = new List<TreeNode>();
        isOpen = false;
        _isntanec = this;
    }

    public static TreeNode GenTreeNode(string name, string fullPath, TreeNode parent, bool isOpen, TreeNodeType nodeType, int height){

        TreeNode node = new TreeNode();
        node.Name = name;
        node.FullPath = fullPath;
        node.Parent = parent;
        node.IsOpen = isOpen;
        node.NodeType = nodeType;
        node.Height = height;
        return node;
    }

    public int Height { get; set; }
    public TreeNodeType NodeType { get; set; }
    public bool IsOpen { get; set; }
    public static TreeNode Instance { get; set; }
    public List<TreeNode> Childs { get; set; }
    public TreeNode Parent { get; set; }
    public string FullPath { get; set; }
    public string Name { get; set; }
}
