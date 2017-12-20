// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/PBR_rock"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Normal("Normal", 2D) = "bump" {}
		_T_Groundtile1_MaskMat("T_Groundtile1_MaskMat", 2D) = "white" {}
		_T_Groundtile1_MaskAdd("T_Groundtile1_MaskAdd", 2D) = "white" {}
		_Tiling_1("Tiling_1", Float) = 0
		_Mat1_C("Mat1_C", 2D) = "white" {}
		_Mat1_N("Mat1_N", 2D) = "white" {}
		_Mat1_RME("Mat1_RME", 2D) = "white" {}
		_Tiling_2("Tiling_2", Float) = 0
		_Mat2_C("Mat2_C", 2D) = "white" {}
		_Mat2_N("Mat2_N", 2D) = "white" {}
		_Mat2_RME("Mat2_RME", 2D) = "white" {}
		_Tiling_3("Tiling_3", Float) = 0
		_Mat3_C("Mat3_C", 2D) = "white" {}
		_Mat3_N("Mat3_N", 2D) = "white" {}
		_Mat3_RME("Mat3_RME", 2D) = "white" {}
		_Roughness_add("Roughness_add", Float) = 0.15
		_Ao("Ao", Range( 0 , 1)) = 0.2
		_Edge_color("Edge_color", Color) = (0,0,0,0)
		_Emissive("Emissive", Float) = 0
		[Toggle]_Highlight_platform("Highlight_platform", Float) = 0
		_normal_intensity("normal_intensity", Range( 0 , 1)) = 1
		_Highlight_value("Highlight_value", Range( 0 , 1)) = 0
		_Color_blend("Color_blend", Color) = (0,0,0,0)
		_Light_correction("Light_correction", Float) = 0
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
			float2 texcoord_0;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform float _normal_intensity;
		uniform sampler2D _Mat2_N;
		uniform float _Tiling_2;
		uniform sampler2D _Mat1_N;
		uniform float _Tiling_1;
		uniform sampler2D _T_Groundtile1_MaskMat;
		uniform float4 _T_Groundtile1_MaskMat_ST;
		uniform sampler2D _Mat3_N;
		uniform float _Tiling_3;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Highlight_platform;
		uniform sampler2D _Mat2_C;
		uniform sampler2D _Mat1_C;
		uniform sampler2D _Mat3_C;
		uniform float _Ao;
		uniform sampler2D _T_Groundtile1_MaskAdd;
		uniform float4 _T_Groundtile1_MaskAdd_ST;
		uniform float4 _Edge_color;
		uniform float _Highlight_value;
		uniform float4 _Color_blend;
		uniform float _Light_correction;
		uniform sampler2D _Mat2_RME;
		uniform sampler2D _Mat1_RME;
		uniform sampler2D _Mat3_RME;
		uniform float _Emissive;
		uniform float _Roughness_add;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );;
			float2 temp_output_127_0 = ( ( i.texcoord_0 * _Tiling_2 ) * ase_objectScale.x );
			float2 temp_output_122_0 = ( ( i.texcoord_0 * _Tiling_1 ) * ase_objectScale.x );
			float2 uv_T_Groundtile1_MaskMat = i.uv_texcoord * _T_Groundtile1_MaskMat_ST.xy + _T_Groundtile1_MaskMat_ST.zw;
			float4 tex2DNode47 = tex2D( _T_Groundtile1_MaskMat, uv_T_Groundtile1_MaskMat );
			float3 lerpResult61 = lerp( UnpackScaleNormal( tex2D( _Mat2_N, temp_output_127_0 ) ,_normal_intensity ) , UnpackScaleNormal( tex2D( _Mat1_N, temp_output_122_0 ) ,_normal_intensity ) , tex2DNode47.r);
			float2 temp_output_129_0 = ( ( i.texcoord_0 * _Tiling_3 ) * ase_objectScale.x );
			float3 lerpResult62 = lerp( lerpResult61 , UnpackScaleNormal( tex2D( _Mat3_N, temp_output_129_0 ) ,_normal_intensity ) , tex2DNode47.g);
			float3 Normal63 = lerpResult62;
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = BlendNormals( Normal63 , UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,1.0 ) );
			float4 tex2DNode114 = tex2D( _Mat2_C, temp_output_127_0 );
			float4 lerpResult59 = lerp( tex2DNode114 , tex2D( _Mat1_C, temp_output_122_0 ) , tex2DNode47.r);
			float4 lerpResult60 = lerp( lerpResult59 , tex2D( _Mat3_C, temp_output_129_0 ) , tex2DNode47.g);
			float2 uv_T_Groundtile1_MaskAdd = i.uv_texcoord * _T_Groundtile1_MaskAdd_ST.xy + _T_Groundtile1_MaskAdd_ST.zw;
			float4 tex2DNode46 = tex2D( _T_Groundtile1_MaskAdd, uv_T_Groundtile1_MaskAdd );
			float4 lerpResult75 = lerp( lerpResult60 , ( lerpResult60 * _Ao ) , tex2DNode46.g);
			float4 lerpResult77 = lerp( lerpResult75 , ( tex2DNode114 * _Edge_color ) , tex2DNode46.r);
			float4 Albedo44 = lerpResult77;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float clampResult22 = clamp( -ase_worldNormal.z , 0.0 , 1.0 );
			float clampResult14 = clamp( ( pow( clampResult22 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult17 = lerp( Albedo44 , ( Albedo44 * float4(0.2156863,0.282353,0.4196079,0) ) , clampResult14);
			float clampResult35 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float clampResult39 = clamp( ( pow( clampResult35 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult40 = lerp( lerpResult17 , ( Albedo44 * 1.4 ) , clampResult39);
			float4 lerpResult28 = lerp( Albedo44 , lerpResult40 , _Highlight_value);
			float4 blendOpSrc131 = lerp(Albedo44,lerpResult28,_Highlight_platform);
			float4 blendOpDest131 = _Color_blend;
			float4 lerpResult134 = lerp( ( saturate( 2.0f*blendOpSrc131*blendOpDest131 + blendOpSrc131*blendOpSrc131*(1.0f - 2.0f*blendOpDest131) )) , lerp(Albedo44,lerpResult28,_Highlight_platform) , i.vertexColor.g);
			o.Albedo = ( lerpResult134 * _Light_correction ).rgb;
			float4 lerpResult64 = lerp( tex2D( _Mat2_RME, temp_output_127_0 ) , tex2D( _Mat1_RME, temp_output_122_0 ) , tex2DNode47.r);
			float4 lerpResult65 = lerp( lerpResult64 , tex2D( _Mat3_RME, temp_output_129_0 ) , tex2DNode47.g);
			float4 RME66 = lerpResult65;
			float3 temp_cast_1 = (( ( tex2DNode46.b + RME66.b ) * _Emissive )).xxx;
			o.Emission = temp_cast_1;
			o.Metallic = 0.0;
			o.Smoothness = ( 1.0 - ( RME66.r + _Roughness_add ) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				Input customInputData;
				vertexDataFunc( v, customInputData );
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
-1913;29;1906;1004;994.189;853.233;1.3;True;True
Node;AmplifyShaderEditor.RangedFloatNode;83;-3483.362,460.2121;Float;False;Property;_Tiling_1;Tiling_1;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;120;-3525.898,317.4635;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;84;-3505.25,1335.453;Float;False;Property;_Tiling_2;Tiling_2;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;87;-3454.317,2123.366;Float;False;Property;_Tiling_3;Tiling_3;11;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ObjectScaleNode;123;-3253.336,458.0667;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-3283.813,1199.093;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-3230.407,335.325;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;58;-2839.75,248.3901;Float;False;381;721;Red;3;111;112;113;Material_1;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;-3266.43,2041.623;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-3047.459,1205.145;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-2994.053,341.3773;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;57;-2848.219,1110.142;Float;False;381;720.9998;Green;3;114;115;116;Material_2;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;47;-2731.536,2756.923;Float;True;Property;_T_Groundtile1_MaskMat;T_Groundtile1_MaskMat;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-3067.036,2043.411;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;56;-2843.17,2012.426;Float;False;381;721.0001;Blue;3;117;118;119;Material_3;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;111;-2796.905,304.1977;Float;True;Property;_Mat1_C;Mat1_C;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;114;-2785.629,1167.642;Float;True;Property;_Mat2_C;Mat2_C;8;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;117;-2787.57,2077.576;Float;True;Property;_Mat3_C;Mat3_C;12;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;59;-2200.174,2053.168;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WorldNormalVector;8;-4316.181,-1326.29;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;60;-2029.128,2052.403;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;76;-1912.646,1023.122;Float;False;Property;_Ao;Ao;16;0;0.2;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-3451.781,-962.29;Float;False;Constant;_Float4;Float 4;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;15;-3453.082,-1038.99;Float;False;Constant;_Float3;Float 3;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1711.571,992.7242;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;46;-1754.117,240.5636;Float;True;Property;_T_Groundtile1_MaskAdd;T_Groundtile1_MaskAdd;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;78;-1950.932,1157.943;Float;False;Property;_Edge_color;Edge_color;17;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NegateNode;23;-4092.681,-1279.79;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-3469.818,-648.8235;Float;False;Constant;_Float5;Float 5;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;75;-1541.29,962.881;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WorldNormalVector;30;-4158.896,-972.6129;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-3824.881,-1098.79;Float;False;Constant;_Float1;Float 1;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-1536.513,1099.718;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;33;-3471.118,-725.5237;Float;False;Constant;_Float7;Float 7;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;22;-3900.281,-1255.09;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;35;-3918.317,-941.624;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-3632.481,-1032.49;Float;False;Constant;_Float2;Float 2;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;77;-1285.273,977.2715;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.PowerNode;10;-3636.381,-1187.19;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-3842.916,-785.324;Float;False;Constant;_Float8;Float 8;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;26;-2840.265,-1193.855;Float;False;Constant;_Color0;Color 0;5;0;0.2156863,0.282353,0.4196079,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-1843.326,2055.267;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;45;-2922.283,-760.2347;Float;False;44;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-3456.982,-1154.69;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-3650.516,-719.0236;Float;False;Constant;_Float9;Float 9;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;37;-3654.416,-873.7239;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;14;-3294.482,-1157.29;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-2813.565,-1008.955;Float;False;Constant;_Float6;Float 6;5;0;1.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2550.965,-1083.055;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3475.018,-841.2239;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;17;-2356.603,-1033.983;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ClampOpNode;39;-3312.518,-843.8239;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2558.699,-958.1456;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;113;-2790.791,730.0394;Float;True;Property;_Mat1_RME;Mat1_RME;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;116;-2779.515,1591.542;Float;True;Property;_Mat2_RME;Mat2_RME;10;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;40;-2345.035,-902.2158;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;29;-2435.473,-661.6988;Float;False;Property;_Highlight_value;Highlight_value;21;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;119;-2781.456,2501.476;Float;True;Property;_Mat3_RME;Mat3_RME;14;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;124;-3230.406,644.2023;Float;False;Property;_normal_intensity;normal_intensity;20;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;64;-2202.644,2467.847;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;28;-2065.466,-816.8553;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;112;-2792.743,506.5692;Float;True;Property;_Mat1_N;Mat1_N;5;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;115;-2781.467,1368.072;Float;True;Property;_Mat2_N;Mat2_N;9;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;65;-2043.664,2465.889;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ToggleSwitchNode;27;-1851.062,-719.4846;Float;False;Property;_Highlight_platform;Highlight_platform;19;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;133;-70.98888,-180.3895;Float;False;Property;_Color_blend;Color_blend;22;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;61;-2187.519,2241.581;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1776.269,474.3281;Float;False;66;0;1;COLOR
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-1857.862,2468.752;Float;False;RME;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;118;-2783.408,2278.006;Float;True;Property;_Mat3_N;Mat3_N;13;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;135;-18.23413,-438.7609;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;131;232.6646,-186.6295;Float;False;SoftLight;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;92;-755.1174,692.275;Float;False;Constant;_Float13;Float 13;21;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-278.3486,286.4106;Float;False;Property;_Roughness_add;Roughness_add;15;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;62;-2033.215,2242.74;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.BreakToComponentsNode;71;-1576.677,477.0505;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;134;322.1181,-406.0897;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.GetLocalVarNode;69;-456.1378,552.6901;Float;False;63;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;73;-1301.58,262.9671;Float;False;Property;_Emissive;Emissive;18;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;6;0.6514162,203.4106;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-1847.413,2245.603;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;137;255.1109,-497.033;Float;False;Property;_Light_correction;Light_correction;23;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-1262.344,344.9939;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;90;-539.907,649.7944;Float;True;Property;_Normal;Normal;0;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;79;143.4044,80.64252;Float;False;Constant;_Float0;Float 0;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;507.311,-394.3329;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1120.736,341.8653;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;91;-171.6362,614.7443;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;5;141.5772,169.2511;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;576.0913,23.72815;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/PBR_rock;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;126;0;120;0
WireConnection;126;1;84;0
WireConnection;121;0;120;0
WireConnection;121;1;83;0
WireConnection;128;0;120;0
WireConnection;128;1;87;0
WireConnection;127;0;126;0
WireConnection;127;1;123;1
WireConnection;122;0;121;0
WireConnection;122;1;123;1
WireConnection;129;0;128;0
WireConnection;129;1;123;1
WireConnection;111;1;122;0
WireConnection;114;1;127;0
WireConnection;117;1;129;0
WireConnection;59;0;114;0
WireConnection;59;1;111;0
WireConnection;59;2;47;1
WireConnection;60;0;59;0
WireConnection;60;1;117;0
WireConnection;60;2;47;2
WireConnection;74;0;60;0
WireConnection;74;1;76;0
WireConnection;23;0;8;3
WireConnection;75;0;60;0
WireConnection;75;1;74;0
WireConnection;75;2;46;2
WireConnection;130;0;114;0
WireConnection;130;1;78;0
WireConnection;22;0;23;0
WireConnection;22;1;15;0
WireConnection;22;2;16;0
WireConnection;35;0;30;2
WireConnection;35;1;33;0
WireConnection;35;2;32;0
WireConnection;77;0;75;0
WireConnection;77;1;130;0
WireConnection;77;2;46;1
WireConnection;10;0;22;0
WireConnection;10;1;11;0
WireConnection;44;0;77;0
WireConnection;12;0;10;0
WireConnection;12;1;13;0
WireConnection;37;0;35;0
WireConnection;37;1;34;0
WireConnection;14;0;12;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;18;0;45;0
WireConnection;18;1;26;0
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;17;0;45;0
WireConnection;17;1;18;0
WireConnection;17;2;14;0
WireConnection;39;0;38;0
WireConnection;39;1;33;0
WireConnection;39;2;32;0
WireConnection;21;0;45;0
WireConnection;21;1;20;0
WireConnection;113;1;122;0
WireConnection;116;1;127;0
WireConnection;40;0;17;0
WireConnection;40;1;21;0
WireConnection;40;2;39;0
WireConnection;119;1;129;0
WireConnection;64;0;116;0
WireConnection;64;1;113;0
WireConnection;64;2;47;1
WireConnection;28;0;45;0
WireConnection;28;1;40;0
WireConnection;28;2;29;0
WireConnection;112;1;122;0
WireConnection;112;5;124;0
WireConnection;115;1;127;0
WireConnection;115;5;124;0
WireConnection;65;0;64;0
WireConnection;65;1;119;0
WireConnection;65;2;47;2
WireConnection;27;0;45;0
WireConnection;27;1;28;0
WireConnection;61;0;115;0
WireConnection;61;1;112;0
WireConnection;61;2;47;1
WireConnection;66;0;65;0
WireConnection;118;1;129;0
WireConnection;118;5;124;0
WireConnection;131;0;27;0
WireConnection;131;1;133;0
WireConnection;62;0;61;0
WireConnection;62;1;118;0
WireConnection;62;2;47;2
WireConnection;71;0;70;0
WireConnection;134;0;131;0
WireConnection;134;1;27;0
WireConnection;134;2;135;2
WireConnection;6;0;71;0
WireConnection;6;1;7;0
WireConnection;63;0;62;0
WireConnection;80;0;46;3
WireConnection;80;1;71;2
WireConnection;90;5;92;0
WireConnection;136;0;134;0
WireConnection;136;1;137;0
WireConnection;72;0;80;0
WireConnection;72;1;73;0
WireConnection;91;0;69;0
WireConnection;91;1;90;0
WireConnection;5;0;6;0
WireConnection;0;0;136;0
WireConnection;0;1;91;0
WireConnection;0;2;72;0
WireConnection;0;3;79;0
WireConnection;0;4;5;0
ASEEND*/
//CHKSM=5834EC5FCBE5B541A05FFC52E3C9B9172953841E