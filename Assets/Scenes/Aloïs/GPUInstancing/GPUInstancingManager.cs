using UnityEngine;

public class GPUInstancingManager : MonoBehaviour {

	public GPUInstancingBehaviour[] behaviours;

	void Update(){
		for (int i = 0; i < behaviours.Length; i++) {
			DrawAllTheMeshes (behaviours [i]);
		}
	}

	void DrawAllTheMeshes(GPUInstancingBehaviour _gpui){
		Graphics.DrawMeshInstanced (_gpui.mesh, _gpui.subMeshIndex, _gpui.mat, _gpui.matrices, _gpui.instances, null, _gpui.shadowCasting, _gpui.receiveShadows, _gpui.layer);
	}
}
