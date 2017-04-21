// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:1,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:32767,y:32498,varname:node_2865,prsc:2|diff-741-OUT,spec-8506-R,gloss-6860-R,normal-5964-RGB,emission-9439-OUT,amdfl-7148-OUT,tess-1159-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32117,y:32667,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31921,y:32805,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32620,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5089-OUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32274,y:33154,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-5089-OUT;n:type:ShaderForge.SFN_Tex2d,id:8506,x:32274,y:32806,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_8506,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5089-OUT;n:type:ShaderForge.SFN_Tex2d,id:6860,x:32274,y:32979,ptovrint:False,ptlb:Roughness,ptin:_Roughness,varname:node_6860,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5089-OUT;n:type:ShaderForge.SFN_Lerp,id:1612,x:31618,y:31664,varname:node_1612,prsc:2|A-451-OUT,B-821-OUT,T-8983-OUT;n:type:ShaderForge.SFN_Color,id:1513,x:31268,y:31516,ptovrint:False,ptlb:Shadow_color,ptin:_Shadow_color,varname:node_1513,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Vector1,id:821,x:31268,y:31664,varname:node_821,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:3665,x:30639,y:32765,varname:node_3665,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:5089,x:31222,y:32901,varname:node_5089,prsc:2|A-3665-UVOUT,B-736-OUT;n:type:ShaderForge.SFN_ValueProperty,id:736,x:30624,y:32680,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:node_736,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Power,id:8983,x:31453,y:31738,varname:node_8983,prsc:2|VAL-240-OUT,EXP-6491-OUT;n:type:ShaderForge.SFN_Vector1,id:6491,x:31268,y:31875,varname:node_6491,prsc:2,v1:1;n:type:ShaderForge.SFN_Fresnel,id:4905,x:31272,y:32140,varname:node_4905,prsc:2|EXP-5032-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5032,x:31082,y:32151,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_5032,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:295,x:31470,y:32140,varname:node_295,prsc:2|A-4905-OUT,B-7567-OUT;n:type:ShaderForge.SFN_Vector1,id:7567,x:31272,y:32076,varname:node_7567,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Lerp,id:9357,x:31655,y:32101,varname:node_9357,prsc:2|A-821-OUT,B-5489-RGB,T-295-OUT;n:type:ShaderForge.SFN_LightColor,id:2119,x:32732,y:31973,varname:node_2119,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:8399,x:31830,y:32101,varname:node_8399,prsc:2|IN-9357-OUT;n:type:ShaderForge.SFN_Multiply,id:7148,x:32948,y:32071,varname:node_7148,prsc:2|A-2119-RGB,B-5579-OUT;n:type:ShaderForge.SFN_Vector1,id:5579,x:32718,y:32122,varname:node_5579,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Add,id:9439,x:32041,y:31825,varname:node_9439,prsc:2|A-1612-OUT,B-8399-OUT;n:type:ShaderForge.SFN_Vector1,id:240,x:31268,y:31782,varname:node_240,prsc:2,v1:0;n:type:ShaderForge.SFN_Color,id:5489,x:31709,y:31943,ptovrint:False,ptlb:Fresnel_color,ptin:_Fresnel_color,varname:node_5489,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:1159,x:32682,y:32871,varname:node_1159,prsc:2,v1:0.001;n:type:ShaderForge.SFN_HsvToRgb,id:741,x:32492,y:32443,varname:node_741,prsc:2|H-1663-OUT,S-4577-OUT,V-6773-OUT;n:type:ShaderForge.SFN_RgbToHsv,id:5344,x:32180,y:32446,varname:node_5344,prsc:2|IN-6343-OUT;n:type:ShaderForge.SFN_Add,id:1663,x:32340,y:32282,varname:node_1663,prsc:2|A-6750-OUT,B-5344-HOUT;n:type:ShaderForge.SFN_ValueProperty,id:6750,x:32159,y:32282,ptovrint:False,ptlb:Hue,ptin:_Hue,varname:node_6750,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:4577,x:32340,y:32158,varname:node_4577,prsc:2|A-9211-OUT,B-5344-SOUT;n:type:ShaderForge.SFN_Add,id:6773,x:32340,y:32035,varname:node_6773,prsc:2|A-5701-OUT,B-5344-VOUT;n:type:ShaderForge.SFN_ValueProperty,id:9211,x:32159,y:32158,ptovrint:False,ptlb:Sat,ptin:_Sat,varname:node_9211,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:5701,x:32159,y:32035,ptovrint:False,ptlb:Val,ptin:_Val,varname:node_5701,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:451,x:31453,y:31529,varname:node_451,prsc:2|A-1513-RGB,B-7605-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1687,x:31279,y:31438,ptovrint:False,ptlb:SHadow_multiplier,ptin:_SHadow_multiplier,varname:node_1687,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:495,x:31454,y:31250,varname:node_495,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRange,id:801,x:31625,y:31250,varname:node_801,prsc:2,frmn:0,frmx:1,tomn:0,tomx:1|IN-1024-OUT;n:type:ShaderForge.SFN_Multiply,id:6304,x:31803,y:31250,varname:node_6304,prsc:2|A-801-OUT,B-801-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2683,x:31980,y:31250,varname:node_2683,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6304-OUT;n:type:ShaderForge.SFN_Add,id:536,x:32153,y:31250,varname:node_536,prsc:2|A-2683-R,B-2683-G;n:type:ShaderForge.SFN_ScreenPos,id:863,x:31284,y:30859,varname:node_863,prsc:2,sctp:2;n:type:ShaderForge.SFN_ValueProperty,id:1401,x:31249,y:31158,ptovrint:False,ptlb:Transition,ptin:_Transition,varname:node_1401,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:7605,x:32554,y:31273,varname:node_7605,prsc:2|A-1687-OUT,B-2818-OUT,T-9992-OUT;n:type:ShaderForge.SFN_Vector1,id:2818,x:32326,y:31198,varname:node_2818,prsc:2,v1:0;n:type:ShaderForge.SFN_Power,id:1326,x:32347,y:31421,varname:node_1326,prsc:2|VAL-3264-OUT,EXP-9415-OUT;n:type:ShaderForge.SFN_Clamp01,id:9992,x:32550,y:31100,varname:node_9992,prsc:2|IN-1326-OUT;n:type:ShaderForge.SFN_Multiply,id:3264,x:32129,y:31054,varname:node_3264,prsc:2|A-536-OUT,B-3779-OUT;n:type:ShaderForge.SFN_Vector1,id:3779,x:31801,y:31042,varname:node_3779,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1024,x:31594,y:30982,varname:node_1024,prsc:2|A-1122-OUT,B-1401-OUT;n:type:ShaderForge.SFN_Vector1,id:9415,x:32153,y:31433,varname:node_9415,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Add,id:1122,x:31487,y:30869,varname:node_1122,prsc:2|A-863-UVOUT,B-1139-OUT;n:type:ShaderForge.SFN_Vector2,id:1139,x:31284,y:30989,varname:node_1139,prsc:2,v1:-0.5,v2:-0.5;proporder:736-7736-6665-5964-8506-6860-1513-5032-5489-6750-9211-5701-1687-1401;pass:END;sub:END;*/

Shader "Shader Forge/PBR_st_notess" {
    Properties {
        _Tiling ("Tiling", Float ) = 1
        _MainTex ("Base Color", 2D) = "white" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Metallic ("Metallic", 2D) = "white" {}
        _Roughness ("Roughness", 2D) = "white" {}
        _Shadow_color ("Shadow_color", Color) = (1,0,0,1)
        _Fresnel ("Fresnel", Float ) = 0
        _Fresnel_color ("Fresnel_color", Color) = (0.5,0.5,0.5,1)
        _Hue ("Hue", Float ) = 0
        _Sat ("Sat", Float ) = 0
        _Val ("Val", Float ) = 0
        _SHadow_multiplier ("SHadow_multiplier", Float ) = 1
        _Transition ("Transition", Float ) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "DisableBatching"="True"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _Metallic; uniform float4 _Metallic_ST;
            uniform sampler2D _Roughness; uniform float4 _Roughness_ST;
            uniform float4 _Shadow_color;
            uniform float _Tiling;
            uniform float _Fresnel;
            uniform float4 _Fresnel_color;
            uniform float _Hue;
            uniform float _Sat;
            uniform float _Val;
            uniform float _SHadow_multiplier;
            uniform float _Transition;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD11;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return 0.001;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_5089 = (i.uv0*_Tiling);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_5089, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _Roughness_var = tex2D(_Roughness,TRANSFORM_TEX(node_5089, _Roughness));
                float gloss = 1.0 - _Roughness_var.r; // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _Metallic_var = tex2D(_Metallic,TRANSFORM_TEX(node_5089, _Metallic));
                float3 specularColor = _Metallic_var.r;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5089, _MainTex));
                float3 node_6343 = (_MainTex_var.rgb*_Color.rgb);
                float4 node_5344_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_5344_p = lerp(float4(float4(node_6343,0.0).zy, node_5344_k.wz), float4(float4(node_6343,0.0).yz, node_5344_k.xy), step(float4(node_6343,0.0).z, float4(node_6343,0.0).y));
                float4 node_5344_q = lerp(float4(node_5344_p.xyw, float4(node_6343,0.0).x), float4(float4(node_6343,0.0).x, node_5344_p.yzx), step(node_5344_p.x, float4(node_6343,0.0).x));
                float node_5344_d = node_5344_q.x - min(node_5344_q.w, node_5344_q.y);
                float node_5344_e = 1.0e-10;
                float3 node_5344 = float3(abs(node_5344_q.z + (node_5344_q.w - node_5344_q.y) / (6.0 * node_5344_d + node_5344_e)), node_5344_d / (node_5344_q.x + node_5344_e), node_5344_q.x);;
                float3 diffuseColor = (lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac((_Hue+node_5344.r)+float3(0.0,-1.0/3.0,1.0/3.0)))-1),(_Sat+node_5344.g))*(_Val+node_5344.b)); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz)*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += (_LightColor0.rgb*0.2); // Diffuse Ambient Light
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float2 node_801 = (((sceneUVs.rg+float2(-0.5,-0.5))*_Transition)*1.0+0.0);
                float2 node_2683 = (node_801*node_801).rg;
                float node_821 = 0.0;
                float3 emissive = (lerp((_Shadow_color.rgb*lerp(_SHadow_multiplier,0.0,saturate(pow(((node_2683.r+node_2683.g)*1.0),0.5)))),float3(node_821,node_821,node_821),pow(0.0,1.0))+saturate(lerp(float3(node_821,node_821,node_821),_Fresnel_color.rgb,(pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*0.2))));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
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
            
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform sampler2D _Metallic; uniform float4 _Metallic_ST;
            uniform sampler2D _Roughness; uniform float4 _Roughness_ST;
            uniform float4 _Shadow_color;
            uniform float _Tiling;
            uniform float _Fresnel;
            uniform float4 _Fresnel_color;
            uniform float _Hue;
            uniform float _Sat;
            uniform float _Val;
            uniform float _SHadow_multiplier;
            uniform float _Transition;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 screenPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return 0.001;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_5089 = (i.uv0*_Tiling);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_5089, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _Roughness_var = tex2D(_Roughness,TRANSFORM_TEX(node_5089, _Roughness));
                float gloss = 1.0 - _Roughness_var.r; // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _Metallic_var = tex2D(_Metallic,TRANSFORM_TEX(node_5089, _Metallic));
                float3 specularColor = _Metallic_var.r;
                float specularMonochrome;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5089, _MainTex));
                float3 node_6343 = (_MainTex_var.rgb*_Color.rgb);
                float4 node_5344_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_5344_p = lerp(float4(float4(node_6343,0.0).zy, node_5344_k.wz), float4(float4(node_6343,0.0).yz, node_5344_k.xy), step(float4(node_6343,0.0).z, float4(node_6343,0.0).y));
                float4 node_5344_q = lerp(float4(node_5344_p.xyw, float4(node_6343,0.0).x), float4(float4(node_6343,0.0).x, node_5344_p.yzx), step(node_5344_p.x, float4(node_6343,0.0).x));
                float node_5344_d = node_5344_q.x - min(node_5344_q.w, node_5344_q.y);
                float node_5344_e = 1.0e-10;
                float3 node_5344 = float3(abs(node_5344_q.z + (node_5344_q.w - node_5344_q.y) / (6.0 * node_5344_d + node_5344_e)), node_5344_d / (node_5344_q.x + node_5344_e), node_5344_q.x);;
                float3 diffuseColor = (lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac((_Hue+node_5344.r)+float3(0.0,-1.0/3.0,1.0/3.0)))-1),(_Sat+node_5344.g))*(_Val+node_5344.b)); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Metallic; uniform float4 _Metallic_ST;
            uniform sampler2D _Roughness; uniform float4 _Roughness_ST;
            uniform float4 _Shadow_color;
            uniform float _Tiling;
            uniform float _Fresnel;
            uniform float4 _Fresnel_color;
            uniform float _Hue;
            uniform float _Sat;
            uniform float _Val;
            uniform float _SHadow_multiplier;
            uniform float _Transition;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.screenPos = o.pos;
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return 0.001;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : SV_Target {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float2 node_801 = (((sceneUVs.rg+float2(-0.5,-0.5))*_Transition)*1.0+0.0);
                float2 node_2683 = (node_801*node_801).rg;
                float node_821 = 0.0;
                o.Emission = (lerp((_Shadow_color.rgb*lerp(_SHadow_multiplier,0.0,saturate(pow(((node_2683.r+node_2683.g)*1.0),0.5)))),float3(node_821,node_821,node_821),pow(0.0,1.0))+saturate(lerp(float3(node_821,node_821,node_821),_Fresnel_color.rgb,(pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*0.2))));
                
                float2 node_5089 = (i.uv0*_Tiling);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5089, _MainTex));
                float3 node_6343 = (_MainTex_var.rgb*_Color.rgb);
                float4 node_5344_k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 node_5344_p = lerp(float4(float4(node_6343,0.0).zy, node_5344_k.wz), float4(float4(node_6343,0.0).yz, node_5344_k.xy), step(float4(node_6343,0.0).z, float4(node_6343,0.0).y));
                float4 node_5344_q = lerp(float4(node_5344_p.xyw, float4(node_6343,0.0).x), float4(float4(node_6343,0.0).x, node_5344_p.yzx), step(node_5344_p.x, float4(node_6343,0.0).x));
                float node_5344_d = node_5344_q.x - min(node_5344_q.w, node_5344_q.y);
                float node_5344_e = 1.0e-10;
                float3 node_5344 = float3(abs(node_5344_q.z + (node_5344_q.w - node_5344_q.y) / (6.0 * node_5344_d + node_5344_e)), node_5344_d / (node_5344_q.x + node_5344_e), node_5344_q.x);;
                float3 diffColor = (lerp(float3(1,1,1),saturate(3.0*abs(1.0-2.0*frac((_Hue+node_5344.r)+float3(0.0,-1.0/3.0,1.0/3.0)))-1),(_Sat+node_5344.g))*(_Val+node_5344.b));
                float specularMonochrome;
                float3 specColor;
                float4 _Metallic_var = tex2D(_Metallic,TRANSFORM_TEX(node_5089, _Metallic));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic_var.r, specColor, specularMonochrome );
                float4 _Roughness_var = tex2D(_Roughness,TRANSFORM_TEX(node_5089, _Roughness));
                float roughness = _Roughness_var.r;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
