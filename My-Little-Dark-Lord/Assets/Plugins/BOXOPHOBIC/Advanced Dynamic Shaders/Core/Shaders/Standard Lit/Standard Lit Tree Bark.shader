// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Tree Bark"
{
	Properties
	{
		[HideInInspector]_Internal_Version("Internal_Version", Float) = 232
		[BBanner(ADS Standard Lit, Tree Bark)]_ADSStandardLitTreeBark("< ADS Standard Lit Tree Bark>", Float) = 1
		[BInteractive(_Mode, 1)]_RenderCutt("# RenderCutt", Float) = 0
		[HideInInspector]_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[BCategory(Trunk)]_TRUNKK("[ TRUNKK ]", Float) = 0
		_Color("Trunk Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex("Trunk Albedo", 2D) = "white" {}
		_NormalScale("Trunk Normal Scale", Float) = 1
		[NoScaleOffset]_NormalTex("Trunk Normal", 2D) = "bump" {}
		[NoScaleOffset]_SurfaceTex("Trunk Surface", 2D) = "white" {}
		_Smoothness("Trunk Smoothness (A)", Range( 0 , 1)) = 0.5
		_Occlusion("Trunk Occlusion (G)", Range( 0 , 1)) = 0
		_TrunkVariation("Trunk Variation", Range( 0 , 1)) = 1
		[Space(10)]_UVZero("Trunk UVs", Vector) = (1,1,0,0)
		[BCategory(Base)]_BASEE("[ BASEE ]", Float) = 0
		[Toggle(_ENABLEBASE_ON)] _EnableBase("Enable Base", Float) = 0
		[BInteractive(_ENABLEBASE_ON)]_BlendingBasee("# BlendingBasee", Float) = 0
		_Color3("Base Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex3("Base Albedo", 2D) = "white" {}
		_NormalScale3("Base Normal Scale", Float) = 1
		[NoScaleOffset][Normal]_NormalTex3("Base Normal", 2D) = "bump" {}
		[NoScaleOffset]_SurfaceTex3("Base Surface", 2D) = "white" {}
		_Smoothness3("Base Smoothness (A)", Range( 0 , 1)) = 0.5
		_Occlusion3("Base Occlusion (G)", Range( 0 , 1)) = 0
		[Space(10)]_UVZero3("Base UVs", Vector) = (1,1,0,0)
		_BaseBlendAmount("Base Blend Amount", Range( 0.0001 , 1)) = 0.1
		_BaseBlendVariation("Base Blend Variation", Range( 0.0001 , 1)) = 0.1
		_BaseBlendContrast("Base Blend Contrast", Range( 0 , 0.999)) = 0.9
		_BaseBlendNormals("Base Blend Normals", Range( 0 , 1)) = 0
		[BCategory(Settings)]_SETTINGSS("[ SETTINGSS ]", Float) = 0
		[HideInInspector]_MotionNoise("Motion Noise", Float) = 1
		_GlobalTurbulence("Global Turbulence", Range( 0 , 1)) = 1
		_VertexOcclusion("Vertex Occlusion", Range( 0 , 1)) = 0
		[BCategory(Trunk Motion)]_TRUNKMOTIONN("[ TRUNK MOTIONN ]", Float) = 0
		[BMessage(Info, The Trunk Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10)]_TrunkMotionParameters("!!! Trunk Motion Parameters !!!", Float) = 0
		_MotionAmplitude("Trunk Motion Amplitude", Float) = 0
		_MotionSpeed("Trunk Motion Speed", Float) = 0
		_MotionScale("Trunk Motion Scale", Float) = 0
		[BCategory(Branch Motion)]_BRANCHMOTIONN("[ BRANCH MOTIONN ]", Float) = 0
		[BMessage(Info, The Branch Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10)]_BranchMotionParameters("!!! Branch Motion Parameters !!!", Float) = 0
		_MotionAmplitude2("Branch Motion Amplitude", Float) = 0
		_MotionSpeed2("Branch Motion Speed", Float) = 0
		_MotionScale2("Branch Motion Scale", Float) = 0
		_MotionVariation2("Branch Motion Variation", Float) = 0
		_MotionVertical2("Branch Motion Vertical", Range( 0 , 1)) = 0
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
		[HideInInspector]_Internal_TypeTreeBark("Internal_TypeTreeBark", Float) = 1
		[HideInInspector]_Internal_DebugMask("Internal_DebugMask", Float) = 1
		[HideInInspector]_Internal_DebugMask2("Internal_DebugMask2", Float) = 1
		[HideInInspector]_Internal_DebugVariation("Internal_DebugVariation", Float) = 1
		[HideInInspector]_Internal_SetByScript("Internal_SetByScript", Float) = 0
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "DisableBatching" = "True" }
		LOD 300
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _ENABLEBASE_ON
		 
		//ADS Features
		//ADS End
		  
		#pragma exclude_renderers gles 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float2 uv4_texcoord4;
			float4 vertexColor : COLOR;
			float4 uv_tex4coord;
			float4 screenPosition;
		};

		uniform float _MotionNoise;
		uniform half _BASEE;
		uniform half _Internal_TypeTreeBark;
		uniform half _ADSStandardLitTreeBark;
		uniform half _TrunkMotionParameters;
		uniform half _TRUNKK;
		uniform half _BRANCHMOTIONN;
		uniform half _TRUNKMOTIONN;
		uniform half _BlendingBasee;
		uniform half _Internal_SetByScript;
		uniform half _Internal_DebugVariation;
		uniform half _Internal_DebugMask2;
		uniform half _ADVANCEDD;
		uniform half _SETTINGSS;
		uniform half _Internal_DebugMask;
		uniform half _BatchingInfo;
		uniform half _Internal_ADS;
		uniform half _RenderCutt;
		uniform half _BranchMotionParameters;
		uniform half4 _MainUVs;
		uniform float _Mode;
		uniform float _Glossiness;
		uniform half _CullMode;
		uniform float _BumpScale;
		uniform sampler2D _MetallicGlossMap;
		uniform half _Internal_UnityToBoxophobic;
		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform half _Internal_LitStandard;
		uniform half _Internal_Version;
		uniform half ADS_GlobalScale;
		uniform float _MotionScale;
		uniform half ADS_GlobalSpeed;
		uniform float _MotionSpeed;
		uniform half ADS_GlobalAmplitude;
		uniform float _MotionAmplitude;
		uniform half3 ADS_GlobalDirection;
		uniform half ADS_GlobalLeavesAmount;
		uniform half ADS_GlobalLeavesVar;
		uniform float _MotionScale2;
		uniform float _MotionSpeed2;
		uniform float _MotionVariation2;
		uniform float _MotionAmplitude2;
		uniform float _MotionVertical2;
		uniform sampler2D ADS_TurbulenceTex;
		uniform half ADS_TurbulenceSpeed;
		uniform half ADS_TurbulenceScale;
		uniform half ADS_TurbulenceContrast;
		uniform float _GlobalTurbulence;
		uniform half _NormalScale;
		uniform sampler2D _NormalTex;
		uniform half4 _UVZero;
		uniform half _TrunkVariation;
		uniform half _NormalScale3;
		uniform sampler2D _NormalTex3;
		uniform half4 _UVZero3;
		uniform sampler2D _AlbedoTex;
		uniform half _BaseBlendAmount;
		uniform half _BaseBlendVariation;
		uniform float _BaseBlendContrast;
		uniform half _BaseBlendNormals;
		uniform sampler2D _AlbedoTex3;
		uniform half4 _Color;
		uniform half4 _Color3;
		uniform sampler2D _SurfaceTex;
		uniform half _Smoothness;
		uniform sampler2D _SurfaceTex3;
		uniform half _Smoothness3;
		uniform half _VertexOcclusion;
		uniform half _Occlusion;
		uniform half _Occlusion3;
		uniform half _Cutoff;


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
			half MotionScale60_g2031 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g2031 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g2031 = _Time.y * MotionSpeed62_g2031;
			float2 temp_output_95_0_g2031 = ( ( (ase_worldPos).xz * MotionScale60_g2031 ) + mulTime90_g2031 );
			half MotionlAmplitude58_g2031 = ( ADS_GlobalAmplitude * _MotionAmplitude );
			float2 temp_output_92_0_g2031 = ( sin( temp_output_95_0_g2031 ) * MotionlAmplitude58_g2031 );
			float2 temp_output_160_0_g2031 = ( temp_output_92_0_g2031 + MotionlAmplitude58_g2031 + MotionlAmplitude58_g2031 );
			half3 GlobalDirection349_g2031 = ADS_GlobalDirection;
			float3 break339_g2031 = mul( unity_WorldToObject, float4( GlobalDirection349_g2031 , 0.0 ) ).xyz;
			float2 appendResult340_g2031 = (float2(break339_g2031.x , break339_g2031.z));
			half2 MotionDirection59_g2031 = appendResult340_g2031;
			half Packed_Trunk21634 = ( v.color.r * v.color.r );
			half ADS_TreeLeavesAffectMotion168_g2011 = 0.2;
			half ADS_TreeLeavesAmount157_g2011 = ADS_GlobalLeavesAmount;
			half localunity_ObjectToWorld0w1_g2012 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g2012 = ( unity_ObjectToWorld[2].w );
			float temp_output_142_0_g2011 = saturate( ( ADS_TreeLeavesAmount157_g2011 - ( frac( ( localunity_ObjectToWorld0w1_g2012 + localunity_ObjectToWorld2w3_g2012 ) ) * ADS_GlobalLeavesVar ) ) );
			half LeavesAmountSimple172_g2011 = temp_output_142_0_g2011;
			float lerpResult156_g2011 = lerp( ADS_TreeLeavesAffectMotion168_g2011 , 1.0 , LeavesAmountSimple172_g2011);
			half MotionMask137_g2031 = ( Packed_Trunk21634 * lerpResult156_g2011 );
			float2 temp_output_94_0_g2031 = ( ( temp_output_160_0_g2031 * MotionDirection59_g2031 ) * MotionMask137_g2031 );
			float2 break311_g2031 = temp_output_94_0_g2031;
			float3 appendResult308_g2031 = (float3(break311_g2031.x , 0.0 , break311_g2031.y));
			half3 Motion_Trunk1749 = appendResult308_g2031;
			half MotionScale60_g2032 = ( ADS_GlobalScale * _MotionScale2 );
			half MotionSpeed62_g2032 = ( ADS_GlobalSpeed * _MotionSpeed2 );
			float mulTime90_g2032 = _Time.y * MotionSpeed62_g2032;
			float3 temp_output_95_0_g2032 = ( ( ase_worldPos * MotionScale60_g2032 ) + mulTime90_g2032 );
			half Packed_Variation1675 = v.color.a;
			half MotionVariation269_g2032 = ( _MotionVariation2 * Packed_Variation1675 );
			half MotionlAmplitude58_g2032 = ( ADS_GlobalAmplitude * _MotionAmplitude2 );
			float3 temp_output_92_0_g2032 = ( sin( ( temp_output_95_0_g2032 + MotionVariation269_g2032 ) ) * MotionlAmplitude58_g2032 );
			half3 GlobalDirection349_g2032 = ADS_GlobalDirection;
			float3 lerpResult280_g2032 = lerp( GlobalDirection349_g2032 , float3(0,1,0) , _MotionVertical2);
			half3 MotionDirection59_g2032 = mul( unity_WorldToObject, float4( lerpResult280_g2032 , 0.0 ) ).xyz;
			half Packed_Branch1862 = v.color.g;
			half ADS_TreeLeavesAffectMotion168_g2009 = 0.2;
			half ADS_TreeLeavesAmount157_g2009 = ADS_GlobalLeavesAmount;
			half localunity_ObjectToWorld0w1_g2010 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g2010 = ( unity_ObjectToWorld[2].w );
			float temp_output_142_0_g2009 = saturate( ( ADS_TreeLeavesAmount157_g2009 - ( frac( ( localunity_ObjectToWorld0w1_g2010 + localunity_ObjectToWorld2w3_g2010 ) ) * ADS_GlobalLeavesVar ) ) );
			half LeavesAmountSimple172_g2009 = temp_output_142_0_g2009;
			float lerpResult156_g2009 = lerp( ADS_TreeLeavesAffectMotion168_g2009 , 1.0 , LeavesAmountSimple172_g2009);
			half MotionMask137_g2032 = ( Packed_Branch1862 * lerpResult156_g2009 );
			float3 temp_output_94_0_g2032 = ( ( temp_output_92_0_g2032 * MotionDirection59_g2032 ) * MotionMask137_g2032 );
			half3 Motion_Branch1750 = temp_output_94_0_g2032;
			float2 temp_cast_4 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g2035 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g2035 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g2035 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g2035 = (float3(localunity_ObjectToWorld0w1_g2035 , localunity_ObjectToWorld1w2_g2035 , localunity_ObjectToWorld2w3_g2035));
			float2 panner73_g2033 = ( _Time.y * temp_cast_4 + ( (appendResult6_g2035).xz * ADS_TurbulenceScale ));
			float lerpResult136_g2033 = lerp( 1.0 , saturate( pow( abs( tex2Dlod( ADS_TurbulenceTex, float4( panner73_g2033, 0, 0.0) ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			half Motion_Turbulence2199 = lerpResult136_g2033;
			half3 Motion_Output1220 = ( ( Motion_Trunk1749 + Motion_Branch1750 ) * Motion_Turbulence2199 );
			v.vertex.xyz += Motion_Output1220;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult564 = (float2(_UVZero.x , _UVZero.y));
			float2 appendResult565 = (float2(_UVZero.z , _UVZero.w));
			float2 temp_output_575_0 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			float2 break1299 = temp_output_575_0;
			half localunity_ObjectToWorld0w1_g1420 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g1420 = ( unity_ObjectToWorld[2].w );
			float2 appendResult1104 = (float2(break1299.x , ( break1299.y + ( localunity_ObjectToWorld0w1_g1420 + localunity_ObjectToWorld2w3_g1420 ) )));
			float2 lerpResult1825 = lerp( temp_output_575_0 , appendResult1104 , _TrunkVariation);
			half2 Main_UVs587 = lerpResult1825;
			half3 Main_NormalTex620 = UnpackScaleNormal( tex2D( _NormalTex, Main_UVs587 ), _NormalScale );
			float2 appendResult1482 = (float2(_UVZero3.x , _UVZero3.y));
			float2 appendResult1483 = (float2(_UVZero3.z , _UVZero3.w));
			float2 temp_output_1485_0 = ( ( i.uv4_texcoord4 * appendResult1482 ) + appendResult1483 );
			half3 Base_NormaTex1490 = UnpackScaleNormal( tex2D( _NormalTex3, temp_output_1485_0 ), _NormalScale3 );
			float4 tex2DNode18 = tex2D( _AlbedoTex, Main_UVs587 );
			half Main_AlbedoTex_Alpha616 = tex2DNode18.a;
			half Base_Height18_g1959 = Main_AlbedoTex_Alpha616;
			half Packed_Trunk1813 = i.vertexColor.r;
			float temp_output_7_0_g1884 = 0.0;
			half localunity_ObjectToWorld0w1_g1421 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g1421 = ( unity_ObjectToWorld[2].w );
			float lerpResult2138 = lerp( 1.0 , frac( ( localunity_ObjectToWorld0w1_g1421 + localunity_ObjectToWorld2w3_g1421 ) ) , _BaseBlendVariation);
			float temp_output_84_0_g1959 = ( ( Packed_Trunk1813 - temp_output_7_0_g1884 ) / ( ( _BaseBlendAmount * lerpResult2138 ) - temp_output_7_0_g1884 ) );
			half OneMinusMask2_g1959 = ( 1.0 - temp_output_84_0_g1959 );
			float temp_output_22_0_g1959 = _BaseBlendContrast;
			half Contrast93_g1959 = temp_output_22_0_g1959;
			float temp_output_7_0_g1963 = 0.0;
			float temp_output_116_0_g1959 = ( ( Contrast93_g1959 - temp_output_7_0_g1963 ) / ( 2.0 - temp_output_7_0_g1963 ) );
			float temp_output_7_0_g1964 = temp_output_116_0_g1959;
			half BlendSimple_Output108_g1959 = saturate( ( ( ( ( ( 1.0 - Base_Height18_g1959 ) * OneMinusMask2_g1959 ) + OneMinusMask2_g1959 ) - temp_output_7_0_g1964 ) / ( ( 1.0 - temp_output_116_0_g1959 ) - temp_output_7_0_g1964 ) ) );
			half Mask_BaseBlend1491 = BlendSimple_Output108_g1959;
			float3 lerpResult1502 = lerp( Main_NormalTex620 , Base_NormaTex1490 , Mask_BaseBlend1491);
			float3 lerpResult2192 = lerp( lerpResult1502 , BlendNormals( Main_NormalTex620 , Base_NormaTex1490 ) , _BaseBlendNormals);
			half3 Blending_BaseNormal1897 = lerpResult2192;
			#ifdef _ENABLEBASE_ON
				float3 staticSwitch2184 = Blending_BaseNormal1897;
			#else
				float3 staticSwitch2184 = Main_NormalTex620;
			#endif
			half3 OUT_NORMAL1512 = staticSwitch2184;
			o.Normal = OUT_NORMAL1512;
			half4 Main_AlbedoTex487 = tex2DNode18;
			float4 tex2DNode1415 = tex2D( _AlbedoTex3, temp_output_1485_0 );
			half4 Base_AlbedoTex1489 = tex2DNode1415;
			float4 lerpResult1495 = lerp( Main_AlbedoTex487 , Base_AlbedoTex1489 , Mask_BaseBlend1491);
			half4 Blending_BaseAlbedo1896 = lerpResult1495;
			#ifdef _ENABLEBASE_ON
				float4 staticSwitch2173 = Blending_BaseAlbedo1896;
			#else
				float4 staticSwitch2173 = Main_AlbedoTex487;
			#endif
			half4 OUT_ALBEDO1501 = staticSwitch2173;
			half4 Main_Color486 = _Color;
			half4 Base_Color1533 = _Color3;
			float4 lerpResult1536 = lerp( Main_Color486 , Base_Color1533 , Mask_BaseBlend1491);
			half4 Blending_BaseColor1898 = lerpResult1536;
			#ifdef _ENABLEBASE_ON
				float4 staticSwitch2177 = Blending_BaseColor1898;
			#else
				float4 staticSwitch2177 = Main_Color486;
			#endif
			half4 OUT_COLOR1539 = staticSwitch2177;
			o.Albedo = ( OUT_ALBEDO1501 * OUT_COLOR1539 ).rgb;
			float4 tex2DNode645 = tex2D( _SurfaceTex, Main_UVs587 );
			half Main_SurfaceTex_A744 = tex2DNode645.a;
			half Main_Smoothness660 = ( Main_SurfaceTex_A744 * _Smoothness );
			float4 tex2DNode2049 = tex2D( _SurfaceTex3, temp_output_1485_0 );
			half Base_SurfaceTex_A2051 = tex2DNode2049.a;
			half Base_Smoothness748 = ( Base_SurfaceTex_A2051 * _Smoothness3 );
			float lerpResult2059 = lerp( Main_Smoothness660 , Base_Smoothness748 , Mask_BaseBlend1491);
			half Blending_BaseSmoothness2060 = lerpResult2059;
			#ifdef _ENABLEBASE_ON
				float staticSwitch2180 = Blending_BaseSmoothness2060;
			#else
				float staticSwitch2180 = Main_Smoothness660;
			#endif
			half OUT_SMOOTHNESS2071 = staticSwitch2180;
			o.Smoothness = OUT_SMOOTHNESS2071;
			float lerpResult1308 = lerp( 1.0 , i.uv_tex4coord.z , _VertexOcclusion);
			half Vertex_Occlusion1312 = lerpResult1308;
			half Main_SurfaceTex_G1788 = tex2DNode645.g;
			float lerpResult1793 = lerp( 1.0 , Main_SurfaceTex_G1788 , _Occlusion);
			half Main_Occlusion1794 = lerpResult1793;
			half Base_SurfaceTex_G2050 = tex2DNode2049.g;
			float lerpResult2055 = lerp( 1.0 , Base_SurfaceTex_G2050 , _Occlusion3);
			half Base_Occlusion2056 = lerpResult2055;
			float lerpResult2065 = lerp( Main_Occlusion1794 , Base_Occlusion2056 , Mask_BaseBlend1491);
			half Blending_BaseOcclusion2064 = lerpResult2065;
			#ifdef _ENABLEBASE_ON
				float staticSwitch2176 = Blending_BaseOcclusion2064;
			#else
				float staticSwitch2176 = Main_Occlusion1794;
			#endif
			half OUT_AO2077 = staticSwitch2176;
			o.Occlusion = ( Vertex_Occlusion1312 * OUT_AO2077 );
			o.Alpha = 1;
			float temp_output_41_0_g2037 = 1.0;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39_g2037 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g2037 = Dither4x4Bayer( fmod(clipScreen39_g2037.x, 4), fmod(clipScreen39_g2037.y, 4) );
			float temp_output_47_0_g2037 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g2037 = step( dither39_g2037, temp_output_47_0_g2037 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g2037 = ( temp_output_41_0_g2037 * dither39_g2037 );
			#else
				float staticSwitch40_g2037 = temp_output_41_0_g2037;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g2037 = staticSwitch40_g2037;
			#else
				float staticSwitch50_g2037 = temp_output_41_0_g2037;
			#endif
			clip( staticSwitch50_g2037 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Utils/ADS Fallback"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;540.0958;3018.08;1;True;False
Node;AmplifyShaderEditor.Vector4Node;563;-1280,-800;Half;False;Property;_UVZero;Trunk UVs;17;0;Create;False;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-1280,-1024;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;564;-1024,-800;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-832,-1024;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;565;-1024,-720;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;1102;-768,-768;Float;False;Object Position;-1;;1420;b9555b68a3d67c54f91597a05086026a;0;0;4;FLOAT3;7;FLOAT;0;FLOAT;4;FLOAT;5
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-624,-1024;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1109;-512,-768;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;1299;-480,-944;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;1101;-224,-816;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;1480;-1280,-32;Half;False;Property;_UVZero3;Base UVs;28;0;Create;False;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1826;-256,-704;Half;False;Property;_TrunkVariation;Trunk Variation;16;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;1104;-128,-944;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;1482;-1024,-32;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1481;-1280,-256;Float;False;3;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;1825;64,-1024;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;1815;384,0;Float;False;Object Position;-1;;1421;b9555b68a3d67c54f91597a05086026a;0;0;4;FLOAT3;7;FLOAT;0;FLOAT;4;FLOAT;5
Node;AmplifyShaderEditor.DynamicAppendNode;1483;-1024,48;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1484;-832,-256;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1817;576,48;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;587;256,-1024;Half;False;Main_UVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;588;640,-1024;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;644;3328,-1024;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1485;-624,-256;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;1816;704,48;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1633;-1280,1920;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1822;384,144;Half;False;Property;_BaseBlendVariation;Base Blend Variation;30;0;Create;True;0;0;False;1;;0.1;0;0.0001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2135;704,-32;Float;False;const;-1;;1842;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2138;864,64;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;645;3584,-1024;Float;True;Property;_SurfaceTex;Trunk Surface;13;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2049;-384,128;Float;True;Property;_SurfaceTex3;Base Surface;25;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1813;-1024,1920;Half;False;Packed_Trunk;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1426;384,-96;Half;False;Property;_BaseBlendAmount;Base Blend Amount;29;0;Create;True;0;0;False;1;;0.1;0;0.0001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;848,-1024;Float;True;Property;_AlbedoTex;Trunk Albedo;10;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1748;384,-256;Float;False;1813;Packed_Trunk;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1788;3968,-928;Half;False;Main_SurfaceTex_G;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2050;-64,128;Half;False;Base_SurfaceTex_G;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2137;1024,-96;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2112;384,-176;Float;False;const;-1;;1845;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;1152,-896;Half;False;Main_AlbedoTex_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;604;2304,-1024;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;2116;512,1152;Float;False;const;-1;;1886;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1795;-384,1280;Float;False;1788;Main_SurfaceTex_G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2051;-64,192;Half;False;Base_SurfaceTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2052;512,1472;Half;False;Property;_Occlusion3;Base Occlusion (G);27;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;655;2304,-896;Half;False;Property;_NormalScale;Trunk Normal Scale;11;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;744;3968,-768;Half;False;Main_SurfaceTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1769;-1024,2240;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1488;-768,0;Half;False;Property;_NormalScale3;Base Normal Scale;23;0;Create;False;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1494;1408,-64;Float;False;616;Main_AlbedoTex_Alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1425;1408,-256;Float;False;Remap To 0-1;-1;;1884;5eda8a2bb94ebef4ab0e43d19291cd8b;0;3;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2053;512,1280;Float;False;2050;Base_SurfaceTex_G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2115;-384,1152;Float;False;const;-1;;1885;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1792;-384,1472;Half;False;Property;_Occlusion;Trunk Occlusion (G);15;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1419;1408,128;Float;False;Property;_BaseBlendContrast;Base Blend Contrast;31;0;Create;True;0;0;False;0;0.9;0;0;0.999;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;4352,-944;Half;False;Property;_Smoothness;Trunk Smoothness (A);14;0;Create;False;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1862;-1024,1984;Half;False;Packed_Branch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;656;-640,384;Float;False;2051;Base_SurfaceTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1532;-1280,384;Half;False;Property;_Color3;Base Color;21;0;Create;False;0;0;False;0;1,1,1,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;2197;1792,-256;Float;False;ADS Trunck Blend;-1;;1959;c9c5a3a6ad630c04ebe1fafb6abbcaa0;1,113,0;4;84;FLOAT;0;False;63;FLOAT;0;False;61;FLOAT;0;False;22;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;750;-640,464;Half;False;Property;_Smoothness3;Base Smoothness (A);26;0;Create;False;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;409;1536,-1024;Half;False;Property;_Color;Trunk Color;9;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;1793;0,1152;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2055;896,1152;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1415;-384,-256;Float;True;Property;_AlbedoTex3;Base Albedo;22;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1675;-1024,2368;Half;False;Packed_Variation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1634;-832,2240;Half;False;Packed_Trunk2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1486;-384,-64;Float;True;Property;_NormalTex3;Base Normal;24;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;749;4352,-1024;Float;False;744;Main_SurfaceTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;607;2576,-1024;Float;True;Property;_NormalTex;Trunk Normal;12;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;2944,-1024;Half;False;Main_NormalTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1490;-64,-64;Half;False;Base_NormaTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;1792,-1024;Half;False;Main_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1491;2048,-256;Half;False;Mask_BaseBlend;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2205;0,2432;Float;False;ADS Leaves Amount;-1;;2011;ee8761bdf5e2c1e4b8e0ff49e8488b33;0;1;152;FLOAT;0;False;3;FLOAT;154;FLOAT;148;FLOAT;167
Node;AmplifyShaderEditor.GetLocalVarNode;1957;0,2304;Float;False;1634;Packed_Trunk2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;1152,-1024;Half;False;Main_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1489;-64,-256;Half;False;Base_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2056;1088,1152;Half;False;Base_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1794;192,1152;Half;False;Main_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;745;4672,-1040;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;657;-320,384;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1533;-960,384;Half;False;Base_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;2206;1280,2512;Float;False;ADS Leaves Amount;-1;;2009;ee8761bdf5e2c1e4b8e0ff49e8488b33;0;1;152;FLOAT;0;False;3;FLOAT;154;FLOAT;148;FLOAT;167
Node;AmplifyShaderEditor.RangedFloatNode;1732;1280,2240;Float;False;Property;_MotionVariation2;Branch Motion Variation;51;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1811;1280,2400;Float;False;1862;Packed_Branch;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1712;1280,2320;Float;False;1675;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1508;2560,176;Float;False;1490;Base_NormaTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;4864,-1040;Half;False;Main_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1561;1280,2080;Float;False;Property;_MotionScale2;Branch Motion Scale;50;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2024;448,2304;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2062;2560,688;Float;False;1794;Main_Occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1563;1280,1920;Float;False;Property;_MotionAmplitude2;Branch Motion Amplitude;48;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1555;0,1920;Float;False;Property;_MotionAmplitude;Trunk Motion Amplitude;43;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1556;0,2000;Float;False;Property;_MotionSpeed;Trunk Motion Speed;44;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1497;2560,-16;Float;False;1489;Base_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1534;2560,-256;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2061;2560,752;Float;False;2056;Base_Occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1557;0,2080;Float;False;Property;_MotionScale;Trunk Motion Scale;45;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2029;1728,2400;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1562;1280,2000;Float;False;Property;_MotionSpeed2;Branch Motion Speed;49;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1503;2560,112;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;748;-128,384;Half;False;Base_Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1496;2560,-80;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1499;2560,864;Float;False;1491;Mask_BaseBlend;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2139;1280,2160;Float;False;Property;_MotionVertical2;Branch Motion Vertical;52;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1733;1536,2240;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1535;2560,-192;Float;False;1533;Base_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1502;3008,128;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;1536;3008,-256;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;2190;2992,256;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;2065;3008,688;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2193;2960,368;Half;False;Property;_BaseBlendNormals;Base Blend Normals;32;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2211;1920,1920;Float;False;ADS Motion Generic;40;;2032;81cab27e2a487a645a4ff5eb3c63bd27;6,252,1,278,1,228,1,292,1,330,1,326,1;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;2210;640,1920;Float;False;ADS Motion Generic;40;;2031;81cab27e2a487a645a4ff5eb3c63bd27;6,252,0,278,0,228,0,292,0,330,0,326,0;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2058;2560,512;Float;False;660;Main_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2057;2560,576;Float;False;748;Base_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1495;3008,-80;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;2192;3200,128;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;2198;-1280,2496;Float;False;ADS Global Turbulence;34;;2033;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.RegisterLocalVarNode;1750;2240,1920;Half;False;Motion_Branch;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;2059;3008,512;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2064;3312,688;Half;False;Blending_BaseOcclusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1896;3344,-80;Half;False;Blending_BaseAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1749;896,1920;Half;False;Motion_Trunk;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1898;3344,-256;Half;False;Blending_BaseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2060;3312,512;Half;False;Blending_BaseSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2170;3840,512;Float;False;1794;Main_Occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2163;3840,-64;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1638;-1280,1280;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;2154;3840,-176;Float;False;1898;Blending_BaseColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2153;3840,592;Float;False;2064;Blending_BaseOcclusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1897;3344,128;Half;False;Blending_BaseNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1309;-1280,1472;Half;False;Property;_VertexOcclusion;Vertex Occlusion;38;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2159;3840,16;Float;False;1896;Blending_BaseAlbedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2199;-1024,2496;Half;False;Motion_Turbulence;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2168;3840,-256;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;2114;-1280,1152;Float;False;const;-1;;2036;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1796;2560,2016;Float;False;1750;Motion_Branch;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1752;2560,1920;Float;False;1749;Motion_Trunk;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;2177;4224,-256;Float;False;Property;_EnableBase;Enable Base;19;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Off;Base;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;2173;4224,-64;Float;False;Property;_EnableBase;Enable Base;19;0;Create;True;0;0;False;0;0;0;0;False;;Toggle;2;Off;Base;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1802;2816,1920;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;1308;-896,1152;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;2176;4224,512;Float;False;Property;_EnableBase;Enable Base;19;0;Create;True;0;0;False;0;0;0;0;False;;Toggle;2;Off;Base;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2178;3840,128;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2201;2560,2208;Float;False;2199;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2171;3840,208;Float;False;1897;Blending_BaseNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2179;3840,320;Float;False;660;Main_Smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2162;3840,400;Float;False;2060;Blending_BaseSmoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;2184;4224,128;Float;False;Property;_EnableBase;Enable Base;19;0;Create;True;0;0;False;0;0;0;0;False;;Toggle;2;Off;Base;Create;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1312;-704,1152;Half;False;Vertex_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1501;4480,-64;Half;False;OUT_ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2077;4480,512;Half;False;OUT_AO;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1629;3136,1920;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1539;4480,-256;Half;False;OUT_COLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;2180;4224,320;Float;False;Property;_EnableBase;Enable Base;19;0;Create;True;0;0;False;0;0;0;0;False;;Toggle;2;Off;Base;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1220;3296,1920;Half;False;Motion_Output;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1313;-1280,-1856;Float;False;1312;Vertex_Occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2071;4480,320;Half;False;OUT_SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2209;-640,-1856;Half;False;Constant;_Float6;Float 6;70;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1512;4480,128;Half;False;OUT_NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1280,-2176;Float;False;1501;OUT_ALBEDO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2107;-1280,-1792;Float;False;2077;OUT_AO;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1076;-1280,-2112;Float;False;1539;OUT_COLOR;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1930;1168,-2688;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;70;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1469;-768,-3360;Half;False;Property;_SETTINGSS;[ SETTINGSS ];33;0;Create;True;0;0;True;1;BCategory(Settings);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;654;-1280,-1952;Float;False;2071;OUT_SMOOTHNESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2196;-352,-3168;Half;False;Property;_BatchingInfo;!!! BatchingInfo;56;0;Create;True;0;0;True;1;BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-624,-2688;Half;False;Property;_CullType;Cull Type;5;1;[Enum];Create;True;3;Off;0;Front;1;Back;2;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2207;2176,-2688;Float;False;ADS Features Support;-1;;2038;217a332a46517ae4cb8ca16677bdb217;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-2688;Half;False;Property;_DstBlend;_DstBlend;75;1;[HideInInspector];Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1929;3968,-848;Half;False;Main_SurfaceTex_B;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1636;-1024,2048;Half;False;Packed_Leaf;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1471;128,-3360;Half;False;Property;_ADVANCEDD;[ ADVANCEDD ];55;0;Create;True;0;0;True;1;BCategory(Advanced);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1473;-112,-3360;Half;False;Property;_LEAFMOTIONN;[ LEAF MOTIONN ];53;0;Create;True;0;0;False;1;BCategory(Leaf Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1708;-976,-3168;Half;False;Property;_BranchMotionParameters;!!! Branch Motion Parameters !!!;47;0;Create;True;0;0;True;1;BMessage(Info, The Branch Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;624;-1280,-2048;Float;False;1512;OUT_NORMAL;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1938;0,-2688;Float;False;Internal Unity Props;58;;2039;b286e6ef621b64a4fb35da1e13fa143f;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2212;240,-2688;Float;False;ADS Internal Version;0;;2040;858e1f7f7bf8673449834f9aaa5bae83;0;0;1;FLOAT;5
Node;AmplifyShaderEditor.RangedFloatNode;1935;928,-2688;Half;False;Property;_Internal_LitStandard;Internal_LitStandard;68;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1493;1408,32;Float;False;1492;Base_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-800,-2688;Half;False;Property;_RenderType;Render Type;4;1;[Enum];Create;True;2;Opaque;0;Cutout;1;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2047;480,-2688;Half;False;Property;_Internal_ADS;Internal_ADS;57;1;[HideInInspector];Create;True;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1476;-1280,-3264;Half;False;Property;_RenderCutt;# RenderCutt;6;0;Create;True;0;0;True;1;BInteractive(_Mode, 1);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1466;-1280,-3360;Half;False;Property;_RENDERINGG;[ RENDERINGG ];3;0;Create;True;0;0;False;1;BCategory(Rendering);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1492;-64,-176;Half;False;Base_AlbedoTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1723;-1280,-3168;Half;False;Property;_TrunkMotionParameters;!!! Trunk Motion Parameters !!!;42;0;Create;True;0;0;True;1;BMessage(Info, The Trunk Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1472;-1280,-3456;Half;False;Property;_ADSStandardLitTreeBark;< ADS Standard Lit Tree Bark>;2;0;Create;True;0;0;True;1;BBanner(ADS Standard Lit, Tree Bark);1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1468;-1088,-3360;Half;False;Property;_TRUNKK;[ TRUNKK ];8;0;Create;True;0;0;True;1;BCategory(Trunk);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1470;-576,-3360;Half;False;Property;_TRUNKMOTIONN;[ TRUNK MOTIONN ];39;0;Create;True;0;0;True;1;BCategory(Trunk Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1688;-352,-3360;Half;False;Property;_BRANCHMOTIONN;[ BRANCH MOTIONN ];46;0;Create;True;0;0;True;1;BCategory(Branch Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2108;-1024,-1856;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1513;-928,-3360;Half;False;Property;_BASEE;[ BASEE ];18;0;Create;True;0;0;True;1;BCategory(Base);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1934;672,-2688;Half;False;Property;_Internal_TypeTreeBark;Internal_TypeTreeBark;69;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-960,-2688;Half;False;Property;_ZWrite;_ZWrite;76;1;[HideInInspector];Create;True;2;Off;0;On;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-448,-2688;Half;False;Property;_Cutoff;Cutout;7;1;[HideInInspector];Create;False;3;Off;0;Front;1;Back;2;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1724;-672,-3168;Half;False;Property;_LeafMtionParameters;!!! Leaf Mtion Parameters !!!;54;0;Create;True;0;0;False;1;BMessage(Info, The Leaf Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1932;1936,-2688;Half;False;Property;_Internal_SetByScript;Internal_SetByScript;73;1;[HideInInspector];Create;True;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2200;0,2176;Float;False;2199;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2048;1664,-2688;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;72;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1075;-1024,-2176;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1931;1408,-2688;Half;False;Property;_Internal_DebugMask2;Internal_DebugMask2;71;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;2208;-448,-1856;Float;False;ADS LODFade Dither;-1;;2037;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1221;-512,-1664;Float;False;1220;Motion_Output;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;646;3968,-1024;Half;False;Main_SurfaceTex_R;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-2688;Half;False;Property;_SrcBlend;_SrcBlend;74;1;[HideInInspector];Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1799;-1088,-3264;Half;False;Property;_BlendingBasee;# BlendingBasee;20;0;Create;True;0;0;True;1;BInteractive(_ENABLEBASE_ON);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-128,-2176;Float;False;True;2;Float;ADSShaderGUI;300;0;Standard;BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Tree Bark;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;True;False;False;False;True;Back;0;False;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Opaque;;Geometry;All;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;True;550;10;True;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;300;Utils/ADS Fallback;-1;-1;-1;-1;0;False;0;0;False;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;1546;384,-384;Float;False;1880.808;100;Blend Height Mask;0;;1,0.234,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;712;-1280,-1152;Float;False;1745.222;100;Main UVs;0;;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2188;3840,-384;Float;False;1048.21;100;Trunk Base Blending Final;0;;1,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2189;2560,-384;Float;False;1027.098;100;Trunk Base Blending ;0;;1,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1542;-1280,1792;Float;False;4786.924;100;Tree Motion;0;;0.03448272,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1465;-1280,-3584;Float;False;1793.477;100;Drawers;0;;1,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;715;3328,-1152;Float;False;872;100;Smoothness Texture(Metallic, AO, Height, Smoothness);0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1937;0,-2816;Float;False;2399.312;100;Internal Only;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;708;2304,-1152;Float;False;833.139;100;Normal Texture;0;;0.5019608,0.5019608,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1544;-1280,1024;Float;False;2559.027;100;Ambient Occlusion;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;760;640,-1152;Float;False;1364.434;100;Main Texture and Color;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1545;-1280,-384;Float;False;1541.176;100;Base Blend Inputs;0;;1,0.234,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;751;4352,-1152;Float;False;761.9668;100;Metallic / Smoothness;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1280,-2816;Float;False;1085;100;Rendering (Unused);0;;1,0,0.503,1;0;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;1109;0;1102;0
WireConnection;1109;1;1102;5
WireConnection;1299;0;575;0
WireConnection;1101;0;1299;1
WireConnection;1101;1;1109;0
WireConnection;1104;0;1299;0
WireConnection;1104;1;1101;0
WireConnection;1482;0;1480;1
WireConnection;1482;1;1480;2
WireConnection;1825;0;575;0
WireConnection;1825;1;1104;0
WireConnection;1825;2;1826;0
WireConnection;1483;0;1480;3
WireConnection;1483;1;1480;4
WireConnection;1484;0;1481;0
WireConnection;1484;1;1482;0
WireConnection;1817;0;1815;0
WireConnection;1817;1;1815;5
WireConnection;587;0;1825;0
WireConnection;1485;0;1484;0
WireConnection;1485;1;1483;0
WireConnection;1816;0;1817;0
WireConnection;2138;0;2135;0
WireConnection;2138;1;1816;0
WireConnection;2138;2;1822;0
WireConnection;645;1;644;0
WireConnection;2049;1;1485;0
WireConnection;1813;0;1633;1
WireConnection;18;1;588;0
WireConnection;1788;0;645;2
WireConnection;2050;0;2049;2
WireConnection;2137;0;1426;0
WireConnection;2137;1;2138;0
WireConnection;616;0;18;4
WireConnection;2051;0;2049;4
WireConnection;744;0;645;4
WireConnection;1769;0;1633;1
WireConnection;1769;1;1633;1
WireConnection;1425;6;1748;0
WireConnection;1425;7;2112;0
WireConnection;1425;8;2137;0
WireConnection;1862;0;1633;2
WireConnection;2197;84;1425;0
WireConnection;2197;63;1494;0
WireConnection;2197;22;1419;0
WireConnection;1793;0;2115;0
WireConnection;1793;1;1795;0
WireConnection;1793;2;1792;0
WireConnection;2055;0;2116;0
WireConnection;2055;1;2053;0
WireConnection;2055;2;2052;0
WireConnection;1415;1;1485;0
WireConnection;1675;0;1633;4
WireConnection;1634;0;1769;0
WireConnection;1486;1;1485;0
WireConnection;1486;5;1488;0
WireConnection;607;1;604;0
WireConnection;607;5;655;0
WireConnection;620;0;607;0
WireConnection;1490;0;1486;0
WireConnection;486;0;409;0
WireConnection;1491;0;2197;0
WireConnection;487;0;18;0
WireConnection;1489;0;1415;0
WireConnection;2056;0;2055;0
WireConnection;1794;0;1793;0
WireConnection;745;0;749;0
WireConnection;745;1;294;0
WireConnection;657;0;656;0
WireConnection;657;1;750;0
WireConnection;1533;0;1532;0
WireConnection;660;0;745;0
WireConnection;2024;0;1957;0
WireConnection;2024;1;2205;148
WireConnection;2029;0;1811;0
WireConnection;2029;1;2206;148
WireConnection;748;0;657;0
WireConnection;1733;0;1732;0
WireConnection;1733;1;1712;0
WireConnection;1502;0;1503;0
WireConnection;1502;1;1508;0
WireConnection;1502;2;1499;0
WireConnection;1536;0;1534;0
WireConnection;1536;1;1535;0
WireConnection;1536;2;1499;0
WireConnection;2190;0;1503;0
WireConnection;2190;1;1508;0
WireConnection;2065;0;2062;0
WireConnection;2065;1;2061;0
WireConnection;2065;2;1499;0
WireConnection;2211;220;1563;0
WireConnection;2211;221;1562;0
WireConnection;2211;222;1561;0
WireConnection;2211;218;1733;0
WireConnection;2211;136;2029;0
WireConnection;2211;279;2139;0
WireConnection;2210;220;1555;0
WireConnection;2210;221;1556;0
WireConnection;2210;222;1557;0
WireConnection;2210;136;2024;0
WireConnection;1495;0;1496;0
WireConnection;1495;1;1497;0
WireConnection;1495;2;1499;0
WireConnection;2192;0;1502;0
WireConnection;2192;1;2190;0
WireConnection;2192;2;2193;0
WireConnection;1750;0;2211;0
WireConnection;2059;0;2058;0
WireConnection;2059;1;2057;0
WireConnection;2059;2;1499;0
WireConnection;2064;0;2065;0
WireConnection;1896;0;1495;0
WireConnection;1749;0;2210;0
WireConnection;1898;0;1536;0
WireConnection;2060;0;2059;0
WireConnection;1897;0;2192;0
WireConnection;2199;0;2198;85
WireConnection;2177;1;2168;0
WireConnection;2177;0;2154;0
WireConnection;2173;1;2163;0
WireConnection;2173;0;2159;0
WireConnection;1802;0;1752;0
WireConnection;1802;1;1796;0
WireConnection;1308;0;2114;0
WireConnection;1308;1;1638;3
WireConnection;1308;2;1309;0
WireConnection;2176;1;2170;0
WireConnection;2176;0;2153;0
WireConnection;2184;1;2178;0
WireConnection;2184;0;2171;0
WireConnection;1312;0;1308;0
WireConnection;1501;0;2173;0
WireConnection;2077;0;2176;0
WireConnection;1629;0;1802;0
WireConnection;1629;1;2201;0
WireConnection;1539;0;2177;0
WireConnection;2180;1;2179;0
WireConnection;2180;0;2162;0
WireConnection;1220;0;1629;0
WireConnection;2071;0;2180;0
WireConnection;1512;0;2184;0
WireConnection;1929;0;645;3
WireConnection;1636;0;1633;3
WireConnection;1492;0;1415;4
WireConnection;2108;0;1313;0
WireConnection;2108;1;2107;0
WireConnection;1075;0;36;0
WireConnection;1075;1;1076;0
WireConnection;2208;41;2209;0
WireConnection;646;0;645;1
WireConnection;0;0;1075;0
WireConnection;0;1;624;0
WireConnection;0;4;654;0
WireConnection;0;5;2108;0
WireConnection;0;10;2208;0
WireConnection;0;11;1221;0
ASEEND*/
//CHKSM=E15AC5C7C96FA318F71ADF29C75B7D1C9CC52BD2