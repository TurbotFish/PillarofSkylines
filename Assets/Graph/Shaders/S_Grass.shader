// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:1,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2454,x:32719,y:32712,varname:node_2454,prsc:2|emission-3131-OUT,alpha-4885-R,clip-4885-R,voffset-4949-OUT;n:type:ShaderForge.SFN_Tex2d,id:8792,x:32321,y:32577,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_8792,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4885,x:32364,y:32855,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_4885,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3131,x:32521,y:32712,varname:node_3131,prsc:2|A-4957-OUT,B-1433-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1433,x:32321,y:32776,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_1433,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:6975,x:31655,y:33333,varname:node_6975,prsc:2,uv:0;n:type:ShaderForge.SFN_Lerp,id:2895,x:31863,y:33266,varname:node_2895,prsc:2|A-3390-OUT,B-5708-OUT,T-6975-V;n:type:ShaderForge.SFN_Sin,id:5708,x:31655,y:33192,varname:node_5708,prsc:2|IN-8656-OUT;n:type:ShaderForge.SFN_Vector1,id:3390,x:31655,y:33137,varname:node_3390,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:8656,x:31487,y:33192,varname:node_8656,prsc:2|A-1570-OUT,B-9051-OUT;n:type:ShaderForge.SFN_Time,id:7655,x:31020,y:33079,varname:node_7655,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9051,x:31215,y:33339,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9051,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:731,x:32090,y:33266,varname:node_731,prsc:2|A-2895-OUT,B-1170-OUT;n:type:ShaderForge.SFN_NormalVector,id:1170,x:31863,y:33388,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:4949,x:32259,y:33266,varname:node_4949,prsc:2|A-731-OUT,B-8971-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8971,x:32090,y:33414,ptovrint:False,ptlb:VO_intensity,ptin:_VO_intensity,varname:node_8971,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:1570,x:31215,y:33185,varname:node_1570,prsc:2|A-7655-T,B-8964-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:473,x:30585,y:33103,varname:node_473,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8964,x:31033,y:33208,varname:node_8964,prsc:2|A-9476-OUT,B-5580-OUT,C-7099-OUT;n:type:ShaderForge.SFN_Multiply,id:9476,x:30812,y:33103,varname:node_9476,prsc:2|A-473-X,B-1997-OUT;n:type:ShaderForge.SFN_Vector1,id:1997,x:30585,y:33270,varname:node_1997,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:5580,x:30812,y:33220,varname:node_5580,prsc:2|A-473-Y,B-1997-OUT;n:type:ShaderForge.SFN_Multiply,id:7099,x:30812,y:33346,varname:node_7099,prsc:2|A-473-Z,B-1997-OUT;n:type:ShaderForge.SFN_RgbToHsv,id:6483,x:32489,y:32427,varname:node_6483,prsc:2|IN-8792-RGB;n:type:ShaderForge.SFN_HsvToRgb,id:4957,x:32739,y:32428,varname:node_4957,prsc:2|H-4505-OUT,S-1600-OUT,V-2643-OUT;n:type:ShaderForge.SFN_Add,id:4505,x:32628,y:32287,varname:node_4505,prsc:2|A-6483-HOUT,B-3214-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3214,x:32417,y:32346,ptovrint:False,ptlb:Hue,ptin:_Hue,varname:node_3214,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:1600,x:32628,y:32162,varname:node_1600,prsc:2|A-6483-SOUT,B-2899-OUT;n:type:ShaderForge.SFN_Add,id:2643,x:32628,y:32028,varname:node_2643,prsc:2|A-6483-VOUT,B-1343-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2899,x:32417,y:32196,ptovrint:False,ptlb:Sat,ptin:_Sat,varname:node_2899,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:1343,x:32417,y:32079,ptovrint:False,ptlb:Val,ptin:_Val,varname:node_1343,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:8792-4885-1433-9051-8971-3214-2899-1343;pass:END;sub:END;*/

Shader "Custom/Grass" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _Opacity ("Opacity", 2D) = "white" {}
        _Emission ("Emission", Float ) = 0
        _Speed ("Speed", Float ) = 0
        _VO_intensity ("VO_intensity", Float ) = 1
        _Hue ("Hue", Float ) = 0
        _Sat ("Sat", Float ) = 0
        _Val ("Val", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "DisableBatching"="True"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
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
            uniform float4 _TimeEditor;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform sampler2D _Opacity; uniform float4 _Opacity_ST;
            uniform float _Emission;
            uniform float _Speed;
            uniform float _VO_intensity;
            uniform float _Hue;
            uniform float _Sat;
            uniform float _Val;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float4 node_7655 = _Time + _TimeEditor;
                float node_1997 = 1.0;
                v.vertex.xyz += ((lerp(0.0,sin(((node_7655.g+((objPos.r*node_1997)*(objPos.g*node_1997)*(objPos.b*node_1997)))*_Speed)),o.uv0.g)*v.normal)*_VO_intensity);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _Opacity_var = tex2D(_Opacity,TRANSFORM_TEX(i.uv0, _Opacity));
                clip(_Opacity_var.r - 0.5);
////// Lighting:
////// Emissive:
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float4 node_6483_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_6483_p = lerp(float4(float4(_Texture_var.rgb,0.0).zy, node_6483_k.wz), float4(float4(_Texture_var.rgb,0.0).yz, node_6483_k.xy), step(float4(_Texture_var.rgb,0.0).z, float4(_Texture_var.rgb,0.0).y));
                float4 node_6483_q = lerp(float4(node_6483_p.xyw, float4(_Texture_var.rgb,0.0).x), float4(float4(_Texture_var.rgb,0.0).x, node_6483_p.yzx), step(node_6483_p.x, float4(_Texture_var.rgb,0.0).x));
                float node_6483_d = node_6483_q.x - min(node_6483_q.w, node_6483_q.y);
                float node_6483_e = 1.0e-10;
                float3 node_6483 = float3(abs(node_6483_q.z + (node_6483_q.w - node_6483_q.y) / (6.0 * node_6483_d + node_6483_e)), node_6483_d / (node_6483_q.x + node_6483_e), node_6483_q.x);;
                float3 emissive = ((lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac((node_6483.r+_Hue)+float3(0.0,-1.0/3.0,1.0/3.0)))-1),(node_6483.g+_Sat))*(node_6483.b+_Val))*_Emission);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,_Opacity_var.r);
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
            uniform sampler2D _Opacity; uniform float4 _Opacity_ST;
            uniform float _Speed;
            uniform float _VO_intensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float4 node_7655 = _Time + _TimeEditor;
                float node_1997 = 1.0;
                v.vertex.xyz += ((lerp(0.0,sin(((node_7655.g+((objPos.r*node_1997)*(objPos.g*node_1997)*(objPos.b*node_1997)))*_Speed)),o.uv0.g)*v.normal)*_VO_intensity);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _Opacity_var = tex2D(_Opacity,TRANSFORM_TEX(i.uv0, _Opacity));
                clip(_Opacity_var.r - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
