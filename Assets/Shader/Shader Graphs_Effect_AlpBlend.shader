Shader "Custom/Effect_AlphaBlend_Fixed"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
        [HDR] Color_C8BB4834 ("Color", Vector) = (0.4622642,0.4622642,0.4622642,1)
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

            Blend SrcAlpha OneMinusSrcAlpha  // Chế độ Alpha Blend tiêu chuẩn
            ZWrite Off
            Cull Off                     // Hiển thị cả 2 mặt

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
            float4 Color_C8BB4834;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = (v.uv * _MainTex_ST.xy) + _MainTex_ST.zw;
                o.color = v.color; // Lấy màu từ Particle System truyền vào
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Lấy texture chính
                float4 texColor = _MainTex.Sample(sampler_MainTex, i.uv);

                // Nhân với màu Color trong Material và màu gốc của hạt
                float4 finalColor = texColor * Color_C8BB4834 * i.color;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}