Shader "Custom/FireSlash_Fixed"
{
    Properties
    {
        _Detail ("Detail", Range(1, 20)) = 3
        [NoScaleOffset] _Mask ("Mask", 2D) = "white" {}
        [HDR] _BaseColor ("BaseColor", Vector) = (0.9622642,0.7966631,0.249644,1)
        [HDR] _RampColor ("RampColor", Vector) = (2,1.341609,0.235849,1)
        _ShadowRange ("ShadowRange", Range(0, 1)) = 0.2
        [HDR] _ShadowColor ("ShadowColor", Vector) = (0.4528302,0.2712709,0.2712709,1)
        _BrightRange ("BrightRange", Range(0, 1)) = 0.4
        [HDR] _BrightColor ("BrightColor", Vector) = (0,0,0,1)
        _DetailColor ("DetailColor", Vector) = (1,1,1,1)
        [HDR] _EdgeColor ("EdgeColor", Vector) = (0,0,0,1)
        _Tiling ("Tiling", Vector) = (1,1,0,0)
        _Bump ("Bump", Range(0, 10)) = 0
        _Pivot_Speed ("Pivot&Speed", Vector) = (0,0,0,0)
        Vector1_1226408C ("NoiseBlend", Range(-3, 3)) = -1.7
        [ToggleUI] Boolean_88D26759 ("DarkCut", Float) = 0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
        }

        Pass
        {
            Name "ForwardBase"
            Tags { "LightMode" = "ForwardBase" }

            Blend SrcAlpha One           // Additive Blend giúp hiệu ứng chém lửa sáng rực, có thể đổi thành SrcAlpha OneMinusSrcAlpha nếu muốn trong suốt thông thường
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                float4 color : COLOR;
            };

            Texture2D _Mask;
            SamplerState sampler_Mask;

            float _Detail;
            float4 _BaseColor;
            float4 _RampColor;
            float _ShadowRange;
            float4 _ShadowColor;
            float _BrightRange;
            float4 _BrightColor;
            float4 _DetailColor;
            float4 _EdgeColor;
            float2 _Tiling;
            float _Bump;
            float4 _Pivot_Speed;
            float Vector1_1226408C;
            float Boolean_88D26759;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // Hàm tạo noise procedural cho lửa
            float noise(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 1. Dịch chuyển UV theo tốc độ (_Pivot_Speed.zw hoặc xy) và Tiling
                float2 speedOffset = _Pivot_Speed.zw * _Time.y;
                float2 scaledUV = (uv * _Tiling) + speedOffset;

                // 2. Lấy mẫu texture Mask
                float4 maskTex = _Mask.Sample(sampler_Mask, uv);

                // 3. Tạo hiệu ứng nhiễu lửa dựa trên Detail và Bump
                float n = noise(scaledUV * _Detail + _Bump);
                float noiseVal = saturate(n + Vector1_1226408C);

                // 4. Pha trộn các lớp màu HDR
                float4 colorLayer = lerp(_BaseColor, _RampColor, noiseVal);
                colorLayer = lerp(colorLayer, _ShadowColor, step(noiseVal, _ShadowRange));
                colorLayer = lerp(colorLayer, _BrightColor, step(_BrightRange, noiseVal));
                colorLayer *= _DetailColor;

                if (Boolean_88D26759 > 0.5)
                {
                    colorLayer += _EdgeColor;
                }

                // 5. Kết hợp với Mask và Vertex Color của Particle System
                float4 finalColor = colorLayer * i.color;
                finalColor.a *= maskTex.r * i.color.a;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}