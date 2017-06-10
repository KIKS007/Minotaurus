// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BourelShield"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags 
		{ 
			"RenderType" = "Transparent"
			"Queue"="Transparent" 
		}

		Pass
		{
			
			Blend One One
			ZWrite off //prevent artefacts
			Cull Off //see through object
			
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 screenuv : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 objectPos : TEXCOORD3;
				float depth : DEPTH;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//intersection
				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) / 2;
				o.screenuv.y = 1 - o.screenuv.y; //platform specific - potentially useless
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;

				//outline
				o.objectPos = v.vertex.xyz;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}

			float4 _Color;
			sampler2D _CameraDepthNormalsTexture; 
			sampler2D _DispTex;
			float _Magnitude;
			float _Speed;
			
			fixed4 frag (v2f i) : SV_Target
			{

				//edges
				float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
				float diff = screenDepth - i.depth;
				float intersect = 0;

				if (diff > 0)
					intersect = 1 - smoothstep(0, _ProjectionParams.w * 0.5, diff);

				//outline 
				float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * 2;
				float glow = max(intersect, rim);

				//color
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 niceColor = fixed4(lerp(_Color.rgb, fixed3(1, 1, 1), pow(glow, 4)), 1);
				col *= niceColor + glow;

				return col;
			}
			ENDCG
		}
	}
}