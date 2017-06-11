// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UVDisplacementAlpha"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_DispTex ("Displacement Texture", 2D) = "white" {}
		_Magnitude ("Magnitude", Range(0, 1)) = 0.05
		_Speed("Speed", Range(0, 5)) = 1
	}
	SubShader
	{
			Tags
		{
			"Queue" = "Transparent"
		}

			Pass
		{

			Blend One One

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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float4 _Color;
			sampler2D _MainTex;
			sampler2D _DispTex;
			float _Magnitude;
			float _Speed;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 distuv = float2(i.uv.x + _Time.x * _Speed, i.uv.y + _Time.x * _Speed);

				float2 disp = tex2D(_DispTex, distuv.xy).xy;
				disp = ((disp * 2) - 1) * _Magnitude;

				fixed4 col = tex2D(_MainTex, i.uv + disp);
				col *= _Color;
				col.w *= col.x;
				return col;
			}
			ENDCG
		}
	}
}
