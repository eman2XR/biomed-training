/*
 * PIDI Planar Reflections 3
 * Developed  by : Jorge Pinal Negrete.
 * Copyright(c) 2015-2019, Jorge Pinal Negrete.  All Rights Reserved. 
 *  
*/

Shader "PIDI Shaders Collection/Planar Reflections 3/Unlit/Broken Reflections"
{
	Properties
	{
		[PerRendererData]_ReflectionTex ("Reflection Texture", 2D) = "black" {}
		_ReflectionTint("Reflection Tint", Color) = (1,1,1,1)
		_BrokenPattern("Broken Pattern Hor(R) Ver(G) Dir(B)",2D) = "gray"{}
		_BrokenOffsetX("Broken Offset X", Range(-1,1)) = 0
		_BrokenOffsetY("Broken Offset Y", Range(-1,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 screenPos : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
			};

			sampler2D _ReflectionTex;
			float4 _ReflectionTex_ST;


			sampler2D _BrokenPattern;
			float4 _BrokenPattern_ST;

			half _BrokenOffsetX;
			half _BrokenOffsetY;

			half4 _ReflectionTint;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeGrabScreenPos(o.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				i.screenPos.xy /= i.screenPos.w;
				i.screenPos.x = 1-i.screenPos.x;
					
				half4 broken = tex2D(_BrokenPattern, i.uv*_BrokenPattern_ST.xy+_BrokenPattern_ST.zw);

				i.screenPos.x += broken.r*_BrokenOffsetX*sign(broken.b-0.5);
				i.screenPos.y += broken.g*_BrokenOffsetY*sign(broken.b-0.5);

				fixed4 col = tex2D(_ReflectionTex, i.screenPos.xy)*_ReflectionTint;

				return col;
			}
			ENDCG
		}
	}
}
