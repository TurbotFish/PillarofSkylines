using UnityEngine;

[AddComponentMenu("Editor/Hierarchy Style")]
public class HierarchyStyle : MonoBehaviour
{
#if UNITY_EDITOR
    public Color nameColor, backgroundColor, commentColor = new Color(0, 0, 0, .5f);
    public string comment;
#endif
}