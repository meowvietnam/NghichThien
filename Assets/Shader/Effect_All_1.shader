Shader "Effect/All" {
	Properties {
		[Header(Instruction)] _Tile1 ("Custom：添加Custom1.xy、Custom1.xyzw、Custom2.xyzw", Float) = 0
		_Tile2 ("Custom1.xy是主贴图流动，Custom1.z是溶解强度，Custom1.w是扰动强度", Float) = 0
		_Tile3 ("Custom2.xy是遮罩流动，Custom2.z是顶点偏移强度，Custom2.w是空气扭曲强度", Float) = 0
		[Header(Set Mode)] [KeywordEnum(Mesh,Particle)] _Custom2xyKey ("制作模式", Float) = 1
		[Enum(AlphaBlend,10,Additive,1)] _Dst ("材质模式", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode ("剔除模式", Float) = 2
		[Enum(Default,0,On,1,Off,2)] _ZWrite ("深度模式", Float) = 0
		_DepthFade ("深度消隐强度", Float) = 0
		[KeywordEnum(Off,On)] _FresnelKey ("菲尼尔开关", Float) = 0
		[HDR] _FresnelColor ("菲尼尔颜色", Vector) = (1,1,1,1)
		_FresnelScale ("菲尼尔宽度", Float) = 1
		_FresnelPower ("菲尼尔强度", Float) = 2
		[KeywordEnum(Off,On)] _if_ModeDepthFade1 ("边缘虚化开关", Float) = 0
		_ModeFresnel_Scale1 ("虚化范围", Float) = 1
		_ModeFresnel_Power1 ("虚化范围过渡", Float) = 0
		_ModeFresnel_Bias1 ("虚化强度", Float) = 0
		[Header(Main Mode)] [HDR] _Color ("颜色", Vector) = (1,1,1,1)
		_MainTex ("主贴图", 2D) = "white" {}
		_MainTex_U ("主贴图流动_U", Float) = 0
		_MainTex_V ("主贴图流动_V", Float) = 0
		[Toggle(_MAINTEXTODISSOLVE1_ON)] _MainTexToDissolve1 ("主贴图流动影响溶解", Float) = 0
		[Header(Dissslve Mode)] _DissolveTex ("溶解贴图", 2D) = "white" {}
		_DissolveTex_U1 ("溶解贴图流动_U", Float) = 0
		_DissolveTex_V1 ("溶解贴图流动_V", Float) = 0
		_DissolveIntensityCustom1z ("溶解强度", Range(0, 1)) = 0
		_SoftaDissolve ("软硬边强度", Range(0, 1)) = 0
		[HDR] _DissolveLineColor ("亮边颜色", Vector) = (1,1,1,1)
		_LineWidth ("亮边值", Range(0, 0.85)) = 0
		[Header(Noise Mode)] _DestortionTex ("扰动贴图", 2D) = "white" {}
		_DestortionIntensity ("扰动强度", Float) = 0
		_DestortionTex_U1 ("扰动流动_U", Float) = 0
		_DestortionTex_V1 ("扰动流动_V", Float) = 0
		[Header(Mask Mode)] _MaskTex ("遮罩贴图", 2D) = "white" {}
		[KeywordEnum(On,Off)] _MaskCustomDataKey2 ("遮罩自定义数据开关", Float) = 1
		_MaskTex_U1 ("遮罩贴图流动_U", Float) = 0
		_MaskTex_V1 ("遮罩贴图流动_V", Float) = 0
		_MaskTex02 ("遮罩贴图02", 2D) = "white" {}
		_MaskTex02_U1 ("遮罩流动02_U", Float) = 0
		_MaskTex02_V1 ("遮罩流动02_V", Float) = 0
		[Toggle(_DESTORTIONTOVERTEXOFFSET_ON)] _DestortionToVertexOffset ("扰动影响顶点偏移", Float) = 0
		[Header(Vertex Mode)] _VertexOffsetTex ("顶点偏移贴图", 2D) = "white" {}
		_VertexOffsetIntensity ("顶点偏移强度", Float) = 0
		_VertexOffsetTex_U1 ("顶点偏移流动_U", Float) = 0
		_VertexOffsetTex_V1 ("顶点偏移流动_V", Float) = 0
		[Header(AirDistortion Mode)] [KeywordEnum(Off,On)] _AirDistortionSwitch1 ("空气扭曲开关", Float) = 0
		_AirDistortionTex ("空气扭曲贴图", 2D) = "white" {}
		_AirDistortion_Intensity1 ("扭曲强度", Float) = 0
		_AirDistortionTex_U1 ("扭曲流动_U", Float) = 0
		_AirDistortionTex_V1 ("扭曲流动_V", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] _tex4coord2 ("", 2D) = "white" {}
		[HideInInspector] _tex4coord ("", 2D) = "white" {}
		[HideInInspector] _tex4coord3 ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
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
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}