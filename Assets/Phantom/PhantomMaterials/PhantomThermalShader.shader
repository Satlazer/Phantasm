﻿Shader "Phantom/PhantomThermalShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Temperature ("Temperature", Range(0,1)) = 1.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" 
		"Queue" = "Transparent"}
		LOD 200
		
		ZTest Less
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
          	float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		float _Temperature;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = _Temperature * saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Alpha = c.a;
		}
		ENDCG


		// SEE THROUGH WALLS


		ZTest Always
		Blend One One


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
          	float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		float _Temperature;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = 0.05f * _Temperature * saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
