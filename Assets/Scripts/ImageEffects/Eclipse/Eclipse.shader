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
	

	float3 MyRGB2HSV(float3 c) {
		float3 hsv = 0;

		float _min = min(c.r, min(c.g, c.b));
		float _max = max(c.r, max(c.g, c.b));
		float delta = _max - _min;

		hsv.z = _max;

		if (delta == 0) {
			hsv.x = 0;
			hsv.y = 0;

		} else {
			hsv.y = delta/_max;
			
			float deltaR = (((_max - c.r)/6) + (delta/2)) / delta;
			float deltaG = (((_max - c.g)/6) + (delta/2)) / delta;
			float deltaB = (((_max - c.b)/6) + (delta/2)) / delta;

			if (c.r == _max) { hsv.x = deltaB - deltaG; }
			else 
			if (c.g == _max) { hsv.x = (1/3) + deltaR - deltaB;}
			else 
			if (c.b == _max) {hsv.x = (2/3) + deltaG - deltaR;}
			
			if (hsv.x < 0) {hsv.x += 1;}
			if (hsv.x > 1) {hsv.x -= 1;}
		}
		return hsv;
	}
	
	float3 MyHSV2RGB(float3 c) {
		float3 rgb = 0;

		if (c.y == 0) {
			rgb.r = c.z;
			rgb.g = c.z;
			rgb.b = c.z;
		} else {
			float h = c.x * 6;
			if (h == 6) h == 0;
			int i = int (h);
			float one = c.z * (1 - c.y);
			float two = c.z * (1 - c.y * (h - i));
			float three = c.z * (1 - c.y * (1 - (h - i)));

			if (i == 0) { rgb = float3(c.z, three, one); }
			else
			if (i == 1) { rgb = float3(two, c.z, one); }
			else
			if (i == 2) { rgb = float3(one, c.z, three); }
			else
			if (i == 3) { rgb = float3(one, two, c.z); }
			else
			if (i == 4) { rgb = float3(three, one, c.z); }
			else { rgb = float3(c.z, one, two); }

		}
		return rgb;
	}

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

		final = lerp(src, final, _Intensity);
		/*
		float3 hsv = MyRGB2HSV(final.rgb);
		hsv.x = 1 - hsv.x;
		final = float4(MyHSV2RGB(hsv), final.a);
		*/
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
