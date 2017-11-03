Shader "Alo/Replacement/SlopeAngleDetectionShader" {
	Properties {
		_Color ("Color", Color) = (0,0,0,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//[NoScaleOffset]_RampTex ("Ramp Texture", 2D) = "white"{}
		_FlatColour ("Flat Slope Colour", Color) = (1,1,1,1)
		_ClimbColour ("Climb Slope Colour", Color) = (0,1,1,1)
		_SlideColour ("Slide Slope Colour", Color) = (1,1,0,1)
		_WallColour ("Wall Slope Colour", Color) = (0,0,0,1)
		_WrongColour ("Wrong Slope Colour", Color) = (1,0,1,1)

		_FlatSlope ("fff", Range(0,1)) = 0.95
		_ClimbSlopeMax ("fff", Range(0,1)) = 0.85
		_ClimbSlopeMin ("fff", Range(0,1)) = 0.8
		_SlideSlopeMax ("fff", Range(0,1)) = 0.35
		_SlideSlopeMin ("fff", Range(0,1)) = 0.3
		_WallSlope ("fff", Range(0,1)) = 0.05
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 normal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _RampTex;
		float4 _FlatColour;
		float4 _ClimbColour;
		float4 _SlideColour;
		float4 _WallColour;
		float4 _WrongColour;

		float _FlatSlope;
		float _ClimbSlopeMax;
		float _ClimbSlopeMin;
		float _SlideSlopeMax;
		float _SlideSlopeMin;
		float _WallSlope;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float4 slopeColour = float4(0.13, 0.54, 0.08,1);
			float _Green = o.Normal.g;

			//float4 slopeColour = tex2D(_RampTex, float2(_Green, 0.5));

			if(_Green < 0.95){
				slopeColour = float4(0.5,0.91,0.3,1);//wrong slope
			}

			if(_Green <= 0.85){
				slopeColour = float4(1,1,0,1);
			}

			if(_Green < 0.8){
				slopeColour = float4(0.73,0.11,0.11,1);//wrong slope
			}

			if(_Green <= 0.35){
				slopeColour = float4(0.11,0.27,0.73,1);
			}

			if(_Green < 0.3){
				slopeColour = float4(0.73,0.11,0.11,1);//wrong slope
			}

			if(_Green <= 0.05){
				slopeColour = 0;
			}

			o.Albedo = slopeColour;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = slopeColour;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
