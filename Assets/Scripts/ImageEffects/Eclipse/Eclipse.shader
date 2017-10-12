Shader "Hidden/PoS - Eclipse" {
    Properties {
        _MainTex("-", 2D) = "" {}
        _Noise ("Noise", 2D) = "white" {}
        _Deformation ("Deformation", Range(0, 1)) = 0.2
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;

    uniform float4 _TimeEditor;
    uniform sampler2D _Noise; uniform float4 _Noise_ST;
    uniform float _Deformation;
	uniform float _Speed;
	uniform int _Iterations;
	uniform float2 _Direction;
	
	uniform float _Threshold;
    uniform float _Intensity;

    half4 frag(v2f_img i) : SV_Target {
        half4 src = tex2D(_MainTex, i.uv);
		
        #if UNITY_UV_STARTS_AT_TOP
            float grabSign = -_ProjectionParams.x;
        #else
            float grabSign = _ProjectionParams.x;
        #endif

		float4 timer = (_Time + _TimeEditor) * _Speed;

		float2 noiseUV = (i.uv + timer.g * _Direction);
		float4 _Noise_var = tex2D(_Noise, TRANSFORM_TEX(noiseUV, _Noise) );

		float4 final = src;

		for(int j = 0; j < _Iterations; j++) {
			
			float3 deformedUV = lerp(float3((i.uv), 0.0), _Noise_var.rgb, j * _Deformation/_Iterations);
			float4 newIteration = tex2D(_MainTex, deformedUV);

			float a = (newIteration.r +newIteration.g + newIteration.b)/3;
			final = lerp(final, saturate(1-(1-final)*(1-newIteration)), saturate((a+_Threshold) * (1 - j/float(_Iterations))));
		}
		return lerp(src, final, _Intensity);
    }
    ENDCG

    SubShader {
        Pass {
			ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag 
			#pragma target 4.0
            ENDCG
        }
    }
}
