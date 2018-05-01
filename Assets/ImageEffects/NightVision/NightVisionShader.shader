﻿Shader "Hidden/NightVisionShader"
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
			
			float rand(float2 co)
			{
				return frac(sin(1000.0*dot(co.xy, float2(21.5739, 43.421))) * 617284.3);
			}

			float rand(float co)
			{
				return frac(sin(1000.0 * co) * 617284.3);
			}

			sampler2D _MainTex;
			
			float uAmount;
			float RandomNumber;
			float uLightMult;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				float3 vision;

				float2 RandomNumber2 = float2(RandomNumber, RandomNumber);

				//vision.r = (col.r * 0.393 * 0.1f) + (col.g * 0.769 * 0.1f) + (col.b * 0.189 * 0.1f);
				//vision.g = (col.r * 0.349 * 1.5f) + (col.g * 0.686 * 1.5f) + (col.b * 0.168 * 1.5f) + 0.1f;
				//vision.b = (col.r * 0.272 * 0.1f) + (col.g * 0.534 * 0.1f) + (col.b * 0.131 * 0.1f);

				vision.r = col.r + col.g + col.b;
				vision.g = vision.r;
				vision.b = vision.r;

				vision.r *= 0.15;
				vision.b *= 0.15;

				vision.rgb *= uLightMult;

				//outColor.rgb = mix(outColor.rgb, vec3(rand(vec2(uGrain.x + texcoord.x, uGrain.y + texcoord.y))), uAmount);
				float2 uvRound = floor(i.uv * 100.0f) / 100.0f; 
				uvRound.x = floor(i.uv.x * 177.7f) / 177.7f;
				uvRound.y = floor(i.uv.y * 100.0f) / 100.0f;
				//outColor.rgb = mix(source.rgb, vec3(luminance), uAmount);
				col = float4(vision.xyz, col.w);
				//col.rgb = lerp(col.rgb, 
				//	float3(rand(RandomNumber + uvRound), rand(RandomNumber + uvRound), rand(RandomNumber + uvRound)),
				//	uAmount);
				//(float2(RandomNumber + i.uv.x, RandomNumber + i.uv.x)
				//col = rand(i.uv);

				return col;

			}
			ENDCG
		}
	}
}
