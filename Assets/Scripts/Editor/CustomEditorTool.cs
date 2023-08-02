
using UnityEditor;
using UnityEngine;

public class CustomEditorTool { 
    [MenuItem("GameObject/UI/CreateLocalizeText", true)]
    private static void MakeLocalizeText()
    {
        GameObject selected = Selection.activeObject as GameObject;
        GameObject go = new GameObject("Text");

        GameObjectUtility.SetParentAndAlign(go, selected.transform.parent.gameObject);
        go.transform.position = selected.transform.position;
        go.transform.rotation = selected.transform.rotation;
        GameObjectUtility.SetParentAndAlign(selected, go);

        //go.AddComponent<> 로컬라이즈 텍스트 관련 코드 넣기
        Undo.RegisterCreatedObjectUndo(go, "Parented " + go.name);
    }
}
