// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Plant"
{
	Properties
	{
		[HideInInspector]_Internal_Version("Internal_Version", Float) = 232
		[BBanner(ADS Simple Lit, Plant)]_ADSSimpleLitPlant("< ADS Simple Lit Plant >", Float) = 1
		[BCategory(Rendering)]_RENDERINGG("[ RENDERINGG ]", Float) = 0
		[Enum(Two Sided,0,Back,1,Front,2)]_RenderFaces("Render Faces", Float) = 0
		[Enum(Mirrored Normals,0,Flipped Normals,1)]_NormalInvertOnBackface("Render Backface", Float) = 1
		_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[BCategory(Main)]_MAINN("[ MAINN ]", Float) = 0
		_Color("Plant Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex("Plant Albedo", 2D) = "white" {}
		_NormalScale("Plant Normal Scale", Float) = 1
		[NoScaleOffset]_NormalTex("Plant Normal", 2D) = "bump" {}
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
		LOD 200
		Cull [_RenderFaces]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		 
		//ADS Features
		//ADS End
		  
		#pragma exclude_renderers gles 
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float4 vertexToFrag1248;
			float4 screenPosition;
		};

		uniform float _MotionNoise;
		uniform half _MAINN;
		uniform half _MOTIONLEAFF;
		uniform half _Internal_DebugVariation;
		uniform half _Internal_TypePlant;
		uniform half _SETTINGS;
		uniform half _PlantMotionParameters;
		uniform half _Internal_DebugMask;
		uniform half _Internal_Version;
		uniform half _Internal_SetByScript;
		uniform half _Internal_DebugMask2;
		uniform half _BatchingInfo;
		uniform half _LeafMotionParameters;
		uniform half _MOTIONPLANTT;
		uniform half _RenderFaces;
		uniform half _Internal_LitSimple;
		uniform half _Cutoff;
		uniform half _ADSSimpleLitPlant;
		uniform half4 _MainUVs;
		uniform float _Mode;
		uniform float _Glossiness;
		uniform half _CullMode;
		uniform float _BumpScale;
		uniform sampler2D _MetallicGlossMap;
		uniform half _Internal_UnityToBoxophobic;
		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform half _Internal_ADS;
		uniform half _RENDERINGG;
		uniform half _ADVANCEDD;
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
		uniform half _NormalInvertOnBackface;
		uniform half4 _Color;
		uniform sampler2D _AlbedoTex;
		uniform half4 ADS_GlobalTintColorOne;
		uniform half4 ADS_GlobalTintColorTwo;
		uniform half ADS_GlobalTintIntensity;
		uniform half _GlobalTint;


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
			half MotionScale60_g1901 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g1901 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g1901 = _Time.y * MotionSpeed62_g1901;
			float3 temp_output_95_0_g1901 = ( ( ase_worldPos * MotionScale60_g1901 ) + mulTime90_g1901 );
			half Packed_Variation1129 = v.color.a;
			half MotionVariation269_g1901 = ( _MotionVariation * Packed_Variation1129 );
			half MotionlAmplitude58_g1901 = ( ADS_GlobalAmplitude * _MotionAmplitude );
			float3 temp_output_92_0_g1901 = ( sin( ( temp_output_95_0_g1901 + MotionVariation269_g1901 ) ) * MotionlAmplitude58_g1901 );
			float3 temp_output_160_0_g1901 = ( temp_output_92_0_g1901 + MotionlAmplitude58_g1901 + MotionlAmplitude58_g1901 );
			float2 temp_cast_0 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g1581 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1581 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1581 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1581 = (float3(localunity_ObjectToWorld0w1_g1581 , localunity_ObjectToWorld1w2_g1581 , localunity_ObjectToWorld2w3_g1581));
			float2 panner73_g1579 = ( _Time.y * temp_cast_0 + ( (appendResult6_g1581).xz * ADS_TurbulenceScale ));
			float lerpResult136_g1579 = lerp( 1.0 , saturate( pow( abs( tex2Dlod( ADS_TurbulenceTex, float4( panner73_g1579, 0, 0.0) ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			half Motion_Turbulence1267 = lerpResult136_g1579;
			float3 lerpResult293_g1901 = lerp( temp_output_92_0_g1901 , temp_output_160_0_g1901 , Motion_Turbulence1267);
			half3 GlobalDirection349_g1901 = ADS_GlobalDirection;
			float3 lerpResult280_g1901 = lerp( GlobalDirection349_g1901 , float3(0,1,0) , _MotionVertical);
			half3 MotionDirection59_g1901 = mul( unity_WorldToObject, float4( lerpResult280_g1901 , 0.0 ) ).xyz;
			half Packed_Plant1134 = v.color.r;
			half MotionMask137_g1901 = Packed_Plant1134;
			float3 temp_output_94_0_g1901 = ( ( lerpResult293_g1901 * MotionDirection59_g1901 ) * MotionMask137_g1901 );
			half3 Motion_Plant1158 = temp_output_94_0_g1901;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 break311_g1902 = ase_vertex3Pos;
			half MotionFlutterScale60_g1902 = ( ADS_GlobalScale * _MotionScale3 );
			half MotionFlutterSpeed62_g1902 = ( ADS_GlobalSpeed * _MotionSpeed3 );
			float mulTime303_g1902 = _Time.y * MotionFlutterSpeed62_g1902;
			half MotionlFlutterAmplitude58_g1902 = ( ADS_GlobalAmplitude * _MotionAmplitude3 );
			half Packed_Leaf1169 = v.color.b;
			half MotionMask137_g1902 = Packed_Leaf1169;
			float3 ase_vertexNormal = v.normal.xyz;
			half3 Motion_Leaf1160 = ( sin( ( ( ( break311_g1902.x + break311_g1902.y + break311_g1902.z ) * MotionFlutterScale60_g1902 ) + mulTime303_g1902 ) ) * MotionlFlutterAmplitude58_g1902 * MotionMask137_g1902 * ase_vertexNormal );
			half3 Motion_Output1167 = ( ( Motion_Plant1158 + Motion_Leaf1160 ) * Motion_Turbulence1267 );
			half localunity_ObjectToWorld0w1_g1905 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1905 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1905 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1905 = (float3(localunity_ObjectToWorld0w1_g1905 , localunity_ObjectToWorld1w2_g1905 , localunity_ObjectToWorld2w3_g1905));
			float4 tex2DNode140_g1903 = tex2Dlod( ADS_GlobalTex, float4( ( ( (appendResult6_g1905).xz * (ADS_GlobalUVs).xy ) + (ADS_GlobalUVs).zw ), 0, 0.0) );
			half ADS_GlobalTex_B198_g1903 = tex2DNode140_g1903.b;
			float lerpResult156_g1903 = lerp( ADS_GlobalSizeMin , ADS_GlobalSizeMax , ADS_GlobalTex_B198_g1903);
			float3 temp_output_41_0_g1908 = ( ( lerpResult156_g1903 * _GlobalSize ) * ase_vertex3Pos );
			float3 lerpResult57_g1908 = lerp( temp_output_41_0_g1908 , -ase_vertex3Pos , ( 1.0 - max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) ) ));
			#ifdef LOD_FADE_CROSSFADE
				float3 staticSwitch40_g1908 = lerpResult57_g1908;
			#else
				float3 staticSwitch40_g1908 = temp_output_41_0_g1908;
			#endif
			#ifdef ADS_LODFADE_SCALE
				float3 staticSwitch58_g1908 = staticSwitch40_g1908;
			#else
				float3 staticSwitch58_g1908 = temp_output_41_0_g1908;
			#endif
			half3 Global_Size1252 = staticSwitch58_g1908;
			v.vertex.xyz += ( Motion_Output1167 + Global_Size1252 );
			float4 temp_cast_3 = (1.0).xxxx;
			half4 ADS_GlobalTintColorOne176_g1903 = ADS_GlobalTintColorOne;
			half4 ADS_GlobalTintColorTwo177_g1903 = ADS_GlobalTintColorTwo;
			half ADS_GlobalTex_R180_g1903 = tex2DNode140_g1903.r;
			float4 lerpResult147_g1903 = lerp( ADS_GlobalTintColorOne176_g1903 , ADS_GlobalTintColorTwo177_g1903 , ADS_GlobalTex_R180_g1903);
			half ADS_GlobalTintIntensity181_g1903 = ADS_GlobalTintIntensity;
			half GlobalTint186_g1903 = _GlobalTint;
			float4 lerpResult150_g1903 = lerp( temp_cast_3 , ( lerpResult147_g1903 * ADS_GlobalTintIntensity181_g1903 ) , GlobalTint186_g1903);
			o.vertexToFrag1248 = lerpResult150_g1903;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NormalTex607 = i.uv_texcoord;
			float3 temp_output_13_0_g1909 = UnpackScaleNormal( tex2D( _NormalTex, uv_NormalTex607 ), _NormalScale );
			float3 break17_g1909 = temp_output_13_0_g1909;
			float switchResult12_g1909 = (((i.ASEVFace>0)?(break17_g1909.z):(-break17_g1909.z)));
			float3 appendResult18_g1909 = (float3(break17_g1909.x , break17_g1909.y , switchResult12_g1909));
			float3 lerpResult20_g1909 = lerp( temp_output_13_0_g1909 , appendResult18_g1909 , _NormalInvertOnBackface);
			half3 Main_NormalTex620 = lerpResult20_g1909;
			o.Normal = Main_NormalTex620;
			half4 Main_Color486 = _Color;
			float2 uv_AlbedoTex18 = i.uv_texcoord;
			float4 tex2DNode18 = tex2D( _AlbedoTex, uv_AlbedoTex18 );
			half4 Main_AlbedoTex487 = tex2DNode18;
			half4 Gloabl_Tint1251 = i.vertexToFrag1248;
			o.Albedo = saturate( ( Main_Color486 * Main_AlbedoTex487 * Gloabl_Tint1251 ) ).rgb;
			o.Alpha = 1;
			half Main_AlbedoTex_A616 = tex2DNode18.a;
			float temp_output_41_0_g1913 = Main_AlbedoTex_A616;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39_g1913 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g1913 = Dither4x4Bayer( fmod(clipScreen39_g1913.x, 4), fmod(clipScreen39_g1913.y, 4) );
			float temp_output_47_0_g1913 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g1913 = step( dither39_g1913, temp_output_47_0_g1913 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g1913 = ( temp_output_41_0_g1913 * dither39_g1913 );
			#else
				float staticSwitch40_g1913 = temp_output_41_0_g1913;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g1913 = staticSwitch40_g1913;
			#else
				float staticSwitch50_g1913 = temp_output_41_0_g1913;
			#endif
			clip( staticSwitch50_g1913 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Utils/ADS Fallback"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;1181.959;3172.59;1;True;False
Node;AmplifyShaderEditor.VertexColorNode;1124;-1280,-640;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1266;-1280,-384;Float;False;ADS Global Turbulence;15;;1579;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.RegisterLocalVarNode;1129;-1024,-512;Half;False;Packed_Variation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1174;-512,-320;Float;False;Property;_MotionVariation;Plant Motion Variation;31;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1172;-512,-240;Float;False;1129;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1267;-1024,-384;Half;False;Motion_Turbulence;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1134;-1024,-640;Half;False;Packed_Plant;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1169;-1024,-576;Half;False;Packed_Leaf;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1268;-512,-144;Float;False;1267;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1245;-512,-400;Float;False;Property;_MotionVertical;Plant Motion Vertical;32;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1173;-256,-320;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1152;-512,-48;Float;False;1134;Packed_Plant;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1139;768,-640;Float;False;Property;_MotionAmplitude3;Leaf Flutter Amplitude;35;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1144;768,-384;Float;False;1169;Packed_Leaf;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1150;768,-480;Float;False;Property;_MotionScale3;Leaf Flutter Scale;37;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1142;-512,-480;Float;False;Property;_MotionScale;Plant Motion Scale;30;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1147;-512,-560;Float;False;Property;_MotionSpeed;Plant Motion Speed;29;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1143;-512,-640;Float;False;Property;_MotionAmplitude;Plant Motion Amplitude;28;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1153;768,-560;Float;False;Property;_MotionSpeed3;Leaf Flutter Speed;36;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1265;1088,-640;Float;False;ADS Motion Flutter;-1;;1902;87d8028e5f83178498a65cfa9f0e9ace;1,312,0;5;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;136;FLOAT;0;False;310;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1280;128,-640;Float;False;ADS Motion Generic;25;;1901;81cab27e2a487a645a4ff5eb3c63bd27;6,252,1,278,1,228,1,292,2,330,1,326,1;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1160;1408,-640;Half;False;Motion_Leaf;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1158;384,-640;Half;False;Motion_Plant;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;1262;-1280,512;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1247;-1280,384;Float;False;ADS Global Settings;19;;1903;0fe83146627632b4981f5a0aa1b63801;0;1;171;FLOAT;0;False;3;COLOR;85;COLOR;165;FLOAT;157
Node;AmplifyShaderEditor.GetLocalVarNode;1163;1792,-640;Float;False;1158;Motion_Plant;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1161;1792,-544;Float;False;1160;Motion_Leaf;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;18;-1280,-1280;Float;True;Property;_AlbedoTex;Plant Albedo;9;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;409;-592,-1280;Half;False;Property;_Color;Plant Color;8;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexToFragmentNode;1248;-768,384;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1164;2048,-640;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1269;1792,-448;Float;False;1267;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;655;0,-1280;Half;False;Property;_NormalScale;Plant Normal Scale;10;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1250;-960,512;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;-336,-1280;Half;False;Main_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;-976,-1280;Half;False;Main_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1279;-768,512;Float;False;ADS LODFade Scale;-1;;1908;768eaebf5ab5e9748a01997bf1b9d313;0;1;41;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1270;320,-1088;Half;False;Property;_NormalInvertOnBackface;Render Backface;5;1;[Enum];Create;False;2;Mirrored Normals;0;Flipped Normals;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;607;256,-1280;Float;True;Property;_NormalTex;Plant Normal;11;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1166;2240,-640;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1251;-384,384;Half;False;Gloabl_Tint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1236;-1280,-2368;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;-976,-1152;Half;False;Main_AlbedoTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1257;592,-1280;Float;False;ADS Normal Backface;-1;;1909;4f53bc25e6d8da34db70401bcf363a2a;0;2;13;FLOAT3;0,0,0;False;30;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1255;-1280,-2304;Float;False;1251;Gloabl_Tint;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1167;2400,-640;Half;False;Motion_Output;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1252;-384,512;Half;False;Global_Size;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1235;-1280,-2432;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1175;-1280,-1920;Float;False;1167;Motion_Output;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1237;-944,-2432;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-2048;Float;False;616;Main_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;816,-1280;Half;False;Main_NormalTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1254;-1280,-1856;Float;False;1252;Global_Size;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-1280,-2944;Half;False;Property;_RenderFaces;Render Faces;4;1;[Enum];Create;True;3;Two Sided;0;Back;1;Front;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1179;-1024,-1920;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-1120,-2944;Half;False;Property;_Cutoff;Cutout;6;0;Create;False;3;Off;0;Front;1;Back;2;0;True;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1196;256,-2944;Half;False;Property;_Internal_LitSimple;Internal_LitSimple;51;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;624;-1280,-2176;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1264;-976,-3520;Half;False;Property;_LeafMotionParameters;!!! Leaf Motion Parameters !!!;34;0;Create;True;0;0;True;1;BMessage(Info, The Leaf Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1117;-752,-3616;Half;False;Property;_MOTIONPLANTT;[ MOTION PLANTT ];24;0;Create;True;0;0;True;1;BCategory(Plant Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1272;-1024,-2048;Float;False;ADS LODFade Dither;-1;;1913;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;749;1920,-1280;Float;False;744;Main_SurfaceTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;745;2240,-1280;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1118;-272,-3616;Half;False;Property;_ADVANCEDD;[ ADVANCEDD ];38;0;Create;True;0;0;True;1;BCategory(Advanced);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1113;-1280,-3616;Half;False;Property;_RENDERINGG;[ RENDERINGG ];3;0;Create;True;0;0;True;1;BCategory(Rendering);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;2432,-1280;Half;False;OUT_SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1119;-1280,-3712;Half;False;Property;_ADSSimpleLitPlant;< ADS Simple Lit Plant >;2;0;Create;True;0;0;True;1;BBanner(ADS Simple Lit, Plant);1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1240;-176,-2944;Half;False;Property;_Internal_ADS;Internal_ADS;40;1;[HideInInspector];Create;True;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1199;-640,-2944;Float;False;Internal Unity Props;41;;1914;b286e6ef621b64a4fb35da1e13fa143f;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1261;-672,-3520;Half;False;Property;_BatchingInfo;!!! BatchingInfo;39;0;Create;True;0;0;True;1;BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1195;16,-2944;Half;False;Property;_Internal_TypePlant;Internal_TypePlant;52;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1271;1536,-2944;Float;False;ADS Features Support;-1;;1911;217a332a46517ae4cb8ca16677bdb217;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1263;-1280,-3520;Half;False;Property;_PlantMotionParameters;!!! Plant Motion Parameters !!!;27;0;Create;True;0;0;True;1;BMessage(Info, The Plant Motion Parameters will be overridden by the ADS Materials Helper Component, _Internal_SetByScript, 1, 0, 10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1116;-928,-3616;Half;False;Property;_SETTINGS;[ SETTINGS ];14;0;Create;True;0;0;True;1;BCategory(Settings);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1171;-528,-3616;Half;False;Property;_MOTIONLEAFF;[ MOTION LEAFF ];33;0;Create;True;0;0;True;1;BCategory(Leaf Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1115;-1088,-3616;Half;False;Property;_MAINN;[ MAINN ];7;0;Create;True;0;0;True;1;BCategory(Main);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1241;992,-2944;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;56;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;645;1152,-1280;Float;True;Property;_SurfaceTex;Plant Surface;13;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;294;1920,-1200;Half;False;Property;_Smoothness;Plant Smoothness;12;0;Create;False;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1193;1264,-2944;Half;False;Property;_Internal_SetByScript;Internal_SetByScript;55;1;[HideInInspector];Create;True;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1256;-736,-2432;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1192;736,-2944;Half;False;Property;_Internal_DebugMask2;Internal_DebugMask2;54;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;646;1536,-1280;Half;False;Main_SurfaceTex_R;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1191;496,-2944;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;53;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;744;1536,-1152;Half;False;Main_SurfaceTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1281;-416,-2944;Float;False;ADS Internal Version;0;;1912;858e1f7f7bf8673449834f9aaa5bae83;0;0;1;FLOAT;5
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-512,-2432;Float;False;True;2;Float;ADSShaderGUI;200;0;Lambert;BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Plant;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;True;Off;0;False;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;550;10;False;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;200;Utils/ADS Fallback;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;1198;-640,-3072;Float;False;2398.369;100;Internal Only;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;715;1152,-1408;Float;False;1501.26;100;Surface Input;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1280,-3072;Float;False;417.3682;100;Rendering And Settings;0;;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1253;-1280,256;Float;False;1090.415;100;Globals;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;708;0,-1408;Float;False;1024.6;100;Normal Texture;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;760;-1280,-1408;Float;False;1152.612;100;Main Texture and Color;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1170;-1280,-768;Float;False;3885.181;100;Motion;0;;0.03448272,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1112;-1280,-3840;Float;False;1185.27;100;Drawers;0;;1,0.4980392,0,1;0;0
WireConnection;1129;0;1124;4
WireConnection;1267;0;1266;85
WireConnection;1134;0;1124;1
WireConnection;1169;0;1124;3
WireConnection;1173;0;1174;0
WireConnection;1173;1;1172;0
WireConnection;1265;220;1139;0
WireConnection;1265;221;1153;0
WireConnection;1265;222;1150;0
WireConnection;1265;136;1144;0
WireConnection;1280;220;1143;0
WireConnection;1280;221;1147;0
WireConnection;1280;222;1142;0
WireConnection;1280;218;1173;0
WireConnection;1280;287;1268;0
WireConnection;1280;136;1152;0
WireConnection;1280;279;1245;0
WireConnection;1160;0;1265;0
WireConnection;1158;0;1280;0
WireConnection;1248;0;1247;85
WireConnection;1164;0;1163;0
WireConnection;1164;1;1161;0
WireConnection;1250;0;1247;157
WireConnection;1250;1;1262;0
WireConnection;486;0;409;0
WireConnection;487;0;18;0
WireConnection;1279;41;1250;0
WireConnection;607;5;655;0
WireConnection;1166;0;1164;0
WireConnection;1166;1;1269;0
WireConnection;1251;0;1248;0
WireConnection;616;0;18;4
WireConnection;1257;13;607;0
WireConnection;1257;30;1270;0
WireConnection;1167;0;1166;0
WireConnection;1252;0;1279;0
WireConnection;1237;0;1235;0
WireConnection;1237;1;1236;0
WireConnection;1237;2;1255;0
WireConnection;620;0;1257;0
WireConnection;1179;0;1175;0
WireConnection;1179;1;1254;0
WireConnection;1272;41;791;0
WireConnection;745;0;749;0
WireConnection;745;1;294;0
WireConnection;660;0;294;0
WireConnection;1256;0;1237;0
WireConnection;646;0;645;1
WireConnection;744;0;645;4
WireConnection;0;0;1256;0
WireConnection;0;1;624;0
WireConnection;0;10;1272;0
WireConnection;0;11;1179;0
ASEEND*/
//CHKSM=089A380B325E0EA3328C811479358859E2A74369