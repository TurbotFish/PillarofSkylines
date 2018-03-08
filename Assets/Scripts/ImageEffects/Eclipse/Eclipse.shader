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
	uniform float2 _CameraSpeed;
	
	uniform float _Threshold;
    uniform float _Intensity;
	
	// Vignette
    uniform float _Falloff;
    uniform float _Power;

    float4 frag(v2f_img i) : SV_Target {
        half4 src = tex2D(_MainTex, i.uv);
		float4 timer = (_Time + _TimeEditor) * _Speed;

		float2 noiseUV = (i.uv + timer.g * _Direction);
		float4 _Noise_var = tex2D(_Noise, TRANSFORM_TEX(noiseUV, _Noise) );

		_Noise_var.x = lerp(_Noise_var.x, (_CameraSpeed.x + 1)/2, abs(_CameraSpeed.x));
		_Noise_var.y = lerp(_Noise_var.y, (_CameraSpeed.y + 1)/2, abs(_CameraSpeed.y));

		float4 final = src;

		// VIGNETTE
        float2 coord = (i.uv - 0.5) * 2; // multiply by screenSize if adjusting needed
        float rf = sqrt(dot(coord, coord)) * _Falloff;

		float rf2_1 = pow(rf, _Power) + 1.0;
        float e = 1.0 / (rf2_1 * rf2_1);
		// END Vignette

		// ITERATIONS
		for(int j = 0; j < _Iterations; j++) {
			float3 deformedUV = lerp(float3((i.uv), 1), _Noise_var.rgb, (j+1) * _Deformation/float(_Iterations) * e);
			float4 newIteration = tex2D(_MainTex, deformedUV);

			float a = (newIteration.r + newIteration.g + newIteration.b)/3 + _Threshold;
			float b = (final.r + final.g + final.b)/3;

			float t = saturate((a) * (1.0 - float(j)/float(_Iterations)));
			final = lerp(final, newIteration, t*(1-e));
		}
		// END Iterations

		final = float4(final.r, saturate(final.g + final.r), saturate(final.b + final.r), final.a);
		final = lerp(src, final, _Intensity);

		return final;
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
