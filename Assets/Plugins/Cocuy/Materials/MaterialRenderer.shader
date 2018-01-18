
Shader "Cocuy/Material Renderer" 
{
	SubShader 
	{
		Blend SrcAlpha OneMinusSrcAlpha 
		Tags {"Queue" = "Transparent" }
    	Pass 
    	{
			ZTest LEqual

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 4.5
			#pragma vertex vert
			#pragma fragment frag
			
			StructuredBuffer<float> _Particles;
			StructuredBuffer<float4> _ColourRamp;
			float2 _Size;

			struct v2f 
			{
    			float4  pos : SV_POSITION;
    			float2  uv : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
    			v2f OUT;
    			OUT.pos = UnityObjectToClipPos(v.vertex);
    			OUT.uv = v.texcoord.xy;
    			return OUT;
			}
			
			float4 frag(v2f IN) : COLOR
			{
				int x = (int)(IN.uv.x*_Size.x);
				int y = (int)(IN.uv.y*_Size.y);		
				return _ColourRamp[clamp(_Particles[y*_Size.x + x], 0.0, 255.0)];
			}
			
			ENDCG
    	}
	}
	Fallback "Diffuse"
}