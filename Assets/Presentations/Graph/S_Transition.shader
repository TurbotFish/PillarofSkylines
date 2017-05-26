// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2260,x:32719,y:32712,varname:node_2260,prsc:2|emission-2858-RGB,alpha-3020-OUT;n:type:ShaderForge.SFN_Color,id:2858,x:32275,y:32500,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_2858,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7311,x:32315,y:32841,ptovrint:False,ptlb:Transition,ptin:_Transition,varname:node_7311,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:9248,x:32426,y:32427,varname:node_9248,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRange,id:4696,x:32607,y:32427,varname:node_4696,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-9248-UVOUT;n:type:ShaderForge.SFN_Multiply,id:9546,x:32791,y:32396,varname:node_9546,prsc:2|A-4696-OUT,B-4696-OUT;n:type:ShaderForge.SFN_ComponentMask,id:800,x:33000,y:32431,varname:node_800,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9546-OUT;n:type:ShaderForge.SFN_Add,id:6519,x:32277,y:32964,varname:node_6519,prsc:2|A-800-R,B-800-G;n:type:ShaderForge.SFN_Lerp,id:3020,x:32565,y:32908,varname:node_3020,prsc:2|A-7311-OUT,B-9917-OUT,T-8532-OUT;n:type:ShaderForge.SFN_Vector1,id:9917,x:32432,y:32728,varname:node_9917,prsc:2,v1:0;n:type:ShaderForge.SFN_Clamp01,id:8532,x:32429,y:33069,varname:node_8532,prsc:2|IN-6519-OUT;proporder:2858-7311;pass:END;sub:END;*/

Shader "Custom/Transition" {
    Properties {
        _Color ("Color", Color) = (0,0,0,1)
        _Transition ("Transition", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Transition;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = _Color.rgb;
                float3 finalColor = emissive;
                float2 node_4696 = (i.uv0*2.0+-1.0);
                float2 node_800 = (node_4696*node_4696).rg;
                fixed4 finalRGBA = fixed4(finalColor,lerp(_Transition,0.0,saturate((node_800.r+node_800.g))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
