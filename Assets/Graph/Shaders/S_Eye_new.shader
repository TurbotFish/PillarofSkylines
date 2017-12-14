// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Eye"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Normal("Normal", 2D) = "white" {}
		_Normal_Tiling("Normal_Tiling", Float) = 1
		_Noise_tiling("Noise_tiling", Float) = 0.2
		_Noise_panner("Noise_panner", Vector) = (0.2,0,0,0)
		_Noise_intensity("Noise_intensity", Range( 0 , 1)) = 0
		_VO_intensity("VO_intensity", Float) = 0
		_MaskA("MaskA", 2D) = "white" {}
		_MaskB("MaskB", 2D) = "white" {}
		_Eye_C("Eye_C", 2D) = "white" {}
		_Fresnel_color("Fresnel_color", Vector) = (1,0,0,0)
		_Hue("Hue", Float) = 0
		_Pupille_Emissive("Pupille_Emissive", Float) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow exclude_path:forward vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
			float2 texcoord_2;
			float2 texcoord_3;
			float2 texcoord_4;
			float3 worldPos;
			float3 worldNormal;
			float2 texcoord_5;
			float2 texcoord_6;
		};

		uniform sampler2D _Eye_C;
		uniform sampler2D _MaskB;
		uniform sampler2D _MaskA;
		uniform float _Hue;
		uniform sampler2D _Normal;
		uniform float2 _Noise_panner;
		uniform float _Noise_tiling;
		uniform float _Noise_intensity;
		uniform float _Normal_Tiling;
		uniform float _Pupille_Emissive;
		uniform float3 _Fresnel_color;
		uniform float _VO_intensity;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_3.xy = v.texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
			o.texcoord_4.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			float3 temp_cast_0 = (0.0).xxx;
			o.texcoord_5.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			float2 panner10 = ( ( o.texcoord_5 * _Noise_tiling ) + 1.0 * _Time.y * _Noise_panner);
			float4 tex2DNode6 = tex2Dlod( _Eye_C, float4( panner10, 0.0 , 0.0 ) );
			float3 ase_vertexNormal = v.normal.xyz;
			o.texcoord_6.xy = v.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
			float4 tex2DNode20 = tex2Dlod( _MaskA, float4( o.texcoord_6, 0.0 , 0.0 ) );
			float3 lerpResult21 = lerp( temp_cast_0 , ( ( tex2DNode6.g * ase_vertexNormal ) * _VO_intensity ) , tex2DNode20.g);
			v.vertex.xyz += lerpResult21;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 tex2DNode23 = tex2D( _Eye_C, i.texcoord_0 );
			float2 panner50 = ( ( i.texcoord_1 * 2.0 ) + 1.0 * _Time.y * float2( 0.5,0 ));
			float3 lerpResult59 = lerp( float3( 0,0,0 ) , float3(2,0,0) , ( pow( tex2D( _MaskB, panner50 ).g , 5.0 ) * 5.0 ));
			float4 tex2DNode20 = tex2D( _MaskA, i.texcoord_2 );
			float3 lerpResult54 = lerp( float3( 0,0,0 ) , lerpResult59 , tex2DNode20.b);
			float2 panner86 = ( ( i.texcoord_3 * float2( 1,4 ) ) + 1.0 * _Time.y * float2( 1,0 ));
			float lerpResult84 = lerp( 0.0 , pow( tex2D( _MaskB, panner86 ).g , 1.0 ) , tex2D( _MaskB, i.texcoord_3 ).r);
			float temp_output_94_0 = ( 1.0 - ( lerpResult84 * 0.5 ) );
			float3 hsvTorgb121 = RGBToHSV( ( ( tex2DNode23.r + lerpResult54 ) * temp_output_94_0 ) );
			float3 hsvTorgb122 = HSVToRGB( float3(( hsvTorgb121 + _Hue ).x,hsvTorgb121.y,hsvTorgb121.z) );
			o.Albedo = hsvTorgb122;
			float3 temp_cast_1 = (0.0).xxx;
			float3 temp_cast_2 = (0.0).xxx;
			float2 panner10 = ( ( i.texcoord_4 * _Noise_tiling ) + 1.0 * _Time.y * _Noise_panner);
			float4 tex2DNode6 = tex2D( _Eye_C, panner10 );
			float clampResult44 = clamp( pow( UnpackScaleNormal( tex2D( _Normal, ( ( i.texcoord_2 + ( tex2DNode6.g * _Noise_intensity ) ) * _Normal_Tiling ) ) ,1.0 ).r , 5.0 ) , 0.0 , 1.0 );
			float3 lerpResult38 = lerp( temp_cast_2 , float3(3,0,0) , clampResult44);
			float3 lerpResult48 = lerp( temp_cast_1 , lerpResult38 , tex2DNode20.b);
			float mulTime111 = _Time.y * 3.0;
			float clampResult77 = clamp( ( ( tex2DNode23.b * (0.5 + (sin( mulTime111 ) - -1.0) * (1.0 - 0.5) / (1.0 - -1.0)) ) * _Pupille_Emissive ) , 0.0 , 1.0 );
			float3 lerpResult71 = lerp( lerpResult48 , float3(1,0,0) , clampResult77);
			float3 ase_worldPos = i.worldPos;
			fixed3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNode25 = ( 0.0 + 1.0 * pow( 1.0 - dot( ase_worldNormal, ase_worldViewDir ), 5.0 ) );
			float3 lerpResult104 = lerp( lerpResult71 , _Fresnel_color , fresnelNode25);
			float3 hsvTorgb120 = RGBToHSV( ( lerpResult104 * temp_output_94_0 ) );
			float3 hsvTorgb119 = HSVToRGB( float3(( hsvTorgb120 + _Hue ).x,hsvTorgb120.y,hsvTorgb120.z) );
			o.Emission = hsvTorgb119;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-6;104;1906;1004;1185.435;695.7726;1.847641;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1829,-56;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-1756,57;Float;False;Property;_Noise_tiling;Noise_tiling;2;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1577,-55;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;11;-1442,22;Float;False;Property;_Noise_panner;Noise_panner;3;0;0.2,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;93;-421.8305,-1063.347;Float;True;Property;_Eye_C;Eye_C;8;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.PannerNode;10;-1257,-51;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;6;-1039,-71;Float;True;Property;_Noise;Noise;3;0;Assets/Scenes/Boris/Workshop_ressources/Perlin_noise.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;41;-1029.113,-175.1157;Float;False;Property;_Noise_intensity;Noise_intensity;4;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1037,126;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-657.1125,-55.11572;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;-651.7183,-378.2838;Float;False;Constant;_Float6;Float 6;10;0;2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-730.7311,-514.1406;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-749,125;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;14;-759,242;Float;False;Property;_Normal_Tiling;Normal_Tiling;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;112;185.153,399.4999;Float;False;Constant;_Float14;Float 14;11;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-586,44;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.Vector2Node;90;-370.6377,-1874.795;Float;False;Constant;_Vector6;Vector 6;12;0;1,4;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;82;-470.8473,-2016.034;Float;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-386.4544,-516.796;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleTimeNode;111;370.099,406.9471;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;52;-389.1529,-382.2845;Float;False;Constant;_Vector2;Vector 2;10;0;0.5,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-554,214;Float;False;Constant;_Float1;Float 1;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;109;533.5447,555.9471;Float;False;Constant;_Float12;Float 12;11;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-426.365,-845.2156;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;50;-209.0535,-487.0229;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SinOpNode;106;564.3552,401.7297;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-165.6382,-2006.795;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;43;-473.0014,-102.3308;Float;False;Constant;_Emission_power;Emission_power;9;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;110;544.0792,633.9471;Float;False;Constant;_Float13;Float 13;11;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;108;541.1287,476.0461;Float;False;Constant;_Float4;Float 4;11;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector2Node;87;-186.7565,-1795.261;Float;False;Constant;_Vector5;Vector 5;12;0;1,0;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-390.9781,93.28053;Float;True;Property;_Normal;Normal;0;0;Assets/Scripts/ImageEffects/Eclipse/sf_noise_clouds.png;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TexturePropertyNode;83;-440.6442,-2230.995;Float;True;Property;_MaskB;MaskB;7;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.PowerNode;47;-232.5192,-112.2856;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;45;-236.5004,-187.853;Float;False;Constant;_Float7;Float 7;9;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;58;127.0862,-311.6412;Float;False;Constant;_Float9;Float 9;10;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;107;719.1595,398.7237;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;23;-125.3861,-839.151;Float;True;Property;_TextureSample0;Texture Sample 0;8;0;Assets/Graph/Textures/LIFE/T_eye_C.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;49;-18.73573,-512.5689;Float;True;Property;_TextureSample1;Texture Sample 1;9;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;86;2.806063,-1798.676;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;46;-231.3562,-13.97838;Float;False;Constant;_Float8;Float 8;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;907.0286,344.6176;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;39;107.8524,-113.0815;Float;False;Constant;_Vector1;Vector 1;8;0;3,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;57;307.4134,-503.7289;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;580.8208,275.7882;Float;False;Property;_Pupille_Emissive;Pupille_Emissive;11;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;44;-48.51391,-118.7919;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;85;204.9397,-1803.422;Float;True;Property;_TextureSample3;Texture Sample 3;8;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;380.362,-1606.796;Float;False;Constant;_Float10;Float 10;12;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;729.3355,214.1222;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;81;206.2949,-2005.033;Float;True;Property;_TextureSample2;Texture Sample 2;10;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;20;-404.7436,897.6764;Float;True;Property;_MaskA;MaskA;6;0;Assets/Graph/Textures/LIFE/T_eye_Mask_A.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;91;567.3621,-1738.795;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;60;405.4174,-674.9092;Float;False;Constant;_Vector3;Vector 3;10;0;2,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;469.2908,-504.8885;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;38;290.6543,-126.104;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;27;-41.30373,-984.4874;Float;False;Constant;_Float3;Float 3;8;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;72;712.0707,59.29372;Float;False;Constant;_Vector0;Vector 0;10;0;1,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;84;766.0453,-1873.895;Float;True;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;48;465.3953,-159.6136;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;59;632.7868,-512.8761;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.ClampOpNode;77;890.8644,200.7301;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;26;-45.48309,-1072.011;Float;False;Constant;_Float2;Float 2;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;98;858.9749,-1649.483;Float;False;Constant;_Float11;Float 11;11;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;25;155.303,-1046.569;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;80;1032.15,-53.09607;Float;False;Property;_Fresnel_color;Fresnel_color;9;0;1,0,0;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;1030.271,-1870.651;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;54;818.5114,-516.4575;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;71;1095.604,153.6607;Float;False;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0,0,0;False;2;FLOAT;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.OneMinusNode;94;1205.824,-1869.879;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;53;1025.14,-557.7187;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.LerpOp;104;1278.552,-14.56565;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;1446.782,-411.3882;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;1439.921,-133.5423;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.NormalVertexDataNode;17;-355.4597,692.9683;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-119.4598,650.9683;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;19;-124.4598,750.9683;Float;False;Property;_VO_intensity;VO_intensity;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;125;1621.406,-500.8204;Float;False;Property;_Hue;Hue;10;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RGBToHSVNode;120;1592.406,-6.820374;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RGBToHSVNode;121;1623.406,-405.3204;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;123;1832.406,-159.8204;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;22;-92.81915,835.2407;Float;False;Constant;_Float0;Float 0;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;77.54023,657.9683;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;124;1834.406,-523.8204;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.HSVToRGBNode;122;1885.406,-392.3204;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.HSVToRGBNode;119;1854.406,6.179626;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;21;274.7218,782.6149;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0;False;2;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2156.28,-67.64063;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Eye;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;DeferredOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;7;0
WireConnection;8;1;9;0
WireConnection;10;0;8;0
WireConnection;10;2;11;0
WireConnection;6;0;93;0
WireConnection;6;1;10;0
WireConnection;40;0;6;2
WireConnection;40;1;41;0
WireConnection;5;0;4;0
WireConnection;5;1;40;0
WireConnection;13;0;5;0
WireConnection;13;1;14;0
WireConnection;55;0;51;0
WireConnection;55;1;56;0
WireConnection;111;0;112;0
WireConnection;50;0;55;0
WireConnection;50;2;52;0
WireConnection;106;0;111;0
WireConnection;89;0;82;0
WireConnection;89;1;90;0
WireConnection;3;1;13;0
WireConnection;3;5;12;0
WireConnection;47;0;3;1
WireConnection;47;1;43;0
WireConnection;107;0;106;0
WireConnection;107;1;110;0
WireConnection;107;2;109;0
WireConnection;107;3;108;0
WireConnection;107;4;109;0
WireConnection;23;0;93;0
WireConnection;23;1;24;0
WireConnection;49;0;83;0
WireConnection;49;1;50;0
WireConnection;86;0;89;0
WireConnection;86;2;87;0
WireConnection;105;0;23;3
WireConnection;105;1;107;0
WireConnection;57;0;49;2
WireConnection;57;1;58;0
WireConnection;44;0;47;0
WireConnection;44;1;45;0
WireConnection;44;2;46;0
WireConnection;85;0;83;0
WireConnection;85;1;86;0
WireConnection;75;0;105;0
WireConnection;75;1;74;0
WireConnection;81;0;83;0
WireConnection;81;1;82;0
WireConnection;20;1;4;0
WireConnection;91;0;85;2
WireConnection;91;1;92;0
WireConnection;61;0;57;0
WireConnection;61;1;58;0
WireConnection;38;0;45;0
WireConnection;38;1;39;0
WireConnection;38;2;44;0
WireConnection;84;1;91;0
WireConnection;84;2;81;1
WireConnection;48;0;45;0
WireConnection;48;1;38;0
WireConnection;48;2;20;3
WireConnection;59;1;60;0
WireConnection;59;2;61;0
WireConnection;77;0;75;0
WireConnection;77;1;45;0
WireConnection;77;2;46;0
WireConnection;25;2;26;0
WireConnection;25;3;27;0
WireConnection;97;0;84;0
WireConnection;97;1;98;0
WireConnection;54;1;59;0
WireConnection;54;2;20;3
WireConnection;71;0;48;0
WireConnection;71;1;72;0
WireConnection;71;2;77;0
WireConnection;94;0;97;0
WireConnection;53;0;23;1
WireConnection;53;1;54;0
WireConnection;104;0;71;0
WireConnection;104;1;80;0
WireConnection;104;2;25;0
WireConnection;96;0;53;0
WireConnection;96;1;94;0
WireConnection;95;0;104;0
WireConnection;95;1;94;0
WireConnection;16;0;6;2
WireConnection;16;1;17;0
WireConnection;120;0;95;0
WireConnection;121;0;96;0
WireConnection;123;0;120;0
WireConnection;123;1;125;0
WireConnection;18;0;16;0
WireConnection;18;1;19;0
WireConnection;124;0;121;0
WireConnection;124;1;125;0
WireConnection;122;0;124;0
WireConnection;122;1;121;2
WireConnection;122;2;121;3
WireConnection;119;0;123;0
WireConnection;119;1;120;2
WireConnection;119;2;120;3
WireConnection;21;0;22;0
WireConnection;21;1;18;0
WireConnection;21;2;20;2
WireConnection;0;0;122;0
WireConnection;0;2;119;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=9B1CB0891B9BEF5D02EBCFB82F57EBA08D62390B