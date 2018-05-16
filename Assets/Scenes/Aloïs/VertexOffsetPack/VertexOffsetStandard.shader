// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Aurélie/VertexOffsetPBR" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MaxBendAngle ("Bend Angle Max", Float) = 60
		_BendingDistMin ("Bending Full Effect Dist.", Float) = 2
		_BendingDistMax ("Bending No Effect Dist.", Float) = 5
		_WindDirection ("Wind Direction", Vector) = (0,0,1,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _PlayerPos;
		float _BendingDistMin;
		float _BendingDistMax;
		float _MaxBendAngle;
		float _WindIntensity;
		sampler2D _WindTex;
		float3 _WindDirection;

		float3 RotationMatXYZ(float3 _vertex, float3 rotation){
		float3x3 mat;
			mat[0] = float3(
				cos(rotation.y) * cos(rotation.z),
				-cos(rotation.y) * sin(rotation.z), 
				sin(rotation.y)
			);
			mat[1] = float3(
				cos(rotation.x) * sin(rotation.z) + sin(rotation.x) * sin(rotation.y) * cos(rotation.z),
				cos(rotation.x) * cos(rotation.z) - sin(rotation.x) * sin(rotation.y) * sin(rotation.z),
				-sin(rotation.x) * cos(rotation.y)
			);
			mat[2] = float3(
				sin(rotation.x) * sin(rotation.z) - cos(rotation.x) * sin(rotation.y) * cos(rotation.z),
				sin(rotation.x) * cos(rotation.z) + cos(rotation.x) * sin(rotation.y) * sin(rotation.z),
				cos(rotation.x) * cos(rotation.y)
			);
			return mul(mat, _vertex);
		}

		float3 ApplyWind(float3 _vec, float3 _rotation){
			_rotation *= 3.1416 / 180;
			_vec = RotationMatXYZ(_vec, _rotation);
			return _vec;
		}

		void vert(inout appdata_full v){
			float rotationMask = v.color.r;
			float4 pivotWS =  mul(unity_ObjectToWorld,float4(0,0,0,1));
			float2 player2Vert2D = pivotWS.xz - _PlayerPos.xz;
			float3 player2Vert3D = pivotWS.xyz - _PlayerPos.xyz;
			float sqrDist = dot(player2Vert3D, player2Vert3D); 
			float bendPercent = 1 - saturate((sqrDist - _BendingDistMin)/(_BendingDistMax - _BendingDistMin));
			float bendAmount = bendPercent * _MaxBendAngle * rotationMask;
			float3 _bendRotation = normalize(float3(player2Vert2D.y,0, -player2Vert2D.x)) * bendAmount;
			v.vertex.xyz = ApplyWind(v.vertex.xyz, _bendRotation);
			v.normal = ApplyWind(v.normal.xyz, _bendRotation);

			//wind
			//float3 windDir = float3(0.0,0,1.0);//global in the future

			float3 windDir = saturate(_WindDirection.xyz);

			float windSpeed = 3;
			float _offset = (pivotWS.x * (2.9) * -sign(windDir.x) + pivotWS.z * (2.9) * -sign(windDir.z) + pivotWS.y * 0.8) * 0.5;

			windDir = mul(unity_WorldToObject, windDir);
			windDir = normalize(windDir);

			float windIntensity = tex2Dlod(_WindTex, float4(_Time.x,_Time.x, 0,0)).r;
			windIntensity = saturate(windIntensity + 0.0);//not necessary with a good wind map

			float angle = windIntensity * (sin(_Time.y * windSpeed + _offset) * 0.65+0.35) * (_MaxBendAngle) * rotationMask;
			float3 _windRotation = float3( windDir.z, 0, -windDir.x) * angle;

			v.vertex.xyz = ApplyWind(v.vertex.xyz, _windRotation);
			v.normal = ApplyWind(v.normal.xyz, _windRotation);
		}

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
