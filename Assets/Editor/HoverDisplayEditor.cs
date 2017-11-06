using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HoverDisplay))]
public class HoverDisplayEditor : Editor {

    void OnSceneGUI() {
        HoverDisplay t = target as HoverDisplay;

        Debug.Log(t);

        float size = HandleUtility.GetHandleSize(t.transform.position);

        GUIStyle style = new GUIStyle();
        style.fixedHeight = style.fixedWidth = 200 / size;
        
        Handles.Label(t.transform.position + Vector3.up * 2, new GUIContent(t.image), style);
    }

}
