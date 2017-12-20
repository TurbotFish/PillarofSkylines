// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/PBR_tp_vp"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Normal("Normal", 2D) = "bump" {}
		_MaskAdd("MaskAdd", 2D) = "white" {}
		_Roughness_add("Roughness_add", Float) = 0.15
		_Ao("Ao", Range( 0 , 1)) = 0.2
		_Edge_color("Edge_color", Color) = (0,0,0,0)
		_Emissive("Emissive", Float) = 0
		[Toggle]_Highlight_platform("Highlight_platform", Float) = 1
		_Highlight_value("Highlight_value", Range( 0 , 1)) = 0
		_Tiling_1("Tiling_1", Float) = 0
		_C1("C1", 2D) = "white" {}
		_N1("N1", 2D) = "bump" {}
		_RME1("RME1", 2D) = "white" {}
		_Tiling_2("Tiling_2", Float) = 0
		_C2("C2", 2D) = "white" {}
		_N2("N2", 2D) = "bump" {}
		_RME2("RME2", 2D) = "white" {}
		_Tiling_3("Tiling_3", Float) = 0
		_C3("C3", 2D) = "white" {}
		_N3("N3", 2D) = "bump" {}
		_RME3("RME3", 2D) = "white" {}
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform sampler2D _N2;
		uniform float _Tiling_2;
		uniform sampler2D _N1;
		uniform float _Tiling_1;
		uniform sampler2D _N3;
		uniform float _Tiling_3;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Highlight_platform;
		uniform sampler2D _C2;
		uniform sampler2D _C1;
		uniform sampler2D _C3;
		uniform float _Ao;
		uniform sampler2D _MaskAdd;
		uniform float4 _MaskAdd_ST;
		uniform float4 _Edge_color;
		uniform float _Highlight_value;
		uniform float _Emissive;
		uniform sampler2D _RME2;
		uniform sampler2D _RME1;
		uniform sampler2D _RME3;
		uniform float _Roughness_add;


		inline float3 TriplanarNormal( sampler2D topBumpMap, sampler2D midBumpMap, sampler2D botBumpMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign(worldNormal);
			half3 xNorm; half3 yNorm; half3 zNorm;
			if(vertex == 1){
			xNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );
			yNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			zNorm = UnpackNormal( tex2Dlod( topBumpMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );
			} else {
			xNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.zx ) );
			zNorm = UnpackNormal( tex2D( topBumpMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			}
			xNorm = normalize( half3( xNorm.xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ) );
			yNorm = normalize( half3( yNorm.xy + worldNormal.zx, worldNormal.y));
			zNorm = normalize( half3( zNorm.xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ) );
			xNorm = xNorm.zyx;
			yNorm = yNorm.yzx;
			zNorm = zNorm.xyz;
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float vertex )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			if(vertex == 1){
			xNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zy * float2( nsign.x, 1.0 )).xy,0,0) ) );
			yNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.zx).xy,0,0) ) );
			zNorm = ( tex2Dlod( topTexMap, float4((tilling * worldPos.xy * float2( -nsign.z, 1.0 )).xy,0,0) ) );
			} else {
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.zx ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			}
			return xNorm* projNormal.x + yNorm* projNormal.y + zNorm* projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal137 = TriplanarNormal( _N2, _N2, _N2, ase_worldPos, ase_worldNormal, 2.0, _Tiling_2, 0 );
			float3 tanTriplanarNormal137 = mul( ase_worldToTangent, worldTriplanarNormal137 );
			float3 worldTriplanarNormal133 = TriplanarNormal( _N1, _N1, _N1, ase_worldPos, ase_worldNormal, 2.0, _Tiling_1, 0 );
			float3 tanTriplanarNormal133 = mul( ase_worldToTangent, worldTriplanarNormal133 );
			float lerpResult143 = lerp( 0.0 , 1.0 , i.vertexColor.g);
			float3 lerpResult61 = lerp( tanTriplanarNormal137 , tanTriplanarNormal133 , lerpResult143);
			float3 worldTriplanarNormal140 = TriplanarNormal( _N3, _N3, _N3, ase_worldPos, ase_worldNormal, 2.0, _Tiling_3, 0 );
			float3 tanTriplanarNormal140 = mul( ase_worldToTangent, worldTriplanarNormal140 );
			float3 lerpResult62 = lerp( lerpResult61 , tanTriplanarNormal140 , i.vertexColor.r);
			float3 Normal63 = lerpResult62;
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = BlendNormals( Normal63 , UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,1.0 ) );
			float4 triplanar135 = TriplanarSampling( _C2, _C2, _C2, ase_worldPos, ase_worldNormal, 2.0, _Tiling_2, 0 );
			float4 triplanar131 = TriplanarSampling( _C1, _C1, _C1, ase_worldPos, ase_worldNormal, 2.0, _Tiling_1, 0 );
			float4 lerpResult59 = lerp( triplanar135 , triplanar131 , lerpResult143);
			float4 triplanar138 = TriplanarSampling( _C3, _C3, _C3, ase_worldPos, ase_worldNormal, 2.0, _Tiling_3, 0 );
			float4 lerpResult60 = lerp( lerpResult59 , triplanar138 , i.vertexColor.r);
			float2 uv_MaskAdd = i.uv_texcoord * _MaskAdd_ST.xy + _MaskAdd_ST.zw;
			float4 tex2DNode46 = tex2D( _MaskAdd, uv_MaskAdd );
			float4 lerpResult75 = lerp( lerpResult60 , ( lerpResult60 * _Ao ) , tex2DNode46.g);
			float4 lerpResult77 = lerp( lerpResult75 , ( triplanar135 * _Edge_color ) , tex2DNode46.r);
			float4 Albedo44 = lerpResult77;
			float clampResult22 = clamp( -ase_worldNormal.z , 0.0 , 1.0 );
			float clampResult14 = clamp( ( pow( clampResult22 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult17 = lerp( Albedo44 , ( Albedo44 * float4(0.2156863,0.282353,0.4196079,0) ) , clampResult14);
			float clampResult35 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float clampResult39 = clamp( ( pow( clampResult35 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult40 = lerp( lerpResult17 , ( Albedo44 * 1.4 ) , clampResult39);
			float4 lerpResult28 = lerp( Albedo44 , lerpResult40 , _Highlight_value);
			o.Albedo = lerp(Albedo44,lerpResult28,_Highlight_platform).xyz;
			float3 temp_cast_3 = (( ( tex2DNode46.b + 0.0 ) * _Emissive )).xxx;
			o.Emission = temp_cast_3;
			o.Metallic = 0.0;
			float4 triplanar136 = TriplanarSampling( _RME2, _RME2, _RME2, ase_worldPos, ase_worldNormal, 2.0, _Tiling_2, 0 );
			float4 triplanar134 = TriplanarSampling( _RME1, _RME1, _RME1, ase_worldPos, ase_worldNormal, 2.0, _Tiling_1, 0 );
			float4 lerpResult64 = lerp( triplanar136 , triplanar134 , lerpResult143);
			float4 triplanar139 = TriplanarSampling( _RME3, _RME3, _RME3, ase_worldPos, ase_worldNormal, 2.0, _Tiling_3, 0 );
			float4 lerpResult65 = lerp( lerpResult64 , triplanar139 , i.vertexColor.r);
			float4 RME66 = lerpResult65;
			o.Smoothness = ( 1.0 - ( RME66.x + _Roughness_add ) );
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
				surfIN.worldPos = worldPos;
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
-1902;30;1906;1004;744.5114;283.5932;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;83;-3485.962,367.9121;Float;False;Property;_Tiling_1;Tiling_1;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;144;-2600.667,2946.803;Float;False;Constant;_Float11;Float 11;22;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;145;-2602.717,3023.292;Float;False;Constant;_Float12;Float 12;22;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;141;-2627.666,2767.771;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;84;-3505.25,1335.453;Float;False;Property;_Tiling_2;Tiling_2;14;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;-3944.316,1135.939;Float;False;Constant;_Float10;Float 10;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;58;-2839.75,248.3901;Float;False;381;721;Red;3;131;133;134;Material_1;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;57;-2848.219,1110.142;Float;False;381;720.9998;Green;3;135;136;137;Material_2;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;143;-2373.758,2944.487;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;56;-2843.17,2012.426;Float;False;381;721.0001;Blue;3;138;139;140;Material_3;1,1,1,1;0;0
Node;AmplifyShaderEditor.TriplanarNode;135;-2823.191,1183.02;Float;True;Spherical;World;False;C2;_C2;white;15;None;Mid Texture 1;_MidTexture1;white;23;None;Bot Texture 1;_BotTexture1;white;24;None;c2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;131;-2818.55,298.5682;Float;True;Spherical;World;False;C1;_C1;white;11;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;c1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;87;-3485.047,2113.123;Float;False;Property;_Tiling_3;Tiling_3;18;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;59;-2200.174,2053.168;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;138;-2819.174,2090.666;Float;True;Spherical;World;False;C3;_C3;white;19;None;Mid Texture 2;_MidTexture2;white;23;None;Bot Texture 2;_BotTexture2;white;24;None;c3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;76;-2019.939,1013.368;Float;False;Property;_Ao;Ao;4;0;0.2;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;8;-4316.181,-1326.29;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;60;-2029.128,2052.403;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;15;-3453.082,-1038.99;Float;False;Constant;_Float3;Float 3;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.NegateNode;23;-4092.681,-1279.79;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;46;-1754.117,240.5636;Float;True;Property;_MaskAdd;MaskAdd;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-3451.781,-962.29;Float;False;Constant;_Float4;Float 4;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;78;-1964.932,1221.943;Float;False;Property;_Edge_color;Edge_color;5;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1711.571,992.7242;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;32;-3469.818,-648.8235;Float;False;Constant;_Float5;Float 5;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-3824.881,-1098.79;Float;False;Constant;_Float1;Float 1;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-1536.513,1099.718;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;22;-3900.281,-1255.09;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;75;-1541.29,962.881;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.WorldNormalVector;30;-4158.896,-972.6129;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-3471.118,-725.5237;Float;False;Constant;_Float7;Float 7;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;136;-2813.204,1602.932;Float;True;Spherical;World;False;RME2;_RME2;white;17;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;rme2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;10;-3636.381,-1187.19;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-3842.916,-785.324;Float;False;Constant;_Float8;Float 8;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;35;-3918.317,-941.624;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-3632.481,-1032.49;Float;False;Constant;_Float2;Float 2;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;134;-2806.584,720.4589;Float;True;Spherical;World;False;RME1;_RME1;white;13;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;rme1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;77;-1285.273,977.2715;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-3456.982,-1154.69;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;45;-2922.283,-760.2347;Float;False;44;0;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;139;-2808.492,2511.271;Float;True;Spherical;World;False;RME3;_RME3;white;21;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;rme3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-3650.516,-719.0236;Float;False;Constant;_Float9;Float 9;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-1843.326,2055.267;Float;False;Albedo;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;26;-2840.265,-1193.855;Float;False;Constant;_Color0;Color 0;5;0;0.2156863,0.282353,0.4196079,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;37;-3654.416,-873.7239;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;64;-2202.644,2467.847;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;20;-2813.565,-1008.955;Float;False;Constant;_Float6;Float 6;5;0;1.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;65;-2043.664,2465.889;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3475.018,-841.2239;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;133;-2817.815,500.051;Float;True;Spherical;World;True;N1;_N1;bump;12;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;n1;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2550.965,-1083.055;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;14;-3294.482,-1157.29;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;137;-2824.435,1382.524;Float;True;Spherical;World;True;N2;_N2;bump;16;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;n2;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;17;-2356.603,-1033.983;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;39;-3312.518,-843.8239;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;61;-2187.519,2241.581;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.TriplanarNode;140;-2818.438,2292.148;Float;True;Spherical;World;True;N3;_N3;bump;20;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;n3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2558.699,-958.1456;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1776.269,474.3281;Float;False;66;0;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-1857.862,2468.752;Float;False;RME;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;40;-2345.035,-902.2158;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;29;-2435.473,-661.6988;Float;False;Property;_Highlight_value;Highlight_value;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;62;-2033.215,2242.74;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.BreakToComponentsNode;71;-1576.677,477.0505;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-278.3486,286.4106;Float;False;Property;_Roughness_add;Roughness_add;3;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-757.4713,-454.064;Float;False;Constant;_Float13;Float 13;21;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;28;-2065.466,-816.8553;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;73;-1301.58,262.9671;Float;False;Property;_Emissive;Emissive;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;6;0.6514162,203.4106;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-1847.413,2245.603;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.GetLocalVarNode;69;-458.4917,-593.6489;Float;False;63;0;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;90;-542.2609,-496.5446;Float;True;Property;_Normal;Normal;0;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-1262.344,344.9939;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;27;-1851.062,-719.4846;Float;False;Property;_Highlight_platform;Highlight_platform;7;0;1;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1120.736,341.8653;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;5;141.5772,169.2511;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;124;-3480.263,809.0059;Float;False;Property;_normal_intensity;normal_intensity;8;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;91;-173.9901,-531.5947;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;142;-2809.315,3144.449;Float;True;Property;_MaskMat;MaskMat;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;79;143.4044,80.64252;Float;False;Constant;_Float0;Float 0;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;146;-2432.373,3132.954;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;576.0913,23.72815;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/PBR_tp_vp;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;143;0;144;0
WireConnection;143;1;145;0
WireConnection;143;2;141;2
WireConnection;135;3;84;0
WireConnection;135;4;132;0
WireConnection;131;3;83;0
WireConnection;131;4;132;0
WireConnection;59;0;135;0
WireConnection;59;1;131;0
WireConnection;59;2;143;0
WireConnection;138;3;87;0
WireConnection;138;4;132;0
WireConnection;60;0;59;0
WireConnection;60;1;138;0
WireConnection;60;2;141;1
WireConnection;23;0;8;3
WireConnection;74;0;60;0
WireConnection;74;1;76;0
WireConnection;130;0;135;0
WireConnection;130;1;78;0
WireConnection;22;0;23;0
WireConnection;22;1;15;0
WireConnection;22;2;16;0
WireConnection;75;0;60;0
WireConnection;75;1;74;0
WireConnection;75;2;46;2
WireConnection;136;3;84;0
WireConnection;136;4;132;0
WireConnection;10;0;22;0
WireConnection;10;1;11;0
WireConnection;35;0;30;2
WireConnection;35;1;33;0
WireConnection;35;2;32;0
WireConnection;134;3;83;0
WireConnection;134;4;132;0
WireConnection;77;0;75;0
WireConnection;77;1;130;0
WireConnection;77;2;46;1
WireConnection;12;0;10;0
WireConnection;12;1;13;0
WireConnection;139;3;87;0
WireConnection;139;4;132;0
WireConnection;44;0;77;0
WireConnection;37;0;35;0
WireConnection;37;1;34;0
WireConnection;64;0;136;0
WireConnection;64;1;134;0
WireConnection;64;2;143;0
WireConnection;65;0;64;0
WireConnection;65;1;139;0
WireConnection;65;2;141;1
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;133;3;83;0
WireConnection;133;4;132;0
WireConnection;18;0;45;0
WireConnection;18;1;26;0
WireConnection;14;0;12;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;137;3;84;0
WireConnection;137;4;132;0
WireConnection;17;0;45;0
WireConnection;17;1;18;0
WireConnection;17;2;14;0
WireConnection;39;0;38;0
WireConnection;39;1;33;0
WireConnection;39;2;32;0
WireConnection;61;0;137;0
WireConnection;61;1;133;0
WireConnection;61;2;143;0
WireConnection;140;3;87;0
WireConnection;140;4;132;0
WireConnection;21;0;45;0
WireConnection;21;1;20;0
WireConnection;66;0;65;0
WireConnection;40;0;17;0
WireConnection;40;1;21;0
WireConnection;40;2;39;0
WireConnection;62;0;61;0
WireConnection;62;1;140;0
WireConnection;62;2;141;1
WireConnection;71;0;70;0
WireConnection;28;0;45;0
WireConnection;28;1;40;0
WireConnection;28;2;29;0
WireConnection;6;0;71;0
WireConnection;6;1;7;0
WireConnection;63;0;62;0
WireConnection;90;5;92;0
WireConnection;80;0;46;3
WireConnection;27;0;45;0
WireConnection;27;1;28;0
WireConnection;72;0;80;0
WireConnection;72;1;73;0
WireConnection;5;0;6;0
WireConnection;91;0;69;0
WireConnection;91;1;90;0
WireConnection;146;0;144;0
WireConnection;146;1;145;0
WireConnection;146;2;142;2
WireConnection;0;0;27;0
WireConnection;0;1;91;0
WireConnection;0;2;72;0
WireConnection;0;3;79;0
WireConnection;0;4;5;0
ASEEND*/
//CHKSM=EF1AE6CE3EF0431F8F3B9FD8C820EF3DD991A934