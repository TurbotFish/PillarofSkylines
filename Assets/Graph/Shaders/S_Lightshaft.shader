// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bobo/Lightshaft"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Color("Color", Color) = (0,0,0,0)
		_Scale("Scale", Float) = 1
		_Power("Power", Float) = 1
		_Emissive_intensity("Emissive_intensity", Float) = 1
		_Distancefade("Distancefade", Float) = 0.01
		_Float9("Float 9", Float) = 0
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_Vo_scale("Vo_scale", Range( 0 , 1)) = 1
		_FadeStart_Offset("FadeStart_Offset", Range( -1 , 0)) = 0
		[Toggle]_SinFade("SinFade", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+5" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 texcoord_0;
			float2 texcoord_1;
		};

		uniform float4 _Color;
		uniform float _Emissive_intensity;
		uniform float _Scale;
		uniform float _Power;
		uniform float _FadeStart_Offset;
		uniform float _Float9;
		uniform float _Distancefade;
		uniform float _Opacity;
		uniform float _SinFade;
		uniform float _Vo_scale;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float mulTime22 = _Time.y * 1.3;
			float temp_output_21_0 = sin( mulTime22 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float lerpResult38 = lerp( (0.8 + (temp_output_21_0 - -1.0) * (1.1 - 0.8) / (1.0 - -1.0)) , 0.0 , o.texcoord_1.y);
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( lerpResult38 * ase_vertexNormal ) * _Vo_scale );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime22 = _Time.y * 1.3;
			float temp_output_21_0 = sin( mulTime22 );
			o.Emission = ( ( _Color * _Emissive_intensity ) * (0.6 + (temp_output_21_0 - -1.0) * (1.0 - 0.6) / (1.0 - -1.0)) ).rgb;
			float3 ase_worldPos = i.worldPos;
			fixed3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNode5 = ( 0.0 + _Scale * pow( 1.0 - dot( ase_worldNormal, ase_worldViewDir ), _Power ) );
			float clampResult6 = clamp( fresnelNode5 , 0.0 , 1.0 );
			float clampResult32 = clamp( ( distance( ase_worldPos , _WorldSpaceCameraPos ) * _Float9 ) , 0.0 , 1.0 );
			o.Alpha = ( ( ( ( ( 1.0 - clampResult6 ) * ( ( i.texcoord_0.y * 3.0 ) * ( 1.0 - ( pow( ( i.texcoord_0.y - _FadeStart_Offset ) , 7.0 ) * 5.0 ) ) ) ) * ( clampResult32 * ( 1.0 - saturate( ( distance( ase_worldPos , _WorldSpaceCameraPos ) * _Distancefade ) ) ) ) ) * _Opacity ) * lerp(1.0,(0.3 + (sin( _Time.y ) - -1.0) * (1.0 - 0.3) / (1.0 - -1.0)),_SinFade) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13201
-1913;29;1906;1004;1434.724;67.73488;1.199235;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-693.6952,75.21002;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;62;-1148.559,303.9121;Float;False;Property;_FadeStart_Offset;FadeStart_Offset;8;0;0;-1;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceCameraPos;75;-886.3962,906.55;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.RangedFloatNode;47;-653.6392,319.1445;Float;False;Constant;_FadeStart_Power;FadeStart_Power;6;0;7;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;60;-845.1612,285.082;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldPosInputsNode;76;-816.4907,754.5057;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-822.6952,-125.79;Float;False;Property;_Power;Power;2;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceCameraPos;31;-886.7013,599.5976;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;30;-816.7958,447.5533;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;78;-552.2554,968.7595;Float;False;Property;_Distancefade;Distancefade;4;0;0.01;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PowerNode;46;-450.6934,306.9179;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;-461.3541,403.9451;Float;False;Constant;_Float12;Float 12;6;0;5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-819.6952,-278.79;Float;False;Constant;_Float5;Float 5;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;77;-550.331,847.6549;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-821.6952,-199.79;Float;False;Property;_Scale;Scale;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-552.5605,661.8071;Float;False;Property;_Float9;Float 9;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;29;-550.6361,540.7025;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;-563.6952,-20.78998;Float;False;Constant;_Float2;Float 2;1;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-290.3541,396.9451;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-645.6952,210.21;Float;False;Constant;_Float0;Float 0;0;0;3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-558.6952,-94.78998;Float;False;Constant;_Float1;Float 1;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.FresnelNode;5;-654.6952,-236.79;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-368.6976,896.8502;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;-459.281,-855.2441;Float;False;Constant;_Float3;Float 3;4;0;1.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;49;-154.5336,231.5575;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;22;-272.281,-847.2441;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;26;-67.28101,-1127.244;Float;False;Constant;_Float6;Float 6;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;27;-66.28101,-935.2441;Float;False;Constant;_Float7;Float 7;4;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-369.0027,589.8978;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;66;9.985309,697.6846;Float;False;Constant;_Float13;Float 13;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;6;-386.6952,-230.79;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;80;-242.6843,886.8563;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-444.6952,194.21;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;65;160.321,704.4413;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;20.71809,134.8495;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;32;-200.4603,580.3401;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;82;-354.2131,774.1282;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;41;165.9461,-259.239;Float;False;Constant;_Float11;Float 11;5;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-72.28101,-1211.244;Float;False;Constant;_Float4;Float 4;4;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;21;-68.9539,-853.9545;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;15;-242.6952,-235.79;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;40;163.7536,-182.2476;Float;False;Constant;_Float10;Float 10;5;0;1.1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;63;510.3932,663.6662;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-61.69519,-76.78998;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;68;474.9207,824.137;Float;False;Constant;_Float15;Float 15;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;39;339.2027,-324.3427;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;69;485.0557,906.9061;Float;False;Constant;_Float16;Float 16;8;0;0.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;67;481.6774,749.8137;Float;False;Constant;_Float14;Float 14;8;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-14.8296,573.856;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;281.9636,175.4749;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;64;652.2831,655.2204;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;234.0948,367.0039;Float;False;Property;_Opacity;Opacity;6;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;18;-373.3949,-517.69;Float;False;Property;_Emissive_intensity;Emissive_intensity;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;38;549.7456,-320.923;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;37;524.7997,-192.2544;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;28;-67.28101,-1010.244;Float;False;Constant;_Float8;Float 8;4;0;0.6;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;4;-358.6952,-723.79;Float;False;Property;_Color;Color;0;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;73;556.2072,490.8539;Float;False;Constant;_Float17;Float 17;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;600.2395,190.7885;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TFHCRemap;24;133.719,-857.2441;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;759.5278,-351.4326;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-71.69495,-667.79;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;53;736.4127,-72.92875;Float;False;Property;_Vo_scale;Vo_scale;7;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;72;722.918,481.4174;Float;False;Property;_SinFade;SinFade;9;0;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;144.2461,-640.7545;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;861.7396,285.2931;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;1047.708,-172.2786;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;148.537,531.6872;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1247.068,43.7627;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Bobo/Lightshaft;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;5;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;60;0;1;2
WireConnection;60;1;62;0
WireConnection;46;0;60;0
WireConnection;46;1;47;0
WireConnection;77;0;76;0
WireConnection;77;1;75;0
WireConnection;29;0;30;0
WireConnection;29;1;31;0
WireConnection;50;0;46;0
WireConnection;50;1;51;0
WireConnection;5;1;13;0
WireConnection;5;2;11;0
WireConnection;5;3;12;0
WireConnection;79;0;77;0
WireConnection;79;1;78;0
WireConnection;49;0;50;0
WireConnection;22;0;23;0
WireConnection;33;0;29;0
WireConnection;33;1;34;0
WireConnection;6;0;5;0
WireConnection;6;1;9;0
WireConnection;6;2;10;0
WireConnection;80;0;79;0
WireConnection;2;0;1;2
WireConnection;2;1;3;0
WireConnection;65;0;66;0
WireConnection;48;0;2;0
WireConnection;48;1;49;0
WireConnection;32;0;33;0
WireConnection;32;1;26;0
WireConnection;32;2;27;0
WireConnection;82;0;80;0
WireConnection;21;0;22;0
WireConnection;15;0;6;0
WireConnection;63;0;65;0
WireConnection;16;0;15;0
WireConnection;16;1;48;0
WireConnection;39;0;21;0
WireConnection;39;1;25;0
WireConnection;39;2;27;0
WireConnection;39;3;41;0
WireConnection;39;4;40;0
WireConnection;81;0;32;0
WireConnection;81;1;82;0
WireConnection;35;0;16;0
WireConnection;35;1;81;0
WireConnection;64;0;63;0
WireConnection;64;1;67;0
WireConnection;64;2;68;0
WireConnection;64;3;69;0
WireConnection;64;4;68;0
WireConnection;38;0;39;0
WireConnection;38;2;1;2
WireConnection;42;0;35;0
WireConnection;42;1;43;0
WireConnection;24;0;21;0
WireConnection;24;1;25;0
WireConnection;24;2;27;0
WireConnection;24;3;28;0
WireConnection;24;4;27;0
WireConnection;36;0;38;0
WireConnection;36;1;37;0
WireConnection;17;0;4;0
WireConnection;17;1;18;0
WireConnection;72;0;73;0
WireConnection;72;1;64;0
WireConnection;19;0;17;0
WireConnection;19;1;24;0
WireConnection;71;0;42;0
WireConnection;71;1;72;0
WireConnection;52;0;36;0
WireConnection;52;1;53;0
WireConnection;0;2;19;0
WireConnection;0;9;71;0
WireConnection;0;11;52;0
ASEEND*/
//CHKSM=0E6C343DAA148BD603C539905978E99813F1EBE3