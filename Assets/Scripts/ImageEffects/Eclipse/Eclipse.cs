﻿using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/ColorOverlay (Grr)")]
public class Eclipse : MonoBehaviour {
	#region Public Properties

	[SerializeField]
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

    [SerializeField, Header("Vignetting")]
    float _power = 2;
    public float Power {
        get { return _power; }
        set { _power = value; }
    }

    [SerializeField]
    float _fallOff = 0.4f;
    public float FallOff {
        get { return _fallOff; }
        set { _fallOff = value; }
    }

    [SerializeField, Header("Sources")]
	Texture _noise;
	public Texture Noise {
		get { return _noise; }
		set { _noise = value; }
	}

	[SerializeField, Range(0,1), Header("Color Change")]
	float _colorChangeR;
	public float colorChangeR {
		get { return _colorChangeR; }
		set { _colorChangeR = value; }
	}
	[SerializeField, Range(0,1)]
	float _colorChangeG;
	public float colorChangeG {
		get { return _colorChangeG; }
		set { _colorChangeG = value; }
	}
	[SerializeField, Range(0,1)]
	float _colorChangeB;
	public float colorChangeB {
		get { return _colorChangeB; }
		set { _colorChangeB = value; }
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

    [HideInInspector]
    public Vector2 camSpeed;
    Vector2 lastCamSpeed;

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (_material == null) {
			_material = new Material(_shader);
			_material.hideFlags = HideFlags.DontSave;
		}
        
        camSpeed.x = Mathf.Clamp(-camSpeed.x, -1, 1);
        camSpeed.y = Mathf.Clamp(-camSpeed.y, -1, 1);

        camSpeed = Vector2.Lerp(lastCamSpeed, camSpeed, Time.deltaTime * 10);

		_material.SetFloat("_Threshold", _threshold);
		_material.SetFloat("_Intensity", _intensity);
		_material.SetFloat("_Deformation", _deformation);
		_material.SetVector("_Direction", _direction);
        _material.SetVector("_CameraSpeed", camSpeed);
        _material.SetFloat("_Speed", _speed);
		_material.SetInt("_Iterations", _iterations);
		_material.SetTexture("_Noise", _noise);
        _material.SetFloat("_Falloff", _fallOff);
        _material.SetFloat("_Power", _power);
		_material.SetFloat("_ColorChangeR", _colorChangeR);
		_material.SetFloat("_ColorChangeG", _colorChangeG);
		_material.SetFloat("_ColorChangeB", _colorChangeB);

        Graphics.Blit(source, destination, _material, 0);

        lastCamSpeed = camSpeed;
    }

	#endregion
}
