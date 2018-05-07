// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Crystalthing"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ThingDiffuse("ThingDiffuse", 2D) = "white" {}
		_ThingNormal("ThingNormal", 2D) = "white" {}
		_Crystal_noise("Crystal_noise", 2D) = "white" {}
		_Fking_Noise("Fking_Noise", 2D) = "white" {}
		_Cracks_Normal("Cracks_Normal", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,0.4352941,0,1)
		[HDR]_Color2("Color 2", Color) = (1,0.4352941,0,1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
		};

		uniform sampler2D _ThingNormal;
		uniform sampler2D _Cracks_Normal;
		uniform sampler2D _ThingDiffuse;
		uniform sampler2D _Crystal_noise;
		uniform float4 _Color2;
		uniform float4 _Color0;
		uniform sampler2D _Fking_Noise;


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
			float3 worldTriplanarNormal128 = TriplanarNormal( _ThingNormal, _ThingNormal, _ThingNormal, ase_worldPos, ase_worldNormal, 3.0, 0.05, 0 );
			float3 tanTriplanarNormal128 = mul( ase_worldToTangent, worldTriplanarNormal128 );
			float3 worldTriplanarNormal144 = TriplanarNormal( _Cracks_Normal, _Cracks_Normal, _Cracks_Normal, ase_worldPos, ase_worldNormal, 3.0, 0.05, 0 );
			float3 tanTriplanarNormal144 = mul( ase_worldToTangent, worldTriplanarNormal144 );
			o.Normal = BlendNormals( tanTriplanarNormal128 , tanTriplanarNormal144 );
			float4 triplanar125 = TriplanarSampling( _ThingDiffuse, _ThingDiffuse, _ThingDiffuse, ase_worldPos, ase_worldNormal, 3.0, 0.05, 0 );
			float4 triplanar129 = TriplanarSampling( _Crystal_noise, _Crystal_noise, _Crystal_noise, ase_worldPos, ase_worldNormal, 3.0, 0.05, 0 );
			float4 lerpResult146 = lerp( float4(0,0.1215686,0.2279412,0) , triplanar125 , triplanar129.x);
			o.Albedo = lerpResult146.xyz;
			float4 triplanar148 = TriplanarSampling( _Fking_Noise, _Fking_Noise, _Fking_Noise, ase_worldPos, ase_worldNormal, 3.0, 0.05, 0 );
			float4 lerpResult149 = lerp( _Color2 , _Color0 , triplanar148.x);
			float4 temp_cast_2 = (0.0).xxxx;
			float4 lerpResult137 = lerp( lerpResult149 , temp_cast_2 , triplanar129.x);
			o.Emission = lerpResult137.rgb;
			float lerpResult130 = lerp( 0.8 , 0.0 , triplanar129.x);
			o.Metallic = lerpResult130;
			float lerpResult135 = lerp( 0.5 , 0.0 , triplanar129.x);
			o.Smoothness = lerpResult135;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
-1913;29;1906;1004;754.3494;-27.694;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;126;25.75739,-27.01803;Float;False;Constant;_Float0;Float 0;1;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;139;-126.681,422.4692;Float;False;Constant;_Float7;Float 7;4;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;127;22.7574,-126.018;Float;False;Constant;_Float1;Float 1;1;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;150;520.067,-400.9719;Float;False;Property;_Color2;Color 2;6;1;[HDR];1,0.4352941,0,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;136;310.8861,-404.8346;Float;False;Property;_Color0;Color 0;5;1;[HDR];1,0.4352941,0,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;129;48.26758,394.8489;Float;True;Spherical;World;False;Crystal_noise;_Crystal_noise;white;2;Assets/Graph/Textures/BAText/Master/T_ConcreteB_C.png;Mid Texture 3;_MidTexture3;white;1;None;Bot Texture 3;_BotTexture3;white;2;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;148;828.8463,548.3014;Float;True;Spherical;World;False;Fking_Noise;_Fking_Noise;white;3;Assets/Graph/Textures/BAText/Master/T_ConcreteB_C.png;Mid Texture 4;_MidTexture4;white;1;None;Bot Texture 4;_BotTexture4;white;2;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;147;688.0461,-224.4986;Float;False;Constant;_Color1;Color 1;5;0;0,0.1215686,0.2279412,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;128;218.0676,46.84894;Float;True;Spherical;World;True;ThingNormal;_ThingNormal;white;1;Assets/Graph/Textures/BAText/Master/T_ConcreteB_C.png;Mid Texture 2;_MidTexture2;white;1;None;Bot Texture 2;_BotTexture2;white;2;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;131;701.886,18.56546;Float;False;Constant;_Float2;Float 2;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;144;40.76475,586.1133;Float;True;Spherical;World;True;Cracks_Normal;_Cracks_Normal;white;4;Assets/Graph/Textures/BAText/Master/T_ConcreteB_C.png;Mid Texture 0;_MidTexture0;white;1;None;Bot Texture 0;_BotTexture0;white;2;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;138;749.0861,-494.8346;Float;False;Constant;_Float6;Float 6;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;125;209.7574,-144.018;Float;True;Spherical;World;False;ThingDiffuse;_ThingDiffuse;white;0;Assets/Graph/Textures/BAText/Master/T_ConcreteB_C.png;Mid Texture 0;_MidTexture0;white;1;None;Bot Texture 0;_BotTexture0;white;2;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;700.886,97.56546;Float;False;Constant;_Float3;Float 3;3;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RelayNode;140;630.319,402.4692;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;149;750.4672,-362.5719;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;134;706.6922,181.9377;Float;False;Constant;_Float5;Float 5;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;133;705.6922,260.9377;Float;False;Constant;_Float4;Float 4;3;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BlendNormalsNode;145;460.8461,586.7013;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;137;945.0861,-366.8346;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;146;950.4461,-154.0986;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;130;895.4675,79.44888;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;135;900.2737,242.8211;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1268.836,-51.31799;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Bobo/Crystalthing;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;1;5;5;10;True;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;129;3;139;0
WireConnection;129;4;126;0
WireConnection;148;3;139;0
WireConnection;148;4;126;0
WireConnection;128;3;127;0
WireConnection;128;4;126;0
WireConnection;144;3;139;0
WireConnection;144;4;126;0
WireConnection;125;3;127;0
WireConnection;125;4;126;0
WireConnection;140;0;129;1
WireConnection;149;0;150;0
WireConnection;149;1;136;0
WireConnection;149;2;148;1
WireConnection;145;0;128;0
WireConnection;145;1;144;0
WireConnection;137;0;149;0
WireConnection;137;1;138;0
WireConnection;137;2;140;0
WireConnection;146;0;147;0
WireConnection;146;1;125;0
WireConnection;146;2;140;0
WireConnection;130;0;132;0
WireConnection;130;1;131;0
WireConnection;130;2;140;0
WireConnection;135;0;133;0
WireConnection;135;1;134;0
WireConnection;135;2;140;0
WireConnection;0;0;146;0
WireConnection;0;1;145;0
WireConnection;0;2;137;0
WireConnection;0;3;130;0
WireConnection;0;4;135;0
ASEEND*/
//CHKSM=021939403420ECDBB5A912DFDFA5FEF94365F500