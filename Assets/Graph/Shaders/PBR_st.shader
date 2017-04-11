// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:1,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:32765,y:32329,varname:node_2865,prsc:2|diff-6343-OUT,spec-8506-R,gloss-6860-R,normal-5964-RGB,emission-3036-OUT,amdfl-7148-OUT,disp-4241-OUT,tess-2066-OUT;n:type:ShaderForge.SFN_Multiply,id:6343,x:32117,y:32667,varname:node_6343,prsc:2|A-7736-RGB,B-6665-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:31921,y:32805,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32620,ptovrint:True,ptlb:Base Color,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5089-OUT;n:type:ShaderForge.SFN_Tex2d,id:5964,x:32274,y:33154,ptovrint:True,ptlb:Normal Map,ptin:_BumpMap,varname:_BumpMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-5089-OUT;n:type:ShaderForge.SFN_Tex2d,id:8506,x:32274,y:32806,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_8506,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5089-OUT;n:type:ShaderForge.SFN_Tex2d,id:6860,x:32274,y:32979,ptovrint:False,ptlb:Roughness,ptin:_Roughness,varname:node_6860,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5089-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1818,x:33575,y:32955,ptovrint:False,ptlb:Tesselation,ptin:_Tesselation,varname:node_1818,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:3661,x:31472,y:33458,ptovrint:False,ptlb:Displacement,ptin:_Displacement,varname:node_3661,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5089-OUT;n:type:ShaderForge.SFN_Multiply,id:5568,x:31724,y:33437,varname:node_5568,prsc:2|A-1544-OUT,B-7184-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7184,x:31472,y:33647,ptovrint:False,ptlb:Displace_intensity,ptin:_Displace_intensity,varname:node_7184,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:4241,x:31949,y:33437,varname:node_4241,prsc:2|A-5568-OUT,B-6276-OUT;n:type:ShaderForge.SFN_NormalVector,id:6276,x:31724,y:33572,prsc:2,pt:False;n:type:ShaderForge.SFN_Lerp,id:1612,x:31618,y:31664,varname:node_1612,prsc:2|A-1513-RGB,B-821-OUT,T-8983-OUT;n:type:ShaderForge.SFN_Color,id:1513,x:31268,y:31516,ptovrint:False,ptlb:Shadow_color,ptin:_Shadow_color,varname:node_1513,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Vector1,id:821,x:31268,y:31664,varname:node_821,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:3665,x:30606,y:32746,varname:node_3665,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:5089,x:31222,y:32901,varname:node_5089,prsc:2|A-3701-OUT,B-736-OUT;n:type:ShaderForge.SFN_ValueProperty,id:736,x:30624,y:32680,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:node_736,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Power,id:8983,x:31453,y:31738,varname:node_8983,prsc:2|VAL-240-OUT,EXP-6491-OUT;n:type:ShaderForge.SFN_Vector1,id:6491,x:31268,y:31875,varname:node_6491,prsc:2,v1:1;n:type:ShaderForge.SFN_Fresnel,id:4905,x:31272,y:32140,varname:node_4905,prsc:2|EXP-5032-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5032,x:31082,y:32151,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_5032,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:295,x:31470,y:32140,varname:node_295,prsc:2|A-4905-OUT,B-7567-OUT;n:type:ShaderForge.SFN_Vector1,id:7567,x:31272,y:32076,varname:node_7567,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Lerp,id:9357,x:31655,y:32101,varname:node_9357,prsc:2|A-821-OUT,B-5489-RGB,T-295-OUT;n:type:ShaderForge.SFN_LightColor,id:2119,x:32729,y:32178,varname:node_2119,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:8399,x:31830,y:32101,varname:node_8399,prsc:2|IN-9357-OUT;n:type:ShaderForge.SFN_Multiply,id:7148,x:32928,y:32308,varname:node_7148,prsc:2|A-2119-RGB,B-5579-OUT;n:type:ShaderForge.SFN_Vector1,id:5579,x:32753,y:32329,varname:node_5579,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Add,id:9439,x:32041,y:31825,varname:node_9439,prsc:2|A-1612-OUT,B-8399-OUT;n:type:ShaderForge.SFN_Vector1,id:240,x:31268,y:31782,varname:node_240,prsc:2,v1:0;n:type:ShaderForge.SFN_Color,id:5489,x:31709,y:31943,ptovrint:False,ptlb:Fresnel_color,ptin:_Fresnel_color,varname:node_5489,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ViewPosition,id:2265,x:33213,y:33228,varname:node_2265,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:3721,x:33213,y:33087,varname:node_3721,prsc:2;n:type:ShaderForge.SFN_Distance,id:68,x:33423,y:33159,varname:node_68,prsc:2|A-3721-XYZ,B-2265-XYZ;n:type:ShaderForge.SFN_Divide,id:3572,x:33595,y:33159,varname:node_3572,prsc:2|A-68-OUT,B-1270-OUT;n:type:ShaderForge.SFN_Power,id:8694,x:33769,y:33159,varname:node_8694,prsc:2|VAL-3572-OUT,EXP-2953-OUT;n:type:ShaderForge.SFN_Clamp01,id:467,x:33937,y:33159,varname:node_467,prsc:2|IN-8694-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1270,x:33412,y:33314,ptovrint:False,ptlb:Tansition_distance,ptin:_Tansition_distance,varname:node_1270,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4.5;n:type:ShaderForge.SFN_ValueProperty,id:2953,x:33595,y:33314,ptovrint:False,ptlb:Falloff,ptin:_Falloff,varname:node_2953,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:500;n:type:ShaderForge.SFN_Lerp,id:2066,x:34105,y:33156,varname:node_2066,prsc:2|A-1818-OUT,B-2017-OUT,T-467-OUT;n:type:ShaderForge.SFN_Vector1,id:2017,x:33575,y:33012,varname:node_2017,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:4123,x:30852,y:32901,varname:node_4123,prsc:2|A-3665-U,B-3389-OUT;n:type:ShaderForge.SFN_Divide,id:2346,x:30429,y:32937,varname:node_2346,prsc:2|A-246-OUT,B-6070-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6070,x:30218,y:33022,ptovrint:False,ptlb:Initial_scale,ptin:_Initial_scale,varname:node_6070,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Append,id:3701,x:31026,y:32901,varname:node_3701,prsc:2|A-4123-OUT,B-3665-V;n:type:ShaderForge.SFN_ValueProperty,id:246,x:30218,y:32937,ptovrint:False,ptlb:X_scale,ptin:_X_scale,varname:node_246,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_SwitchProperty,id:3389,x:30606,y:32919,ptovrint:False,ptlb:Scale_rectification,ptin:_Scale_rectification,varname:node_3389,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8298-OUT,B-2346-OUT;n:type:ShaderForge.SFN_Vector1,id:8298,x:30218,y:33088,varname:node_8298,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:4917,x:34105,y:33361,varname:node_4917,prsc:2|A-5691-OUT,B-9322-OUT,T-467-OUT;n:type:ShaderForge.SFN_Vector3,id:5691,x:33861,y:33411,varname:node_5691,prsc:2,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Vector3,id:9322,x:33872,y:33504,varname:node_9322,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Add,id:5760,x:32322,y:32010,varname:node_5760,prsc:2|A-20-OUT,B-9439-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:20,x:32145,y:32010,ptovrint:False,ptlb:Visualize_Tesselation,ptin:_Visualize_Tesselation,varname:node_20,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-240-OUT,B-4917-OUT;n:type:ShaderForge.SFN_Multiply,id:1544,x:32356,y:33584,varname:node_1544,prsc:2|A-3661-RGB,B-7997-OUT;n:type:ShaderForge.SFN_Lerp,id:7997,x:32154,y:33621,varname:node_7997,prsc:2|A-5236-OUT,B-5195-OUT,T-467-OUT;n:type:ShaderForge.SFN_Vector1,id:5236,x:31971,y:33597,varname:node_5236,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:5195,x:31971,y:33655,varname:node_5195,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:3036,x:32617,y:31817,varname:node_3036,prsc:2|A-5760-OUT,B-9847-OUT;n:type:ShaderForge.SFN_Color,id:4821,x:32149,y:31511,ptovrint:False,ptlb:Emissivge_color,ptin:_Emissivge_color,varname:node_4821,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9847,x:32364,y:31577,varname:node_9847,prsc:2|A-4821-RGB,B-3121-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3121,x:32149,y:31692,ptovrint:False,ptlb:Emissive_intensity,ptin:_Emissive_intensity,varname:node_3121,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:736-7736-6665-5964-8506-6860-1818-3661-7184-1270-2953-1513-5032-5489-3389-6070-246-20-4821-3121;pass:END;sub:END;*/

Shader "Shader Forge/PBR_st" {
    Properties {
        _Tiling ("Tiling", Float ) = 1
        _MainTex ("Base Color", 2D) = "white" {}
        _Color ("Color", Color) = (0.5019608,0.5019608,0.5019608,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Metallic ("Metallic", 2D) = "white" {}
        _Roughness ("Roughness", 2D) = "white" {}
        _Tesselation ("Tesselation", Float ) = 0
        _Displacement ("Displacement", 2D) = "white" {}
        _Displace_intensity ("Displace_intensity", Float ) = 0
        _Tansition_distance ("Tansition_distance", Float ) = 4.5
        _Falloff ("Falloff", Float ) = 500
        _Shadow_color ("Shadow_color", Color) = (1,0,0,1)
        _Fresnel ("Fresnel", Float ) = 0
        _Fresnel_color ("Fresnel_color", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _Scale_rectification ("Scale_rectification", Float ) = 1
        _Initial_scale ("Initial_scale", Float ) = 10
        _X_scale ("X_scale", Float ) = 0
        [MaterialToggle] _Visualize_Tesselation ("Visualize_Tesselation", Float ) = 0
        _Emissivge_color ("Emissivge_color", Color) = (0.5,0.5,0.5,1)
        _Emissive_intensity ("Emissive_intensity", Float ) = 0
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
            uniform float _Tesselation;
            uniform sampler2D _Displacement; uniform float4 _Displacement_ST;
            uniform float _Displace_intensity;
            uniform float4 _Shadow_color;
            uniform float _Tiling;
            uniform float _Fresnel;
            uniform float4 _Fresnel_color;
            uniform float _Tansition_distance;
            uniform float _Falloff;
            uniform float _Initial_scale;
            uniform float _X_scale;
            uniform fixed _Scale_rectification;
            uniform fixed _Visualize_Tesselation;
            uniform float4 _Emissivge_color;
            uniform float _Emissive_intensity;
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
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
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
                void displacement (inout VertexInput v){
                    float2 node_5089 = (float2((v.texcoord0.r*lerp( 1.0, (_X_scale/_Initial_scale), _Scale_rectification )),v.texcoord0.g)*_Tiling);
                    float4 _Displacement_var = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_5089, _Displacement),0.0,0));
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    v.vertex.xyz += (((_Displacement_var.rgb*lerp(1.0,0.0,node_467))*_Displace_intensity)*v.normal);
                }
                float Tessellation(TessVertex v){
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    return lerp(_Tesselation,0.01,node_467);
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
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_5089 = (float2((i.uv0.r*lerp( 1.0, (_X_scale/_Initial_scale), _Scale_rectification )),i.uv0.g)*_Tiling);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_5089, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
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
                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
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
                float node_240 = 0.0;
                float node_467 = saturate(pow((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                float node_821 = 0.0;
                float3 emissive = ((lerp( node_240, lerp(float3(1,0,0),float3(0,0,1),node_467), _Visualize_Tesselation )+(lerp(_Shadow_color.rgb,float3(node_821,node_821,node_821),pow(node_240,1.0))+saturate(lerp(float3(node_821,node_821,node_821),_Fresnel_color.rgb,(pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*0.2)))))+(_Emissivge_color.rgb*_Emissive_intensity));
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
            uniform float _Tesselation;
            uniform sampler2D _Displacement; uniform float4 _Displacement_ST;
            uniform float _Displace_intensity;
            uniform float4 _Shadow_color;
            uniform float _Tiling;
            uniform float _Fresnel;
            uniform float4 _Fresnel_color;
            uniform float _Tansition_distance;
            uniform float _Falloff;
            uniform float _Initial_scale;
            uniform float _X_scale;
            uniform fixed _Scale_rectification;
            uniform fixed _Visualize_Tesselation;
            uniform float4 _Emissivge_color;
            uniform float _Emissive_intensity;
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
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
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
                void displacement (inout VertexInput v){
                    float2 node_5089 = (float2((v.texcoord0.r*lerp( 1.0, (_X_scale/_Initial_scale), _Scale_rectification )),v.texcoord0.g)*_Tiling);
                    float4 _Displacement_var = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_5089, _Displacement),0.0,0));
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    v.vertex.xyz += (((_Displacement_var.rgb*lerp(1.0,0.0,node_467))*_Displace_intensity)*v.normal);
                }
                float Tessellation(TessVertex v){
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    return lerp(_Tesselation,0.01,node_467);
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
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_5089 = (float2((i.uv0.r*lerp( 1.0, (_X_scale/_Initial_scale), _Scale_rectification )),i.uv0.g)*_Tiling);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_5089, _BumpMap)));
                float3 normalLocal = _BumpMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
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
                float3 diffuseColor = (_MainTex_var.rgb*_Color.rgb); // Need this for specular when using metallic
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
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float _Tesselation;
            uniform sampler2D _Displacement; uniform float4 _Displacement_ST;
            uniform float _Displace_intensity;
            uniform float _Tiling;
            uniform float _Tansition_distance;
            uniform float _Falloff;
            uniform float _Initial_scale;
            uniform float _X_scale;
            uniform fixed _Scale_rectification;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
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
                void displacement (inout VertexInput v){
                    float2 node_5089 = (float2((v.texcoord0.r*lerp( 1.0, (_X_scale/_Initial_scale), _Scale_rectification )),v.texcoord0.g)*_Tiling);
                    float4 _Displacement_var = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_5089, _Displacement),0.0,0));
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    v.vertex.xyz += (((_Displacement_var.rgb*lerp(1.0,0.0,node_467))*_Displace_intensity)*v.normal);
                }
                float Tessellation(TessVertex v){
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    return lerp(_Tesselation,0.01,node_467);
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
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
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
            uniform float _Tesselation;
            uniform sampler2D _Displacement; uniform float4 _Displacement_ST;
            uniform float _Displace_intensity;
            uniform float4 _Shadow_color;
            uniform float _Tiling;
            uniform float _Fresnel;
            uniform float4 _Fresnel_color;
            uniform float _Tansition_distance;
            uniform float _Falloff;
            uniform float _Initial_scale;
            uniform float _X_scale;
            uniform fixed _Scale_rectification;
            uniform fixed _Visualize_Tesselation;
            uniform float4 _Emissivge_color;
            uniform float _Emissive_intensity;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
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
                void displacement (inout VertexInput v){
                    float2 node_5089 = (float2((v.texcoord0.r*lerp( 1.0, (_X_scale/_Initial_scale), _Scale_rectification )),v.texcoord0.g)*_Tiling);
                    float4 _Displacement_var = tex2Dlod(_Displacement,float4(TRANSFORM_TEX(node_5089, _Displacement),0.0,0));
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    v.vertex.xyz += (((_Displacement_var.rgb*lerp(1.0,0.0,node_467))*_Displace_intensity)*v.normal);
                }
                float Tessellation(TessVertex v){
                    float node_467 = saturate(pow((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                    return lerp(_Tesselation,0.01,node_467);
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
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_240 = 0.0;
                float node_467 = saturate(pow((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_Tansition_distance),_Falloff));
                float node_821 = 0.0;
                o.Emission = ((lerp( node_240, lerp(float3(1,0,0),float3(0,0,1),node_467), _Visualize_Tesselation )+(lerp(_Shadow_color.rgb,float3(node_821,node_821,node_821),pow(node_240,1.0))+saturate(lerp(float3(node_821,node_821,node_821),_Fresnel_color.rgb,(pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)*0.2)))))+(_Emissivge_color.rgb*_Emissive_intensity));
                
                float2 node_5089 = (float2((i.uv0.r*lerp( 1.0, (_X_scale/_Initial_scale), _Scale_rectification )),i.uv0.g)*_Tiling);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_5089, _MainTex));
                float3 diffColor = (_MainTex_var.rgb*_Color.rgb);
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
