#if UNITY_EDITOR
using UnityEngine;

[AddComponentMenu("Editor/Hierarchy Style")]
public class HierarchyStyle : MonoBehaviour {
	public Color nameColor, backgroundColor, commentColor = new Color(0, 0, 0, .5f);
	public string comment;
}
#endif