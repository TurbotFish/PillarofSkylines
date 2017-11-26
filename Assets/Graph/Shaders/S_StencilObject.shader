Shader "Custom/S_StencilObject" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }
		Pass {
			Stencil {
				Ref 4
				Comp equal
				Pass keep 
			}
			ZWrite Off
		
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			float4 _Color;

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                return _Color;
            }
            ENDCG
		
		}
	}
	FallBack "Diffuse"
}
