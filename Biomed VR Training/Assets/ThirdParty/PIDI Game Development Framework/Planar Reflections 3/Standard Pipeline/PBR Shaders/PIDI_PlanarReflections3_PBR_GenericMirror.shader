/*
 * PIDI Planar Reflections 3
 * Developed  by : Jorge Pinal Negrete.
 * Copyright© 2015-2019, Jorge Pinal Negrete.  All Rights Reserved. 
 *  
*/

Shader "PIDI Shaders Collection/Planar Reflections 3/PBR/Generic Mirror" {
	Properties {
		
		
		[PerRendererData] _ReflectionTex("Reflection Tex", 2D) = "black"{}
		[Header(Reflection Settings)]
		_ReflectionTint("Reflection Tint", Color) = (1,1,1,1)


		[Space(8)][Header(Decal 0 Settings)][Space(8)]
		_Decal0Tint("Decal 0 Tint", Color) = (1,1,1,1)
		_Decal0Tex("Decal 0 Albedo", 2D) = "black"{}
		[NoScaleOffset] _Decal0Normal("Decal 0 Normal", 2D) = "bump"{}
		_Decal0Metallic("Decal 0 Metallic", Range(0,1)) = 0.5
		_Decal0Smoothness("Decal 0 Smoothness", Range(0,1)) = 0.5

		[Space(8)][Header(Decal 1 Settings)][Space(8)]
		_Decal1Tint("Decal 1 Tint", Color) = (1,1,1,1)
		_Decal1Tex("Decal 1 Albedo", 2D) = "black"{}
		[NoScaleOffset] _Decal1Normal("Decal 1 Normal", 2D) = "bump"{}
		_Decal1Metallic("Decal 1 Metallic", Range(0,1)) = 0.5
		_Decal1Smoothness("Decal 1 Smoothness", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 2.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_Decal0Tex;
			float2 uv_Decal1Tex;
			float4 screenPos;
		};


		sampler2D _ReflectionTex;

		sampler2D _Decal0Tex;
		sampler2D _Decal1Tex;
		sampler2D _Decal0Normal;
		sampler2D _Decal1Normal;

		half4 _ReflectionTint;
		half4 _Decal0Tint;
		half4 _Decal1Tint;

		half _Decal0Metallic;
		half _Decal1Metallic;
		half _Decal0Smoothness;
		half _Decal1Smoothness;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			float2 reflectionUV = IN.screenPos.xy/IN.screenPos.w;

			reflectionUV.x = 1-reflectionUV;


			half4 decal0Diff = tex2D(_Decal0Tex,IN.uv_Decal0Tex);
			half4 decal1Diff = tex2D(_Decal1Tex,IN.uv_Decal1Tex);

			half3 baseNormal = fixed3(0,0,1);

			o.Albedo = decal0Diff.rgb*_Decal0Tint*decal0Diff.a+decal1Diff.rgb*_Decal1Tint*decal1Diff.a*(1-decal0Diff.a);
			o.Normal = baseNormal*(1-max(decal0Diff.a,decal1Diff.a))+UnpackNormal(tex2D(_Decal0Normal,IN.uv_Decal0Tex))*decal0Diff.a+UnpackNormal(tex2D(_Decal0Normal,IN.uv_Decal0Tex))*decal1Diff.a*(1-decal0Diff.a);
			o.Metallic = _Decal0Metallic*decal0Diff.a+_Decal1Metallic*decal1Diff.a*(1-decal0Diff.a);
			o.Smoothness = _Decal0Smoothness*decal0Diff.a+_Decal1Smoothness*decal1Diff.a*(1-decal0Diff.a);
			o.Emission = tex2D(_ReflectionTex, reflectionUV)*_ReflectionTint*(1-max(decal0Diff.a,decal1Diff.a));
			o.Alpha = 1;
		
		}
		ENDCG
	}
	FallBack "Diffuse"
}
