// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Generic"
{
	Properties
	{
		[HideInInspector]_Internal_Version("Internal_Version", Float) = 232
		[BBanner(ADS Standard Lit, Generic)]_ADSStandardLitGeneric("< ADS Standard Lit Generic >", Float) = 1
		[BCategory(Rendering)]_RENDERINGG("[ RENDERINGG ]", Float) = 0
		[Enum(Opaque,0,Cutout,1,Fade,2,Transparent,3)]_RenderType("Render Type", Float) = 0
		[Enum(Two Sided,0,Back,1,Front,2)]_RenderFaces("Render Faces", Float) = 0
		[Enum(Mirrored Normals,0,Flipped Normals,1)]_NormalInvertOnBackface("Render Backface", Float) = 1
		[BInteractive(_RenderType, 1)]_RenderTypee("# _RenderTypee", Float) = 0
		_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[BCategory(Main)]_MAINN("[ MAINN ]", Float) = 0
		_Color("Main Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex("Main Albedo", 2D) = "white" {}
		_NormalScale("Main Normal Scale", Float) = 1
		[NoScaleOffset]_NormalTex("Main Normal", 2D) = "bump" {}
		[NoScaleOffset]_SurfaceTex("Main Surface", 2D) = "white" {}
		_Metallic("Main Metallic", Range( 0 , 1)) = 0
		_Smoothness("Main Smoothness", Range( 0 , 1)) = 0.5
		[Space(10)]_UVZero("Main UVs", Vector) = (1,1,0,0)
		[BCategory(Settings)]_SETTINGSS("[ SETTINGSS ]", Float) = 0
		[HideInInspector]_MotionNoise("Motion Noise", Float) = 1
		_GlobalTurbulence("Global Turbulence", Range( 0 , 1)) = 1
		[BCategory(Motion)]_MOTIONN("[ MOTIONN ]", Float) = 0
		[KeywordEnum(World,Local)] _MotionSpace("Motion Space", Float) = 0
		[BInteractive(_MotionSpace, 1)]_MotionSpacee("# MotionSpacee", Float) = 0
		_MotionLocalDirection("Motion Local Direction", Vector) = (1,0,0,0)
		[BInteractive(ON)]_MotionSpacee_ON("# MotionSpacee_ON", Float) = 0
		_MotionAmplitude("Motion Amplitude", Float) = 0
		_MotionSpeed("Motion Speed", Float) = 0
		_MotionScale("Motion Scale", Float) = 0
		_MotionVariation("Motion Variation", Float) = 0
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
		[HideInInspector]_Internal_TypeGeneric("Internal_TypeGeneric", Float) = 1
		[HideInInspector]_Internal_DebugMask("Internal_DebugMask", Float) = 1
		[HideInInspector]_LocalDirection("Internal_LocalDirection", Vector) = (0,0,0,0)
		[HideInInspector]_SrcBlend("_SrcBlend", Float) = 1
		[HideInInspector]_DstBlend("_DstBlend", Float) = 10
		[HideInInspector]_ZWrite("_ZWrite", Float) = 1
		[HideInInspector]_Internal_DebugVariation("Internal_DebugVariation", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "DisableBatching" = "True" }
		LOD 300
		Cull [_RenderFaces]
		ZWrite [_ZWrite]
		Blend [_SrcBlend] [_DstBlend]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature _MOTIONSPACE_WORLD _MOTIONSPACE_LOCAL
		#pragma shader_feature _RENDERTYPEKEY_OPAQUE _RENDERTYPEKEY_CUT _RENDERTYPEKEY_FADE _RENDERTYPEKEY_TRANSPARENT
		 
		//ADS Features
		//ADS End
		  
		#pragma exclude_renderers gles 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float4 screenPosition;
		};

		uniform float _MotionNoise;
		uniform half _DstBlend;
		uniform half _MAINN;
		uniform half4 _MainUVs;
		uniform float _Mode;
		uniform float _Glossiness;
		uniform half _CullMode;
		uniform float _BumpScale;
		uniform sampler2D _MetallicGlossMap;
		uniform half _Internal_UnityToBoxophobic;
		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform half _Internal_DebugMask;
		uniform half _ZWrite;
		uniform half _RenderType;
		uniform half _Cutoff;
		uniform half _SETTINGSS;
		uniform half _MOTIONN;
		uniform half _Internal_TypeGeneric;
		uniform half _RenderTypee;
		uniform half _Internal_ADS;
		uniform half _SrcBlend;
		uniform half _BatchingInfo;
		uniform half _Internal_LitStandard;
		uniform half _MotionSpacee_ON;
		uniform half _RenderFaces;
		uniform half _RENDERINGG;
		uniform half _ADSStandardLitGeneric;
		uniform half _Internal_DebugVariation;
		uniform half _MotionSpacee;
		uniform half _ADVANCEDD;
		uniform half3 _LocalDirection;
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
		uniform half3 _MotionLocalDirection;
		uniform half _NormalScale;
		uniform sampler2D _NormalTex;
		uniform half4 _UVZero;
		uniform half _NormalInvertOnBackface;
		uniform sampler2D _AlbedoTex;
		uniform half4 _Color;
		uniform sampler2D _SurfaceTex;
		uniform half _Metallic;
		uniform half _Smoothness;


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
			float3 ase_vertex3Pos = v.vertex.xyz;
			#if defined(_MOTIONSPACE_WORLD)
				float3 staticSwitch345_g1889 = ase_worldPos;
			#elif defined(_MOTIONSPACE_LOCAL)
				float3 staticSwitch345_g1889 = ase_vertex3Pos;
			#else
				float3 staticSwitch345_g1889 = ase_worldPos;
			#endif
			half MotionScale60_g1889 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g1889 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g1889 = _Time.y * MotionSpeed62_g1889;
			float3 temp_output_95_0_g1889 = ( ( staticSwitch345_g1889 * MotionScale60_g1889 ) + mulTime90_g1889 );
			half Packed_Variation1138 = v.color.a;
			half MotionVariation269_g1889 = ( _MotionVariation * Packed_Variation1138 );
			half MotionlAmplitude58_g1889 = ( ADS_GlobalAmplitude * _MotionAmplitude );
			float3 temp_output_92_0_g1889 = ( sin( ( temp_output_95_0_g1889 + MotionVariation269_g1889 ) ) * MotionlAmplitude58_g1889 );
			float3 temp_output_160_0_g1889 = ( temp_output_92_0_g1889 + MotionlAmplitude58_g1889 + MotionlAmplitude58_g1889 );
			float2 temp_cast_0 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g1874 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1874 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1874 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1874 = (float3(localunity_ObjectToWorld0w1_g1874 , localunity_ObjectToWorld1w2_g1874 , localunity_ObjectToWorld2w3_g1874));
			float2 panner73_g1872 = ( _Time.y * temp_cast_0 + ( (appendResult6_g1874).xz * ADS_TurbulenceScale ));
			float lerpResult136_g1872 = lerp( 1.0 , saturate( pow( abs( tex2Dlod( ADS_TurbulenceTex, float4( panner73_g1872, 0, 0.0) ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			half Motion_Turbulence1162 = lerpResult136_g1872;
			float3 lerpResult293_g1889 = lerp( temp_output_92_0_g1889 , temp_output_160_0_g1889 , Motion_Turbulence1162);
			half3 GlobalDirection349_g1889 = ADS_GlobalDirection;
			#if defined(_MOTIONSPACE_WORLD)
				float3 staticSwitch343_g1889 = mul( unity_WorldToObject, float4( GlobalDirection349_g1889 , 0.0 ) ).xyz;
			#elif defined(_MOTIONSPACE_LOCAL)
				float3 staticSwitch343_g1889 = _MotionLocalDirection;
			#else
				float3 staticSwitch343_g1889 = mul( unity_WorldToObject, float4( GlobalDirection349_g1889 , 0.0 ) ).xyz;
			#endif
			half3 MotionDirection59_g1889 = staticSwitch343_g1889;
			half Packed_Mask1141 = v.color.r;
			half MotionMask137_g1889 = Packed_Mask1141;
			float3 temp_output_94_0_g1889 = ( ( lerpResult293_g1889 * MotionDirection59_g1889 ) * MotionMask137_g1889 );
			half3 Motion_Generic1148 = temp_output_94_0_g1889;
			half3 Motion_Output1152 = ( Motion_Generic1148 * Motion_Turbulence1162 );
			v.vertex.xyz += Motion_Output1152;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult564 = (float2(_UVZero.x , _UVZero.y));
			float2 appendResult565 = (float2(_UVZero.z , _UVZero.w));
			half2 Main_UVs587 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			float3 temp_output_13_0_g1892 = UnpackScaleNormal( tex2D( _NormalTex, Main_UVs587 ), _NormalScale );
			float3 break17_g1892 = temp_output_13_0_g1892;
			float switchResult12_g1892 = (((i.ASEVFace>0)?(break17_g1892.z):(-break17_g1892.z)));
			float3 appendResult18_g1892 = (float3(break17_g1892.x , break17_g1892.y , switchResult12_g1892));
			float3 lerpResult20_g1892 = lerp( temp_output_13_0_g1892 , appendResult18_g1892 , _NormalInvertOnBackface);
			half3 Main_NormalTex620 = lerpResult20_g1892;
			o.Normal = Main_NormalTex620;
			float4 tex2DNode18 = tex2D( _AlbedoTex, Main_UVs587 );
			half4 Main_AlbedoTex487 = tex2DNode18;
			half4 Main_Color486 = _Color;
			float4 temp_output_1075_0 = ( Main_AlbedoTex487 * Main_Color486 );
			half Main_Color_A1057 = _Color.a;
			half Main_AlbedoTex_A616 = tex2DNode18.a;
			#if defined(_RENDERTYPEKEY_OPAQUE)
				float4 staticSwitch1114 = temp_output_1075_0;
			#elif defined(_RENDERTYPEKEY_CUT)
				float4 staticSwitch1114 = temp_output_1075_0;
			#elif defined(_RENDERTYPEKEY_FADE)
				float4 staticSwitch1114 = temp_output_1075_0;
			#elif defined(_RENDERTYPEKEY_TRANSPARENT)
				float4 staticSwitch1114 = ( Main_AlbedoTex487 * Main_Color486 * Main_Color_A1057 * Main_AlbedoTex_A616 );
			#else
				float4 staticSwitch1114 = temp_output_1075_0;
			#endif
			o.Albedo = staticSwitch1114.rgb;
			float4 tex2DNode645 = tex2D( _SurfaceTex, Main_UVs587 );
			half MAin_SurfaceTex_R646 = tex2DNode645.r;
			half OUT_METALLIC748 = ( MAin_SurfaceTex_R646 * _Metallic );
			o.Metallic = OUT_METALLIC748;
			half Main_SurfaceTex_A744 = tex2DNode645.a;
			half OUT_SMOOTHNESS660 = ( Main_SurfaceTex_A744 * _Smoothness );
			o.Smoothness = OUT_SMOOTHNESS660;
			float temp_output_1133_0 = 1.0;
			float temp_output_1058_0 = ( Main_Color_A1057 * Main_AlbedoTex_A616 );
			#if defined(_RENDERTYPEKEY_OPAQUE)
				float staticSwitch1112 = temp_output_1133_0;
			#elif defined(_RENDERTYPEKEY_CUT)
				float staticSwitch1112 = temp_output_1133_0;
			#elif defined(_RENDERTYPEKEY_FADE)
				float staticSwitch1112 = temp_output_1058_0;
			#elif defined(_RENDERTYPEKEY_TRANSPARENT)
				float staticSwitch1112 = temp_output_1058_0;
			#else
				float staticSwitch1112 = temp_output_1133_0;
			#endif
			float temp_output_41_0_g1900 = staticSwitch1112;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39_g1900 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g1900 = Dither4x4Bayer( fmod(clipScreen39_g1900.x, 4), fmod(clipScreen39_g1900.y, 4) );
			float temp_output_47_0_g1900 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g1900 = step( dither39_g1900, temp_output_47_0_g1900 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g1900 = ( temp_output_41_0_g1900 * dither39_g1900 );
			#else
				float staticSwitch40_g1900 = temp_output_41_0_g1900;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g1900 = staticSwitch40_g1900;
			#else
				float staticSwitch50_g1900 = temp_output_41_0_g1900;
			#endif
			o.Alpha = staticSwitch50_g1900;
			#if defined(_RENDERTYPEKEY_OPAQUE)
				float staticSwitch1113 = temp_output_1133_0;
			#elif defined(_RENDERTYPEKEY_CUT)
				float staticSwitch1113 = Main_AlbedoTex_A616;
			#elif defined(_RENDERTYPEKEY_FADE)
				float staticSwitch1113 = temp_output_1133_0;
			#elif defined(_RENDERTYPEKEY_TRANSPARENT)
				float staticSwitch1113 = temp_output_1133_0;
			#else
				float staticSwitch1113 = temp_output_1133_0;
			#endif
			float temp_output_41_0_g1901 = staticSwitch1113;
			float2 clipScreen39_g1901 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g1901 = Dither4x4Bayer( fmod(clipScreen39_g1901.x, 4), fmod(clipScreen39_g1901.y, 4) );
			float temp_output_47_0_g1901 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g1901 = step( dither39_g1901, temp_output_47_0_g1901 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g1901 = ( temp_output_41_0_g1901 * dither39_g1901 );
			#else
				float staticSwitch40_g1901 = temp_output_41_0_g1901;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g1901 = staticSwitch40_g1901;
			#else
				float staticSwitch50_g1901 = temp_output_41_0_g1901;
			#endif
			clip( staticSwitch50_g1901 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Utils/ADS Fallback"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;930.1174;3520.632;1;True;False
Node;AmplifyShaderEditor.Vector4Node;563;-1280,-672;Half;False;Property;_UVZero;Main UVs;17;0;Create;False;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;564;-1024,-672;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-1280,-896;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;565;-1024,-592;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-832,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;1154;-1280,-128;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1161;-1280,192;Float;False;ADS Global Turbulence;19;;1872;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-624,-896;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1138;-1024,-48;Half;False;Packed_Variation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1162;-1024,192;Half;False;Motion_Turbulence;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1141;-1024,-128;Half;False;Packed_Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1140;-512,192;Float;False;1138;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1139;-512,112;Float;False;Property;_MotionVariation;Motion Variation;33;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;587;-448,-896;Half;False;Main_UVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;644;2688,-896;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1142;-512,32;Float;False;Property;_MotionScale;Motion Scale;32;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1170;-512,512;Half;False;Property;_MotionLocalDirection;Motion Local Direction;28;0;Create;False;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;1144;-512,-128;Float;False;Property;_MotionAmplitude;Motion Amplitude;30;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1163;-512,288;Float;False;1162;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1143;-256,112;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1145;-512,-48;Float;False;Property;_MotionSpeed;Motion Speed;31;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1146;-512,384;Float;False;1141;Packed_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;588;-128,-896;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;1171;128,-128;Float;False;ADS Motion Generic;24;;1889;81cab27e2a487a645a4ff5eb3c63bd27;6,252,2,278,1,228,1,292,2,330,2,326,2;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;18;80,-896;Float;True;Property;_AlbedoTex;Main Albedo;11;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;409;768,-896;Half;False;Property;_Color;Main Color;10;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;645;2944,-896;Float;True;Property;_SurfaceTex;Main Surface;14;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;646;3344,-896;Half;False;MAin_SurfaceTex_R;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;604;1536,-896;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;655;1536,-768;Half;False;Property;_NormalScale;Main Normal Scale;12;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;384,-768;Half;False;Main_AlbedoTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1057;1024,-800;Half;False;Main_Color_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;744;3344,-768;Half;False;Main_SurfaceTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1148;384,-128;Half;False;Motion_Generic;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;384,-896;Half;False;Main_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;294;3712,-624;Half;False;Property;_Smoothness;Main Smoothness;16;0;Create;False;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-1568;Float;False;616;Main_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;1024,-896;Half;False;Main_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1150;768,-128;Float;False;1148;Motion_Generic;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;656;3712,-896;Float;False;646;MAin_SurfaceTex_R;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;607;1808,-896;Float;True;Property;_NormalTex;Main Normal;13;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;750;3712,-816;Half;False;Property;_Metallic;Main Metallic;15;0;Create;False;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1059;-1280,-1664;Float;False;1057;Main_Color_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;749;3712,-704;Float;False;744;Main_SurfaceTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1164;768,-32;Float;False;1162;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1165;1856,-704;Half;False;Property;_NormalInvertOnBackface;Render Backface;6;1;[Enum];Create;False;2;Mirrored Normals;0;Flipped Normals;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;745;4032,-720;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1151;1024,-128;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1280,-2432;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1115;-1280,-2272;Float;False;1057;Main_Color_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1133;-896,-1792;Float;False;const;-1;;1893;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1116;-1280,-2192;Float;False;616;Main_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;-1024,-1664;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;657;4032,-896;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1134;2128,-896;Float;False;ADS Normal Backface;-1;;1892;4f53bc25e6d8da34db70401bcf363a2a;0;2;13;FLOAT3;0,0,0;False;30;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1076;-1280,-2368;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;4224,-720;Half;False;OUT_SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1113;-640,-1632;Float;False;Property;_RenderTypeKey;RenderTypeKey;5;0;Create;True;0;0;False;0;0;0;0;False;_ALPHABLEND_ON;KeywordEnum;4;Opaque;Cut;Fade;Transparent;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1075;-1024,-2432;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;2368,-896;Half;False;Main_NormalTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;748;4224,-896;Half;False;OUT_METALLIC;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1112;-640,-1792;Float;False;Property;_RenderTypeKey;RenderTypeKey;5;0;Create;True;0;0;False;0;0;0;0;False;_ALPHABLEND_ON;KeywordEnum;4;Opaque;Cut;Fade;Transparent;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1117;-1024,-2304;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1152;1216,-128;Half;False;Motion_Output;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1128;912,-2944;Half;False;Property;_Internal_LitStandard;Internal_LitStandard;47;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1160;-640,-3520;Half;False;Property;_BatchingInfo;!!! BatchingInfo;35;0;Create;True;0;0;True;1;BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1169;-512,655;Float;False;Property;_MotionSpace;Motion Space;26;0;Create;False;0;0;True;0;0;0;0;True;;KeywordEnum;2;World;Local;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1173;-880,-3520;Half;False;Property;_MotionSpacee_ON;# MotionSpacee_ON;29;0;Create;True;0;0;True;1;BInteractive(ON);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1166;1664,-2944;Float;False;ADS Features Support;-1;;1903;217a332a46517ae4cb8ca16677bdb217;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1114;-640,-2432;Float;False;Property;_RenderTypeKey;RenderTypeKey;5;0;Create;True;0;0;False;0;0;0;0;False;_ALPHABLEND_ON;KeywordEnum;4;Opaque;Cut;Fade;Transparent;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-2944;Half;False;Property;_SrcBlend;_SrcBlend;51;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1131;480,-2944;Half;False;Property;_Internal_ADS;Internal_ADS;36;1;[HideInInspector];Create;True;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1107;-576,-3616;Half;False;Property;_ADVANCEDD;[ ADVANCEDD ];34;0;Create;True;0;0;True;1;BCategory(Advanced);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1172;-1072,-3520;Half;False;Property;_MotionSpacee;# MotionSpacee;27;0;Create;True;0;0;True;1;BInteractive(_MotionSpace, 1);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1177;240,-2944;Float;False;ADS Internal Version;0;;1904;858e1f7f7bf8673449834f9aaa5bae83;0;0;1;FLOAT;5
Node;AmplifyShaderEditor.Vector3Node;1175;-1280,288;Half;False;Property;_LocalDirection;Internal_LocalDirection;50;1;[HideInInspector];Create;False;0;0;True;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;1101;-1280,-3616;Half;False;Property;_RENDERINGG;[ RENDERINGG ];3;0;Create;True;0;0;True;1;BCategory(Rendering);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-640,-2944;Half;False;Property;_RenderFaces;Render Faces;5;1;[Enum];Create;True;3;Two Sided;0;Back;1;Front;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1132;1392,-2944;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;54;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1102;-1280,-3712;Half;False;Property;_ADSStandardLitGeneric;< ADS Standard Lit Generic >;2;0;Create;True;0;0;True;1;BBanner(ADS Standard Lit, Generic);1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;624;-1280,-2016;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1168;-384,-1632;Float;False;ADS LODFade Dither;-1;;1901;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1167;-384,-1792;Float;False;ADS LODFade Dither;-1;;1900;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1129;1152,-2944;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;49;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1127;0,-2944;Float;False;Internal Unity Props;37;;1902;b286e6ef621b64a4fb35da1e13fa143f;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-2944;Half;False;Property;_DstBlend;_DstBlend;52;1;[HideInInspector];Create;True;0;0;True;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;752;-1280,-1936;Float;False;748;OUT_METALLIC;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1125;-384,-1472;Float;False;1152;Motion_Output;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1104;-1088,-3616;Half;False;Property;_MAINN;[ MAINN ];9;0;Create;True;0;0;True;1;BCategory(Main);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1106;-752,-3616;Half;False;Property;_MOTIONN;[ MOTIONN ];23;0;Create;True;0;0;True;1;BCategory(Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;654;-1280,-1856;Float;False;660;OUT_SMOOTHNESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1103;-1280,-3520;Half;False;Property;_RenderTypee;# _RenderTypee;7;0;Create;True;0;0;True;1;BInteractive(_RenderType, 1);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1126;672,-2944;Half;False;Property;_Internal_TypeGeneric;Internal_TypeGeneric;48;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-816,-2944;Half;False;Property;_RenderType;Render Type;4;1;[Enum];Create;True;4;Opaque;0;Cutout;1;Fade;2;Transparent;3;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-960,-2944;Half;False;Property;_ZWrite;_ZWrite;53;1;[HideInInspector];Create;True;2;Off;0;On;1;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1105;-928,-3616;Half;False;Property;_SETTINGSS;[ SETTINGSS ];18;0;Create;True;0;0;True;1;BCategory(Settings);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-448,-2944;Half;False;Property;_Cutoff;Cutout;8;0;Create;False;3;Off;0;Front;1;Back;2;0;True;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,-2176;Float;False;True;2;Float;ADSShaderGUI;300;0;Standard;BOXOPHOBIC/Advanced Dynamic Shaders/Standard Lit/Generic;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;True;Off;0;True;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Opaque;;Geometry;All;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;1;5;True;550;10;True;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;300;Utils/ADS Fallback;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1280,-3072;Float;False;1084;100;Rendering;0;;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1153;-1280,-256;Float;False;2698.834;100;Motion;0;;0.03448272,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1130;0,-3072;Float;False;1888.072;100;Internal Only;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;751;3712,-1024;Float;False;713.7266;100;Metallic / Smoothness;0;;1,0.7450981,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;712;-1280,-1024;Float;False;1039.27;100;Main UVs;0;;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1100;-1280,-3840;Float;False;897.0701;100;Drawers;0;;1,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;708;1536,-1024;Float;False;1038.73;100;Normal Texture;0;;0.5019608,0.5019608,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;715;2688,-1024;Float;False;890.0676;100;Smoothness Texture;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;760;-128,-1024;Float;False;1361.88;100;Main Texture and Color;0;;0,0.751724,1,1;0;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;1138;0;1154;4
WireConnection;1162;0;1161;85
WireConnection;1141;0;1154;1
WireConnection;587;0;575;0
WireConnection;1143;0;1139;0
WireConnection;1143;1;1140;0
WireConnection;1171;220;1144;0
WireConnection;1171;221;1145;0
WireConnection;1171;222;1142;0
WireConnection;1171;218;1143;0
WireConnection;1171;287;1163;0
WireConnection;1171;136;1146;0
WireConnection;1171;342;1170;0
WireConnection;18;1;588;0
WireConnection;645;1;644;0
WireConnection;646;0;645;1
WireConnection;616;0;18;4
WireConnection;1057;0;409;4
WireConnection;744;0;645;4
WireConnection;1148;0;1171;0
WireConnection;487;0;18;0
WireConnection;486;0;409;0
WireConnection;607;1;604;0
WireConnection;607;5;655;0
WireConnection;745;0;749;0
WireConnection;745;1;294;0
WireConnection;1151;0;1150;0
WireConnection;1151;1;1164;0
WireConnection;1058;0;1059;0
WireConnection;1058;1;791;0
WireConnection;657;0;656;0
WireConnection;657;1;750;0
WireConnection;1134;13;607;0
WireConnection;1134;30;1165;0
WireConnection;660;0;745;0
WireConnection;1113;1;1133;0
WireConnection;1113;0;791;0
WireConnection;1113;2;1133;0
WireConnection;1113;3;1133;0
WireConnection;1075;0;36;0
WireConnection;1075;1;1076;0
WireConnection;620;0;1134;0
WireConnection;748;0;657;0
WireConnection;1112;1;1133;0
WireConnection;1112;0;1133;0
WireConnection;1112;2;1058;0
WireConnection;1112;3;1058;0
WireConnection;1117;0;36;0
WireConnection;1117;1;1076;0
WireConnection;1117;2;1115;0
WireConnection;1117;3;1116;0
WireConnection;1152;0;1151;0
WireConnection;1114;1;1075;0
WireConnection;1114;0;1075;0
WireConnection;1114;2;1075;0
WireConnection;1114;3;1117;0
WireConnection;1168;41;1113;0
WireConnection;1167;41;1112;0
WireConnection;0;0;1114;0
WireConnection;0;1;624;0
WireConnection;0;3;752;0
WireConnection;0;4;654;0
WireConnection;0;9;1167;0
WireConnection;0;10;1168;0
WireConnection;0;11;1125;0
ASEEND*/
//CHKSM=ADA4C63450F9A4977A5C9F98E021426DDA11E155