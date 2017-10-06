// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:2,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1,x:34981,y:32532,varname:node_1,prsc:2|diff-654-OUT,spec-678-OUT,gloss-374-OUT,normal-662-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33845,y:32796,ptovrint:False,ptlb:Blue Texture,ptin:_BlueTexture,varname:_BlueTexture,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:26,x:33844,y:32332,ptovrint:False,ptlb:Red Texture,ptin:_RedTexture,varname:_RedTexture,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:311,x:33572,y:33770,ptovrint:False,ptlb:Blue Normal,ptin:_BlueNormal,varname:_BlueNormal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:313,x:33572,y:33311,ptovrint:False,ptlb:Red Normal,ptin:_RedNormal,varname:_RedNormal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:374,x:34548,y:32674,ptovrint:False,ptlb:Roughness,ptin:_Roughness,varname:_Gloss,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:409,x:33844,y:32571,ptovrint:False,ptlb:Green Texture,ptin:_GreenTexture,varname:_GreenTexture,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:561,x:32874,y:33667,varname:node_561,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:602,x:33572,y:33548,ptovrint:False,ptlb:Green Normal,ptin:_GreenNormal,varname:_GreenNormal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:636,x:33570,y:34680,ptovrint:False,ptlb:Blue Specular,ptin:_BlueSpecular,varname:_BlueSpecular,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:638,x:33572,y:34224,ptovrint:False,ptlb:Red Specular,ptin:_RedSpecular,varname:_RedSpecular,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:642,x:33572,y:34450,ptovrint:False,ptlb:Green Specular,ptin:_GreenSpecular,varname:_GreenSpecular,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ChannelBlend,id:654,x:34248,y:32571,varname:node_654,prsc:2,chbt:0|M-1015-OUT,R-26-RGB,G-409-RGB,B-3-RGB,A-738-RGB;n:type:ShaderForge.SFN_ChannelBlend,id:662,x:33969,y:33592,varname:node_662,prsc:2,chbt:0|M-1015-OUT,R-313-RGB,G-602-RGB,B-311-RGB,A-1090-RGB;n:type:ShaderForge.SFN_ChannelBlend,id:678,x:34009,y:34419,varname:node_678,prsc:2,chbt:0|M-1015-OUT,R-638-RGB,G-642-RGB,B-636-RGB,A-1087-RGB;n:type:ShaderForge.SFN_Tex2d,id:738,x:33845,y:33034,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:1015,x:33179,y:33394,varname:node_1015,prsc:2|A-561-RGB,B-1058-OUT;n:type:ShaderForge.SFN_OneMinus,id:1058,x:33161,y:33637,varname:node_1058,prsc:2|IN-561-A;n:type:ShaderForge.SFN_Tex2d,id:1087,x:33570,y:34902,ptovrint:False,ptlb:Alpha Specular,ptin:_AlphaSpecular,varname:_AlphaSpecular,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1090,x:33572,y:33999,ptovrint:False,ptlb:Alpha Normal,ptin:_AlphaNormal,varname:_AlphaNormal,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;proporder:26-409-3-313-602-311-638-642-636-738-1087-1090-374;pass:END;sub:END;*/

Shader "Ikari Vertex Painter/Mobile/Diffuse/Vertex Color RGBA Blend" {
    Properties {
        _RedTexture ("Red Texture", 2D) = "white" {}
        _GreenTexture ("Green Texture", 2D) = "white" {}
        _BlueTexture ("Blue Texture", 2D) = "white" {}
        _RedNormal ("Red Normal", 2D) = "bump" {}
        _GreenNormal ("Green Normal", 2D) = "bump" {}
        _BlueNormal ("Blue Normal", 2D) = "bump" {}
        _RedSpecular ("Red Specular", 2D) = "white" {}
        _GreenSpecular ("Green Specular", 2D) = "white" {}
        _BlueSpecular ("Blue Specular", 2D) = "white" {}
        _Alpha ("Alpha", 2D) = "white" {}
        _AlphaSpecular ("Alpha Specular", 2D) = "white" {}
        _AlphaNormal ("Alpha Normal", 2D) = "bump" {}
        _Roughness ("Roughness", Range(0, 1)) = 0
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
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _BlueTexture; uniform float4 _BlueTexture_ST;
            uniform sampler2D _RedTexture; uniform float4 _RedTexture_ST;
            uniform sampler2D _BlueNormal; uniform float4 _BlueNormal_ST;
            uniform sampler2D _RedNormal; uniform float4 _RedNormal_ST;
            uniform fixed _Roughness;
            uniform sampler2D _GreenTexture; uniform float4 _GreenTexture_ST;
            uniform sampler2D _GreenNormal; uniform float4 _GreenNormal_ST;
            uniform sampler2D _BlueSpecular; uniform float4 _BlueSpecular_ST;
            uniform sampler2D _RedSpecular; uniform float4 _RedSpecular_ST;
            uniform sampler2D _GreenSpecular; uniform float4 _GreenSpecular_ST;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
            uniform sampler2D _AlphaSpecular; uniform float4 _AlphaSpecular_ST;
            uniform sampler2D _AlphaNormal; uniform float4 _AlphaNormal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
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
                float4 node_1015 = float4(i.vertexColor.rgb,(1.0 - i.vertexColor.a));
                fixed3 _RedNormal_var = UnpackNormal(tex2D(_RedNormal,TRANSFORM_TEX(i.uv0, _RedNormal)));
                fixed3 _GreenNormal_var = UnpackNormal(tex2D(_GreenNormal,TRANSFORM_TEX(i.uv0, _GreenNormal)));
                fixed3 _BlueNormal_var = UnpackNormal(tex2D(_BlueNormal,TRANSFORM_TEX(i.uv0, _BlueNormal)));
                fixed3 _AlphaNormal_var = UnpackNormal(tex2D(_AlphaNormal,TRANSFORM_TEX(i.uv0, _AlphaNormal)));
                float3 normalLocal = (node_1015.r*_RedNormal_var.rgb + node_1015.g*_GreenNormal_var.rgb + node_1015.b*_BlueNormal_var.rgb + node_1015.a*_AlphaNormal_var.rgb);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 1.0 - _Roughness; // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                fixed4 _RedSpecular_var = tex2D(_RedSpecular,TRANSFORM_TEX(i.uv0, _RedSpecular));
                fixed4 _GreenSpecular_var = tex2D(_GreenSpecular,TRANSFORM_TEX(i.uv0, _GreenSpecular));
                fixed4 _BlueSpecular_var = tex2D(_BlueSpecular,TRANSFORM_TEX(i.uv0, _BlueSpecular));
                fixed4 _AlphaSpecular_var = tex2D(_AlphaSpecular,TRANSFORM_TEX(i.uv0, _AlphaSpecular));
                float3 specularColor = (node_1015.r*_RedSpecular_var.rgb + node_1015.g*_GreenSpecular_var.rgb + node_1015.b*_BlueSpecular_var.rgb + node_1015.a*_AlphaSpecular_var.rgb);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                fixed4 _RedTexture_var = tex2D(_RedTexture,TRANSFORM_TEX(i.uv0, _RedTexture));
                fixed4 _GreenTexture_var = tex2D(_GreenTexture,TRANSFORM_TEX(i.uv0, _GreenTexture));
                fixed4 _BlueTexture_var = tex2D(_BlueTexture,TRANSFORM_TEX(i.uv0, _BlueTexture));
                fixed4 _Alpha_var = tex2D(_Alpha,TRANSFORM_TEX(i.uv0, _Alpha));
                float3 diffuseColor = (node_1015.r*_RedTexture_var.rgb + node_1015.g*_GreenTexture_var.rgb + node_1015.b*_BlueTexture_var.rgb + node_1015.a*_Alpha_var.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
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
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _BlueTexture; uniform float4 _BlueTexture_ST;
            uniform sampler2D _RedTexture; uniform float4 _RedTexture_ST;
            uniform sampler2D _BlueNormal; uniform float4 _BlueNormal_ST;
            uniform sampler2D _RedNormal; uniform float4 _RedNormal_ST;
            uniform fixed _Roughness;
            uniform sampler2D _GreenTexture; uniform float4 _GreenTexture_ST;
            uniform sampler2D _GreenNormal; uniform float4 _GreenNormal_ST;
            uniform sampler2D _BlueSpecular; uniform float4 _BlueSpecular_ST;
            uniform sampler2D _RedSpecular; uniform float4 _RedSpecular_ST;
            uniform sampler2D _GreenSpecular; uniform float4 _GreenSpecular_ST;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
            uniform sampler2D _AlphaSpecular; uniform float4 _AlphaSpecular_ST;
            uniform sampler2D _AlphaNormal; uniform float4 _AlphaNormal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
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
                float4 node_1015 = float4(i.vertexColor.rgb,(1.0 - i.vertexColor.a));
                fixed3 _RedNormal_var = UnpackNormal(tex2D(_RedNormal,TRANSFORM_TEX(i.uv0, _RedNormal)));
                fixed3 _GreenNormal_var = UnpackNormal(tex2D(_GreenNormal,TRANSFORM_TEX(i.uv0, _GreenNormal)));
                fixed3 _BlueNormal_var = UnpackNormal(tex2D(_BlueNormal,TRANSFORM_TEX(i.uv0, _BlueNormal)));
                fixed3 _AlphaNormal_var = UnpackNormal(tex2D(_AlphaNormal,TRANSFORM_TEX(i.uv0, _AlphaNormal)));
                float3 normalLocal = (node_1015.r*_RedNormal_var.rgb + node_1015.g*_GreenNormal_var.rgb + node_1015.b*_BlueNormal_var.rgb + node_1015.a*_AlphaNormal_var.rgb);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 1.0 - _Roughness; // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                fixed4 _RedSpecular_var = tex2D(_RedSpecular,TRANSFORM_TEX(i.uv0, _RedSpecular));
                fixed4 _GreenSpecular_var = tex2D(_GreenSpecular,TRANSFORM_TEX(i.uv0, _GreenSpecular));
                fixed4 _BlueSpecular_var = tex2D(_BlueSpecular,TRANSFORM_TEX(i.uv0, _BlueSpecular));
                fixed4 _AlphaSpecular_var = tex2D(_AlphaSpecular,TRANSFORM_TEX(i.uv0, _AlphaSpecular));
                float3 specularColor = (node_1015.r*_RedSpecular_var.rgb + node_1015.g*_GreenSpecular_var.rgb + node_1015.b*_BlueSpecular_var.rgb + node_1015.a*_AlphaSpecular_var.rgb);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                fixed4 _RedTexture_var = tex2D(_RedTexture,TRANSFORM_TEX(i.uv0, _RedTexture));
                fixed4 _GreenTexture_var = tex2D(_GreenTexture,TRANSFORM_TEX(i.uv0, _GreenTexture));
                fixed4 _BlueTexture_var = tex2D(_BlueTexture,TRANSFORM_TEX(i.uv0, _BlueTexture));
                fixed4 _Alpha_var = tex2D(_Alpha,TRANSFORM_TEX(i.uv0, _Alpha));
                float3 diffuseColor = (node_1015.r*_RedTexture_var.rgb + node_1015.g*_GreenTexture_var.rgb + node_1015.b*_BlueTexture_var.rgb + node_1015.a*_Alpha_var.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
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
