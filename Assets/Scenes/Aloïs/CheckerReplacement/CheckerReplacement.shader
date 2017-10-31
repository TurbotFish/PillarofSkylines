Shader "Alo/Replacement/CheckerReplacement" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float _CheckerScale;
		float _CheckerMinContrast;
		float _CheckerMaxContrast;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 wPos = IN.worldPos;
			float xValue = floor(wPos.x * _CheckerScale * 2) - floor(floor(wPos.x * _CheckerScale* 2) * 0.5) * 2.0;
			float yValue = floor(wPos.y * _CheckerScale* 2) - floor(floor(wPos.y * _CheckerScale* 2) * 0.5) * 2.0;
			float zValue = floor(wPos.z * _CheckerScale* 2) - floor(floor(wPos.z * _CheckerScale* 2) * 0.5) * 2.0;
			o.Albedo = clamp(abs(yValue - abs(xValue - zValue)), _CheckerMinContrast, _CheckerMaxContrast) * _Color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	SubShader {
		Tags { "RenderType"="Transparent"
		"Queue"="Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		CGPROGRAM
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
