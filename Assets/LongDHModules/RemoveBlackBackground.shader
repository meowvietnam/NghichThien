Shader "Unlit/RemoveBlackBackground_Bright"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Intensity ("Độ rực sáng", Range(0, 5)) = 1.0
        _Cutoff ("Lọc bỏ vùng tối", Range(0, 1)) = 0.05
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
        
        // CHÌA KHÓA Ở ĐÂY: Chuyển sang chế độ cộng màu
        // Nó sẽ tự động triệt tiêu màu đen và làm màu xanh rực rỡ hơn
        Blend One One 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float _Intensity;
            float _Cutoff;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Lấy màu từ Texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // Tính độ sáng để lọc bớt những vùng xám mờ không cần thiết
                float brightness = max(col.r, max(col.g, col.b));
                
                // Loại bỏ những pixel quá tối dựa trên _Cutoff
                // Giúp viền trông gọn gàng, không bị lem nhem
                col.rgb *= smoothstep(_Cutoff, _Cutoff + 0.1, brightness);
                
                // Nhân thêm với cường độ để tăng độ rực cho màu xanh
                col.rgb *= _Intensity;
                
                // Trong chế độ Blend One One, giá trị Alpha không còn quan trọng,
                // nhưng ta vẫn trả về để đúng cấu trúc.
                col.a = 1.0; 
                
                return col;
            }
            ENDCG
        }
    }
}