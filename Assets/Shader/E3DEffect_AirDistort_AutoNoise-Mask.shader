Shader "E3DEffect/AirDistort/AutoNoise-Mask" {
	Properties {
		_DistortMap ("DistortMap", 2D) = "white" {}
		_Speed ("Speed", Range(0, 1)) = 0.1
		_Distrot ("Distrot", Range(0, 0.1)) = 0.255728
		_DistortOffset ("DistortOffset", Range(-2, 2)) = -0.8
		_MaskMap ("MaskMap", 2D) = "white" {}
		_Alpha ("Alpha", Range(0, 5)) = 2
		_Tiling ("Tiling", Range(0, 5)) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	//CustomEditor "ASEMaterialInspector"
}