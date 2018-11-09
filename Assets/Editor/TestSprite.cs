using UnityEngine;
using UnityEditor;

public class TestSprite {

	// Use this for initialization
    [MenuItem("Test/Sprite")]
	public static void Test () {
        string path = "Assets/Resources/Diulei";
        string subPath = path.Substring(path.IndexOf("Assets/"));
        Debug.LogError(subPath);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
