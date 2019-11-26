// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Cloth"
{
	Properties
	{
		[HideInInspector]_Internal_Version("Internal_Version", Float) = 232
		[BBanner(ADS Simple Lit, Cloth)]_ADSSimpleLitCloth("< ADS Simple Lit Cloth >", Float) = 1
		[BCategory(Rendering)]_RENDERINGG("[ RENDERINGG ]", Float) = 0
		[Enum(Opaque,0,Cutout,1,Fade,2,Transparent,3)]_RenderType("Render Type", Float) = 0
		[Enum(Two Sided,0,Back,1,Front,2)]_RenderFaces("Render Faces", Float) = 0
		[Enum(Mirrored Normals,0,Flipped Normals,1)]_NormalInvertOnBackface("Render Backface", Float) = 1
		[BInteractive(_RenderType, 1)]_RenderMode("# RenderMode", Float) = 0
		_Cutoff("Cutout", Range( 0 , 1)) = 0.5
		[BCategory(Alpha)]_ALPHAA("[ ALPHAA ]", Float) = 0
		[KeywordEnum(Main,Alpha)] _AlphaFrom("Alpha From", Float) = 1
		[BInteractive(_AlphaFrom, 1)]_AlphaFromm("# AlphaFromm", Float) = 0
		[NoScaleOffset]_AlphaTexture("Alpha Texture", 2D) = "white" {}
		_AlphaUVs("Alpha UVs", Vector) = (1,1,0,0)
		[BCategory(Cloth)]_CLOTHH("[ CLOTHH ]", Float) = 0
		_Color("Cloth Color", Color) = (1,1,1,1)
		[NoScaleOffset]_AlbedoTex("Cloth Albedo", 2D) = "white" {}
		_NormalScale("Cloth Normal Scale", Float) = 1
		[NoScaleOffset]_NormalTex("Cloth Normal", 2D) = "bump" {}
		[Space(10)]_UVZero("Cloth UVs", Vector) = (1,1,0,0)
		[BCategory(Symbol)]_SYMBOLL("[ SYMBOLL ]", Float) = 0
		[Enum(Multiplied,0,Sticker,1)]_SymbolMode("Symbol Mode", Float) = 0
		_SymbolColor("Symbol Color", Color) = (1,1,1,1)
		[NoScaleOffset]_SymbolTexture("Symbol Texture", 2D) = "gray" {}
		_SymbolRotation("Symbol Rotation", Range( 0 , 360)) = 0
		_SymbolUVs("Symbol UVs", Vector) = (1,1,1,0)
		[BCategory(Setting)]_SETTINGSS("[ SETTINGSS ]", Float) = 0
		[HideInInspector]_MotionNoise("Motion Noise", Float) = 1
		_GlobalTurbulence("Global Turbulence", Range( 0 , 1)) = 1
		[BCategory(Cloth Motion)]_CLOTHMOTIONN("[ CLOTH MOTIONN ]", Float) = 0
		[KeywordEnum(World,Local)] _MotionSpace("Cloth Motion Space", Float) = 0
		[BInteractive(_MotionSpace, 1)]_MotionSpacee("# MotionSpacee", Float) = 0
		_MotionLocalDirection("Cloth Local Direction", Vector) = (0,0,0,0)
		[BInteractive(ON)]_MotionSpacee_ON("# MotionSpacee_ON", Float) = 0
		_MotionAmplitude("Cloth Motion Amplitude", Float) = 0
		_MotionSpeed("Cloth Motion Speed", Float) = 0
		_MotionScale("Cloth Motion Scale", Float) = 0
		_MotionVariation("Cloth Motion Variation", Float) = 0
		[BCategory(Detail Motion)]_DETAILMOTIONN("[ DETAIL MOTIONN ]", Float) = 0
		_MotionLocalDirection3("Cloth Detail Direction", Vector) = (1,1,1,0)
		_MotionAmplitude3("Cloth Detail Amplitude", Float) = 0
		_MotionSpeed3("Cloth Detail Speed", Float) = 0
		_MotionScale3("Cloth Detail Scale", Float) = 0
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
		[HideInInspector]_Internal_TypeCloth("Internal_TypeCloth", Float) = 1
		[HideInInspector]_Internal_DebugMask("Internal_DebugMask", Float) = 1
		[HideInInspector]_Internal_DebugVariation("Internal_DebugVariation", Float) = 1
		[HideInInspector]_SrcBlend("_SrcBlend", Float) = 1
		[HideInInspector]_DstBlend("_DstBlend", Float) = 10
		[HideInInspector]_ZWrite("_ZWrite", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "DisableBatching" = "True" }
		LOD 200
		Cull [_RenderFaces]
		ZWrite [_ZWrite]
		Blend [_SrcBlend] [_DstBlend]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature _MOTIONSPACE_WORLD _MOTIONSPACE_LOCAL
		#pragma shader_feature _RENDERTYPEKEY_OPAQUE _RENDERTYPEKEY_CUT _RENDERTYPEKEY_FADE _RENDERTYPEKEY_TRANSPARENT
		#pragma shader_feature _ALPHAFROM_MAIN _ALPHAFROM_ALPHA
		 
		//ADS Features
		//ADS End
		  
		#pragma exclude_renderers gles 
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
			float4 screenPosition;
		};

		uniform float _MotionNoise;
		uniform half _ADVANCEDD;
		uniform half _RenderFaces;
		uniform half4 _MainUVs;
		uniform float _Mode;
		uniform float _Glossiness;
		uniform half _CullMode;
		uniform float _BumpScale;
		uniform sampler2D _MetallicGlossMap;
		uniform half _Internal_UnityToBoxophobic;
		uniform sampler2D _MainTex;
		uniform sampler2D _BumpMap;
		uniform half _ALPHAA;
		uniform half _ADSSimpleLitCloth;
		uniform half _CLOTHMOTIONN;
		uniform half _Cutoff;
		uniform half _Internal_ADS;
		uniform half _RenderType;
		uniform half _SETTINGSS;
		uniform half _ZWrite;
		uniform half _Internal_LitSimple;
		uniform half _MotionSpacee_ON;
		uniform half _CLOTHH;
		uniform half _RENDERINGG;
		uniform half _DstBlend;
		uniform half _Internal_DebugVariation;
		uniform half _RenderMode;
		uniform half _MotionSpacee;
		uniform half _Internal_TypeCloth;
		uniform half _Internal_DebugMask;
		uniform half _DETAILMOTIONN;
		uniform half _AlphaFromm;
		uniform half _SrcBlend;
		uniform half _SYMBOLL;
		uniform half _BatchingInfo;
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
		uniform half3 _MotionLocalDirection3;
		uniform float _MotionScale3;
		uniform float _MotionSpeed3;
		uniform float _MotionAmplitude3;
		uniform half _NormalScale;
		uniform sampler2D _NormalTex;
		uniform half4 _UVZero;
		uniform half _NormalInvertOnBackface;
		uniform half4 _Color;
		uniform sampler2D _AlbedoTex;
		uniform half4 _SymbolColor;
		uniform sampler2D _SymbolTexture;
		uniform half4 _SymbolUVs;
		uniform half _SymbolRotation;
		uniform half _SymbolMode;
		uniform sampler2D _AlphaTexture;
		uniform half4 _AlphaUVs;


		inline half2 RotateUV453( half2 UV , half Angle )
		{
			return mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 );;
		}


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
				float3 staticSwitch345_g1913 = ase_worldPos;
			#elif defined(_MOTIONSPACE_LOCAL)
				float3 staticSwitch345_g1913 = ase_vertex3Pos;
			#else
				float3 staticSwitch345_g1913 = ase_worldPos;
			#endif
			half MotionScale60_g1913 = ( ADS_GlobalScale * _MotionScale );
			half MotionSpeed62_g1913 = ( ADS_GlobalSpeed * _MotionSpeed );
			float mulTime90_g1913 = _Time.y * MotionSpeed62_g1913;
			float3 temp_output_95_0_g1913 = ( ( staticSwitch345_g1913 * MotionScale60_g1913 ) + mulTime90_g1913 );
			half Packed_Variation1149 = v.color.a;
			half MotionVariation269_g1913 = ( _MotionVariation * Packed_Variation1149 );
			half MotionlAmplitude58_g1913 = ( ADS_GlobalAmplitude * _MotionAmplitude );
			float3 temp_output_92_0_g1913 = ( sin( ( temp_output_95_0_g1913 + MotionVariation269_g1913 ) ) * MotionlAmplitude58_g1913 );
			float3 temp_output_160_0_g1913 = ( temp_output_92_0_g1913 + MotionlAmplitude58_g1913 + MotionlAmplitude58_g1913 );
			float2 temp_cast_0 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g1881 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1881 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1881 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1881 = (float3(localunity_ObjectToWorld0w1_g1881 , localunity_ObjectToWorld1w2_g1881 , localunity_ObjectToWorld2w3_g1881));
			float2 panner73_g1879 = ( _Time.y * temp_cast_0 + ( (appendResult6_g1881).xz * ADS_TurbulenceScale ));
			float lerpResult136_g1879 = lerp( 1.0 , saturate( pow( abs( tex2Dlod( ADS_TurbulenceTex, float4( panner73_g1879, 0, 0.0) ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			half Motion_Turbulence1167 = lerpResult136_g1879;
			float3 lerpResult293_g1913 = lerp( temp_output_92_0_g1913 , temp_output_160_0_g1913 , Motion_Turbulence1167);
			half3 GlobalDirection349_g1913 = ADS_GlobalDirection;
			#if defined(_MOTIONSPACE_WORLD)
				float3 staticSwitch343_g1913 = mul( unity_WorldToObject, float4( GlobalDirection349_g1913 , 0.0 ) ).xyz;
			#elif defined(_MOTIONSPACE_LOCAL)
				float3 staticSwitch343_g1913 = _MotionLocalDirection;
			#else
				float3 staticSwitch343_g1913 = mul( unity_WorldToObject, float4( GlobalDirection349_g1913 , 0.0 ) ).xyz;
			#endif
			half3 MotionDirection59_g1913 = staticSwitch343_g1913;
			half Packed_Cloth1150 = v.color.r;
			half MotionMask137_g1913 = Packed_Cloth1150;
			float3 temp_output_94_0_g1913 = ( ( lerpResult293_g1913 * MotionDirection59_g1913 ) * MotionMask137_g1913 );
			half3 Motion_Cloth1159 = temp_output_94_0_g1913;
			float3 break311_g1912 = ( ase_vertex3Pos * _MotionLocalDirection3 );
			half MotionFlutterScale60_g1912 = ( ADS_GlobalScale * _MotionScale3 );
			half MotionFlutterSpeed62_g1912 = ( ADS_GlobalSpeed * _MotionSpeed3 );
			float mulTime303_g1912 = _Time.y * MotionFlutterSpeed62_g1912;
			half MotionlFlutterAmplitude58_g1912 = ( ADS_GlobalAmplitude * _MotionAmplitude3 );
			half MotionMask137_g1912 = Packed_Cloth1150;
			float3 ase_vertexNormal = v.normal.xyz;
			half3 Motion_Flutter1183 = ( sin( ( ( ( break311_g1912.x + break311_g1912.y + break311_g1912.z ) * MotionFlutterScale60_g1912 ) + mulTime303_g1912 ) ) * MotionlFlutterAmplitude58_g1912 * MotionMask137_g1912 * ase_vertexNormal );
			half3 Motion_Output1163 = ( ( Motion_Cloth1159 + Motion_Flutter1183 ) * Motion_Turbulence1167 );
			v.vertex.xyz += Motion_Output1163;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult564 = (float2(_UVZero.x , _UVZero.y));
			float2 appendResult565 = (float2(_UVZero.z , _UVZero.w));
			half2 Main_UVs587 = ( ( i.uv_texcoord * appendResult564 ) + appendResult565 );
			float3 temp_output_13_0_g1914 = UnpackScaleNormal( tex2D( _NormalTex, Main_UVs587 ), _NormalScale );
			float3 break17_g1914 = temp_output_13_0_g1914;
			float switchResult12_g1914 = (((i.ASEVFace>0)?(break17_g1914.z):(-break17_g1914.z)));
			float3 appendResult18_g1914 = (float3(break17_g1914.x , break17_g1914.y , switchResult12_g1914));
			float3 lerpResult20_g1914 = lerp( temp_output_13_0_g1914 , appendResult18_g1914 , _NormalInvertOnBackface);
			half3 Main_NormalTex620 = lerpResult20_g1914;
			o.Normal = Main_NormalTex620;
			half4 Main_Color486 = _Color;
			float4 tex2DNode18 = tex2D( _AlbedoTex, Main_UVs587 );
			half4 Main_AlbedoTex487 = tex2DNode18;
			float4 temp_output_518_0 = ( Main_Color486 * Main_AlbedoTex487 );
			half4 SymbolColor492 = _SymbolColor;
			float2 temp_cast_0 = (0.5).xx;
			float2 appendResult870 = (float2(_SymbolUVs.x , _SymbolUVs.y));
			float2 appendResult579 = (float2(_SymbolUVs.z , _SymbolUVs.w));
			half2 UV453 = ( ( ( ( i.uv_texcoord - temp_cast_0 ) * appendResult870 ) + 0.5 ) + appendResult579 );
			half Angle453 = radians( _SymbolRotation );
			half2 localRotateUV453 = RotateUV453( UV453 , Angle453 );
			half2 SymbolUVs488 = localRotateUV453;
			float4 tex2DNode401 = tex2D( _SymbolTexture, SymbolUVs488 );
			half4 SymbolTex490 = tex2DNode401;
			half SymbolTexAlpha963 = tex2DNode401.a;
			float4 lerpResult967 = lerp( temp_output_518_0 , ( SymbolColor492 * SymbolTex490 * saturate( ( Main_AlbedoTex487 + _SymbolMode ) ) ) , SymbolTexAlpha963);
			float4 switchResult478 = (((i.ASEVFace>0)?(lerpResult967):(temp_output_518_0)));
			#if defined(_RENDERTYPEKEY_OPAQUE)
				float4 staticSwitch1120 = switchResult478;
			#elif defined(_RENDERTYPEKEY_CUT)
				float4 staticSwitch1120 = switchResult478;
			#elif defined(_RENDERTYPEKEY_FADE)
				float4 staticSwitch1120 = lerpResult967;
			#elif defined(_RENDERTYPEKEY_TRANSPARENT)
				float4 staticSwitch1120 = lerpResult967;
			#else
				float4 staticSwitch1120 = switchResult478;
			#endif
			half4 OUT_ALBEDO416 = staticSwitch1120;
			half Main_Color_A1057 = _Color.a;
			float2 appendResult598 = (float2(_AlphaUVs.x , _AlphaUVs.y));
			float2 appendResult601 = (float2(_AlphaUVs.z , _AlphaUVs.w));
			half2 AlphaUV603 = ( ( i.uv_texcoord * appendResult598 ) + appendResult601 );
			half AlphaTextureRed595 = tex2D( _AlphaTexture, AlphaUV603 ).r;
			half Main_AlbedoTex_A616 = tex2DNode18.a;
			#if defined(_ALPHAFROM_MAIN)
				float staticSwitch615 = Main_AlbedoTex_A616;
			#elif defined(_ALPHAFROM_ALPHA)
				float staticSwitch615 = AlphaTextureRed595;
			#else
				float staticSwitch615 = AlphaTextureRed595;
			#endif
			half Out_OPACITY407 = staticSwitch615;
			#if defined(_RENDERTYPEKEY_OPAQUE)
				float4 staticSwitch1124 = OUT_ALBEDO416;
			#elif defined(_RENDERTYPEKEY_CUT)
				float4 staticSwitch1124 = OUT_ALBEDO416;
			#elif defined(_RENDERTYPEKEY_FADE)
				float4 staticSwitch1124 = OUT_ALBEDO416;
			#elif defined(_RENDERTYPEKEY_TRANSPARENT)
				float4 staticSwitch1124 = ( OUT_ALBEDO416 * Main_Color_A1057 * Out_OPACITY407 );
			#else
				float4 staticSwitch1124 = OUT_ALBEDO416;
			#endif
			o.Albedo = staticSwitch1124.rgb;
			float temp_output_1141_0 = 1.0;
			float temp_output_1058_0 = ( Main_Color_A1057 * Out_OPACITY407 );
			#if defined(_RENDERTYPEKEY_OPAQUE)
				float staticSwitch1117 = temp_output_1141_0;
			#elif defined(_RENDERTYPEKEY_CUT)
				float staticSwitch1117 = temp_output_1141_0;
			#elif defined(_RENDERTYPEKEY_FADE)
				float staticSwitch1117 = temp_output_1058_0;
			#elif defined(_RENDERTYPEKEY_TRANSPARENT)
				float staticSwitch1117 = temp_output_1058_0;
			#else
				float staticSwitch1117 = temp_output_1141_0;
			#endif
			float temp_output_41_0_g1918 = staticSwitch1117;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen39_g1918 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g1918 = Dither4x4Bayer( fmod(clipScreen39_g1918.x, 4), fmod(clipScreen39_g1918.y, 4) );
			float temp_output_47_0_g1918 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g1918 = step( dither39_g1918, temp_output_47_0_g1918 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g1918 = ( temp_output_41_0_g1918 * dither39_g1918 );
			#else
				float staticSwitch40_g1918 = temp_output_41_0_g1918;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g1918 = staticSwitch40_g1918;
			#else
				float staticSwitch50_g1918 = temp_output_41_0_g1918;
			#endif
			o.Alpha = staticSwitch50_g1918;
			#if defined(_RENDERTYPEKEY_OPAQUE)
				float staticSwitch1119 = temp_output_1141_0;
			#elif defined(_RENDERTYPEKEY_CUT)
				float staticSwitch1119 = Out_OPACITY407;
			#elif defined(_RENDERTYPEKEY_FADE)
				float staticSwitch1119 = temp_output_1141_0;
			#elif defined(_RENDERTYPEKEY_TRANSPARENT)
				float staticSwitch1119 = temp_output_1141_0;
			#else
				float staticSwitch1119 = temp_output_1141_0;
			#endif
			float temp_output_41_0_g1917 = staticSwitch1119;
			float2 clipScreen39_g1917 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither39_g1917 = Dither4x4Bayer( fmod(clipScreen39_g1917.x, 4), fmod(clipScreen39_g1917.y, 4) );
			float temp_output_47_0_g1917 = max( unity_LODFade.x , step( unity_LODFade.x , 0.0 ) );
			dither39_g1917 = step( dither39_g1917, temp_output_47_0_g1917 );
			#ifdef LOD_FADE_CROSSFADE
				float staticSwitch40_g1917 = ( temp_output_41_0_g1917 * dither39_g1917 );
			#else
				float staticSwitch40_g1917 = temp_output_41_0_g1917;
			#endif
			#ifdef ADS_LODFADE_DITHER
				float staticSwitch50_g1917 = staticSwitch40_g1917;
			#else
				float staticSwitch50_g1917 = temp_output_41_0_g1917;
			#endif
			clip( staticSwitch50_g1917 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Utils/ADS Fallback"
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;613.868;3942.213;1;True;False
Node;AmplifyShaderEditor.FunctionNode;1142;-1280,-256;Float;False;const;-1;;1482;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,3;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;563;-1280,-928;Half;False;Property;_UVZero;Cloth UVs;19;0;Create;False;0;0;False;1;Space(10);1,1,0,0;1.4,1.4,0.38,0.38;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;586;-1280,-160;Half;False;Property;_SymbolUVs;Symbol UVs;25;0;Create;True;0;0;False;0;1,1,1,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;578;-1280,-384;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;564;-1024,-928;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-1280,-1152;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;580;-1024,-384;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;870;-1024,-160;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;565;-1024,-848;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;1143;-832,-256;Float;False;const;-1;;1483;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-832,-1152;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;581;-832,-384;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-624,-1152;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;873;-832,-64;Half;False;Property;_SymbolRotation;Symbol Rotation;24;0;Create;True;0;0;False;0;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;579;-1024,-80;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;583;-640,-384;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;457;-512,-64;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;587;-448,-1152;Half;False;Main_UVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;872;-448,-384;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;597;-1280,-1696;Half;False;Property;_AlphaUVs;Alpha UVs;13;0;Create;True;0;0;False;0;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;453;-336,-80;Half;False;mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 )@;2;False;2;True;UV;FLOAT2;0,0;In;;Float;False;True;Angle;FLOAT;0;In;;Float;False;Rotate UV;True;False;0;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;588;-128,-1152;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;598;-1024,-1696;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;599;-1280,-1920;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;18;80,-1152;Float;True;Property;_AlbedoTex;Cloth Albedo;16;1;[NoScaleOffset];Create;False;0;0;False;0;None;72ad54e421175b6478ee511a3f554151;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;488;-192,-384;Half;False;SymbolUVs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;600;-896,-1920;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;601;-1024,-1616;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;520;128,-384;Float;False;488;SymbolUVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;384,-1152;Half;False;Main_AlbedoTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;602;-704,-1920;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;411;1024,-384;Half;False;Property;_SymbolColor;Symbol Color;22;0;Create;True;0;0;False;0;1,1,1,1;0.6911764,0.1880406,0.1880406,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;401;320,-384;Float;True;Property;_SymbolTexture;Symbol Texture;23;1;[NoScaleOffset];Create;True;0;0;False;0;None;9f30c233026087244afe349779444873;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;970;1664,-176;Half;False;Property;_SymbolMode;Symbol Mode;21;1;[Enum];Create;True;2;Multiplied;0;Sticker;1;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;515;1664,-256;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;409;768,-1152;Half;False;Property;_Color;Cloth Color;15;0;Create;False;0;0;False;0;1,1,1,1;0.8897059,0.8685912,0.8308283,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;603;-448,-1920;Half;False;AlphaUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;1147;-1280,384;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;490;640,-384;Half;False;SymbolTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;492;1280,-384;Half;False;SymbolColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;486;1024,-1152;Half;False;Main_Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;593;-128,-1920;Float;False;603;AlphaUV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;975;1904,-224;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1166;-1280,640;Float;False;ADS Global Turbulence;27;;1879;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.RegisterLocalVarNode;1149;-1024,464;Half;False;Packed_Variation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1167;-1024,640;Half;False;Motion_Turbulence;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;964;1664,-320;Float;False;490;SymbolTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;517;1664,-64;Float;False;486;Main_Color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1152;-512,704;Float;False;1149;Packed_Variation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;594;80,-1920;Float;True;Property;_AlphaTexture;Alpha Texture;12;1;[NoScaleOffset];Create;True;0;0;False;0;None;3eff9796fc48a1e4bbf3369bd5608176;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1150;-1024,384;Half;False;Packed_Cloth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;965;1664,-384;Float;False;492;SymbolColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;963;640,-256;Half;False;SymbolTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;976;2032,-224;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;516;1664,0;Float;False;487;Main_AlbedoTex;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1151;-512,624;Float;False;Property;_MotionVariation;Cloth Motion Variation;41;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1155;-512,864;Float;False;1150;Packed_Cloth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;518;1920,-64;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;966;2176,-384;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;595;432,-1920;Half;False;AlphaTextureRed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1168;-512,784;Float;False;1167;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1202;768,736;Half;False;Property;_MotionLocalDirection3;Cloth Detail Direction;43;0;Create;False;0;0;False;0;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;1178;768,384;Float;False;Property;_MotionAmplitude3;Cloth Detail Amplitude;44;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1157;-256,624;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1191;-512,1024;Half;False;Property;_MotionLocalDirection;Cloth Local Direction;36;0;Create;False;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;1181;768,640;Float;False;1150;Packed_Cloth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1153;-512,384;Float;False;Property;_MotionAmplitude;Cloth Motion Amplitude;38;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1179;768,464;Float;False;Property;_MotionSpeed3;Cloth Detail Speed;45;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;384,-1024;Half;False;Main_AlbedoTex_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1156;-512,544;Float;False;Property;_MotionScale;Cloth Motion Scale;40;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1154;-512,464;Float;False;Property;_MotionSpeed;Cloth Motion Speed;39;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1180;768,544;Float;False;Property;_MotionScale3;Cloth Detail Scale;46;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;968;2176,-64;Float;False;963;SymbolTexAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1201;128,384;Float;False;ADS Motion Generic;32;;1913;81cab27e2a487a645a4ff5eb3c63bd27;6,252,2,278,1,228,1,292,2,330,2,326,2;8;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;218;FLOAT;0;False;287;FLOAT;0;False;136;FLOAT;0;False;279;FLOAT;0;False;342;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;619;768,-1792;Float;False;595;AlphaTextureRed;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1200;1088,384;Float;False;ADS Motion Flutter;-1;;1912;87d8028e5f83178498a65cfa9f0e9ace;1,312,1;5;220;FLOAT;0;False;221;FLOAT;0;False;222;FLOAT;0;False;136;FLOAT;0;False;310;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;967;2400,-384;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;618;768,-1920;Float;False;616;Main_AlbedoTex_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;478;2624,-224;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;615;1088,-1920;Float;False;Property;_AlphaFrom;Alpha From;10;0;Create;True;0;0;False;0;0;1;1;True;;KeywordEnum;2;Main;Alpha;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1159;384,384;Half;False;Motion_Cloth;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1183;1344,384;Half;False;Motion_Flutter;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;604;1536,-1152;Float;False;587;Main_UVs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1184;1792,464;Float;False;1183;Motion_Flutter;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;655;1536,-1024;Half;False;Property;_NormalScale;Cloth Normal Scale;17;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;407;1472,-1920;Half;False;Out_OPACITY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1161;1792,384;Float;False;1159;Motion_Cloth;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;1120;2880,-384;Float;False;Property;_RenderTypeKey;RenderTypeKey;5;0;Create;True;0;0;False;0;0;0;0;False;_ALPHABLEND_ON;KeywordEnum;4;Opaque;Cut;Fade;Transparent;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1057;1024,-1056;Half;False;Main_Color_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1171;1856,-960;Half;False;Property;_NormalInvertOnBackface;Render Backface;6;1;[Enum];Create;False;2;Mirrored Normals;0;Flipped Normals;1;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1169;1792,576;Float;False;1167;Motion_Turbulence;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;416;3200,-384;Half;False;OUT_ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-1280,-2560;Float;False;407;Out_OPACITY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;607;1808,-1152;Float;True;Property;_NormalTex;Cloth Normal;18;1;[NoScaleOffset];Create;False;0;0;False;0;None;df6fc4dee84a265438350a8f465fbc0f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;1185;2048,384;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1059;-1280,-2688;Float;False;1057;Main_Color_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1058;-1024,-2688;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1141;-1024,-2816;Float;False;const;-1;;1915;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1077;-1280,-3072;Float;False;416;OUT_ALBEDO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1121;-1280,-2864;Float;False;407;Out_OPACITY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1122;-1280,-2944;Float;False;1057;Main_Color_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1162;2240,384;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1144;2128,-1152;Float;False;ADS Normal Backface;-1;;1914;4f53bc25e6d8da34db70401bcf363a2a;0;2;13;FLOAT3;0,0,0;False;30;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;620;2352,-1152;Half;False;Main_NormalTex;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;1117;-768,-2816;Float;False;Property;_RenderTypeKey;RenderTypeKey;5;0;Create;True;0;0;False;0;0;0;0;False;_ALPHABLEND_ON;KeywordEnum;4;Opaque;Cut;Fade;Transparent;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1119;-768,-2656;Float;False;Property;_RenderTypeKey;RenderTypeKey;5;0;Create;True;0;0;False;0;0;0;0;False;_ALPHABLEND_ON;KeywordEnum;4;Opaque;Cut;Fade;Transparent;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1123;-1024,-2944;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1163;2432,384;Half;False;Motion_Output;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1197;-896,-4160;Half;False;Property;_MotionSpacee;# MotionSpacee;35;0;Create;True;0;0;True;1;BInteractive(_MotionSpace, 1);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1107;-1280,-4160;Half;False;Property;_RenderMode;# RenderMode;7;0;Create;True;0;0;True;1;BInteractive(_RenderType, 1);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1136;656,-3584;Half;False;Property;_Internal_TypeCloth;Internal_TypeCloth;61;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;489;640,-320;Half;False;SymbolTexRed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1096;-1280,-4256;Half;False;Property;_RENDERINGG;[ RENDERINGG ];3;0;Create;True;0;0;True;1;BCategory(Rendering);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1099;-928,-4256;Half;False;Property;_CLOTHH;[ CLOTHH ];14;0;Create;True;0;0;True;1;BCategory(Cloth);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1140;1392,-3584;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;63;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-1120,-3584;Half;False;Property;_DstBlend;_DstBlend;65;1;[HideInInspector];Create;True;0;0;True;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1133;1136,-3584;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;62;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1165;-384,-4160;Half;False;Property;_BatchingInfo;!!! BatchingInfo;48;0;Create;True;0;0;True;1;BMessage(Info, Batching is not currently supported Please use GPU Instancing instead for better performance, 0, 0);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1100;-768,-4256;Half;False;Property;_SYMBOLL;[ SYMBOLL ];20;0;Create;True;0;0;True;1;BCategory(Symbol);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1206;224,-3584;Float;False;ADS Internal Version;0;;1923;858e1f7f7bf8673449834f9aaa5bae83;0;0;1;FLOAT;5
Node;AmplifyShaderEditor.RangedFloatNode;1139;464,-3584;Half;False;Property;_Internal_ADS;Internal_ADS;49;1;[HideInInspector];Create;True;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1172;1664,-3584;Float;False;ADS Features Support;-1;;1919;217a332a46517ae4cb8ca16677bdb217;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1176;-192,-4256;Half;False;Property;_DETAILMOTIONN;[ DETAIL MOTIONN ];42;0;Create;True;0;0;True;1;BCategory(Detail Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-1280,-3584;Half;False;Property;_SrcBlend;_SrcBlend;64;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1108;-1088,-4160;Half;False;Property;_AlphaFromm;# AlphaFromm;11;0;Create;True;0;0;True;1;BInteractive(_AlphaFrom, 1);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1198;-672,-4160;Half;False;Property;_MotionSpacee_ON;# MotionSpacee_ON;37;0;Create;True;0;0;True;1;BInteractive(ON);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1174;-512,-2656;Float;False;ADS LODFade Dither;-1;;1917;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1098;-1088,-4256;Half;False;Property;_ALPHAA;[ ALPHAA ];9;0;Create;True;0;0;True;1;BCategory(Alpha);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1132;-512,-2496;Float;False;1163;Motion_Output;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1097;-1280,-4352;Half;False;Property;_ADSSimpleLitCloth;< ADS Simple Lit Cloth >;2;0;Create;True;0;0;True;1;BBanner(ADS Simple Lit, Cloth);1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1103;48,-4256;Half;False;Property;_ADVANCEDD;[ ADVANCEDD ];47;0;Create;True;0;0;True;1;BCategory(Advanced);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1138;-768,-2912;Float;False;620;Main_NormalTex;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1135;0,-3584;Float;False;Internal Unity Props;50;;1916;b286e6ef621b64a4fb35da1e13fa143f;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-640,-3584;Half;False;Property;_RenderFaces;Render Faces;5;1;[Enum];Create;True;3;Two Sided;0;Back;1;Front;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1102;-416,-4256;Half;False;Property;_CLOTHMOTIONN;[ CLOTH MOTIONN ];31;0;Create;True;0;0;True;1;BCategory(Cloth Motion);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;925;-960,-3584;Half;False;Property;_ZWrite;_ZWrite;66;1;[HideInInspector];Create;True;2;Off;0;On;1;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1173;-512,-2816;Float;False;ADS LODFade Dither;-1;;1918;f1eaf6a5452c7c7458970a3fc3fa22c1;1,44,0;1;41;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1190;-512,1168;Float;False;Property;_MotionSpace;Cloth Motion Space;34;0;Create;False;0;0;True;0;0;0;0;True;;KeywordEnum;2;World;Local;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1134;896,-3584;Half;False;Property;_Internal_LitSimple;Internal_LitSimple;60;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-816,-3584;Half;False;Property;_RenderType;Render Type;4;1;[Enum];Create;True;4;Opaque;0;Cutout;1;Fade;2;Transparent;3;0;True;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-448,-3584;Half;False;Property;_Cutoff;Cutout;8;0;Create;False;3;Off;0;Front;1;Back;2;0;True;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1101;-592,-4256;Half;False;Property;_SETTINGSS;[ SETTINGSS ];26;0;Create;True;0;0;True;1;BCategory(Setting);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1124;-768,-3072;Float;False;Property;_RenderTypeKey;RenderTypeKey;5;0;Create;True;0;0;False;0;0;0;0;False;_ALPHABLEND_ON;KeywordEnum;4;Opaque;Cut;Fade;Transparent;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-128,-3072;Float;False;True;2;Float;ADSShaderGUI;200;0;Lambert;BOXOPHOBIC/Advanced Dynamic Shaders/Simple Lit/Cloth;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;False;True;Off;0;True;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;Geometry;All;True;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;1;5;True;550;10;True;553;0;1;False;550;10;False;553;0;True;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;200;Utils/ADS Fallback;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;675;128,-512;Float;False;1346.432;100;Symbol Texture and Color;0;;1,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;712;-1280,-1280;Float;False;1026.447;100;Main UVs;0;;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;683;-1280,-3712;Float;False;1087.281;100;Rendering;0;;1,0,0.503,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;708;1536,-1280;Float;False;1020.609;100;Normal Texture;0;;0.5019608,0.5019608,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;710;-1280,-2048;Float;False;1021;100;Alpha UVs;0;;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;723;768,-2048;Float;False;895.356;100;Alpha Mode;0;;0.5,0.5,0.5,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1137;0,-3712;Float;False;1889.217;100;Internal Only;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;711;-128,-2048;Float;False;767;100;Alpha Texture;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;760;-128,-1280;Float;False;1359.504;100;Main Texture and Color;0;;0,0.751724,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1106;-1280,-4480;Float;False;1283.359;101;Drawers;0;;1,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;962;-1280,-512;Float;False;1292.263;100;Symbol UVs;0;;1,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;728;1664,-512;Float;False;1728.608;100;Symbol Layer combined with Main Layer;0;;1,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1164;-1280,256;Float;False;4268.694;100;Motion;0;;0.03448272,1,0,1;0;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;580;0;578;0
WireConnection;580;1;1142;0
WireConnection;870;0;586;1
WireConnection;870;1;586;2
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;581;0;580;0
WireConnection;581;1;870;0
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;579;0;586;3
WireConnection;579;1;586;4
WireConnection;583;0;581;0
WireConnection;583;1;1143;0
WireConnection;457;0;873;0
WireConnection;587;0;575;0
WireConnection;872;0;583;0
WireConnection;872;1;579;0
WireConnection;453;0;872;0
WireConnection;453;1;457;0
WireConnection;598;0;597;1
WireConnection;598;1;597;2
WireConnection;18;1;588;0
WireConnection;488;0;453;0
WireConnection;600;0;599;0
WireConnection;600;1;598;0
WireConnection;601;0;597;3
WireConnection;601;1;597;4
WireConnection;487;0;18;0
WireConnection;602;0;600;0
WireConnection;602;1;601;0
WireConnection;401;1;520;0
WireConnection;603;0;602;0
WireConnection;490;0;401;0
WireConnection;492;0;411;0
WireConnection;486;0;409;0
WireConnection;975;0;515;0
WireConnection;975;1;970;0
WireConnection;1149;0;1147;4
WireConnection;1167;0;1166;85
WireConnection;594;1;593;0
WireConnection;1150;0;1147;1
WireConnection;963;0;401;4
WireConnection;976;0;975;0
WireConnection;518;0;517;0
WireConnection;518;1;516;0
WireConnection;966;0;965;0
WireConnection;966;1;964;0
WireConnection;966;2;976;0
WireConnection;595;0;594;1
WireConnection;1157;0;1151;0
WireConnection;1157;1;1152;0
WireConnection;616;0;18;4
WireConnection;1201;220;1153;0
WireConnection;1201;221;1154;0
WireConnection;1201;222;1156;0
WireConnection;1201;218;1157;0
WireConnection;1201;287;1168;0
WireConnection;1201;136;1155;0
WireConnection;1201;342;1191;0
WireConnection;1200;220;1178;0
WireConnection;1200;221;1179;0
WireConnection;1200;222;1180;0
WireConnection;1200;136;1181;0
WireConnection;1200;310;1202;0
WireConnection;967;0;518;0
WireConnection;967;1;966;0
WireConnection;967;2;968;0
WireConnection;478;0;967;0
WireConnection;478;1;518;0
WireConnection;615;1;618;0
WireConnection;615;0;619;0
WireConnection;1159;0;1201;0
WireConnection;1183;0;1200;0
WireConnection;407;0;615;0
WireConnection;1120;1;478;0
WireConnection;1120;0;478;0
WireConnection;1120;2;967;0
WireConnection;1120;3;967;0
WireConnection;1057;0;409;4
WireConnection;416;0;1120;0
WireConnection;607;1;604;0
WireConnection;607;5;655;0
WireConnection;1185;0;1161;0
WireConnection;1185;1;1184;0
WireConnection;1058;0;1059;0
WireConnection;1058;1;791;0
WireConnection;1162;0;1185;0
WireConnection;1162;1;1169;0
WireConnection;1144;13;607;0
WireConnection;1144;30;1171;0
WireConnection;620;0;1144;0
WireConnection;1117;1;1141;0
WireConnection;1117;0;1141;0
WireConnection;1117;2;1058;0
WireConnection;1117;3;1058;0
WireConnection;1119;1;1141;0
WireConnection;1119;0;791;0
WireConnection;1119;2;1141;0
WireConnection;1119;3;1141;0
WireConnection;1123;0;1077;0
WireConnection;1123;1;1122;0
WireConnection;1123;2;1121;0
WireConnection;1163;0;1162;0
WireConnection;489;0;401;1
WireConnection;1174;41;1119;0
WireConnection;1173;41;1117;0
WireConnection;1124;1;1077;0
WireConnection;1124;0;1077;0
WireConnection;1124;2;1077;0
WireConnection;1124;3;1123;0
WireConnection;0;0;1124;0
WireConnection;0;1;1138;0
WireConnection;0;9;1173;0
WireConnection;0;10;1174;0
WireConnection;0;11;1132;0
ASEEND*/
//CHKSM=CD2ADBBDB3E171F1630D8796DCCB4C14311AC6EF