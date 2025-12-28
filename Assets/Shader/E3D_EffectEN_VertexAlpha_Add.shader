Shader "E3D/EffectEN/VertexAlpha_Add" {
	Properties {
		_MainColor ("MainColor", Vector) = (0.5,0.5,0.5,1)
		_Falloff ("Falloff", Range(0, 10)) = 0
		_Power ("Power", Range(0, 10)) = 1
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
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
	//CustomEditor "ShaderForgeMaterialInspector"
}