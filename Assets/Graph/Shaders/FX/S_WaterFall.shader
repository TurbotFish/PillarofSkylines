// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:4,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:340,x:32876,y:32779,varname:node_340,prsc:2|diff-1255-RGB,spec-6151-OUT,gloss-5338-OUT,emission-1344-OUT,alpha-8468-OUT,refract-2621-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5877,x:31374,y:33421,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_5877,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Color,id:1255,x:32167,y:32717,ptovrint:False,ptlb:Color_copy,ptin:_Color_copy,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.0999135,c2:0.2828899,c3:0.3088235,c4:1;n:type:ShaderForge.SFN_Multiply,id:7166,x:32555,y:32870,varname:node_7166,prsc:2|A-1255-RGB,B-7496-OUT;n:type:ShaderForge.SFN_Vector1,id:7496,x:32167,y:32867,varname:node_7496,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Slider,id:6151,x:33162,y:32790,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_6151,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:5338,x:33162,y:32901,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5338,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:9927,x:32046,y:33619,ptovrint:False,ptlb:Up_mask,ptin:_Up_mask,varname:node_9927,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9514663dfa51ed54bba73797f727e49c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:287,x:32255,y:33373,varname:node_287,prsc:2|A-5877-OUT,B-2471-OUT,T-9927-R;n:type:ShaderForge.SFN_Tex2d,id:1465,x:31008,y:33656,ptovrint:False,ptlb:Cloud,ptin:_Cloud,varname:node_1465,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1039-OUT;n:type:ShaderForge.SFN_Lerp,id:2471,x:31749,y:33659,varname:node_2471,prsc:2|A-5877-OUT,B-5755-OUT,T-7604-OUT;n:type:ShaderForge.SFN_Vector1,id:5755,x:31390,y:33574,varname:node_5755,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:8629,x:31209,y:33698,varname:node_8629,prsc:2|A-1465-R,B-600-OUT;n:type:ShaderForge.SFN_Power,id:7604,x:31390,y:33698,varname:node_7604,prsc:2|VAL-8629-OUT,EXP-600-OUT;n:type:ShaderForge.SFN_Vector1,id:600,x:31008,y:33827,varname:node_600,prsc:2,v1:3;n:type:ShaderForge.SFN_TexCoord,id:2565,x:30370,y:33642,varname:node_2565,prsc:2,uv:0;n:type:ShaderForge.SFN_Lerp,id:5937,x:32526,y:33409,varname:node_5937,prsc:2|A-5755-OUT,B-287-OUT,T-6640-R;n:type:ShaderForge.SFN_Tex2d,id:6640,x:32243,y:33580,ptovrint:False,ptlb:Upper_mask,ptin:_Upper_mask,varname:node_6640,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7fa70003de97da04dbda91ec1cbf9996,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1581,x:30359,y:33810,varname:node_1581,prsc:2|A-2783-OUT,B-3016-T;n:type:ShaderForge.SFN_ValueProperty,id:2381,x:29734,y:33839,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_2381,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:1039,x:30677,y:33713,varname:node_1039,prsc:2|A-2565-UVOUT,B-1581-OUT;n:type:ShaderForge.SFN_Append,id:2783,x:30136,y:33786,varname:node_2783,prsc:2|A-5965-OUT,B-5630-OUT;n:type:ShaderForge.SFN_Time,id:3016,x:30136,y:33921,varname:node_3016,prsc:2;n:type:ShaderForge.SFN_Vector1,id:5965,x:29921,y:33768,varname:node_5965,prsc:2,v1:0;n:type:ShaderForge.SFN_RemapRange,id:5630,x:29921,y:33839,varname:node_5630,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.1|IN-2381-OUT;n:type:ShaderForge.SFN_Clamp01,id:8468,x:32538,y:33160,varname:node_8468,prsc:2|IN-5937-OUT;n:type:ShaderForge.SFN_Tex2d,id:6391,x:32075,y:33036,ptovrint:False,ptlb:normal,ptin:_normal,varname:node_6391,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1039-OUT;n:type:ShaderForge.SFN_Append,id:3952,x:32255,y:33053,varname:node_3952,prsc:2|A-6391-R,B-6391-G;n:type:ShaderForge.SFN_Lerp,id:2621,x:32740,y:33249,varname:node_2621,prsc:2|A-6702-OUT,B-7142-OUT,T-8468-OUT;n:type:ShaderForge.SFN_Vector1,id:6702,x:32569,y:32797,varname:node_6702,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:7142,x:32437,y:33053,varname:node_7142,prsc:2|A-3952-OUT,B-3423-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3423,x:32255,y:33219,ptovrint:False,ptlb:refraction,ptin:_refraction,varname:node_3423,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:2344,x:32739,y:32625,ptovrint:False,ptlb:emission,ptin:_emission,varname:node_2344,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1344,x:32725,y:32887,varname:node_1344,prsc:2|A-7166-OUT,B-2344-OUT;proporder:5877-1255-6151-5338-9927-1465-6640-2381-6391-3423-2344;pass:END;sub:END;*/

Shader "Custom/Waterfall" {
    Properties {
        _Opacity ("Opacity", Float ) = 0.5
        _Color_copy ("Color_copy", Color) = (0.0999135,0.2828899,0.3088235,1)
        _Specular ("Specular", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0
        _Up_mask ("Up_mask", 2D) = "white" {}
        _Cloud ("Cloud", 2D) = "white" {}
        _Upper_mask ("Upper_mask", 2D) = "white" {}
        _Speed ("Speed", Float ) = 1
        _normal ("normal", 2D) = "white" {}
        _refraction ("refraction", Float ) = 1
        _emission ("emission", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay"
            "RenderType"="Transparent"
        }
        LOD 200
        GrabPass{ }
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
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _Opacity;
            uniform float4 _Color_copy;
            uniform float _Specular;
            uniform float _Gloss;
            uniform sampler2D _Up_mask; uniform float4 _Up_mask_ST;
            uniform sampler2D _Cloud; uniform float4 _Cloud_ST;
            uniform sampler2D _Upper_mask; uniform float4 _Upper_mask_ST;
            uniform float _Speed;
            uniform sampler2D _normal; uniform float4 _normal_ST;
            uniform float _refraction;
            uniform float _emission;
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
                float4 screenPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_6702 = 0.0;
                float4 node_3016 = _Time + _TimeEditor;
                float2 node_1039 = (i.uv0+(float2(0.0,(_Speed*0.1+0.0))*node_3016.g));
                float4 _normal_var = tex2D(_normal,TRANSFORM_TEX(node_1039, _normal));
                float node_5755 = 0.0;
                float4 _Cloud_var = tex2D(_Cloud,TRANSFORM_TEX(node_1039, _Cloud));
                float node_600 = 3.0;
                float4 _Up_mask_var = tex2D(_Up_mask,TRANSFORM_TEX(i.uv0, _Up_mask));
                float4 _Upper_mask_var = tex2D(_Upper_mask,TRANSFORM_TEX(i.uv0, _Upper_mask));
                float node_8468 = saturate(lerp(node_5755,lerp(_Opacity,lerp(_Opacity,node_5755,pow((_Cloud_var.r*node_600),node_600)),_Up_mask_var.r),_Upper_mask_var.r));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + lerp(float2(node_6702,node_6702),(float2(_normal_var.r,_normal_var.g)*_refraction),node_8468);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = _Color_copy.rgb;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = ((_Color_copy.rgb*0.7)*_emission);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,node_8468),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _Opacity;
            uniform float4 _Color_copy;
            uniform float _Specular;
            uniform float _Gloss;
            uniform sampler2D _Up_mask; uniform float4 _Up_mask_ST;
            uniform sampler2D _Cloud; uniform float4 _Cloud_ST;
            uniform sampler2D _Upper_mask; uniform float4 _Upper_mask_ST;
            uniform float _Speed;
            uniform sampler2D _normal; uniform float4 _normal_ST;
            uniform float _refraction;
            uniform float _emission;
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
                float4 screenPos : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_6702 = 0.0;
                float4 node_3016 = _Time + _TimeEditor;
                float2 node_1039 = (i.uv0+(float2(0.0,(_Speed*0.1+0.0))*node_3016.g));
                float4 _normal_var = tex2D(_normal,TRANSFORM_TEX(node_1039, _normal));
                float node_5755 = 0.0;
                float4 _Cloud_var = tex2D(_Cloud,TRANSFORM_TEX(node_1039, _Cloud));
                float node_600 = 3.0;
                float4 _Up_mask_var = tex2D(_Up_mask,TRANSFORM_TEX(i.uv0, _Up_mask));
                float4 _Upper_mask_var = tex2D(_Upper_mask,TRANSFORM_TEX(i.uv0, _Upper_mask));
                float node_8468 = saturate(lerp(node_5755,lerp(_Opacity,lerp(_Opacity,node_5755,pow((_Cloud_var.r*node_600),node_600)),_Up_mask_var.r),_Upper_mask_var.r));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + lerp(float2(node_6702,node_6702),(float2(_normal_var.r,_normal_var.g)*_refraction),node_8468);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = _Color_copy.rgb;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * node_8468,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}