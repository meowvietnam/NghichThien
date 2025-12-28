Shader "Hovl/Particles/Lightning" {
	Properties {
		_MainTexture ("Main Texture", 2D) = "white" {}
		_Noise ("Noise", 2D) = "white" {}
		_FlowMap ("Flow Map", 2D) = "white" {}
		_VFlowSpeed ("V Flow Speed", Float) = 2
		_UFlowSpeed ("U Flow Speed", Float) = 4
		_FlowStrength ("Flow Strength", Float) = 0.1
		_Color ("Color", Vector) = (1,1,1,1)
		_Emission ("Emission", Float) = 2
		_ShinnySpeed ("Shinny Speed", Float) = 30
		[Toggle] _UseShinny ("Use Shinny", Float) = 0
		[MaterialToggle] _Usedepth ("Use depth?", Float) = 0
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
}