﻿using UnityEngine;
using System.Collections.Generic;

public class GPUIDisplayManager : MonoBehaviour {

	//Ce script a été optimisé avec l'esprit de Bruno Mabille

	Dictionary<int, List<Matrix4x4>> transformsID = new Dictionary<int, List<Matrix4x4>> ();
	List<Matrix4x4> matrices = new List<Matrix4x4> ();
	List<int> indices = new List<int> ();

	public static GPUIDisplayManager displayManager;

	int numberOfCalls;
	int boundaryLow;
	int instancesPerList;

	List<Matrix4x4>[] matrices1023 = new List<Matrix4x4>[300];

	bool updatedThisFrame;

	//temp, should be on seeder
	public Mesh meshToDraw;
	public Material materialToDraw;
	public UnityEngine.Rendering.ShadowCastingMode shadowMode;
	public Transform player;

	public Gradient colourVariations;

	string east = "EastPlane";
	string west = "WestPlane";
	int eastLayer;
	int westLayer;
	int currentLayer;

	//Testing colour variations
	MaterialPropertyBlock properties;
	Vector4[] customColors = new Vector4[1023];

	void Awake(){
		displayManager = this;

		for (int i = 0; i < matrices1023.Length; i++) {
			matrices1023 [i] = new List<Matrix4x4> ();
		}

		eastLayer = LayerMask.NameToLayer (east);
		westLayer = LayerMask.NameToLayer (west);

		///doesn't work great
		//colour variation test
		properties = new MaterialPropertyBlock();
		for (int i = 0; i < customColors.Length; i++) {
			customColors [i] = colourVariations.Evaluate ((float)i / (float)customColors.Length);
		}
		properties.SetVectorArray ("_Color", customColors);
		///:sadface:

	}

	public void AddStuffToDraw(List<Matrix4x4> _mat, int _id){
		transformsID.Add (_id, _mat);
		indices.Add (_id);
		updatedThisFrame = true;
	}

	public void RemoveStuffToDraw(List<Matrix4x4> _mat, int _id){
		transformsID.Remove (_id);
		indices.Remove (_id);
		updatedThisFrame = true;
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

			//test colour variation

			Graphics.DrawMeshInstanced (meshToDraw, 0, materialToDraw, matrices1023[i], properties, shadowMode, false, currentLayer, null);




		}
	}

	void RearrangeListOfObjectsToDraw(){
		matrices.Clear ();

		for (int i = 0; i < indices.Count; i++) {
			matrices.AddRange (transformsID[indices[i]]);
		}

		numberOfCalls = matrices.Count / 1024 + 1;
		//Debug.Log ("calls : "+numberOfCalls+"    vertices : "+matrices.Count);

		for (int i = 0; i < numberOfCalls; i++) {
			boundaryLow = i * 1023;
			instancesPerList = Mathf.Min (1023, matrices.Count - boundaryLow);

			matrices1023 [i].Clear ();
			matrices1023 [i].AddRange (matrices.GetRange (boundaryLow, instancesPerList));
		}
	}

	void SetGPUILayer(){
		currentLayer = player.position.x > 0 ? eastLayer : westLayer;
	}

}
