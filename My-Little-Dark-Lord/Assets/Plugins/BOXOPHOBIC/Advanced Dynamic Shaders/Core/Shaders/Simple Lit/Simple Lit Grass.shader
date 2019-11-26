// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Grass"
{
	Properties
	{
		[HideInInspector]_Internal_Version("Internal_Version", Float) = 232
		[BBanner(ADS Simple Lit, Grass)]_ADSSimpleLitGrass("< ADS Simple Lit Grass >", Float) = 1
		[BCategory(Rendering)]_RENDERINGG("[ RENDERINGG ]", Float) = 0
		[Enum(Two Sided,0,Back,1,Front,2)]_RenderFaces("Render Faces", Float) = 0
		[Enum(Mirrored Normals,0,Flipped Normals,1)]_NormalInvertOnBackface("Render Backface", Float) = 1
		_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[BCategory(Main)]_MAINN("[ MAINN ]", Float) = 0
		_Color("Grass Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex("Grass Albedo", 2D) = "white" {}
		_NormalScale("Grass Normal Scale", Float) = 1
		[NoScaleOffset]_NormalTex("Grass Normal", 2D) = "bump" {}
		[BCategory(Settings)]_SETTINGS("[ SETTINGS ]", Float) = 0
		[HideInInspector]_MotionNoise("Motion Noise", Float) = 1
		_GlobalTurbulence("Global Turbulence", Range( 0 , 1)) = 1
		_GlobalTint("Global Tint", Range( 0 , 1)) = 1
		_GlobalSize("Global Size", Range( 0 , 1)) = 1
		[BCategory(Grass Motion)]_MOTIONGRASS("[ MOTION GRASS ]", Float) = 0
		_MotionAmplitude("Grass Motion Amplitude", Float) = 0
		_MotionSpeed("Grass Motion Speed", Float) = 0
		_MotionScale("Grass Motion Scale", Float) = 0
		_MotionVariation("Grass Motion Variation", Float) = 0
		[BCategory(Leaf Motion)]_LEAFMOTIONN("[ LEAF MOTIONN ]", Float) = 0
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
		[HideInInspector]_Internal_TypeGrass("Internal_TypeGrass", Float) = 1
		[HideInInspector]_Internal_DebugMask("Internal_DebugMask", Float) = 1
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
			float4 vertexToFrag1244;
			float4 screenPosition;
		};

		uniform float _MotionNoise;
		uniform half _MOTIONGRASS;
		uniform half _BatchingInfo;
		uniform half _ADVANCEDD;
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
		uniform half _SETTINGS;
		uniform half _Internal_LitSimple;
		uniform half _Cutoff;
		uniform half _LEAFMOTIONN;
		uniform half _ADSSimpleLitGrass;
		uniform half _RenderFaces;
		uniform half _MAINN;
		uniform half _Internal_DebugVariation;
		uniform half _Internal_DebugMask;
		uniform half _Internal_TypeGrass;
		uniform half _RENDERINGG;
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
			half MotionScale60_g1892 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g1892 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g1892 = _Time.y * MotionSpeed62_g1892;
			float2 temp_output_95_0_g1892 = ( ( (ase_worldPos).xz * MotionScale60_g1892 ) + mulTime90_g1892 );
			half Packed_Variation1261 = v.color.a;
			half MotionVariation269_g1892 = ( _MotionVariation * Packed_Variation1261 );
			half MotionlAmplitude58_g1892 = ( ADS_GlobalAmplitude * _MotionAmplitude );
			float2 temp_output_92_0_g1892 = ( sin( ( temp_output_95_0_g1892 + MotionVariation269_g1892 ) ) * MotionlAmplitude58_g1892 );
			float2 temp_output_160_0_g1892 = ( temp_output_92_0_g1892 + MotionlAmplitude58_g1892 + MotionlAmplitude58_g1892 );
			float2 temp_cast_0 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g1581 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1581 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1581 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1581 = (float3(localunity_ObjectToWorld0w1_g1581 , localunity_ObjectToWorld1w2_g1581 , localunity_ObjectToWorld2w3_g1581));
			float2 panner73_g1579 = ( _Time.y * temp_cast_0 + ( (appendResult6_g1581).xz * ADS_TurbulenceScale ));
			float lerpResult136_g1579 = lerp( 1.0 , saturate( pow( abs( tex2Dlod( ADS_TurbulenceTex, float4( panner73_g1579, 0, 0.0) ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			half Motion_Turbulence1262 = lerpResult136_g1579;
			float2 lerpResult293_g1892 = lerp( temp_output_92_0_g1892 , temp_output_160_0_g1892 , Motion_Turbulence1262);
			half3 GlobalDirection349_g1892 = ADS_GlobalDirection;
			float3 break339_g1892 = mul( unity_WorldToObject, float4( GlobalDirection349_g1892 , 0.0 ) ).xyz;
			float2 appendResult340_g1892 = (float2(break339_g1892.x , break339_g1892.z));
			half2 MotionDirection59_g1892 = appendResult340_g1892;
			half Packed_Grass1263 = v.color.r;
			half MotionMask137_g1892 = Packed_Grass1263;
			float2 temp_output_94_0_g1892 = ( ( lerpResult293_g1892 * MotionDirection59_g1892 ) * MotionMask137_g1892 );
			float2 break311_g1892 = temp_output_94_0_g1892;
			float3 appendResult308_g1892 = (float3(break311_g1892.x , 0.0 , break311_g1892.y));
			half3 Motion_Grass1279 = appendResult308_g1892;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 break311_g1893 = ase_vertex3Pos;
			half MotionFlutterScale60_g1893 = ( ADS_GlobalScale * _MotionScale3 );
			half MotionFlutterSpeed62_g1893 = ( ADS_GlobalSpeed * _MotionSpeed3 );
			float mulTime303_g1893 = _Time.y * MotionFlutterSpeed62_g1893;
			half MotionlFlutterAmplitude58_g1893 = ( ADS_GlobalAmplitude * _MotionAmplitude3 );
			half MotionMask137_g1893 = Packed_Grass1263;
			float3 ase_vertexNormal = v.normal.xyz;
			half3 Motion_Leaf1278 = ( sin( ( ( ( break311_g1893.x + break311_g1893.y + break311_g1893.z ) * MotionFlutterScale60_g1893 ) + mulTime303_g1893 ) ) * MotionlFlutterAmplitude58_g1893 * MotionMask137_g1893 * ase_vertexNormal );
			half3 Motion_Output1285 = ( ( Motion_Grass1279 + Motion_Leaf1278 ) * Motion_Turbulence1262 );
			half localunity_ObjectToWorld0w1_g1896 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1896 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1896 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1896 = (float3(localunity_ObjectToWorld0w1_g1896 , localunity_ObjectToWorld1w2_g1896 , localunity_ObjectToWorld2w3_g1896));
			float4 tex2DNode140_g1894 = tex2Dlod( ADS_GlobalTex, float4( ( ( (appendResult6_g1896).xz * (ADS_GlobalUVs).xy ) + (ADS_GlobalUVs).zw ), 0, 0.0) );
			half ADS_GlobalTex_B198_g1894 = tex2DNode140_g1894.b;
			float lerpResult156_g1894 = lerp( ADS_GlobalSizeMin , ADS_GlobalSizeMax , ADS_GlobalTex_B198_g1894);
			float3 temp_output_41_0_g1899 = ( ( lerpResult156_g1894 * _GlobalSize ) * ase_vertex3Pos );
			float3 lerpResult57_g1899 = lerp( temp_output_41_0_g1899 , -ase_vertex3Pos , ( 1.0 - max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) ) ));
			#ifdef LOD_FADE_CROSSFADE
				float3 staticSwitch40_g1899 = lerpResult57_g1899;
			#else
				float3 staticSwitch40_g1899 = temp_output_41_0_g1899;
			#endif
			#ifdef ADS_LODFADE_SCALE
				float3 staticSwitch58_g1899 = staticSwitch40_g1899;
			#else
				float3 staticSwitch58_g1899 = temp_output_41_0_g1899;
			#endif
			half3 Global_Size1248 = staticSwitch58_g1899;
			v.vertex.xyz += ( Motion_Output1285 + Global_Size1248 );
			float4 temp_cast_3 = (1.0).xxxx;
			half4 ADS_GlobalTintColorOne176_g1894 = ADS_GlobalTintColorOne;
			half4 ADS_GlobalTintColorTwo177_g1894 = ADS_GlobalTintColorTwo;
			half ADS_GlobalTex_R180_g1894 = tex2DNode140_g1894.r;
			float4 lerpResult147_g1894 = lerp( ADS_GlobalTintColorOne176_g1894 , ADS_GlobalTintColorTwo177_g1894 , ADS_GlobalTex_R180_g1894);
			half ADS_GlobalTintIntensity181_g1894 = ADS_GlobalTintIntensity;
			half GlobalTint186_g1894 = _GlobalTint;
			float4 lerpResult150_g1894 = lerp( temp_cast_3 , ( lerpResult147_g1894 * ADS_GlobalTintIntensity181_g1894 ) , GlobalTint186_g1894);
			o.vertexToFrag1244 = lerpResult150_g1894;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NormalTex607 = i.uv_texcoord;
			float3 temp_output_13_0_g1900 = UnpackScaleNormal( tex2D( _NormalTex, uv_NormalTex607 ), _NormalScale );
			float3 break17_g1900 = temp_output_13_0_g1900;
			float switchResult12_g1900 = (((i.ASEVFace>0)?(break17_g1900.z):(-break17_g1900.z)));
			float3 appendResult18_g1900 = (float3(break17_g1900.x , break17_g1900.y , switchResult12_g1900));
			float3 lerpResult20_g1900 = lerp( temp_output_13_0_g1900 , appendResult18_g1900 , _NormalInvertOnBackface);
			half3 Main_NormalTex620 = lerpResult20_g1900;
			o.Normal = Main_NormalTex620;
			half4 Main_Color486 = _Color;
			float2 uv_AlbedoTex18 = i.uv_texcoord;
			float4 tex2DNode18 = tex2D( _AlbedoTex, uv_AlbedoTex18 );
			half4 Main_AlbedoTex487 = tex2DNode18;
			half4 Gloabl_Tint1247 = i.vertexToFrag1244;
			o.Albedo = saturate( ( Main_Color486 * Main_AlbedoTex487 * Gloabl_Tint1247 ) ).rgb;
			o.Alpha = 1;
			half Main_AlbedoTex_A616 = tex2DNode18.a;
			float temp_output_41_0_g1903 = Main_AlbedoTex_A616;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39_g1903 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g1903 = Dither4x4Bayer( fmod(clipScreen39_g1903.x, 4), fmod(clipScreen39_g1903.y, 4) );
			float temp_output_47_0_g1903 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g1903 = step( dither39_g1903, temp_output_47_0_g1903 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g1903 = ( temp_output_41_0_g1903 * dither39_g1903 );
			#else
				float staticSwitch40_g1903 = temp_output_41_0_g1903;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g1903 = staticSwitch40_g1903;
			#else
				float staticSwitch50_g1903 = temp_output_41_0_g1903;
			#endif
			clip( staticSwitch50_g1903 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Utils/ADS Fallback"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;1229.136;3466.338;1;True;False
Node;AmplifyShaderEditor.VertexColorNode;1259;-1280,-640;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1260;-1280,-448;Float;False;ADS Global Turbulence;17;;1579;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.RegisterLocalVarNode;1261;-1024,-576;Half;False;Packed_Variation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1264;-512,-400;Float;False;Property;_MotionVariation;Grass Motion Variation;30;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1263;-1024,-640;Half;False;Packed_Grass;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1262;-1024,-448;Half;False;Motion_Turbulence;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1265;-512,-320;Float;False;1261;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1274;-512,-480;Float;False;Property;_MotionScale;Grass Motion Scale;29;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1275;-512,-640;Float;False;Property;_MotionAmplitude;Grass Motion Amplitude;27;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1273;-512,-560;Float;False;Property;_MotionSpeed;Grass Motion Speed;28;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1272;-256,-400;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1271;768,-384;Float;False;1263;Packed_Grass;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1270;-512,-224;Float;False;1262;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1269;-512,-128;Float;False;1263;Packed_Grass;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1268;768,-480;Float;False;Property;_MotionScale3;Leaf Flutter Scale;34;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1267;768,-640;Float;False;Property;_MotionAmplitude3;Leaf Flutter Amplitude;32;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1266;768,-560;Float;False;Property;_MotionSpeed3;Leaf Flutter Speed;33;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1277;1152,-640;Float;False;ADS Motion Flutter;-1;;1893;87d8028e5f83178498a65cfa9f0e9ace;1,312,0;5;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;136;FLOAT;0;False;310;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1292;128,-640;Float;False;ADS Motion Generic;2;;1892;81cab27e2a487a645a4ff5eb3c63bd27;6,252,0,278,1,228,1,292,2,330,0,326,0;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1278;1408,-640;Half;False;Motion_Leaf;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1279;384,-640;Half;False;Motion_Grass;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1281;1792,-640;Float;False;1279;Motion_Grass;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1243;-1280,256;Float;False;ADS Global Settings;21;;1894;0fe83146627632b4981f5a0aa1b63801;0;1;171;FLOAT;0;False;3;COLOR;85;COLOR;165;FLOAT;157
Node;AmplifyShaderEditor.PosVertexDataNode;1258;-1280,384;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1280;1792,-544;Float;False;1278;Motion_Leaf;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;18;-1280,-1280;Float;True;Property;_AlbedoTex;Grass Albedo;11;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;409;-592,-1280;Half;False;Property;_Color;Grass Color;10;0;Create;False;0;0;False;0;1,1,1,1;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexToFragmentNode;1244;-768,256;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;655;0,-1280;Half;False;Property;_NormalScale;Grass Normal Scale;12;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1282;1792,-448;Float;False;1262;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1283;2048,-640;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1246;-1024,384;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;607;256,-1280;Float;True;Property;_NormalTex;Grass Normal;13;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;-336,-1280;Half;False;Main_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1284;2240,-640;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;-976,-1280;Half;False;Main_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1247;-384,256;Half;False;Gloabl_Tint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1288;320,-1088;Half;False;Property;_NormalInvertOnBackface;Render Backface;7;1;[Enum];Create;False;2;Mirrored Normals;0;Flipped Normals;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1290;-768,384;Float;False;ADS LODFade Scale;-1;;1899;768eaebf5ab5e9748a01997bf1b9d313;0;1;41;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1235;-1280,-2432;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1251;-1280,-2304;Float;False;1247;Gloabl_Tint;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;-976,-1152;Half;False;Main_AlbedoTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1285;2432,-640;Half;False;Motion_Output;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1248;-384,384;Half;False;Global_Size;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1236;-1280,-2368;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1253;576,-1280;Float;False;ADS Normal Backface;-1;;1900;4f53bc25e6d8da34db70401bcf363a2a;0;2;13;FLOAT3;0,0,0;False;30;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1250;-1280,-1856;Float;False;1248;Global_Size;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-2048;Float;False;616;Main_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1175;-1280,-1920;Float;False;1285;Motion_Output;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;816,-1280;Half;False;Main_NormalTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1237;-896,-2432;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1289;1056,-2944;Float;False;ADS Features Support;-1;;1902;217a332a46517ae4cb8ca16677bdb217;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1115;-1088,-3616;Half;False;Property;_MAINN;[ MAINN ];9;0;Create;True;0;0;True;1;BCategory(Main);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;294;1920,-1200;Half;False;Property;_Smoothness;Grass Smoothness;14;0;Create;False;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-1280,-2944;Half;False;Property;_RenderFaces;Render Faces;6;1;[Enum];Create;True;3;Two Sided;0;Back;1;Front;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;624;-1280,-2176;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;645;1152,-1280;Float;True;Property;_SurfaceTex;Grass Surface;15;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;646;1536,-1280;Half;False;Main_SurfaceTex_R;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;749;1920,-1280;Float;False;744;Main_SurfaceTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1113;-1280,-3616;Half;False;Property;_RENDERINGG;[ RENDERINGG ];5;0;Create;True;0;0;True;1;BCategory(Rendering);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1293;-400,-2944;Float;False;ADS Internal Version;0;;1904;858e1f7f7bf8673449834f9aaa5bae83;0;0;1;FLOAT;5
Node;AmplifyShaderEditor.FunctionNode;1291;-1024,-2048;Float;False;ADS LODFade Dither;-1;;1903;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1241;768,-2944;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;51;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1191;512,-2944;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;50;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1195;32,-2944;Half;False;Property;_Internal_TypeGrass;Internal_TypeGrass;49;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1119;-1280,-3712;Half;False;Property;_ADSSimpleLitGrass;< ADS Simple Lit Grass >;4;0;Create;True;0;0;True;1;BBanner(ADS Simple Lit, Grass);1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1179;-1024,-1920;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1118;-304,-3616;Half;False;Property;_ADVANCEDD;[ ADVANCEDD ];35;0;Create;True;0;0;True;1;BCategory(Advanced);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1240;-160,-2944;Half;False;Property;_Internal_ADS;Internal_ADS;37;1;[HideInInspector];Create;True;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1252;-720,-2432;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1117;-752,-3616;Half;False;Property;_MOTIONGRASS;[ MOTION GRASS ];26;0;Create;True;0;0;True;1;BCategory(Grass Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1257;-1280,-3520;Half;False;Property;_BatchingInfo;!!! BatchingInfo;36;0;Create;True;0;0;True;1;BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;744;1536,-1152;Half;False;Main_SurfaceTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-1120,-2944;Half;False;Property;_Cutoff;Cutout;8;0;Create;False;3;Off;0;Front;1;Back;2;0;True;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1287;-528,-3616;Half;False;Property;_LEAFMOTIONN;[ LEAF MOTIONN ];31;0;Create;True;0;0;True;1;BCategory(Leaf Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;2432,-1280;Half;False;OUT_SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;745;2240,-1280;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1199;-640,-2944;Float;False;Internal Unity Props;38;;1901;b286e6ef621b64a4fb35da1e13fa143f;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1116;-928,-3616;Half;False;Property;_SETTINGS;[ SETTINGS ];16;0;Create;True;0;0;True;1;BCategory(Settings);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1196;272,-2944;Half;False;Property;_Internal_LitSimple;Internal_LitSimple;48;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-512,-2432;Float;False;True;2;Float;ADSShaderGUI;200;0;Lambert;BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Grass;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;True;Off;0;False;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;550;10;False;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;200;Utils/ADS Fallback;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;1198;-640,-3072;Float;False;1917.62;100;Internal Only;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;715;1152,-1408;Float;False;1501.26;100;Surface Input;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1286;-1280,-768;Float;False;3910.489;100;Motion;0;;0.03448272,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;760;-1280,-1408;Float;False;1152.612;100;Main Texture and Color;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1249;-1280,128;Float;False;1090.415;100;Globals;0;;1,0.7686275,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;708;0,-1408;Float;False;1024.6;100;Normal Texture;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1280,-3072;Float;False;417.3682;100;Rendering And Settings;0;;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1112;-1280,-3840;Float;False;1185.27;100;Drawers;0;;1,0.4980392,0,1;0;0
WireConnection;1261;0;1259;4
WireConnection;1263;0;1259;1
WireConnection;1262;0;1260;85
WireConnection;1272;0;1264;0
WireConnection;1272;1;1265;0
WireConnection;1277;220;1267;0
WireConnection;1277;221;1266;0
WireConnection;1277;222;1268;0
WireConnection;1277;136;1271;0
WireConnection;1292;220;1275;0
WireConnection;1292;221;1273;0
WireConnection;1292;222;1274;0
WireConnection;1292;218;1272;0
WireConnection;1292;287;1270;0
WireConnection;1292;136;1269;0
WireConnection;1278;0;1277;0
WireConnection;1279;0;1292;0
WireConnection;1244;0;1243;85
WireConnection;1283;0;1281;0
WireConnection;1283;1;1280;0
WireConnection;1246;0;1243;157
WireConnection;1246;1;1258;0
WireConnection;607;5;655;0
WireConnection;486;0;409;0
WireConnection;1284;0;1283;0
WireConnection;1284;1;1282;0
WireConnection;487;0;18;0
WireConnection;1247;0;1244;0
WireConnection;1290;41;1246;0
WireConnection;616;0;18;4
WireConnection;1285;0;1284;0
WireConnection;1248;0;1290;0
WireConnection;1253;13;607;0
WireConnection;1253;30;1288;0
WireConnection;620;0;1253;0
WireConnection;1237;0;1235;0
WireConnection;1237;1;1236;0
WireConnection;1237;2;1251;0
WireConnection;646;0;645;1
WireConnection;1291;41;791;0
WireConnection;1179;0;1175;0
WireConnection;1179;1;1250;0
WireConnection;1252;0;1237;0
WireConnection;744;0;645;4
WireConnection;660;0;294;0
WireConnection;745;0;749;0
WireConnection;745;1;294;0
WireConnection;0;0;1252;0
WireConnection;0;1;624;0
WireConnection;0;10;1291;0
WireConnection;0;11;1179;0
ASEEND*/
//CHKSM=F24E659F4465B508E04C0F856F1EED38C49CDD8C