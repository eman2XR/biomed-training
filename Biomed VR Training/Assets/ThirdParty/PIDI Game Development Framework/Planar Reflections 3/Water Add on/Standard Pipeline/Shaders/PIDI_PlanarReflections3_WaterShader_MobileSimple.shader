/*
 * PIDI Planar Reflections 3
 * Developed  by : Jorge Pinal Negrete.
 * Copyright(c) 2015-2020, Jorge Pinal Negrete.  All Rights Reserved. 
 *  
*/

Shader "PIDI Shaders Collection/Planar Reflections 3/Water Shaders/Mobile Water" {
	Properties {
		
		[Header(Water Settings)][Space(8)]
		_SurfaceColor( "Surface Color", Color ) = (0,0.6,0.85,1)	
		_ReflectionTint( "Reflection Tint", Color ) = (0,0.6,0.85,1)			
		_WaterBottom( "Bottom Color", Color ) = (0,0.4,0.55,1)	
		

		[Header(Waves Settings)][Space(8)]
		_WavesDistortion( "Waves Distortion", Range(0.005, 0.1)) = 0.03
		_WavesSpeed( "Waves Speed ( XY / ZW )", Vector ) = (1,0.3,-0.5,0.15)
		_WavesPettern( "Waves Pattern", 2D ) = "bump"{}
				

		[Header(PBR Properties)][Space(8)]
		_Glossiness( "Smoothness", Range(0,1) ) = 0.5
		_SpecColor( "Specular Color", Color) = (0.3,0.3,0.3,1)


		[PerRendererData]_ReflectionTex("Reflection Tex", 2D) = "black"{}


	}

	SubShader {
		Tags { "RenderType"="Geometry" "Queue"="Transparent-200" }
		LOD 200

		GrabPass{ "_WaterRefraction" }


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular noshadow vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 2.0

		sampler2D _ReflectionTex;
		sampler2D _WaterRefraction;
		sampler2D _CameraDepthTexture;
		
		sampler2D _WaterColors;
		sampler2D _WavesPettern;
		sampler2D _FoamTexture;

		float4 _WavesPettern_ST;
		float4 _FoamTexture_ST;

		fixed4 _ReflectionTint;
		fixed4 _SurfaceColor;
		fixed4 _WaterBottom;
		fixed4 _FoamColor;


		half4 _WavesSpeed;

		half _WaterBottomLevel;
		half _WaterClarity;
		half _WavesDistortion;
		half _FoamWidth;
		half _StaticFoam;
		half _Glossiness;



		struct Input {
			float4 screenPos;
			float3 worldPos;
			float eyeDepth;
			float3 viewDir;
		};

		void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            COMPUTE_EYEDEPTH(o.eyeDepth);
        }

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {

			fixed2 reflectionUV = IN.screenPos.xy/max(IN.screenPos.w, 0.0000001);
			fixed2 refractionUV = reflectionUV;
			reflectionUV.x = 1-reflectionUV.x;

			float3 wavesOne = UnpackNormal(tex2D(_WavesPettern,(IN.worldPos.xz+_Time.y*_WavesSpeed.xy)*_WavesPettern_ST.xy));
			float3 wavesTwo = UnpackNormal(tex2D(_WavesPettern,(IN.worldPos.xz+_Time.y*_WavesSpeed.zw)*_WavesPettern_ST.zw));

			float3 finalWaves = BlendNormals(wavesOne, wavesTwo);

			reflectionUV += finalWaves*_WavesDistortion;
			refractionUV += finalWaves*_WavesDistortion;

			fixed4 refColor = tex2D(_ReflectionTex, reflectionUV);

			o.Albedo = _SurfaceColor;
			o.Specular = _SpecColor;
			o.Smoothness = _Glossiness;
			o.Emission =  lerp(refColor*_ReflectionTint,tex2D(_WaterRefraction,refractionUV)*_WaterBottom,pow(dot(normalize(IN.viewDir),o.Normal),1.5));//;//;
			o.Normal = normalize(finalWaves);
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Transparent"
}
