using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DecalRenderer : MonoBehaviour {


	public Mesh m_CubeMesh;
	Camera cam;
	CommandBuffer buff;

	//#if UNITY_EDITOR
		Camera editorCam;
		CommandBuffer editorBuff;
	//#endif

	//if multiple cameras
	//Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer> (); 

//	//if one camera
	//KeyValuePair<Camera, CommandBuffer> m_KVP = new KeyValuePair<Camera, CommandBuffer>();

	void OnDisable(){
		//multiple cameras
//		foreach (KeyValuePair<Camera, CommandBuffer> _KVPair in m_Cameras) {
//			if (_KVPair.Key)
//				_KVPair.Key.RemoveCommandBuffer (CameraEvent.AfterLighting, _KVPair.Value);
//		}

//		//single camera
		//if (m_KVP.Key)
			//m_KVP.Key.RemoveCommandBuffer (CameraEvent.AfterLighting, m_KVP.Value);

		//if (buff != null)
			//cam.RemoveCommandBuffer (CameraEvent.AfterLighting, cam);
	}

	void Start(){
		//bool showDecals = gameObject.activeInHierarchy && enabled;

//		if (!showDecals) {
//			OnDisable ();
//			return;
//		}

//		Camera cam = Camera.current;
//		if (!cam)
//			return;

		#if UNITY_EDITOR
		if(editorCam == null){
			if(SceneView.currentDrawingSceneView != null)
				editorCam = SceneView.currentDrawingSceneView.camera;
		}
		#endif

		if (cam == null)
			cam = this.gameObject.GetComponent<Camera> ();

	}
	void Update(){

		if (cam == null)
			cam = this.gameObject.GetComponent<Camera> ();



		if (buff == null) {
			buff = new CommandBuffer ();
			buff.name = "Deferred decals";
			cam.AddCommandBuffer (CameraEvent.BeforeLighting, buff);
		} else {
			buff.Clear ();
		}

		DeferredDecalSystem system = DeferredDecalSystem.instance;
		int normalsID = Shader.PropertyToID ("_NormalsCopy");
		buff.GetTemporaryRT (normalsID, -1, -1);
		buff.Blit (BuiltinRenderTextureType.GBuffer2, normalsID);

		buff.SetRenderTarget (BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal decal in system.m_DecalsDiffuse) {
			buff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
		}

		buff.SetRenderTarget (BuiltinRenderTextureType.GBuffer2, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal decal in system.m_DecalsNormals) {
			buff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
		}

		RenderTargetIdentifier[] mrt = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer2 };
		buff.SetRenderTarget (mrt, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal decal in system.m_DecalsBoth) {
			buff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);

		}

		buff.ReleaseTemporaryRT (normalsID);

		#if UNITY_EDITOR

		if(SceneView.currentDrawingSceneView == null)
			return;

		if(editorCam == null){
			editorCam = SceneView.currentDrawingSceneView.camera;
		}

		if (editorBuff == null) {
			editorBuff = new CommandBuffer ();
			editorBuff.name = "Deferred decals";
			editorCam.AddCommandBuffer (CameraEvent.BeforeLighting, editorBuff);
		} else {
			editorBuff.Clear ();
		}

		editorBuff.GetTemporaryRT (normalsID, -1, -1);
		editorBuff.Blit (BuiltinRenderTextureType.GBuffer2, normalsID);

		editorBuff.SetRenderTarget (BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal decal in system.m_DecalsDiffuse) {
			editorBuff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
		}

		editorBuff.SetRenderTarget (BuiltinRenderTextureType.GBuffer2, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal decal in system.m_DecalsNormals) {
			editorBuff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
		}

		RenderTargetIdentifier[] editormrt = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer2 };
		editorBuff.SetRenderTarget (editormrt, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal decal in system.m_DecalsBoth) {
			editorBuff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);

		}
		editorBuff.ReleaseTemporaryRT (normalsID);
		#endif
	}
}


//		if (m_Cameras.ContainsKey (cam)) {
//			buff = m_Cameras [cam];
//			buff.Clear ();
//		} else {
//			buff = new CommandBuffer ();
//			buff.name = "Deferred decals";
//			m_Cameras [cam] = buff;
//			cam.AddCommandBuffer (CameraEvent.BeforeLighting, buff);
//		}


//		DeferredDecalSystem system = DeferredDecalSystem.instance;
//		int normalsID = Shader.PropertyToID ("_NormalsCopy");
//		buff.GetTemporaryRT (normalsID, -1, -1);
//		buff.Blit (BuiltinRenderTextureType.GBuffer2, normalsID);
//
//		buff.SetRenderTarget (BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.CameraTarget);
//		foreach (Decal decal in system.m_DecalsDiffuse) {
//			buff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
//		}
//
//		buff.SetRenderTarget (BuiltinRenderTextureType.GBuffer2, BuiltinRenderTextureType.CameraTarget);
//		foreach (Decal decal in system.m_DecalsNormals) {
//			buff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
//		}
//
//		RenderTargetIdentifier[] mrt = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer2 };
//		buff.SetRenderTarget (mrt, BuiltinRenderTextureType.CameraTarget);
//		foreach (Decal decal in system.m_DecalsBoth) {
//			buff.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
//
//		}
//
//		buff.ReleaseTemporaryRT (normalsID);
	//}



public class DeferredDecalSystem {
	static DeferredDecalSystem m_Instance;
	static public DeferredDecalSystem instance {
		get {

			if (m_Instance == null)
				m_Instance = new DeferredDecalSystem ();

			return m_Instance;
		}
	}

	internal HashSet<Decal> m_DecalsDiffuse = new HashSet<Decal> ();
	internal HashSet<Decal> m_DecalsNormals = new HashSet<Decal> ();
	internal HashSet<Decal> m_DecalsBoth = new HashSet<Decal> ();

	public void AddDecal(Decal d){
		RemoveDecal (d);

		if (d.m_DecalType == Decal.DecalType.DiffuseOnly)
			m_DecalsDiffuse.Add (d);

		if (d.m_DecalType == Decal.DecalType.NormalsOnly)
			m_DecalsNormals.Add (d);

		if (d.m_DecalType == Decal.DecalType.DiffuseAndNormals)
			m_DecalsBoth.Add (d);

	}

	public void RemoveDecal(Decal d){
		m_DecalsDiffuse.Remove (d);
		m_DecalsNormals.Remove (d);
		m_DecalsBoth.Remove (d);
	}
}
