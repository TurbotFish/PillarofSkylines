using UnityEngine.Rendering;
using UnityEngine;
using System;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class NormalBlendGaussian : MonoBehaviour {
	

	public Shader blurShader;

	[Range(0,200)]
	public int iterations;

	[Range(0,10f)]
	public float depthCheckRange = .01f;

	[NonSerialized]
	Material blurMat;

	Camera mainCam;
	CommandBuffer cBuffer;

	void OnEnable(){

		Initialize ();
		RemoveBuffer (mainCam, cBuffer);
		GenerateBuffer (mainCam, cBuffer);
	}

	void OnDisable(){
		RemoveBuffer (mainCam, cBuffer);
		DestroyImmediate (blurMat);
	}


	void OnValidate(){
		if (!enabled)
			return;

		RemoveBuffer (mainCam, cBuffer);
		Initialize ();
		blurMat.SetFloat ("_Range", depthCheckRange);
		GenerateBuffer (mainCam, cBuffer);
	}


	void RemoveBuffer(Camera _cam, CommandBuffer _buff){
		if (_cam == null)
			return;

		CommandBuffer[] _buffs = _cam.GetCommandBuffers (CameraEvent.BeforeLighting);
		foreach (CommandBuffer item in _buffs) {
			if (item.name == "Normal Blend Buffer Gaussian")
				_cam.RemoveCommandBuffer (CameraEvent.BeforeLighting, item);
		}
	}

	void Initialize(){

		if (mainCam == null)
			mainCam = GetComponent<Camera> ();

		//if (blurMat == null) {
			blurMat = new Material (blurShader);
			blurMat.hideFlags = HideFlags.HideAndDontSave;
		//}

		blurMat.SetFloat ("_Range", depthCheckRange);

		cBuffer = new CommandBuffer ();
		cBuffer.name = "Normal Blend Buffer Gaussian";

	}

	void GenerateBuffer(Camera _cam, CommandBuffer _buff){

		int _camWidth = _cam.pixelWidth;
		int _camHeight = _cam.pixelHeight;

		int _normalsCopyID0 = Shader.PropertyToID ("_NormalsCopyTexture0");

		_buff.GetTemporaryRT (_normalsCopyID0, _camWidth, _camHeight, 0, FilterMode.Bilinear);
		_buff.Blit (BuiltinRenderTextureType.GBuffer2, _normalsCopyID0, blurMat, 0);
		_buff.Blit (_normalsCopyID0, BuiltinRenderTextureType.GBuffer2, blurMat, 1);

		for (int i = 0; i < iterations; i++) {
			_buff.Blit (BuiltinRenderTextureType.GBuffer2, _normalsCopyID0, blurMat, 0);
			_buff.Blit (_normalsCopyID0, BuiltinRenderTextureType.GBuffer2, blurMat, 1);
		}

		//_buff.Blit (_normalsCopyID0, BuiltinRenderTextureType.GBuffer2);
		_buff.ReleaseTemporaryRT (_normalsCopyID0);

		_cam.AddCommandBuffer (CameraEvent.BeforeLighting, _buff);
	}
}
