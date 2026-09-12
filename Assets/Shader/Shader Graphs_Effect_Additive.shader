Shader "Custom/Effect_Additive_Fixed"
{
    Properties
    {
        [HDR] Color_C809F131 ("Color", Vector) = (0.5188679,0.5188679,0.5188679,1)
        [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
        Vector1_68F50BA2 ("CameraOffset", Float) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
        }

        Pass
        {
            Name "ForwardBase"
            Tags { "LightMode" = "ForwardBase" }

            Blend SrcAlpha One           // Chế độ Additive (Cộng màu sáng rực cho hiệu ứng phép thuật)
            ZWrite Off
            Cull Off                     // Hiển thị 2 mặt không bị mất tích khi xoay góc nhìn

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
            float4 _MainTex_ST;
            float4 Color_C809F131;
            float Vector1_68F50BA2;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = (v.uv * _MainTex_ST.xy) + _MainTex_ST.zw;
                o.color = v.color; // Lấy màu sắc từ Particle System
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Lấy texture chính
                float4 texColor = _MainTex.Sample(sampler_MainTex, i.uv);

                // Nhân với Color HDR chỉnh trong Material và Vertex Color của hạt
                float4 finalColor = texColor * Color_C809F131 * i.color;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}