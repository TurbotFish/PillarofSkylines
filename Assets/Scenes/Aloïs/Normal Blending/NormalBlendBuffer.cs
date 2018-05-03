using UnityEngine;
using UnityEngine.Rendering;
using System;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class NormalBlendBuffer : MonoBehaviour {

	//public bool ApplyOnSceneView = true;

	public Shader blurShader;

	[Range(0,10)]
	public int iterations;

	[Range(0,10f)]
	public float depthCheckRange = .01f;

	[NonSerialized]
	Material blurMat;

	Camera mainCam;
	//Camera sceneViewCam;
	CommandBuffer cBuffer;
	//CommandBuffer cBufferScene;

	void OnEnable(){

		Initialize ();
		RemoveBuffer (mainCam, cBuffer);
		GenerateBuffer (mainCam, cBuffer);

//		#if UNITY_EDITOR
//		RemoveBuffer (sceneViewCam, cBufferScene);
//		if (ApplyOnSceneView)
//			GenerateBuffer (sceneViewCam, cBufferScene);
//		
//		#endif
	}

	void OnDisable(){
		RemoveBuffer (mainCam, cBuffer);

//		#if UNITY_EDITOR
//
//		RemoveBuffer (sceneViewCam, cBufferScene);
//		#endif

		DestroyImmediate (blurMat);
	}


	void OnValidate(){
		if (!enabled)
			return;

		RemoveBuffer (mainCam, cBuffer);
		Initialize ();
		blurMat.SetFloat ("_Range", depthCheckRange);
		GenerateBuffer (mainCam, cBuffer);

//		#if UNITY_EDITOR
//		RemoveBuffer (sceneViewCam, cBufferScene);
//		if (ApplyOnSceneView) 
//			GenerateBuffer (sceneViewCam, cBufferScene);
//		#endif
	}


	void RemoveBuffer(Camera _cam, CommandBuffer _buff){
		if (_cam == null)
			return;

		CommandBuffer[] _buffs = _cam.GetCommandBuffers (CameraEvent.BeforeLighting);
		foreach (CommandBuffer item in _buffs) {
			if (item.name == "Normal Blend Buffer" || item.name == "Normal Blend Buffer Scene")
				_cam.RemoveCommandBuffer (CameraEvent.BeforeLighting, item);
		}
	}

	void Initialize(){
		
		if (mainCam == null)
			mainCam = GetComponent<Camera> ();

//		#if UNITY_EDITOR
//		if(sceneViewCam == null){
//			if(UnityEditor.SceneView.lastActiveSceneView.camera != null)
//				sceneViewCam = UnityEditor.SceneView.lastActiveSceneView.camera;
//		}
//		#endif

		if (blurMat == null) {
			blurMat = new Material (blurShader);
			blurMat.hideFlags = HideFlags.HideAndDontSave;
		}

		blurMat.SetFloat ("_Range", depthCheckRange);

		cBuffer = new CommandBuffer ();
		cBuffer.name = "Normal Blend Buffer";

			
//		#if UNITY_EDITOR
//		cBufferScene = new CommandBuffer ();
//		cBufferScene.name = "Normal Blend Buffer Scene";
//		#endif
	}

	void GenerateBuffer(Camera _cam, CommandBuffer _buff){

		int _camWidth = _cam.pixelWidth;
		int _camHeight = _cam.pixelHeight;

		int _normalsCopyID0 = Shader.PropertyToID ("_NormalsCopyTexture0");

		_buff.GetTemporaryRT (_normalsCopyID0, _camWidth, _camHeight, 0, FilterMode.Bilinear);
		_buff.Blit (BuiltinRenderTextureType.GBuffer2, _normalsCopyID0, blurMat);

		for (int i = 0; i < iterations; i++) {
			_buff.Blit (_normalsCopyID0, BuiltinRenderTextureType.GBuffer2, blurMat);
			_buff.Blit (BuiltinRenderTextureType.GBuffer2, _normalsCopyID0, blurMat);
		}

		_buff.Blit (_normalsCopyID0, BuiltinRenderTextureType.GBuffer2);
		_buff.ReleaseTemporaryRT (_normalsCopyID0);

		_cam.AddCommandBuffer (CameraEvent.BeforeLighting, _buff);
	}
}
