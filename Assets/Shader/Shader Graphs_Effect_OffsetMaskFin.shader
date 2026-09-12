Shader "Custom/Effect_OffsetMaskFin_Fixed"
{
    Properties
    {
        [HDR] _Color ("Color", Vector) = (0.5,0.5,0.5,1)
        [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
        _MainTilingOffset ("MainTilingOffset", Vector) = (1,1,0,0)
        
        [NoScaleOffset] _MaskTex ("MaskTex", 2D) = "white" {}
        _MaskTilingOffset ("MaskTilingOffset", Vector) = (1,1,0,0)
        
        [NoScaleOffset] _DistortTex ("DistortTex", 2D) = "white" {}
        _DistortTilingOffset ("DistortTilingOffset", Vector) = (1,1,0,0)
        _Distort ("Distort", Range(0, 1)) = 0
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

            Blend SrcAlpha One           // Đổi thành Blend SrcAlpha OneMinusSrcAlpha nếu hiệu ứng cần Alpha Blend bình thường
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

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float4 _MainTilingOffset;

            Texture2D _MaskTex;
            SamplerState sampler_MaskTex;
            float4 _MaskTilingOffset;

            Texture2D _DistortTex;
            SamplerState sampler_DistortTex;
            float4 _DistortTilingOffset;

            float4 _Color;
            float _Distort;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // Lấy màu từ Particle System
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 1. Tính toán hiệu ứng Distort (Làm méo UV)
                float2 distortUV = uv * _DistortTilingOffset.xy + _DistortTilingOffset.zw;
                float4 distortColor = _DistortTex.Sample(sampler_DistortTex, distortUV);
                float2 distortOffset = (distortColor.rg * 2.0 - 1.0) * _Distort;
                uv += distortOffset; // Áp dụng độ méo vào UV chính

                // 2. Main Texture với Tiling và Offset riêng
                float2 mainUV = uv * _MainTilingOffset.xy + _MainTilingOffset.zw;
                float4 mainTexColor = _MainTex.Sample(sampler_MainTex, mainUV);

                // 3. Mask Texture với Tiling và Offset riêng
                float2 maskUV = uv * _MaskTilingOffset.xy + _MaskTilingOffset.zw;
                float4 maskColor = _MaskTex.Sample(sampler_MaskTex, maskUV);
                float maskVal = maskColor.a; // Hoặc dùng maskColor.r tùy theo texture mặt nạ của bạn

                // Nếu chưa gán MaskTex, mặc định mask = 1
                float finalMask = (maskColor.r > 0.0 || maskColor.a > 0.0) ? maskVal : 1.0;

                // 4. Kết hợp màu sắc, HDR Color, Vertex Color và Mask
                float4 finalColor = mainTexColor * _Color * i.color;
                finalColor.a *= finalMask;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}