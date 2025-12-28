Shader "QinYou/Particles/ParticlesAlphaTintColor" {
	Properties {
		_TintColor ("Tint Color", Vector) = (1,1,1,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Emission ("Emission", Range(0, 8)) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Float) = 10
		_TintTex ("Tint Texture", 2D) = "white" {}
		[KeywordEnum(Off, Add, Multi)] _OverlayMode ("贴图叠加模式", Float) = 0
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