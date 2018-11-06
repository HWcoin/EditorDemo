using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AssetBundleCreator {

    public static void BuildAssetBundle() { 
    
    }

    [MenuItem("AssetBundle/Texture2D/texture packer", false, 1)]
    public static void Texture2DCreator() {
        string[] guids = Selection.assetGUIDs;
        foreach (string guid in guids) {
            //Assets/UI/Sprites/folderName
            string folderPath = AssetDatabase.GUIDToAssetPath(guid);
            BuildTextures2Resources(folderPath);
        }


        /*
        Texture2D[] texture;
        Rect[] rects;
        Texture2D altas = new Texture2D(2048, 2048);
        rects = altas.PackTextures(texture, 2, 2048);
        byte[] buffer = altas.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/alta.jpg", buffer);
        AssetDatabase.Refresh();
         * 
         * Texture2D t = AssetDatabase.LoadAssetAtPath("Assets/Textures/texture.jpg", typeof(Texture2D)) as Texture2D;     
         * */
    }

    public static void BuildTextures2Resources(string folderpath) {

        string datapath = Application.dataPath;
        int preIndex = datapath.LastIndexOf("/");
        string prePath = datapath.Substring(0, preIndex + 1);
        string folderFullPath = prePath + folderpath;
        DirectoryInfo folder = new DirectoryInfo(folderFullPath);

        string targetFolder = Application.dataPath+"/Resources/"+folder.Name; 
        string targetName = targetFolder +"/"+folder.Name+".png";
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder.Replace("\\", "/"));
        }
        else {//删除旧的
        }

        List<Texture2D> texList = new List<Texture2D>();

        foreach(FileInfo fileInfo in folder.GetFiles()){
            string fileFullName = fileInfo.FullName;
            if (!Path.GetExtension(fileFullName).Contains(".meta")) {
                string texPath = fileFullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                texList.Add(AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D)) as Texture2D);
            }
        }
        if (texList.Count > 0) {
            Rect[] rects;
            Texture2D altas = new Texture2D(1024, 1024);
            rects = altas.PackTextures(texList.ToArray(), 2, 2048);
            byte[] buff = altas.EncodeToPNG();
            File.WriteAllBytes(targetName, buff);
            AssetDatabase.Refresh();
        }
    }

    //public static string[]
}
