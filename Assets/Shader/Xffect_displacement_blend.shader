Shader "Xffect/displacement/blend" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("Main Texture", 2D) = "white" {}
		_DispMap ("Displacement Map (RG)", 2D) = "white" {}
		_MaskTex ("Mask(R)", 2D) = "white" {}
		_DispScrollSpeedX ("Map Scroll Speed X", Float) = 0
		_DispScrollSpeedY ("Map Scroll Speed Y", Float) = 0
		_DispX ("Displacement Strength X", Float) = 0
		_DispY ("Displacement Strength Y", Float) = 0.2
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