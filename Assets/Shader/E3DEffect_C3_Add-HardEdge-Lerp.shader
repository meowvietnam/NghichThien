Shader "E3DEffect/C3/Add-HardEdge-Lerp" {
	Properties {
		_Cutoff ("Mask Clip Value", Float) = 0.5
		_Base ("Base", 2D) = "white" {}
		_Shape ("Shape", 2D) = "white" {}
		[HDR] _BaseColor ("Base-Color", Vector) = (0,0,0,0)
		[HDR] _AddColor ("Add-Color", Vector) = (0,0,0,0)
		_BaseAlpha ("BaseAlpha", Range(0, 1)) = 0.5411765
		_EdgeClip ("EdgeClip", Range(0, 10)) = 0
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
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}