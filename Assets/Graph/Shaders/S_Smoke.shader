// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9754,x:32719,y:32712,varname:node_9754,prsc:2|emission-3541-OUT;n:type:ShaderForge.SFN_Tex2d,id:2493,x:32218,y:32575,ptovrint:False,ptlb:Smoke,ptin:_Smoke,varname:node_2493,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5eb36dd93de83b043ae2cf85022693db,ntxv:0,isnm:False|UVIN-1732-OUT;n:type:ShaderForge.SFN_Tex2d,id:9736,x:31626,y:32537,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_9736,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-8320-UVOUT;n:type:ShaderForge.SFN_Panner,id:8320,x:31403,y:32537,varname:node_8320,prsc:2,spu:-0.3,spv:-0.2|UVIN-444-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:444,x:30812,y:32357,varname:node_444,prsc:2,uv:0;n:type:ShaderForge.SFN_Lerp,id:1732,x:32057,y:32560,varname:node_1732,prsc:2|A-7768-OUT,B-2814-OUT,T-8867-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2814,x:31816,y:32537,varname:node_2814,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-9736-RGB;n:type:ShaderForge.SFN_Slider,id:8867,x:31693,y:32725,ptovrint:False,ptlb:node_8867,ptin:_node_8867,varname:node_8867,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.219171,max:1;n:type:ShaderForge.SFN_TexCoord,id:8140,x:31644,y:32368,varname:node_8140,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:4747,x:32030,y:32814,ptovrint:False,ptlb:Emissive power,ptin:_Emissivepower,varname:node_4747,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.05515805,max:30;n:type:ShaderForge.SFN_Append,id:755,x:31160,y:32547,varname:node_755,prsc:2|A-1409-OUT,B-444-V;n:type:ShaderForge.SFN_Multiply,id:1409,x:31051,y:32249,varname:node_1409,prsc:2|A-7664-OUT,B-444-U;n:type:ShaderForge.SFN_Slider,id:7664,x:30655,y:32206,ptovrint:False,ptlb:Tile,ptin:_Tile,varname:node_7664,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7.769894,max:30;n:type:ShaderForge.SFN_Multiply,id:3541,x:32528,y:32765,varname:node_3541,prsc:2|A-1723-OUT,B-4747-OUT,C-6595-OUT;n:type:ShaderForge.SFN_TexCoord,id:7152,x:31465,y:32834,varname:node_7152,prsc:2,uv:0;n:type:ShaderForge.SFN_Power,id:8690,x:31765,y:32906,varname:node_8690,prsc:2|VAL-7152-V,EXP-3785-OUT;n:type:ShaderForge.SFN_OneMinus,id:80,x:31698,y:33061,varname:node_80,prsc:2|IN-7152-V;n:type:ShaderForge.SFN_Power,id:5006,x:31880,y:33089,varname:node_5006,prsc:2|VAL-80-OUT,EXP-3785-OUT;n:type:ShaderForge.SFN_Add,id:7407,x:32054,y:32951,varname:node_7407,prsc:2|A-8690-OUT,B-5006-OUT;n:type:ShaderForge.SFN_OneMinus,id:6595,x:32246,y:32951,varname:node_6595,prsc:2|IN-7407-OUT;n:type:ShaderForge.SFN_Slider,id:3785,x:31240,y:33194,ptovrint:False,ptlb:Dégradé,ptin:_Dgrad,varname:node_3785,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7.925201,max:20;n:type:ShaderForge.SFN_Time,id:7610,x:31505,y:32054,varname:node_7610,prsc:2;n:type:ShaderForge.SFN_Add,id:3035,x:31963,y:32067,varname:node_3035,prsc:2|A-5111-OUT,B-8140-V;n:type:ShaderForge.SFN_Append,id:7768,x:31980,y:32281,varname:node_7768,prsc:2|A-8140-U,B-3035-OUT;n:type:ShaderForge.SFN_Multiply,id:5111,x:31695,y:31952,varname:node_5111,prsc:2|A-8210-OUT,B-7610-T;n:type:ShaderForge.SFN_Slider,id:8210,x:31248,y:31864,ptovrint:False,ptlb:Vitesse,ptin:_Vitesse,varname:node_8210,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1723,x:32441,y:32550,varname:node_1723,prsc:2|A-7877-RGB,B-2493-A;n:type:ShaderForge.SFN_Color,id:7877,x:32303,y:32304,ptovrint:False,ptlb:node_7877,ptin:_node_7877,varname:node_7877,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2006921,c2:0.9105356,c3:0.9411765,c4:1;proporder:2493-9736-8867-4747-7664-3785-8210-7877;pass:END;sub:END;*/

Shader "Custom/Smoke" {
    Properties {
        _Smoke ("Smoke", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Alpha ("Alpha", 2D) = "white" {}
        _node_8867 ("node_8867", Range(0, 1)) = 0.219171
        _Emissivepower ("Emissive power", Range(0, 30)) = 0.05515805
        _Tile ("Tile", Range(0, 30)) = 7.769894
        _Dgrad ("Dégradé", Range(0, 20)) = 7.925201
        _Vitesse ("Vitesse", Range(-1, 1)) = 0
        _node_7877 ("node_7877", Color) = (0.2006921,0.9105356,0.9411765,1)
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
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0

            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;

            uniform sampler2D _Smoke; uniform float4 _Smoke_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;

            uniform float _node_8867;
            uniform float _Emissivepower;
            uniform float _Dgrad;
            uniform float _Vitesse;
            uniform float4 _node_7877;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				float4 projPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
				o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:

                float sceneZ = max(0, LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0, i.projPos.z - _ProjectionParams.g);

                float4 timer = _Time + _TimeEditor;




                float2 panner = (i.uv0 + timer.g * float2(-0.3,-0.2));

                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(panner, _Noise));

                float3 uv_deformed = lerp(float3(float2(i.uv0.r,((_Vitesse * timer.g)+i.uv0.g)),0.0),_Noise_var.rgb.rgb,_node_8867);
                float4 _Smoke_var = tex2D(_Smoke,TRANSFORM_TEX(uv_deformed, _Smoke));

                float3 emissive = ((_node_7877.rgb*_Smoke_var.a) * _Emissivepower * (1.0 - (pow(i.uv0.g,_Dgrad)+pow((1.0 - i.uv0.g),_Dgrad)))) * tex2D(_Alpha,TRANSFORM_TEX(i.uv0, _Alpha));


                float3 finalColor = emissive * saturate((sceneZ-partZ));
				
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);

                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
