Shader "Hidden/GradientFogGrr" {
	Properties{
		_MainTex("", 2D) = "white" {}
		_Gradient("", 2D) = "white" {}
		_FogStart("Fog Start", Float) = 0.0
		_FogEnd("Fog End", Float) = 1000.0
		_FarClipPlane("Far Clip Plane", Float) = 1000.0
	}
	SubShader{
		ZWrite Off

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _DRAW_OVER_SKYBOX
			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;

			sampler2D _MainTex;
			sampler2D _Gradient;

			float _FogStart;
			float _FogEnd;
			float3 _FrustumCorners[4];

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 scrPos: TEXCOORD1;
				float3 ray : TEXCOORD2;
			};

			//Our Vertex Shader
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.scrPos = ComputeScreenPos(o.pos);
				o.ray = _FrustumCorners[v.uv.x + 2 * v.uv.y];
				return o;
			}

			//Our Fragment Shader
			half4 frag(v2f i) : COLOR {
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				depth = Linear01Depth(depth);
				float depthValue = depth * _FogEnd - _FogStart;

				// Radial Distance Calculation
				depthValue = length(i.ray * depth);
				depthValue = depthValue * (-1 / (_FogEnd - _FogStart)) + (_FogEnd / (_FogEnd - _FogStart));
				depthValue = saturate(depthValue);

				float4 col = tex2Dlod(_Gradient, float4(1-depthValue, 0, 0, 0)); // Color we pick from the Gradient Texture using the depthValue

				float4 src = tex2D(_MainTex, i.uv); // Screen texture

				float4 final = lerp(src, col, col.a);

				#if !_DRAW_OVER_SKYBOX
				if (depth >= 0.9999) // If too far
					final = src; // Don't draw fog
				#endif

				return final;
			}
		ENDCG
		}
	}
}