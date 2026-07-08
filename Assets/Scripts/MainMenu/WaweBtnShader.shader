Shader "Custom/UI_WaveDistortion"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Các biến điều chỉnh hiệu ứng sóng
        _WaveSpeed ("Wave Speed", Float) = 5.0
        _WaveFrequency ("Wave Frequency", Float) = 10.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.05
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _WaveSpeed;
            float _WaveFrequency;
            float _WaveAmplitude;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                
                // Biến đổi tọa độ UV để làm biến dạng (Distortion) trục Y
                // v.texcoord.x đóng vai trò là "Mặt nạ": x tăng dần từ trái (0) sang phải (1)
                // Giúp phần đuôi bên phải lượn sóng mạnh hơn phần đầu bên trái
                float wave = sin(v.texcoord.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude * v.texcoord.x;
                
                // Cộng độ lệch sóng vào trục Y của Vertex
                v.vertex.y += wave * 100.0; // Nhân với 100 vì tọa độ UI Canvas lớn hơn tọa độ Mesh thường

                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                return color;
            }
            ENDCG
        }
    }
}