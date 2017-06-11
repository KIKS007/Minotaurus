// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ToonShader"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.0001
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_MainColor("MainColor", Color) = (1,1,1,0)
		[HDR]_RimColor("RimColor", Color) = (0,1,0.8758622,0)
		_RimPower("RimPower", Range( 0 , 10)) = 0.5
		_RimOffset("RimOffset", Float) = 0.24
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", 2D) = "black" {}
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_EmissionValue("EmissionValue", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:outlineVertexDataFunc
		uniform fixed4 _ASEOutlineColor;
		uniform fixed _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o ) { o.Emission = _ASEOutlineColor.rgb; o.Alpha = 1; }
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _MainColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _ToonRamp;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimColor;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _EmissionColor;
		uniform float _EmissionValue;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#if DIRECTIONAL
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode42 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_lightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float2 temp_cast_1 = (saturate( (dot( WorldNormalVector( i , tex2DNode42 ) , ase_lightDir )*0.5 + 0.5) )).xx;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_indirectDiffuse = ShadeSH9( float4( ase_worldNormal, 1 ) );
			float3 temp_cast_3 = (ase_lightAtten).xxx;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			c.rgb = ( ( ( ( ( _MainColor * tex2D( _Albedo, uv_Albedo ) ) * tex2D( _ToonRamp, temp_cast_1 ) ) * ( _LightColor0 * float4( ( ase_indirectDiffuse + temp_cast_3 ) , 0.0 ) ) ) + ( saturate( ( ( ase_lightAtten * dot( WorldNormalVector( i , tex2DNode42 ) , ase_lightDir ) ) * pow( ( 1.0 - saturate( ( dot( WorldNormalVector( i , tex2DNode42 ) , worldViewDir ) + _RimOffset ) ) ) , _RimPower ) ) ) * ( _RimColor * _LightColor0 ) ) ) + ( ( tex2D( _Emission, uv_Emission ) + _EmissionColor ) * _EmissionValue ) ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10005
2567;238;2546;1014;1706.308;353.8424;1;True;False
Node;AmplifyShaderEditor.SamplerNode;42;-3237.42,-213.8218;Float;True;Property;_Normal;Normal;6;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;1;-2513.212,167.2564;Float;False;507.201;385.7996;Comment;3;7;3;4;N . V;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;3;-2417.212,375.2564;Float;False;World;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;2;-1841.213,407.2564;Float;False;1617.938;553.8222;;13;33;30;29;23;22;26;20;21;16;18;13;10;5;Rim Light;0;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-2465.212,215.2564;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;6;-2513.212,-328.7436;Float;False;540.401;320.6003;Comment;3;15;8;9;N . L;0;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1761.213,679.2564;Float;False;Property;_RimOffset;RimOffset;4;0;0.24;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;7;-2161.212,295.2564;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;14;-1379.213,-40.74359;Float;False;812;304;Comment;3;11;36;37;Attenuation and Ambient;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-1553.213,567.2564;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WorldNormalVector;8;-2401.212,-280.7436;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;9;-2449.212,-120.7436;Float;False;1;0;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.CommentaryNode;11;-1640.312,-476.4431;Float;False;723.599;290;Also know as Lambert Wrap or Half Lambert;1;12;Diffuse Wrap;0;0
Node;AmplifyShaderEditor.SaturateNode;13;-1393.213,567.2564;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-1590.312,-301.4431;Float;False;Constant;_Float3;Float 3;0;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;15;-2113.212,-216.7436;Float;False;2;0;FLOAT3;0.0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-1329.213,695.2564;Float;False;Property;_RimPower;RimPower;3;0;0.5;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.LightAttenuation;36;-1295.398,177.606;Float;False;0;1;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;18;-1217.213,567.2564;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ScaleAndOffsetNode;17;-1324.91,-426.4431;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;19;-1091.712,-419.6435;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.IndirectDiffuseLighting;37;-1227.098,94.90607;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;24;-1016.311,-794.7427;Float;True;Property;_Albedo;Albedo;5;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Misc/Checkers.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PowerNode;21;-1025.213,567.2564;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1041.213,455.2564;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;70;-1010.943,-1003.19;Float;False;Property;_MainColor;MainColor;1;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-883.2125,119.2564;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.LightColorNode;26;-753.2125,855.2564;Float;False;0;1;COLOR
Node;AmplifyShaderEditor.ColorNode;22;-865.2125,679.2564;Float;False;Property;_RimColor;RimColor;2;1;[HDR];0,1,0.8758622,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;25;-721.2125,-440.7436;Float;True;Property;_ToonRamp;ToonRamp;0;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Misc/WarmToonRamp.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LightColorNode;28;-1267.213,7.256409;Float;False;0;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-581.9437,-770.4906;Float;False;2;0;COLOR;0.0;False;1;FLOAT4;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-785.2125,535.2564;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-723.2125,7.256409;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;63;176.5607,-602.3899;Float;True;Property;_Emission;Emission;7;0;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;66;189.9607,-410.09;Float;False;Property;_EmissionColor;EmissionColor;8;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-305.2125,-456.7436;Float;False;2;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SaturateNode;30;-593.2125,535.2564;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-561.2125,663.2564;Float;False;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-11.61234,-207.5436;Float;False;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-401.2125,535.2564;Float;False;2;0;FLOAT;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;64;224.9607,-240.7899;Float;False;Property;_EmissionValue;EmissionValue;9;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;67;522.9607,-418.09;Float;False;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;650.9607,-184.09;Float;False;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;35;334.7875,23.25641;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;69;842.9607,20.91003;Float;False;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1057.369,-24.29474;Float;False;True;6;Float;ASEMaterialInspector;0;CustomLighting;ToonShader;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;32;0;0.5;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;True;0.0001;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;2;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;42;0
WireConnection;7;0;4;0
WireConnection;7;1;3;0
WireConnection;10;0;7;0
WireConnection;10;1;5;0
WireConnection;8;0;42;0
WireConnection;13;0;10;0
WireConnection;15;0;8;0
WireConnection;15;1;9;0
WireConnection;18;0;13;0
WireConnection;17;0;15;0
WireConnection;17;1;12;0
WireConnection;17;2;12;0
WireConnection;19;0;17;0
WireConnection;21;0;18;0
WireConnection;21;1;16;0
WireConnection;20;0;36;0
WireConnection;20;1;15;0
WireConnection;27;0;37;0
WireConnection;27;1;36;0
WireConnection;25;1;19;0
WireConnection;73;0;70;0
WireConnection;73;1;24;0
WireConnection;23;0;20;0
WireConnection;23;1;21;0
WireConnection;31;0;28;0
WireConnection;31;1;27;0
WireConnection;32;0;73;0
WireConnection;32;1;25;0
WireConnection;30;0;23;0
WireConnection;29;0;22;0
WireConnection;29;1;26;0
WireConnection;34;0;32;0
WireConnection;34;1;31;0
WireConnection;33;0;30;0
WireConnection;33;1;29;0
WireConnection;67;0;63;0
WireConnection;67;1;66;0
WireConnection;68;0;67;0
WireConnection;68;1;64;0
WireConnection;35;0;34;0
WireConnection;35;1;33;0
WireConnection;69;0;35;0
WireConnection;69;1;68;0
WireConnection;0;2;69;0
ASEEND*/
//CHKSM=1692AEEDA62BE98E6825D898830313B7A9918834