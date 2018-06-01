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

	uniform float4 _LuminosityInfluence;
	uniform float4 _ColourInfluence;

	uniform float _ColorChangeR;
	uniform float _ColorChangeG;
	uniform float _ColorChangeB;

	uniform float _Threshold;
    uniform float _Intensity;
	
	// Vignette
    uniform float _Falloff;
    uniform float _Power;

    float4 frag(v2f_img i) : SV_Target {
        half4 src = tex2D(_MainTex, i.uv);
		float4 timer = (_Time + _TimeEditor) * _Speed;
		float4 final = src;

		// DEFORMATION
		float2 noiseUV = (i.uv + timer.g * _Direction);
		float4 _Noise_var = tex2D(_Noise, TRANSFORM_TEX(noiseUV, _Noise) );

		_Noise_var.x = lerp(_Noise_var.x, (_CameraSpeed.x + 1)/2, abs(_CameraSpeed.x));
		_Noise_var.y = lerp(_Noise_var.y, (_CameraSpeed.y + 1)/2, abs(_CameraSpeed.y));
		//END Deformation

		// VIGNETTE
        float2 coord = (i.uv - 0.5) * 2;
        float rf = sqrt(dot(coord, coord)) * _Falloff;

		float rf2_1 = pow(rf, _Power) + 1.0;
        float e = 1.0 / (rf2_1 * rf2_1);
		// END Vignette

		// ITERATIONS
		for(int j = 0; j < _Iterations; j++) {
			float3 deformedUV = lerp(float3((i.uv), 1), _Noise_var.rgb, /*(1-e) */ (j+1) * _Deformation/float(_Iterations));
			float4 newIteration = tex2D(_MainTex, deformedUV);

			float a = (newIteration.r + newIteration.g + newIteration.b)/3 + _Threshold;

			float t = saturate((a) * (1.0 - float(j)/float(_Iterations)));

			final = lerp(final, newIteration, t*(1-e));
			//final = newIteration; // for a cleaner look (but looks less "phantomatic" I guess)
		}
		// END Iterations

		//float v = 0.2126 * final.r + 0.7152 * final.g + 0.0722 * final.b;
		float v = _LuminosityInfluence.r * final.r + _LuminosityInfluence.g * final.g + _LuminosityInfluence.b * final.b;
		
		//final = float4(lerp(final.r,saturate(final.r+final.b),_ColorChangeR), lerp(final.g,saturate(final.g + final.r),_ColorChangeG), lerp(final.b,saturate(final.b + final.r),_ColorChangeB), final.a);
		final = float4(lerp(final.r, saturate(v * _ColourInfluence.r), _ColorChangeR), 
					   lerp(final.g, saturate(v * _ColourInfluence.g), _ColorChangeG), 
					   lerp(final.b, saturate(v * _ColourInfluence.b), _ColorChangeB), final.a);

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
