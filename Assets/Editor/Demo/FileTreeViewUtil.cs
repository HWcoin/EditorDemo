using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class FileTreeViewUtil : EditorWindow
{
    private List<string> list = new List<string> ();
    private static FileTreeNode root = null;
    private FileTreeNode currentNode;
    private static FileTreeViewUtil _instance = new FileTreeViewUtil();
	private int treeIndex = 0;
    private static FileTreeViewUtil window;     // 自定义窗体
 
	[MenuItem("H3D/构建树视图")]
 
	static void Init(){
        window = EditorWindow.GetWindow<FileTreeViewUtil>();   // 创建自定义窗体
		window.titleContent = new GUIContent("构建树视图");         // 窗口的标题
		window.Show();
		_instance.GetAssets ();
		_instance.CreateTree ();
              // 创建树
	}
 
//	void Awake()
//	{
//		Debug.Log ("Awake");
//	}
 
	void Start()
	{
		Debug.Log ("Start");
	}
 
//	void Update()
//	{
//		Debug.Log ("Update");
//	}
 
 
	private void GetAssets()
	{
		list.Clear ();
		list.Add ("生物/动物");
		list.Add ("生物/动物/宠物/猫");
		list.Add ("生物/动物/宠物/狗");
//		list.Add ("生物/动物/野生/老虎");
//		list.Add ("生物/动物/野生/狮子");
 
		list.Add ("生物/植物");
		list.Add ("生物/植物/蔬菜/白菜");
		list.Add ("生物/植物/蔬菜/萝卜");
//		list.Add ("生物/植物/水果/苹果");
//		list.Add ("生物/植物/水果/橘子");
	
		Debug.Log ("获取数据完成");
	}
 
	private void CreateTree()
	{
        root = FileTreeNode.Get().GenerateFileTree(list);
		Debug.Log ("生成文件树完成");
//		ShowFileTree (root, 0);
//		Debug.Log ("显示文件树完成");
	}


    private void ShowFileTree(FileTreeNode node, int level)
	{
		string prefix = "";
		for (int i = 0; i < level; i++)
		{
			prefix += "~";
		}
		Debug.Log (prefix + node.name);
		if (node == null || node.children == null) 
		{
			return;
		}
		for (int i = 0; i < node.children.Count; i++) 
		{
			ShowFileTree (node.children[i], level+1);
		}
	}


    private void DrawFileTree(FileTreeNode node, int level)
	{
		if (node == null) 
		{
			return;
		}
		GUIStyle style = new GUIStyle();
		style.normal.background = null;
		style.normal.textColor = Color.white;
		if (node == currentNode) 
		{
			style.normal.textColor = Color.red;
		}
 
		Rect rect = new Rect(5+20*level, 5+20*treeIndex, node.name.Length*25, 20);
		treeIndex++;

        if (node.nodeType == FileTreeNode.TreeNodeType.Switch)
        {
			node.isOpen = EditorGUI.Foldout (rect, node.isOpen, node.name, true);
		}
		else
		{
			if (GUI.Button (rect, node.name, style)) 
			{
				Debug.Log (node.name);
				currentNode = node;
			}
		}
	
		if (node==null || !node.isOpen || node.children == null) 
		{
			return;
		}
		for (int i = 0; i < node.children.Count; i++) 
		{
			DrawFileTree (node.children[i], level+1);
		}
	}

    bool canOut = false;
	void OnGUI()
	{
		treeIndex = 0;
		DrawFileTree (root, 0);
    }
}
