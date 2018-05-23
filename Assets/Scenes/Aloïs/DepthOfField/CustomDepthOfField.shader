Shader "Hidden/AloDoF" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex, _CameraDepthTexture;
		float4 _MainTex_TexelSize;

		float _StartOffset, _MaxDistance;
		sampler2D _CoCTex, _FarTex;

		float4 _Points64[64];
		float4 _Points16[16];
		float _BokehRadius, _PixelStep;

		


		struct VertexData {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct Interpolators {
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};

		Interpolators VertexProgram (VertexData v){
			Interpolators i;
			i.pos = UnityObjectToClipPos(v.vertex);
			i.uv = v.uv;

			return i;
		}

	ENDCG

	SubShader {
		
		Cull Off
		ZTest Always
		ZWrite Off

		Pass {//Circle of Confusion Pass 0
			CGPROGRAM
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			half FragmentProgram (Interpolators i) : SV_Target {
				half depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				depth = LinearEyeDepth(depth);

				//catlikecoding version
				//float coc = (depth - _FocusDistance) / _FocusRange;
				//coc = clamp(coc, -1, 1);

				//pixelmischielf version
				//float signedDistance = depth - _FocusDistance;
				//float coc = smoothstep(0, _FocusRange, abs(signedDistance)) * sign(signedDistance);

				//no foreground blur version
				float distance = depth - _StartOffset;
				float coc = smoothstep(_StartOffset, _MaxDistance, distance);

				
				return coc;
			}

			ENDCG
		}

		Pass {//Bokeh Pass 1
			CGPROGRAM
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			#include "BokehKernels.cginc"

			half4 FragmentProgram (Interpolators i) : SV_Target {
				
				half3 color = 0;
				half coc = tex2D(_CoCTex, i.uv).r;
				float2 sampleStep = _MainTex_TexelSize * _BokehRadius * coc;
				
				for (int j = 0; j < kernelSampleCount; j++){
					float2 o = kernel[j] * sampleStep;

					float4 tap = tex2D(_MainTex, i.uv +o);

					color += tap.rgb;

				}

				color *= 1.0 / kernelSampleCount;

				return half4(color, coc);
			}

			ENDCG
		}

		Pass {//bokeh blur pass 2
			CGPROGRAM
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			#include "BokehKernels.cginc"

			half4 FragmentProgram (Interpolators i) : SV_Target {
				
				float4 col = tex2D(_MainTex, i.uv);
				float3 maxValue = col.rgb;
				float coc = col.a;

				float2 sampleStep = _MainTex_TexelSize * _BokehRadius * coc;

				for (int j = 0; j < blurSampleCount; j++){
					float2 sUV = i.uv + sampleStep * blurKernel[j];
					float4 tap = tex2D(_MainTex, sUV);

					maxValue = max(tap.rgb, maxValue);
				}

				return float4(maxValue, coc);
			}

			ENDCG
		}

		Pass {//blend pass 3
			CGPROGRAM
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			half4 FragmentProgram (Interpolators i) : SV_Target {
				
				float4 source = tex2D(_MainTex, i.uv);
				float4 farCol = tex2D(_FarTex, i.uv);
				float coc = tex2D(_CoCTex, i.uv).r;

				float blendFactor = smoothstep(0.1, 1.0, coc);

				source.rgb = lerp(source.rgb, farCol.rgb, coc);

				return float4(source.rgb , 1);
			}

			ENDCG
		}

		Pass {//coc horizontal gaussian blur pass 4
			CGPROGRAM
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			half4 FragmentProgram (Interpolators i) : SV_Target {
				
				float2 uvL = float2(i.uv.x - _MainTex_TexelSize.x, i.uv.y);
				float2 uvR = float2(i.uv.x + _MainTex_TexelSize.x, i.uv.y);

				float cocL = tex2D(_MainTex, uvL).r;
				float coc = tex2D(_MainTex, i.uv).r;
				float cocR = tex2D(_MainTex, uvR).r;

				float color = 0.28 * cocL + 0.44 * coc + 0.28 * cocL;

				return float4(coc, 0, 0, 1);
				//return float4(-coc.r,0,0,1);
			}

			ENDCG
		}

		Pass {//coc vertical gaussian blur pass 5
			CGPROGRAM
			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			half4 FragmentProgram (Interpolators i) : SV_Target {
				
				float2 uvD = float2(i.uv.x, i.uv.y - _MainTex_TexelSize.y);
				float2 uvU = float2(i.uv.x,  i.uv.y + _MainTex_TexelSize.y);

				float cocD = tex2D(_MainTex, uvD).r;
				float coc = tex2D(_MainTex, i.uv).r;
				float cocU = tex2D(_MainTex, uvU).r;

				float color = 0.28 * cocD + 0.44 * coc + 0.28 * cocU;

				return float4(color, 0, 0, 1);
				//return float4(0, 0,1,1);
			}

			ENDCG
		}
	}
}
