using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AssetBundleCreator {

    [MenuItem("AssetBundle/AssetBundle/build by prefab")]
    public static void BuildAssetBundle() {
        Object[] selectAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        foreach (Object obj in selectAssets) {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            Debug.LogError(sourcePath);
            string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";

            if (BuildPipeline.BuildAssetBundles(Application.dataPath + "/StreamingAssets", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
            AssetDatabase.Refresh();
        }
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
        List<TextureImporter> texImporterList = new List<TextureImporter>();

        foreach(FileInfo fileInfo in folder.GetFiles()){
            string fileFullName = fileInfo.FullName;
            if (!Path.GetExtension(fileFullName).Contains(".meta")) {
                string texPath = fileFullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                texList.Add(AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D)) as Texture2D);
                texImporterList.Add(AssetImporter.GetAtPath(texPath) as TextureImporter);
            }
        }
        if (texList.Count > 0) {
            Rect[] rects;
            Texture2D altas = new Texture2D(1024, 1024);
            rects = altas.PackTextures(texList.ToArray(), 2, 2048);
            byte[] buff = altas.EncodeToPNG();
            File.WriteAllBytes(targetName, buff);
            AssetDatabase.Refresh();
            TextureImporter targetImporter = AssetImporter.GetAtPath(targetName) as TextureImporter;
            Debug.LogError("targetImporter is null :" + (targetImporter == null));

            List<SpriteMetaData> metaDatas = new List<SpriteMetaData>();
            Debug.LogError("texList:" + texList.Count);
            Debug.LogError("texImporterList:" + texImporterList.Count);
            for (int i = 0; i < texList.Count; i++) {
                Texture2D curTexInfo = texList[i];

                Rect curRect = rects[i];

                Rect newRect = new Rect(curRect.x, curRect.y, curRect.width, curRect.height);

                SpriteMetaData metaData = new SpriteMetaData();
                metaData.name = curTexInfo.name;
                metaData.rect = curRect;
                metaData.border = texImporterList[i].spriteBorder;
                metaData.pivot = new Vector2(0.5f, 0.5f);
                metaDatas.Add(metaData);
            }
            Debug.LogError(targetName);
            targetImporter.textureType = TextureImporterType.Advanced;
            targetImporter.spritesheet = metaDatas.ToArray();
            targetImporter.maxTextureSize = 1024;
            targetImporter.filterMode = FilterMode.Bilinear;
            targetImporter.wrapMode = TextureWrapMode.Clamp;
            //targetImporter.
            targetImporter.textureFormat = TextureImporterFormat.ARGB16;

            targetImporter.SetPlatformTextureSettings("Standalone", 1024, TextureImporterFormat.ARGB16);
            //targetImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.ARGB16, true);
            //targetImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.ARGB16);
            targetImporter.compressionQuality = 100;
            TextureImporterSettings setting = new TextureImporterSettings();
            targetImporter.ReadTextureSettings(setting);
            targetImporter.SetTextureSettings(setting);
        }
    }

    //public static string[]
}
