// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5161,x:32719,y:32712,varname:node_5161,prsc:2|diff-1323-OUT,normal-5206-OUT;n:type:ShaderForge.SFN_Tex2d,id:9799,x:32217,y:32607,ptovrint:False,ptlb:Mousse,ptin:_Mousse,varname:node_9799,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:976f15af08ad2164d9bc4d72882ed52e,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:570,x:31465,y:32551,ptovrint:False,ptlb:Rock,ptin:_Rock,varname:node_570,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ba510c00b0789674ab6850de773f469c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:152,x:31312,y:32910,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_152,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:1323,x:32528,y:32751,varname:node_1323,prsc:2|A-9799-RGB,B-570-RGB,T-653-OUT;n:type:ShaderForge.SFN_Slider,id:2717,x:31145,y:32783,ptovrint:False,ptlb:Power double,ptin:_Powerdouble,varname:node_2717,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2889535,max:1;n:type:ShaderForge.SFN_Add,id:938,x:31694,y:32805,varname:node_938,prsc:2|A-2717-OUT,B-152-RGB;n:type:ShaderForge.SFN_Clamp01,id:6336,x:31873,y:32805,varname:node_6336,prsc:2|IN-938-OUT;n:type:ShaderForge.SFN_Power,id:9225,x:32059,y:32887,varname:node_9225,prsc:2|VAL-6336-OUT,EXP-2087-OUT;n:type:ShaderForge.SFN_Slider,id:2087,x:31572,y:32969,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_2087,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.442639,max:5;n:type:ShaderForge.SFN_Blend,id:653,x:32320,y:32841,varname:node_653,prsc:2,blmd:10,clmp:True|SRC-570-RGB,DST-9225-OUT;n:type:ShaderForge.SFN_Lerp,id:5206,x:32441,y:33094,varname:node_5206,prsc:2|A-4904-RGB,B-5640-RGB,T-4858-OUT;n:type:ShaderForge.SFN_Blend,id:4858,x:32269,y:33196,varname:node_4858,prsc:2,blmd:10,clmp:True|SRC-5640-RGB,DST-9225-OUT;n:type:ShaderForge.SFN_Tex2d,id:4904,x:32020,y:33042,ptovrint:False,ptlb:Mousse_N,ptin:_Mousse_N,varname:node_4904,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:7a0afabecb6c49644aba88c9b044e68b,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:5640,x:31666,y:33176,ptovrint:False,ptlb:Rocks_Normal,ptin:_Rocks_Normal,varname:node_5640,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3dc6e45a04329194b994dbeb1136a9c5,ntxv:3,isnm:True;proporder:9799-570-152-2717-2087-4904-5640;pass:END;sub:END;*/

Shader "Custom/Sol_Shader" {
    Properties {
        _Mousse ("Mousse", 2D) = "white" {}
        _Rock ("Rock", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Powerdouble ("Power double", Range(0, 1)) = 0.2889535
        _Power ("Power", Range(0, 5)) = 3.442639
        _Mousse_N ("Mousse_N", 2D) = "bump" {}
        _Rocks_Normal ("Rocks_Normal", 2D) = "bump" {}
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
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Mousse; uniform float4 _Mousse_ST;
            uniform sampler2D _Rock; uniform float4 _Rock_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Powerdouble;
            uniform float _Power;
            uniform sampler2D _Mousse_N; uniform float4 _Mousse_N_ST;
            uniform sampler2D _Rocks_Normal; uniform float4 _Rocks_Normal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Mousse_N_var = UnpackNormal(tex2D(_Mousse_N,TRANSFORM_TEX(i.uv0, _Mousse_N)));
                float3 _Rocks_Normal_var = UnpackNormal(tex2D(_Rocks_Normal,TRANSFORM_TEX(i.uv0, _Rocks_Normal)));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float3 node_9225 = pow(saturate((_Powerdouble+_Noise_var.rgb)),_Power);
                float3 normalLocal = lerp(_Mousse_N_var.rgb,_Rocks_Normal_var.rgb,saturate(( node_9225 > 0.5 ? (1.0-(1.0-2.0*(node_9225-0.5))*(1.0-_Rocks_Normal_var.rgb)) : (2.0*node_9225*_Rocks_Normal_var.rgb) )));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Mousse_var = tex2D(_Mousse,TRANSFORM_TEX(i.uv0, _Mousse));
                float4 _Rock_var = tex2D(_Rock,TRANSFORM_TEX(i.uv0, _Rock));
                float3 diffuseColor = lerp(_Mousse_var.rgb,_Rock_var.rgb,saturate(( node_9225 > 0.5 ? (1.0-(1.0-2.0*(node_9225-0.5))*(1.0-_Rock_var.rgb)) : (2.0*node_9225*_Rock_var.rgb) )));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Mousse; uniform float4 _Mousse_ST;
            uniform sampler2D _Rock; uniform float4 _Rock_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Powerdouble;
            uniform float _Power;
            uniform sampler2D _Mousse_N; uniform float4 _Mousse_N_ST;
            uniform sampler2D _Rocks_Normal; uniform float4 _Rocks_Normal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Mousse_N_var = UnpackNormal(tex2D(_Mousse_N,TRANSFORM_TEX(i.uv0, _Mousse_N)));
                float3 _Rocks_Normal_var = UnpackNormal(tex2D(_Rocks_Normal,TRANSFORM_TEX(i.uv0, _Rocks_Normal)));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(i.uv0, _Noise));
                float3 node_9225 = pow(saturate((_Powerdouble+_Noise_var.rgb)),_Power);
                float3 normalLocal = lerp(_Mousse_N_var.rgb,_Rocks_Normal_var.rgb,saturate(( node_9225 > 0.5 ? (1.0-(1.0-2.0*(node_9225-0.5))*(1.0-_Rocks_Normal_var.rgb)) : (2.0*node_9225*_Rocks_Normal_var.rgb) )));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Mousse_var = tex2D(_Mousse,TRANSFORM_TEX(i.uv0, _Mousse));
                float4 _Rock_var = tex2D(_Rock,TRANSFORM_TEX(i.uv0, _Rock));
                float3 diffuseColor = lerp(_Mousse_var.rgb,_Rock_var.rgb,saturate(( node_9225 > 0.5 ? (1.0-(1.0-2.0*(node_9225-0.5))*(1.0-_Rock_var.rgb)) : (2.0*node_9225*_Rock_var.rgb) )));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
