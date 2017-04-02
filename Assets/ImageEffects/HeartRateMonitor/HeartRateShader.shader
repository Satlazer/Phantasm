﻿Shader "Hidden/HeartRateShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _HeartRateTex;
			sampler2D _GridTex;
			fixed4 _Color;
			float2 _UVMult;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				fixed4 scrollHeart = tex2D(_HeartRateTex, (i.uv * _UVMult) - (_Time * 10.0f).rr);
				col = col * scrollHeart;
				scrollHeart =  tex2D(_GridTex, i.uv);
				col.rgb += scrollHeart.rgb;
				return col;
			}
			ENDCG
		}
	}
}
