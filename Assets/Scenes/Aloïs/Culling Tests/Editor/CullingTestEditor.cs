using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CullingTestEditor : ShaderGUI {

	Material target;
	MaterialEditor editor;
	MaterialProperty[] properties;
	MaterialProperty _culling;

	public override void OnGUI (MaterialEditor editor, MaterialProperty[] props){
		this.target = editor.target as Material;
		//this.editor = editor;
		this.properties = props;
		//base.OnGUI (materialEditor, properties);

		_culling = FindProperty ("_Cull", properties);
		DoCulling ();
	}

	void DoCulling(){
		

		EditorGUI.BeginChangeCheck ();
		CullMode mode = (CullMode)Mathf.RoundToInt (_culling.floatValue);
		mode = (CullMode)EditorGUILayout.EnumPopup ("Culling", mode);
		if (EditorGUI.EndChangeCheck ()) {
			target.SetInt ("_Cull", (int)mode);
		}
	}
}
