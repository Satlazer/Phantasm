﻿Shader "Hidden/CAShader"
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
			
			float2 Pack(float2 param)
			{
				return param * 0.5 + 0.5;
			}

			float2 Unpack(float2 param)
			{
				return param + param - 1.0;
			}

			sampler2D _MainTex;
			
			float _Dispersal = 1.00;
			float _Offset;


			fixed4 frag(v2f i) : SV_Target
			{
				float2 _Aspect = float2(1.0f, 1.0f);
				float _rOffset = -_Offset;
				float _gOffset = 0.000;
				float _bOffset = _Offset;

				float2 TexOffset = Unpack(i.uv.xy);

				float2 rTexOffset = TexOffset;
				float2 gTexOffset = TexOffset;
				float2 bTexOffset = TexOffset;

				float2 absTexOffset = abs(TexOffset);
				float2 isPositive = absTexOffset / TexOffset;

				if ((TexOffset.x) != 0.0)
				{
					float distanceToCenter = absTexOffset.x * _Aspect.x;
					float distanceToCenterSquared = pow(distanceToCenter, _Dispersal);

					rTexOffset.x += _rOffset * distanceToCenterSquared * isPositive.x;
					gTexOffset.x += _gOffset * distanceToCenterSquared * isPositive.x;
					bTexOffset.x += _bOffset * distanceToCenterSquared * isPositive.x;
				}

				if ((TexOffset.y) != 0.0)
				{
					float distanceToCenter = absTexOffset.y * _Aspect.y;
					float distanceToCenterSquared = pow(distanceToCenter, _Dispersal);

					rTexOffset.y += _rOffset * distanceToCenterSquared * isPositive.y;
					gTexOffset.y += _gOffset * distanceToCenterSquared * isPositive.y;
					bTexOffset.y += _bOffset * distanceToCenterSquared * isPositive.y;
				}

				float4 rValue = 0.0f;// = float4(0.0f, 0.0f, 0.0f, 0.0f);
				float4 gValue = 0.0f;// = float4(0.0f, 0.0f, 0.0f, 0.0f);
				float4 bValue = 0.0f;// = float4(0.0f, 0.0f, 0.0f, 0.0f);

				//rTexOffset = rTexOffset;
				//gTexOffset = gTexOffset;
				//bTexOffset = bTexOffset;
				rTexOffset = Pack(rTexOffset);
				gTexOffset = Pack(gTexOffset);
				bTexOffset = Pack(bTexOffset);

				float4 sceneR4 = tex2D(_MainTex, rTexOffset);
				float4 sceneR3 = tex2D(_MainTex, lerp(rTexOffset, gTexOffset, 0.250));
				float4 sceneR2 = tex2D(_MainTex, lerp(rTexOffset, gTexOffset, 0.500));
				float4 sceneR1 = tex2D(_MainTex, lerp(rTexOffset, gTexOffset, 0.750));
				float4 sceneG0 = tex2D(_MainTex, gTexOffset);
				float4 sceneB1 = tex2D(_MainTex, lerp(bTexOffset, gTexOffset, 0.750));
				float4 sceneB2 = tex2D(_MainTex, lerp(bTexOffset, gTexOffset, 0.500));
				float4 sceneB3 = tex2D(_MainTex, lerp(bTexOffset, gTexOffset, 0.250));
				float4 sceneB4 = tex2D(_MainTex, bTexOffset);





				rValue += sceneR4 * 0.333;
				gValue += sceneG0;
				bValue += sceneB4 * 0.333;

				rValue += sceneR3 * 0.667;
				rValue += sceneR2;
				rValue += sceneR1 * 0.667;
				rValue += sceneG0 * 0.333;

				bValue += sceneB3 * 0.667;
				bValue += sceneB2;
				bValue += sceneB1 * 0.667;
				bValue += sceneG0 * 0.333;

				gValue += sceneR1 * 0.667;
				gValue += sceneB1 * 0.667;
				gValue += sceneR2 * 0.333;
				gValue += sceneB2 * 0.333;

				rValue *= 0.33333333333333333;
				gValue *= 0.33333333333333333;
				bValue *= 0.33333333333333333;

				//rValue = tex2D(_MainTex, rTexOffset);
				//gValue = tex2D(_MainTex, gTexOffset);
				//bValue = tex2D(_MainTex, bTexOffset);

				// Combine the offset colors.
				fixed4 col = float4(rValue.r, gValue.g, bValue.b, 1.0);

				return col;




			}
			ENDCG
		}
	}
}
