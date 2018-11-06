using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyUI {

    /* 自己扩展的组件放这里 */
    [MenuItem("GameObject/MyUI/Create Image")]
    public static void CreateImage() {
        GameObject go = new GameObject("Image");
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>();

        AddToSelectedObj(go);
    }
    /* end */

    [MenuItem("GameObject/MyUI/Create Container", false, 1)]
    public static void CreateContainer() {
        GameObject go = new GameObject("Container");
        go.AddComponent<RectTransform>();

        AddToSelectedObj(go);
    }

    public static void AddToSelectedObj(GameObject child) {
        GameObject selectedGameObj = Selection.activeGameObject;
        Scene currentScene = SceneManager.GetActiveScene();
        if (selectedGameObj == null)
        {
            SceneManager.MoveGameObjectToScene(child, currentScene);
        }
        else
        {
            child.GetComponent<RectTransform>().SetParent(selectedGameObj.transform);
        }
    }

    [MenuItem("GameObject/MyUI/Create Panel", false, 1)]
    public static void CreatePanel(MenuCommand menuCommand) {
        GameObject go = new GameObject("Panel_Empty");
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(EditorSettings.PANEL_WIDTH, EditorSettings.PANEL_HEIGHT);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;
        rect.localRotation = new Quaternion();
        rect.localScale = Vector3.one;

        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] allObjects = currentScene.GetRootGameObjects();
        GameObject canvasGO = null;
        RectTransform rectCanvas = null;

        GameObject selectedGameObj = Selection.activeGameObject;

        if (selectedGameObj == null)
        {
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Equals("Canvas"))
                {
                    canvasGO = obj;
                    rectCanvas = obj.GetComponent<RectTransform>();
                    break;
                }
            }
        }
        else {
            canvasGO = selectedGameObj;
            rectCanvas = selectedGameObj.GetComponent<RectTransform>();
        }

        if (canvasGO == null) {
            canvasGO = new GameObject("Canvas");

            rectCanvas = canvasGO.AddComponent<RectTransform>();
            canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            
            rectCanvas.sizeDelta = new Vector2(EditorSettings.PANEL_WIDTH, EditorSettings.PANEL_HEIGHT);
            rectCanvas.anchorMin = Vector2.zero;
            rectCanvas.anchorMax = Vector2.zero;
            rectCanvas.pivot = new Vector2(0.5f, 0.5f);
            rectCanvas.localRotation = new Quaternion();
            rectCanvas.localScale = Vector3.one;
            rectCanvas.localPosition = new Vector3(EditorSettings.PANEL_WIDTH / 2, EditorSettings.PANEL_HEIGHT / 2);

            SceneManager.MoveGameObjectToScene(canvasGO, currentScene);
        }

        rect.SetParent(canvasGO.transform);
    }
}
