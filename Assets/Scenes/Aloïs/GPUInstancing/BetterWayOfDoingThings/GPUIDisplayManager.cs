using UnityEngine;
using System.Collections.Generic;

public class GPUIDisplayManager : MonoBehaviour {

	//Ce script a été optimisé avec l'esprit de Bruno Mabille

	public enum GPUIMethod {Method1, Method2, Method3};
	public GPUIMethod GPUI;


	Dictionary<int, List<Matrix4x4>> transformsID = new Dictionary<int, List<Matrix4x4>> ();
	List<Matrix4x4> matrices = new List<Matrix4x4> ();
	List<int> indices = new List<int> ();

	public static GPUIDisplayManager displayManager;

	int numberOfCalls;
	int boundaryLow;
	int boundaryHigh;
	int instancesPerList;

	List<Matrix4x4>[] matrices1023 = new List<Matrix4x4>[80];

	bool updatedThisFrame;

	//temp, should be on seeder
	public Mesh meshToDraw;
	public Material materialToDraw;


	void Awake(){
		displayManager = this;

		for (int i = 0; i < matrices1023.Length; i++) {
			matrices1023 [i] = new List<Matrix4x4> ();
		}
	}

	public void AddStuffToDraw(List<Matrix4x4> _mat, int _id){
		transformsID.Add (_id, _mat);
		indices.Add (_id);
		RearrangeListOfObjectsToDraw ();
	}

	public void RemoveStuffToDraw(List<Matrix4x4> _mat, int _id){
		transformsID.Remove (_id);
		indices.Remove (_id);
		RearrangeListOfObjectsToDraw ();
	}

	void LateUpdate(){
		
		GPUIDraw();
		updatedThisFrame = false;
	}

	void GPUIDraw(){

		if (matrices.Count == 0)
			return;

		for (int i = 0; i < numberOfCalls; i++) {
			#region method1
			if(GPUI == GPUIMethod.Method1){
				boundaryLow = i * 1023;
				boundaryHigh = Mathf.Min (1023, matrices.Count - boundaryLow);
				Graphics.DrawMeshInstanced (meshToDraw, 0, materialToDraw, matrices.GetRange (boundaryLow, boundaryHigh), null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null);
			}
			#endregion

			#region method2
			if(GPUI == GPUIMethod.Method2){
				Graphics.DrawMeshInstanced (meshToDraw, 0, materialToDraw, matrices1023[i], null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null);
			}
			#endregion
		}
	}

	void RearrangeListOfObjectsToDraw(){
		matrices.Clear ();

//		foreach (List<Matrix4x4> item in transformsID.Values) {
//			matrices.AddRange (item);
//		}

		//replace foreach
		for (int i = 0; i < indices.Count; i++) {
			matrices.AddRange (transformsID[indices[i]]);
		}

		numberOfCalls = matrices.Count / 1024 + 1;
		//Debug.Log ("calls : "+numberOfCalls+"    vertices : "+matrices.Count);


		#region method2
		if(GPUI == GPUIMethod.Method2){
			//method 2: smoother when seeders don't change too often
			for (int i = 0; i < numberOfCalls; i++) {
				boundaryLow = i * 1023;
				instancesPerList = Mathf.Min (1023, matrices.Count - boundaryLow);

				matrices1023 [i].Clear ();
				matrices1023 [i].AddRange (matrices.GetRange (boundaryLow, instancesPerList));
			}
		}
		#endregion


	}

}
