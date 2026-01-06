Shader "Shader Graphs/off_set_432" {
	Properties {
		[HDR] Color_e619130ae1024a9a99d8d2983789daae ("Color", Vector) = (11.18176,11.18176,11.18176,1)
		[NoScaleOffset] Texture2D_b1fb5b9f694d4e85b0680a5ce6b06749 ("main texture", 2D) = "white" {}
		Vector2_ed04973d70fb48458bd14b4b06c32aad ("main spped", Vector) = (1,0,0,0)
		Vector2_e9d19dc70fae47b388e53225a6714a44 ("maintex tilling", Vector) = (1,1,0,0)
		Vector1_055a57cbe14649069ba8b5c703dc20cc ("dissolveScale", Float) = 3.76
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
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
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}