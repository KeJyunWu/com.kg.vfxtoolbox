Shader "hidden/two_pass_linear_sampling_gaussian_blur" 
{
	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma multi_compile LITTLE_KERNEL MEDIUM_KERNEL BIG_KERNEL
	#include "GaussianBlur.cginc"
	
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	uniform float _Sigma;
	uniform float2 _TexelSize;
	
	float4 frag_horizontal (v2f_img i) : COLOR
	{
		return GaussianBlurLinearSampling(_TexelSize, i.uv, _MainTex, _Sigma, float2(1,0));
	}
	
	float4 frag_vertical (v2f_img i) : COLOR
	{				
		return GaussianBlurLinearSampling(_TexelSize, i.uv, _MainTex, _Sigma, float2(0,1));
	}
	ENDCG
	
	Properties 
	{ 
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Cull Off ZWrite Off ZTest Always

	    Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag_horizontal
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag_vertical
			ENDCG
		}
	}
}