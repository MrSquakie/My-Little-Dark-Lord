// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Tree Leaf"
{
	Properties
	{
		[HideInInspector]_Internal_Version("Internal_Version", Float) = 232
		[BBanner(ADS Simple Lit, Tree Leaf)]_ADSSimpleLitTreeLeaf("< ADS Simple Lit Tree Leaf >", Float) = 1
		[BCategory(Rendering)]_RENDERINGG("[ RENDERINGG ]", Float) = 0
		[Enum(Two Sided,0,Back,1,Front,2)]_RenderFaces("Render Faces", Float) = 0
		[Enum(Mirrored Normals,0,Flipped Normals,1)]_NormalInvertOnBackface("Render Backface", Float) = 1
		_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[BCategory(Leaf)]_LEAFF("[ LEAFF ]", Float) = 0
		_Color("Leaf Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex("Leaf Albedo", 2D) = "white" {}
		_NormalScale("Leaf Normal Scale", Float) = 1
		[NoScaleOffset]_NormalTex("Leaf Normal", 2D) = "bump" {}
		[BCategory(Settings)]_SETTINGSS("[ SETTINGSS ]", Float) = 0
		[HideInInspector]_MotionNoise("Motion Noise", Float) = 1
		_GlobalTurbulence("Global Turbulence", Range( 0 , 1)) = 1
		_GlobalTint("Global Tint", Range( 0 , 1)) = 1
		_DetailTint("Detail Tint", Range( 0 , 1)) = 1
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
		[BCategory(Leaf Motion)]_LEAFMOTIONN("[ LEAF MOTIONN ]", Float) = 0
		[BMessage(Info, The Leaf Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10)]_LeafMtionParameters("!!! Leaf Mtion Parameters !!!", Float) = 0
		_MotionAmplitude3("Leaf Flutter Amplitude", Float) = 0
		_MotionSpeed3("Leaf Flutter Speed", Float) = 0
		_MotionScale3("Leaf Flutter Scale", Float) = 0
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
		[HideInInspector]_Internal_LitSimple("Internal_LitSimple", Float) = 1
		[HideInInspector]_Internal_TypeTreeLeaf("Internal_TypeTreeLeaf", Float) = 1
		[HideInInspector]_Internal_DebugMask("Internal_DebugMask", Float) = 1
		[HideInInspector]_Internal_DebugMask2("Internal_DebugMask2", Float) = 1
		[HideInInspector]_Internal_DebugMask3("Internal_DebugMask3", Float) = 1
		[HideInInspector]_Internal_DebugVariation("Internal_DebugVariation", Float) = 1
		[HideInInspector]_Internal_SetByScript("Internal_SetByScript", Float) = 0
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
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
		#pragma target 3.0
		#pragma multi_compile_instancing
		 
		//ADS Features
		//ADS End
		  
		#pragma exclude_renderers gles 
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float4 vertexColor : COLOR;
			float4 uv_tex4coord;
			float4 screenPosition;
		};

		uniform float _MotionNoise;
		uniform half _ADVANCEDD;
		uniform half _BRANCHMOTIONN;
		uniform half4 _MainUVs;
		uniform float _Mode;
		uniform float _Glossiness;
		uniform half _CullMode;
		uniform float _BumpScale;
		uniform sampler2D _MetallicGlossMap;
		uniform half _Internal_UnityToBoxophobic;
		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform half _Internal_DebugVariation;
		uniform half _Internal_TypeTreeLeaf;
		uniform half _Internal_SetByScript;
		uniform half _Internal_ADS;
		uniform half _TrunkMotionParameters;
		uniform half _BatchingInfo;
		uniform half _Internal_LitSimple;
		uniform half _Internal_DebugMask3;
		uniform half _Internal_DebugMask2;
		uniform half _LeafMtionParameters;
		uniform half _LEAFF;
		uniform half _SETTINGSS;
		uniform half _BranchMotionParameters;
		uniform half _RENDERINGG;
		uniform half _RenderFaces;
		uniform half _TRUNKMOTIONN;
		uniform half _ADSSimpleLitTreeLeaf;
		uniform half _Cutoff;
		uniform half _LEAFMOTIONN;
		uniform half _Internal_DebugMask;
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
		uniform float _MotionScale3;
		uniform float _MotionSpeed3;
		uniform float _MotionAmplitude3;
		uniform sampler2D ADS_TurbulenceTex;
		uniform half ADS_TurbulenceSpeed;
		uniform half ADS_TurbulenceScale;
		uniform half ADS_TurbulenceContrast;
		uniform float _GlobalTurbulence;
		uniform half _NormalScale;
		uniform sampler2D _NormalTex;
		uniform half _NormalInvertOnBackface;
		uniform sampler2D _AlbedoTex;
		uniform half4 _Color;
		uniform half4 ADS_GlobalTintColorOne;
		uniform half4 ADS_GlobalTintColorTwo;
		uniform half _DetailTint;
		uniform half ADS_GlobalTintIntensity;
		uniform half _GlobalTint;
		uniform half _VertexOcclusion;


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
			half MotionScale60_g2113 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g2113 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g2113 = _Time.y * MotionSpeed62_g2113;
			float2 temp_output_95_0_g2113 = ( ( (ase_worldPos).xz * MotionScale60_g2113 ) + mulTime90_g2113 );
			half MotionlAmplitude58_g2113 = ( ADS_GlobalAmplitude * _MotionAmplitude );
			float2 temp_output_92_0_g2113 = ( sin( temp_output_95_0_g2113 ) * MotionlAmplitude58_g2113 );
			float2 temp_output_160_0_g2113 = ( temp_output_92_0_g2113 + MotionlAmplitude58_g2113 + MotionlAmplitude58_g2113 );
			half3 GlobalDirection349_g2113 = ADS_GlobalDirection;
			float3 break339_g2113 = mul( unity_WorldToObject, float4( GlobalDirection349_g2113 , 0.0 ) ).xyz;
			float2 appendResult340_g2113 = (float2(break339_g2113.x , break339_g2113.z));
			half2 MotionDirection59_g2113 = appendResult340_g2113;
			half Packed_Trunk21809 = ( v.color.r * v.color.r );
			half ADS_TreeLeavesAffectMotion168_g1878 = 0.2;
			half ADS_TreeLeavesAmount157_g1878 = ADS_GlobalLeavesAmount;
			half localunity_ObjectToWorld0w1_g1879 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g1879 = ( unity_ObjectToWorld[2].w );
			float temp_output_142_0_g1878 = saturate( ( ADS_TreeLeavesAmount157_g1878 - ( frac( ( localunity_ObjectToWorld0w1_g1879 + localunity_ObjectToWorld2w3_g1879 ) ) * ADS_GlobalLeavesVar ) ) );
			half LeavesAmountSimple172_g1878 = temp_output_142_0_g1878;
			float lerpResult156_g1878 = lerp( ADS_TreeLeavesAffectMotion168_g1878 , 1.0 , LeavesAmountSimple172_g1878);
			half MotionMask137_g2113 = ( Packed_Trunk21809 * lerpResult156_g1878 );
			float2 temp_output_94_0_g2113 = ( ( temp_output_160_0_g2113 * MotionDirection59_g2113 ) * MotionMask137_g2113 );
			float2 break311_g2113 = temp_output_94_0_g2113;
			float3 appendResult308_g2113 = (float3(break311_g2113.x , 0.0 , break311_g2113.y));
			half3 Motion_Trunk1749 = appendResult308_g2113;
			half MotionScale60_g2114 = ( ADS_GlobalScale * _MotionScale2 );
			half MotionSpeed62_g2114 = ( ADS_GlobalSpeed * _MotionSpeed2 );
			float mulTime90_g2114 = _Time.y * MotionSpeed62_g2114;
			float3 temp_output_95_0_g2114 = ( ( ase_worldPos * MotionScale60_g2114 ) + mulTime90_g2114 );
			half Packed_Variation1815 = v.color.a;
			half MotionVariation269_g2114 = ( _MotionVariation2 * Packed_Variation1815 );
			half MotionlAmplitude58_g2114 = ( ADS_GlobalAmplitude * _MotionAmplitude2 );
			float3 temp_output_92_0_g2114 = ( sin( ( temp_output_95_0_g2114 + MotionVariation269_g2114 ) ) * MotionlAmplitude58_g2114 );
			half3 GlobalDirection349_g2114 = ADS_GlobalDirection;
			float3 lerpResult280_g2114 = lerp( GlobalDirection349_g2114 , float3(0,1,0) , _MotionVertical2);
			half3 MotionDirection59_g2114 = mul( unity_WorldToObject, float4( lerpResult280_g2114 , 0.0 ) ).xyz;
			half Packed_Branch1830 = v.color.g;
			half ADS_TreeLeavesAffectMotion168_g1876 = 0.2;
			half ADS_TreeLeavesAmount157_g1876 = ADS_GlobalLeavesAmount;
			half localunity_ObjectToWorld0w1_g1877 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g1877 = ( unity_ObjectToWorld[2].w );
			float temp_output_142_0_g1876 = saturate( ( ADS_TreeLeavesAmount157_g1876 - ( frac( ( localunity_ObjectToWorld0w1_g1877 + localunity_ObjectToWorld2w3_g1877 ) ) * ADS_GlobalLeavesVar ) ) );
			half LeavesAmountSimple172_g1876 = temp_output_142_0_g1876;
			float lerpResult156_g1876 = lerp( ADS_TreeLeavesAffectMotion168_g1876 , 1.0 , LeavesAmountSimple172_g1876);
			half MotionMask137_g2114 = ( Packed_Branch1830 * lerpResult156_g1876 );
			float3 temp_output_94_0_g2114 = ( ( temp_output_92_0_g2114 * MotionDirection59_g2114 ) * MotionMask137_g2114 );
			half3 Motion_Branch1750 = temp_output_94_0_g2114;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 break311_g2115 = ase_vertex3Pos;
			half MotionFlutterScale60_g2115 = ( ADS_GlobalScale * _MotionScale3 );
			half MotionFlutterSpeed62_g2115 = ( ADS_GlobalSpeed * _MotionSpeed3 );
			float mulTime303_g2115 = _Time.y * MotionFlutterSpeed62_g2115;
			half MotionlFlutterAmplitude58_g2115 = ( ADS_GlobalAmplitude * _MotionAmplitude3 );
			half Packed_Leaf1819 = v.color.b;
			half MotionMask137_g2115 = Packed_Leaf1819;
			float3 ase_vertexNormal = v.normal.xyz;
			half3 Motion_Leaf1751 = ( sin( ( ( ( break311_g2115.x + break311_g2115.y + break311_g2115.z ) * MotionFlutterScale60_g2115 ) + mulTime303_g2115 ) ) * MotionlFlutterAmplitude58_g2115 * MotionMask137_g2115 * ase_vertexNormal );
			float2 temp_cast_4 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g2118 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g2118 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g2118 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g2118 = (float3(localunity_ObjectToWorld0w1_g2118 , localunity_ObjectToWorld1w2_g2118 , localunity_ObjectToWorld2w3_g2118));
			float2 panner73_g2116 = ( _Time.y * temp_cast_4 + ( (appendResult6_g2118).xz * ADS_TurbulenceScale ));
			float lerpResult136_g2116 = lerp( 1.0 , saturate( pow( abs( tex2Dlod( ADS_TurbulenceTex, float4( panner73_g2116, 0, 0.0) ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			half Motion_Turbulence1921 = lerpResult136_g2116;
			half3 Motion_Output1220 = ( ( Motion_Trunk1749 + Motion_Branch1750 + Motion_Leaf1751 ) * Motion_Turbulence1921 );
			v.vertex.xyz += Motion_Output1220;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NormalTex607 = i.uv_texcoord;
			float3 temp_output_13_0_g2127 = UnpackScaleNormal( tex2D( _NormalTex, uv_NormalTex607 ), _NormalScale );
			float3 break17_g2127 = temp_output_13_0_g2127;
			float switchResult12_g2127 = (((i.ASEVFace>0)?(break17_g2127.z):(-break17_g2127.z)));
			float3 appendResult18_g2127 = (float3(break17_g2127.x , break17_g2127.y , switchResult12_g2127));
			float3 lerpResult20_g2127 = lerp( temp_output_13_0_g2127 , appendResult18_g2127 , _NormalInvertOnBackface);
			half3 Main_NormalTex620 = lerpResult20_g2127;
			o.Normal = Main_NormalTex620;
			float2 uv_AlbedoTex18 = i.uv_texcoord;
			float4 tex2DNode18 = tex2D( _AlbedoTex, uv_AlbedoTex18 );
			half4 Main_AlbedoTex487 = tex2DNode18;
			half4 Main_Color486 = _Color;
			float4 temp_cast_0 = (1.0).xxxx;
			half4 ADS_GlobalTintColorOne176_g2122 = ADS_GlobalTintColorOne;
			half4 ADS_GlobalTintColorTwo177_g2122 = ADS_GlobalTintColorTwo;
			half localunity_ObjectToWorld0w1_g2126 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g2126 = ( unity_ObjectToWorld[2].w );
			half Packed_Variation1815 = i.vertexColor.a;
			float lerpResult194_g2122 = lerp( frac( ( localunity_ObjectToWorld0w1_g2126 + localunity_ObjectToWorld2w3_g2126 ) ) , Packed_Variation1815 , _DetailTint);
			float4 lerpResult166_g2122 = lerp( ADS_GlobalTintColorOne176_g2122 , ADS_GlobalTintColorTwo177_g2122 , lerpResult194_g2122);
			half ADS_GlobalTintIntensity181_g2122 = ADS_GlobalTintIntensity;
			half GlobalTint186_g2122 = _GlobalTint;
			float4 lerpResult168_g2122 = lerp( temp_cast_0 , ( lerpResult166_g2122 * ADS_GlobalTintIntensity181_g2122 ) , GlobalTint186_g2122);
			half4 Gloabl_Tint1908 = lerpResult168_g2122;
			float lerpResult1308 = lerp( 1.0 , i.uv_tex4coord.z , _VertexOcclusion);
			half Vertex_Occlusion1312 = lerpResult1308;
			o.Albedo = saturate( ( Main_AlbedoTex487 * Main_Color486 * Gloabl_Tint1908 * Vertex_Occlusion1312 ) ).rgb;
			o.Alpha = 1;
			half Main_AlbedoTex_A616 = tex2DNode18.a;
			float temp_output_152_0_g2120 = i.uv_tex4coord.w;
			half ADS_TreeLeavesAmount157_g2120 = ADS_GlobalLeavesAmount;
			half localunity_ObjectToWorld0w1_g2121 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g2121 = ( unity_ObjectToWorld[2].w );
			float temp_output_142_0_g2120 = saturate( ( ADS_TreeLeavesAmount157_g2120 - ( frac( ( localunity_ObjectToWorld0w1_g2121 + localunity_ObjectToWorld2w3_g2121 ) ) * ADS_GlobalLeavesVar ) ) );
			float lerpResult175_g2120 = lerp( 0.0 , ceil( ( temp_output_152_0_g2120 - temp_output_142_0_g2120 ) ) , step( temp_output_152_0_g2120 , 3.0 ));
			half Opacity1306 = saturate( ( Main_AlbedoTex_A616 - lerpResult175_g2120 ) );
			float temp_output_41_0_g2130 = Opacity1306;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39_g2130 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g2130 = Dither4x4Bayer( fmod(clipScreen39_g2130.x, 4), fmod(clipScreen39_g2130.y, 4) );
			float temp_output_47_0_g2130 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g2130 = step( dither39_g2130, temp_output_47_0_g2130 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g2130 = ( temp_output_41_0_g2130 * dither39_g2130 );
			#else
				float staticSwitch40_g2130 = temp_output_41_0_g2130;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g2130 = staticSwitch40_g2130;
			#else
				float staticSwitch50_g2130 = temp_output_41_0_g2130;
			#endif
			clip( staticSwitch50_g2130 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Utils/ADS Fallback"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;596.2007;3028.753;1.043226;True;False
Node;AmplifyShaderEditor.VertexColorNode;1804;-1280,768;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1808;-1024,1072;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1815;-1024,1216;Half;False;Packed_Variation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1809;-832,1088;Half;False;Packed_Trunk2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1830;-1024,848;Half;False;Packed_Branch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1929;-384,1280;Float;False;ADS Leaves Amount;-1;;1878;ee8761bdf5e2c1e4b8e0ff49e8488b33;0;1;152;FLOAT;0;False;3;FLOAT;154;FLOAT;148;FLOAT;167
Node;AmplifyShaderEditor.GetLocalVarNode;1637;-384,1152;Float;False;1809;Packed_Trunk2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1819;-1024,928;Half;False;Packed_Leaf;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1820;896,1264;Float;False;1830;Packed_Branch;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1930;896,1360;Float;False;ADS Leaves Amount;-1;;1876;ee8761bdf5e2c1e4b8e0ff49e8488b33;0;1;152;FLOAT;0;False;3;FLOAT;154;FLOAT;148;FLOAT;167
Node;AmplifyShaderEditor.RangedFloatNode;1732;896,1088;Float;False;Property;_MotionVariation2;Branch Motion Variation;41;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1712;896,1168;Float;False;1815;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1562;896,848;Float;False;Property;_MotionSpeed2;Branch Motion Speed;39;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1722;2240,1024;Float;False;1819;Packed_Leaf;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1557;-384,928;Float;False;Property;_MotionScale;Trunk Motion Scale;35;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1556;-384,848;Float;False;Property;_MotionSpeed;Trunk Motion Speed;34;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1892;0,1152;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1716;2240,768;Float;False;Property;_MotionAmplitude3;Leaf Flutter Amplitude;45;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1714;2240,928;Float;False;Property;_MotionScale3;Leaf Flutter Scale;47;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1555;-384,768;Float;False;Property;_MotionAmplitude;Trunk Motion Amplitude;33;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1903;896,1008;Float;False;Property;_MotionVertical2;Branch Motion Vertical;42;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1561;896,928;Float;False;Property;_MotionScale2;Branch Motion Scale;40;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1715;2240,848;Float;False;Property;_MotionSpeed3;Leaf Flutter Speed;46;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1733;1152,1088;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-1280,-896;Float;True;Property;_AlbedoTex;Leaf Albedo;10;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1563;896,768;Float;False;Property;_MotionAmplitude2;Branch Motion Amplitude;38;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1895;1344,1264;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1924;-1280,-32;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;-976,-768;Half;False;Main_AlbedoTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1919;2560,768;Float;False;ADS Motion Flutter;-1;;2115;87d8028e5f83178498a65cfa9f0e9ace;1,312,0;5;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;136;FLOAT;0;False;310;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1935;256,768;Float;False;ADS Motion Generic;30;;2113;81cab27e2a487a645a4ff5eb3c63bd27;6,252,0,278,0,228,0,292,0,330,0,326,0;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1936;1536,768;Float;False;ADS Motion Generic;30;;2114;81cab27e2a487a645a4ff5eb3c63bd27;6,252,1,278,1,228,1,292,1,330,1,326,1;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1309;0,192;Half;False;Property;_VertexOcclusion;Vertex Occlusion;28;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1750;1856,768;Half;False;Motion_Branch;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1751;2816,768;Half;False;Motion_Leaf;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1931;-1008,0;Float;False;ADS Leaves Amount;-1;;2120;ee8761bdf5e2c1e4b8e0ff49e8488b33;0;1;152;FLOAT;0;False;3;FLOAT;154;FLOAT;148;FLOAT;167
Node;AmplifyShaderEditor.GetLocalVarNode;1888;-1280,-128;Float;False;616;Main_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1905;-1280,1920;Float;False;1815;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1749;512,768;Half;False;Motion_Trunk;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1920;-1280,1344;Float;False;ADS Global Turbulence;19;;2116;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1814;0,0;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1901;0,-128;Float;False;const;-1;;2119;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1796;3200,864;Float;False;1750;Motion_Branch;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;1308;384,-128;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1921;-1024,1344;Half;False;Motion_Turbulence;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1906;-1024,1920;Float;False;ADS Global Settings;23;;2122;0fe83146627632b4981f5a0aa1b63801;0;1;171;FLOAT;0;False;3;COLOR;85;COLOR;165;FLOAT;157
Node;AmplifyShaderEditor.GetLocalVarNode;1797;3200,960;Float;False;1751;Motion_Leaf;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;655;384,-896;Half;False;Property;_NormalScale;Leaf Normal Scale;11;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1752;3200,768;Float;False;1749;Motion_Trunk;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1889;-608,-128;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;409;-384,-896;Half;False;Property;_Color;Leaf Color;9;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;-128,-896;Half;False;Main_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1801;3584,768;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;1890;-464,-128;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1908;-704,1920;Half;False;Gloabl_Tint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1932;720,-704;Half;False;Property;_NormalInvertOnBackface;Render Backface;6;1;[Enum];Create;False;2;Mirrored Normals;0;Flipped Normals;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;-976,-896;Half;False;Main_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1312;576,-128;Half;False;Vertex_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1923;3200,1056;Float;False;1921;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;607;656,-896;Float;True;Property;_NormalTex;Leaf Normal;12;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1306;-320,-128;Half;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1076;-1280,-1984;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1914;-1280,-1920;Float;False;1908;Gloabl_Tint;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1915;976,-896;Float;False;ADS Normal Backface;-1;;2127;4f53bc25e6d8da34db70401bcf363a2a;0;2;13;FLOAT3;0,0,0;False;30;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1280,-2048;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1629;3776,768;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1313;-1280,-1856;Float;False;1312;Vertex_Occlusion;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1783;-1280,-1664;Float;False;1306;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;1200,-896;Half;False;Main_NormalTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1075;-768,-2048;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1220;3936,768;Half;False;Motion_Output;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;1343;-576,-2048;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-960,-2560;Half;False;Property;_ZWrite;_ZWrite;70;1;[HideInInspector];Create;True;2;Off;0;On;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1902;895.382,-127.3066;Float;False;const;-1;;2129;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1470;-544,-3232;Half;False;Property;_TRUNKMOTIONN;[ TRUNK MOTIONN ];29;0;Create;True;0;0;True;1;BCategory(Trunk Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-624,-2560;Half;False;Property;_RenderFaces;Render Faces;5;1;[Enum];Create;True;3;Two Sided;0;Back;1;Front;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1466;-1280,-3232;Half;False;Property;_RENDERINGG;[ RENDERINGG ];3;0;Create;True;0;0;True;1;BCategory(Rendering);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1708;-976,-3040;Half;False;Property;_BranchMotionParameters;!!! Branch Motion Parameters !!!;37;0;Create;True;0;0;True;1;BMessage(Info, The Branch Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1469;-736,-3232;Half;False;Property;_SETTINGSS;[ SETTINGSS ];18;0;Create;True;0;0;True;1;BCategory(Settings);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1468;-1088,-3232;Half;False;Property;_LEAFF;[ LEAFF ];8;0;Create;True;0;0;True;1;BCategory(Leaf);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1724;-672,-3040;Half;False;Property;_LeafMtionParameters;!!! Leaf Mtion Parameters !!!;44;0;Create;True;0;0;True;1;BMessage(Info, The Leaf Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-2560;Half;False;Property;_DstBlend;_DstBlend;69;1;[HideInInspector];Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;744;1921,-704;Half;False;Main_SurfaceTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;624;-1280,-1760;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1836;1920,-768;Half;False;Main_SurfaceTex_B;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1933;2368,-2560;Float;False;ADS Features Support;-1;;2131;217a332a46517ae4cb8ca16677bdb217;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1934;-1024,-1664;Float;False;ADS LODFade Dither;-1;;2130;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1937;224,-2560;Float;False;ADS Internal Version;0;;2132;858e1f7f7bf8673449834f9aaa5bae83;0;0;1;FLOAT;5
Node;AmplifyShaderEditor.RangedFloatNode;1837;1136,-2560;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;63;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1473;-80,-3232;Half;False;Property;_LEAFMOTIONN;[ LEAF MOTIONN ];43;0;Create;True;0;0;True;1;BCategory(Leaf Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1922;-384,1024;Float;False;1921;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;646;1920,-896;Half;False;Main_SurfaceTex_R;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1472;-1280,-3328;Half;False;Property;_ADSSimpleLitTreeLeaf;< ADS Simple Lit Tree Leaf >;2;0;Create;True;0;0;True;1;BBanner(ADS Simple Lit, Tree Leaf);1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;2816,-896;Half;False;SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1799;-1280,-3136;Half;False;Property;_EnableBlendingg;# EnableBlendingg;17;0;Create;True;0;0;False;1;BInteractive(_ENABLEBLENDING_ON);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-448,-2560;Half;False;Property;_Cutoff;Cutout;7;0;Create;False;3;Off;0;Front;1;Back;2;0;True;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;2304,-816;Half;False;Property;_Smoothness;Leaf Smoothness;14;0;Create;False;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1841;656,-2560;Half;False;Property;_Internal_TypeTreeLeaf;Internal_TypeTreeLeaf;62;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;645;1536,-896;Float;True;Property;_SurfaceTex;Leaf Surface;13;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;745;2624,-896;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;749;2304,-896;Float;False;744;Main_SurfaceTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1839;2112,-2560;Half;False;Property;_Internal_SetByScript;Internal_SetByScript;67;1;[HideInInspector];Create;True;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1221;-1280,-1536;Float;False;1220;Motion_Output;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1897;1856,-2560;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;66;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-2560;Half;False;Property;_SrcBlend;_SrcBlend;68;1;[HideInInspector];Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1471;160,-3232;Half;False;Property;_ADVANCEDD;[ ADVANCEDD ];48;0;Create;True;0;0;True;1;BCategory(Advanced);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-800,-2560;Half;False;Property;_RenderType;Render Type;4;1;[Enum];Create;True;2;Opaque;0;Cutout;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1513;-928,-3232;Half;False;Property;_BLENDINGG;[ BLENDINGG ];16;0;Create;True;0;0;False;1;BCategory(Trunk Blending);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1846;0,-2560;Float;False;Internal Unity Props;51;;2128;b286e6ef621b64a4fb35da1e13fa143f;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1688;-320,-3232;Half;False;Property;_BRANCHMOTIONN;[ BRANCH MOTIONN ];36;0;Create;True;0;0;True;1;BCategory(Branch Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1865;-1024,768;Half;False;Packed_Trunk;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1793;1280,-128;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1845;1616,-2560;Half;False;Property;_Internal_DebugMask3;Internal_DebugMask3;65;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1788;1920,-832;Half;False;Main_SurfaceTex_G;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1792;896,192;Half;False;Property;_Occlusion;Leaf Occlusion;15;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1838;1376,-2560;Half;False;Property;_Internal_DebugMask2;Internal_DebugMask2;64;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1795;896,0;Float;False;1788;Main_SurfaceTex_G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1723;-1280,-3040;Half;False;Property;_TrunkMotionParameters;!!! Trunk Motion Parameters !!!;32;0;Create;True;0;0;True;1;BMessage(Info, The Trunk Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1896;464,-2560;Half;False;Property;_Internal_ADS;Internal_ADS;50;1;[HideInInspector];Create;True;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1794;1472,-128;Half;False;Main_Occlusion;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1842;912,-2560;Half;False;Property;_Internal_LitSimple;Internal_LitSimple;61;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1057;-128,-800;Half;False;Main_Color_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1918;-352,-3040;Half;False;Property;_BatchingInfo;!!! BatchingInfo;49;0;Create;True;0;0;True;1;BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-128,-2048;Float;False;True;2;Float;ADSShaderGUI;300;0;Lambert;BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Tree Leaf;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;True;False;False;False;True;Off;0;False;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;True;550;10;True;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;300;Utils/ADS Fallback;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;1465;-1280,-3456;Float;False;1614.13;100;Drawers;0;;1,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;708;384,-1024;Float;False;1027.031;100;Normal Texture;0;;0.5019608,0.5019608,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;715;1536,-1024;Float;False;638;100;Surface Texture (Metallic, AO, SubSurface, Smoothness);0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1280,-2688;Float;False;1104;100;Rendering (Unused);0;;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1844;0,-2688;Float;False;2588.058;100;Internal Only;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;751;2304,-1024;Float;False;709.9668;100;Metallic / Smoothness;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1913;-1280,1792;Float;False;771.015;100;Globals;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1543;-1280,-256;Float;False;1155.176;100;Leaf Amount;0;;0.5,0.5,0.5,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1544;0,-256;Float;False;1668.041;100;Ambient Occlusion;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;760;-1280,-1024;Float;False;1364.434;100;Main Texture and Color;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1542;-1280,640;Float;False;5414.462;100;Tree Motion;0;;0.03448272,1,0,1;0;0
WireConnection;1808;0;1804;1
WireConnection;1808;1;1804;1
WireConnection;1815;0;1804;4
WireConnection;1809;0;1808;0
WireConnection;1830;0;1804;2
WireConnection;1819;0;1804;3
WireConnection;1892;0;1637;0
WireConnection;1892;1;1929;148
WireConnection;1733;0;1732;0
WireConnection;1733;1;1712;0
WireConnection;1895;0;1820;0
WireConnection;1895;1;1930;148
WireConnection;616;0;18;4
WireConnection;1919;220;1716;0
WireConnection;1919;221;1715;0
WireConnection;1919;222;1714;0
WireConnection;1919;136;1722;0
WireConnection;1935;220;1555;0
WireConnection;1935;221;1556;0
WireConnection;1935;222;1557;0
WireConnection;1935;136;1892;0
WireConnection;1936;220;1563;0
WireConnection;1936;221;1562;0
WireConnection;1936;222;1561;0
WireConnection;1936;218;1733;0
WireConnection;1936;136;1895;0
WireConnection;1936;279;1903;0
WireConnection;1750;0;1936;0
WireConnection;1751;0;1919;0
WireConnection;1931;152;1924;4
WireConnection;1749;0;1935;0
WireConnection;1308;0;1901;0
WireConnection;1308;1;1814;3
WireConnection;1308;2;1309;0
WireConnection;1921;0;1920;85
WireConnection;1906;171;1905;0
WireConnection;1889;0;1888;0
WireConnection;1889;1;1931;154
WireConnection;486;0;409;0
WireConnection;1801;0;1752;0
WireConnection;1801;1;1796;0
WireConnection;1801;2;1797;0
WireConnection;1890;0;1889;0
WireConnection;1908;0;1906;165
WireConnection;487;0;18;0
WireConnection;1312;0;1308;0
WireConnection;607;5;655;0
WireConnection;1306;0;1890;0
WireConnection;1915;13;607;0
WireConnection;1915;30;1932;0
WireConnection;1629;0;1801;0
WireConnection;1629;1;1923;0
WireConnection;620;0;1915;0
WireConnection;1075;0;36;0
WireConnection;1075;1;1076;0
WireConnection;1075;2;1914;0
WireConnection;1075;3;1313;0
WireConnection;1220;0;1629;0
WireConnection;1343;0;1075;0
WireConnection;744;0;645;4
WireConnection;1836;0;645;3
WireConnection;1934;41;1783;0
WireConnection;646;0;645;1
WireConnection;660;0;745;0
WireConnection;745;0;749;0
WireConnection;745;1;294;0
WireConnection;1865;0;1804;1
WireConnection;1793;0;1902;0
WireConnection;1793;1;1795;0
WireConnection;1793;2;1792;0
WireConnection;1788;0;645;2
WireConnection;1794;0;1793;0
WireConnection;1057;0;409;4
WireConnection;0;0;1343;0
WireConnection;0;1;624;0
WireConnection;0;10;1934;0
WireConnection;0;11;1221;0
ASEEND*/
//CHKSM=AF84987574D00B2A61665E02A2C53D0A8A706D63