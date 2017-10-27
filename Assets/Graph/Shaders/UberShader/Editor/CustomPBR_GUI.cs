using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomPBR_GUI : ShaderGUI {

	Material target;
	MaterialEditor editor;
	MaterialProperty[] properties;
	static ColorPickerHDRConfig emissionConfig = new ColorPickerHDRConfig (0f, 99f, 1f / 99f, 3f);

	bool shouldShowAlphaCutoff;

	public override void OnGUI (MaterialEditor editor, MaterialProperty[] properties){
		this.target = editor.target as Material;
		this.editor = editor;
		this.properties = properties;
		DoRenderingMode ();
		DoCulling ();
		DoCelShading ();
		DoSubSurfaceScattering ();
		DoMain ();
		DoSecondary ();
		DoCheckerDebug ();
	}

	void DoMain(){
		GUILayout.Label ("Main Maps", EditorStyles.boldLabel);

		MaterialProperty mainTex = FindProperty ("_MainTex", properties);

		editor.TexturePropertySingleLine (MakeLabel(mainTex, "Albedo (RGB)"), mainTex, FindProperty("_Tint"));
		if (shouldShowAlphaCutoff) {
			DoAlphaCutoff ();
		}
		DoMetallic ();
		DoSmoothness ();
		DoNormals ();
		DoOcclusion ();
		DoEmission ();
		DoDetailMask ();
		editor.TextureScaleOffsetProperty (mainTex);
	}

	void DoAlphaCutoff(){
		MaterialProperty slider = FindProperty ("_AlphaCutoff");
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
		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_EMISSION_MAP", map.textureValue);
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
		if (celShaded) {
			editor.ShaderProperty (shadowTransition, MakeLabel (shadowTransition));
			editor.ShaderProperty (shadowStrength, MakeLabel (shadowStrength));
		}
		EditorGUI.indentLevel -= 2;
	}

	void DoSubSurfaceScattering(){
		GUILayout.Label ("Sub Surface Scattering", EditorStyles.boldLabel);
		//MaterialProperty power = FindProperty ("_PowerSSS");
		//MaterialProperty scale = FindProperty ("_ScaleSSS");
		//MaterialProperty distortion = FindProperty ("_DistortionSSS");
		//MaterialProperty atten = FindProperty ("_AttenuationSSS");
		//MaterialProperty ambient = FindProperty ("_AmbientSSS");
		//MaterialProperty diffuse = FindProperty ("_DiffuseSSS");

		MaterialProperty thickness = FindProperty ("_ThicknessMap");
		Texture tex = thickness.textureValue;

		EditorGUI.BeginChangeCheck ();

		bool subSurfaceOn = EditorGUILayout.Toggle ("Translucency", IsKeywordEnabled ("_SSS"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_SSS", subSurfaceOn);

		}


		if (subSurfaceOn) {
			
			//editor.ShaderProperty (power, MakeLabel (power));
			//editor.ShaderProperty (scale, MakeLabel (scale));
			//editor.ShaderProperty (distortion, MakeLabel (distortion));
			//editor.ShaderProperty (atten, MakeLabel (atten));
			//editor.ShaderProperty (ambient, MakeLabel (ambient));
			//editor.ShaderProperty (diffuse, MakeLabel (diffuse));
			editor.TexturePropertySingleLine (MakeLabel (thickness, "Thickness (R)"), thickness);

			bool useSSSColour2 = EditorGUILayout.Toggle ("Colour 2", IsKeywordEnabled ("_SSSColour2"));
			SetKeyword ("_SSSColour2", useSSSColour2);
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


	void DoCulling(){
		CullMode mode = CullMode.Back;
		if (IsKeywordEnabled("_CULL_BACK")) {
			mode = CullMode.Back;
		} else if (IsKeywordEnabled("_CULL_FRONT")) {
			mode = CullMode.Front;
		} else if (IsKeywordEnabled("_CULL_OFF")) {
			mode = CullMode.Off;
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
				} else if (mode == CullMode.Front) {
					m.SetInt ("_Cull", (int)CullMode.Front);
				} else if (mode == CullMode.Off) {
					m.SetInt ("_Cull", (int)CullMode.Off);
				}
			}
		}
	}

	void DoCheckerDebug(){
		GUILayout.Label ("Checker debug", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck ();
		bool checkerOn = EditorGUILayout.Toggle ("On", IsKeywordEnabled ("CHECKER_DEBUG"));

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("CHECKER_DEBUG", checkerOn);
		}
	}


	enum SmoothnessSource {
		Uniform, Albedo, Metallic
	}

	enum RenderingMode {
		Opaque, Cutout, Fade, Transparent
	}

	enum CullingMode {
		Back, Front, Off
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
			}
		};
	}

	bool IsKeywordEnabled (string keyword) {
		return target.IsKeywordEnabled (keyword);
	}

	void RecordAction(string label){
		editor.RegisterPropertyChangeUndo (label);
	}
}
