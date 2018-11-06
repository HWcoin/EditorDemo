using UnityEngine;
using UnityEditor;
using System;


public class Texture2DPacker{

    /// <summary>
    /// 使用代码将多张图集打包成一张图片
    /// </summary>
    [MenuItem("Pack")]
    public static void PackAll2One(){
        Texture2D[] texture;
        Rect[] rects;
        Texture2D altas = new Texture2D (2048, 2048);
        rects = altas.PackTextures (texture, 2, 2048);
        byte[] buffer = altas.EncodeToPNG();
        File.WriteAllBytes (Application.dataPath + "/alta.jpg",buffer);
        AssetDatabase.Refresh();
        //Texture2D t = AssetDatabase.LoadAssetAtPath("Assets/Textures/texture.jpg", typeof(Texture2D)) as Texture2D;
        //如何使用这张图集
        /*
        string str = "text";//Resources文件夹下面的名字
        //SpriteRenderer[] sp = Resources.LoadAll<SpriteRenderer>(str);
        Sprite[] sprite = Resources.LoadAll<Sprite>(str);
        foreach (var item in sprite)
        {
            print(item.name);
            
        }
        btn.sprite = sprite[2];
         * */
    }
}