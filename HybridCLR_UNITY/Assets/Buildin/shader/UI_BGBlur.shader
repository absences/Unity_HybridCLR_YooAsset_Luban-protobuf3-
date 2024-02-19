// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/UI_BGBlur"
{
	Properties
	{
		_MainTex("No Use Texture", 2D) = "white" {}
		_Size("Size", Range(0,2)) = 1.206
		_Dark("Dark", Range(0,1)) = 0.385
	}

	CGINCLUDE
#include "UnityCG.cginc"

	sampler2D _GrabTexture;
	float4 _GrabTexture_TexelSize;
	float _Size;
	float _Dark;

	struct v2f
	{
		float4 uvgrab : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
		o.uvgrab.xy = (float2(o.vertex.x, -o.vertex.y) + o.vertex.w) * 0.5;
#else
		o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y) + o.vertex.w) * 0.5;
#endif
		o.uvgrab.zw = o.vertex.zw;
		return o;
	}
	ENDCG

	SubShader
	{
		Tags {
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		LOD 100
		ZWrite Off

		GrabPass {Tags{"LightMode" = "Always"}}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				half4 sum = half4(0,0,0,0);
				#define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18, 0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);
				return sum ;
			}
			ENDCG
		}

		GrabPass{ Tags{ "LightMode" = "Always" } }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				half4 sum = half4(0,0,0,0);
				#define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight
				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18, 0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);
				return sum * _Dark;
			}
			ENDCG
		}
	}
	//Fallback "UI/Unlit/Transparent"
}