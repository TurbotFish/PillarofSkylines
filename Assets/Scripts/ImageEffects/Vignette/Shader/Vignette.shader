Shader "Hidden/VignetteGrr" {
    Properties {
        _MainTex("-", 2D) = "" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _Aspect;
    float _Falloff;
    float _Power;
	half4 _Color;

    half4 frag(v2f_img i) : SV_Target {
        float2 coord = (i.uv - 0.5) * _Aspect * 2;
        float rf = sqrt(dot(coord, coord)) * _Falloff;

		float rf2_1 = pow(rf, _Power) + 1.0;
        float e = 1.0 / (rf2_1 * rf2_1);

        half4 src = tex2D(_MainTex, i.uv);
		return (src - _Color) * e + _Color;
    }
    ENDCG

    SubShader {
        Pass {
			ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
