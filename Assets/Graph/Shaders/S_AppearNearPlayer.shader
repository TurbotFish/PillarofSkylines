Shader "Custom/AppearNearPlayer" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {} 
        _VisibleDistance ("Visibility Distance", float) = 10.0 
        _OutlineWidth ("Outline Width", float) = 3.0 
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseInfluence ("Noise Influence", float) = 1.0 
        _OutlineColour ("Outline Colour", color) = (1.0,1.0,0.0,1.0) 
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass {
        Blend SrcAlpha OneMinusSrcAlpha
         
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag       
 
        // Access the shaderlab properties
        uniform sampler2D _MainTex;
        uniform sampler2D _NoiseTex;
        uniform float4 _PlayerPosition;
        uniform float _VisibleDistance;
        uniform float _OutlineWidth;
        uniform float _NoiseInfluence;
        uniform float4 _OutlineColour;
         
        // Input to vertex shader
        struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
        };
        // Input to fragment shader
        struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 position_in_world_space : TEXCOORD0;
            float4 tex : TEXCOORD1;
        };
          
        // VERTEX SHADER
        vertexOutput vert(vertexInput input) 
        {
            vertexOutput output; 
            output.pos =  UnityObjectToClipPos(input.vertex);
            output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
            output.tex = input.texcoord;
            return output;
        }
  
        // FRAGMENT SHADER
        float4 frag(vertexOutput input) : COLOR {
            float dist = distance(input.position_in_world_space, _PlayerPosition);

			float4 noise = tex2D(_NoiseTex, input.tex);
			float targetDistance = (_VisibleDistance + noise.r * _NoiseInfluence);

            // Return appropriate colour
            if (dist < targetDistance) {
                return tex2D(_MainTex, input.tex); // Visible
            }
            else if (dist < (targetDistance + _OutlineWidth)) {
                return _OutlineColour; // Edge of visible range
            }
            else {
                float4 tex = tex2D(_MainTex, input.tex); // Outside visible range
				clip(-1);
                tex.a = 0;
                return tex;
            }
        }
 
        ENDCG
        } // End Pass
    } // End Subshader
    FallBack "Diffuse"
} // End Shader