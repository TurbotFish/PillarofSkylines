using UnityEngine;
using System.Collections.Generic;

public class GPUIDisplayManager : MonoBehaviour {

	Dictionary<int, List<Matrix4x4>> transformsID = new Dictionary<int, List<Matrix4x4>> ();
	List<Matrix4x4> matrices = new List<Matrix4x4> ();
	List<int> indices = new List<int> ();

	public static GPUIDisplayManager displayManager;

	int numberOfCalls;
	//int[,] boundaries = {{0,1023},{1024,2047}, };
	int boundaryLow;
	int boundaryHigh;

	//temp, should be on seeder
	public Mesh meshToDraw;
	public Material materialToDraw;

	int batchSize = 1023;

	void Start(){
		displayManager = this;
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

	void Update(){
		GPUIDraw();

		//Debug.Log (matrices.Count);
	}

	void GPUIDraw(){

		if (matrices.Count == 0)
			return;

		for (int i = 0; i < numberOfCalls; i++) {
			boundaryLow = i * 1023;
			//boundaryHigh = numberOfCalls * 1024 - 1;
			boundaryHigh = Mathf.Min (1023, matrices.Count - boundaryLow);

			//Debug.Log ("low:  " + boundaryLow + "   high:  " + boundaryHigh);

			//Graphics.DrawMeshInstanced (meshToDraw, 0, materialToDraw, matrices.GetRange (boundaryLow, boundaryHigh));
			Graphics.DrawMeshInstanced(meshToDraw,0,materialToDraw,matrices.GetRange (boundaryLow, boundaryHigh),null,UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null);
		}
		//Graphics.DrawMeshInstanced (meshToDraw, 0, materialToDraw, matrices.GetRange (0, 1023));
		//Graphics.DrawMeshInstanced(meshToDraw, 0,materialToDraw, matrices.ToArray(), 1023, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
	}

	void RearrangeListOfObjectsToDraw(){
		matrices.Clear ();
		foreach (List<Matrix4x4> item in transformsID.Values) {
			matrices.AddRange (item);
		}
			//replace foreach
		//		for (int i = 0; i < indices.Count; i++) {
		//			matrices.AddRange (transformsID[indices[i]]);
		//		}

		numberOfCalls = matrices.Count / 1024 + 1;
		//Debug.Log ("calls : "+numberOfCalls+"    vertices : "+matrices.Count);

	}
}
