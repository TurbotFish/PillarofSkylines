// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Blockout"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ColorPlatform("ColorPlatform", Color) = (0,0.1724138,1,0)
		_ColorGrimpette("ColorGrimpette", Color) = (0,0.751724,1,0)
		_ColorGrimpe("ColorGrimpe", Color) = (0,1,0.006896496,0)
		_ColorGlisse("ColorGlisse", Color) = (1,0.6827586,0,0)
		_Platform_slope("Platform_slope", Range( 0 , 1)) = 0.95
		_Grimpe_slopeMax("Grimpe_slopeMax", Range( 0 , 1)) = 0.85
		_Grimpe_slopeLow("Grimpe_slopeLow", Range( 0 , 1)) = 0.8
		_Glisse_slopeMax("Glisse_slopeMax", Range( 0 , 1)) = 0.35
		_Glisse_slopeLow("Glisse_slopeLow", Range( 0 , 1)) = 0.3
		_ColorWall("ColorWall", Color) = (0.1102941,0.1102941,0.1102941,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
		};

		uniform float _Platform_slope;
		uniform float4 _ColorPlatform;
		uniform float _Grimpe_slopeMax;
		uniform float4 _ColorGrimpette;
		uniform float4 _ColorGrimpe;
		uniform float _Grimpe_slopeLow;
		uniform float _Glisse_slopeMax;
		uniform float4 _ColorGlisse;
		uniform float _Glisse_slopeLow;
		uniform float4 _ColorWall;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_cast_0 = (0.5).xxxx;
			float4 temp_cast_1 = (0.5).xxxx;
			float4 ifLocalVar65 = 0;
			if( i.worldNormal.y <= 0.05 )
				ifLocalVar65 = _ColorWall;
			else
				ifLocalVar65 = temp_cast_1;
			float4 ifLocalVar61 = 0;
			if( i.worldNormal.y >= _Glisse_slopeLow )
				ifLocalVar61 = _ColorGlisse;
			else
				ifLocalVar61 = ifLocalVar65;
			float4 ifLocalVar57 = 0;
			if( i.worldNormal.y > _Glisse_slopeMax )
				ifLocalVar57 = temp_cast_0;
			else if( i.worldNormal.y == _Glisse_slopeMax )
				ifLocalVar57 = _ColorGlisse;
			else if( i.worldNormal.y < _Glisse_slopeMax )
				ifLocalVar57 = ifLocalVar61;
			float4 ifLocalVar53 = 0;
			if( i.worldNormal.y >= _Grimpe_slopeLow )
				ifLocalVar53 = _ColorGrimpe;
			else
				ifLocalVar53 = ifLocalVar57;
			float4 ifLocalVar51 = 0;
			if( i.worldNormal.y > _Grimpe_slopeMax )
				ifLocalVar51 = _ColorGrimpette;
			else if( i.worldNormal.y == _Grimpe_slopeMax )
				ifLocalVar51 = _ColorGrimpe;
			else if( i.worldNormal.y < _Grimpe_slopeMax )
				ifLocalVar51 = ifLocalVar53;
			float4 ifLocalVar49 = 0;
			if( i.worldNormal.y >= _Platform_slope )
				ifLocalVar49 = _ColorPlatform;
			else
				ifLocalVar49 = ifLocalVar51;
			o.Albedo = ifLocalVar49.rgb;
			o.Emission = ( ifLocalVar49 * 0.5 ).rgb;
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
				float3 worldPos : TEXCOORD6;
				float3 worldNormal : TEXCOORD1;
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
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldNormal = IN.worldNormal;
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
1927;29;1906;1004;1799.727;960.6868;2.065779;True;True
Node;AmplifyShaderEditor.RangedFloatNode;60;-1657.76,86.56844;Float;False;Constant;_Float4;Float 4;0;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;68;-1730.901,-595.4716;Float;False;Property;_ColorWall;ColorWall;9;0;0.1102941,0.1102941,0.1102941,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;46;-1072.194,25.41755;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;66;-1613.509,962.2236;Float;False;Constant;_Float1;Float 1;9;0;0.05;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;59;-1449.098,-578.5964;Float;False;Property;_ColorGlisse;ColorGlisse;3;0;1,0.6827586,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;65;-1403.725,978.0727;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;62;-1512.245,724.707;Float;False;Property;_Glisse_slopeLow;Glisse_slopeLow;8;0;0.3;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;58;-1521.015,646.8319;Float;False;Property;_Glisse_slopeMax;Glisse_slopeMax;7;0;0.35;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;61;-1102.797,830.3959;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;54;-1387.412,411.4216;Float;False;Property;_Grimpe_slopeLow;Grimpe_slopeLow;6;0;0.8;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;57;-880.5195,668.0889;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;56;-1233.263,-585.0397;Float;False;Property;_ColorGrimpe;ColorGrimpe;2;0;0,1,0.006896496,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;55;-1017.429,-565.7112;Float;False;Property;_ColorGrimpette;ColorGrimpette;1;0;0,0.751724,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;53;-667.9065,447.4221;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;52;-1387.353,330.7611;Float;False;Property;_Grimpe_slopeMax;Grimpe_slopeMax;5;0;0.85;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;50;-1144.858,175.6559;Float;False;Property;_Platform_slope;Platform_slope;4;0;0.95;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;51;-464.9578,297.6267;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;48;-785.4882,-554.4362;Float;False;Property;_ColorPlatform;ColorPlatform;0;0;0,0.1724138,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ConditionalIfNode;49;-237.8482,152.6633;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;64;-176.2593,475.668;Float;False;Constant;_Float0;Float 0;9;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;47;-247.5126,-92.16388;Float;False;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;158.0237,321.3571;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;45;-775.8242,-345.0445;Float;False;Constant;_Color0;Color 0;0;0;1,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;420,65;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Blockout;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;65;0;46;2
WireConnection;65;1;66;0
WireConnection;65;2;60;0
WireConnection;65;3;68;0
WireConnection;65;4;68;0
WireConnection;61;0;46;2
WireConnection;61;1;62;0
WireConnection;61;2;59;0
WireConnection;61;3;59;0
WireConnection;61;4;65;0
WireConnection;57;0;46;2
WireConnection;57;1;58;0
WireConnection;57;2;60;0
WireConnection;57;3;59;0
WireConnection;57;4;61;0
WireConnection;53;0;46;2
WireConnection;53;1;54;0
WireConnection;53;2;56;0
WireConnection;53;3;56;0
WireConnection;53;4;57;0
WireConnection;51;0;46;2
WireConnection;51;1;52;0
WireConnection;51;2;55;0
WireConnection;51;3;56;0
WireConnection;51;4;53;0
WireConnection;49;0;46;2
WireConnection;49;1;50;0
WireConnection;49;2;48;0
WireConnection;49;3;48;0
WireConnection;49;4;51;0
WireConnection;47;0;48;0
WireConnection;47;1;45;0
WireConnection;47;2;46;2
WireConnection;63;0;49;0
WireConnection;63;1;64;0
WireConnection;0;0;49;0
WireConnection;0;2;63;0
ASEEND*/
//CHKSM=729EF4ECB0A679316B82398DB20A28D3FA9F0344