Shader "Shader Graphs/AlphaBlend_UVmove_nodouble" {
	Properties {
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
		_MainTex_TilingOffset ("MainTex_TilingOffset", Vector) = (1,1,0,0)
		[HDR] _Color ("Color", Vector) = (1,1,1,1)
		_TimeX ("TimeX", Float) = 0
		_TimeY ("TimeY", Float) = 0
		[NoScaleOffset] _Mask ("Mask", 2D) = "white" {}
		_Mask_TilingOffset ("Mask_TilingOffset", Vector) = (1,1,0,0)
		[ToggleUI] _R_Alpha ("R_Alpha", Float) = 0
		_DistortStrength ("DistortStrength", Range(-0.2, 0.2)) = 0
		[NoScaleOffset] _DistortTex ("DistortTex", 2D) = "white" {}
		_DistortTime ("DistortTime", Float) = 0
		[ToggleUI] _DistortOn ("DistortOn", Float) = 0
		Vector2_F6BA5D4D ("DistortTexTiling", Vector) = (1,1,0,0)
		[HDR] Color_ECD6C29 ("SceneColor", Vector) = (0,0,0,1)
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
}