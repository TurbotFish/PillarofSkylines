Shader "Alo/Blur/BoxBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}


	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		sampler2D _CameraDepthTexture;
		float _Range;

		struct VertexData {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct Interpolators {
			float2 uv : TEXCOORD0;
			float4 pos : SV_POSITION;
		};

		half3 Sample(float2 uv) {
			return tex2D(_MainTex, uv).rgb;
		}

		half3 SampleBox(float2 uv, float delta) {
			float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
			half3 s = 
					Sample(uv + o.xy) + Sample(uv + o.zy) +
					Sample(uv + o.xw) + Sample(uv + o.zw);
			return s * 0.25f;
		}

		float SampleDepth(float2 uv) {
			float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv).r;
			d = LinearEyeDepth(d);
			return d;
		}

		half4 SampleBoxWithDepthCheck(float2 uv, float delta) {

			float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;

			//half3 _color = Sample(uv);
			half4 _color = tex2D(_MainTex, uv);
			float _depth = SampleDepth(uv);

			half3 _colXY = Sample(uv + o.xy);
			half3 _colZY = Sample(uv + o.zy);
			half3 _colXW = Sample(uv + o.xw);
			half3 _colZW = Sample(uv + o.zw);

			float _diffXY = saturate(abs(SampleDepth(uv + o.xy) - _depth) / _Range);
			float _diffZY = saturate(abs(SampleDepth(uv + o.zy) - _depth) / _Range);
			float _diffXW = saturate(abs(SampleDepth(uv + o.xw) - _depth) / _Range);
			float _diffZW = saturate(abs(SampleDepth(uv + o.zw) - _depth) / _Range);

			float _dotXY = saturate(dot(_color.rgb, _colXY));
			float _dotZY = saturate(dot(_color.rgb, _colZY));
			float _dotXW = saturate(dot(_color.rgb, _colXW));
			float _dotZW = saturate(dot(_color.rgb, _colZW));

//			float _dotXY = dot(_color.rgb, _colXY);
//			float _dotZY = dot(_color.rgb, _colZY);
//			float _dotXW = dot(_color.rgb, _colXW);
//			float _dotZW = dot(_color.rgb, _colZW);

			_colXY = lerp(_colXY, _color.rgb, _dotXY);
			_colZY = lerp(_colZY, _color.rgb, _dotZY);
			_colXW = lerp(_colXW, _color.rgb, _dotXW);
			_colZW = lerp(_colZW, _color.rgb, _dotZW);

			half3 s = lerp(_colXY, _color.rgb, _diffXY) + lerp(_colZY, _color.rgb, _diffZY) +
						lerp(_colXW, _color.rgb, _diffXW) + lerp(_colZW, _color.rgb, _diffZW);

			s *= 0.25f;
			return float4(lerp(s, _color.rgb, _color.a), _color.a);
			//return s;
			//return float3(_color.a,_color.a,_color.a);
		}

		Interpolators VertexProgram (VertexData v) {
			Interpolators i;
			i.pos = UnityObjectToClipPos(v.vertex);
			i.uv = v.uv;
			return i;
		}

	ENDCG

	SubShader {

		Pass {//0

			CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

			
				half4 FragmentProgram (Interpolators i) : SV_Target	{




					return SampleBoxWithDepthCheck(i.uv, 1);
				}
			ENDCG
		}

//		Pass {//1
//
//			CGPROGRAM
//				#pragma vertex VertexProgram
//				#pragma fragment FragmentProgram
//
//			
//				half4 FragmentProgram (Interpolators i) : SV_Target	{
//					
//					return half4(SampleBox(i.uv, 0.5), 1);
//				}
//			ENDCG
//		}
	}
}
