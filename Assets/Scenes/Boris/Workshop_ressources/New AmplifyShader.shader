// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/test_terrain"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Base_color(" Base_color", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Tiling("Tiling", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha exclude_path:forward vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Normal;
		uniform float _Tiling;
		uniform sampler2D _Base_color;


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
			float3 normalWorld = UnityObjectToWorldNormal( v.normal );
			v.normal = normalize( normalWorld );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3(ase_worldTangent, ase_worldBitangent, ase_worldNormal);
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal10 = TriplanarNormal( _Normal, _Normal, _Normal, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float3 tanTriplanarNormal10 = mul( ase_worldToTangent, worldTriplanarNormal10 );
			o.Normal = tanTriplanarNormal10;
			float4 triplanar6 = TriplanarSampling( _Base_color, _Base_color, _Base_color, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			o.Albedo = triplanar6.xyz;
			o.Smoothness = 0.0;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
7;29;1906;1004;2712.387;497.4555;2;True;True
Node;AmplifyShaderEditor.RangedFloatNode;19;-843.687,378.5445;Float;False;Property;_Tiling;Tiling;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;4;-1146.478,538.0374;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;9;-853.0002,-112.8001;Float;True;Property;_Base_color; Base_color;2;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TexturePropertyNode;11;-877.8,107.2999;Float;True;Property;_Normal;Normal;2;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;20;-833.1872,453.2444;Float;False;Constant;_Float1;Float 1;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.AppendNode;14;-563.5872,721.4445;Float;False;FLOAT4;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.NormalizeNode;16;-351.7868,585.4446;Float;False;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.TriplanarNode;6;-599.2909,-88.5618;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-371.6905,356.9383;Float;False;Constant;_Float0;Float 0;0;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;13;-731.0872,725.4446;Float;False;3;0;FLOAT;0,0,0,0;False;1;FLOAT4;0.0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.AppendNode;12;-873.6869,732.7446;Float;False;FLOAT4;0;0;0;0;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;10;-607.0912,106.4381;Float;True;Spherical;World;True;Top Texture 1;_TopTexture1;bump;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1,0;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;Bobo/test_terrain;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;DeferredOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;4;1
WireConnection;14;1;13;0
WireConnection;16;0;4;0
WireConnection;6;0;9;0
WireConnection;6;3;19;0
WireConnection;6;4;20;0
WireConnection;13;0;4;1
WireConnection;13;1;12;0
WireConnection;12;0;4;2
WireConnection;12;1;4;3
WireConnection;10;0;11;0
WireConnection;10;3;19;0
WireConnection;10;4;20;0
WireConnection;0;0;6;0
WireConnection;0;1;10;0
WireConnection;0;4;2;0
WireConnection;0;12;16;0
ASEEND*/
//CHKSM=1B532C862D83120D8EFFC7F1637A2DFBEEB94141