using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class TestBuildAB : EditorWindow {

    [MenuItem("Tools/BuildImg")]
    public static void BuildImg() {

        string root = Application.dataPath + "/Sprits/Altas";
        DirectoryInfo altas = new DirectoryInfo(root);
        DirectoryInfo[] mapDirs = altas.GetDirectories();

        AssetBundleBuild[] buildMap = new AssetBundleBuild[mapDirs.Length];

        for (int i = 0; i < mapDirs.Length; i++) { 
            DirectoryInfo curr_dir = mapDirs[i];
            buildMap[i].assetBundleName = curr_dir.Name+".mk";
            
            FileInfo[] curr_files = curr_dir.GetFiles();
            string[] all_pics = new string[curr_files.Length/2];
            //G:/EditorExtend/EditorDemo/Assets/Sprits/Altas/${dir}/filename
            int index_j = 0;
            for (int j = 0; j < curr_files.Length; j++) {
                
                string fullName = curr_files[j].FullName.Replace("\\", "/");;
                if (!fullName.EndsWith(".meta")){
                    int index = fullName.IndexOf("Assets");
                    all_pics[index_j++] = fullName.Substring(index);
                    Debug.LogError(all_pics[index_j++]);
                }
                //int index = curr_files[j].FullName.IndexOf("/Assets");
                //all_pics[j] = curr_files[j].FullName.Substring(index);
                //Debug.LogError(all_pics[j]);
            }
            buildMap[i].assetNames = all_pics;
        }
        BuildPipeline.BuildAssetBundles("Assets/BuildAB", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        //BuildPipeline.BuildAssetBundles("Assets/BuildAB", );
    }
}
