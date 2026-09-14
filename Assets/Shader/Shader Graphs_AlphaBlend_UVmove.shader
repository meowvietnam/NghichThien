Shader "Custom/AlphaBlend_UVmove_Fixed"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
        _MainTex_TilingOffset ("MainTex_TilingOffset", Vector) = (1,1,0,0)
        [HDR] _Color ("Color", Vector) = (1,1,1,1)
        _TimeX ("TimeX", Float) = 0
        _TimeY ("TimeY", Float) = 0
        
        [NoScaleOffset] _Mask ("Mask", 2D) = "white" {}
        _Mask_TilingOffset ("Mask_TilingOffset", Vector) = (1,1,0,0)
        [ToggleUI] _R_Alpha ("R_Alpha", Float) = 0
        
        _DistortStrength ("DistortStrength", Range(-0.2, 0.2)) = 0
        [NoScaleOffset] _DistortTex ("DistortTex", 2D) = "white" {}
        _DistortTime ("DistortTime", Float) = 0
        [ToggleUI] _DistortOn ("DistortOn", Float) = 0
        Vector2_F6BA5D4D ("DistortTexTiling", Vector) = (1,1,0,0)
        
        [HDR] Color_ECD6C29 ("SceneColor", Vector) = (0,0,0,1)
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

            Blend SrcAlpha OneMinusSrcAlpha  // Nếu muốn sáng rực kiểu Additive hãy đổi thành: Blend SrcAlpha One
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
            float4 _MainTex_TilingOffset;

            Texture2D _Mask;
            SamplerState sampler_Mask;
            float4 _Mask_TilingOffset;

            Texture2D _DistortTex;
            SamplerState sampler_DistortTex;
            float4 Vector2_F6BA5D4D;

            float4 _Color;
            float _TimeX;
            float _TimeY;
            float _R_Alpha;
            float _DistortStrength;
            float _DistortTime;
            float _DistortOn;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // Lấy màu từ Particle System truyền vào
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 1. Xử lý Distort (Làm méo UV)
                if (_DistortOn > 0.5)
                {
                    float2 distortUV = uv * Vector2_F6BA5D4D.xy + Vector2_F6BA5D4D.zw;
                    distortUV += _Time.y * _DistortTime;
                    float4 distortTex = _DistortTex.Sample(sampler_DistortTex, distortUV);
                    float2 distortOffset = (distortTex.rg * 2.0 - 1.0) * _DistortStrength;
                    uv += distortOffset;
                }

                // 2. Chuyển động UV cho MainTex theo thời gian (_TimeX, _TimeY)
                float2 mainUV = uv * _MainTex_TilingOffset.xy + _MainTex_TilingOffset.zw;
                mainUV += float2(_TimeX, _TimeY) * _Time.y;
                float4 mainTexColor = _MainTex.Sample(sampler_MainTex, mainUV);

                // 3. Xử lý Mask (Lọc theo kênh R hoặc Alpha tùy chọn)
                float2 maskUV = uv * _Mask_TilingOffset.xy + _Mask_TilingOffset.zw;
                float4 maskColor = _Mask.Sample(sampler_Mask, maskUV);
                float maskVal = (_R_Alpha > 0.5) ? maskColor.r : maskColor.a;
                
                // Nếu chưa gán Mask texture, mặc định mask = 1 để không bị đen thui
                float finalMask = (_Mask.Sample(sampler_Mask, maskUV).a > 0.0 || _Mask.Sample(sampler_Mask, maskUV).r > 0.0) ? maskVal : 1.0;

                // 4. Nhân đầy đủ màu sắc, Color HDR, Vertex Color của Particle và Alpha Mask
                float4 finalColor = mainTexColor * _Color * i.color;
                finalColor.a *= finalMask;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}