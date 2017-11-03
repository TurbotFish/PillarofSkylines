﻿using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/ColorOverlay (Grr)")]
public class Eclipse : MonoBehaviour {
	#region Public Properties

	[SerializeField, Range(-1.0f, 1.0f)]
	float _threshold = 0f;
	public float Threshold {
		get { return _threshold; }
		set { _threshold = value; }
	}

	[SerializeField, Range(0.0f, 1.0f)]
	float _intensity = 1f;
	public float Intensity {
		get { return _intensity; }
		set { _intensity = value; }
	}

	[SerializeField, Range(0.0f, 1.0f)]
	float _deformation = 0.07f;
	public float Deformation {
		get { return _deformation; }
		set { _deformation = value; }
	}
	
	[SerializeField]
	Vector2 _direction = new Vector2(-.3f, -.2f);
	public Vector2 Direction {
		get { return _direction; }
		set { _direction = value; }
	}

	[SerializeField, Range(0.0f, 1.0f)]
	float _speed = 0.2f;
	public float Speed {
		get { return _speed; }
		set { _speed = value; }
	}

	[SerializeField]
	int _iterations = 15;
	public int Iterations {
		get { return _iterations; }
		set { _iterations = value; }
	}

	[SerializeField]
	Texture _noise;
	public Texture Noise {
		get { return _noise; }
		set { _noise = value; }
	}

	#endregion

	#region Private Properties

	[SerializeField] Shader _shader;
	Material _material;
	
	#endregion

	#region MonoBehaviour Functions

	void OnValidate() {
		if (_material == null) {
			_material = new Material(_shader);
			_material.hideFlags = HideFlags.DontSave;
		}
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (_material == null) {
			_material = new Material(_shader);
			_material.hideFlags = HideFlags.DontSave;
		}

		_material.SetFloat("_Threshold", _threshold);
		_material.SetFloat("_Intensity", _intensity);
		_material.SetFloat("_Deformation", _deformation);
		_material.SetVector("_Direction", _direction);
		_material.SetFloat("_Speed", _speed);
		_material.SetInt("_Iterations", _iterations);
		_material.SetTexture("_Noise", _noise);

		Graphics.Blit(source, destination, _material, 0);
	}

	#endregion
}