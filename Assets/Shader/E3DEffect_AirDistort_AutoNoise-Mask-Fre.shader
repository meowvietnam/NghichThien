Shader "E3DEffect/AirDistort/AutoNoise-Mask-Fre" {
	Properties {
		_DistortMap ("DistortMap", 2D) = "white" {}
		_Speed ("Speed", Range(0, 1)) = 0.1
		_Distrot ("Distrot", Range(0, 0.1)) = 0.1
		_DistortOffset ("DistortOffset", Range(-2, 2)) = -0.8
		_MaskMap ("MaskMap", 2D) = "white" {}
		_Alpha ("Alpha", Range(0, 5)) = 2
		_Tiling ("Tiling", Range(0, 5)) = 0
		_DisFrePower ("Dis-Fre-Power", Range(0, 30)) = 6.318185
		_DisFreScale ("Dis-Fre-Scale", Range(0, 2)) = 1.430902
		[Toggle] _DisFreinvert ("Dis-Fre-invert", Float) = 1
		[HDR] _FreColor ("Fre-Color", Vector) = (1,1,1,1)
		_ColorFrePower ("Color-Fre-Power", Range(0, 30)) = 6.318185
		_ColorFreScale ("Color-Fre-Scale", Range(0, 2)) = 1.430902
		[Toggle] _ColorFreinvert ("Color-Fre-invert", Float) = 1
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