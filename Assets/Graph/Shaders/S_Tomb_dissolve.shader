// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Tomb_dissolve"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Normal("Normal", 2D) = "bump" {}
		_MaskAdd("MaskAdd", 2D) = "white" {}
		_Roughness_add("Roughness_add", Float) = 0.15
		_TextureSample2("Texture Sample 2", 2D) = "white" {}
		_Ao("Ao", Range( 0 , 1)) = 0.2
		_Edge_color("Edge_color", Color) = (0,0,0,0)
		[Toggle]_Highlight_platform("Highlight_platform", Float) = 1
		_Highlight_value("Highlight_value", Range( 0 , 1)) = 0
		_Tiling_3("Tiling_3", Float) = 0
		_C3("C3", 2D) = "white" {}
		_N3("N3", 2D) = "bump" {}
		_RME3("RME3", 2D) = "white" {}
		_Dissolve("Dissolve", Range( 0 , 1)) = 1
		_Noise("Noise", 2D) = "white" {}
		_Noise_tiling("Noise_tiling", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
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
			float2 uv_texcoord;
			float2 texcoord_0;
		};

		uniform sampler2D _N3;
		uniform float _Tiling_3;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Highlight_platform;
		uniform sampler2D _C3;
		uniform float _Ao;
		uniform sampler2D _MaskAdd;
		uniform float4 _MaskAdd_ST;
		uniform float4 _Edge_color;
		uniform float _Highlight_value;
		uniform float _Dissolve;
		uniform sampler2D _Noise;
		uniform float _Noise_tiling;
		uniform sampler2D _TextureSample2;
		uniform sampler2D _RME3;
		uniform float _Roughness_add;
		uniform float _Cutoff = 0.5;


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal140 = TriplanarNormal( _N3, _N3, _N3, ase_worldPos, ase_worldNormal, 2.0, _Tiling_3, 0 );
			float3 tanTriplanarNormal140 = mul( ase_worldToTangent, worldTriplanarNormal140 );
			float3 Normal63 = tanTriplanarNormal140;
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = BlendNormals( Normal63 , UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,1.0 ) );
			float4 triplanar138 = TriplanarSampling( _C3, _C3, _C3, ase_worldPos, ase_worldNormal, 2.0, _Tiling_3, 0 );
			float2 uv_MaskAdd = i.uv_texcoord * _MaskAdd_ST.xy + _MaskAdd_ST.zw;
			float4 tex2DNode46 = tex2D( _MaskAdd, uv_MaskAdd );
			float4 lerpResult75 = lerp( triplanar138 , ( triplanar138 * _Ao ) , tex2DNode46.g);
			float4 lerpResult77 = lerp( lerpResult75 , ( triplanar138 * _Edge_color ) , tex2DNode46.r);
			float4 Albedo44 = lerpResult77;
			float clampResult22 = clamp( -ase_worldNormal.z , 0.0 , 1.0 );
			float clampResult14 = clamp( ( pow( clampResult22 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult17 = lerp( Albedo44 , ( Albedo44 * float4(0.2156863,0.282353,0.4196079,0) ) , clampResult14);
			float clampResult35 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float clampResult39 = clamp( ( pow( clampResult35 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult40 = lerp( lerpResult17 , ( Albedo44 * 1.4 ) , clampResult39);
			float4 lerpResult28 = lerp( Albedo44 , lerpResult40 , _Highlight_value);
			o.Albedo = lerp(Albedo44,lerpResult28,_Highlight_platform).xyz;
			float3 localPos = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) );
			float3 localNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float4 triplanar155 = TriplanarSampling( _Noise, _Noise, _Noise, localPos, localNormal, 2.0, _Noise_tiling, 0 );
			float temp_output_178_0 = ( ( (-0.6 + (( 1.0 - _Dissolve ) - 0.0) * (0.6 - -0.6) / (1.0 - 0.0)) + triplanar155.x ) * ( 1.0 - ( i.texcoord_0.x + (-1.0 + (_Dissolve - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) ) ) );
			float clampResult166 = clamp( (-4.0 + (temp_output_178_0 - 0.0) * (4.0 - -4.0) / (1.0 - 0.0)) , 0.0 , 1.0 );
			float temp_output_167_0 = ( 1.0 - clampResult166 );
			float4 appendResult173 = (float4(temp_output_167_0 , 0.0 , 0.0 , 0.0));
			o.Emission = ( temp_output_167_0 * ( tex2D( _TextureSample2, appendResult173.xy ) * 3.0 ) ).xyz;
			o.Metallic = 0.0;
			float4 triplanar139 = TriplanarSampling( _RME3, _RME3, _RME3, ase_worldPos, ase_worldNormal, 2.0, _Tiling_3, 0 );
			float4 RME66 = triplanar139;
			o.Smoothness = ( 1.0 - ( RME66.x + _Roughness_add ) );
			o.Alpha = 1;
			clip( temp_output_178_0 - _Cutoff );
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
-1913;29;1906;1004;564.7623;-381.5765;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;183;-113.3059,546.8859;Float;False;Constant;_Float19;Float 19;18;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;153;34.59244,728.895;Float;False;Property;_Dissolve;Dissolve;15;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;185;87.96068,576.8038;Float;False;Constant;_Float21;Float 21;18;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;186;89.32056,650.2388;Float;False;Constant;_Float22;Float 22;18;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;184;-111.946,624.4006;Float;False;Constant;_Float20;Float 20;18;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;56;-2843.17,2012.426;Float;False;381;721.0001;Blue;3;138;139;140;Material_3;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;177;104.7056,453.3687;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;87;-3485.047,2113.123;Float;False;Property;_Tiling_3;Tiling_3;11;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;182;335.4639,548.2455;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;154;321.0383,734.2711;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;-3890.513,1697.88;Float;False;Constant;_Float10;Float 10;14;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;156;102.0944,922.4468;Float;False;Property;_Noise_tiling;Noise_tiling;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;8;-4316.181,-1326.29;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;76;-2019.939,1013.368;Float;False;Property;_Ao;Ao;6;0;0.2;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;151;501.5042,729.4719;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-0.6;False;4;FLOAT;0.6;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;155;314.9741,926.1371;Float;True;Spherical;Object;False;Noise;_Noise;white;16;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;Mid Texture 0;_MidTexture0;white;26;None;Bot Texture 0;_BotTexture0;white;27;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;138;-2819.174,2090.666;Float;True;Spherical;World;False;C3;_C3;white;12;None;Mid Texture 2;_MidTexture2;white;23;None;Bot Texture 2;_BotTexture2;white;24;None;c3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;180;376.2614,408.175;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;78;-1964.932,1221.943;Float;False;Property;_Edge_color;Edge_color;7;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-1711.571,992.7242;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.NegateNode;23;-4092.681,-1279.79;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;179;525.9319,479.2435;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;15;-3453.082,-1038.99;Float;False;Constant;_Float3;Float 3;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;46;-1754.117,240.5636;Float;True;Property;_MaskAdd;MaskAdd;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;152;708.2502,711.9738;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-3451.781,-962.29;Float;False;Constant;_Float4;Float 4;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;75;-1541.29,962.881;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;22;-3900.281,-1255.09;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;30;-4158.896,-972.6129;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-3471.118,-725.5237;Float;False;Constant;_Float7;Float 7;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;818.9319,411.2435;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;32;-3469.818,-648.8235;Float;False;Constant;_Float5;Float 5;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;175;-225.4666,1815.303;Float;False;Constant;_Float17;Float 17;18;0;-4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-1536.513,1099.718;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;11;-3824.881,-1098.79;Float;False;Constant;_Float1;Float 1;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;176;-222.4666,1894.303;Float;False;Constant;_Float18;Float 18;18;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;174;-227.4666,1731.303;Float;False;Constant;_Float16;Float 16;18;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-3842.916,-785.324;Float;False;Constant;_Float8;Float 8;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;35;-3918.317,-941.624;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;165;-33.15071,1740.017;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-4.0;False;4;FLOAT;4.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-3632.481,-1032.49;Float;False;Constant;_Float2;Float 2;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;77;-1285.273,977.2715;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.PowerNode;10;-3636.381,-1187.19;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;37;-3654.416,-873.7239;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;26;-2840.265,-1193.855;Float;False;Constant;_Color0;Color 0;5;0;0.2156863,0.282353,0.4196079,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-3650.516,-719.0236;Float;False;Constant;_Float9;Float 9;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-3456.982,-1154.69;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;45;-2922.283,-760.2347;Float;False;44;0;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;166;47.36783,1549.436;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-1851.186,2074.916;Float;False;Albedo;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;14;-3294.482,-1157.29;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3475.018,-841.2239;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2550.965,-1083.055;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;139;-2808.492,2511.271;Float;True;Spherical;World;False;RME3;_RME3;white;14;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;rme3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;167;217.4037,1542.223;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-2813.565,-1008.955;Float;False;Constant;_Float6;Float 6;5;0;1.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-1857.862,2468.752;Float;False;RME;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1776.269,474.3281;Float;False;66;0;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;39;-3312.518,-843.8239;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2558.699,-958.1456;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;17;-2356.603,-1033.983;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;173;226.7999,1764.065;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;169;422.8587,1754.133;Float;True;Property;_TextureSample2;Texture Sample 2;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-127.815,-478.8031;Float;False;Constant;_Float13;Float 13;21;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;40;-2345.035,-902.2158;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.BreakToComponentsNode;71;-1576.677,477.0505;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;140;-2818.438,2292.148;Float;True;Spherical;World;True;N3;_N3;bump;13;None;Mid Texture 0;_MidTexture0;white;23;None;Bot Texture 0;_BotTexture0;white;24;None;n3;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;1.0;False;4;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;29;-2435.473,-661.6988;Float;False;Property;_Highlight_value;Highlight_value;10;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;172;553.6929,1988.049;Float;False;Constant;_Float14;Float 14;18;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-278.3486,286.4106;Float;False;Property;_Roughness_add;Roughness_add;4;0;0.15;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;28;-2065.466,-816.8553;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;90;87.39537,-521.2839;Float;True;Property;_Normal;Normal;1;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.GetLocalVarNode;69;171.1646,-618.3882;Float;False;63;0;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;771.7625,1781.826;Float;False;2;2;0;FLOAT4;0.0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-1847.413,2245.603;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;6;0.6514162,203.4106;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;61;-2187.519,2241.581;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;145;-2602.717,3023.292;Float;False;Constant;_Float12;Float 12;22;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;73;-1301.58,262.9671;Float;False;Property;_Emissive;Emissive;8;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;754.4026,1576.903;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT4;0;False;1;FLOAT4
Node;AmplifyShaderEditor.VertexColorNode;141;-2627.666,2767.771;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;27;-1851.062,-719.4846;Float;False;Property;_Highlight_platform;Highlight_platform;9;0;1;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;62;-2033.215,2242.74;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.BlendNormalsNode;91;455.6662,-556.3339;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;144;-2600.667,2946.803;Float;False;Constant;_Float11;Float 11;22;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;59;-2200.174,2053.168;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;60;-2029.128,2052.403;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1120.736,341.8653;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;64;-2202.644,2467.847;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;143;-2373.758,2944.487;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-1262.344,344.9939;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;146;-2432.373,3132.954;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;65;-2043.664,2465.889;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;79;143.4044,80.64252;Float;False;Constant;_Float0;Float 0;17;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;5;141.5772,169.2511;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;142;-2809.315,3144.449;Float;True;Property;_MaskMat;MaskMat;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1055.272,47.2298;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Tomb_dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Masked;0.5;True;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;182;0;153;0
WireConnection;182;1;183;0
WireConnection;182;2;184;0
WireConnection;182;3;185;0
WireConnection;182;4;186;0
WireConnection;154;0;153;0
WireConnection;151;0;154;0
WireConnection;155;3;156;0
WireConnection;155;4;132;0
WireConnection;138;3;87;0
WireConnection;138;4;132;0
WireConnection;180;0;177;1
WireConnection;180;1;182;0
WireConnection;74;0;138;0
WireConnection;74;1;76;0
WireConnection;23;0;8;3
WireConnection;179;0;180;0
WireConnection;152;0;151;0
WireConnection;152;1;155;1
WireConnection;75;0;138;0
WireConnection;75;1;74;0
WireConnection;75;2;46;2
WireConnection;22;0;23;0
WireConnection;22;1;15;0
WireConnection;22;2;16;0
WireConnection;178;0;152;0
WireConnection;178;1;179;0
WireConnection;130;0;138;0
WireConnection;130;1;78;0
WireConnection;35;0;30;2
WireConnection;35;1;33;0
WireConnection;35;2;32;0
WireConnection;165;0;178;0
WireConnection;165;2;174;0
WireConnection;165;3;175;0
WireConnection;165;4;176;0
WireConnection;77;0;75;0
WireConnection;77;1;130;0
WireConnection;77;2;46;1
WireConnection;10;0;22;0
WireConnection;10;1;11;0
WireConnection;37;0;35;0
WireConnection;37;1;34;0
WireConnection;12;0;10;0
WireConnection;12;1;13;0
WireConnection;166;0;165;0
WireConnection;44;0;77;0
WireConnection;14;0;12;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;18;0;45;0
WireConnection;18;1;26;0
WireConnection;139;3;87;0
WireConnection;139;4;132;0
WireConnection;167;0;166;0
WireConnection;66;0;139;0
WireConnection;39;0;38;0
WireConnection;39;1;33;0
WireConnection;39;2;32;0
WireConnection;21;0;45;0
WireConnection;21;1;20;0
WireConnection;17;0;45;0
WireConnection;17;1;18;0
WireConnection;17;2;14;0
WireConnection;173;0;167;0
WireConnection;169;1;173;0
WireConnection;40;0;17;0
WireConnection;40;1;21;0
WireConnection;40;2;39;0
WireConnection;71;0;70;0
WireConnection;140;3;87;0
WireConnection;140;4;132;0
WireConnection;28;0;45;0
WireConnection;28;1;40;0
WireConnection;28;2;29;0
WireConnection;90;5;92;0
WireConnection;171;0;169;0
WireConnection;171;1;172;0
WireConnection;63;0;140;0
WireConnection;6;0;71;0
WireConnection;6;1;7;0
WireConnection;61;2;143;0
WireConnection;170;0;167;0
WireConnection;170;1;171;0
WireConnection;27;0;45;0
WireConnection;27;1;28;0
WireConnection;62;0;61;0
WireConnection;62;1;140;0
WireConnection;62;2;141;1
WireConnection;91;0;69;0
WireConnection;91;1;90;0
WireConnection;59;2;143;0
WireConnection;60;0;59;0
WireConnection;60;1;138;0
WireConnection;60;2;141;1
WireConnection;72;0;80;0
WireConnection;72;1;73;0
WireConnection;64;2;143;0
WireConnection;143;0;144;0
WireConnection;143;1;145;0
WireConnection;143;2;141;2
WireConnection;80;0;46;3
WireConnection;146;0;144;0
WireConnection;146;1;145;0
WireConnection;146;2;142;2
WireConnection;65;0;64;0
WireConnection;65;1;139;0
WireConnection;65;2;141;1
WireConnection;5;0;6;0
WireConnection;0;0;27;0
WireConnection;0;1;91;0
WireConnection;0;2;170;0
WireConnection;0;3;79;0
WireConnection;0;4;5;0
WireConnection;0;10;178;0
ASEEND*/
//CHKSM=013E7D9FEFE4C0821D6200CD3166BDA31C87B021