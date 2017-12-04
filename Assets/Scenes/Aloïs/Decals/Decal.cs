using UnityEngine;

[ExecuteInEditMode]
public class Decal : MonoBehaviour {

	public Color boxColour;

	public enum DecalType {
		DiffuseOnly, NormalsOnly, DiffuseAndNormals
	}

	public DecalType m_DecalType;
	public Material m_Material;


	void Start(){
		DeferredDecalSystem.instance.AddDecal (this);

	}

	void OnEnable(){
		DeferredDecalSystem.instance.AddDecal (this);
	}
		
	void OnDisable(){
		DeferredDecalSystem.instance.RemoveDecal (this);
	}

	void OnDrawGizmos(){
		DrawGizmo (false);
	}

	void OnDrawGizmosSelected(){
		DrawGizmo (true);
	}


	void DrawGizmo(bool selected){
		Color _col = boxColour;
		_col.a = selected ? .3f : .1f;
		Gizmos.color = _col;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube (Vector3.zero, Vector3.one);
		_col.a = selected ? .5f : .2f;
		Gizmos.color = _col;
		Gizmos.DrawWireCube (Vector3.zero, Vector3.one);
	}

}
