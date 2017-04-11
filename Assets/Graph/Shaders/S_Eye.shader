// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:7621,x:32719,y:32712,varname:node_7621,prsc:2|emission-3882-OUT,voffset-5395-OUT;n:type:ShaderForge.SFN_Color,id:8652,x:31939,y:32179,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_8652,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9728,x:32107,y:32274,varname:node_9728,prsc:2|A-8652-RGB,B-5589-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5589,x:31939,y:32353,ptovrint:False,ptlb:Emissive_intensity,ptin:_Emissive_intensity,varname:node_5589,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:8112,x:31733,y:32473,varname:node_8112,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:4483,x:31917,y:32617,varname:node_4483,prsc:2,v1:2;n:type:ShaderForge.SFN_Posterize,id:9403,x:32107,y:32483,varname:node_9403,prsc:2|IN-5991-OUT,STPS-4483-OUT;n:type:ShaderForge.SFN_Add,id:5991,x:31917,y:32483,varname:node_5991,prsc:2|A-8112-U,B-9976-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3957,x:31722,y:32642,ptovrint:False,ptlb:Offset,ptin:_Offset,varname:node_3957,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:3882,x:32391,y:32434,varname:node_3882,prsc:2|A-9728-OUT,B-7168-OUT,T-6163-OUT;n:type:ShaderForge.SFN_Vector1,id:7168,x:32107,y:32420,varname:node_7168,prsc:2,v1:0;n:type:ShaderForge.SFN_RemapRange,id:9976,x:31811,y:32740,varname:node_9976,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-3957-OUT;n:type:ShaderForge.SFN_Clamp01,id:6163,x:32266,y:32572,varname:node_6163,prsc:2|IN-9403-OUT;n:type:ShaderForge.SFN_NormalVector,id:189,x:31972,y:33288,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:510,x:32174,y:33161,varname:node_510,prsc:2|A-9302-OUT,B-189-OUT;n:type:ShaderForge.SFN_Multiply,id:5395,x:32380,y:33161,varname:node_5395,prsc:2|A-510-OUT,B-9547-OUT;n:type:ShaderForge.SFN_Multiply,id:969,x:31788,y:33161,varname:node_969,prsc:2|A-5587-T,B-7940-OUT;n:type:ShaderForge.SFN_Sin,id:5106,x:31972,y:33161,varname:node_5106,prsc:2|IN-969-OUT;n:type:ShaderForge.SFN_Time,id:5587,x:31610,y:33161,varname:node_5587,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:7940,x:31610,y:33317,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_7940,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:9547,x:32174,y:33320,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_9547,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_RemapRange,id:9302,x:32089,y:32974,varname:node_9302,prsc:2,frmn:-1,frmx:1,tomn:-1,tomx:0|IN-5106-OUT;proporder:8652-5589-3957-7940-9547;pass:END;sub:END;*/

Shader "Custom/Eye" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Emissive_intensity ("Emissive_intensity", Float ) = 1
        _Offset ("Offset", Float ) = 0
        _Speed ("Speed", Float ) = 1
        _Intensity ("Intensity", Float ) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _Emissive_intensity;
            uniform float _Offset;
            uniform float _Speed;
            uniform float _Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_5587 = _Time + _TimeEditor;
                v.vertex.xyz += (((sin((node_5587.g*_Speed))*0.5+-0.5)*v.normal)*_Intensity);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_7168 = 0.0;
                float node_4483 = 2.0;
                float3 emissive = lerp((_Color.rgb*_Emissive_intensity),float3(node_7168,node_7168,node_7168),saturate(floor((i.uv0.r+(_Offset*1.0+-0.5)) * node_4483) / (node_4483 - 1)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _Speed;
            uniform float _Intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_5587 = _Time + _TimeEditor;
                v.vertex.xyz += (((sin((node_5587.g*_Speed))*0.5+-0.5)*v.normal)*_Intensity);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
