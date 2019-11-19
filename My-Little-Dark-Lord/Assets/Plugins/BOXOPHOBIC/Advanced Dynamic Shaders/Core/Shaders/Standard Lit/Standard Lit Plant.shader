// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Plant"
{
	Properties
	{
		[HideInInspector]_Internal_Version("Internal_Version", Float) = 232
		[BBanner(ADS Standard Lit, Plant)]_ADSStandardLitPlant("< ADS Standard Lit Plant >", Float) = 1
		[BCategory(Rendering)]_RENDERINGG("[ RENDERINGG ]", Float) = 0
		[Enum(Two Sided,0,Back,1,Front,2)]_RenderFaces("Render Faces", Float) = 0
		_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[BCategory(Main)]_MAINN("[ MAINN ]", Float) = 0
		_Color("Plant Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex("Plant Albedo", 2D) = "white" {}
		_NormalScale("Plant Normal Scale", Float) = 1
		[NoScaleOffset]_NormalTex("Plant Normal", 2D) = "bump" {}
		[NoScaleOffset]_SurfaceTex("Plant Surface", 2D) = "white" {}
		_Smoothness("Plant Smoothness (A)", Range( 0 , 1)) = 1
		_SubsurfaceColor("Plant Subsurface (B)", Color) = (1,1,1,1)
		[BCategory(Settings)]_SETTINGS("[ SETTINGS ]", Float) = 0
		[HideInInspector]_MotionNoise("Motion Noise", Float) = 1
		_GlobalTurbulence("Global Turbulence", Range( 0 , 1)) = 1
		_GlobalTint("Global Tint", Range( 0 , 1)) = 1
		_GlobalSize("Global Size", Range( 0 , 1)) = 1
		[BCategory(Plant Motion)]_MOTIONPLANTT("[ MOTION PLANTT ]", Float) = 0
		[BMessage(Info, The Plant Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10)]_PlantMotionParameters("!!! Plant Motion Parameters !!!", Float) = 0
		_MotionAmplitude("Plant Motion Amplitude", Float) = 0
		_MotionSpeed("Plant Motion Speed", Float) = 0
		_MotionScale("Plant Motion Scale", Float) = 0
		_MotionVariation("Plant Motion Variation", Float) = 0
		_MotionVertical("Plant Motion Vertical", Range( 0 , 1)) = 0
		[BCategory(Leaf Motion)]_MOTIONLEAFF("[ MOTION LEAFF ]", Float) = 0
		[BMessage(Info, The Leaf Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10)]_LeafMotionParameters("!!! Leaf Motion Parameters !!!", Float) = 0
		_MotionAmplitude3("Leaf Motion Amplitude", Float) = 0
		_MotionSpeed3("Leaf Motion Speed", Float) = 0
		_MotionScale3("Leaf Motion Scale", Float) = 0
		[BCategory(Advanced)]_ADVANCEDD("[ ADVANCEDD ]", Float) = 0
		[BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0)]_BatchingInfo("!!! BatchingInfo", Float) = 0
		[HideInInspector]_Internal_ADS("Internal_ADS", Float) = 1
		[HideInInspector]_MetallicGlossMap("_MetallicGlossMap", 2D) = "white" {}
		[HideInInspector]_MainUVs("_MainUVs", Vector) = (1,1,0,0)
		[HideInInspector]_BumpMap("_BumpMap", 2D) = "white" {}
		[HideInInspector]_MainTex("_MainTex", 2D) = "white" {}
		[HideInInspector]_CullMode("_CullMode", Float) = 0
		[HideInInspector]_Glossiness("_Glossiness", Float) = 0
		[HideInInspector]_Mode("_Mode", Float) = 0
		[HideInInspector]_BumpScale("_BumpScale", Float) = 0
		[HideInInspector]_Internal_UnityToBoxophobic("_Internal_UnityToBoxophobic", Float) = 0
		[HideInInspector]_Internal_LitStandard("Internal_LitStandard", Float) = 1
		[HideInInspector]_Internal_TypePlant("Internal_TypePlant", Float) = 1
		[HideInInspector]_Internal_DebugMask("Internal_DebugMask", Float) = 1
		[HideInInspector]_Internal_DebugMask2("Internal_DebugMask2", Float) = 1
		[HideInInspector]_Internal_SetByScript("Internal_SetByScript", Float) = 0
		[HideInInspector]_Internal_DebugVariation("Internal_DebugVariation", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "DisableBatching" = "True" }
		LOD 300
		Cull [_RenderFaces]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		 
		//ADS Features
		//ADS End
		  
		#pragma exclude_renderers gles 
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float4 vertexToFrag1207;
			float4 screenPosition;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Transmission;
		};

		uniform float _MotionNoise;
		uniform half _Internal_SetByScript;
		uniform half _ADSStandardLitPlant;
		uniform half _SETTINGS;
		uniform half _ADVANCEDD;
		uniform half _MAINN;
		uniform half _LeafMotionParameters;
		uniform half _BatchingInfo;
		uniform half _PlantMotionParameters;
		uniform half _MOTIONPLANTT;
		uniform half _RENDERINGG;
		uniform half _Internal_LitStandard;
		uniform half _Internal_DebugVariation;
		uniform half _Internal_DebugMask;
		uniform half _Internal_TypePlant;
		uniform half _RenderFaces;
		uniform half _MOTIONLEAFF;
		uniform half _Internal_ADS;
		uniform half4 _MainUVs;
		uniform float _Mode;
		uniform float _Glossiness;
		uniform half _CullMode;
		uniform float _BumpScale;
		uniform sampler2D _MetallicGlossMap;
		uniform half _Internal_UnityToBoxophobic;
		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform half _Internal_DebugMask2;
		uniform half _Cutoff;
		uniform half _Internal_Version;
		uniform half ADS_GlobalScale;
		uniform float _MotionScale;
		uniform half ADS_GlobalSpeed;
		uniform float _MotionSpeed;
		uniform float _MotionVariation;
		uniform half ADS_GlobalAmplitude;
		uniform float _MotionAmplitude;
		uniform sampler2D ADS_TurbulenceTex;
		uniform half ADS_TurbulenceSpeed;
		uniform half ADS_TurbulenceScale;
		uniform half ADS_TurbulenceContrast;
		uniform float _GlobalTurbulence;
		uniform half3 ADS_GlobalDirection;
		uniform float _MotionVertical;
		uniform float _MotionScale3;
		uniform float _MotionSpeed3;
		uniform float _MotionAmplitude3;
		uniform half ADS_GlobalSizeMin;
		uniform half ADS_GlobalSizeMax;
		uniform sampler2D ADS_GlobalTex;
		uniform half4 ADS_GlobalUVs;
		uniform half _GlobalSize;
		uniform half _NormalScale;
		uniform sampler2D _NormalTex;
		uniform sampler2D _AlbedoTex;
		uniform half4 _Color;
		uniform half4 ADS_GlobalTintColorOne;
		uniform half4 ADS_GlobalTintColorTwo;
		uniform half ADS_GlobalTintIntensity;
		uniform half _GlobalTint;
		uniform sampler2D _SurfaceTex;
		uniform half _Smoothness;
		uniform half4 _SubsurfaceColor;


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half MotionScale60_g1891 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g1891 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g1891 = _Time.y * MotionSpeed62_g1891;
			float3 temp_output_95_0_g1891 = ( ( ase_worldPos * MotionScale60_g1891 ) + mulTime90_g1891 );
			half Packed_Variation1129 = v.color.a;
			half MotionVariation269_g1891 = ( _MotionVariation * Packed_Variation1129 );
			half MotionlAmplitude58_g1891 = ( ADS_GlobalAmplitude * _MotionAmplitude );
			float3 temp_output_92_0_g1891 = ( sin( ( temp_output_95_0_g1891 + MotionVariation269_g1891 ) ) * MotionlAmplitude58_g1891 );
			float3 temp_output_160_0_g1891 = ( temp_output_92_0_g1891 + MotionlAmplitude58_g1891 + MotionlAmplitude58_g1891 );
			float2 temp_cast_0 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g1798 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1798 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1798 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1798 = (float3(localunity_ObjectToWorld0w1_g1798 , localunity_ObjectToWorld1w2_g1798 , localunity_ObjectToWorld2w3_g1798));
			float2 panner73_g1796 = ( _Time.y * temp_cast_0 + ( (appendResult6_g1798).xz * ADS_TurbulenceScale ));
			float lerpResult136_g1796 = lerp( 1.0 , saturate( pow( abs( tex2Dlod( ADS_TurbulenceTex, float4( panner73_g1796, 0, 0.0) ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			half Motion_Turbulence1226 = lerpResult136_g1796;
			float3 lerpResult293_g1891 = lerp( temp_output_92_0_g1891 , temp_output_160_0_g1891 , Motion_Turbulence1226);
			half3 GlobalDirection349_g1891 = ADS_GlobalDirection;
			float3 lerpResult280_g1891 = lerp( GlobalDirection349_g1891 , float3(0,1,0) , _MotionVertical);
			half3 MotionDirection59_g1891 = mul( unity_WorldToObject, float4( lerpResult280_g1891 , 0.0 ) ).xyz;
			half Packed_Plant1134 = v.color.r;
			half MotionMask137_g1891 = Packed_Plant1134;
			float3 temp_output_94_0_g1891 = ( ( lerpResult293_g1891 * MotionDirection59_g1891 ) * MotionMask137_g1891 );
			half3 Motion_Plant1158 = temp_output_94_0_g1891;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 break311_g1808 = ase_vertex3Pos;
			half MotionFlutterScale60_g1808 = ( ADS_GlobalScale * _MotionScale3 );
			half MotionFlutterSpeed62_g1808 = ( ADS_GlobalSpeed * _MotionSpeed3 );
			float mulTime303_g1808 = _Time.y * MotionFlutterSpeed62_g1808;
			half MotionlFlutterAmplitude58_g1808 = ( ADS_GlobalAmplitude * _MotionAmplitude3 );
			half Packed_Leaf1169 = v.color.b;
			half MotionMask137_g1808 = Packed_Leaf1169;
			float3 ase_vertexNormal = v.normal.xyz;
			half3 Motion_Leaf1160 = ( sin( ( ( ( break311_g1808.x + break311_g1808.y + break311_g1808.z ) * MotionFlutterScale60_g1808 ) + mulTime303_g1808 ) ) * MotionlFlutterAmplitude58_g1808 * MotionMask137_g1808 * ase_vertexNormal );
			half3 Motion_Output1167 = ( ( Motion_Plant1158 + Motion_Leaf1160 ) * Motion_Turbulence1226 );
			half localunity_ObjectToWorld0w1_g1894 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1894 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1894 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1894 = (float3(localunity_ObjectToWorld0w1_g1894 , localunity_ObjectToWorld1w2_g1894 , localunity_ObjectToWorld2w3_g1894));
			float4 tex2DNode140_g1892 = tex2Dlod( ADS_GlobalTex, float4( ( ( (appendResult6_g1894).xz * (ADS_GlobalUVs).xy ) + (ADS_GlobalUVs).zw ), 0, 0.0) );
			half ADS_GlobalTex_B198_g1892 = tex2DNode140_g1892.b;
			float lerpResult156_g1892 = lerp( ADS_GlobalSizeMin , ADS_GlobalSizeMax , ADS_GlobalTex_B198_g1892);
			float3 temp_output_41_0_g1897 = ( ( lerpResult156_g1892 * _GlobalSize ) * ase_vertex3Pos );
			float3 lerpResult57_g1897 = lerp( temp_output_41_0_g1897 , -ase_vertex3Pos , ( 1.0 - max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) ) ));
			#ifdef LOD_FADE_CROSSFADE
				float3 staticSwitch40_g1897 = lerpResult57_g1897;
			#else
				float3 staticSwitch40_g1897 = temp_output_41_0_g1897;
			#endif
			#ifdef ADS_LODFADE_SCALE
				float3 staticSwitch58_g1897 = staticSwitch40_g1897;
			#else
				float3 staticSwitch58_g1897 = temp_output_41_0_g1897;
			#endif
			half3 Global_Size1211 = staticSwitch58_g1897;
			v.vertex.xyz += ( Motion_Output1167 + Global_Size1211 );
			float4 temp_cast_3 = (1.0).xxxx;
			half4 ADS_GlobalTintColorOne176_g1892 = ADS_GlobalTintColorOne;
			half4 ADS_GlobalTintColorTwo177_g1892 = ADS_GlobalTintColorTwo;
			half ADS_GlobalTex_R180_g1892 = tex2DNode140_g1892.r;
			float4 lerpResult147_g1892 = lerp( ADS_GlobalTintColorOne176_g1892 , ADS_GlobalTintColorTwo177_g1892 , ADS_GlobalTex_R180_g1892);
			half ADS_GlobalTintIntensity181_g1892 = ADS_GlobalTintIntensity;
			half GlobalTint186_g1892 = _GlobalTint;
			float4 lerpResult150_g1892 = lerp( temp_cast_3 , ( lerpResult147_g1892 * ADS_GlobalTintIntensity181_g1892 ) , GlobalTint186_g1892);
			o.vertexToFrag1207 = lerpResult150_g1892;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			half3 transmission = max(0 , -dot(s.Normal, gi.light.dir)) * gi.light.color * s.Transmission;
			half4 d = half4(s.Albedo * transmission , 0);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + d;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_NormalTex607 = i.uv_texcoord;
			float3 break17_g1898 = UnpackScaleNormal( tex2D( _NormalTex, uv_NormalTex607 ), _NormalScale );
			float switchResult12_g1898 = (((i.ASEVFace>0)?(break17_g1898.z):(-break17_g1898.z)));
			float3 appendResult18_g1898 = (float3(break17_g1898.x , break17_g1898.y , switchResult12_g1898));
			half3 Main_NormalTex620 = appendResult18_g1898;
			o.Normal = Main_NormalTex620;
			float2 uv_AlbedoTex18 = i.uv_texcoord;
			float4 tex2DNode18 = tex2D( _AlbedoTex, uv_AlbedoTex18 );
			half4 Main_AlbedoTex487 = tex2DNode18;
			half4 Main_Color486 = _Color;
			half4 Gloabl_Tint1210 = i.vertexToFrag1207;
			o.Albedo = saturate( ( Main_AlbedoTex487 * Main_Color486 * Gloabl_Tint1210 ) ).rgb;
			float2 uv_SurfaceTex645 = i.uv_texcoord;
			float4 tex2DNode645 = tex2D( _SurfaceTex, uv_SurfaceTex645 );
			half Main_SurfaceTex_A744 = tex2DNode645.a;
			half OUT_SMOOTHNESS660 = ( Main_SurfaceTex_A744 * _Smoothness );
			o.Smoothness = OUT_SMOOTHNESS660;
			half Main_SurfaceTex_B1230 = tex2DNode645.b;
			half4 OUT_TRANSMISSION1235 = ( Main_SurfaceTex_B1230 * _SubsurfaceColor );
			o.Transmission = OUT_TRANSMISSION1235.rgb;
			o.Alpha = 1;
			half Main_AlbedoTex_A616 = tex2DNode18.a;
			float temp_output_41_0_g1899 = Main_AlbedoTex_A616;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39_g1899 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g1899 = Dither4x4Bayer( fmod(clipScreen39_g1899.x, 4), fmod(clipScreen39_g1899.y, 4) );
			float temp_output_47_0_g1899 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g1899 = step( dither39_g1899, temp_output_47_0_g1899 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g1899 = ( temp_output_41_0_g1899 * dither39_g1899 );
			#else
				float staticSwitch40_g1899 = temp_output_41_0_g1899;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g1899 = staticSwitch40_g1899;
			#else
				float staticSwitch50_g1899 = temp_output_41_0_g1899;
			#endif
			clip( staticSwitch50_g1899 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Utils/ADS Fallback"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;1118.286;3148.984;1;True;False
Node;AmplifyShaderEditor.VertexColorNode;1124;-1280,-256;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1129;-1024,-128;Half;False;Packed_Variation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1225;-1280,0;Float;False;ADS Global Turbulence;15;;1796;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.RangedFloatNode;1174;-512,64;Float;False;Property;_MotionVariation;Plant Motion Variation;31;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1172;-512,144;Float;False;1129;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1169;-1024,-192;Half;False;Packed_Leaf;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1134;-1024,-256;Half;False;Packed_Plant;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1226;-1024,0;Half;False;Motion_Turbulence;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1227;-512,256;Float;False;1226;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1152;-512,336;Float;False;1134;Packed_Plant;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1204;-512,-16;Float;False;Property;_MotionVertical;Plant Motion Vertical;32;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1173;-256,64;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1139;768,-256;Float;False;Property;_MotionAmplitude3;Leaf Motion Amplitude;35;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1147;-512,-176;Float;False;Property;_MotionSpeed;Plant Motion Speed;29;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1150;768,-96;Float;False;Property;_MotionScale3;Leaf Motion Scale;37;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1153;768,-176;Float;False;Property;_MotionSpeed3;Leaf Motion Speed;36;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1143;-512,-256;Float;False;Property;_MotionAmplitude;Plant Motion Amplitude;28;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1144;768,0;Float;False;1169;Packed_Leaf;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1142;-512,-96;Float;False;Property;_MotionScale;Plant Motion Scale;30;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1242;64,-256;Float;False;ADS Motion Generic;25;;1891;81cab27e2a487a645a4ff5eb3c63bd27;6,252,1,278,1,228,1,292,2,330,1,326,1;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1224;1088,-256;Float;False;ADS Motion Flutter;-1;;1808;87d8028e5f83178498a65cfa9f0e9ace;1,312,0;5;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;136;FLOAT;0;False;310;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1158;384,-256;Half;False;Motion_Plant;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1160;1408,-256;Half;False;Motion_Leaf;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1161;1792,-160;Float;False;1160;Motion_Leaf;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;645;1152,-896;Float;True;Property;_SurfaceTex;Plant Surface;11;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;1221;-1280,896;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1163;1792,-256;Float;False;1158;Motion_Plant;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1206;-1280,768;Float;False;ADS Global Settings;19;;1892;0fe83146627632b4981f5a0aa1b63801;0;1;171;FLOAT;0;False;3;COLOR;85;COLOR;165;FLOAT;157
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1209;-1024,896;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1230;1536,-896;Half;False;Main_SurfaceTex_B;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;655;0,-896;Half;False;Property;_NormalScale;Plant Normal Scale;9;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;409;-592,-896;Half;False;Property;_Color;Plant Color;7;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;744;1536,-768;Half;False;Main_SurfaceTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1228;1792,-64;Float;False;1226;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-1280,-896;Float;True;Property;_AlbedoTex;Plant Albedo;8;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexToFragmentNode;1207;-768,768;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1164;2048,-256;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;749;1920,-896;Float;False;744;Main_SurfaceTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1210;-384,768;Half;False;Gloabl_Tint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;-976,-896;Half;False;Main_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;607;256,-896;Float;True;Property;_NormalTex;Plant Normal;10;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1233;2816,-800;Half;False;Property;_SubsurfaceColor;Plant Subsurface (B);13;0;Create;False;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1232;2816,-896;Float;False;1230;Main_SurfaceTex_B;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;1920,-816;Half;False;Property;_Smoothness;Plant Smoothness (A);12;0;Create;False;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1240;-768,896;Float;False;ADS LODFade Scale;-1;;1897;768eaebf5ab5e9748a01997bf1b9d313;0;1;41;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;-336,-896;Half;False;Main_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1166;2240,-256;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;-976,-768;Half;False;Main_AlbedoTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1167;2400,-256;Half;False;Motion_Output;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1083;-1280,-2112;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1211;-384,896;Half;False;Global_Size;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1280,-2176;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;745;2240,-896;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1234;3136,-896;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1213;-1280,-2032;Float;False;1210;Gloabl_Tint;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1231;592,-896;Float;False;Normal BackFace;-1;;1898;121446c878db06f4c847f9c5afed7cfe;0;1;13;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-1664;Float;False;616;Main_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1235;3328,-896;Half;False;OUT_TRANSMISSION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1175;-1280,-1536;Float;False;1167;Motion_Output;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1214;-1280,-1456;Float;False;1211;Global_Size;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;832,-896;Half;False;Main_NormalTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1074;-768,-2176;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;2400,-896;Half;False;OUT_SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1171;-528,-3360;Half;False;Property;_MOTIONLEAFF;[ MOTION LEAFF ];33;0;Create;True;0;0;True;1;BCategory(Leaf Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;624;-1280,-1920;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1201;-160,-2688;Half;False;Property;_Internal_ADS;Internal_ADS;40;1;[HideInInspector];Create;True;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1109;-576,-2176;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1239;-1024,-1664;Float;False;ADS LODFade Dither;-1;;1899;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1195;32,-2688;Half;False;Property;_Internal_TypePlant;Internal_TypePlant;52;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-1280,-2688;Half;False;Property;_RenderFaces;Render Faces;4;1;[Enum];Create;True;3;Two Sided;0;Back;1;Front;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1237;-1280,-1776;Float;False;1235;OUT_TRANSMISSION;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1179;-1024,-1536;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1243;-400,-2688;Float;False;ADS Internal Version;0;;1902;858e1f7f7bf8673449834f9aaa5bae83;0;0;1;FLOAT;5
Node;AmplifyShaderEditor.RangedFloatNode;862;-1120,-2688;Half;False;Property;_Cutoff;Cutout;5;0;Create;False;3;Off;0;Front;1;Back;2;0;True;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1199;-640,-2688;Float;False;Internal Unity Props;41;;1900;b286e6ef621b64a4fb35da1e13fa143f;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1238;1536,-2688;Float;False;ADS Features Support;-1;;1901;217a332a46517ae4cb8ca16677bdb217;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1192;752,-2688;Half;False;Property;_Internal_DebugMask2;Internal_DebugMask2;54;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1115;-1088,-3360;Half;False;Property;_MAINN;[ MAINN ];6;0;Create;True;0;0;True;1;BCategory(Main);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1223;-976,-3264;Half;False;Property;_LeafMotionParameters;!!! Leaf Motion Parameters !!!;34;0;Create;True;0;0;True;1;BMessage(Info, The Leaf Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1220;-672,-3264;Half;False;Property;_BatchingInfo;!!! BatchingInfo;39;0;Create;True;0;0;True;1;BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1118;-272,-3360;Half;False;Property;_ADVANCEDD;[ ADVANCEDD ];38;0;Create;True;0;0;True;1;BCategory(Advanced);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1193;1280,-2688;Half;False;Property;_Internal_SetByScript;Internal_SetByScript;55;1;[HideInInspector];Create;True;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1119;-1280,-3456;Half;False;Property;_ADSStandardLitPlant;< ADS Standard Lit Plant >;2;0;Create;True;0;0;True;1;BBanner(ADS Standard Lit, Plant);1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1116;-928,-3360;Half;False;Property;_SETTINGS;[ SETTINGS ];14;0;Create;True;0;0;True;1;BCategory(Settings);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1196;272,-2688;Half;False;Property;_Internal_LitStandard;Internal_LitStandard;51;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1202;1008,-2688;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;56;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1191;512,-2688;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;53;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;654;-1280,-1840;Float;False;660;OUT_SMOOTHNESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1222;-1280,-3264;Half;False;Property;_PlantMotionParameters;!!! Plant Motion Parameters !!!;27;0;Create;True;0;0;True;1;BMessage(Info, The Plant Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1117;-752,-3360;Half;False;Property;_MOTIONPLANTT;[ MOTION PLANTT ];24;0;Create;True;0;0;True;1;BCategory(Plant Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1113;-1280,-3360;Half;False;Property;_RENDERINGG;[ RENDERINGG ];3;0;Create;True;0;0;True;1;BCategory(Rendering);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-128,-2048;Float;False;True;2;Float;ADSShaderGUI;300;0;Standard;BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Plant;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;True;False;False;False;True;Off;0;False;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;550;10;False;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;300;Utils/ADS Fallback;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;1236;2816,-1024;Float;False;770.26;100;Transmission;0;;0.7843137,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1280,-2816;Float;False;417.3682;100;Rendering And Settings;0;;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1112;-1280,-3584;Float;False;1185.27;100;Drawers;0;;1,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;708;0,-1024;Float;False;1024.6;100;Normal Texture;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1198;-640,-2816;Float;False;2400.52;100;Internal Only;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;715;1152,-1024;Float;False;1473.26;100;Surface Input;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1170;-1280,-384;Float;False;3876.78;100;Motion;0;;0.03448272,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;760;-1280,-1024;Float;False;1152.612;100;Main Texture and Color;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1212;-1280,640;Float;False;1090.415;100;Globals;0;;1,0.7686275,0,1;0;0
WireConnection;1129;0;1124;4
WireConnection;1169;0;1124;3
WireConnection;1134;0;1124;1
WireConnection;1226;0;1225;85
WireConnection;1173;0;1174;0
WireConnection;1173;1;1172;0
WireConnection;1242;220;1143;0
WireConnection;1242;221;1147;0
WireConnection;1242;222;1142;0
WireConnection;1242;218;1173;0
WireConnection;1242;287;1227;0
WireConnection;1242;136;1152;0
WireConnection;1242;279;1204;0
WireConnection;1224;220;1139;0
WireConnection;1224;221;1153;0
WireConnection;1224;222;1150;0
WireConnection;1224;136;1144;0
WireConnection;1158;0;1242;0
WireConnection;1160;0;1224;0
WireConnection;1209;0;1206;157
WireConnection;1209;1;1221;0
WireConnection;1230;0;645;3
WireConnection;744;0;645;4
WireConnection;1207;0;1206;85
WireConnection;1164;0;1163;0
WireConnection;1164;1;1161;0
WireConnection;1210;0;1207;0
WireConnection;487;0;18;0
WireConnection;607;5;655;0
WireConnection;1240;41;1209;0
WireConnection;486;0;409;0
WireConnection;1166;0;1164;0
WireConnection;1166;1;1228;0
WireConnection;616;0;18;4
WireConnection;1167;0;1166;0
WireConnection;1211;0;1240;0
WireConnection;745;0;749;0
WireConnection;745;1;294;0
WireConnection;1234;0;1232;0
WireConnection;1234;1;1233;0
WireConnection;1231;13;607;0
WireConnection;1235;0;1234;0
WireConnection;620;0;1231;0
WireConnection;1074;0;36;0
WireConnection;1074;1;1083;0
WireConnection;1074;2;1213;0
WireConnection;660;0;745;0
WireConnection;1109;0;1074;0
WireConnection;1239;41;791;0
WireConnection;1179;0;1175;0
WireConnection;1179;1;1214;0
WireConnection;0;0;1109;0
WireConnection;0;1;624;0
WireConnection;0;4;654;0
WireConnection;0;6;1237;0
WireConnection;0;10;1239;0
WireConnection;0;11;1179;0
ASEEND*/
//CHKSM=9CD6CF52E2788B636F4F61FE20ED8C266F2C5A59