//
// CloudShader.shader: Fixed function shader for clouds.
//
// Author:
//   Based on the Unity3D built-in shaders
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

Shader "Cloud/Cloud" {
	Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	
	Category {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Lighting Off
		ColorMask RGB
		Cull Off
		ZWrite Off
		Fog {Mode Off}
	
		SubShader {
			Pass {
				Blend SrcAlpha OneMinusSrcAlpha
			
				BindChannels {
					Bind "Vertex", vertex
					Bind "Texcoord", texcoord
					Bind "Color", color
				}

				SetTexture [_MainTex] {
					combine primary * texture
				}
			}
		}
	}
}
