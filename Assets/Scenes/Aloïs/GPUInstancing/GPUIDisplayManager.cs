using UnityEngine;
using System.Collections.Generic;

public class GPUIDisplayManager : MonoBehaviour {

	//Ce script a été optimisé avec l'esprit de Bruno Mabille

	Dictionary<int, List<Matrix4x4>> transformsID = new Dictionary<int, List<Matrix4x4>> ();
	List<Matrix4x4> matrices = new List<Matrix4x4> ();
	List<int> indices = new List<int> ();

	//public static GPUIDisplayManager displayManager;

	int numberOfCalls;
	int boundaryLow;
	int instancesPerList;

	List<Matrix4x4>[] matrices1023 = new List<Matrix4x4>[150];

	bool updatedThisFrame;

	//temp, should be on seeder
	public Mesh meshToDraw;
	public Material materialToDraw;
	public UnityEngine.Rendering.ShadowCastingMode shadowMode;
	public Transform player;


	Texture2D eastMap;
	Texture2D westMap;

	string east = "EastPlane";
	string west = "WestPlane";
	int eastLayer;
	int westLayer;
	int currentLayer;
	Texture2D colorVariationMap;

	void Awake(){
		Shader.WarmupAllShaders ();
		//displayManager = this;

		for (int i = 0; i < matrices1023.Length; i++) {
			matrices1023 [i] = new List<Matrix4x4> ();
		}

		eastLayer = LayerMask.NameToLayer (east);
		westLayer = LayerMask.NameToLayer (west);

		SurfaceTextureHolder _mapHolder = (SurfaceTextureHolder)Resources.Load ("ScriptableObjects/GrassColorMaps");
		eastMap = _mapHolder.eastTex;
		westMap = _mapHolder.westTex;
	}

	public void AddStuffToDraw(List<Matrix4x4> _mat, int _id){
        if (!indices.Contains(_id) && _mat.Count > 0)
        {
            transformsID.Add(_id, _mat);
            indices.Add(_id);
            updatedThisFrame = true;
            //Debug.LogFormat("GPUIDisplayManager: AddStuffToDraw: added {0} matrices!", _mat.Count);
        }
		//Debug.Log (_id);
	}

	public void RemoveStuffToDraw(List<Matrix4x4> _mat, int _id){
		transformsID.Remove (_id);
		indices.Remove (_id);
		updatedThisFrame = true;
        //Debug.LogFormat("GPUIDisplayManager: RemoveStuffToDraw: removed {0} matrices!", _mat.Count);
    }

	void LateUpdate(){
		if (updatedThisFrame)
			RearrangeListOfObjectsToDraw ();

		SetGPUILayer ();

		GPUIDraw();
		updatedThisFrame = false;
	}

	void GPUIDraw(){

		if (matrices.Count == 0)
			return;

		for (int i = 0; i < numberOfCalls; i++) {

			Graphics.DrawMeshInstanced (meshToDraw, 0, materialToDraw, matrices1023[i], null, shadowMode, false, currentLayer, null);
		}
	}

	void RearrangeListOfObjectsToDraw(){
		matrices.Clear ();

		for (int i = 0; i < indices.Count; i++) {
			matrices.AddRange (transformsID[indices[i]]);
		}

		numberOfCalls = matrices.Count / 1024 + 1;
		//Debug.Log (numberOfCalls);
		if (numberOfCalls > matrices1023.Length) {
			numberOfCalls = Mathf.Min (numberOfCalls, matrices1023.Length);
			Debug.LogError ("Trying to draw too many instances of " + meshToDraw.name);
		}

		//Debug.Log ("calls : "+numberOfCalls+"    vertices : "+matrices.Count);

		for (int i = 0; i < numberOfCalls; i++) {
			boundaryLow = i * 1023;
			instancesPerList = Mathf.Min (1023, matrices.Count - boundaryLow);

			matrices1023 [i].Clear ();
			matrices1023 [i].AddRange (matrices.GetRange (boundaryLow, instancesPerList));
		}
	}

	void SetGPUILayer(){
		//TO DO: optimise this to happen when it's changed
		if (player.position.x > 0) {
			currentLayer = eastLayer;
			colorVariationMap = eastMap;

			//materialToDraw.EnableKeyword ("_GPUI_EAST");
		} else {
			currentLayer = westLayer;
			colorVariationMap = westMap;
			//materialToDraw.DisableKeyword ("_GPUI_EAST");
		}
		Shader.SetGlobalTexture ("_GPUIColorMap", colorVariationMap);
	}

}
