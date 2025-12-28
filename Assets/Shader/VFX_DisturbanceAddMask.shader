Shader "VFX/DisturbanceAddMask" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "black" {}
		_MainUVspeed ("Main UVspeed", Vector) = (0,0,0,0)
		_Color ("Tint Color", Vector) = (1,1,1,1)
		_AlphaTex ("Alpha (RGB)", 2D) = "white" {}
		_noiseTex ("Noise Tex", 2D) = "white" {}
		_maskTex ("Mask Tex", 2D) = "white" {}
		_HeatSpeed ("Heat speed", Vector) = (0,0,0,0)
		_HeatForce ("Heat Force", Range(0, 0.2)) = 0
		[KeywordEnum(Off, On)] DayNight ("Day Night", Float) = 0
		_Emission ("Emission", Range(0, 8)) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
		_Stencil ("Stencil ID", Float) = 0
		_StencilComp ("Stencil Comparison", Float) = 8
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}