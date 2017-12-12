// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/PBR"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_BaseColor("BaseColor", 2D) = "white" {}
		_RMAo("RMAo", 2D) = "white" {}
		_Float0("Float 0", Float) = 1
		_Normal("Normal", 2D) = "white" {}
		_Roughness("Roughness", Float) = 0.15
		_Highlight_value("Highlight_value", Range( 0 , 1)) = 0
		_Highlight_intensity("Highlight_intensity", Range( 1 , 2)) = 1.4
		[Toggle]_Highlight_platform("Highlight_platform", Float) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Emissive("Emissive", Float) = 0
		_Color2("Color 2", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform float _Float0;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Highlight_platform;
		uniform sampler2D _BaseColor;
		uniform float4 _BaseColor_ST;
		uniform float _Highlight_intensity;
		uniform float _Highlight_value;
		uniform float4 _Color2;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Emissive;
		uniform sampler2D _RMAo;
		uniform float4 _RMAo_ST;
		uniform float _Roughness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,_Float0 );
			float2 uv_BaseColor = i.uv_texcoord * _BaseColor_ST.xy + _BaseColor_ST.zw;
			float4 tex2DNode1 = tex2D( _BaseColor, uv_BaseColor );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float clampResult22 = clamp( -ase_worldNormal.z , 0.0 , 1.0 );
			float clampResult14 = clamp( ( pow( clampResult22 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult17 = lerp( tex2DNode1 , ( tex2DNode1 * float4(0.2156863,0.282353,0.4196079,0) ) , clampResult14);
			float clampResult35 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float clampResult39 = clamp( ( pow( clampResult35 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult40 = lerp( lerpResult17 , ( tex2DNode1 * _Highlight_intensity ) , clampResult39);
			float4 lerpResult28 = lerp( tex2DNode1 , lerpResult40 , _Highlight_value);
			float4 blendOpSrc45 = lerp(tex2DNode1,lerpResult28,_Highlight_platform);
			float4 blendOpDest45 = _Color2;
			float4 lerpResult47 = lerp( ( saturate( 2.0f*blendOpSrc45*blendOpDest45 + blendOpSrc45*blendOpSrc45*(1.0f - 2.0f*blendOpDest45) )) , lerp(tex2DNode1,lerpResult28,_Highlight_platform) , i.vertexColor.g);
			o.Albedo = lerpResult47.rgb;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float3 temp_cast_1 = (( tex2D( _TextureSample0, uv_TextureSample0 ).r * _Emissive )).xxx;
			o.Emission = temp_cast_1;
			float2 uv_RMAo = i.uv_texcoord * _RMAo_ST.xy + _RMAo_ST.zw;
			float4 tex2DNode4 = tex2D( _RMAo, uv_RMAo );
			o.Metallic = tex2DNode4.g;
			o.Smoothness = ( 1.0 - ( tex2DNode4.r + _Roughness ) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;978.335;1031.375;1.45497;True;True
Node;AmplifyShaderEditor.WorldNormalVector;8;-2323.208,-991.814;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-1458.808,-627.8139;Float;False;Constant;_Float4;Float 4;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;23;-2099.708,-945.3141;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;15;-1460.108,-704.514;Float;False;Constant;_Float3;Float 3;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;30;-2108.56,-552.0919;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-1419.481,-228.3028;Float;False;Constant;_Float5;Float 5;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-1831.907,-764.3142;Float;False;Constant;_Float1;Float 1;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;22;-1907.307,-920.6141;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-1420.781,-305.0028;Float;False;Constant;_Float7;Float 7;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-1639.507,-698.014;Float;False;Constant;_Float2;Float 2;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;10;-1643.407,-852.7141;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-1792.58,-364.803;Float;False;Constant;_Float8;Float 8;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;35;-1867.98,-521.103;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;26;-587.518,-964.5259;Float;False;Constant;_Color0;Color 0;5;0;0.2156863,0.282353,0.4196079,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;37;-1604.08,-453.2029;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-1600.18,-298.5028;Float;False;Constant;_Float9;Float 9;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1464.008,-820.2141;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-733,-184;Float;True;Property;_BaseColor;BaseColor;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1424.681,-420.7029;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-579.6806,-675.1541;Float;False;Property;_Highlight_intensity;Highlight_intensity;5;0;1.4;1;2;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-298.2176,-853.7261;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;14;-1301.508,-822.8141;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-305.9513,-728.8164;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;17;-103.8557,-804.6541;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;39;-1262.181,-423.3029;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;40;-92.28789,-672.8866;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;29;-184.4646,-488.9291;Float;False;Property;_Highlight_value;Highlight_value;5;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;28;187.2818,-587.5262;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ToggleSwitchNode;27;344.582,-470.5259;Float;False;Property;_Highlight_platform;Highlight_platform;6;0;1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;4;-723,257;Float;True;Property;_RMAo;RMAo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-434,435;Float;False;Property;_Roughness;Roughness;4;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;44;648.6349,-121.6571;Float;False;Property;_Color2;Color 2;22;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;-391.7881,949.7422;Float;False;Property;_Emissive;Emissive;8;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-905,94;Float;False;Property;_Float0;Float 0;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;41;-546.6394,751.1404;Float;True;Property;_TextureSample0;Texture Sample 0;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-263,361;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;45;952.2884,-127.8971;Float;False;SoftLight;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.VertexColorNode;46;701.3896,-380.0285;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-173.7881,774.7422;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;5;-285,274;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;47;1041.742,-347.3573;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;2;-733,44;Float;True;Property;_Normal;Normal;3;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;370.5,1.3;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/PBR;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;23;0;8;3
WireConnection;22;0;23;0
WireConnection;22;1;15;0
WireConnection;22;2;16;0
WireConnection;10;0;22;0
WireConnection;10;1;11;0
WireConnection;35;0;30;2
WireConnection;35;1;33;0
WireConnection;35;2;32;0
WireConnection;37;0;35;0
WireConnection;37;1;34;0
WireConnection;12;0;10;0
WireConnection;12;1;13;0
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;18;0;1;0
WireConnection;18;1;26;0
WireConnection;14;0;12;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;21;0;1;0
WireConnection;21;1;20;0
WireConnection;17;0;1;0
WireConnection;17;1;18;0
WireConnection;17;2;14;0
WireConnection;39;0;38;0
WireConnection;39;1;33;0
WireConnection;39;2;32;0
WireConnection;40;0;17;0
WireConnection;40;1;21;0
WireConnection;40;2;39;0
WireConnection;28;0;1;0
WireConnection;28;1;40;0
WireConnection;28;2;29;0
WireConnection;27;0;1;0
WireConnection;27;1;28;0
WireConnection;6;0;4;1
WireConnection;6;1;7;0
WireConnection;45;0;27;0
WireConnection;45;1;44;0
WireConnection;42;0;41;1
WireConnection;42;1;43;0
WireConnection;5;0;6;0
WireConnection;47;0;45;0
WireConnection;47;1;27;0
WireConnection;47;2;46;2
WireConnection;2;5;3;0
WireConnection;0;0;47;0
WireConnection;0;1;2;0
WireConnection;0;2;42;0
WireConnection;0;3;4;2
WireConnection;0;4;5;0
ASEEND*/
//CHKSM=E21BFAB24D33706F9B6D269D2EC4B79BA9AB1FA5