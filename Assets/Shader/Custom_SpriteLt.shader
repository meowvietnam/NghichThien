Shader "Custom/SpriteLt" {
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_MainColor ("Main Color", Vector) = (1,1,1,1)
		_LtSoft ("Light Soft", Range(0, 8)) = 1
		_LtExpand ("Light Expand", Float) = 1
		_LtOuterStrength ("Outer Light Strength", Range(0, 10)) = 1
		_LtInnerStrength ("Inner Light Strength", Range(0, 10)) = 1
		_LtColor ("LtColor", Vector) = (1,1,1,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}