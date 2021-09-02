Shader "Explosion_Shader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull模式", Float) = 1
        [Header(TEXTURES)]
        [MainTexture] _BaseMap("基础贴图", 2D) = "white" {}
        [HDR] _MainHDR("MainHDR", Color) = (1, 1, 1, 1)

        _Noise("噪点贴图", 2D) = "white" {}

        //[Header(UV FLOW)]
        //[Toggle(ENABLE_UVFLOW)] _enableUVFlow("UV滚动", Float) = 0
        _UVFlowSpeed("UV滚动速度", Vector) = (0, 0, 0, 0)

        //[Header(UV WARP)]
        //[Toggle(ENABLE_UVWARP)] _enableUVWarp("UV扰动", Float) = 0
        _WarpIntensity("扰动强度", range(0, 1)) = 0
        _UVWarpSpeed("UV扰动速度", Vector) = (0, 0, 0, 0)

        //[Header(DISSOLVE)]
        //[Toggle(ENABLE_DISSOLVE)] _enableDissolve("溶解", Float) = 0
        [HDR] _HDRColor("HDR颜色", Color) = (1, 1, 1, 1)
        _DissolveAmount("溶解程度", range(0, 1)) = 0.5
        _EdgeWidth("边缘粗细", range(0, 0.8)) = 0.01
        _EdgeSmoothnessOut("外边缘硬度", range(0, 0.5)) = 0.05
        _EdgeSmoothnessIn("内边缘硬度", range(0, 0.25)) = 0.05

        //[Toggle(USE_MASK)] _useMask("使用遮罩", Float) = 0
        _Mask("溶解方向遮罩", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

        Pass
        {
            ZWrite Off 
            Blend SrcAlpha OneMinusSrcAlpha
            Cull[_Cull]

            HLSLPROGRAM

            #pragma shader_feature ENABLE_UVFLOW 
            #pragma shader_feature ENABLE_UVWARP
            #pragma shader_feature ENABLE_DISSOLVE
            #pragma shader_feature USE_MASK
            #pragma shader_feature DOUBLE_SIDE

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            #if (ENABLE_UVWARP || ENABLE_DISSOLVE)
                TEXTURE2D(_Noise);
                SAMPLER(sampler_Noise);
            #endif

            #if USE_MASK
                TEXTURE2D(_Mask);
                SAMPLER(sampler_Mask);
            #endif 
            
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _HDRColor, _MainHDR;

            #if ENABLE_UVFLOW
            half2 _UVFlowSpeed;
            #endif

            #if (ENABLE_UVWARP || ENABLE_DISSOLVE)
            float4 _Noise_ST;
            #endif

            #if ENABLE_UVWARP
            half4 _UVTiling, _UVOffset;
            half2 _UVWarpSpeed;
            half _WarpIntensity;
            #endif

            #if ENABLE_DISSOLVE
            half _DissolveAmount, _EdgeWidth, _NoiseStrength, _EdgeSmoothnessOut, _EdgeSmoothnessIn;
            #endif

            #if USE_MASK
            half4 _Mask_ST;
            #endif 
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            float remap(float In, float2 InMinMax, float2 OutMinMax)
            {
                return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half2 finalUV = IN.uv;

                #if ENABLE_UVFLOW //UV滚动
                    half2 uvFlow = _Time.y * _UVFlowSpeed;
                    finalUV += uvFlow;
                #endif

                #if ENABLE_UVWARP //UV扭曲
                    half4 bias = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, IN.uv + _Time.y * _UVWarpSpeed);
                    half2 uvWarp = bias * _WarpIntensity;
                    finalUV += uvWarp;
                #endif

                #if ENABLE_DISSOLVE
                    half4 dissolveValue = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, IN.uv * _Noise_ST.xy + _Noise_ST.zw);
                    //dissolveValue = remap(dissolveValue, float2(0, 1), float2(-_NoiseStrength, _NoiseStrength));
                    half remappedAmount = remap(_DissolveAmount, float2(0, 1), float2(1, -0.2)); 
                    half alpha = smoothstep(0, _EdgeSmoothnessOut, dissolveValue - remappedAmount);
                    half4 edgeColor = smoothstep(0, _EdgeSmoothnessIn, remappedAmount - dissolveValue + _EdgeWidth) * _HDRColor;

                    #if DOUBLE_SIDE
                        half edge = alpha - smoothstep(0, _EdgeSmoothnessIn, (remappedAmount - dissolveValue) - _EdgeWidth);
                    #endif

                    #if USE_MASK
                        half4 mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, IN.uv * _Mask_ST.xy + _Mask_ST.zw);
                        remappedAmount = remap(_DissolveAmount, float2(0, 1), float2(-0.5, 2)); 
                        //dissolveValue *= mask;
                        alpha = smoothstep(0, _EdgeSmoothnessOut, mask - (remappedAmount - dissolveValue));
                        edgeColor = smoothstep(0, _EdgeSmoothnessIn, remappedAmount - dissolveValue - (mask - _EdgeWidth)) * _HDRColor;
                        #if DOUBLE_SIDE
                            edge = alpha - smoothstep(0, _EdgeSmoothnessIn, (mask - _EdgeWidth) - (remappedAmount - dissolveValue));
                        #endif
                    #endif 
                #endif

                half4 basemap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, finalUV * _BaseMap_ST.xy + _BaseMap_ST.zw);
                float4 color = basemap * _MainHDR;
               
                #if ENABLE_DISSOLVE
                color += edgeColor;
                color.a = alpha;
                    #if DOUBLE_SIDE
                    color += edgeColor;
                    color.a = edge * _HDRColor.a;
                    #endif
                #endif
                

                color.a *= _MainHDR.a;
                color.a *= basemap.w;
                return color;
            }
            ENDHLSL
        }
    }
    
    CustomEditor "UnityEditor.ExplosionGUI"
}
