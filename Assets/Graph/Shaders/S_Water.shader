// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3228,x:33014,y:32677,varname:node_3228,prsc:2|diff-1871-OUT,spec-3261-OUT,gloss-9437-OUT,normal-458-OUT,emission-6477-OUT,amdfl-6065-OUT,alpha-1495-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3261,x:31898,y:32741,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_3261,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Color,id:5284,x:31844,y:32513,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_5284,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:9437,x:31898,y:32816,ptovrint:False,ptlb:Roughness,ptin:_Roughness,varname:node_9437,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_DepthBlend,id:5420,x:31489,y:33297,varname:node_5420,prsc:2|DIST-8835-OUT;n:type:ShaderForge.SFN_Lerp,id:7795,x:32190,y:32780,varname:node_7795,prsc:2|A-5284-RGB,B-6729-RGB,T-7818-OUT;n:type:ShaderForge.SFN_Color,id:6729,x:31844,y:32351,ptovrint:False,ptlb:Color_Depth,ptin:_Color_Depth,varname:node_6729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:8835,x:31327,y:33297,ptovrint:False,ptlb:Depth,ptin:_Depth,varname:node_8835,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:3475,x:31653,y:33297,varname:node_3475,prsc:2|A-5420-OUT,B-6982-OUT;n:type:ShaderForge.SFN_Vector1,id:6982,x:31489,y:33440,varname:node_6982,prsc:2,v1:5;n:type:ShaderForge.SFN_Power,id:2190,x:31822,y:33297,varname:node_2190,prsc:2|VAL-3475-OUT,EXP-7863-OUT;n:type:ShaderForge.SFN_Vector1,id:7863,x:31653,y:33440,varname:node_7863,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:7818,x:31996,y:33297,varname:node_7818,prsc:2|IN-2190-OUT;n:type:ShaderForge.SFN_Lerp,id:2263,x:32190,y:32980,varname:node_2263,prsc:2|A-7443-OUT,B-6632-OUT,T-7818-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7443,x:31898,y:32935,ptovrint:False,ptlb:Opacity1,ptin:_Opacity1,varname:node_7443,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:6632,x:31898,y:33020,ptovrint:False,ptlb:Opacity2,ptin:_Opacity2,varname:node_6632,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_DepthBlend,id:706,x:31485,y:33564,varname:node_706,prsc:2|DIST-6007-OUT;n:type:ShaderForge.SFN_Multiply,id:2192,x:31649,y:33564,varname:node_2192,prsc:2|A-706-OUT,B-7931-OUT;n:type:ShaderForge.SFN_Vector1,id:7931,x:31485,y:33707,varname:node_7931,prsc:2,v1:5;n:type:ShaderForge.SFN_Power,id:2227,x:31818,y:33564,varname:node_2227,prsc:2|VAL-2192-OUT,EXP-2370-OUT;n:type:ShaderForge.SFN_Vector1,id:2370,x:31649,y:33707,varname:node_2370,prsc:2,v1:10;n:type:ShaderForge.SFN_Clamp01,id:6387,x:31992,y:33564,varname:node_6387,prsc:2|IN-2227-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6007,x:31290,y:33564,ptovrint:False,ptlb:Ecume_depth,ptin:_Ecume_depth,varname:node_6007,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:1871,x:32388,y:32780,varname:node_1871,prsc:2|A-512-OUT,B-7795-OUT,T-6387-OUT;n:type:ShaderForge.SFN_Vector1,id:512,x:32190,y:32914,varname:node_512,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:1495,x:32388,y:33003,varname:node_1495,prsc:2|A-512-OUT,B-2263-OUT,T-6387-OUT;n:type:ShaderForge.SFN_Multiply,id:9886,x:32603,y:32780,varname:node_9886,prsc:2|A-1871-OUT,B-145-OUT;n:type:ShaderForge.SFN_Vector1,id:145,x:32388,y:32925,varname:node_145,prsc:2,v1:1;n:type:ShaderForge.SFN_Cubemap,id:7282,x:32561,y:32563,ptovrint:False,ptlb:Cubemap,ptin:_Cubemap,varname:node_7282,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:6383c55fe39edef44add52c306908971,pvfc:0;n:type:ShaderForge.SFN_Tex2d,id:3304,x:32747,y:33106,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_3304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-3237-UVOUT;n:type:ShaderForge.SFN_Panner,id:3237,x:32557,y:33148,varname:node_3237,prsc:2,spu:0,spv:-0.01|UVIN-2904-UVOUT,DIST-1771-OUT;n:type:ShaderForge.SFN_TexCoord,id:2904,x:32388,y:33148,varname:node_2904,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:6366,x:32201,y:33443,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_6366,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:6065,x:32793,y:32541,varname:node_6065,prsc:2|A-7282-RGB,B-3507-OUT;n:type:ShaderForge.SFN_Vector1,id:3507,x:32561,y:32478,varname:node_3507,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Time,id:8849,x:32201,y:33295,varname:node_8849,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1771,x:32388,y:33317,varname:node_1771,prsc:2|A-8849-T,B-6366-OUT;n:type:ShaderForge.SFN_Fresnel,id:5389,x:33059,y:32487,varname:node_5389,prsc:2|EXP-4855-OUT;n:type:ShaderForge.SFN_Add,id:6477,x:32794,y:32818,varname:node_6477,prsc:2|A-5389-OUT,B-9886-OUT;n:type:ShaderForge.SFN_Vector1,id:4855,x:32866,y:32441,varname:node_4855,prsc:2,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:7616,x:32730,y:33384,ptovrint:False,ptlb:Normal_intensity,ptin:_Normal_intensity,varname:node_7616,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:458,x:32964,y:33231,varname:node_458,prsc:2|A-3875-OUT,B-3304-RGB,T-7616-OUT;n:type:ShaderForge.SFN_Vector3,id:3875,x:32730,y:33267,varname:node_3875,prsc:2,v1:0,v2:0,v3:1;proporder:5284-3261-9437-6729-8835-7443-6632-6007-7282-3304-6366-7616;pass:END;sub:END;*/

Shader "Custom/Water" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Metallic ("Metallic", Float ) = 0
        _Roughness ("Roughness", Float ) = 0
        _Color_Depth ("Color_Depth", Color) = (0.5,0.5,0.5,1)
        _Depth ("Depth", Float ) = 0
        _Opacity1 ("Opacity1", Float ) = 0.5
        _Opacity2 ("Opacity2", Float ) = 1
        _Ecume_depth ("Ecume_depth", Float ) = 0
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _Normal ("Normal", 2D) = "bump" {}
        _Speed ("Speed", Float ) = 1
        _Normal_intensity ("Normal_intensity", Float ) = 0
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _Metallic;
            uniform float4 _Color;
            uniform float _Roughness;
            uniform float4 _Color_Depth;
            uniform float _Depth;
            uniform float _Opacity1;
            uniform float _Opacity2;
            uniform float _Ecume_depth;
            uniform samplerCUBE _Cubemap;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Speed;
            uniform float _Normal_intensity;
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
                float4 projPos : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
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
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_8849 = _Time + _TimeEditor;
                float2 node_3237 = (i.uv0+(node_8849.g*_Speed)*float2(0,-0.01));
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_3237, _Normal)));
                float3 normalLocal = lerp(float3(0,0,1),_Normal_var.rgb,_Normal_intensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _Roughness; // Convert roughness to gloss
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
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float node_512 = 1.0;
                float node_7818 = saturate(pow((saturate((sceneZ-partZ)/_Depth)*5.0),1.0));
                float node_6387 = saturate(pow((saturate((sceneZ-partZ)/_Ecume_depth)*5.0),10.0));
                float3 node_1871 = lerp(float3(node_512,node_512,node_512),lerp(_Color.rgb,_Color_Depth.rgb,node_7818),node_6387);
                float3 diffuseColor = node_1871; // Need this for specular when using metallic
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
                indirectDiffuse += (texCUBE(_Cubemap,viewReflectDirection).rgb*0.5); // Diffuse Ambient Light
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = (pow(1.0-max(0,dot(normalDirection, viewDirection)),4.0)+(node_1871*1.0));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,lerp(node_512,lerp(_Opacity1,_Opacity2,node_7818),node_6387));
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float _Metallic;
            uniform float4 _Color;
            uniform float _Roughness;
            uniform float4 _Color_Depth;
            uniform float _Depth;
            uniform float _Opacity1;
            uniform float _Opacity2;
            uniform float _Ecume_depth;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Speed;
            uniform float _Normal_intensity;
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
                float4 projPos : TEXCOORD7;
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
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_8849 = _Time + _TimeEditor;
                float2 node_3237 = (i.uv0+(node_8849.g*_Speed)*float2(0,-0.01));
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_3237, _Normal)));
                float3 normalLocal = lerp(float3(0,0,1),_Normal_var.rgb,_Normal_intensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _Roughness; // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float node_512 = 1.0;
                float node_7818 = saturate(pow((saturate((sceneZ-partZ)/_Depth)*5.0),1.0));
                float node_6387 = saturate(pow((saturate((sceneZ-partZ)/_Ecume_depth)*5.0),10.0));
                float3 node_1871 = lerp(float3(node_512,node_512,node_512),lerp(_Color.rgb,_Color_Depth.rgb,node_7818),node_6387);
                float3 diffuseColor = node_1871; // Need this for specular when using metallic
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
                fixed4 finalRGBA = fixed4(finalColor * lerp(node_512,lerp(_Opacity1,_Opacity2,node_7818),node_6387),0);
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
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
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float _Metallic;
            uniform float4 _Color;
            uniform float _Roughness;
            uniform float4 _Color_Depth;
            uniform float _Depth;
            uniform float _Ecume_depth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 projPos : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_512 = 1.0;
                float node_7818 = saturate(pow((saturate((sceneZ-partZ)/_Depth)*5.0),1.0));
                float node_6387 = saturate(pow((saturate((sceneZ-partZ)/_Ecume_depth)*5.0),10.0));
                float3 node_1871 = lerp(float3(node_512,node_512,node_512),lerp(_Color.rgb,_Color_Depth.rgb,node_7818),node_6387);
                o.Emission = (pow(1.0-max(0,dot(normalDirection, viewDirection)),4.0)+(node_1871*1.0));
                
                float3 diffColor = node_1871;
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = _Roughness;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
