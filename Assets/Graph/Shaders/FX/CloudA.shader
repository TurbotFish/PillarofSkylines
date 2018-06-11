// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/VolCloud"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_FresnelBiais("FresnelBiais", Float) = 0
		_FresnelPower("FresnelPower", Float) = 0
		_FresnelScale("FresnelScale", Float) = 0
		_NoiseB_Intensity("NoiseB_Intensity", Float) = 0
		_NoiseA_Intensity("NoiseA_Intensity", Float) = 0
		_NoiseA("NoiseA", 2D) = "white" {}
		_NoiseB("NoiseB", 2D) = "white" {}
		_NoiseA_Speed("NoiseA_Speed", Vector) = (0,0.5,0,0)
		_NoiseB_Speed("NoiseB_Speed", Vector) = (0,0.5,0,0)
		_NoiseA_Tiling("NoiseA_Tiling", Float) = 1
		_NoiseB_Tiling("NoiseB_Tiling", Float) = 1
		_DepthDistance("DepthDistance", Float) = 10
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+10" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float _FresnelBiais;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform sampler2D _NoiseA;
		uniform float2 _NoiseA_Speed;
		uniform float _NoiseA_Tiling;
		uniform float _NoiseA_Intensity;
		uniform sampler2D _NoiseB;
		uniform float2 _NoiseB_Speed;
		uniform float _NoiseB_Tiling;
		uniform float _NoiseB_Intensity;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthDistance;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float2 appendResult53 = (float2(ase_vertex3Pos.y , ase_vertex3Pos.z));
			float2 panner79 = ( appendResult53 + _Time.y * _NoiseA_Speed);
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 temp_output_40_0 = abs( ase_worldNormal );
			float dotResult42 = dot( temp_output_40_0 , float3(1,1,1) );
			float3 BlendComponents44 = ( temp_output_40_0 / dotResult42 );
			float2 appendResult55 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.z));
			float2 panner80 = ( appendResult55 + _Time.y * _NoiseA_Speed);
			float2 appendResult54 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.y));
			float2 panner71 = ( appendResult54 + _Time.y * _NoiseA_Speed);
			float2 appendResult108 = (float2(ase_vertex3Pos.y , ase_vertex3Pos.z));
			float2 panner113 = ( appendResult108 + _Time.y * _NoiseB_Speed);
			float2 appendResult106 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.z));
			float2 panner114 = ( appendResult106 + _Time.y * _NoiseB_Speed);
			float2 appendResult107 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.y));
			float2 panner115 = ( appendResult107 + _Time.y * _NoiseB_Speed);
			float4 blendOpSrc138 = ( ( ( ( tex2Dlod( _NoiseA, float4( ( panner79 * _NoiseA_Tiling ), 0.0 , 0.0 ) ) * BlendComponents44.x ) + ( tex2Dlod( _NoiseA, float4( ( panner80 * _NoiseA_Tiling ), 0.0 , 0.0 ) ) * BlendComponents44.y ) ) + ( tex2Dlod( _NoiseA, float4( ( panner71 * _NoiseA_Tiling ), 0.0 , 0.0 ) ) * BlendComponents44.z ) ) * _NoiseA_Intensity );
			float4 blendOpDest138 = ( ( ( ( tex2Dlod( _NoiseB, float4( ( panner113 * _NoiseB_Tiling ), 0.0 , 0.0 ) ) * BlendComponents44.x ) + ( tex2Dlod( _NoiseB, float4( ( panner114 * _NoiseB_Tiling ), 0.0 , 0.0 ) ) * BlendComponents44.y ) ) + ( tex2Dlod( _NoiseB, float4( ( panner115 * _NoiseB_Tiling ), 0.0 , 0.0 ) ) * BlendComponents44.z ) ) * _NoiseB_Intensity );
			float3 ase_vertexNormal = v.normal.xyz;
			float4 temp_output_7_0 = ( 	max( blendOpSrc138, blendOpDest138 ) * float4( ase_vertexNormal , 0.0 ) );
			v.vertex.xyz += temp_output_7_0.rgb;
		}

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float temp_output_8_0 = 0.8;
			float3 temp_cast_0 = (temp_output_8_0).xxx;
			o.Emission = temp_cast_0;
			float3 ase_worldPos = i.worldPos;
			fixed3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNode9 = ( _FresnelBiais + _FresnelScale * pow( 1.0 - dot( ase_worldNormal, ase_worldViewDir ), _FresnelPower ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 appendResult53 = (float2(ase_vertex3Pos.y , ase_vertex3Pos.z));
			float2 panner79 = ( appendResult53 + _Time.y * _NoiseA_Speed);
			float3 temp_output_40_0 = abs( i.worldNormal );
			float dotResult42 = dot( temp_output_40_0 , float3(1,1,1) );
			float3 BlendComponents44 = ( temp_output_40_0 / dotResult42 );
			float2 appendResult55 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.z));
			float2 panner80 = ( appendResult55 + _Time.y * _NoiseA_Speed);
			float2 appendResult54 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.y));
			float2 panner71 = ( appendResult54 + _Time.y * _NoiseA_Speed);
			float2 appendResult108 = (float2(ase_vertex3Pos.y , ase_vertex3Pos.z));
			float2 panner113 = ( appendResult108 + _Time.y * _NoiseB_Speed);
			float2 appendResult106 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.z));
			float2 panner114 = ( appendResult106 + _Time.y * _NoiseB_Speed);
			float2 appendResult107 = (float2(ase_vertex3Pos.x , ase_vertex3Pos.y));
			float2 panner115 = ( appendResult107 + _Time.y * _NoiseB_Speed);
			float4 blendOpSrc138 = ( ( ( ( tex2D( _NoiseA, ( panner79 * _NoiseA_Tiling ) ) * BlendComponents44.x ) + ( tex2D( _NoiseA, ( panner80 * _NoiseA_Tiling ) ) * BlendComponents44.y ) ) + ( tex2D( _NoiseA, ( panner71 * _NoiseA_Tiling ) ) * BlendComponents44.z ) ) * _NoiseA_Intensity );
			float4 blendOpDest138 = ( ( ( ( tex2D( _NoiseB, ( panner113 * _NoiseB_Tiling ) ) * BlendComponents44.x ) + ( tex2D( _NoiseB, ( panner114 * _NoiseB_Tiling ) ) * BlendComponents44.y ) ) + ( tex2D( _NoiseB, ( panner115 * _NoiseB_Tiling ) ) * BlendComponents44.z ) ) * _NoiseB_Intensity );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float4 temp_output_7_0 = ( 	max( blendOpSrc138, blendOpDest138 ) * float4( ase_vertexNormal , 0.0 ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth140 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth140 = abs( ( screenDepth140 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthDistance ) );
			float3 temp_output_5_0_g1 = ( ( ase_worldPos - _WorldSpaceCameraPos ) / 20.0 );
			float dotResult8_g1 = dot( temp_output_5_0_g1 , temp_output_5_0_g1 );
			float clampResult10_g1 = clamp( dotResult8_g1 , 0.0 , 1.0 );
			o.Alpha = ( ( ( ( 1.0 - saturate( fresnelNode9 ) ) + temp_output_7_0 ) * saturate( distanceDepth140 ) ) * saturate( pow( clampResult10_g1 , 1.0 ) ) ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float3 worldPos : TEXCOORD6;
				float4 screenPos : TEXCOORD7;
				float3 worldNormal : TEXCOORD1;
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
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
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
-1905;31;1906;1004;2249.386;531.9409;1.539953;True;True
Node;AmplifyShaderEditor.WorldNormalVector;68;-6858.576,-52.10682;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector3Node;41;-6531.695,134.7429;Float;False;Constant;_Vector1;Vector 1;-1;0;1,1,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.AbsOpNode;40;-6498.577,-45.70683;Float;False;1;0;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.DotProductOpNode;42;-6324.677,20.69087;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;43;-6162.577,-45.70683;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-6002.577,-45.70683;Float;True;BlendComponents;1;False;1;0;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.PosVertexDataNode;52;-5346.577,82.29318;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;50;-5346.577,-461.7068;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;81;-5321.82,234.8725;Float;False;Constant;_Float1;Float 1;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;45;-5698.577,-205.7068;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;46;-5698.577,82.29318;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;98;-5353.143,1491.836;Float;False;Constant;_Float3;Float 3;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;99;-5377.9,795.2563;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;100;-5377.9,1339.257;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;101;-5377.9,1051.256;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;97;-5729.9,1339.257;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;96;-5729.9,1051.256;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;48;-5346.577,-205.7068;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;102;-5196.114,1207.212;Float;False;Property;_NoiseB_Speed;NoiseB_Speed;8;0;0,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;47;-5426.577,-253.7068;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;72;-5164.791,-49.75101;Float;False;Property;_NoiseA_Speed;NoiseA_Speed;7;0;0,0.5;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;83;-5168.498,240.6584;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;55;-5135.877,-175.5757;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;54;-5126.552,107.4502;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;77;-5437.766,296.2286;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;53;-5140.112,-403.49;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;105;-5457.9,1003.256;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;103;-5199.821,1497.622;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;108;-5171.435,853.4734;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;104;-5469.089,1553.193;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;106;-5167.2,1081.387;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.DynamicAppendNode;107;-5157.875,1364.414;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;78;-5383.804,310.8127;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;49;-5394.577,-285.7068;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;71;-4921.515,84.46457;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.BreakToComponentsNode;51;-5698.577,-61.70683;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;113;-4953.031,816.8054;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;79;-4921.708,-440.1577;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;110;-5425.9,971.2563;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;87;-5166.607,326.3476;Float;False;Property;_NoiseA_Tiling;NoiseA_Tiling;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;80;-4920.886,-164.8296;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;114;-4952.209,1092.133;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.BreakToComponentsNode;111;-5729.9,1195.256;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;112;-5415.127,1567.777;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;109;-5197.93,1583.312;Float;False;Property;_NoiseB_Tiling;NoiseB_Tiling;10;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;115;-4952.838,1341.429;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;57;-4209.844,-17.41556;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-4684.389,-163.7578;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;70;-4746.458,-743.6903;Float;True;Property;_NoiseA;NoiseA;5;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-4678.755,-439.7944;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-4693.64,93.68478;Float;False;2;2;0;FLOAT2;0.0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;75;-4198.963,304.8289;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;56;-4209.844,-289.4155;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;118;-4241.166,1239.547;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-4715.712,1093.205;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;119;-4230.286,1561.792;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-4710.079,817.1694;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-4724.963,1350.649;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.WireNode;116;-4241.166,967.5474;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;120;-4777.781,513.2736;Float;True;Property;_NoiseB;NoiseB;6;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.WireNode;76;-4159.585,274.202;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;58;-4467.625,-222.8538;Float;True;Property;_TextureSample4;Texture Sample 4;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;69;-4464.84,55.08321;Float;True;Property;_TextureSample8;Texture Sample 8;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;60;-4462.543,-478.3159;Float;True;Property;_TextureSample6;Texture Sample 6;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;61;-4177.844,-321.4155;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;59;-4177.844,-49.41557;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;125;-4209.167,935.5475;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;123;-4209.167,1207.547;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;128;-4493.866,778.6475;Float;True;Property;_TextureSample8;Texture Sample 8;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;126;-4496.164,1312.047;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WireNode;124;-4190.908,1531.166;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;127;-4498.948,1034.109;Float;True;Property;_TextureSample4;Texture Sample 4;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-4097.844,-497.4155;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-4097.844,14.58443;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-4097.844,-225.4155;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-4129.167,1271.548;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-4129.167,759.5474;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;-4129.167,1031.547;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-3857.844,-385.4155;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;65;-3841.844,-33.41556;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;133;-3889.167,871.5474;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.WireNode;132;-3873.167,1223.547;Float;False;1;0;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-3601.844,-129.4155;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;18;-3362.417,-141.1423;Float;False;Property;_NoiseA_Intensity;NoiseA_Intensity;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;137;-3348.397,1015.682;Float;False;Property;_NoiseB_Intensity;NoiseB_Intensity;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-3633.167,1127.547;Float;True;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;16;-1955.556,179.2192;Float;False;Property;_FresnelBiais;FresnelBiais;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;14;-1948.556,255.2193;Float;False;Property;_FresnelScale;FresnelScale;2;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;15;-1948.556,333.2192;Float;False;Property;_FresnelPower;FresnelPower;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-3194.886,1165.538;Float;False;2;2;0;COLOR;0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-3208.906,8.713577;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.FresnelNode;9;-1732.556,222.2193;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;144;-1032.546,565.901;Float;False;Property;_DepthDistance;DepthDistance;11;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;13;-1491.556,225.2192;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;4;-1698.556,692.2192;Float;True;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BlendOpsNode;138;-2879.242,602.8931;Float;False;Lighten;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;162;-931.1866,342.7524;Float;False;Constant;_Float5;Float 5;12;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;161;-926.5668,251.8952;Float;False;Constant;_Float4;Float 4;12;0;20;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceCameraPos;160;-1051.303,144.0985;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;11;-1350.556,223.2193;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1447.556,617.2192;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT3;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.DepthFade;140;-852.7206,568.9689;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;143;-660.1127,568.1075;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.FunctionNode;159;-660.1549,162.5779;Float;False;SphereMask;-1;;1;3;0;FLOAT3;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-1246.556,495.2192;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SaturateNode;164;-338.3048,165.6578;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-495.9423,486.8688;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;22;-499.2023,6.992371;Float;False;Constant;_Float2;Float 2;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-324.2023,-41.00763;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-358.2253,-215.9484;Float;False;Constant;_Float0;Float 0;2;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;-31.8542,164.1179;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-115.8887,-196.998;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;148,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Bobo/VolCloud;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;10;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;1;255;255;7;3;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;68;0
WireConnection;42;0;40;0
WireConnection;42;1;41;0
WireConnection;43;0;40;0
WireConnection;43;1;42;0
WireConnection;44;0;43;0
WireConnection;45;0;44;0
WireConnection;46;0;44;0
WireConnection;97;0;44;0
WireConnection;96;0;44;0
WireConnection;47;0;45;0
WireConnection;83;0;81;0
WireConnection;55;0;48;1
WireConnection;55;1;48;3
WireConnection;54;0;52;1
WireConnection;54;1;52;2
WireConnection;77;0;46;2
WireConnection;53;0;50;2
WireConnection;53;1;50;3
WireConnection;105;0;96;0
WireConnection;103;0;98;0
WireConnection;108;0;99;2
WireConnection;108;1;99;3
WireConnection;104;0;97;2
WireConnection;106;0;101;1
WireConnection;106;1;101;3
WireConnection;107;0;100;1
WireConnection;107;1;100;2
WireConnection;78;0;77;0
WireConnection;49;0;47;0
WireConnection;71;0;54;0
WireConnection;71;2;72;0
WireConnection;71;1;83;0
WireConnection;51;0;44;0
WireConnection;113;0;108;0
WireConnection;113;2;102;0
WireConnection;113;1;103;0
WireConnection;79;0;53;0
WireConnection;79;2;72;0
WireConnection;79;1;83;0
WireConnection;110;0;105;0
WireConnection;80;0;55;0
WireConnection;80;2;72;0
WireConnection;80;1;83;0
WireConnection;114;0;106;0
WireConnection;114;2;102;0
WireConnection;114;1;103;0
WireConnection;111;0;44;0
WireConnection;112;0;104;0
WireConnection;115;0;107;0
WireConnection;115;2;102;0
WireConnection;115;1;103;0
WireConnection;57;0;51;1
WireConnection;86;0;80;0
WireConnection;86;1;87;0
WireConnection;85;0;79;0
WireConnection;85;1;87;0
WireConnection;84;0;71;0
WireConnection;84;1;87;0
WireConnection;75;0;78;0
WireConnection;56;0;49;0
WireConnection;118;0;111;1
WireConnection;122;0;114;0
WireConnection;122;1;109;0
WireConnection;119;0;112;0
WireConnection;117;0;113;0
WireConnection;117;1;109;0
WireConnection;121;0;115;0
WireConnection;121;1;109;0
WireConnection;116;0;110;0
WireConnection;76;0;75;0
WireConnection;58;0;70;0
WireConnection;58;1;86;0
WireConnection;69;0;70;0
WireConnection;69;1;84;0
WireConnection;60;0;70;0
WireConnection;60;1;85;0
WireConnection;61;0;56;0
WireConnection;59;0;57;0
WireConnection;125;0;116;0
WireConnection;123;0;118;0
WireConnection;128;0;120;0
WireConnection;128;1;117;0
WireConnection;126;0;120;0
WireConnection;126;1;121;0
WireConnection;124;0;119;0
WireConnection;127;0;120;0
WireConnection;127;1;122;0
WireConnection;62;0;60;0
WireConnection;62;1;61;0
WireConnection;63;0;69;0
WireConnection;63;1;76;0
WireConnection;64;0;58;0
WireConnection;64;1;59;0
WireConnection;129;0;126;0
WireConnection;129;1;124;0
WireConnection;130;0;128;0
WireConnection;130;1;125;0
WireConnection;131;0;127;0
WireConnection;131;1;123;0
WireConnection;66;0;62;0
WireConnection;66;1;64;0
WireConnection;65;0;63;0
WireConnection;133;0;130;0
WireConnection;133;1;131;0
WireConnection;132;0;129;0
WireConnection;67;0;66;0
WireConnection;67;1;65;0
WireConnection;134;0;133;0
WireConnection;134;1;132;0
WireConnection;136;0;134;0
WireConnection;136;1;137;0
WireConnection;17;0;67;0
WireConnection;17;1;18;0
WireConnection;9;1;16;0
WireConnection;9;2;14;0
WireConnection;9;3;15;0
WireConnection;13;0;9;0
WireConnection;138;0;17;0
WireConnection;138;1;136;0
WireConnection;11;0;13;0
WireConnection;7;0;138;0
WireConnection;7;1;4;0
WireConnection;140;0;144;0
WireConnection;143;0;140;0
WireConnection;159;0;160;0
WireConnection;159;1;161;0
WireConnection;159;2;162;0
WireConnection;10;0;11;0
WireConnection;10;1;7;0
WireConnection;164;0;159;0
WireConnection;141;0;10;0
WireConnection;141;1;143;0
WireConnection;21;0;9;0
WireConnection;21;1;22;0
WireConnection;163;0;141;0
WireConnection;163;1;164;0
WireConnection;19;0;8;0
WireConnection;19;1;21;0
WireConnection;0;2;8;0
WireConnection;0;9;163;0
WireConnection;0;11;7;0
ASEEND*/
//CHKSM=80C0237629D42A7181963FED4F8D2A4BAB315619