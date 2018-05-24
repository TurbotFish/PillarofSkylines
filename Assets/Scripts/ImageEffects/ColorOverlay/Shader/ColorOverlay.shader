Shader "Hidden/ColorOverlayGrr" {
    Properties {
        _MainTex("-", 2D) = "" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float _Intensity;
	half4 _Color;

    half4 frag(v2f_img i) : SV_Target {
        half4 src = tex2D(_MainTex, i.uv);
		
		#if _MULTIPLY
			_Color *= src;
		#elif _SCREEN
			_Color = 1 - (1-src)*(1-_Color);
		#elif _OVERLAY
			float a = (src.r +src.g + src.b)/3;
			if (a < 0.5)
				_Color *= 2 * src;
			else
				_Color = 1- 2 * (1 - src)*(1-_Color);
		#endif

		return lerp(src, _Color, _Intensity);
    }
    ENDCG

    SubShader {
        Pass {
			ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
			#pragma multi_compile __ _MULTIPLY _SCREEN _OVERLAY
            ENDCG
        }
    }
}
