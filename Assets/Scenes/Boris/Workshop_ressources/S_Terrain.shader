// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/test_terrain"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Base_color2(" Base_color2", 2D) = "white" {}
		_Normal2("Normal2", 2D) = "bump" {}
		_RME2("RME2", 2D) = "white" {}
		_Base_color3(" Base_color3", 2D) = "white" {}
		_Normal3("Normal3", 2D) = "bump" {}
		_RME3("RME3", 2D) = "white" {}
		_Tiling("Tiling", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Normal2;
		uniform float _Tiling;
		uniform sampler2D _Normal3;
		uniform sampler2D _Base_color2;
		uniform sampler2D _Base_color3;
		uniform sampler2D _RME2;
		uniform sampler2D _RME3;


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
			float3 worldTriplanarNormal74 = TriplanarNormal( _Normal2, _Normal2, _Normal2, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float3 tanTriplanarNormal74 = mul( ase_worldToTangent, worldTriplanarNormal74 );
			float lerpResult75 = lerp( tanTriplanarNormal74.x , 0.0 , i.vertexColor.g);
			float4 appendResult73 = (float4(lerpResult75 , tanTriplanarNormal74.y , tanTriplanarNormal74.z , 0.0));
			float3 worldTriplanarNormal82 = TriplanarNormal( _Normal3, _Normal3, _Normal3, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float3 tanTriplanarNormal82 = mul( ase_worldToTangent, worldTriplanarNormal82 );
			float3 lerpResult160 = lerp( float3(0,0,1) , tanTriplanarNormal82 , 0.5);
			float4 lerpResult85 = lerp( appendResult73 , float4( lerpResult160 , 0.0 ) , i.vertexColor.g);
			o.Normal = lerpResult85.xyz;
			float4 triplanar46 = TriplanarSampling( _Base_color2, _Base_color2, _Base_color2, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float4 triplanar78 = TriplanarSampling( _Base_color3, _Base_color3, _Base_color3, ase_worldPos, ase_worldNormal, 1.0, _Tiling, 0 );
			float clampResult172 = clamp( ase_worldNormal.y , 0.0 , 1.0 );
			float clampResult177 = clamp( ( pow( clampResult172 , 4.0 ) * 3.0 ) , 0.0 , 1.0 );
			float4 lerpResult178 = lerp( triplanar78 , ( triplanar78 * 1.4 ) , clampResult177);
			float4 lerpResult80 = lerp( triplanar46 , lerpResult178 , i.vertexColor.g);
			o.Albedo = lerpResult80.xyz;
			float4 triplanar142 = TriplanarSampling( _RME2, _RME2, _RME2, ase_worldPos, ase_worldNormal, 0.0, _Tiling, 0 );
			float4 triplanar143 = TriplanarSampling( _RME3, _RME3, _RME3, ase_worldPos, ase_worldNormal, 0.0, _Tiling, 0 );
			float4 lerpResult151 = lerp( triplanar142 , triplanar143 , i.vertexColor.g);
			o.Metallic = lerpResult151.y;
			o.Smoothness = ( 1.0 - ( lerpResult151.x + 0.1 ) );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
7;29;1906;1004;1048.323;987.4681;1.67827;True;True
Node;AmplifyShaderEditor.RangedFloatNode;171;-1181.144,-1837.092;Float;False;Constant;_Float8;Float 8;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;169;-1179.844,-1760.392;Float;False;Constant;_Float5;Float 5;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;170;-1868.922,-2084.181;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;140;-1363.113,293.6673;Float;True;Property;_RME3;RME3;5;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.RangedFloatNode;173;-1552.942,-1896.893;Float;False;Constant;_Float10;Float 10;5;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;172;-1628.343,-2053.193;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;19;-1714.054,-529.9269;Float;False;Property;_Tiling;Tiling;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;139;-1369.397,503.7573;Float;True;Property;_RME2;RME2;2;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TriplanarNode;143;-1089.766,292.8056;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;bump;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;-1712.839,-435.5039;Float;False;Constant;_Float1;Float 1;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;25;-2055.221,180.9636;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;174;-1360.542,-1830.592;Float;False;Constant;_Float12;Float 12;5;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;79;-1244.671,-1297.49;Float;True;Property;_Base_color3; Base_color3;3;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.PowerNode;175;-1364.442,-1985.293;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;142;-1096.05,502.8955;Float;True;Spherical;World;False;Top Texture 4;_TopTexture4;bump;-1;None;Mid Texture 4;_MidTexture4;white;-1;None;Bot Texture 4;_BotTexture4;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;76;-1309.353,-224.0393;Float;True;Property;_Normal2;Normal2;1;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-1185.044,-1952.793;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;78;-960.9626,-1276.251;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;159;-554.5247,-1137.309;Float;False;Constant;_Float0;Float 0;7;0;1.4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;31;-1708.948,-339.4997;Float;False;Constant;_Float2;Float 2;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;81;-1303.069,-434.1292;Float;True;Property;_Normal3;Normal3;4;0;None;True;bump;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TriplanarNode;74;-1036.006,-224.9011;Float;True;Spherical;World;True;Top Texture 2;_TopTexture2;bump;-1;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;151;-269.9068,359.822;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ClampOpNode;177;-1022.544,-1955.393;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-395.3322,-1261.047;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TexturePropertyNode;45;-1251.201,-1086.027;Float;True;Property;_Base_color2; Base_color2;0;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.Vector3Node;168;-705.3892,-667.0881;Float;False;Constant;_Vector0;Vector 0;7;0;0,0,1;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;152;-88.95718,366.1107;Float;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;75;-648.4466,-238.2534;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TriplanarNode;82;-1029.722,-434.9909;Float;True;Spherical;World;True;Top Texture 3;_TopTexture3;bump;-1;None;Mid Texture 3;_MidTexture3;white;-1;None;Bot Texture 3;_BotTexture3;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;156;-14.29468,521.1464;Float;False;Constant;_Float6;Float 6;11;0;0.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;161;-704.1001,-496.4246;Float;False;Constant;_Float4;Float 4;7;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;73;-481.355,-199.7224;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;160;-494.0191,-629.3071;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;154;206.0245,376.9929;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;178;-410.622,-995.2516;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TriplanarNode;46;-965.7268,-1064.788;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;5;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;SAMPLER2D;;False;3;FLOAT;0.02;False;4;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;80;-96.87323,-1092.838;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;85;-91.48658,-380.1156;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.OneMinusNode;153;349.6137,387.5502;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;986.8106,7.634031;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/test_terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;172;0;170;2
WireConnection;172;1;171;0
WireConnection;172;2;169;0
WireConnection;143;0;140;0
WireConnection;143;3;19;0
WireConnection;175;0;172;0
WireConnection;175;1;173;0
WireConnection;142;0;139;0
WireConnection;142;3;19;0
WireConnection;176;0;175;0
WireConnection;176;1;174;0
WireConnection;78;0;79;0
WireConnection;78;3;19;0
WireConnection;78;4;20;0
WireConnection;74;0;76;0
WireConnection;74;3;19;0
WireConnection;74;4;20;0
WireConnection;151;0;142;0
WireConnection;151;1;143;0
WireConnection;151;2;25;2
WireConnection;177;0;176;0
WireConnection;177;1;171;0
WireConnection;177;2;169;0
WireConnection;158;0;78;0
WireConnection;158;1;159;0
WireConnection;152;0;151;0
WireConnection;75;0;74;1
WireConnection;75;1;31;0
WireConnection;75;2;25;2
WireConnection;82;0;81;0
WireConnection;82;3;19;0
WireConnection;82;4;20;0
WireConnection;73;0;75;0
WireConnection;73;1;74;2
WireConnection;73;2;74;3
WireConnection;160;0;168;0
WireConnection;160;1;82;0
WireConnection;160;2;161;0
WireConnection;154;0;152;0
WireConnection;154;1;156;0
WireConnection;178;0;78;0
WireConnection;178;1;158;0
WireConnection;178;2;177;0
WireConnection;46;0;45;0
WireConnection;46;3;19;0
WireConnection;46;4;20;0
WireConnection;80;0;46;0
WireConnection;80;1;178;0
WireConnection;80;2;25;2
WireConnection;85;0;73;0
WireConnection;85;1;160;0
WireConnection;85;2;25;2
WireConnection;153;0;154;0
WireConnection;0;0;80;0
WireConnection;0;1;85;0
WireConnection;0;3;152;1
WireConnection;0;4;153;0
ASEEND*/
//CHKSM=871ED06BFEA1BB8B2DF0B148C511A6B29DF66812