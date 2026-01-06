Shader "Shader/ASE_add" {
	Properties {
		_Texture ("Texture（主贴图）", 2D) = "white" {}
		[HDR] _Color_Tex ("Color_Tex", Vector) = (1,1,1,1)
		_Tex_x ("Tex_x", Float) = 0
		_Tex_y ("Tex_y", Float) = 0
		_Mask ("Mask", 2D) = "white" {}
		[HDR] _Mask_Color ("Mask_Color", Vector) = (1,1,1,1)
		_Mask_x ("Mask_x", Float) = 0
		_Mask_y ("Mask_y", Float) = 0
		[Toggle(_MASK2_ON)] _Mask2 ("Mask2", Float) = 0
		_Mask2_Size ("Mask2_Size", Float) = 10
		_Mask2_glow ("Mask2_glow", Float) = 1
		[Toggle] _Mask_Noise_on_off ("Mask_Noise_on_off", Float) = 0
		_Mask_Noise ("Mask_Noise", 2D) = "white" {}
		_Diss_Mask ("Diss_Mask", Float) = 0
		_Mask_Noise_x ("Mask_Noise_x", Float) = 0
		_Mask_Noise_y ("Mask_Noise_y", Float) = 0
		_Noise ("Noise", 2D) = "white" {}
		_Diss ("Diss", Float) = 0
		_Noise_x ("Noise_x", Float) = 0
		_Noise_y ("Noise_y", Float) = 0
		_Noise_Noise ("Noise_Noise", 2D) = "white" {}
		_Diss_Noise_Noise ("Diss_Noise_Noise", Float) = 0
		_Noise_Noise_x ("Noise_Noise_x", Float) = 0
		_Noise_Noise_y ("Noise_Noise_y", Float) = 0
		_Dissolve ("Dissolve", 2D) = "white" {}
		_Dissolve_x ("Dissolve_x", Float) = 0
		_Dissolve_y ("Dissolve_y", Float) = 0
		_DissolveSchedule ("DissolveSchedule", Float) = 0
		_Hard_soft ("Hard_soft", Float) = 0
		[Toggle(_PARTICLE_ON_OFF_ON)] _Particle_on_off ("Particle_on_off", Float) = 0
		[Toggle(_PARTICLE_ON_OFF2_ON)] _Particle_on_off2 ("Particle_on_off2", Float) = 0
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