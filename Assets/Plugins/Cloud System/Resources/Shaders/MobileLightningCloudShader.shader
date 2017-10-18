//
// MobileLightningCloudShader.shader: Surface shader for clouds which uses a simple cloud lighting model.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

Shader "Cloud/Mobile Lightning Cloud" {

	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
	
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "MobileCloudLightingModel.cginc"
		#pragma surface surf MobileCloud alpha noambient nolightmap noforwardadd approxview halfasview vertex:vert

		sampler2D _MainTex;
		fixed4 _Color;
		
		struct Input {
			fixed2 uv_MainTex;
			fixed4 vertexColor;
		};
		
		void vert (inout appdata_full v, out Input o) {
			o.vertexColor.rgb = v.color.rgb;
			o.vertexColor.a = v.color.a;
			
			v.normal = normalize (v.vertex);
			v.tangent = fixed4 (0, 0, 0, 0);
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * (IN.vertexColor.rgb);
			o.Alpha = c.a * (IN.vertexColor.a);
		}
		ENDCG
	}
	
	Fallback "Cloud/Cloud"
}