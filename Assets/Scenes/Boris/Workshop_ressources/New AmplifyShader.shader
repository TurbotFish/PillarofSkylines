// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/test_terrain"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Base_color1(" Base_color1", 2D) = "white" {}
		_Base_color2(" Base_color2", 2D) = "white" {}
		_Base_color3(" Base_color3", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Normal2("Normal2", 2D) = "bump" {}
		_Normal3("Normal3", 2D) = "bump" {}
		_Tiling("Tiling", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Normal2;
		uniform float _Tiling;
		uniform sampler2D _Normal;
		uniform sampler2D _Normal3;
		uniform sampler2D _Base_color2;
		uniform sampler2D _Base_color1;
		uniform sampler2D _Base_color3;


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


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), c.y );
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
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 hsvTorgb62 = HSVToRGB( float3(v.color.r,1.0,1.0) );
			float3 lerpResult66 = lerp( ase_worldNormal , hsvTorgb62 , v.color.b);
			v.normal = lerpResult66;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 worldTriplanarNormal74 = TriplanarNormal( _Normal2, _Normal2, _Normal2, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float3 tanTriplanarNormal74 = mul( ase_worldToTangent, worldTriplanarNormal74 );
			float lerpResult75 = lerp( tanTriplanarNormal74.x , 0.0 , i.vertexColor.b);
			float4 appendResult73 = (float4(lerpResult75 , tanTriplanarNormal74.y , tanTriplanarNormal74.z , 0.0));
			float3 worldTriplanarNormal10 = TriplanarNormal( _Normal, _Normal, _Normal, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float3 tanTriplanarNormal10 = mul( ase_worldToTangent, worldTriplanarNormal10 );
			float lerpResult37 = lerp( tanTriplanarNormal10.x , 0.0 , i.vertexColor.b);
			float4 appendResult36 = (float4(lerpResult37 , tanTriplanarNormal10.y , tanTriplanarNormal10.z , 0.0));
			float3 hsvTorgb71 = HSVToRGB( float3(i.vertexColor.g,1.0,1.0) );
			float4 lerpResult77 = lerp( appendResult73 , appendResult36 , hsvTorgb71.x);
			float3 worldTriplanarNormal82 = TriplanarNormal( _Normal3, _Normal3, _Normal3, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float3 tanTriplanarNormal82 = mul( ase_worldToTangent, worldTriplanarNormal82 );
			float lerpResult83 = lerp( tanTriplanarNormal82.x , 0.0 , i.vertexColor.b);
			float4 appendResult84 = (float4(lerpResult83 , tanTriplanarNormal82.y , tanTriplanarNormal82.z , 0.0));
			float4 lerpResult85 = lerp( lerpResult77 , appendResult84 , hsvTorgb71.y);
			o.Normal = lerpResult85.xyz;
			float4 triplanar46 = TriplanarSampling( _Base_color2, _Base_color2, _Base_color2, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float4 triplanar6 = TriplanarSampling( _Base_color1, _Base_color1, _Base_color1, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float4 lerpResult68 = lerp( triplanar46 , triplanar6 , hsvTorgb71.x);
			float4 triplanar78 = TriplanarSampling( _Base_color3, _Base_color3, _Base_color3, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float4 lerpResult80 = lerp( lerpResult68 , triplanar78 , hsvTorgb71.y);
			o.Albedo = lerpResult80.xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
1927;29;1906;1004;1440.374;204.571;1.440172;True;True
Node;AmplifyShaderEditor.RangedFloatNode;19;-1718.062,-509.2039;Float;False;Property;_Tiling;Tiling;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;76;-1309.353,-224.0393;Float;True;Property;_Normal2;Normal2;4;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;20;-1712.839,-435.5039;Float;False;Constant;_Float1;Float 1;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1293.278,-9.702052;Float;True;Property;_Normal;Normal;3;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TriplanarNode;74;-1036.006,-224.9011;Float;True;Spherical;World;True;Top Texture 2;_TopTexture2;bump;-1;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;81;-1303.069,-434.1292;Float;True;Property;_Normal3;Normal3;5;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;31;-1711.533,-357.5913;Float;False;Constant;_Float2;Float 2;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;10;-1019.932,-10.56384;Float;True;Spherical;World;True;Top Texture 1;_TopTexture1;bump;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;64;-1396.278,983.6747;Float;False;Constant;_Float3;Float 3;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;25;-1429.014,809.5048;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;45;-1251.201,-1086.027;Float;True;Property;_Base_color2; Base_color2;1;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.LerpOp;75;-648.4466,-238.2534;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;82;-1029.722,-434.9909;Float;True;Spherical;World;True;Top Texture 3;_TopTexture3;bump;-1;None;Mid Texture 3;_MidTexture3;white;-1;None;Bot Texture 3;_BotTexture3;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.HSVToRGBNode;71;-1152.568,683.998;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;9;-1250.641,-873.384;Float;True;Property;_Base_color1; Base_color1;0;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.LerpOp;37;-636.0095,-16.64127;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;46;-965.7268,-1064.788;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;83;-642.1629,-448.3433;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;73;-481.355,-199.7224;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.TexturePropertyNode;79;-1244.671,-1297.49;Float;True;Property;_Base_color3; Base_color3;2;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TriplanarNode;6;-966.9326,-852.1457;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;36;-465.2806,14.61483;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.BreakToComponentsNode;72;-877.7773,531.136;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;68;-227.6983,-971.2177;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;78;-960.9626,-1276.251;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;77;-195.5408,-8.838344;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.WorldNormalVector;4;-1445.011,653.4825;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.HSVToRGBNode;62;-1150.95,839.4006;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;84;-475.0716,-409.8123;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;85;-0.4282352,-55.34087;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;80;-33.13239,-1032.132;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;66;-910.7482,842.6326;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;566.6511,1.201161;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/test_terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;74;0;76;0
WireConnection;74;3;19;0
WireConnection;74;4;20;0
WireConnection;10;0;11;0
WireConnection;10;3;19;0
WireConnection;10;4;20;0
WireConnection;75;0;74;1
WireConnection;75;1;31;0
WireConnection;75;2;25;3
WireConnection;82;0;81;0
WireConnection;82;3;19;0
WireConnection;82;4;20;0
WireConnection;71;0;25;2
WireConnection;71;1;64;0
WireConnection;71;2;64;0
WireConnection;37;0;10;1
WireConnection;37;1;31;0
WireConnection;37;2;25;3
WireConnection;46;0;45;0
WireConnection;46;3;19;0
WireConnection;46;4;20;0
WireConnection;83;0;82;1
WireConnection;83;1;31;0
WireConnection;83;2;25;3
WireConnection;73;0;75;0
WireConnection;73;1;74;2
WireConnection;73;2;74;3
WireConnection;6;0;9;0
WireConnection;6;3;19;0
WireConnection;6;4;20;0
WireConnection;36;0;37;0
WireConnection;36;1;10;2
WireConnection;36;2;10;3
WireConnection;72;0;71;0
WireConnection;68;0;46;0
WireConnection;68;1;6;0
WireConnection;68;2;72;0
WireConnection;78;0;79;0
WireConnection;78;3;19;0
WireConnection;78;4;20;0
WireConnection;77;0;73;0
WireConnection;77;1;36;0
WireConnection;77;2;72;0
WireConnection;62;0;25;1
WireConnection;62;1;64;0
WireConnection;62;2;64;0
WireConnection;84;0;83;0
WireConnection;84;1;82;2
WireConnection;84;2;82;3
WireConnection;85;0;77;0
WireConnection;85;1;84;0
WireConnection;85;2;72;1
WireConnection;80;0;68;0
WireConnection;80;1;78;0
WireConnection;80;2;72;1
WireConnection;66;0;4;0
WireConnection;66;1;62;0
WireConnection;66;2;25;3
WireConnection;0;0;80;0
WireConnection;0;1;85;0
WireConnection;0;12;66;0
ASEEND*/
//CHKSM=A7F5AFD9A2E20C97ECD0141041D08B648C1910F7