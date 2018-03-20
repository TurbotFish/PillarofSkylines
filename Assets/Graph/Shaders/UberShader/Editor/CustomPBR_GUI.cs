using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomPBR_GUI : ShaderGUI {

	Material target;
	MaterialEditor editor;
	MaterialProperty[] properties;
	static ColorPickerHDRConfig emissionConfig = new ColorPickerHDRConfig (0f, 99f, 1f / 99f, 3f);

	bool shouldShowAlphaCutoff;
	MaterialProperty _culling;

	public override void OnGUI (MaterialEditor editor, MaterialProperty[] properties){
		this.target = editor.target as Material;
		this.editor = editor;
		this.properties = properties;


		target.SetShaderPassEnabled ("Always", false);


		DoRenderingMode ();

		DoCulling ();
		DoCelShading ();
		DoRefraction ();
		DoSubSurfaceScattering ();
		DoMain ();
		DoSecondary ();
		DoVertexOffset ();
		DoDistanceDither ();
		DoDebug ();

		//unity bug
		//DoDynamicBatching ();
		DoIsPlayer();
		DoRenderQueue ();

		//Debug.Log ("HELLOOOO");
	}

	void DoMain(){
		GUILayout.Label ("Main Maps", EditorStyles.boldLabel);

		MaterialProperty mainTex = FindProperty ("_MainTex", properties);
		DoAlbedoVertexMask ();
		editor.TexturePropertySingleLine (MakeLabel(mainTex, "Albedo (RGB)"), mainTex, FindProperty("_Color"));
		if (shouldShowAlphaCutoff) {
			DoAlphaCutoff ();
		}

		DoMetallic ();
		DoSmoothness ();
		DoNormals ();
		//DoFadeNormals ();
		//DoOcclusion ();
		DoEmission ();
		DoDetailMask ();

		DoRimLight ();
		DoWallTint ();
		DoGroundTint ();
		DoWorldPosAlbedo ();
		editor.TextureScaleOffsetProperty (mainTex);
	}

	void DoWorldPosAlbedo(){
		EditorGUI.BeginChangeCheck ();
		bool useWP = EditorGUILayout.Toggle (MakeLabel ("WorldPos Albedo"), IsKeywordEnabled ("_ALBEDO_WORLDPOS"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_ALBEDO_WORLDPOS", useWP);
		}
	}

	void DoAlbedoVertexMask(){
		EditorGUI.BeginChangeCheck ();
		bool vertexMask = EditorGUILayout.Toggle (MakeLabel ("Vertex Colour Mask"), IsKeywordEnabled ("_ALBEDO_VERTEX_MASK"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_ALBEDO_VERTEX_MASK", vertexMask);
		}
	}

	void DoWallTint(){

		MaterialProperty tintPow = FindProperty ("_WallTintPow");
		MaterialProperty tintCol = FindProperty ("_WallTintCol");

		EditorGUI.BeginChangeCheck ();
		bool wallTint = EditorGUILayout.Toggle (MakeLabel ("Wall Tint"), IsKeywordEnabled ("_WALL_TINT"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_WALL_TINT", wallTint);
		}
		EditorGUI.indentLevel += 1;
		if (wallTint) {
			editor.ShaderProperty (tintPow, MakeLabel ("Power"));
			editor.ShaderProperty (tintCol, MakeLabel ("Colour"));
		}
		EditorGUI.indentLevel -= 1;
	}

	void DoGroundTint(){

		MaterialProperty tintPow = FindProperty ("_GroundTintPow");
		MaterialProperty tintCol = FindProperty ("_GroundTintCol");

		EditorGUI.BeginChangeCheck ();
		bool groundTint = EditorGUILayout.Toggle (MakeLabel ("Ground Tint"), IsKeywordEnabled ("_GROUND_TINT"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_GROUND_TINT", groundTint);
		}
		EditorGUI.indentLevel += 1;
		if (groundTint) {
			editor.ShaderProperty (tintPow, MakeLabel ("Power"));
			editor.ShaderProperty (tintCol, MakeLabel ("Colour"));
		}
		EditorGUI.indentLevel -= 1;
	}

	void DoRimLight(){
		MaterialProperty pow = FindProperty ("_RimPow");
		MaterialProperty scale = FindProperty ("_RimScale");
		MaterialProperty rimCol = FindProperty ("_RimColor");

		EditorGUI.BeginChangeCheck ();
		bool rimLit = EditorGUILayout.Toggle (MakeLabel ("Rim Light"), IsKeywordEnabled ("_RIMLIT"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_RIMLIT", rimLit);
		}

		EditorGUI.indentLevel += 1;
		if (rimLit) {
			editor.ShaderProperty (rimCol, MakeLabel (rimCol));
			editor.ShaderProperty (pow, MakeLabel (pow));
			editor.ShaderProperty (scale, MakeLabel (scale));

		}
		EditorGUI.indentLevel -= 1;
	}

	void DoDebug(){
		GUILayout.Label ("Debug", EditorStyles.boldLabel);

		DoLocalNormalDebug ();
		DoCheckerDebug ();
	}

	void DoAlphaCutoff(){
		MaterialProperty slider = FindProperty ("_Cutoff");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty (slider, MakeLabel (slider));
		EditorGUI.indentLevel -= 2;
	}

	void DoSecondary(){
		GUILayout.Label ("Secondary Maps", EditorStyles.boldLabel);

		MaterialProperty detailTex = FindProperty ("_DetailTex");
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (MakeLabel (detailTex, "Albedo (RGB) multiplied by 2"), detailTex);
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_DETAIL_ALBEDO_MAP", detailTex.textureValue);
		}
		DoSecondaryNormals ();
		editor.TextureScaleOffsetProperty (detailTex);
	}

	MaterialProperty FindProperty(string name){
		return FindProperty (name, properties);
	}

	void DoNormals(){
		MaterialProperty map = FindProperty ("_NormalMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (MakeLabel (map), map, tex ? FindProperty("_BumpScale") : null);
		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_NORMAL_MAP", map.textureValue);
		}

		if (map.textureValue != null) {
			DoFadeNormals ();
		}
	}

	void DoFadeNormals(){
		MaterialProperty _distOne = FindProperty ("_NormalDistFull");
		MaterialProperty _distZero = FindProperty ("_NormalDistCulled");
		EditorGUI.BeginChangeCheck ();
		EditorGUI.indentLevel += 2;
		bool fadeOn = EditorGUILayout.Toggle ("Fade Normals", IsKeywordEnabled ("NORMAL_DISTANCE_FADE"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("NORMAL_DISTANCE_FADE", fadeOn);
		}
		if (fadeOn) {
			editor.ShaderProperty (_distOne, MakeLabel ("Distance Min"));
			editor.ShaderProperty (_distZero, MakeLabel ("Distance Max"));
		}
		EditorGUI.indentLevel -= 2;
	}

	void DoDistanceDither(){
		
		GUILayout.Label ("Dithering", EditorStyles.boldLabel);
		MaterialProperty _distMin = FindProperty ("_DitherDistMin");
		MaterialProperty _distMax = FindProperty ("_DitherDistMax");
		EditorGUI.BeginChangeCheck ();

		bool ditherOn = EditorGUILayout.Toggle(MakeLabel("Distance Dithering"), IsKeywordEnabled ("_DISTANCE_DITHER"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_DISTANCE_DITHER", ditherOn);
		}

		if (ditherOn) {
			editor.ShaderProperty (_distMin, MakeLabel ("Distance Min"));
			editor.ShaderProperty (_distMax, MakeLabel ("Distance Max"));
		}
	}

	void DoSecondaryNormals(){
		MaterialProperty map = FindProperty ("_DetailNormalMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (MakeLabel (map), map, tex ? FindProperty ("_DetailBumpScale") : null);
		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_DETAIL_NORMAL_MAP", map.textureValue);
		}
	}

	void DoMetallic(){
		MaterialProperty map = FindProperty ("_MetallicMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (MakeLabel (map, "Metallic (R)"), map, tex ? null : FindProperty ("_Metallic"));
		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_METALLIC_MAP", map.textureValue);
		}
	}

	void DoEmission(){
		MaterialProperty map = FindProperty ("_EmissionMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();

		editor.TexturePropertyWithHDRColor (MakeLabel (map, "Emission (R)"), map, FindProperty ("_Emission"), emissionConfig, false);
		editor.LightmapEmissionProperty (2);
		if (EditorGUI.EndChangeCheck ()) {

			if (tex != map.textureValue) {
				SetKeyword ("_EMISSION_MAP", map.textureValue);
			}

			foreach (Material m in editor.targets) {
				m.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.BakedEmissive;
			}
		}
	}

	void DoCelShading(){
		GUILayout.Label ("Cel Shading", EditorStyles.boldLabel);
		MaterialProperty shadowTransition = FindProperty ("_ShadowTransition");
		MaterialProperty shadowStrength = FindProperty ("_ShadowStrength");

		EditorGUI.BeginChangeCheck ();

		bool celShaded = EditorGUILayout.Toggle(MakeLabel("Cel Shading"), IsKeywordEnabled ("_CELSHADED"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_CELSHADED", celShaded);
		}
		EditorGUI.indentLevel += 2;
		if (celShaded && IsKeywordEnabled("_RENDERING_TRANSPARENT")) {
			editor.ShaderProperty (shadowTransition, MakeLabel (shadowTransition));
			editor.ShaderProperty (shadowStrength, MakeLabel (shadowStrength));
		}
		EditorGUI.indentLevel -= 2;
	}

	void DoSubSurfaceScattering(){
		GUILayout.Label ("Sub Surface Scattering", EditorStyles.boldLabel);

		MaterialProperty power = FindProperty ("_PowerSSS");
		MaterialProperty scale = FindProperty ("_ScaleSSS");
		MaterialProperty distortion = FindProperty ("_DistortionSSS");
		MaterialProperty atten = FindProperty ("_AttenuationSSS");
		MaterialProperty ambient = FindProperty ("_AmbientSSS");
		MaterialProperty thickness = FindProperty ("_ThicknessMap");
		Texture tex = thickness.textureValue;

		EditorGUI.BeginChangeCheck ();

		bool sssOn = EditorGUILayout.Toggle ("Translucency", IsKeywordEnabled ("_SSS"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_SSS", sssOn);
		}

		if (sssOn) {

			DoTranslucencyMode ();
			DoTranslucencyDiffuse ();

			editor.TexturePropertySingleLine (MakeLabel (thickness, "Thickness (A)"), thickness, IsKeywordEnabled("_SSS_DIFFUSE_MAP") ? null : FindProperty ("_DiffuseSSS"));
			EditorGUI.indentLevel += 2;
			editor.ShaderProperty (power, MakeLabel (power));
			editor.ShaderProperty (scale, MakeLabel (scale));
			editor.ShaderProperty (distortion, MakeLabel (distortion));
			editor.ShaderProperty (atten, MakeLabel (atten));
			editor.ShaderProperty (ambient, MakeLabel (ambient));
			EditorGUI.indentLevel -= 2;
		} else {
			RenderingSettings settings = RenderingSettings.modes [0];
			if (IsKeywordEnabled ("RENDERING_CUTOUT")) {
				settings = RenderingSettings.modes [1];
			} else if (IsKeywordEnabled ("_RENDERING_FADE")) {
				settings = RenderingSettings.modes [2];
			} else if (IsKeywordEnabled ("_RENDERING_TRANSPARENT")) {
				settings = RenderingSettings.modes [3];
			}

			foreach (Material m in editor.targets) {
				
				m.renderQueue = (int)settings.queue;

				m.SetOverrideTag ("RenderType", settings.renderType);
				m.SetInt ("_SrcBlend", (int)settings.srcBlend);
				m.SetInt ("_DstBlend", (int)settings.dstBlend);
				m.SetInt ("_ZWrite", settings.zWrite ? 1 : 0);
			}
		}
		

	}

	void DoTranslucencyDiffuse(){
		DiffuseSSS mode = DiffuseSSS.Picker;
		if (IsKeywordEnabled ("_SSS_DIFFUSE_MAP")) {
			mode = DiffuseSSS.Map;
		}

		EditorGUI.BeginChangeCheck ();
		mode = (DiffuseSSS)EditorGUILayout.EnumPopup (MakeLabel ("SSS Diffuse Mode"), mode);
		if (EditorGUI.EndChangeCheck ()) {
			RecordAction ("Diffuse SSS");
			SetKeyword ("_SSS_DIFFUSE_MAP", mode == DiffuseSSS.Map);
		}
	}

	void DoTranslucencyMode(){
		TranslucencyModes sssMode = TranslucencyModes.OpaqueSSS;
		if (IsKeywordEnabled ("_RENDERING_FADE")) {
			sssMode = TranslucencyModes.FadeSSS;
		} else if (IsKeywordEnabled ("_RENDERING_TRANSPARENT")) {
			sssMode = TranslucencyModes.TransparentSSS;
		} else if (IsKeywordEnabled ("_RENDERING_CUTOUT")) {
			sssMode = TranslucencyModes.CutoutSSS;
		}

		EditorGUI.BeginChangeCheck ();
		sssMode = (TranslucencyModes)EditorGUILayout.EnumPopup (MakeLabel ("SSS Mode"), sssMode);
		if (EditorGUI.EndChangeCheck ()) {
			RecordAction ("Translucency Mode");

			SetKeyword ("_RENDERING_CUTOUT", sssMode == TranslucencyModes.CutoutSSS);
			SetKeyword ("_RENDERING_FADE", sssMode == TranslucencyModes.FadeSSS);
			SetKeyword ("_RENDERING_TRANSPARENT", sssMode == TranslucencyModes.TransparentSSS);


			RenderingSettings settings = RenderingSettings.modes [4];
			if (sssMode == TranslucencyModes.FadeSSS) {
				settings = RenderingSettings.modes [5];
			} else if (sssMode == TranslucencyModes.TransparentSSS) {
				settings = RenderingSettings.modes [6];
			}

			foreach (Material m in editor.targets) {
				

				m.renderQueue = (int)settings.queue;

				m.SetOverrideTag ("RenderType", settings.renderType);
				m.SetInt ("_SrcBlend", (int)settings.srcBlend);
				m.SetInt ("_DstBlend", (int)settings.dstBlend);
				m.SetInt ("_ZWrite", settings.zWrite ? 1 : 0);
			}

		}
	}

	void DoOcclusion(){
		MaterialProperty map = FindProperty ("_OcclusionMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (MakeLabel (map, "Occlusion (G)"), map, map.textureValue ? FindProperty ("_OcclusionStrength") : null);
		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_OCCLUSION_MAP", map.textureValue);
		}
	}

	void DoDetailMask(){
		MaterialProperty mask = FindProperty ("_DetailMask");
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (MakeLabel (mask, "Detail Mask (A)"), mask);
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_DETAIL_MASK", mask.textureValue);
		}
	}

	void DoSmoothness(){
		SmoothnessSource source = SmoothnessSource.Uniform;
		if (IsKeywordEnabled ("_SMOOTHNESS_ALBEDO")) {
			source = SmoothnessSource.Albedo;
		} else if(IsKeywordEnabled("_SMOOTHNESS_METALLIC")){
			source = SmoothnessSource.Metallic;
		}
		MaterialProperty slider = FindProperty ("_Smoothness");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty (slider, MakeLabel (slider));
		EditorGUI.indentLevel += 1;
		EditorGUI.BeginChangeCheck ();
		source = (SmoothnessSource)EditorGUILayout.EnumPopup (MakeLabel ("Source"), source);
		if (EditorGUI.EndChangeCheck ()) {
			RecordAction ("Smoothness Source");
			SetKeyword ("_SMOOTHNESS_ALBEDO", source == SmoothnessSource.Albedo);
			SetKeyword ("_SMOOTHNESS_METALLIC", source == SmoothnessSource.Metallic);
		}
		EditorGUI.indentLevel -= 3;
	}

	void DoSemitransparentShadows(){
		EditorGUI.BeginChangeCheck ();
		bool semitransparentShadows = 
			EditorGUILayout.Toggle (MakeLabel ("Semitransp. Shadows", "Semitransparent Shadows"),
				IsKeywordEnabled ("_SEMITRANSPARENT_SHADOWS"));
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_SEMITRANSPARENT_SHADOWS", semitransparentShadows);
		}

		if (!semitransparentShadows) {
			shouldShowAlphaCutoff = true;
		}
	}

	static GUIContent staticLabel = new GUIContent();

	static GUIContent MakeLabel(MaterialProperty property, string tooltip = null){
		staticLabel.text = property.displayName;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

	static GUIContent MakeLabel(string text, string tooltip = null){
		staticLabel.text = text;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

	void SetKeyword(string keyword, bool state){

		if (state) {
			foreach (Material m in editor.targets) {
				target.EnableKeyword (keyword);
			}
		} else {
			foreach (Material m in editor.targets) {
				target.DisableKeyword (keyword);
			}
		}
	}

	void DoRenderingMode(){
		RenderingMode mode = RenderingMode.Opaque;
		shouldShowAlphaCutoff = false;
		if (IsKeywordEnabled ("_RENDERING_CUTOUT")) {
			mode = RenderingMode.Cutout;
			shouldShowAlphaCutoff = true;
		} else if (IsKeywordEnabled ("_RENDERING_FADE")) {
			mode = RenderingMode.Fade;	
		} else if (IsKeywordEnabled ("_RENDERING_TRANSPARENT")) {
			mode = RenderingMode.Transparent;	
		}

		EditorGUI.BeginChangeCheck ();
		mode = (RenderingMode)EditorGUILayout.EnumPopup (MakeLabel ("Rendering Mode"), mode);
		if (EditorGUI.EndChangeCheck ()) {
			RecordAction ("Rendering Mode");
			SetKeyword ("_RENDERING_CUTOUT", mode == RenderingMode.Cutout);
			SetKeyword ("_RENDERING_FADE", mode == RenderingMode.Fade);
			SetKeyword ("_RENDERING_TRANSPARENT", mode == RenderingMode.Transparent);

			RenderingSettings settings = RenderingSettings.modes [(int)mode];
			foreach (Material m in editor.targets) {
				
				m.renderQueue = (int)settings.queue;

				m.SetOverrideTag ("RenderType", settings.renderType);
				m.SetInt ("_SrcBlend", (int)settings.srcBlend);
				m.SetInt ("_DstBlend", (int)settings.dstBlend);
				m.SetInt ("_ZWrite", settings.zWrite ? 1 : 0);
			
			}
		}

		if (mode == RenderingMode.Fade || mode == RenderingMode.Transparent) {
			DoSemitransparentShadows ();
		}
	}

	void DoRenderQueue(){
		MaterialProperty queue = FindProperty ("_RenderQueue");
		target.SetInt ("_RenderQueue", target.renderQueue);
		editor.ShaderProperty (queue, MakeLabel (queue));
	}

	/*
	void DoCulling(){
		CullMode mode = CullMode.Back;
		if (IsKeywordEnabled("_CULL_BACK")) {
			mode = CullMode.Back;
//			Debug.Log ("GI");
		} else if (IsKeywordEnabled("_CULL_FRONT")) {
			mode = CullMode.Front;
//			Debug.Log ("GO");
		} else if (IsKeywordEnabled("_CULL_OFF")) {
			mode = CullMode.Off;
//			Debug.Log ("GA");
		}

		EditorGUI.BeginChangeCheck ();
		mode = (CullMode)EditorGUILayout.EnumPopup (MakeLabel ("Culling Mode"), mode);
		if (EditorGUI.EndChangeCheck ()) {
			
			RecordAction ("Culling Mode");
			SetKeyword ("_CULL_BACK", mode == CullMode.Back);
			SetKeyword ("_CULL_FRONT", mode == CullMode.Front);
			SetKeyword ("_CULL_OFF", mode == CullMode.Off);

			foreach (Material m in editor.targets) {
				if (mode == CullMode.Back) {
					m.SetInt ("_Cull", (int)CullMode.Back);
					//Debug.Log ((int)CullMode.Back);
				} else if (mode == CullMode.Front) {
					m.SetInt ("_Cull", (int)CullMode.Front);
					//Debug.Log ((int)CullMode.Front);
				} else if (mode == CullMode.Off) {
					m.SetInt ("_Cull", (int)CullMode.Off);
					//Debug.Log ((int)CullMode.Off);
				}
				//Debug.Log (IsKeywordEnabled ("_CULL_BACK"));
			}
		}
	}
	*/

	void DoCulling(){
		_culling = FindProperty ("_Cull");
		EditorGUI.BeginChangeCheck ();

		CullMode mode = (CullMode)Mathf.RoundToInt (_culling.floatValue);
		mode = (CullMode)EditorGUILayout.EnumPopup (MakeLabel ("Culling Mode"), mode);
		if (EditorGUI.EndChangeCheck ()) {
			RecordAction ("Culling Mode");
			foreach (Material m in editor.targets) {
				m.SetInt ("_Cull", (int)mode);
			}
		}
	}


	void DoRefraction(){
		GUILayout.Label ("Refraction", EditorStyles.boldLabel);

//		RenderingMode mode = RenderingMode.Opaque;
//		if (IsKeywordEnabled ("_RENDERING_CUTOUT")) {
//			mode = RenderingMode.Cutout;
//			shouldShowAlphaCutoff = true;
//		} else if (IsKeywordEnabled ("_RENDERING_FADE")) {
//			mode = RenderingMode.Fade;	
//		} else if (IsKeywordEnabled ("_RENDERING_TRANSPARENT")) {
//			mode = RenderingMode.Transparent;	
//		}

		MaterialProperty refracAmount = FindProperty ("_RefractionAmount");

		EditorGUI.BeginChangeCheck ();
		bool refrac = EditorGUILayout.Toggle ("Refraction", IsKeywordEnabled ("_REFRACTION"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_REFRACTION", refrac);

//			foreach (Material m in editor.targets) {
//				if (refrac) {
//					m.renderQueue = (int)RenderQueue.Transparent;
//				} else {
//					if (mode == RenderingMode.Opaque) {
//						m.renderQueue = (int)RenderQueue.Geometry;
//					} else if (mode == RenderingMode.Cutout) {
//						m.renderQueue = (int)RenderQueue.AlphaTest;
//					}
//				}
//			}
		}

		if (refrac) {
			editor.ShaderProperty (refracAmount, MakeLabel (refracAmount));
		}
	}

	void DoVertexOffset(){
		GUILayout.Label ("Vertex Offset", EditorStyles.boldLabel);
		if (IsKeywordEnabled ("_VERTEX_WIND") || IsKeywordEnabled ("_VERTEX_BEND")) {
			VertexMask maskType = VertexMask.Custom;
			if (IsKeywordEnabled ("_VERTEX_MASK_COLOUR")) {
				maskType = VertexMask.VertexColour;
			}

			MaterialProperty maxAngle = FindProperty ("_MaxBendAngle");
			editor.ShaderProperty (maxAngle, "Angle Max");

//			OffsetPlane plane = OffsetPlane.YZ;
//			if (IsKeywordEnabled ("_VERTEX_OFFSET_XY")) {
//				plane = OffsetPlane.XY;
//			} else if (IsKeywordEnabled ("_VERTEX_OFFSET_XZ")) {
//				plane = OffsetPlane.XZ;
//			}


			EditorGUI.BeginChangeCheck ();
			maskType = (VertexMask)EditorGUILayout.EnumPopup (MakeLabel ("Vertex Mask"), maskType);
//			plane = (OffsetPlane)EditorGUILayout.EnumPopup(MakeLabel("Offset Plane")

			if (EditorGUI.EndChangeCheck ()) {
				RecordAction ("Vertex Mask");
				SetKeyword ("_VERTEX_MASK_CUSTOM", maskType == VertexMask.Custom);
				SetKeyword ("_VERTEX_MASK_COLOUR", maskType == VertexMask.VertexColour);
			}

			if (maskType == VertexMask.Custom) {
				MaterialProperty multiplier = FindProperty ("_VertMaskMultiplier");
				MaterialProperty flatOffset = FindProperty ("_VertMaskFlat");
				editor.ShaderProperty (multiplier, MakeLabel ("Multiplier", "0.8 worked ok"));
				editor.ShaderProperty (flatOffset, MakeLabel ("Vertical Offset", "0.3 worked ok"));
			}

		}
		DoWind ();
		DoBending ();

		RecalculateNormals ();

	}

	void DoWind(){
		EditorGUI.BeginChangeCheck ();
		bool windOn = EditorGUILayout.Toggle ("Wind", IsKeywordEnabled ("_VERTEX_WIND"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_VERTEX_WIND", windOn);
		}


	}

	void DoBending(){
		MaterialProperty minDist = FindProperty ("_BendingDistMin");
		MaterialProperty maxDist = FindProperty ("_BendingDistMax");

		EditorGUI.BeginChangeCheck ();
		bool bendingOn = EditorGUILayout.Toggle ("Bending", IsKeywordEnabled ("_VERTEX_BEND"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_VERTEX_BEND", bendingOn);
		}

		if (bendingOn) {
			editor.ShaderProperty (minDist, MakeLabel ("Full Effect Dist"));
			editor.ShaderProperty (maxDist, MakeLabel ("No Effect Dist"));
		}
	}

	void RecalculateNormals(){
		EditorGUI.BeginChangeCheck ();
		bool recalcOn = EditorGUILayout.Toggle ("Recalculate Normals", IsKeywordEnabled ("_RECALCULATE_NORMALS"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_RECALCULATE_NORMALS", recalcOn);
		}
	}

	void DoDynamicBatching(){
		GUILayout.Label ("Dynamic Batching", EditorStyles.boldLabel);
		EditorGUI.BeginChangeCheck ();
		bool batchingEnabled = target.GetTag ("DisableBatching", false, "False") == "False" ? true : false;
		bool batchingOn = EditorGUILayout.Toggle ("Batching Enabled", batchingEnabled);

		if (EditorGUI.EndChangeCheck ()) {
			//Debug.Log (batchingEnabled);
			foreach (Material mat in editor.targets) {
				mat.SetOverrideTag("DisableBatching", batchingOn ? "False" : "True");
			}
		}
	}

	void DoIsPlayer(){
		EditorGUI.BeginChangeCheck ();

		bool avatarShader = EditorGUILayout.Toggle ("Avatar Shader", IsKeywordEnabled ("_PLAYER_SHADER"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_PLAYER_SHADER", avatarShader);
		}

		if (avatarShader) {
			target.renderQueue = (int)RenderQueue.Geometry+501;
		}
	}

	#region debug
	void DoCheckerDebug(){

		EditorGUI.BeginChangeCheck ();
		bool checkerOn = EditorGUILayout.Toggle ("Checker", IsKeywordEnabled ("CHECKER_DEBUG"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("CHECKER_DEBUG", checkerOn);
		}
	}

	void DoLocalNormalDebug(){
		EditorGUI.BeginChangeCheck ();
		bool displayNormals = EditorGUILayout.Toggle ("Local normal", IsKeywordEnabled ("_LOCAL_NORMAL_DEBUG"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_LOCAL_NORMAL_DEBUG", displayNormals);
		}
	}
	#endregion

	enum SmoothnessSource {
		Uniform, Albedo, Metallic
	}

	enum RenderingMode {
		Opaque, Cutout, Fade, Transparent
	}

	enum TranslucencyModes {
		OpaqueSSS, CutoutSSS, FadeSSS, TransparentSSS
	}

	enum VertexMask {
		Custom, VertexColour
	}

	enum DiffuseSSS {
		Map, Picker
	}

	struct RenderingSettings {
		public RenderQueue queue;
		public string renderType;
		public BlendMode srcBlend, dstBlend;
		public bool zWrite;

		public static RenderingSettings[] modes = {
			new RenderingSettings() {
				queue = RenderQueue.Geometry,
				renderType = "",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},
			new RenderingSettings() {
				queue = RenderQueue.AlphaTest,
				renderType = "TransparentCutout",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},
			new RenderingSettings() {
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.SrcAlpha,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			},
			new RenderingSettings() {
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			},
			//opaque SSS
			new RenderingSettings(){
				queue = RenderQueue.Transparent,
				renderType = "Custom",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},
			//fade SSS
			new RenderingSettings(){
				queue = RenderQueue.Transparent,
				renderType = "Custom",
				srcBlend = BlendMode.SrcAlpha,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = true
			},
			//transparent SSS
			new RenderingSettings(){
				queue = RenderQueue.Transparent,
				renderType = "Custom",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = true
			}//,
//			//avatar
//			new RenderingSettings(){
//				queue = RenderQueue.GeometryLast,
//				renderType = "Custom",
//				srcBlend = BlendMode.One,
//				dstBlend = BlendMode.Zero,
//				zWrite = true
//			}
		};
	}

	bool IsKeywordEnabled (string keyword) {
		return target.IsKeywordEnabled (keyword);
	}

	void RecordAction(string label){
		editor.RegisterPropertyChangeUndo (label);
	}

}
