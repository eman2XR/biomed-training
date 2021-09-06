/*
 * PIDI Planar Reflections 3
 * Developed  by : Jorge Pinal Negrete.
 * Copyright(c) 2015-2020, Jorge Pinal Negrete.  All Rights Reserved. 
 *  
*/

Shader "PIDI Shaders Collection/Planar Reflections 3/Water Shaders/Advanced (Tessellation)" {
	Properties {
		[Header(Tessellation Settings)][Space(8)]
		_EdgeLength ("Edge Length", Range(2,50)) = 15
		_DispStrength ("Displacement Strength", Range(0,4)) = 0.5

		[Space(8)][Header(Water Settings)][Space(8)]
		_WaterClarity( "Water Clarity", Range(0,1)) = 0.5
		_WaterBottomLevel( "Water Bottom Level", Range(1,10)) = 0
		_ReflectionTint( "Reflection Tint", Color ) = (0,0.6,0.85,1)		
		_SurfaceColor( "Surface Color", Color ) = (0,0.6,0.85,1)		
		_WaterColors( "Water Colors Gradient", 2D ) = "white"{}
		

		[Header(Waves Settings)][Space(8)]
		_WavesDistortion( "Waves Distortion", Range(0.005, 0.1)) = 0.03
		_WavesSpeed( "Waves Speed ( XY / ZW )", Vector ) = (1,0.3,-0.5,0.15)
		_WavesPattern( "Waves Pattern", 2D ) = "bump"{}

		[Header(Foam Settings)][Space(8)]
		[Toggle]_StaticFoam("Static Foam", Float) = 0
		_FoamColor("Foam Color", Color) = (0.8,0.8,0.8,1)
		_FoamWidth("Foam Width", Range(0,1)) = 0.5
		_FoamTexture( "Foam Texture", 2D ) = "black"{}
		

		[Header(PBR Properties)][Space(8)]
		_Glossiness( "Smoothness", Range(0,1) ) = 0.5
		_SpecColor( "Specular Color", Color) = (0.3,0.3,0.3,1)


		[PerRendererData]_ReflectionTex("Reflection Tex", 2D) = "black"{}


	}

	SubShader {
		Tags { "RenderType"="Alpha-Test" "Queue"="Transparent-200" }
		LOD 200

		GrabPass{ "_WaterRefraction" }



		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular noshadow vertex:vert tessellate:tessEdge

		#pragma target 4.6
        #include "Tessellation.cginc"

		sampler2D _ReflectionTex;
		sampler2D _WaterRefraction;
		sampler2D _CameraDepthTexture;
		
		sampler2D _WaterColors;
		sampler2D _WavesPattern;
		sampler2D _FoamTexture;

		float4 _WavesPattern_ST;
		float4 _FoamTexture_ST;

		fixed4 _ReflectionTint;
		fixed4 _SurfaceColor;
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
			float4 eyeDepth:COLOR;
			float3 viewDir;
		};

		float _EdgeLength;
		float _DispStrength;
		
		float4 tessEdge (appdata_full v0, appdata_full v1, appdata_full v2){
                return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength*4);
        }

		void vert (inout appdata_full v)
        {	
			half3 worldPos = mul( unity_ObjectToWorld , v.vertex).xyz;

			float3 wavesOne = UnpackNormal(tex2Dlod(_WavesPattern,float4((worldPos.xz+_Time.y*_WavesSpeed.xy*5)*_WavesPattern_ST.xy*0.05,1,1)));

			half d = wavesOne.x*_DispStrength;

			v.vertex.xyz += v.normal * d;
            COMPUTE_EYEDEPTH(v.color.r);
        }



		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {

			fixed2 reflectionUV = IN.screenPos.xy/max(IN.screenPos.w, 0.0000001);
			fixed2 refractionUV = reflectionUV;
			reflectionUV.x = 1-reflectionUV.x;

			float3 wavesOne = UnpackNormal(tex2D(_WavesPattern,(IN.worldPos.xz+_Time.y*_WavesSpeed.xy)*_WavesPattern_ST.xy));
			float3 wavesTwo = UnpackNormal(tex2D(_WavesPattern,(IN.worldPos.xz+_Time.y*_WavesSpeed.zw)*_WavesPattern_ST.zw));

			float3 finalWaves = BlendNormals(wavesOne, wavesTwo);

			reflectionUV += finalWaves*_WavesDistortion;
			refractionUV += finalWaves*_WavesDistortion;

			half depth = tex2Dproj(_CameraDepthTexture,IN.screenPos);

			half sceneDepth = (LinearEyeDepth(depth)-_WaterClarity-IN.eyeDepth)/_WaterBottomLevel;

			fixed4 refColor = tex2D(_ReflectionTex, reflectionUV);

			float finalTransparency = saturate( pow(saturate((LinearEyeDepth(depth)-_WaterClarity-IN.eyeDepth)),0.8) );

			fixed4 c = tex2D(_WaterColors,float2(1-sceneDepth,0));
			float foamSway = pow(1-saturate((sin(_Time.y*1.6 + UNITY_PI / 2.0)+1.0)/2.0+(LinearEyeDepth(depth)-(_FoamWidth-0.25)-IN.eyeDepth.r)),2);
			o.Albedo = finalTransparency * saturate(lerp(foamSway,1-saturate(LinearEyeDepth(depth)-(_FoamWidth-0.25)-IN.eyeDepth.r),_StaticFoam)*1.5*_FoamColor*tex2D(_FoamTexture,(IN.worldPos.xz+_Time.y*_WavesSpeed.xy)*_FoamTexture_ST.xy)+lerp(_SurfaceColor,0,pow(dot(normalize(IN.viewDir),o.Normal),1.5)));
			
			fixed4 refraction = tex2D(_WaterRefraction,refractionUV);

			o.Specular = _SpecColor;
			o.Smoothness = _Glossiness;
			o.Emission =  lerp(refColor*_ReflectionTint, refraction *c.rgb,pow(dot(normalize(IN.viewDir),o.Normal),1.5));//;//;
			o.Normal = normalize(finalWaves);
			o.Emission = lerp( refraction.rgb, o.Emission, finalTransparency );
		}
		ENDCG
	}
	FallBack "Transparent"
}
