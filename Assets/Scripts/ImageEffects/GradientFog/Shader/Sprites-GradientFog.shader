Shader "Sprites/GradientFog (Grr)" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
		
        Pass {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#pragma shader_feature _GRADIENT_FOG
            #include "UnitySprites.cginc"
			
			uniform sampler2D _GradientFog;
			float _FogStart;
			float _FogEnd;

			fixed4 frag(v2f IN) : SV_Target {
				
				fixed4 sprite = SampleSpriteTexture (IN.texcoord);
				fixed4 c = sprite * IN.color;
				c.rgb *= c.a;

				c = ApplyGradientFog(c, _FogStart, _FogEnd, _GradientFog);

				return c;
			}
        ENDCG
        }
    }
}
