Shader "Custom/DissolveOffset_Fixed"
{
    Properties
    {
        [HDR] _Color ("Color", Vector) = (1,1,1,1)
        [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
        _MainTex_Tiling_Offset ("MainTex_Tiling&Offset", Vector) = (1,1,0,0)
        _Threshold ("Threshold", Range(-0.3, 0.3)) = 0
        _HardDis ("HardDis", Range(0, 1)) = 0
        [NoScaleOffset] _DissolveTex ("DissolveTex", 2D) = "white" {}
        _DissolveTex_Tiling_Offset ("DissolveTex_Tiling&Offset", Vector) = (1,1,0,0)
        [NoScaleOffset] _Mask ("Mask", 2D) = "white" {}
        [ToggleUI] _MaskOffset ("MaskOffset", Float) = 0
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

            Blend SrcAlpha OneMinusSrcAlpha  // Hoặc dùng Blend SrcAlpha One nếu hiệu ứng sáng rực
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
            float4 _MainTex_Tiling_Offset;

            Texture2D _DissolveTex;
            SamplerState sampler_DissolveTex;
            float4 _DissolveTex_Tiling_Offset;

            Texture2D _Mask;
            SamplerState sampler_Mask;

            float4 _Color;
            float _Threshold;
            float _HardDis;
            float _MaskOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 1. Main Texture với Tiling & Offset
                float2 mainUV = uv * _MainTex_Tiling_Offset.xy + _MainTex_Tiling_Offset.zw;
                float4 mainTex = _MainTex.Sample(sampler_MainTex, mainUV);

                // 2. Dissolve Texture với Tiling & Offset
                float2 dissolveUV = uv * _DissolveTex_Tiling_Offset.xy + _DissolveTex_Tiling_Offset.zw;
                float4 dissolveTex = _DissolveTex.Sample(sampler_DissolveTex, dissolveUV);
                float dissolveVal = dissolveTex.r; // Lấy kênh R làm chuẩn tan biến

                // 3. Tính toán Dissolve (Threshold và HardDis)
                float diff = dissolveVal - _Threshold;
                float dissolveAlpha = smoothstep(0.0, max(_HardDis, 0.001), diff);

                // 4. Mask Texture
                float4 maskTex = _Mask.Sample(sampler_Mask, uv);
                float maskVal = (_MaskOffset > 0.5) ? maskTex.r : maskTex.a;
                float finalMask = (maskTex.r > 0.0 || maskTex.a > 0.0) ? maskVal : 1.0;

                // 5. Kết hợp màu sắc, HDR Color, Vertex Color và Alpha
                float4 finalColor = mainTex * _Color * i.color;
                finalColor.a *= dissolveAlpha * finalMask;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}