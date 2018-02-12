using UnityEngine;

[CreateAssetMenu(fileName = "New GPUI Behaviour", menuName = "ScriptableObjects/GPUI Behaviour")]
public class GPUInstancingBehaviour : ScriptableObject {

	public Mesh mesh;
	public int subMeshIndex = 0;
	public Material material;
	public int instances = 1023;
	public UnityEngine.Rendering.ShadowCastingMode shadowCasting;


	public bool receiveShadows = true;
	[HideInInspector]
	public int layer;

	[HideInInspector]
	public Matrix4x4[] matrices = new Matrix4x4[1023];
}
