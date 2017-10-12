// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Color/BlendCache" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}

	Subshader {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }

		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ AC_QUALITY_MOBILE
				#include "Common.cginc"

				float4 frag( v2f i ) : SV_Target
				{
					float4 lut1 = tex2D( _RgbTex, i.uv );
					float4 lut2 = tex2D( _LerpRgbTex, i.uv );
					return lerp( lut1, lut2, _LerpAmount );
				}
			ENDCG
		}
	}

	Fallback Off
}
