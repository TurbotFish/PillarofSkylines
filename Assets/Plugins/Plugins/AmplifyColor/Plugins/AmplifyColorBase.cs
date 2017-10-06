// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4  || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4  || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
#define UNITY_5
#endif

using AmplifyColor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyColor
{
	public enum Quality
	{
		Mobile,
		Standard
	}
}

[AddComponentMenu( "" )]
public class AmplifyColorBase : MonoBehaviour
{
	public const int LutSize = 32;
	public const int LutWidth = LutSize * LutSize;
	public const int LutHeight = LutSize;
	const int DepthCurveLutRange = 1024;

	// HDR Control
	public float Exposure = 1.0f;
	public bool UseToneMapping = false;
	public bool UseDithering = false;

	// Color Grading
	public AmplifyColor.Quality QualityLevel = AmplifyColor.Quality.Standard;
	public float BlendAmount = 0f;
	public Texture LutTexture = null;
	public Texture LutBlendTexture = null;
	public Texture MaskTexture = null;
	public bool UseDepthMask = false;
	public AnimationCurve DepthMaskCurve = new AnimationCurve( new Keyframe( 0, 1 ), new Keyframe( 1, 1 ) );

	// Effect Volumes
	public bool UseVolumes = false;
	public float ExitVolumeBlendTime = 1.0f;
	public Transform TriggerVolumeProxy = null;
	public LayerMask VolumeCollisionMask = ~0;

	private Camera ownerCamera = null;
	private Shader shaderBase = null;
	private Shader shaderBlend = null;
	private Shader shaderBlendCache = null;
	private Shader shaderMask = null;
	private Shader shaderMaskBlend = null;
	private Shader shaderDepthMask = null;
	private Shader shaderDepthMaskBlend = null;
	private Shader shaderProcessOnly = null;
	private RenderTexture blendCacheLut = null;
	private Texture2D defaultLut = null;
	private Texture2D depthCurveLut = null;
	private Color32[] depthCurveColors = null;
	private ColorSpace colorSpace = ColorSpace.Uninitialized;
	private AmplifyColor.Quality qualityLevel = AmplifyColor.Quality.Standard;

	public Texture2D DefaultLut { get { return ( defaultLut == null ) ? CreateDefaultLut() : defaultLut ; } }

	private Material materialBase = null;
	private Material materialBlend = null;
	private Material materialBlendCache = null;
	private Material materialMask = null;
	private Material materialMaskBlend = null;
	private Material materialDepthMask = null;
	private Material materialDepthMaskBlend = null;
	private Material materialProcessOnly = null;

	private bool blending;
	private float blendingTime;
	private float blendingTimeCountdown;
	private System.Action onFinishBlend;

	private AnimationCurve prevDepthMaskCurve = new AnimationCurve();

	private bool volumesBlending;
	private float volumesBlendingTime;
	private float volumesBlendingTimeCountdown;
	private Texture volumesLutBlendTexture = null;
	private float volumesBlendAmount = 0f;

	public bool IsBlending { get { return blending; } }

	private Texture worldLUT = null;
	private AmplifyColorVolumeBase currentVolumeLut = null;
	private RenderTexture midBlendLUT = null;
	private bool blendingFromMidBlend = false;

	private VolumeEffect worldVolumeEffects = null;
	private VolumeEffect currentVolumeEffects = null;
	private VolumeEffect blendVolumeEffects = null;
	private float worldExposure = 1.0f;
	private float currentExposure = 1.0f;
	private float blendExposure = 1.0f;
	private float effectVolumesBlendAdjust = 0.0f;
	private float effectVolumesBlendAdjusted { get { return Mathf.Clamp01( effectVolumesBlendAdjust < 0.99f ? ( volumesBlendAmount - effectVolumesBlendAdjust ) / ( 1.0f - effectVolumesBlendAdjust ) : 1.0f ); } }
	private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();
	private AmplifyColorTriggerProxyBase actualTriggerProxy = null;

	[HideInInspector] public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();

	[SerializeField, HideInInspector] private string sharedInstanceID = "";
	public string SharedInstanceID { get { return sharedInstanceID; } }

#if TRIAL
	private Texture2D watermark = null;
#endif

	public bool WillItBlend { get { return LutTexture != null && LutBlendTexture != null && !blending; } }

	public void NewSharedInstanceID()
	{
		sharedInstanceID = Guid.NewGuid().ToString();
	}

	void ReportMissingShaders()
	{
		Debug.LogError( "[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color." );
		enabled = false;
	}

	void ReportNotSupported()
	{
		#if UNITY_4
			Debug.LogWarning( "[AmplifyColor] This image effect is not supported on this platform. Please make sure your Unity 4 license supports Full-Screen Post-Processing Effects which is usually reserved forn Pro licenses." );
		#else
			Debug.LogWarning( "[AmplifyColor] This image effect is not supported on this platform." );
		#endif
		enabled = false;
	}

	bool CheckShader( Shader s )
	{
		if ( s == null )
		{
			ReportMissingShaders();
			return false;
		}
		if ( !s.isSupported )
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	bool CheckShaders()
	{
		return CheckShader( shaderBase ) && CheckShader( shaderBlend ) && CheckShader( shaderBlendCache ) &&
			CheckShader( shaderMask ) && CheckShader( shaderMaskBlend ) && CheckShader( shaderProcessOnly );
	}

	bool CheckSupport()
	{
		// Disable if we don't support image effect or render textures
		if ( !SystemInfo.supportsImageEffects)
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	void OnEnable()
	{
		if ( !CheckSupport() )
			return;

		if ( !CreateMaterials() )
			return;

		Texture2D lutTex2d = LutTexture as Texture2D;
		Texture2D lutBlendTex2d = LutBlendTexture as Texture2D;

		if ( ( lutTex2d != null && lutTex2d.mipmapCount > 1 ) || ( lutBlendTex2d != null && lutBlendTex2d.mipmapCount > 1 ) )
			Debug.LogError( "[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. " +
				"Change Texture Type to \"Advanced\" to access Mip settings." );

	#if TRIAL
		watermark = new Texture2D( 4, 4 ) { hideFlags = HideFlags.HideAndDontSave };
		watermark.LoadImage( AmplifyColor.Watermark.ImageData );
	#endif
	}

	void OnDisable()
	{
		if ( actualTriggerProxy != null )
		{
			DestroyImmediate( actualTriggerProxy.gameObject );
			actualTriggerProxy = null;
		}

		ReleaseMaterials();
		ReleaseTextures();

	#if TRIAL
		if ( watermark != null )
		{
			DestroyImmediate( watermark );
			watermark = null;
		}
	#endif
	}

	private void VolumesBlendTo( Texture blendTargetLUT, float blendTimeInSec )
	{
		volumesLutBlendTexture = blendTargetLUT;
		volumesBlendAmount = 0.0f;
		volumesBlendingTime = blendTimeInSec;
		volumesBlendingTimeCountdown = blendTimeInSec;
		volumesBlending = true;

	}

	public void BlendTo( Texture blendTargetLUT, float blendTimeInSec, System.Action onFinishBlend )
	{
		LutBlendTexture = blendTargetLUT;
		BlendAmount = 0.0f;
		this.onFinishBlend = onFinishBlend;
		blendingTime = blendTimeInSec;
		blendingTimeCountdown = blendTimeInSec;
		blending = true;
	}

	private void CheckCamera()
	{
		if  ( ownerCamera == null )
			ownerCamera = GetComponent<Camera>();

		if ( UseDepthMask && ( ownerCamera.depthTextureMode & DepthTextureMode.Depth ) == 0 )
			ownerCamera.depthTextureMode |= DepthTextureMode.Depth;
	}

	private void Start()
	{
		CheckCamera();

		worldLUT = LutTexture;

		worldVolumeEffects = EffectFlags.GenerateEffectData( this );
		blendVolumeEffects = currentVolumeEffects = worldVolumeEffects;

		worldExposure = Exposure;
		blendExposure = currentExposure = worldExposure;
	}

	void Update()
	{
		CheckCamera();

		bool volumesBlendFinished = false;
		if ( volumesBlending )
		{
			volumesBlendAmount = ( volumesBlendingTime - volumesBlendingTimeCountdown ) / volumesBlendingTime;
			volumesBlendingTimeCountdown -= Time.smoothDeltaTime;

			if ( volumesBlendAmount >= 1.0f )
			{
				volumesBlendAmount = 1;
				volumesBlendFinished = true;
			}
		}
		else
			volumesBlendAmount = Mathf.Clamp01( volumesBlendAmount );

		if ( blending )
		{
			BlendAmount = ( blendingTime - blendingTimeCountdown ) / blendingTime;
			blendingTimeCountdown -= Time.smoothDeltaTime;

			if ( BlendAmount >= 1.0f )
			{
				LutTexture = LutBlendTexture;
				BlendAmount = 0.0f;
				blending = false;
				LutBlendTexture = null;

				if ( onFinishBlend != null )
					onFinishBlend();
			}
		}
		else
			BlendAmount = Mathf.Clamp01( BlendAmount );

		if ( UseVolumes )
		{
			if ( actualTriggerProxy == null )
			{
				GameObject obj = new GameObject( name + "+ACVolumeProxy" ) { hideFlags = HideFlags.HideAndDontSave };
				if ( TriggerVolumeProxy != null && TriggerVolumeProxy.GetComponent<Collider2D>() != null )
					actualTriggerProxy = obj.AddComponent<AmplifyColorTriggerProxy2D>();
				else
					actualTriggerProxy = obj.AddComponent<AmplifyColorTriggerProxy>();
				actualTriggerProxy.OwnerEffect = this;
			}

			UpdateVolumes();
		}
		else if ( actualTriggerProxy != null )
		{
			DestroyImmediate( actualTriggerProxy.gameObject );
			actualTriggerProxy = null;
		}

		if ( volumesBlendFinished )
		{
			LutTexture = volumesLutBlendTexture;
			volumesBlendAmount = 0.0f;
			volumesBlending = false;
			volumesLutBlendTexture = null;

			effectVolumesBlendAdjust = 0.0f;
			currentVolumeEffects = blendVolumeEffects;
			currentVolumeEffects.SetValues( this );
			currentExposure = blendExposure;

			if ( blendingFromMidBlend && midBlendLUT != null )
				midBlendLUT.DiscardContents();

			blendingFromMidBlend = false;
		}
	}

	public void EnterVolume( AmplifyColorVolumeBase volume )
	{
		if ( !enteredVolumes.Contains( volume ) )
			enteredVolumes.Insert( 0, volume );
	}

	public void ExitVolume( AmplifyColorVolumeBase volume )
	{
		if ( enteredVolumes.Contains( volume ) )
			enteredVolumes.Remove( volume );
	}

	private void UpdateVolumes()
	{
		if ( volumesBlending )
			currentVolumeEffects.BlendValues( this, blendVolumeEffects, effectVolumesBlendAdjusted );

		if ( volumesBlending )
			Exposure = Mathf.Lerp( currentExposure, blendExposure, effectVolumesBlendAdjusted );

		Transform reference = ( TriggerVolumeProxy == null ) ? transform : TriggerVolumeProxy;
		if ( actualTriggerProxy.transform.parent != reference )
		{
			actualTriggerProxy.Reference = reference;
			actualTriggerProxy.gameObject.layer = reference.gameObject.layer;
		}

		AmplifyColorVolumeBase foundVolume = null;
		int maxPriority = int.MinValue;

		// Find volume with higher priority
		for ( int i = 0; i < enteredVolumes.Count; i++ )
		{
			AmplifyColorVolumeBase vol = enteredVolumes[ i ];
			if ( vol.Priority > maxPriority )
			{
				foundVolume = vol;
				maxPriority = vol.Priority;
			}
		}

		// Trigger blend on volume transition
		if ( foundVolume != currentVolumeLut )
		{
			currentVolumeLut = foundVolume;
			Texture blendTex = ( foundVolume == null ? worldLUT : foundVolume.LutTexture );
			float blendTime = ( foundVolume == null ? ExitVolumeBlendTime : foundVolume.EnterBlendTime );

			if ( volumesBlending && !blendingFromMidBlend && blendTex == LutTexture )
			{
				// Going back to previous volume optimization
				LutTexture = volumesLutBlendTexture;
				volumesLutBlendTexture = blendTex;
				volumesBlendingTimeCountdown = blendTime * ( ( volumesBlendingTime - volumesBlendingTimeCountdown ) / volumesBlendingTime );
				volumesBlendingTime = blendTime;
				currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect( EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted );
				currentExposure = Mathf.Lerp( currentExposure, blendExposure, effectVolumesBlendAdjusted );
				effectVolumesBlendAdjust = 1 - volumesBlendAmount;
				volumesBlendAmount = 1 - volumesBlendAmount;
			}
			else
			{
				if ( volumesBlending )
				{
					materialBlendCache.SetFloat( "_LerpAmount", volumesBlendAmount );

					if ( blendingFromMidBlend )
					{
						Graphics.Blit( midBlendLUT, blendCacheLut );
						materialBlendCache.SetTexture( "_RgbTex", blendCacheLut );
					}
					else
						materialBlendCache.SetTexture( "_RgbTex", LutTexture );

					materialBlendCache.SetTexture( "_LerpRgbTex", ( volumesLutBlendTexture != null ) ? volumesLutBlendTexture : defaultLut );

					Graphics.Blit( midBlendLUT, midBlendLUT, materialBlendCache );

					blendCacheLut.DiscardContents();
				#if UNITY_4
					midBlendLUT.MarkRestoreExpected();
				#endif

					currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect( EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted );
					currentExposure = Mathf.Lerp( currentExposure, blendExposure, effectVolumesBlendAdjusted );
					effectVolumesBlendAdjust = 0.0f;
					blendingFromMidBlend = true;
				}
				VolumesBlendTo( blendTex, blendTime );
			}

			blendVolumeEffects = ( foundVolume == null ) ? worldVolumeEffects : foundVolume.EffectContainer.FindVolumeEffect( this );
			blendExposure = ( foundVolume == null ) ? worldExposure : foundVolume.Exposure;
			if ( blendVolumeEffects == null )
				blendVolumeEffects = worldVolumeEffects;
		}
	}

	private void SetupShader()
	{
		colorSpace = QualitySettings.activeColorSpace;
		qualityLevel = QualityLevel;

		shaderBase = Shader.Find( "Hidden/Amplify Color/Base" );
		shaderBlend = Shader.Find( "Hidden/Amplify Color/Blend" );
		shaderBlendCache = Shader.Find( "Hidden/Amplify Color/BlendCache" );
		shaderMask = Shader.Find( "Hidden/Amplify Color/Mask" );
		shaderMaskBlend = Shader.Find( "Hidden/Amplify Color/MaskBlend" );
		shaderDepthMask = Shader.Find( "Hidden/Amplify Color/DepthMask" );
		shaderDepthMaskBlend = Shader.Find( "Hidden/Amplify Color/DepthMaskBlend" );
		shaderProcessOnly = Shader.Find( "Hidden/Amplify Color/ProcessOnly" );
	}

	private void ReleaseMaterials()
	{
		SafeRelease( ref materialBase );
		SafeRelease( ref materialBlend );
		SafeRelease( ref materialBlendCache );
		SafeRelease( ref materialMask );
		SafeRelease( ref materialMaskBlend );
		SafeRelease( ref materialDepthMask );
		SafeRelease( ref materialDepthMaskBlend );
		SafeRelease( ref materialProcessOnly );
	}

	private Texture2D CreateDefaultLut()
	{
		const int maxSize = LutSize - 1;

		defaultLut = new Texture2D( LutWidth, LutHeight, TextureFormat.RGB24, false, true ) { hideFlags = HideFlags.HideAndDontSave };
		defaultLut.name = "DefaultLut";
		defaultLut.hideFlags = HideFlags.DontSave;
		defaultLut.anisoLevel = 1;
		defaultLut.filterMode = FilterMode.Bilinear;
		Color32[] colors = new Color32[ LutWidth * LutHeight ];

		for ( int z = 0; z < LutSize; z++ )
		{
			int zoffset = z * LutSize;

			for ( int y = 0; y < LutSize; y++ )
			{
				int yoffset = zoffset + y * LutWidth;

				for ( int x = 0; x < LutSize; x++ )
				{
					float fr = x / ( float ) maxSize;
					float fg = y / ( float ) maxSize;
					float fb = z / ( float ) maxSize;
					byte br = ( byte ) ( fr * 255 );
					byte bg = ( byte ) ( fg * 255 );
					byte bb = ( byte ) ( fb * 255 );
					colors[ yoffset + x ] = new Color32( br, bg, bb, 255 );
				}
			}
		}

		defaultLut.SetPixels32( colors );
		defaultLut.Apply();

		return defaultLut;
	}

	private Texture2D CreateDepthCurveLut()
	{
		SafeRelease( ref depthCurveLut );

		depthCurveLut = new Texture2D( DepthCurveLutRange, 1, TextureFormat.Alpha8, false, true ) { hideFlags = HideFlags.HideAndDontSave };
		depthCurveLut.name = "DepthCurveLut";
		depthCurveLut.hideFlags = HideFlags.DontSave;
		depthCurveLut.anisoLevel = 1;
		depthCurveLut.wrapMode = TextureWrapMode.Clamp;
		depthCurveLut.filterMode = FilterMode.Bilinear;

		depthCurveColors = new Color32[ DepthCurveLutRange ];

		return depthCurveLut;
	}

	private void UpdateDepthCurveLut()
	{
		if ( depthCurveLut == null )
			CreateDepthCurveLut();

		const float rcpMaxRange = 1.0f / ( DepthCurveLutRange - 1 );
		float time = 0;

		for ( int x = 0; x < DepthCurveLutRange; x++, time += rcpMaxRange )
			depthCurveColors[ x ].a = ( byte ) Mathf.FloorToInt( Mathf.Clamp01( DepthMaskCurve.Evaluate( time ) ) * 255.0f );

		depthCurveLut.SetPixels32( depthCurveColors );
		depthCurveLut.Apply();
	}

	private void CheckUpdateDepthCurveLut()
	{
		// check if keyframes differ
		bool changed = false;
		if ( DepthMaskCurve.length != prevDepthMaskCurve.length )
			changed = true;
		else
		{
			const float rcpMaxRange = 1.0f / ( DepthCurveLutRange - 1 );
			float time = 0;

			for ( int i = 0; i < DepthMaskCurve.length; i++, time += rcpMaxRange )
			{
				if ( Mathf.Abs( DepthMaskCurve.Evaluate( time ) - prevDepthMaskCurve.Evaluate( time ) ) > float.Epsilon )
				{
					changed = true;
					break;
				}
			}
		}

		if ( depthCurveLut == null || changed )
		{
			// update curve lut texture
			UpdateDepthCurveLut();
			prevDepthMaskCurve = new AnimationCurve( DepthMaskCurve.keys );
		}
	}

	private void CreateHelperTextures()
	{
		ReleaseTextures();

		blendCacheLut = new RenderTexture( LutWidth, LutHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear ) { hideFlags = HideFlags.HideAndDontSave };
		blendCacheLut.name = "BlendCacheLut";
		blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		blendCacheLut.useMipMap = false;
		blendCacheLut.anisoLevel = 0;
		blendCacheLut.Create();

		midBlendLUT = new RenderTexture( LutWidth, LutHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear ) { hideFlags = HideFlags.HideAndDontSave };
		midBlendLUT.name = "MidBlendLut";
		midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		midBlendLUT.useMipMap = false;
		midBlendLUT.anisoLevel = 0;
		midBlendLUT.Create();
	#if UNITY_4
		midBlendLUT.MarkRestoreExpected();
	#endif

		CreateDefaultLut();

		if ( UseDepthMask )
			CreateDepthCurveLut();
	}

	bool CheckMaterialAndShader( Material material, string name )
	{
		if ( material == null || material.shader == null )
		{
			Debug.LogWarning( "[AmplifyColor] Error creating " + name + " material. Effect disabled." );
			enabled = false;
		}
		else if ( !material.shader.isSupported )
		{
			Debug.LogWarning( "[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled." );
			enabled = false;
		}
		else
		{
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		return enabled;
	}

	private bool CreateMaterials()
	{
		SetupShader();
		if ( !CheckShaders() )
			return false;

		ReleaseMaterials();

		materialBase = new Material( shaderBase );
		materialBlend = new Material( shaderBlend );
		materialBlendCache = new Material( shaderBlendCache );
		materialMask = new Material( shaderMask );
		materialMaskBlend = new Material( shaderMaskBlend );
		materialDepthMask = new Material( shaderDepthMask );
		materialDepthMaskBlend = new Material( shaderDepthMaskBlend );
		materialProcessOnly = new Material( shaderProcessOnly );

		bool ok = true;
		ok = ok && CheckMaterialAndShader( materialBase, "BaseMaterial" );
		ok = ok && CheckMaterialAndShader( materialBlend, "BlendMaterial" );
		ok = ok && CheckMaterialAndShader( materialBlendCache, "BlendCacheMaterial" );
		ok = ok && CheckMaterialAndShader( materialMask, "MaskMaterial" );
		ok = ok && CheckMaterialAndShader( materialMaskBlend, "MaskBlendMaterial" );
		ok = ok && CheckMaterialAndShader( materialDepthMask, "DepthMaskMaterial" );
		ok = ok && CheckMaterialAndShader( materialDepthMaskBlend, "DepthMaskBlendMaterial" );
		ok = ok && CheckMaterialAndShader( materialProcessOnly, "ProcessOnlyMaterial" );

		if ( !ok )
			return false;

		CreateHelperTextures();
		return true;
	}

	void SetMaterialKeyword( string keyword, bool state )
	{
	#if UNITY_4
		if ( state )
			Shader.EnableKeyword( keyword );
		else
			Shader.DisableKeyword( keyword );
	#else
		bool keywordEnabled = materialBase.IsKeywordEnabled( keyword );
		if ( state && !keywordEnabled )
		{
			materialBase.EnableKeyword( keyword );
			materialBlend.EnableKeyword( keyword );
			materialBlendCache.EnableKeyword( keyword );
			materialMask.EnableKeyword( keyword );
			materialMaskBlend.EnableKeyword( keyword );
			materialDepthMask.EnableKeyword( keyword );
			materialDepthMaskBlend.EnableKeyword( keyword );
			materialProcessOnly.EnableKeyword( keyword );
		}
		else if ( !state && materialBase.IsKeywordEnabled( keyword ) )
		{
			materialBase.DisableKeyword( keyword );
			materialBlend.DisableKeyword( keyword );
			materialBlendCache.DisableKeyword( keyword );
			materialMask.DisableKeyword( keyword );
			materialMaskBlend.DisableKeyword( keyword );
			materialDepthMask.DisableKeyword( keyword );
			materialDepthMaskBlend.DisableKeyword( keyword );
			materialProcessOnly.DisableKeyword( keyword );
		}
	#endif
	}

	void UpdateShaderKeywords()
	{
		SetMaterialKeyword( "AC_QUALITY_MOBILE", QualityLevel == AmplifyColor.Quality.Mobile );
		SetMaterialKeyword( "AC_DITHERING", UseDithering );
		SetMaterialKeyword( "AC_TONEMAPPING", UseToneMapping );
	}

	private void SafeRelease<T>( ref T obj ) where T : UnityEngine.Object
	{
		if ( obj != null )
		{
			DestroyImmediate( obj );
			obj = null;
		}
	}

	private void ReleaseTextures()
	{
		SafeRelease( ref blendCacheLut );
		SafeRelease( ref midBlendLUT );
		SafeRelease( ref defaultLut );
		SafeRelease( ref depthCurveLut );
	}

	public static bool ValidateLutDimensions( Texture lut )
	{
		bool valid = true;
		if ( lut != null )
		{
			if ( ( lut.width / lut.height ) != lut.height )
			{
				Debug.LogWarning( "[AmplifyColor] Lut " + lut.name + " has invalid dimensions." );
				valid = false;
			}
			else
			{
				if ( lut.anisoLevel != 0 )
					lut.anisoLevel = 0;
			}
		}
		return valid;
	}

	private void UpdatePostEffectParams()
	{
		if ( UseDepthMask )
			CheckUpdateDepthCurveLut();

		Exposure = Mathf.Max( Exposure, 0 );
	}

	private void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
		BlendAmount = Mathf.Clamp01( BlendAmount );

		if ( colorSpace != QualitySettings.activeColorSpace || qualityLevel != QualityLevel )
			CreateMaterials();

		UpdatePostEffectParams();
		UpdateShaderKeywords();

		bool validLut = ValidateLutDimensions( LutTexture );
		bool validLutBlend = ValidateLutDimensions( LutBlendTexture );
		bool skip = ( LutTexture == null && LutBlendTexture == null && volumesLutBlendTexture == null );

		Texture lut = ( LutTexture == null ) ? defaultLut : LutTexture;
		Texture lutBlend = LutBlendTexture;
		int pass = ( colorSpace == ColorSpace.Linear ? 2 : 0 ) + ( ownerCamera.allowHDR ? 1 : 0 );

		bool blend = ( BlendAmount != 0.0f ) || blending;
		bool requiresBlend = blend || ( blend && lutBlend != null );
		bool useBlendCache = requiresBlend;
		bool processOnly = !validLut || !validLutBlend || skip;

		Material material;
		if ( processOnly )
		{
			material = materialProcessOnly;
		}
		else
		{
			if ( requiresBlend || volumesBlending )
			{
				if ( UseDepthMask )
					material = materialDepthMaskBlend;
				else
					material = ( MaskTexture != null ) ? materialMaskBlend : materialBlend;
			}
			else
			{
				if ( UseDepthMask )
					material = materialDepthMask;
				else
					material = ( MaskTexture != null ) ? materialMask : materialBase;
			}
		}

		material.SetFloat( "_Exposure", Exposure );
		material.SetFloat( "_LerpAmount", BlendAmount );
		if ( MaskTexture != null )
			material.SetTexture( "_MaskTex", MaskTexture );
		if ( UseDepthMask )
			material.SetTexture( "_DepthCurveLut", depthCurveLut );

		if ( !processOnly )
		{
			if ( volumesBlending )
			{
				volumesBlendAmount = Mathf.Clamp01( volumesBlendAmount );
				materialBlendCache.SetFloat( "_LerpAmount", volumesBlendAmount );

				if ( blendingFromMidBlend )
					materialBlendCache.SetTexture( "_RgbTex", midBlendLUT );
				else
					materialBlendCache.SetTexture( "_RgbTex", lut );

				materialBlendCache.SetTexture( "_LerpRgbTex", ( volumesLutBlendTexture != null ) ? volumesLutBlendTexture : defaultLut );

				Graphics.Blit( lut, blendCacheLut, materialBlendCache );
			}

			if ( useBlendCache )
			{
				materialBlendCache.SetFloat( "_LerpAmount", BlendAmount );

				RenderTexture temp = null;
				if ( volumesBlending )
				{
					temp = RenderTexture.GetTemporary( blendCacheLut.width, blendCacheLut.height, blendCacheLut.depth, blendCacheLut.format, RenderTextureReadWrite.Linear );

					Graphics.Blit( blendCacheLut, temp );

					materialBlendCache.SetTexture( "_RgbTex", temp );
				}
				else
					materialBlendCache.SetTexture( "_RgbTex", lut );

				materialBlendCache.SetTexture( "_LerpRgbTex", ( lutBlend != null ) ? lutBlend : defaultLut );

				Graphics.Blit( lut, blendCacheLut, materialBlendCache );

				if ( temp != null )
					RenderTexture.ReleaseTemporary( temp );

				material.SetTexture( "_RgbBlendCacheTex", blendCacheLut );

			}
			else if ( volumesBlending )
			{
				material.SetTexture( "_RgbBlendCacheTex", blendCacheLut );
			}
			else
			{
				if ( lut != null )
					material.SetTexture( "_RgbTex", lut );
				if ( lutBlend != null )
					material.SetTexture( "_LerpRgbTex", lutBlend );
			}
		}

		Graphics.Blit( source, destination, material, pass );

		if ( useBlendCache || volumesBlending )
			blendCacheLut.DiscardContents();
	}

#if TRIAL
	void OnGUI()
	{
		if ( watermark != null )
			GUI.DrawTexture( new Rect( 15, Screen.height - watermark.height - 12, watermark.width, watermark.height ), watermark );
	}
#endif
}
