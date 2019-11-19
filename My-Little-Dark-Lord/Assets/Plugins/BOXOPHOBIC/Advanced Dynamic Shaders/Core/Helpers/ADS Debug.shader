// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Utils/ADS Debug"
{
	Properties
	{
		[HideInInspector]_Internal_ADS("_Internal_ADS", Float) = 1
		_GlobalTint("Global Tint", Range( 0 , 1)) = 1
		_DetailTint("Detail Tint", Range( 0 , 1)) = 1
		_GlobalSize("Global Size", Range( 0 , 1)) = 1
		[Enum(Opaque,0,Cutout,1,Fade,2)]_Mode("Blend Mode", Float) = 0
		[HideInInspector]_MotionNoise("Motion Noise", Float) = 1
		_GlobalTurbulence("Global Turbulence", Range( 0 , 1)) = 1
		_Debug_Arrow("Debug_Arrow", Float) = 1
		_Internal_Deprecated("_Internal_Deprecated", Float) = 0
		_MaskType("_MaskType", Float) = 0
		ADS_GlobalTintIntensity("Fetch ADS_GlobalTintIntensity", Float) = 1
		_GlobalTint("Fetch _GlobalTint", Float) = 1
		_MotionNoise("Fetch _MotionNoise", Float) = 1
		[HideInInspector]_Internal_TypeTreeLeaf("Internal_TypeTreeLeaf", Float) = 1
		[HideInInspector]_Internal_TypeTreeBark("Internal_TypeTreeBark", Float) = 1
		[HideInInspector]_Internal_TypeGrass("Internal_TypeGrass", Float) = 1
		[HideInInspector]_Internal_TypePlant("Internal_TypePlant", Float) = 1
		[HideInInspector]_Internal_DebugMask3("Internal_DebugMask3", Float) = 1
		[HideInInspector]_Internal_DebugMask2("Internal_DebugMask2", Float) = 1
		[HideInInspector]_Internal_DebugMask("Internal_DebugMask", Float) = 1
		[HideInInspector]_Internal_DebugVariation("Internal_DebugVariation", Float) = 1
		[HideInInspector]_Internal_TypeGeneric("Internal_TypeGeneric", Float) = 0
		[HideInInspector]_Internal_TypeCloth("Internal_TypeCloth", Float) = 0
		[HideInInspector]_Internal_LitSimple("Internal_LitSimple", Float) = 1
		[HideInInspector]_Internal_LitStandard("Internal_LitStandard", Float) = 1
		[HideInInspector]_Internal_LitAdvanced("Internal_LitAdvanced", Float) = 1
		[Enum(Off,0,Front,1,Back,2)]_CullMode("Cull Mode", Float) = 0
		[Space(10)]_Cutoff("Cutout/Fade", Range( 0 , 1)) = 1
		[NoScaleOffset]_AlbedoTex("Main Texture", 2D) = "white" {}
		[HideInInspector]_ZWrite("_ZWrite", Float) = 1
		[HideInInspector]_SrcBlend("_SrcBlend", Float) = 1
		[HideInInspector]_DstBlend("_DstBlend", Float) = 10
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "DisableBatching" = "True" "IsEmissive" = "true"  }
		LOD 300
		Cull [_CullMode]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma exclude_renderers d3d9 gles 
		#pragma surface surf Lambert keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 uv_tex4coord;
			float vertexToFrag1719;
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float _MotionNoise;
		uniform half _SrcBlend;
		uniform half _DstBlend;
		uniform half _Mode;
		uniform half _ZWrite;
		uniform half _Cutoff;
		uniform half _CullMode;
		uniform half ADS_GlobalSizeMin;
		uniform half ADS_GlobalSizeMax;
		uniform sampler2D ADS_GlobalTex;
		uniform half4 ADS_GlobalUVs;
		uniform half _GlobalSize;
		uniform half _Internal_TypeGrass;
		uniform half _Internal_TypePlant;
		uniform half ADS_DebugMode;
		uniform half _Internal_DebugMask;
		uniform half _Internal_TypeTreeBark;
		uniform half _Internal_TypeCloth;
		uniform half _Internal_TypeGeneric;
		uniform half _Internal_TypeTreeLeaf;
		uniform half _Internal_ADS;
		uniform half _Internal_DebugMask2;
		uniform half _Internal_DebugMask3;
		uniform half _Internal_DebugVariation;
		uniform sampler2D ADS_TurbulenceTex;
		uniform half ADS_TurbulenceSpeed;
		uniform half ADS_TurbulenceScale;
		uniform half ADS_TurbulenceContrast;
		uniform float _GlobalTurbulence;
		uniform half4 ADS_GlobalTintColorOne;
		uniform half4 ADS_GlobalTintColorTwo;
		uniform half ADS_GlobalTintIntensity;
		uniform half _GlobalTint;
		uniform half _DetailTint;
		uniform half _Internal_LitAdvanced;
		uniform half _Internal_LitStandard;
		uniform half _Internal_LitSimple;
		uniform float _MaskType;
		uniform float _Internal_Deprecated;
		uniform half ADS_DebugMin;
		uniform half ADS_DebugMax;
		uniform half _Debug_Arrow;
		uniform sampler2D _AlbedoTex;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			half localunity_ObjectToWorld0w1_g1528 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1528 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1528 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1528 = (float3(localunity_ObjectToWorld0w1_g1528 , localunity_ObjectToWorld1w2_g1528 , localunity_ObjectToWorld2w3_g1528));
			float4 tex2DNode140_g1526 = tex2Dlod( ADS_GlobalTex, float4( ( ( (appendResult6_g1528).xz * (ADS_GlobalUVs).xy ) + (ADS_GlobalUVs).zw ), 0, 0.0) );
			half ADS_GlobalTex_B198_g1526 = tex2DNode140_g1526.b;
			float lerpResult156_g1526 = lerp( ADS_GlobalSizeMin , ADS_GlobalSizeMax , ADS_GlobalTex_B198_g1526);
			float temp_output_1618_157 = ( lerpResult156_g1526 * _GlobalSize );
			half GlobalSize1619 = temp_output_1618_157;
			float3 ase_vertex3Pos = v.vertex.xyz;
			half Internal_TypeFoliage1472 = saturate( ( _Internal_TypeGrass + _Internal_TypePlant ) );
			float3 lerpResult1626 = lerp( float3( 0,0,0 ) , ( GlobalSize1619 * ase_vertex3Pos ) , Internal_TypeFoliage1472);
			v.vertex.xyz += lerpResult1626;
			half DebugMode1487 = ADS_DebugMode;
			float ifLocalVar1374 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 101.0 )
				ifLocalVar1374 = v.color.r;
			float ifLocalVar1355 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 102.0 )
				ifLocalVar1355 = v.color.g;
			float ifLocalVar1407 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 103.0 )
				ifLocalVar1407 = v.color.b;
			float ifLocalVar1363 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 104.0 )
				ifLocalVar1363 = v.color.a;
			float ifLocalVar1754 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 105.0 )
				ifLocalVar1754 = ase_vertex3Pos.x;
			float ifLocalVar1760 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 106.0 )
				ifLocalVar1760 = ase_vertex3Pos.y;
			float ifLocalVar1764 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 107.0 )
				ifLocalVar1764 = ase_vertex3Pos.z;
			float ifLocalVar1657 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 111.0 )
				ifLocalVar1657 = v.texcoord.x;
			float ifLocalVar1656 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 112.0 )
				ifLocalVar1656 = v.texcoord.y;
			float ifLocalVar1655 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 113.0 )
				ifLocalVar1655 = v.texcoord.z;
			float ifLocalVar1658 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 114.0 )
				ifLocalVar1658 = v.texcoord.w;
			float ifLocalVar1674 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 121.0 )
				ifLocalVar1674 = v.texcoord1.x;
			float ifLocalVar1673 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 122.0 )
				ifLocalVar1673 = v.texcoord1.y;
			float ifLocalVar1672 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 123.0 )
				ifLocalVar1672 = v.texcoord1.z;
			float ifLocalVar1675 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 124.0 )
				ifLocalVar1675 = v.texcoord1.w;
			float ifLocalVar1691 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 131.0 )
				ifLocalVar1691 = v.texcoord2.x;
			float ifLocalVar1690 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 132.0 )
				ifLocalVar1690 = v.texcoord2.y;
			float ifLocalVar1689 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 133.0 )
				ifLocalVar1689 = v.texcoord2.z;
			float ifLocalVar1692 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 134.0 )
				ifLocalVar1692 = v.texcoord2.w;
			float ifLocalVar1708 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 141.0 )
				ifLocalVar1708 = v.texcoord3.x;
			float ifLocalVar1707 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 142.0 )
				ifLocalVar1707 = v.texcoord3.y;
			float ifLocalVar1706 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 143.0 )
				ifLocalVar1706 = v.texcoord3.z;
			float ifLocalVar1709 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 144.0 )
				ifLocalVar1709 = v.texcoord3.w;
			o.vertexToFrag1719 = ( ( ifLocalVar1374 + ifLocalVar1355 + ifLocalVar1407 + ifLocalVar1363 ) + ( ifLocalVar1754 + ifLocalVar1760 + ifLocalVar1764 ) + ( ifLocalVar1657 + ifLocalVar1656 + ifLocalVar1655 + ifLocalVar1658 ) + ( ifLocalVar1674 + ifLocalVar1673 + ifLocalVar1672 + ifLocalVar1675 ) + ( ifLocalVar1691 + ifLocalVar1690 + ifLocalVar1689 + ifLocalVar1692 ) + ( ifLocalVar1708 + ifLocalVar1707 + ifLocalVar1706 + ifLocalVar1709 ) );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			half DebugMode1487 = ADS_DebugMode;
			float4 temp_cast_0 = (-99.0).xxxx;
			half Internal_DebugMask1474 = _Internal_DebugMask;
			half Internal_TypeFoliage1472 = saturate( ( _Internal_TypeGrass + _Internal_TypePlant ) );
			half Internal_TypeTreeBark1488 = _Internal_TypeTreeBark;
			half Internal_TypeCloth1469 = _Internal_TypeCloth;
			half Internal_TypeGeneric1475 = _Internal_TypeGeneric;
			half Internal_TypeTreeLeaf1463 = _Internal_TypeTreeLeaf;
			half4 color1285 = IsGammaSpace() ? half4(1,0.241,0.1176471,0) : half4(1,0.0473472,0.01298304,0);
			float4 lerpResult2_g1547 = lerp( temp_cast_0 , ( ( ( i.vertexColor.r * Internal_DebugMask1474 * Internal_TypeFoliage1472 ) + ( ( i.vertexColor.r * i.vertexColor.r ) * Internal_DebugMask1474 * Internal_TypeTreeBark1488 ) + ( i.vertexColor.r * Internal_DebugMask1474 * Internal_TypeCloth1469 ) + ( i.vertexColor.r * Internal_DebugMask1474 * Internal_TypeGeneric1475 ) + ( ( i.vertexColor.r * i.vertexColor.r ) * Internal_DebugMask1474 * Internal_TypeTreeLeaf1463 ) ) * color1285 ) , _Internal_ADS);
			float4 ifLocalVar1321 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 11.0 )
				ifLocalVar1321 = lerpResult2_g1547;
			float4 MOTION_MASK_11309 = ifLocalVar1321;
			float4 temp_cast_1 = (-99.0).xxxx;
			half Internal_DebugMask21397 = _Internal_DebugMask2;
			half4 color1400 = IsGammaSpace() ? half4(1,0.241,0.1176471,0) : half4(1,0.0473472,0.01298304,0);
			float4 lerpResult2_g1546 = lerp( temp_cast_1 , ( ( ( Internal_TypeFoliage1472 * i.vertexColor.b * Internal_DebugMask21397 ) + ( i.vertexColor.g * Internal_DebugMask21397 * Internal_TypeTreeBark1488 ) + ( i.vertexColor.g * Internal_DebugMask21397 * Internal_TypeTreeLeaf1463 ) ) * color1400 ) , _Internal_ADS);
			float4 ifLocalVar1293 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 12.0 )
				ifLocalVar1293 = lerpResult2_g1546;
			float4 MOTION_MASK_21364 = ifLocalVar1293;
			float4 temp_cast_2 = (-99.0).xxxx;
			half Internal_DebugMask31462 = _Internal_DebugMask3;
			half4 color1319 = IsGammaSpace() ? half4(1,0.241,0.1176471,0) : half4(1,0.0473472,0.01298304,0);
			float4 lerpResult2_g1543 = lerp( temp_cast_2 , ( ( ( i.vertexColor.b * Internal_DebugMask31462 ) + ( i.vertexColor.b * Internal_DebugMask31462 ) ) * color1319 ) , _Internal_ADS);
			float4 ifLocalVar1324 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 13.0 )
				ifLocalVar1324 = lerpResult2_g1543;
			float4 MOTION_MASK_31449 = ifLocalVar1324;
			float4 temp_cast_3 = (-99.0).xxxx;
			half Internal_DebugVariation1392 = _Internal_DebugVariation;
			float4 lerpResult2_g1544 = lerp( temp_cast_3 , ( ( ( i.vertexColor.a * Internal_DebugVariation1392 * Internal_TypeFoliage1472 ) + ( i.vertexColor.a * Internal_DebugVariation1392 * Internal_TypeTreeBark1488 ) + ( i.vertexColor.a * Internal_DebugVariation1392 * Internal_TypeCloth1469 ) + ( i.vertexColor.a * Internal_DebugVariation1392 * Internal_TypeGeneric1475 ) + ( i.vertexColor.a * Internal_DebugVariation1392 * Internal_TypeTreeLeaf1463 ) ) * half4(0,1,0.6689656,0) ) , _Internal_ADS);
			float4 ifLocalVar1274 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 14.0 )
				ifLocalVar1274 = lerpResult2_g1544;
			float4 MOTION_VARIATION1313 = ifLocalVar1274;
			float4 color1467 = IsGammaSpace() ? float4(0.1397059,0.1397059,0.1397059,0) : float4(0.01732622,0.01732622,0.01732622,0);
			half4 OtherColor1481 = color1467;
			float2 temp_cast_4 = (ADS_TurbulenceSpeed).xx;
			half localunity_ObjectToWorld0w1_g1541 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1541 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1541 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1541 = (float3(localunity_ObjectToWorld0w1_g1541 , localunity_ObjectToWorld1w2_g1541 , localunity_ObjectToWorld2w3_g1541));
			float2 panner73_g1539 = ( _Time.y * temp_cast_4 + ( (appendResult6_g1541).xz * ADS_TurbulenceScale ));
			float lerpResult136_g1539 = lerp( 1.0 , saturate( pow( abs( tex2D( ADS_TurbulenceTex, panner73_g1539 ).r ) , ADS_TurbulenceContrast ) ) , _GlobalTurbulence);
			float temp_output_1583_85 = lerpResult136_g1539;
			float4 lerpResult1427 = lerp( OtherColor1481 , ( half4(0.5055925,0.6176471,0,0) * temp_output_1583_85 ) , _MotionNoise);
			float4 ifLocalVar1482 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 15.0 )
				ifLocalVar1482 = lerpResult1427;
			float4 MOTION_NOISE1426 = ifLocalVar1482;
			half4 color1455 = IsGammaSpace() ? half4(0.1586208,0,1,0) : half4(0.02164405,0,1,0);
			half4 color1335 = IsGammaSpace() ? half4(0.375,0.6637931,1,0) : half4(0.1160161,0.398147,1,0);
			half localunity_ObjectToWorld0w1_g1528 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1528 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1528 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1528 = (float3(localunity_ObjectToWorld0w1_g1528 , localunity_ObjectToWorld1w2_g1528 , localunity_ObjectToWorld2w3_g1528));
			float4 tex2DNode140_g1526 = tex2D( ADS_GlobalTex, ( ( (appendResult6_g1528).xz * (ADS_GlobalUVs).xy ) + (ADS_GlobalUVs).zw ) );
			half ADS_GlobalTex_B198_g1526 = tex2DNode140_g1526.b;
			float lerpResult156_g1526 = lerp( ADS_GlobalSizeMin , ADS_GlobalSizeMax , ADS_GlobalTex_B198_g1526);
			float temp_output_1618_157 = ( lerpResult156_g1526 * _GlobalSize );
			float4 lerpResult1354 = lerp( color1455 , color1335 , saturate( ( temp_output_1618_157 + 1.0 ) ));
			float4 lerpResult1333 = lerp( OtherColor1481 , lerpResult1354 , Internal_TypeFoliage1472);
			float4 ifLocalVar1351 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 32.0 )
				ifLocalVar1351 = lerpResult1333;
			float4 FOLIAGE_SIZE1373 = ifLocalVar1351;
			float4 temp_cast_5 = (1.0).xxxx;
			half4 ADS_GlobalTintColorOne176_g1534 = ADS_GlobalTintColorOne;
			half4 ADS_GlobalTintColorTwo177_g1534 = ADS_GlobalTintColorTwo;
			half localunity_ObjectToWorld0w1_g1536 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld1w2_g1536 = ( unity_ObjectToWorld[1].w );
			half localunity_ObjectToWorld2w3_g1536 = ( unity_ObjectToWorld[2].w );
			float3 appendResult6_g1536 = (float3(localunity_ObjectToWorld0w1_g1536 , localunity_ObjectToWorld1w2_g1536 , localunity_ObjectToWorld2w3_g1536));
			float4 tex2DNode140_g1534 = tex2D( ADS_GlobalTex, ( ( (appendResult6_g1536).xz * (ADS_GlobalUVs).xy ) + (ADS_GlobalUVs).zw ) );
			half ADS_GlobalTex_R180_g1534 = tex2DNode140_g1534.r;
			float4 lerpResult147_g1534 = lerp( ADS_GlobalTintColorOne176_g1534 , ADS_GlobalTintColorTwo177_g1534 , ADS_GlobalTex_R180_g1534);
			half ADS_GlobalTintIntensity181_g1534 = ADS_GlobalTintIntensity;
			half GlobalTint186_g1534 = _GlobalTint;
			float4 lerpResult150_g1534 = lerp( temp_cast_5 , ( lerpResult147_g1534 * ADS_GlobalTintIntensity181_g1534 ) , GlobalTint186_g1534);
			float4 temp_cast_6 = (1.0).xxxx;
			half localunity_ObjectToWorld0w1_g1538 = ( unity_ObjectToWorld[0].w );
			half localunity_ObjectToWorld2w3_g1538 = ( unity_ObjectToWorld[2].w );
			float lerpResult194_g1534 = lerp( frac( ( localunity_ObjectToWorld0w1_g1538 + localunity_ObjectToWorld2w3_g1538 ) ) , i.uv_tex4coord.w , _DetailTint);
			float4 lerpResult166_g1534 = lerp( ADS_GlobalTintColorOne176_g1534 , ADS_GlobalTintColorTwo177_g1534 , lerpResult194_g1534);
			float4 lerpResult168_g1534 = lerp( temp_cast_6 , ( lerpResult166_g1534 * ADS_GlobalTintIntensity181_g1534 ) , GlobalTint186_g1534);
			float4 lerpResult1617 = lerp( lerpResult150_g1534 , lerpResult168_g1534 , Internal_TypeTreeLeaf1463);
			float4 lerpResult1494 = lerp( OtherColor1481 , ( lerpResult1617 / ADS_GlobalTintIntensity ) , _GlobalTint);
			float4 ifLocalVar1490 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 31.0 )
				ifLocalVar1490 = lerpResult1494;
			float4 FOLAIGE_TINT1306 = ifLocalVar1490;
			float4 temp_cast_7 = (-99.0).xxxx;
			half4 color1394 = IsGammaSpace() ? half4(0.415213,0.6764706,0,0) : half4(0.1437809,0.4152089,0,0);
			half Internal_LitAdvanced1473 = _Internal_LitAdvanced;
			half4 color1320 = IsGammaSpace() ? half4(0.9191176,0.566,0,0) : half4(0.8257746,0.2802941,0,0);
			half Internal_LitStandard1466 = _Internal_LitStandard;
			half4 color1357 = IsGammaSpace() ? half4(0,0.5859026,0.7794118,0) : half4(0,0.3023395,0.5695176,0);
			half Internal_LitSimple1493 = _Internal_LitSimple;
			float4 lerpResult2_g1545 = lerp( temp_cast_7 , ( ( color1394 * Internal_LitAdvanced1473 ) + ( color1320 * Internal_LitStandard1466 ) + ( color1357 * Internal_LitSimple1493 ) ) , _Internal_ADS);
			float4 ifLocalVar1411 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 61.0 )
				ifLocalVar1411 = lerpResult2_g1545;
			float4 SHADER_COMPLEXITY1284 = ifLocalVar1411;
			half4 color1514 = IsGammaSpace() ? half4(0.5,0.5,0.5,0) : half4(0.2140411,0.2140411,0.2140411,0);
			half4 color1515 = IsGammaSpace() ? half4(0.5459431,0.1102941,1,0) : half4(0.2590563,0.01169531,1,0);
			#ifdef INSTANCING_ON
				float4 staticSwitch1523 = color1515;
			#else
				float4 staticSwitch1523 = color1514;
			#endif
			float4 ifLocalVar1519 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 62.0 )
				ifLocalVar1519 = staticSwitch1523;
			float4 SHADER_INSTANCING1520 = ifLocalVar1519;
			float4 temp_cast_8 = (-99.0).xxxx;
			half4 color1556 = IsGammaSpace() ? half4(1,0.709,0,0) : half4(1,0.4609121,0,0);
			half4 color1603 = IsGammaSpace() ? half4(1,0,0,0) : half4(1,0,0,0);
			float mulTime1599 = _Time.y * 3.0;
			float4 lerpResult2_g1542 = lerp( temp_cast_8 , ( saturate( ( ( _MaskType * color1556 ) + ( _Internal_Deprecated * color1603 ) ) ) * (0.25 + (sin( mulTime1599 ) - -1.0) * (1.0 - 0.25) / (1.0 - -1.0)) ) , _Internal_ADS);
			float4 ifLocalVar1563 = 0;
			UNITY_BRANCH 
			if( DebugMode1487 == 64.0 )
				ifLocalVar1563 = lerpResult2_g1542;
			float4 SHADER_ISSUES1564 = saturate( ifLocalVar1563 );
			float MESH_CHANNEL1368 = i.vertexToFrag1719;
			float4 temp_output_1290_0 = ( ( MOTION_MASK_11309 + MOTION_MASK_21364 + MOTION_MASK_31449 + MOTION_VARIATION1313 + MOTION_NOISE1426 ) + ( FOLIAGE_SIZE1373 + FOLAIGE_TINT1306 ) + ( SHADER_COMPLEXITY1284 + SHADER_INSTANCING1520 + SHADER_ISSUES1564 + MESH_CHANNEL1368 ) );
			half DebugMin1721 = ADS_DebugMin;
			float temp_output_7_0_g1567 = DebugMin1721;
			float4 temp_cast_9 = (temp_output_7_0_g1567).xxxx;
			half DebugMax1723 = ADS_DebugMax;
			float4 temp_cast_10 = (-98.0).xxxx;
			float4 lerpResult1738 = lerp( saturate( ( ( temp_output_1290_0 - temp_cast_9 ) / ( DebugMax1723 - temp_output_7_0_g1567 ) ) ) , OtherColor1481 , step( temp_output_1290_0 , temp_cast_10 ));
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV1442 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1442 = ( 0.0 + 1.5 * pow( 1.0 - fresnelNdotV1442, 2.0 ) );
			float4 lerpResult1437 = lerp( half4(1,0.5019608,0,0) , half4(1,0.809,0,0) , saturate( fresnelNode1442 ));
			half4 ArrowColor1438 = lerpResult1437;
			half ArrowDebug1450 = _Debug_Arrow;
			float4 lerpResult1278 = lerp( lerpResult1738 , ArrowColor1438 , ArrowDebug1450);
			o.Albedo = ( lerpResult1278 * 0.1 ).rgb;
			o.Emission = ( lerpResult1278 * ( 1.0 - 0.1 ) ).rgb;
			o.Alpha = 1;
			float2 uv_AlbedoTex18 = i.uv_texcoord;
			float4 tex2DNode18 = tex2D( _AlbedoTex, uv_AlbedoTex18 );
			half MainTexAlpha616 = tex2DNode18.a;
			float lerpResult1508 = lerp( MainTexAlpha616 , 1.0 , Internal_TypeTreeBark1488);
			clip( lerpResult1508 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ADSShaderGUI"
}
/*ASEBEGIN
Version=16800
1927;29;1906;1014;4096.156;4222.315;3.237401;True;False
Node;AmplifyShaderEditor.RangedFloatNode;1590;-10112,-4352;Half;False;Property;_Internal_TypeGrass;Internal_TypeGrass;25;1;[HideInInspector];Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1468;-10112,-4272;Half;False;Property;_Internal_TypePlant;Internal_TypePlant;26;1;[HideInInspector];Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1591;-9888,-4352;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1567;-4096,-1792;Float;False;Property;_Internal_Deprecated;_Internal_Deprecated;18;0;Create;True;0;0;False;0;0;63;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1465;-8960,-4608;Half;False;Property;_Internal_TypeGeneric;Internal_TypeGeneric;31;1;[HideInInspector];Create;True;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1478;-10112,-4496;Half;False;Property;_Internal_DebugMask;Internal_DebugMask;29;1;[HideInInspector];Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1556;-4096,-2096;Half;False;Constant;_Color24;Color 24;3;0;Create;True;0;0;False;0;1,0.709,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1565;-4096,-2176;Float;False;Property;_MaskType;_MaskType;19;0;Create;True;0;0;False;0;0;63;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1603;-4096,-1712;Half;False;Constant;_Color23;Color 23;3;0;Create;True;0;0;False;0;1,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1393;-8384,-4496;Half;False;Property;_Internal_DebugVariation;Internal_DebugVariation;30;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1486;-8384,-4608;Half;False;Property;_Internal_TypeCloth;Internal_TypeCloth;32;1;[HideInInspector];Create;True;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1396;-10112,-4608;Half;False;Property;_Internal_TypeTreeBark;Internal_TypeTreeBark;24;1;[HideInInspector];Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1433;-9536,-4608;Half;False;Property;_Internal_TypeTreeLeaf;Internal_TypeTreeLeaf;23;1;[HideInInspector];Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1395;-8960,-4496;Half;False;Property;_Internal_DebugMask3;Internal_DebugMask3;27;1;[HideInInspector];Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1592;-9760,-4352;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1387;-9536,-4496;Half;False;Property;_Internal_DebugMask2;Internal_DebugMask2;28;1;[HideInInspector];Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1469;-8144,-4608;Half;False;Internal_TypeCloth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1480;-8960,-5888;Half;False;Global;ADS_DebugMode;ADS_DebugMode;3;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1472;-9600,-4352;Half;False;Internal_TypeFoliage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;1599;-4096,-1536;Float;False;1;0;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1462;-8704,-4496;Half;False;Internal_DebugMask3;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1484;-7808,-4608;Half;False;Property;_Internal_LitAdvanced;Internal_LitAdvanced;35;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1470;-7232,-4608;Half;False;Property;_Internal_LitStandard;Internal_LitStandard;34;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1558;-3840,-2176;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1483;-6656,-4608;Half;False;Property;_Internal_LitSimple;Internal_LitSimple;33;1;[HideInInspector];Create;True;0;0;True;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1604;-3840,-1792;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1488;-9856,-4608;Half;False;Internal_TypeTreeBark;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1474;-9856,-4496;Half;False;Internal_DebugMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1463;-9280,-4608;Half;False;Internal_TypeTreeLeaf;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1476;-10112,-2688;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;1371;-10112,-1664;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1475;-8704,-4608;Half;False;Internal_TypeGeneric;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1392;-8096,-4496;Half;False;Internal_DebugVariation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1397;-9280,-4496;Half;False;Internal_DebugMask2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1429;-9856,-2672;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1632;-6272,-1584;Float;False;CONST;-1;;1531;5b64729fb717c5f49a1bc2dab81d5e1c;1,3,1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1369;-10112,-640;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;1597;-3920,-1536;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1399;-10112,-1040;Float;False;1392;Internal_DebugVariation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1372;-10112,-384;Float;False;1392;Internal_DebugVariation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1616;-6528,-2608;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1404;-10112,-960;Float;False;1463;Internal_TypeTreeLeaf;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1493;-6416,-4608;Half;False;Internal_LitSimple;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1473;-7552,-4608;Half;False;Internal_LitAdvanced;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1466;-6992,-4608;Half;False;Internal_LitStandard;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1323;-10112,-3536;Float;False;1474;Internal_DebugMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1441;-10112,256;Float;False;1392;Internal_DebugVariation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1605;-3712,-2176;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1417;-10112,-1408;Float;False;1397;Internal_DebugMask2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1415;-10112,-3712;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1425;-10112,-3008;Float;False;1472;Internal_TypeFoliage;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1383;-10112,-2512;Float;False;1474;Internal_DebugMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1382;-10112,0;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1618;-6272,-1728;Float;False;ADS Global Settings;2;;1526;0fe83146627632b4981f5a0aa1b63801;0;1;171;FLOAT;0;False;3;COLOR;85;COLOR;165;FLOAT;157
Node;AmplifyShaderEditor.GetLocalVarNode;1401;-10112,176;Float;False;1474;Internal_DebugMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1296;-10112,-3120;Float;False;1392;Internal_DebugVariation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1485;-10112,-2064;Float;False;1392;Internal_DebugVariation;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1408;-10112,-2352;Float;False;1462;Internal_DebugMask3;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1422;-10112,-1984;Float;False;1488;Internal_TypeTreeBark;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1358;-10112,-464;Float;False;1474;Internal_DebugMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1435;-10112,-288;Float;False;1469;Internal_TypeCloth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1446;-9856,-1648;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1487;-8736,-5888;Half;False;DebugMode;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1409;-10112,-2432;Float;False;1397;Internal_DebugMask2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1375;-10112,-3200;Float;False;1397;Internal_DebugMask2;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1428;-10112,352;Float;False;1475;Internal_TypeGeneric;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1416;-10112,-1328;Float;False;1462;Internal_DebugMask3;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1461;-10112,-1488;Float;False;1474;Internal_DebugMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1699;-1088,2176;Float;False;Constant;_Float28;Float 28;31;0;Create;True;0;0;False;0;142;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1680;-1792,256;Float;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1648;-1088,-1408;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1671;-1088,-384;Float;False;Constant;_Float14;Float 14;31;0;Create;True;0;0;False;0;121;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1669;-1088,400;Float;False;Constant;_Float13;Float 13;31;0;Create;True;0;0;False;0;124;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1668;-1088,128;Float;False;Constant;_Float10;Float 10;31;0;Create;True;0;0;False;0;123;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1688;-1088,768;Float;False;Constant;_Float20;Float 20;31;0;Create;True;0;0;False;0;131;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1663;-1792,-896;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1682;-1088,1024;Float;False;Constant;_Float15;Float 15;31;0;Create;True;0;0;False;0;132;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1666;-1088,-512;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1687;-1088,1408;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1685;-1088,1280;Float;False;Constant;_Float18;Float 18;31;0;Create;True;0;0;False;0;133;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1695;-1792,896;Float;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1677;-1792,-512;Float;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1678;-1792,-256;Float;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1712;-1792,2048;Float;False;3;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;1384;-1792,-3712;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1410;-1088,-3584;Float;False;Constant;_Float24;Float 24;31;0;Create;True;0;0;False;0;101;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1713;-1792,2304;Float;False;3;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1694;-1792,640;Float;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1700;-1088,1792;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1701;-1088,2048;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1711;-1792,1792;Float;False;3;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1670;-1088,256;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1704;-1088,2560;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1662;-1792,-1152;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1667;-1088,-256;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1665;-1088,-128;Float;False;Constant;_Float8;Float 8;31;0;Create;True;0;0;False;0;122;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1686;-1088,1552;Float;False;Constant;_Float19;Float 19;31;0;Create;True;0;0;False;0;134;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1664;-1088,0;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1696;-1792,1152;Float;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1703;-1088,2704;Float;False;Constant;_Float31;Float 31;31;0;Create;True;0;0;False;0;144;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1645;-1088,-1152;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1647;-1088,-1664;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1661;-1792,-1408;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1684;-1088,896;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1705;-1088,1920;Float;False;Constant;_Float32;Float 32;31;0;Create;True;0;0;False;0;141;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1683;-1088,640;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1651;-1088,-752;Float;False;Constant;_Float6;Float 6;31;0;Create;True;0;0;False;0;114;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1650;-1088,-1024;Float;False;Constant;_Float5;Float 5;31;0;Create;True;0;0;False;0;113;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1679;-1792,0;Float;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1714;-1792,2560;Float;False;3;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1646;-1088,-1280;Float;False;Constant;_Float4;Float 4;31;0;Create;True;0;0;False;0;112;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1698;-1088,2304;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1652;-1088,-896;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1702;-1088,2432;Float;False;Constant;_Float30;Float 30;31;0;Create;True;0;0;False;0;143;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1697;-1792,1408;Float;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1681;-1088,1152;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1660;-1792,-1664;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1653;-1088,-1536;Float;False;Constant;_Float7;Float 7;31;0;Create;True;0;0;False;0;111;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1471;-9088,-3328;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1378;-9088,-1520;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1356;-9088,128;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1413;-9088,-2128;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1386;-1088,-3456;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1406;-9088,-1104;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1418;-9088,-3712;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1477;-9088,-1392;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1631;-6032,-1600;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1359;-1088,-3072;Float;False;Constant;_Float21;Float 21;31;0;Create;True;0;0;False;0;103;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1613;-6272,-2608;Float;False;ADS Global Settings;2;;1534;0fe83146627632b4981f5a0aa1b63801;0;1;171;FLOAT;0;False;3;COLOR;85;COLOR;165;FLOAT;157
Node;AmplifyShaderEditor.VertexColorNode;1388;-1792,-2944;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1307;-1088,-2944;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1403;-9088,-1648;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1430;-9088,0;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1424;-9088,-2416;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1457;-9088,-2544;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1379;-9088,-512;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1377;-1792,-3456;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;1606;-3584,-2176;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1361;-9088,-2672;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1330;-9088,-640;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1467;-8960,-5600;Float;False;Constant;_Color12;Color 12;21;0;Create;True;0;0;False;0;0.1397059,0.1397059,0.1397059,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1339;-1088,-3200;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1329;-1088,-3328;Float;False;Constant;_Float9;Float 9;31;0;Create;True;0;0;False;0;102;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1292;-1792,-3200;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;1602;-3728,-1632;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.25;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1431;-9088,-3200;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1370;-1088,-2800;Float;False;Constant;_Float22;Float 22;31;0;Create;True;0;0;False;0;104;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1757;-1792,-2560;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1758;-1088,-2176;Float;False;Constant;_Float36;Float 36;31;0;Create;True;0;0;False;0;106;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1751;-1088,-2432;Float;False;Constant;_Float34;Float 34;31;0;Create;True;0;0;False;0;105;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1753;-1088,-2560;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1759;-1088,-2304;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1763;-1088,-2048;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1765;-1792,-2048;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;1761;-1792,-2304;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1762;-1088,-1920;Float;False;Constant;_Float37;Float 37;31;0;Create;True;0;0;False;0;107;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1320;-6272,-3456;Half;False;Constant;_Color15;Color 15;3;0;Create;True;0;0;False;0;0.9191176,0.566,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1340;-6272,-3296;Float;False;1466;Internal_LitStandard;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1288;-6272,-3024;Float;False;1493;Internal_LitSimple;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1357;-6272,-3200;Half;False;Constant;_Color16;Color 16;3;0;Create;True;0;0;False;0;0,0.5859026,0.7794118,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1614;-6272,-2496;Float;False;1463;Internal_TypeTreeLeaf;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1394;-6272,-3712;Half;False;Constant;_Color14;Color 14;3;0;Create;True;0;0;False;0;0.415213,0.6764706,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1283;-1088,-3712;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1297;-6272,-3552;Float;False;1473;Internal_LitAdvanced;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1657;-880,-1664;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1764;-880,-2048;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1655;-880,-1152;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1672;-880,0;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1689;-880,1152;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1674;-880,-512;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1658;-880,-896;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;4;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1760;-880,-2304;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1707;-880,2048;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1374;-880,-3712;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1754;-880,-2560;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1706;-880,2304;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1708;-880,1792;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1692;-880,1408;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;4;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1690;-880,896;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1656;-880,-1408;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1448;-8320,-2816;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1455;-6272,-2048;Half;False;Constant;_Color17;Color 17;3;0;Create;True;0;0;False;0;0.1586208,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1400;-8000,-3072;Half;False;Constant;_Color10;Color 10;3;0;Create;True;0;0;False;0;1,0.241,0.1176471,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;1617;-6000,-2608;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1607;-3440,-2176;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1634;-6272,-2352;Half;False;Property;ADS_GlobalTintIntensity;Fetch ADS_GlobalTintIntensity;20;0;Fetch;False;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1285;-8000,-3584;Half;False;Constant;_Color9;Color 9;3;0;Create;True;0;0;False;0;1,0.241,0.1176471,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1443;-8000,-2304;Half;False;Constant;_Color13;Color 13;3;0;Create;True;0;0;False;0;0,1,0.6689656,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1319;-8000,-2688;Half;False;Constant;_Color11;Color 11;3;0;Create;True;0;0;False;0;1,0.241,0.1176471,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;1363;-880,-2944;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;4;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1355;-880,-3456;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1583;-10112,1168;Float;False;ADS Global Turbulence;10;;1539;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.SimpleAddOpNode;1304;-8320,-3200;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1481;-8736,-5600;Half;False;OtherColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1335;-6272,-1888;Half;False;Constant;_Color0;Color 0;3;0;Create;True;0;0;False;0;0.375,0.6637931,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;1633;-5936,-1792;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1407;-880,-3200;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1419;-10112,736;Half;False;Constant;_Color1;Color 1;3;0;Create;True;0;0;False;0;0.5055925,0.6176471,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;1322;-8320,-2432;Float;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1673;-880,-256;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1691;-880,640;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1675;-880,256;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;4;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1709;-880,2560;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;4;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1298;-8320,-3712;Float;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1315;-5952,-3200;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1349;-5952,-3456;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1385;-5952,-3712;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1659;-512,-1280;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1710;-512,2176;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1693;-512,1024;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1676;-512,-128;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1376;-5632,-3712;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1311;-512,-3328;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1305;-6272,-2688;Float;False;1481;OtherColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1464;-7680,-3712;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1454;-7680,-2432;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1380;-9472,640;Float;False;1481;OtherColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1743;-3248,-1984;Float;False;ADS Debug Other;0;;1542;d6ad6eace599767429eafd03127b39e4;0;1;4;COLOR;0,0,0,0;False;1;COLOR;5
Node;AmplifyShaderEditor.RangedFloatNode;1561;-3248,-2096;Float;False;Constant;_Float3;Float 3;21;0;Create;True;0;0;False;0;64;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1628;-5648,-1840;Float;False;1472;Internal_TypeFoliage;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1560;-3248,-2176;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1515;-6272,-1120;Half;False;Constant;_Color8;Color 8;3;0;Create;True;0;0;False;0;0.5459431,0.1102941,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;1354;-5824,-2016;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1334;-7680,-2816;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1514;-6272,-1280;Half;False;Constant;_Color2;Color 2;3;0;Create;True;0;0;False;0;0.5,0.5,0.5,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;1766;-512,-2304;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1436;-10112,912;Half;False;Property;_MotionNoise;Fetch _MotionNoise;22;0;Fetch;False;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1317;-7680,-3200;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1343;-5632,-2048;Float;False;1481;OtherColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;1635;-5808,-2608;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1479;-5888,-2368;Half;False;Property;_GlobalTint;Fetch _GlobalTint;21;0;Fetch;False;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1447;-9472,736;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1316;-7488,-2736;Float;False;Constant;_Float11;Float 11;21;0;Create;True;0;0;False;0;13;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1333;-5376,-2048;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1451;-7488,-2352;Float;False;Constant;_Float27;Float 27;21;0;Create;True;0;0;False;0;14;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1517;-5376,-1152;Float;False;Constant;_Float12;Float 12;33;0;Create;True;0;0;False;0;62;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1275;-5344,-3712;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1338;-7488,-3120;Float;False;Constant;_Float16;Float 16;21;0;Create;True;0;0;False;0;12;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1327;-5184,-2048;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1563;-2992,-2176;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;14;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1282;-7488,-2432;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1402;-7488,-3632;Float;False;Constant;_Float23;Float 23;21;0;Create;True;0;0;False;0;11;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1423;-9152,720;Float;False;Constant;_Float26;Float 26;21;0;Create;True;0;0;False;0;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1412;-5344,-3632;Float;False;Constant;_Float25;Float 25;21;0;Create;True;0;0;False;0;61;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1342;-5184,-1920;Float;False;Constant;_Float17;Float 17;33;0;Create;True;0;0;False;0;32;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1491;-5376,-2688;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1453;-7488,-3712;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1420;-9152,640;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1746;-5344,-3520;Float;False;ADS Debug Other;0;;1545;d6ad6eace599767429eafd03127b39e4;0;1;4;COLOR;0,0,0,0;False;1;COLOR;5
Node;AmplifyShaderEditor.FunctionNode;1745;-7488,-2240;Float;False;ADS Debug Other;0;;1544;d6ad6eace599767429eafd03127b39e4;0;1;4;COLOR;0,0,0,0;False;1;COLOR;5
Node;AmplifyShaderEditor.FunctionNode;1744;-7488,-2624;Float;False;ADS Debug Other;0;;1543;d6ad6eace599767429eafd03127b39e4;0;1;4;COLOR;0,0,0,0;False;1;COLOR;5
Node;AmplifyShaderEditor.SimpleAddOpNode;1715;-192,-128;Float;False;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1748;-7488,-3520;Float;False;ADS Debug Other;0;;1547;d6ad6eace599767429eafd03127b39e4;0;1;4;COLOR;0,0,0,0;False;1;COLOR;5
Node;AmplifyShaderEditor.FunctionNode;1747;-7488,-3008;Float;False;ADS Debug Other;0;;1546;d6ad6eace599767429eafd03127b39e4;0;1;4;COLOR;0,0,0,0;False;1;COLOR;5
Node;AmplifyShaderEditor.LerpOp;1494;-5632,-2688;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1492;-5376,-2560;Float;False;Constant;_Float29;Float 29;33;0;Create;True;0;0;False;0;31;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1523;-5888,-1280;Float;False;Property;INSTANCING;INSTANCING_ON;39;0;Fetch;False;0;0;False;0;0;0;0;False;INSTANCING_ON;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1287;-7488,-3200;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1280;-7488,-2816;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1427;-9312,768;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1518;-5376,-1280;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1490;-5168,-2688;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;31;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;1482;-8960,640;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;14;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;1351;-4976,-2048;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;32;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexToFragmentNode;1719;0,-128;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;1293;-7232,-3200;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;12;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;1321;-7232,-3712;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;11;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;1519;-5168,-1280;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;31;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;1411;-5088,-3712;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;14;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;1274;-7232,-2432;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;14;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;1568;-2800,-2176;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;1324;-7232,-2816;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;13;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1564;-2640,-2176;Float;False;SHADER_ISSUES;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1309;-7056,-3712;Float;False;MOTION_MASK_1;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1368;256,-128;Float;False;MESH_CHANNEL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1284;-4848,-3712;Float;False;SHADER_COMPLEXITY;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1313;-7072,-2432;Float;False;MOTION_VARIATION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1449;-7056,-2816;Float;False;MOTION_MASK_3;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1520;-4848,-1280;Float;False;SHADER_INSTANCING;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1364;-7056,-3200;Float;False;MOTION_MASK_2;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1373;-4800,-2048;Float;False;FOLIAGE_SIZE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1426;-8784,640;Float;False;MOTION_NOISE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1306;-4800,-2688;Float;False;FOLAIGE_TINT;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1326;-7296,-5760;Float;False;1284;SHADER_COMPLEXITY;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1289;-7296,-6160;Float;False;1313;MOTION_VARIATION;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1362;-7296,-6400;Float;False;1309;MOTION_MASK_1;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1300;-7296,-6320;Float;False;1364;MOTION_MASK_2;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1722;-8960,-5696;Half;False;Global;ADS_DebugMax;ADS_DebugMax;3;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1720;-8960,-5792;Half;False;Global;ADS_DebugMin;ADS_DebugMin;3;0;Create;False;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1432;-7296,-5952;Float;False;1373;FOLIAGE_SIZE;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1299;-7296,-5504;Float;False;1368;MESH_CHANNEL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1276;-7296,-6240;Float;False;1449;MOTION_MASK_3;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1524;-7296,-5680;Float;False;1520;SHADER_INSTANCING;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1460;-7296,-6080;Float;False;1426;MOTION_NOISE;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1301;-7296,-5872;Float;False;1306;FOLAIGE_TINT;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1566;-7296,-5600;Float;False;1564;SHADER_ISSUES;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1526;-6976,-5760;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1312;-6992,-6400;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1723;-8736,-5696;Half;False;DebugMax;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1721;-8736,-5792;Half;False;DebugMin;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;1442;-10112,-5504;Float;False;Standard;TangentNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1.5;False;3;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1525;-6976,-5952;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1290;-6736,-6400;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1439;-10112,-5712;Half;False;Constant;_Color6;Color 6;0;0;Create;True;0;0;False;0;1,0.809,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1459;-10112,-5888;Half;False;Constant;_Color7;Color 7;0;0;Create;True;0;0;False;0;1,0.5019608,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1726;-6736,-6272;Float;False;1721;DebugMin;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1440;-9856,-5632;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1727;-6736,-6192;Float;False;1723;DebugMax;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1724;-6400,-6400;Float;False;Remap To 0-1;-1;;1567;5eda8a2bb94ebef4ab0e43d19291cd8b;0;3;6;COLOR;0,0,0,0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1437;-9664,-5888;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1434;-9600,-5696;Half;False;Property;_Debug_Arrow;Debug_Arrow;16;0;Create;True;0;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1741;-6528,-5888;Float;False;Constant;_Float35;Float 35;43;0;Create;True;0;0;False;0;-98;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;1740;-6272,-5888;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1438;-9408,-5888;Half;False;ArrowColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1450;-9408,-5696;Half;False;ArrowDebug;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1222;-6208,-6400;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1750;-6400,-6272;Float;False;1481;OtherColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;18;-9216,-6400;Float;True;Property;_AlbedoTex;Main Texture;38;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1619;-5856,-1696;Half;False;GlobalSize;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1151;-5760,-5952;Half;False;Constant;_Float0;Float 0;20;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1636;-5760,-5440;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1273;-5760,-6144;Float;False;1438;ArrowColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1620;-5760,-5536;Float;False;1619;GlobalSize;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1281;-5760,-6064;Float;False;1450;ArrowDebug;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1738;-5632,-6400;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;616;-8912,-6272;Half;False;MainTexAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;791;-5760,-5824;Float;False;616;MainTexAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1572;-5376,-6080;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1278;-5440,-6400;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1627;-5408,-5536;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1510;-5760,-5744;Float;False;Constant;_Float1;Float 1;38;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1513;-5504,-5312;Float;False;1472;Internal_TypeFoliage;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1509;-5760,-5664;Float;False;1488;Internal_TypeTreeBark;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1545;-4096,-2752;Float;False;1488;Internal_TypeTreeBark;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1536;-3776,-3712;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;563;-10112,-6272;Half;False;Property;_UVZero;Main UVs;39;0;Create;False;0;0;False;1;Space(10);1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1549;-4096,-2656;Half;False;Constant;_Color22;Color 22;3;0;Create;True;0;0;False;0;0.07006919,0.5955882,0.07369344,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;562;-9664,-6400;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;1584;-10112,1232;Float;False;ADS Global Turbulence;10;;1568;047eb809542f42d40b4b5066e22cee72;0;0;1;FLOAT;85
Node;AmplifyShaderEditor.ConditionalIfNode;1541;-2912,-3712;Float;False;True;5;0;FLOAT;0;False;1;FLOAT;14;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1314;-1600,-3712;Half;False;Constant;_Color3;Color 3;3;0;Create;True;0;0;False;0;1,0.4926471,0.4926471,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1286;-1344,-3648;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1308;-10112,-7040;Half;False;Property;_MaskMin;Mask Min;8;0;Create;True;0;0;False;1;Space(10);0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1360;-8704,-7040;Half;False;Property;_Show_MaskSpeedTree;Show_MaskSpeedTree;15;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1366;-1600,-3200;Half;False;Constant;_Color5;Color 5;3;0;Create;True;0;0;False;0;0.5,0.5,1,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1444;-8448,-7040;Half;False;_Show_MaskSpeedTree;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;487;-8912,-6400;Half;False;MainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1389;-1600,-3456;Half;False;Constant;_Color4;Color 4;3;0;Create;True;0;0;False;0;0.4926471,1,0.4926471,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;1537;-3456,-3712;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;565;-9856,-6192;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1548;-4096,-2480;Float;False;1463;Internal_TypeTreeLeaf;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1295;-9568,-7040;Half;False;_MaskMax;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1533;-4096,-3200;Half;False;Constant;_Color20;Color 20;3;0;Create;True;0;0;False;0;0,0.3685599,0.6764706,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1279;-9024,-7040;Half;False;_Show_ADSMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1528;-4096,-3024;Float;False;1475;Internal_TypeGeneric;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1547;-3776,-2656;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1535;-3776,-3456;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1414;-9728,-7040;Half;False;Property;_MaskMax;Mask Max;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1540;-3168,-3712;Float;False;1487;DebugMode;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-9472,-6912;Half;False;Property;_CullMode;Cull Mode;36;1;[Enum];Create;True;3;Off;0;Front;1;Back;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;862;-9088,-6912;Half;False;Property;_Cutoff;Cutout/Fade;37;0;Create;False;3;Off;0;Front;1;Back;2;0;True;1;Space(10);1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1581;-9536,1024;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1542;-2624,-3712;Float;False;SHADER_TYPE;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1546;-4096,-2928;Half;False;Constant;_Color21;Color 21;3;0;Create;True;0;0;False;0;0.5661765,0.4053961,0.2331315,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1532;-4096,-3296;Float;False;1469;Internal_TypeCloth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1531;-4096,-3552;Float;False;1472;Internal_TypeFoliage;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1530;-4096,-3712;Half;False;Constant;_Color19;Color 19;3;0;Create;True;0;0;False;0;0.4072486,0.6544118,0.01443557,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1529;-4096,-3456;Half;False;Constant;_Color18;Color 18;3;0;Create;True;0;0;False;0;0.8088235,0.4852941,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;925;-9280,-6912;Half;False;Property;_ZWrite;_ZWrite;40;1;[HideInInspector];Create;True;2;Off;0;On;1;0;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1626;-5184,-5536;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;561;-10112,-6400;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;1580;-9728,1024;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1271;-9952,-7040;Half;False;_MaskMin;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1577;-9840,1024;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1353;-5680,-1904;Half;False;Property;_GlobalSize;Fetch _GlobalSize;17;0;Fetch;False;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1578;-10112,1024;Float;False;1488;Internal_TypeTreeBark;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1728;-5332.133,-3405.601;Float;False;Constant;_Float33;Float 33;43;0;Create;True;0;0;False;0;-99;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1367;-1344,-3392;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-10112,-6912;Half;False;Property;_SrcBlend;_SrcBlend;41;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1579;-10112,1088;Float;False;1463;Internal_TypeTreeLeaf;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;553;-9952,-6912;Half;False;Property;_DstBlend;_DstBlend;42;1;[HideInInspector];Create;True;0;0;True;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;549;-9728,-6912;Half;False;Property;_Mode;Blend Mode;7;1;[Enum];Create;False;3;Opaque;0;Cutout;1;Fade;2;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1508;-5504,-5824;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1573;-5184,-6080;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1318;-1344,-3136;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1534;-3776,-3200;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1749;-3200,-3520;Float;False;ADS Debug Other;0;;1571;d6ad6eace599767429eafd03127b39e4;0;1;4;COLOR;0,0,0,0;False;1;COLOR;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1150;-5168,-6400;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1544;-3776,-2928;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1539;-3168,-3632;Float;False;Constant;_Float2;Float 2;21;0;Create;True;0;0;False;0;63;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;564;-9856,-6272;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;575;-9456,-6400;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;1458;-9280,-7040;Half;False;Property;_Show_ADSMask;Show_ADSMask;14;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-4736,-6272;Float;False;True;2;Float;ADSShaderGUI;300;0;Lambert;Utils/ADS Debug;False;False;False;False;True;True;True;True;True;True;True;True;False;True;True;True;True;False;False;False;False;Off;0;False;925;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0;True;False;0;False;TransparentCutout;;AlphaTest;All;False;True;True;False;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;1;True;550;1;True;553;0;0;False;550;0;False;553;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;300;;-1;-1;-1;-1;0;False;0;0;True;743;-1;0;True;862;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;1503;-10112,-768;Float;False;1151.458;100;Cloth // Vertex R = Mask / Vertex R = Variation;0;;1,0.8896552,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1498;-10112,-2816;Float;False;1156.727;100;Packed Tree Bark // VertexR = Trunk Mask / VertexG= Branch Mask / VertexB= Leaf / VertexA = AO / UV0.T = Motion Variation;0;;1,0.6677485,0.07352942,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1504;-6272,-2816;Float;False;1663.898;100;Global Tint;0;;1,0.6,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1497;-10112,-1792;Float;False;1156.727;100;Packed Tree Leaf // VertexR = Trunk Mask / VertexG= Branch Mask / VertexB= Leaf / VertexA = AO / UV0.T = Motion Variation;0;;0.6137931,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1543;-4096,-3840;Float;False;1661.688;100;Shader Type;0;;0.6137931,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1552;-4096,-2304;Float;False;1662.707;100;Shader Issues;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1516;-6272,-1408;Float;False;1663.898;100;Instancing;0;;1,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1496;-6272,-3840;Float;False;1661.688;100;Shader Complexity;0;;0.6137931,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1506;-6272,-2176;Float;False;1664.89;100;Global Size;0;;0.3602941,0.4705882,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1502;-1792,-3840;Float;False;1664.767;100;Vertex Color;0;;1,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1505;-10112,-3840;Float;False;1149.95;100;Packed Foliage // Vertex A = Plant Mask / UV0.W = Leaf Mask / UV0.T = Variation;0;;0.1172413,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1501;-10112,512;Float;False;1818.832;100;Motion Noise;0;;0.8206897,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1499;-10112,-128;Float;False;1153.288;100;Generic // Vertex R = Mask / Vertex R = Variation;0;;0.2876297,0.5780365,0.9779412,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1500;-8320,-3840;Float;False;1528.8;100;Motion Debug;0;;1,1,1,1;0;0
WireConnection;1591;0;1590;0
WireConnection;1591;1;1468;0
WireConnection;1592;0;1591;0
WireConnection;1469;0;1486;0
WireConnection;1472;0;1592;0
WireConnection;1462;0;1395;0
WireConnection;1558;0;1565;0
WireConnection;1558;1;1556;0
WireConnection;1604;0;1567;0
WireConnection;1604;1;1603;0
WireConnection;1488;0;1396;0
WireConnection;1474;0;1478;0
WireConnection;1463;0;1433;0
WireConnection;1475;0;1465;0
WireConnection;1392;0;1393;0
WireConnection;1397;0;1387;0
WireConnection;1429;0;1476;1
WireConnection;1429;1;1476;1
WireConnection;1597;0;1599;0
WireConnection;1493;0;1483;0
WireConnection;1473;0;1484;0
WireConnection;1466;0;1470;0
WireConnection;1605;0;1558;0
WireConnection;1605;1;1604;0
WireConnection;1446;0;1371;1
WireConnection;1446;1;1371;1
WireConnection;1487;0;1480;0
WireConnection;1471;0;1425;0
WireConnection;1471;1;1415;3
WireConnection;1471;2;1375;0
WireConnection;1378;0;1371;2
WireConnection;1378;1;1417;0
WireConnection;1378;2;1404;0
WireConnection;1356;0;1382;4
WireConnection;1356;1;1441;0
WireConnection;1356;2;1428;0
WireConnection;1413;0;1476;4
WireConnection;1413;1;1485;0
WireConnection;1413;2;1422;0
WireConnection;1406;0;1371;4
WireConnection;1406;1;1399;0
WireConnection;1406;2;1404;0
WireConnection;1418;0;1415;1
WireConnection;1418;1;1323;0
WireConnection;1418;2;1425;0
WireConnection;1477;0;1371;3
WireConnection;1477;1;1416;0
WireConnection;1631;0;1618;157
WireConnection;1631;1;1632;0
WireConnection;1613;171;1616;4
WireConnection;1403;0;1446;0
WireConnection;1403;1;1461;0
WireConnection;1403;2;1404;0
WireConnection;1430;0;1382;1
WireConnection;1430;1;1401;0
WireConnection;1430;2;1428;0
WireConnection;1424;0;1476;3
WireConnection;1424;1;1408;0
WireConnection;1457;0;1476;2
WireConnection;1457;1;1409;0
WireConnection;1457;2;1422;0
WireConnection;1379;0;1369;4
WireConnection;1379;1;1372;0
WireConnection;1379;2;1435;0
WireConnection;1606;0;1605;0
WireConnection;1361;0;1429;0
WireConnection;1361;1;1383;0
WireConnection;1361;2;1422;0
WireConnection;1330;0;1369;1
WireConnection;1330;1;1358;0
WireConnection;1330;2;1435;0
WireConnection;1602;0;1597;0
WireConnection;1431;0;1415;4
WireConnection;1431;1;1296;0
WireConnection;1431;2;1425;0
WireConnection;1657;0;1647;0
WireConnection;1657;1;1653;0
WireConnection;1657;3;1660;1
WireConnection;1764;0;1763;0
WireConnection;1764;1;1762;0
WireConnection;1764;3;1765;3
WireConnection;1655;0;1645;0
WireConnection;1655;1;1650;0
WireConnection;1655;3;1662;3
WireConnection;1672;0;1664;0
WireConnection;1672;1;1668;0
WireConnection;1672;3;1679;3
WireConnection;1689;0;1681;0
WireConnection;1689;1;1685;0
WireConnection;1689;3;1696;3
WireConnection;1674;0;1666;0
WireConnection;1674;1;1671;0
WireConnection;1674;3;1677;1
WireConnection;1658;0;1652;0
WireConnection;1658;1;1651;0
WireConnection;1658;3;1663;4
WireConnection;1760;0;1759;0
WireConnection;1760;1;1758;0
WireConnection;1760;3;1761;2
WireConnection;1707;0;1701;0
WireConnection;1707;1;1699;0
WireConnection;1707;3;1712;2
WireConnection;1374;0;1283;0
WireConnection;1374;1;1410;0
WireConnection;1374;3;1384;1
WireConnection;1754;0;1753;0
WireConnection;1754;1;1751;0
WireConnection;1754;3;1757;1
WireConnection;1706;0;1698;0
WireConnection;1706;1;1702;0
WireConnection;1706;3;1713;3
WireConnection;1708;0;1700;0
WireConnection;1708;1;1705;0
WireConnection;1708;3;1711;1
WireConnection;1692;0;1687;0
WireConnection;1692;1;1686;0
WireConnection;1692;3;1697;4
WireConnection;1690;0;1684;0
WireConnection;1690;1;1682;0
WireConnection;1690;3;1695;2
WireConnection;1656;0;1648;0
WireConnection;1656;1;1646;0
WireConnection;1656;3;1661;2
WireConnection;1448;0;1424;0
WireConnection;1448;1;1477;0
WireConnection;1617;0;1613;85
WireConnection;1617;1;1613;165
WireConnection;1617;2;1614;0
WireConnection;1607;0;1606;0
WireConnection;1607;1;1602;0
WireConnection;1363;0;1307;0
WireConnection;1363;1;1370;0
WireConnection;1363;3;1388;4
WireConnection;1355;0;1386;0
WireConnection;1355;1;1329;0
WireConnection;1355;3;1377;2
WireConnection;1304;0;1471;0
WireConnection;1304;1;1457;0
WireConnection;1304;2;1378;0
WireConnection;1481;0;1467;0
WireConnection;1633;0;1631;0
WireConnection;1407;0;1339;0
WireConnection;1407;1;1359;0
WireConnection;1407;3;1292;3
WireConnection;1322;0;1431;0
WireConnection;1322;1;1413;0
WireConnection;1322;2;1379;0
WireConnection;1322;3;1356;0
WireConnection;1322;4;1406;0
WireConnection;1673;0;1667;0
WireConnection;1673;1;1665;0
WireConnection;1673;3;1678;2
WireConnection;1691;0;1683;0
WireConnection;1691;1;1688;0
WireConnection;1691;3;1694;1
WireConnection;1675;0;1670;0
WireConnection;1675;1;1669;0
WireConnection;1675;3;1680;4
WireConnection;1709;0;1704;0
WireConnection;1709;1;1703;0
WireConnection;1709;3;1714;4
WireConnection;1298;0;1418;0
WireConnection;1298;1;1361;0
WireConnection;1298;2;1330;0
WireConnection;1298;3;1430;0
WireConnection;1298;4;1403;0
WireConnection;1315;0;1357;0
WireConnection;1315;1;1288;0
WireConnection;1349;0;1320;0
WireConnection;1349;1;1340;0
WireConnection;1385;0;1394;0
WireConnection;1385;1;1297;0
WireConnection;1659;0;1657;0
WireConnection;1659;1;1656;0
WireConnection;1659;2;1655;0
WireConnection;1659;3;1658;0
WireConnection;1710;0;1708;0
WireConnection;1710;1;1707;0
WireConnection;1710;2;1706;0
WireConnection;1710;3;1709;0
WireConnection;1693;0;1691;0
WireConnection;1693;1;1690;0
WireConnection;1693;2;1689;0
WireConnection;1693;3;1692;0
WireConnection;1676;0;1674;0
WireConnection;1676;1;1673;0
WireConnection;1676;2;1672;0
WireConnection;1676;3;1675;0
WireConnection;1376;0;1385;0
WireConnection;1376;1;1349;0
WireConnection;1376;2;1315;0
WireConnection;1311;0;1374;0
WireConnection;1311;1;1355;0
WireConnection;1311;2;1407;0
WireConnection;1311;3;1363;0
WireConnection;1464;0;1298;0
WireConnection;1464;1;1285;0
WireConnection;1454;0;1322;0
WireConnection;1454;1;1443;0
WireConnection;1743;4;1607;0
WireConnection;1354;0;1455;0
WireConnection;1354;1;1335;0
WireConnection;1354;2;1633;0
WireConnection;1334;0;1448;0
WireConnection;1334;1;1319;0
WireConnection;1766;0;1754;0
WireConnection;1766;1;1760;0
WireConnection;1766;2;1764;0
WireConnection;1317;0;1304;0
WireConnection;1317;1;1400;0
WireConnection;1635;0;1617;0
WireConnection;1635;1;1634;0
WireConnection;1447;0;1419;0
WireConnection;1447;1;1583;85
WireConnection;1333;0;1343;0
WireConnection;1333;1;1354;0
WireConnection;1333;2;1628;0
WireConnection;1563;0;1560;0
WireConnection;1563;1;1561;0
WireConnection;1563;3;1743;5
WireConnection;1746;4;1376;0
WireConnection;1745;4;1454;0
WireConnection;1744;4;1334;0
WireConnection;1715;0;1311;0
WireConnection;1715;1;1766;0
WireConnection;1715;2;1659;0
WireConnection;1715;3;1676;0
WireConnection;1715;4;1693;0
WireConnection;1715;5;1710;0
WireConnection;1748;4;1464;0
WireConnection;1747;4;1317;0
WireConnection;1494;0;1305;0
WireConnection;1494;1;1635;0
WireConnection;1494;2;1479;0
WireConnection;1523;1;1514;0
WireConnection;1523;0;1515;0
WireConnection;1427;0;1380;0
WireConnection;1427;1;1447;0
WireConnection;1427;2;1436;0
WireConnection;1490;0;1491;0
WireConnection;1490;1;1492;0
WireConnection;1490;3;1494;0
WireConnection;1482;0;1420;0
WireConnection;1482;1;1423;0
WireConnection;1482;3;1427;0
WireConnection;1351;0;1327;0
WireConnection;1351;1;1342;0
WireConnection;1351;3;1333;0
WireConnection;1719;0;1715;0
WireConnection;1293;0;1287;0
WireConnection;1293;1;1338;0
WireConnection;1293;3;1747;5
WireConnection;1321;0;1453;0
WireConnection;1321;1;1402;0
WireConnection;1321;3;1748;5
WireConnection;1519;0;1518;0
WireConnection;1519;1;1517;0
WireConnection;1519;3;1523;0
WireConnection;1411;0;1275;0
WireConnection;1411;1;1412;0
WireConnection;1411;3;1746;5
WireConnection;1274;0;1282;0
WireConnection;1274;1;1451;0
WireConnection;1274;3;1745;5
WireConnection;1568;0;1563;0
WireConnection;1324;0;1280;0
WireConnection;1324;1;1316;0
WireConnection;1324;3;1744;5
WireConnection;1564;0;1568;0
WireConnection;1309;0;1321;0
WireConnection;1368;0;1719;0
WireConnection;1284;0;1411;0
WireConnection;1313;0;1274;0
WireConnection;1449;0;1324;0
WireConnection;1520;0;1519;0
WireConnection;1364;0;1293;0
WireConnection;1373;0;1351;0
WireConnection;1426;0;1482;0
WireConnection;1306;0;1490;0
WireConnection;1526;0;1326;0
WireConnection;1526;1;1524;0
WireConnection;1526;2;1566;0
WireConnection;1526;3;1299;0
WireConnection;1312;0;1362;0
WireConnection;1312;1;1300;0
WireConnection;1312;2;1276;0
WireConnection;1312;3;1289;0
WireConnection;1312;4;1460;0
WireConnection;1723;0;1722;0
WireConnection;1721;0;1720;0
WireConnection;1525;0;1432;0
WireConnection;1525;1;1301;0
WireConnection;1290;0;1312;0
WireConnection;1290;1;1525;0
WireConnection;1290;2;1526;0
WireConnection;1440;0;1442;0
WireConnection;1724;6;1290;0
WireConnection;1724;7;1726;0
WireConnection;1724;8;1727;0
WireConnection;1437;0;1459;0
WireConnection;1437;1;1439;0
WireConnection;1437;2;1440;0
WireConnection;1740;0;1290;0
WireConnection;1740;1;1741;0
WireConnection;1438;0;1437;0
WireConnection;1450;0;1434;0
WireConnection;1222;0;1724;0
WireConnection;1619;0;1618;157
WireConnection;1738;0;1222;0
WireConnection;1738;1;1750;0
WireConnection;1738;2;1740;0
WireConnection;616;0;18;4
WireConnection;1572;0;1151;0
WireConnection;1278;0;1738;0
WireConnection;1278;1;1273;0
WireConnection;1278;2;1281;0
WireConnection;1627;0;1620;0
WireConnection;1627;1;1636;0
WireConnection;1536;0;1530;0
WireConnection;1536;1;1531;0
WireConnection;562;0;561;0
WireConnection;562;1;564;0
WireConnection;1541;0;1540;0
WireConnection;1541;1;1539;0
WireConnection;1541;3;1749;5
WireConnection;1286;0;1314;0
WireConnection;1286;1;1384;1
WireConnection;1444;0;1360;0
WireConnection;487;0;18;0
WireConnection;1537;0;1536;0
WireConnection;1537;1;1535;0
WireConnection;1537;2;1534;0
WireConnection;1537;3;1544;0
WireConnection;1537;4;1547;0
WireConnection;565;0;563;3
WireConnection;565;1;563;4
WireConnection;1295;0;1414;0
WireConnection;1279;0;1458;0
WireConnection;1547;0;1549;0
WireConnection;1547;1;1548;0
WireConnection;1535;0;1529;0
WireConnection;1535;1;1532;0
WireConnection;1581;0;1583;85
WireConnection;1581;1;1584;85
WireConnection;1581;2;1580;0
WireConnection;1542;0;1541;0
WireConnection;1626;1;1627;0
WireConnection;1626;2;1513;0
WireConnection;1580;0;1577;0
WireConnection;1271;0;1308;0
WireConnection;1577;0;1578;0
WireConnection;1577;1;1579;0
WireConnection;1367;0;1389;0
WireConnection;1367;1;1377;2
WireConnection;1508;0;791;0
WireConnection;1508;1;1510;0
WireConnection;1508;2;1509;0
WireConnection;1573;0;1278;0
WireConnection;1573;1;1572;0
WireConnection;1318;0;1366;0
WireConnection;1318;1;1292;3
WireConnection;1534;0;1533;0
WireConnection;1534;1;1528;0
WireConnection;1749;4;1537;0
WireConnection;1150;0;1278;0
WireConnection;1150;1;1151;0
WireConnection;1544;0;1546;0
WireConnection;1544;1;1545;0
WireConnection;564;0;563;1
WireConnection;564;1;563;2
WireConnection;575;0;562;0
WireConnection;575;1;565;0
WireConnection;0;0;1150;0
WireConnection;0;2;1573;0
WireConnection;0;10;1508;0
WireConnection;0;11;1626;0
ASEEND*/
//CHKSM=F4AA60BE8F0966332AD4208D46EE880A3F897CC4