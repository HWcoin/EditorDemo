using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class FileTreeView : EditorWindow  {

    private static FileTreeView window;     // 自定义窗体
    private static TreeNode root;
    private Vector2 scroll_pos;

    private const int height = 20;
    private TreeNode _currentNode;
    private static int treeIndex = 0;

    private static FtpServerHelper ftpHelper;

    [MenuItem("H3D/文件视图")]
    static void Init()
    {
        //window = EditorWindow.GetWindow<FileTreeView>();   // 创建自定义窗体
        window = (FileTreeView)EditorWindow.GetWindowWithRect(typeof(FileTreeView), new Rect(500, 100, 400, 500), true, "文件夹视图");
        window.titleContent = new GUIContent("构建文件视图");         // 窗口的标题
        window.Show();
        CreateRoot();

        ftpHelper = new FtpServerHelper("192.168.11.239", "pub/web/zhj_ppx/testftp", "ftp", "123456");
    }

    private static void CreateRoot() {
        string rootpath = "G:/EditorExtend/EditorDemo/Temp";
        //"G:/";
        //"G:/EditorExtend/EditorDemo/Temp";
        DirectoryInfo rootInfo = new DirectoryInfo(rootpath);

        root = TreeNode.GenTreeNode(rootpath, rootpath, null, true, TreeNode.TreeNodeType.DIRECTORY_NODE, 0);
        InitChildren(rootpath, root, 0);
        /*
        Debug.LogError(root.FullPath + ":" + root.Height);
        for (int i = 0; i < root.Childs.Count; i++) {
            Debug.LogError(root.Childs[i].FullPath + ":" + root.Childs[i].Height);
        }
         * */
    }
    
    //将parentPath下的子目录和文件都存进node的子节点
    private static void InitChildren(string parentPath, TreeNode node, int height) {
        if (Directory.Exists(parentPath)) {
            DirectoryInfo parentDirectory = new DirectoryInfo(parentPath);

            if (node.Childs == null){
                node.Childs = new List<TreeNode>();
            }
            node.Childs.Clear();

            DirectoryInfo[] subDirectories = parentDirectory.GetDirectories();
            for (int i = 0; i < subDirectories.Length; i++) {
                try
                {
                    DirectoryInfo curDir = subDirectories[i];
                    TreeNode subNode = TreeNode.GenTreeNode(curDir.Name, curDir.FullName.Replace("\\", "/"), node, false, TreeNode.TreeNodeType.DIRECTORY_NODE, height + 1);
                    node.Childs.Add(subNode);
                    InitChildren(curDir.FullName.Replace("\\", "/"), subNode, height + 1);
                }
                catch (Exception ex) {
                    Debug.LogError(ex.Message);
                }
                
            }
            //文件不给访问
            FileInfo[] subFiles = parentDirectory.GetFiles();
            for (int i = 0; i < subFiles.Length; i++) {
                try
                {
                    FileInfo curFile = subFiles[i];
                    TreeNode subFileNode = TreeNode.GenTreeNode(curFile.Name, curFile.FullName.Replace("\\", "/"), node, false, TreeNode.TreeNodeType.FILE_NODE, height + 1);
                    node.Childs.Add(subFileNode);
                }
                catch (Exception ex) {
                    Debug.LogError(ex.Message);
                }
                
            }
        }
    }


    void OnGUI() {
        scroll_pos = GUILayout.BeginScrollView(scroll_pos, true, true, GUILayout.Width(400), GUILayout.Height(380));

        //root
        FileTreeView.treeIndex = 0;
        DrawTreeNode(root);

        GUILayout.EndScrollView();

        if (GUILayout.Button("上传选中的文件（非文件夹）", GUILayout.Width(300))) {
            /*
            if (_currentNode == null || _currentNode.NodeType == TreeNode.TreeNodeType.DIRECTORY_NODE) {
                Debug.LogError("请选择一个文件（非文件夹）");
                return;
            }
             * */
            //ftpHelper.Upload(_currentNode.FullPath);
            //ftpHelper.Download("G:", "TestFtp.txt");
            //ftpHelper.Delete("TestFtp.txt");
            ftpHelper.RemoveDirectory("demo");
            
        }
    }

    private void DrawTreeNode(TreeNode node) {
        if (node == null) return;
        Rect rect = new Rect(node.Height * 20, FileTreeView.treeIndex * 20, node.Name.Length * 25, 20);
        FileTreeView.treeIndex++;
       // Debug.LogError(node.Name + ":" + (node.Height * 20));
        if (node.NodeType == TreeNode.TreeNodeType.DIRECTORY_NODE)
        {
            node.IsOpen = EditorGUI.Foldout(rect, node.IsOpen, node.Name, true, EditorStyles.foldout);
        }
        else
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = null;
            style.normal.textColor = Color.white;
            if (node == _currentNode)
            {
                style.normal.textColor = Color.red;
            }
            if (GUI.Button(rect, node.Name, style))
            {
                _currentNode = node;
            }
        }
        
        if (node.Childs == null || !node.IsOpen || node.Childs.Count <= 0) return;
        for (int i = 0; i < node.Childs.Count; i++) {
            DrawTreeNode(node.Childs[i]);
        }
    }
}
