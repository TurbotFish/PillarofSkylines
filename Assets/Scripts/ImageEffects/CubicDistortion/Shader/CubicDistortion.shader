Shader "Hidden/CubicDistortion" {
	Properties {
		_MainTex ("-", 2D) = "" {}
	}
	Subshader {
		ZTest Always Cull Off ZWrite Off

		Pass {
			Fog { Mode off }      
			
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			float _Intensity;
	
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 scrPos: TEXCOORD1;
			};

			//Our Vertex Shader
			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.scrPos = ComputeScreenPos(o.pos);
				return o;
			}
	
			//Our Fragment Shader
			half4 frag(v2f i) : COLOR {
				half2 coord = (i.uv - 0.5) * 2;
				
				half2 offset;
				offset.x = (1 - coord.y * coord.y) * (coord.x);
				offset.y = (1 - coord.x * coord.x) * (coord.y);

				offset *= _Intensity;

				return tex2D(_MainTex, i.uv - offset);
			}
			ENDCG
		}
	}
	Fallback off
}
