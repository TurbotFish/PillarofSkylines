Shader "Alo/Blur/GaussianBlur7x7"
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

		half4 SampleWithDepthCheck(float2 uv, float4 offset12, float4 offset34, float4 offset56) {

			float4 uv12 = uv.xyxy + _MainTex_TexelSize.xyxy * offset12;
			float4 uv34 = uv.xyxy + _MainTex_TexelSize.xyxy * offset34;
			float4 uv56 = uv.xyxy + _MainTex_TexelSize.xyxy * offset56;

			half4 _color = tex2D(_MainTex, uv);
			float _depth = SampleDepth(uv);

			float _diff1 = saturate(abs(SampleDepth(uv12.xy) - _depth) / _Range);
			float _diff2 = saturate(abs(SampleDepth(uv12.zw) - _depth) / _Range);
			float _diff3 = saturate(abs(SampleDepth(uv34.xy) - _depth) / _Range);
			float _diff4 = saturate(abs(SampleDepth(uv34.zw) - _depth) / _Range);
			float _diff5 = saturate(abs(SampleDepth(uv56.xy) - _depth) / _Range);
			float _diff6 = saturate(abs(SampleDepth(uv56.zw) - _depth) / _Range);


			half3 _col1 = Sample(uv12.xy);
			half3 _col2 = Sample(uv12.zw);
			half3 _col3 = Sample(uv34.xy);
			half3 _col4 = Sample(uv34.zw);
			half3 _col5 = Sample(uv56.xy);
			half3 _col6 = Sample(uv56.zw);

			float _dot1 = saturate(dot(_color.rgb, _col1));
			float _dot2 = saturate(dot(_color.rgb, _col2));
			float _dot3 = saturate(dot(_color.rgb, _col3));
			float _dot4 = saturate(dot(_color.rgb, _col4));
			float _dot5 = saturate(dot(_color.rgb, _col5));
			float _dot6 = saturate(dot(_color.rgb, _col6));

			_col1 = lerp(_col1, _color.rgb, _dot1);
			_col2 = lerp(_col2, _color.rgb, _dot2);
			_col3 = lerp(_col3, _color.rgb, _dot3);
			_col4 = lerp(_col4, _color.rgb, _dot4);
			_col5 = lerp(_col5, _color.rgb, _dot3);
			_col6 = lerp(_col6, _color.rgb, _dot4);


			//half3 s = 0.0625 * lerp(_col1, _color.rgb, _diff1) + 0.25 * lerp(_col2, _color.rgb, _diff2) + 0.375 * _color + 0.25 * lerp(_col3, _color.rgb, _diff3) + 0.0625 * lerp(_col4, _color.rgb, _diff4);

			half3 s = 0.006 * lerp(_col1, _color.rgb, _diff1) + 0.06 * lerp(_col2, _color.rgb, _diff2) + 0.244 * lerp(_col3, _color.rgb, _diff3) + 0.38 * _color.rgb + 0.244 * lerp(_col4, _color.rgb, _diff4) +
						0.06 * lerp(_col5, _color.rgb, _diff5) + 0.006 * lerp(_col6, _color.rgb, _diff6);

			//return float4(lerp(s, _color.rgb, _color.a), _color.a);
			return float4(s,1);
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
				#pragma fragment HorizontalFragmentProgram

			
				half4 HorizontalFragmentProgram (Interpolators i) : SV_Target	{


					float4 uv12 = float4(-3,0,-2,0);
					float4 uv34 = float4(-1,0,1,0);
					float4 uv56 = float4(2,0,3,0);

					return SampleWithDepthCheck(i.uv, uv12, uv34, uv56);
				}
			ENDCG
		}

		Pass {//1

			CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment VerticalFragmentProgram

			
				half4 VerticalFragmentProgram (Interpolators i) : SV_Target	{
					
					float4 uv12 = float4(0,-3,0,-2);
					float4 uv34 = float4(0,-1,0,1);
					float4 uv56 = float4(0,2,0,3);

					return SampleWithDepthCheck(i.uv, uv12, uv34, uv56);
				}
			ENDCG
		}
	}
}