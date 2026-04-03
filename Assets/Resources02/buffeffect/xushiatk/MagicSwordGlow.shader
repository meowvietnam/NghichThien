Shader "Custom/FakeGlowSword_NoPostProcess"
{
    Properties
    {
        _MainTex ("Texture Kiếm", 2D) = "white" {}
        _NoiseTex ("Dùng Noise32 của bạn", 2D) = "white" {}
        _GlowColor ("Màu Hào Quang", Color) = (1, 0.9, 0.3, 1)
        _GlowExpand ("Độ rộng hào quang", Range(0, 0.2)) = 0.05
        _FlowSpeed ("Tốc độ dòng chảy", Vector) = (0.1, 0.2, 0, 0)
    }

    SubShader
    {
        // Quan trọng: Phải để Transparent để thấy hiệu ứng tỏa sáng
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        // Chế độ cộng màu để tạo cảm giác rực rỡ (Lighten)
        Blend SrcAlpha One 
        Cull Off 
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_noise : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowExpand;
            float4 _FlowSpeed;

            v2f vert (appdata v) {
                v2f o;
                // Làm thanh kiếm to ra một chút để tạo viền sáng (Outline Glow)
                float3 normal = normalize(v.vertex.xyz);
                v.vertex.xyz += normal * _GlowExpand; 
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_noise = v.uv + _Time.yy * _FlowSpeed.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Lấy texture gốc
                fixed4 mainCol = tex2D(_MainTex, i.uv);
                
                // 2. Lấy Noise để tạo hiệu ứng "ảo diệu"
                fixed4 noise = tex2D(_NoiseTex, i.uv_noise);
                
                // 3. Tạo hiệu ứng nhấp nháy (Flicker) tự nhiên
                float pulse = 0.8 + 0.2 * sin(_Time.w * 3.0);
                
                // 4. Kết hợp: Màu kiếm + (Noise * Màu Glow)
                fixed4 finalColor;
                finalColor.rgb = (mainCol.rgb + noise.rgb * _GlowColor.rgb) * _GlowColor.a * pulse;
                
                // 5. Xử lý độ trong suốt (Dựa vào Alpha của ảnh gốc để không bị hiện hình vuông)
                finalColor.a = mainCol.a * noise.r;

                return finalColor;
            }
            ENDCG
        }
    }
}