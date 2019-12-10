// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PWS/Procedural/Vegetation SP 2019_02 v3.1"
{
	Properties
	{
		[HideInInspector]_MaskClipValue("Mask Clip Value", Range( 0 , 1)) = 1
		[Enum(UnityEngine.Rendering.CullMode)][Header(GLOBAL SETTINGS)]_CullMode("Cull Mode", Int) = 0
		[Enum(Opaque,0,Cutout Alpha,1,Cutout Black,2)]_RenderMode("Render Mode", Int) = 0
		_AlphaCutoffBias("Alpha Cutoff Bias", Range( 0 , 1)) = 0.49
		_Metallic("Metallic", Range( 0 , 1)) = 0.1
		[Enum(OFF,0,Distance,1,Distance Dither,2,Distance Dither LOD,3)][Header(CROSSFADE)]_CrossfadeMode("Crossfade Mode", Int) = 0
		_CrossfadeDistance("Crossfade Distance", Range( 0 , 512)) = 64
		_CrossfadeDistanceExponent("Crossfade Distance Exponent", Range( 0 , 16)) = 8
		_CrossfadeScale("Crossfade Scale", Range( 0 , 1)) = 0
		[HDR][Header(ALBEDO MAP)]_MainTex("Albedo", 2D) = "white" {}
		[HDR]_Color("Albedo Tint", Color) = (1,1,1,1)
		_AlbedoDetailIntensity("Albedo Detail Intensity", Range( 0 , 1)) = 0
		_AlbedoDetailScale("Albedo Detail Scale", Float) = 0
		_AlbedoDetailDistance("Albedo Detail Distance", Range( 0 , 4)) = 4
		[HDR][NoScaleOffset][Normal][Header(NORMAL MAP)]_BumpMap("Normal", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range( 0 , 1)) = 1
		_NormalDetailStrength("Normal Detail Strength", Range( 0 , 1)) = 0.25
		[NoScaleOffset][Header(ROUGHNESS MAP)]_RoughnessTexture("Roughness Texture", 2D) = "white" {}
		[Enum(Smoothness Cull Off,0,Smoothness Cull Front,1,Smoothness Cull Back,2)][Header (SMOOTHNESS)]_SmoothnessMode("Smoothness Mode", Int) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_SmoothnessVariance("Smoothness Variance", Range( 0 , 1)) = 0.5
		_SmoothnessThreshold("Smoothness Threshold", Range( 0 , 1)) = 0
		[NoScaleOffset][Header(AMBIENT OCCLUSION MAP)]_OcclusionMap("Occlusion Map", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range( 0 , 1)) = 0.5
		[Header(SURFACE MAP)][Toggle]_EnableSurfaceMap("Enable Surface Map", Int) = 0
		[HDR]_SurfaceTexture("Surface Texture", 2D) = "white" {}
		[HDR]_SurfaceTint("Surface Tint", Color) = (1,1,1,0)
		_SurfaceBias("Surface Bias", Range( -1 , 1)) = -0.25
		_SurfaceSaturation("Surface Saturation", Range( 0 , 1)) = 1
		_SurfaceScatterScale("Surface Scatter Scale", Range( 0 , 2)) = 1.5
		_SurfaceBlendStrength("Surface Blend Strength", Range( 0 , 16)) = 0
		_SurfaceDistanceLength("Surface Distance Length", Range( 0.01 , 5)) = 3
		[HDR][NoScaleOffset][Normal]_SurfaceNormalTexture("Surface Normal Texture", 2D) = "bump" {}
		_SurfaceNormalStrength("Surface Normal Strength", Range( 0 , 1)) = 1
		[Header(COVER MAP)][Toggle]_EnableCoverMap("Enable Cover Map", Int) = 0
		[HDR]_CoverTexture("Cover Texture", 2D) = "white" {}
		_CoverIntensity("Cover Intensity", Range( 0 , 10)) = 2
		_CoverBias("Cover Bias", Range( -1 , 1)) = 0
		[HDR][NoScaleOffset][Normal]_CoverNormalTexture("Cover Normal Texture", 2D) = "bump" {}
		_CoverNormalStrength("Cover Normal Strength", Range( 0 , 1)) = 1
		[Header(COLOR MASK)][Toggle]_EnableColorMask("Enable Color Mask", Int) = 0
		[HDR]_ColorMaskTint("Color Mask Tint", Color) = (0,0,0,0)
		_ColorMaskFuzziness("Color Mask Fuzziness", Float) = 0.03
		_ColorMaskRange("Color Mask Range", Range( 0 , 1)) = 0
		[Header(COLOR MASK SEASONS)]_ColorMaskAutumnShift("Color Mask Autumn Shift", Range( 0 , 1)) = 0
		[Header(SQUARE MASK)][Toggle]_EnableSquareMask("Enable Square Mask", Int) = 0
		_SquareMaskTopXRightYBottomZLeftW("Square Mask Top X, Right Y, Bottom Z, Left W", Vector) = (1,1,0,0)
		[Enum(OFF,0,Simple,1,Precise,2)][Header(SUBSURFACE SCATTERING)]_SSSMode("SSS Mode", Int) = 0
		[HDR]_SSSColor("SSS Color", Color) = (1,1,1,1)
		_SSSDistortion("SSS Distortion", Range( 0 , 2.5)) = 1
		_SSSPower("SSS Power", Range( 0 , 5)) = 1
		_SSSIntensity("SSS Intensity", Range( 0 , 10)) = 1
		[Header(WIND)][Toggle]_EnableWindVegetation("Enable Wind Vegetation", Int) = 0
		_WindIntensity("Wind Intensity", Range( 0 , 0.1)) = 0.0366
		_WindFrequency("Wind Frequency", Range( 0 , 10)) = 2.49
		_WindMagnitude("Wind Magnitude", Range( 0 , 1)) = 0.216
		_WindFrequencyDetail("Wind Frequency Detail", Range( 0 , 5)) = 2.3
		_WindMagnitudeDetail("Wind Magnitude Detail", Range( 0 , 1)) = 0.314
		_WindDirection("Wind Direction", Range( -1 , 1)) = 0.042
		[Header(WIND DISTANCE)][Toggle]_EnableWindDistance("Enable Wind Distance", Int) = 1
		_WindDistanceRange("Wind Distance Range", Range( 0 , 64)) = 8.1
		_WindDistanceOffset("Wind Distance Offset", Range( 0 , 8)) = 0
		[Header(WIND VERTICAL MASK)][Toggle]_EnableWindVerticalMask("Enable Wind Vertical Mask", Int) = 1
		_WindVerticalMaskOffset("Wind Vertical Mask Offset", Range( -1 , 1)) = -0.682
		_WindVerticalMaskIntensity("Wind Vertical Mask Intensity", Range( 0 , 1)) = 0.004
		[Toggle][Header(DEBUG)]_EnableDebug("Enable Debug", Int) = 0
		[Enum(Square Mask,0,Color Mask,1,Vertical Mask,2,Distance Mask,3)]_DebugMode("Debug Mode", Int) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull [_CullMode]
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			half ASEVFace : VFACE;
			float4 vertexColor : COLOR;
			float4 screenPosition;
		};

		uniform float _MaskClipValue;
		uniform int _CullMode;
		uniform int _EnableWindVegetation;
		uniform float _WindDirection;
		uniform float _WindFrequency;
		uniform float _WindMagnitude;
		uniform float _WindFrequencyDetail;
		uniform float _WindMagnitudeDetail;
		uniform float _WindIntensity;
		uniform half PWSF_GlobalWindIntensity;
		uniform float _WindVerticalMaskOffset;
		uniform float _WindVerticalMaskIntensity;
		uniform int _EnableWindVerticalMask;
		uniform float _WindDistanceRange;
		uniform float _WindDistanceOffset;
		uniform int _EnableWindDistance;
		uniform float _NormalStrength;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _AlbedoDetailScale;
		uniform float _NormalDetailStrength;
		uniform float _AlbedoDetailDistance;
		uniform float _AlbedoDetailIntensity;
		uniform float _SurfaceNormalStrength;
		uniform sampler2D _SurfaceNormalTexture;
		uniform sampler2D _SurfaceTexture;
		uniform float4 _SurfaceTexture_ST;
		uniform float _SurfaceScatterScale;
		uniform float _SurfaceBias;
		uniform float _SurfaceDistanceLength;
		uniform float _SurfaceBlendStrength;
		uniform int _EnableSurfaceMap;
		uniform float _CoverNormalStrength;
		uniform sampler2D _CoverNormalTexture;
		uniform sampler2D _CoverTexture;
		uniform float4 _CoverTexture_ST;
		uniform float _CoverBias;
		uniform float _CoverIntensity;
		uniform half PWSF_GlobalSnowIntensity;
		uniform int _EnableCoverMap;
		uniform float _SSSPower;
		uniform float _SSSIntensity;
		uniform float _SSSDistortion;
		uniform int _SSSMode;
		uniform float4 _SSSColor;
		uniform float4 _Color;
		uniform int _EnableColorMask;
		uniform float4 _ColorMaskTint;
		uniform float _ColorMaskRange;
		uniform float _ColorMaskFuzziness;
		uniform float4 _SquareMaskTopXRightYBottomZLeftW;
		uniform int _EnableSquareMask;
		uniform float _ColorMaskAutumnShift;
		uniform float4 _SurfaceTint;
		uniform float _SurfaceSaturation;
		uniform int _DebugMode;
		uniform int _EnableDebug;
		uniform float _Metallic;
		uniform sampler2D _RoughnessTexture;
		uniform sampler2D _OcclusionMap;
		uniform float _OcclusionStrength;
		uniform float _Smoothness;
		uniform float _SmoothnessVariance;
		uniform float _SmoothnessThreshold;
		uniform int _SmoothnessMode;
		uniform int _RenderMode;
		uniform float _AlphaCutoffBias;
		uniform float _CrossfadeDistance;
		uniform float _CrossfadeDistanceExponent;
		uniform float _CrossfadeScale;
		uniform int _CrossfadeMode;


		float GetGeometricNormalVariance( float perceptualSmoothness , float3 geometricNormalWS , float screenSpaceVariance , float threshold )
		{
			#define PerceptualSmoothnessToRoughness(perceptualSmoothness) (1.0 - perceptualSmoothness) * (1.0 - perceptualSmoothness)
			#define RoughnessToPerceptualSmoothness(roughness) 1.0 - sqrt(roughness)
			float3 deltaU = ddx(geometricNormalWS);
			float3 deltaV = ddy(geometricNormalWS);
			float variance = screenSpaceVariance * (dot(deltaU, deltaU) + dot(deltaV, deltaV));
			float roughness = PerceptualSmoothnessToRoughness(perceptualSmoothness);
			// Ref: Geometry into Shading - http://graphics.pixar.com/library/BumpRoughness/paper.pdf - equation (3)
			float squaredRoughness = saturate(roughness * roughness + min(2.0 * variance, threshold * threshold)); // threshold can be really low, square the value for easier
			return RoughnessToPerceptualSmoothness(sqrt(squaredRoughness));
		}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		float2 voronoihash636( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi636( float2 v, float time, inout float2 id )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mr = 0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash636( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline float Dither8x8Bayer( int x, int y )
		{
			const float dither[ 64 ] = {
				 1, 49, 13, 61,  4, 52, 16, 64,
				33, 17, 45, 29, 36, 20, 48, 32,
				 9, 57,  5, 53, 12, 60,  8, 56,
				41, 25, 37, 21, 44, 28, 40, 24,
				 3, 51, 15, 63,  2, 50, 14, 62,
				35, 19, 47, 31, 34, 18, 46, 30,
				11, 59,  7, 55, 10, 58,  6, 54,
				43, 27, 39, 23, 42, 26, 38, 22};
			int r = y * 8 + x;
			return dither[r] / 64; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime51_g1701 = _Time.y * _WindMagnitude;
			float2 appendResult50_g1701 = (float2(ase_worldPos.x , ase_worldPos.z));
			float mulTime53_g1701 = _Time.y * _WindMagnitudeDetail;
			float simplePerlin2D60_g1701 = snoise( ( ( appendResult50_g1701 * _WindFrequencyDetail ) + mulTime53_g1701 ) );
			float temp_output_66_0_g1701 = ( ase_vertex3Pos.y * cos( ( ( ( ase_worldPos.x + ase_worldPos.z ) * _WindFrequency ) + mulTime51_g1701 ) ) * simplePerlin2D60_g1701 * ( _WindIntensity + PWSF_GlobalWindIntensity ) );
			float3 appendResult72_g1701 = (float3(temp_output_66_0_g1701 , 0.0 , temp_output_66_0_g1701));
			float3 rotatedValue79_g1701 = RotateAroundAxis( float3( 0,0,0 ), mul( float4( appendResult72_g1701 , 0.0 ), unity_ObjectToWorld ).xyz, float3( 0,1,0 ), ( _WindDirection * UNITY_PI ) );
			float temp_output_74_0_g1701 = ( saturate( ( ase_vertex3Pos.y + _WindVerticalMaskOffset ) ) * _WindVerticalMaskIntensity );
			float lerpResult103_g1701 = lerp( (float)1 , temp_output_74_0_g1701 , (float)_EnableWindVerticalMask);
			float temp_output_23_0_g1702 = ( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WindDistanceRange ) , 1.0 ) - _WindDistanceOffset );
			float temp_output_24_0_g1702 = saturate( temp_output_23_0_g1702 );
			float temp_output_78_0_g1701 = ( ( 1.0 - temp_output_24_0_g1702 ) * 1.0 );
			float lerpResult101_g1701 = lerp( (float)1 , temp_output_78_0_g1701 , (float)_EnableWindDistance);
			float3 Wind_Vertex_Offset161 = ( _EnableWindVegetation * ( rotatedValue79_g1701 * lerpResult103_g1701 * lerpResult101_g1701 ) );
			v.vertex.xyz += Wind_Vertex_Offset161;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float3 tex2DNode127 = UnpackScaleNormal( tex2D( _BumpMap, uv0_MainTex ), _NormalStrength );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 temp_output_4_0_g1697 = ase_worldNormal;
			float3 temp_output_14_0_g1697 = cross( ddy( ase_worldPos ) , temp_output_4_0_g1697 );
			float3 temp_output_9_0_g1697 = ddx( ase_worldPos );
			float dotResult21_g1697 = dot( temp_output_14_0_g1697 , temp_output_9_0_g1697 );
			float time636 = 0.0;
			float2 coords636 = uv0_MainTex * ( _AlbedoDetailScale * 50 );
			float2 id636 = 0;
			float voroi636 = voronoi636( coords636, time636,id636 );
			float temp_output_409_0 = ( ( 1.0 - voroi636 ) - 0.3 );
			float celldetail1106 = temp_output_409_0;
			float3 temp_cast_0 = (saturate( celldetail1106 )).xxx;
			float3 temp_output_1_0_g1696 = temp_cast_0;
			float3 temp_output_2_0_g1696 = ddx( temp_output_1_0_g1696 );
			float temp_output_2_0_g1697 = temp_output_1_0_g1696.x;
			float3 temp_output_7_0_g1696 = ddy( temp_output_1_0_g1696 );
			float ifLocalVar17_g1697 = 0;
			if( dotResult21_g1697 > 0.0 )
				ifLocalVar17_g1697 = 1.0;
			else if( dotResult21_g1697 == 0.0 )
				ifLocalVar17_g1697 = 0.0;
			else if( dotResult21_g1697 < 0.0 )
				ifLocalVar17_g1697 = -1.0;
			float3 normalizeResult23_g1697 = normalize( ( ( abs( dotResult21_g1697 ) * temp_output_4_0_g1697 ) - ( ( ( ( ( temp_output_1_0_g1696 + temp_output_2_0_g1696 ).x - temp_output_2_0_g1697 ) * temp_output_14_0_g1697 ) + ( ( ( temp_output_1_0_g1696 + temp_output_7_0_g1696 ).x - temp_output_2_0_g1697 ) * cross( temp_output_4_0_g1697 , temp_output_9_0_g1697 ) ) ) * ifLocalVar17_g1697 ) ) );
			float temp_output_23_0_g1675 = ( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _AlbedoDetailDistance ) , 1.0 ) - 0.0 );
			float temp_output_24_0_g1675 = saturate( temp_output_23_0_g1675 );
			float detaildistance656 = ( ( 1.0 - temp_output_24_0_g1675 ) * _AlbedoDetailIntensity );
			float3 lerpResult672 = lerp( float3(0.5,0.5,1) , normalizeResult23_g1697 , ( _NormalDetailStrength * detaildistance656 ));
			float3 lerpResult658 = lerp( tex2DNode127 , BlendNormals( tex2DNode127 , lerpResult672 ) , detaildistance656);
			float2 uv0_SurfaceTexture = i.uv_texcoord * _SurfaceTexture_ST.xy + _SurfaceTexture_ST.zw;
			float3 surface_map_normal931 = UnpackScaleNormal( tex2D( _SurfaceNormalTexture, uv0_SurfaceTexture ), _SurfaceNormalStrength );
			float2 uv_SurfaceTexture = i.uv_texcoord * _SurfaceTexture_ST.xy + _SurfaceTexture_ST.zw;
			float4 tex2DNode24_g1698 = tex2D( _SurfaceTexture, uv_SurfaceTexture );
			float simplePerlin3D20_g1698 = snoise( ase_worldPos*_SurfaceScatterScale );
			simplePerlin3D20_g1698 = simplePerlin3D20_g1698*0.5 + 0.5;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform47_g1698 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float HeightMask31_g1698 = saturate(pow(((( 1.0 - tex2DNode24_g1698.a )*saturate( ( simplePerlin3D20_g1698 - ( ( (transform47_g1698).w - _SurfaceBias ) / _SurfaceDistanceLength ) ) ))*4)+(saturate( ( simplePerlin3D20_g1698 - ( ( (transform47_g1698).w - _SurfaceBias ) / _SurfaceDistanceLength ) ) )*2),_SurfaceBlendStrength));
			float surface_map_alpha561 = ( saturate( HeightMask31_g1698 ) * _EnableSurfaceMap );
			float3 lerpResult501 = lerp( lerpResult658 , BlendNormals( surface_map_normal931 , lerpResult658 ) , surface_map_alpha561);
			float2 uv0_CoverTexture = i.uv_texcoord * _CoverTexture_ST.xy + _CoverTexture_ST.zw;
			float3 tex2DNode49_g1699 = UnpackScaleNormal( tex2D( _CoverNormalTexture, uv0_CoverTexture ), _CoverNormalStrength );
			float3 cover_map_normal618 = tex2DNode49_g1699;
			float2 uv_CoverTexture = i.uv_texcoord * _CoverTexture_ST.xy + _CoverTexture_ST.zw;
			float4 tex2DNode52_g1699 = tex2D( _CoverTexture, uv_CoverTexture );
			float temp_output_66_0_g1699 = ( _CoverIntensity + PWSF_GlobalSnowIntensity );
			float temp_output_59_0_g1699 = saturate( ( tex2DNode52_g1699.a * ( (WorldNormalVector( i , tex2DNode49_g1699 )).y + _CoverBias ) * temp_output_66_0_g1699 ) );
			float cover_map_alpha610 = ( temp_output_59_0_g1699 * _EnableCoverMap );
			float3 lerpResult611 = lerp( lerpResult501 , cover_map_normal618 , cover_map_alpha610);
			float3 appendResult785 = (float3(1.0 , 1.0 , -1.0));
			float3 switchResult782 = (((i.ASEVFace>0)?(lerpResult611):(( lerpResult611 * appendResult785 ))));
			float3 NORMAL_MOD159 = switchResult782;
			o.Normal = NORMAL_MOD159;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult66_g1703 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult71_g1703 = dot( ase_worldlightDir , normalizeResult66_g1703 );
			float SSSsimple98_g1703 = max( ( pow( dotResult71_g1703 , _SSSPower ) * _SSSIntensity ) , 0.0 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult18_g1703 = normalize( ( ase_worldlightDir + ( normalize( (WorldNormalVector( i , NORMAL_MOD159 )) ) * _SSSDistortion ) ) );
			float dotResult20_g1703 = dot( ase_worldViewDir , -normalizeResult18_g1703 );
			float SSSprecise99_g1703 = max( ( pow( saturate( dotResult20_g1703 ) , _SSSPower ) * _SSSIntensity ) , 0.0 );
			float lerpResult88_g1703 = lerp( SSSsimple98_g1703 , SSSprecise99_g1703 , (float)( _SSSMode - 1 ));
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode5 = tex2D( _MainTex, uv_MainTex );
			float3 temp_output_11_0 = ( (_Color).rgb * (tex2DNode5).rgb );
			float3 temp_cast_6 = (celldetail1106).xxx;
			float3 blendOpSrc378 = temp_cast_6;
			float3 blendOpDest378 = temp_output_11_0;
			float3 lerpResult661 = lerp( temp_output_11_0 , ( saturate( (( blendOpDest378 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest378 ) * ( 1.0 - blendOpSrc378 ) ) : ( 2.0 * blendOpDest378 * blendOpSrc378 ) ) )) , detaildistance656);
			float3 ALBEDO_MOD16 = lerpResult661;
			float ColorMask67 = ( _EnableColorMask * saturate( ( 1.0 - ( ( distance( _ColorMaskTint.rgb , ALBEDO_MOD16 ) - _ColorMaskRange ) / max( _ColorMaskFuzziness , 1E-05 ) ) ) ) );
			float2 break56_g1695 = i.uv_texcoord;
			float temp_output_45_0_g1695 = frac( break56_g1695.y );
			float temp_output_44_0_g1695 = frac( break56_g1695.x );
			float temp_output_54_0_g1695 = saturate( ( step( ( 1.0 - temp_output_45_0_g1695 ) , _SquareMaskTopXRightYBottomZLeftW.x ) + step( ( 1.0 - temp_output_44_0_g1695 ) , _SquareMaskTopXRightYBottomZLeftW.y ) + step( temp_output_45_0_g1695 , _SquareMaskTopXRightYBottomZLeftW.z ) + step( temp_output_44_0_g1695 , _SquareMaskTopXRightYBottomZLeftW.w ) ) );
			float lerpResult1255 = lerp( (float)1 , temp_output_54_0_g1695 , (float)_EnableSquareMask);
			float SquareMask72 = lerpResult1255;
			float3 autumn365 = ( ( ColorMask67 * SquareMask72 ) * ALBEDO_MOD16 );
			float3 break351 = autumn365;
			float temp_output_354_0 = sin( ( _ColorMaskAutumnShift * UNITY_PI ) );
			float2 appendResult353 = (float2(( break351.x * (0.3 + (( 1.0 - temp_output_354_0 ) - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) ) , break351.y));
			float3 hsvTorgb321 = RGBToHSV( float3( appendResult353 ,  0.0 ) );
			float3 hsvTorgb322 = HSVToRGB( float3(( hsvTorgb321.x - ( _ColorMaskAutumnShift / 3 ) ),( hsvTorgb321.y + saturate( ( _ColorMaskAutumnShift * UNITY_PI ) ) ),( hsvTorgb321.z + ( temp_output_354_0 / 40 ) )) );
			float2 AUTUMN_MOD366 = (hsvTorgb322).xy;
			float3 blendOpSrc337 = ALBEDO_MOD16;
			float3 blendOpDest337 = float3( AUTUMN_MOD366 ,  0.0 );
			float3 hsvTorgb29_g1698 = RGBToHSV( tex2DNode24_g1698.rgb );
			float3 hsvTorgb39_g1698 = HSVToRGB( float3(hsvTorgb29_g1698.x,( hsvTorgb29_g1698.y * _SurfaceSaturation ),hsvTorgb29_g1698.z) );
			float3 surface_map547 = ( (_SurfaceTint).rgb * hsvTorgb39_g1698 );
			float3 lerpResult497 = lerp( ( saturate( ( 1.0 - ( 1.0 - blendOpSrc337 ) * ( 1.0 - blendOpDest337 ) ) )) , surface_map547 , surface_map_alpha561);
			float3 cover_map158 = ( (tex2DNode52_g1699).rgb * temp_output_59_0_g1699 * temp_output_66_0_g1699 );
			float3 lerpResult620 = lerp( lerpResult497 , ( lerpResult497 + cover_map158 ) , cover_map_alpha610);
			float3 temp_output_95_0_g1703 = (saturate( lerpResult620 )).xyz;
			float3 blendOpSrc91_g1703 = ( lerpResult88_g1703 * (_SSSColor).rgb * temp_output_95_0_g1703 );
			float3 blendOpDest91_g1703 = temp_output_95_0_g1703;
			float3 lerpBlendMode91_g1703 = lerp(blendOpDest91_g1703,	max( blendOpSrc91_g1703, blendOpDest91_g1703 ),(float)saturate( _SSSMode ));
			float3 mainmix1093 = lerpBlendMode91_g1703;
			float _176_g1720 = SquareMask72;
			float _1171_g1720 = ColorMask67;
			float ENUM173_g1720 = (float)_DebugMode;
			float lerpResult265_g1720 = lerp( _176_g1720 , _1171_g1720 , saturate( ENUM173_g1720 ));
			float temp_output_74_0_g1701 = ( saturate( ( ase_vertex3Pos.y + _WindVerticalMaskOffset ) ) * _WindVerticalMaskIntensity );
			float VerticalMask155 = temp_output_74_0_g1701;
			float _2170_g1720 = VerticalMask155;
			float temp_output_263_0_g1720 = ( ENUM173_g1720 - 1.0 );
			float lerpResult258_g1720 = lerp( lerpResult265_g1720 , _2170_g1720 , saturate( temp_output_263_0_g1720 ));
			float temp_output_23_0_g1702 = ( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WindDistanceRange ) , 1.0 ) - _WindDistanceOffset );
			float temp_output_24_0_g1702 = saturate( temp_output_23_0_g1702 );
			float temp_output_78_0_g1701 = ( ( 1.0 - temp_output_24_0_g1702 ) * 1.0 );
			float DistanceMask973 = ( 1.0 - temp_output_78_0_g1701 );
			float _3169_g1720 = DistanceMask973;
			float lerpResult262_g1720 = lerp( lerpResult258_g1720 , _3169_g1720 , saturate( ( temp_output_263_0_g1720 - 1.0 ) ));
			half FOUR16_g1720 = lerpResult262_g1720;
			float3 temp_cast_15 = (FOUR16_g1720).xxx;
			float3 lerpResult1248 = lerp( mainmix1093 , temp_cast_15 , (float)_EnableDebug);
			o.Albedo = lerpResult1248;
			o.Metallic = _Metallic;
			float lerpResult763 = lerp( 1.0 , ( tex2D( _OcclusionMap, uv0_MainTex ).r * i.vertexColor.r ) , _OcclusionStrength);
			float perceptualSmoothness9_g1717 = _Smoothness;
			float3 geometricNormalWS9_g1717 = ase_worldNormal;
			float screenSpaceVariance9_g1717 = _SmoothnessVariance;
			float threshold9_g1717 = _SmoothnessThreshold;
			float localGetGeometricNormalVariance9_g1717 = GetGeometricNormalVariance( perceptualSmoothness9_g1717 , geometricNormalWS9_g1717 , screenSpaceVariance9_g1717 , threshold9_g1717 );
			float _176_g1719 = localGetGeometricNormalVariance9_g1717;
			float switchResult16_g1717 = (((i.ASEVFace>0)?(1.0):(localGetGeometricNormalVariance9_g1717)));
			float _1171_g1719 = switchResult16_g1717;
			float ENUM173_g1719 = (float)_SmoothnessMode;
			float lerpResult275_g1719 = lerp( _176_g1719 , _1171_g1719 , saturate( ENUM173_g1719 ));
			float switchResult7_g1717 = (((i.ASEVFace>0)?(localGetGeometricNormalVariance9_g1717):(0.0)));
			float _2170_g1719 = switchResult7_g1717;
			float lerpResult271_g1719 = lerp( lerpResult275_g1719 , _2170_g1719 , saturate( ( ENUM173_g1719 - 1.0 ) ));
			half THREE27_g1719 = lerpResult271_g1719;
			o.Smoothness = ( ( 1.0 - tex2D( _RoughnessTexture, uv0_MainTex ).r ) * lerpResult763 * THREE27_g1719 );
			o.Occlusion = lerpResult763;
			o.Alpha = 1;
			int _176_g1700 = 1;
			float4 maintexraw1007 = tex2DNode5;
			float4 break1009 = maintexraw1007;
			float _1171_g1700 = break1009.a;
			float ENUM173_g1700 = (float)_RenderMode;
			float lerpResult275_g1700 = lerp( (float)_176_g1700 , _1171_g1700 , saturate( ENUM173_g1700 ));
			float _2170_g1700 = max( max( break1009.r , break1009.g ) , break1009.b );
			float lerpResult271_g1700 = lerp( lerpResult275_g1700 , _2170_g1700 , saturate( ( ENUM173_g1700 - 1.0 ) ));
			half THREE27_g1700 = lerpResult271_g1700;
			float temp_output_506_0 = floor( _AlphaCutoffBias );
			float lerpResult507 = lerp( ( SquareMask72 * saturate( ( THREE27_g1700 + _AlphaCutoffBias ) ) ) , temp_output_506_0 , temp_output_506_0);
			float Opacity156 = lerpResult507;
			float temp_output_67_0_g1704 = Opacity156;
			float _176_g1707 = temp_output_67_0_g1704;
			float temp_output_23_0_g1705 = ( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _CrossfadeDistance ) , _CrossfadeDistanceExponent ) - ( 1.0 - _CrossfadeScale ) );
			float temp_output_24_0_g1705 = saturate( temp_output_23_0_g1705 );
			float temp_output_50_0_g1704 = ( temp_output_24_0_g1705 * 1.0 );
			float lerpResult62_g1704 = lerp( temp_output_67_0_g1704 , temp_output_50_0_g1704 , temp_output_50_0_g1704);
			float _1171_g1707 = lerpResult62_g1704;
			float ENUM173_g1707 = (float)_CrossfadeMode;
			float lerpResult265_g1707 = lerp( _176_g1707 , _1171_g1707 , saturate( ENUM173_g1707 ));
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen60_g1704 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither60_g1704 = Dither8x8Bayer( fmod(clipScreen60_g1704.x, 8), fmod(clipScreen60_g1704.y, 8) );
			float temp_output_7_0_g1706 = 0.15;
			dither60_g1704 = step( dither60_g1704, saturate( ( ( ( 1.0 - temp_output_50_0_g1704 ) - temp_output_7_0_g1706 ) / ( 0.85 - temp_output_7_0_g1706 ) ) ) );
			float _2170_g1707 = ( temp_output_67_0_g1704 * dither60_g1704 );
			float temp_output_263_0_g1707 = ( ENUM173_g1707 - 1.0 );
			float lerpResult258_g1707 = lerp( lerpResult265_g1707 , _2170_g1707 , saturate( temp_output_263_0_g1707 ));
			float2 clipScreen52_g1704 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither52_g1704 = Dither8x8Bayer( fmod(clipScreen52_g1704.x, 8), fmod(clipScreen52_g1704.y, 8) );
			float smoothstepResult46_g1704 = smoothstep( 0.0 , 1.0 , unity_LODFade.x);
			dither52_g1704 = step( dither52_g1704, ( smoothstepResult46_g1704 * smoothstepResult46_g1704 ) );
			float temp_output_55_0_g1704 = ( 1.0 - dither52_g1704 );
			float _3169_g1707 = ( lerpResult62_g1704 * saturate( ( temp_output_55_0_g1704 + temp_output_55_0_g1704 ) ) );
			float lerpResult262_g1707 = lerp( lerpResult258_g1707 , _3169_g1707 , saturate( ( temp_output_263_0_g1707 - 1.0 ) ));
			half FOUR16_g1707 = lerpResult262_g1707;
			float Crossfade740 = FOUR16_g1707;
			clip( Crossfade740 - _MaskClipValue );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

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
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.screenPosition;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.screenPosition = IN.customPack2.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
}
/*ASEBEGIN
Version=17300
2023;108.5;1664;832;3274.078;312.6983;1.866354;True;True
Node;AmplifyShaderEditor.CommentaryNode;400;-3570.965,-434.2505;Inherit;False;1177.213;369.9878;;10;410;394;636;376;1018;377;375;409;401;1106;PWSF Cell Detail;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;375;-3540.068,-248.8915;Inherit;False;Property;_AlbedoDetailScale;Albedo Detail Scale;13;0;Create;True;0;0;False;0;0;25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;377;-3472.408,-161.5538;Inherit;False;Constant;_Int9;Int 9;40;0;Create;True;0;0;False;0;50;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;376;-3304.622,-245.5539;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1018;-3393.653,-378.6428;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VoronoiNode;636;-3146.967,-375.1281;Inherit;False;0;0;1;0;1;False;1;False;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.OneMinusNode;394;-2962.8,-376.0763;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;655;-4461.895,-432.1198;Inherit;False;856.0299;250.9365;;4;656;404;647;1278;PWSF Detail Distance;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;3;-3881.546,-945.142;Inherit;False;1479.909;452.0119;;11;1007;16;661;378;660;11;1107;458;461;5;8;ALBEDO;0,0,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;410;-2962.08,-302.1712;Inherit;False;Constant;_Deviant;Deviant;38;0;Create;True;0;0;False;0;0.3;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-3839.171,-706.7532;Inherit;True;Property;_MainTex;Albedo;10;1;[HDR];Create;False;0;0;False;1;Header(ALBEDO MAP);-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;409;-2792.177,-377.7775;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;404;-4422.99,-291.6203;Inherit;False;Property;_AlbedoDetailIntensity;Albedo Detail Intensity;12;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-3790.282,-891.7299;Float;False;Property;_Color;Albedo Tint;11;1;[HDR];Create;False;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;647;-4424.873,-373.5028;Float;False;Property;_AlbedoDetailDistance;Albedo Detail Distance;14;0;Create;True;0;0;False;0;4;4;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;461;-3511.046,-706.319;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1278;-4130.864,-368.9982;Inherit;False;PWSF Distance;-1;;1675;ceab0bf395e37974392d70d82a6a4faf;1,31,2;4;25;FLOAT;32;False;26;FLOAT;1;False;27;FLOAT;0;False;42;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1106;-2639.325,-383.4865;Inherit;False;celldetail;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;458;-3517.004,-892.0193;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-3269.226,-818.0273;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1107;-3267.972,-704.4236;Inherit;False;1106;celldetail;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;656;-3833.992,-375.0079;Inherit;False;detaildistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;378;-3045.333,-781.7148;Inherit;False;Overlay;True;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;660;-3037.209,-656.4601;Inherit;False;656;detaildistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;661;-2800.281,-817.7834;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;19;-5997.362,531.7021;Inherit;False;1064.949;505.848;;8;67;1254;256;23;27;28;21;1279;Color Mask;0.972549,1,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-2620.531,-821.6086;Float;False;ALBEDO_MOD;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;1;-5994.022,79.19144;Inherit;False;1049.768;398.9689;;7;72;1256;1255;258;4;10;1280;PWSF Square Mask;0.9705882,1,0,1;0;0
Node;AmplifyShaderEditor.ColorNode;21;-5882.839,677.7546;Float;False;Property;_ColorMaskTint;Color Mask Tint;45;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;27;-5897.389,938.9346;Float;False;Property;_ColorMaskFuzziness;Color Mask Fuzziness;46;0;Create;True;0;0;False;0;0.03;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;10;-5951.649,289.7591;Float;False;Property;_SquareMaskTopXRightYBottomZLeftW;Square Mask Top X, Right Y, Bottom Z, Left W;50;0;Create;True;0;0;False;0;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;23;-5885.826,586.241;Inherit;False;16;ALBEDO_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-5953.399,157.849;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-5937.19,856.0947;Float;False;Property;_ColorMaskRange;Color Mask Range;47;0;Create;True;0;0;False;0;0;0.423;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1280;-5640.241,294.2701;Inherit;False;PWSF Square Mask;-1;;1695;dbbf5caad9aafae4db16f5960baa0ba4;0;5;25;FLOAT2;0,0;False;57;FLOAT;0;False;60;FLOAT;0;False;61;FLOAT;0;False;62;FLOAT;0;False;2;FLOAT;0;FLOAT;65
Node;AmplifyShaderEditor.IntNode;1256;-5577.517,140.7533;Inherit;False;Property;_EnableSquareMask;Enable Square Mask;49;0;Create;True;0;0;False;2;Header(SQUARE MASK);Toggle;0;0;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;256;-5555.779,608.4977;Inherit;False;Property;_EnableColorMask;Enable Color Mask;44;0;Create;True;0;0;False;2;Header(COLOR MASK);Toggle;0;1;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;1279;-5582.232,693.7624;Inherit;False;PWSF Color Mask v1.0;-1;;1694;c6ae49df77205934392748ab74b1e06e;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;258;-5494.161,217.2569;Inherit;False;Constant;_Int1;Int 1;38;0;Create;True;0;0;False;0;1;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1254;-5320.479,670.0306;Inherit;False;2;2;0;INT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1255;-5330.517,270.7532;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;79;-4461.034,-13.13935;Inherit;False;2066.708;481.4111;;20;1093;254;145;620;184;622;173;497;572;562;337;368;365;333;108;107;94;93;1238;1284;Main Mix;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-5169.575,665.4703;Float;False;ColorMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-5174.486,264.9335;Float;False;SquareMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-4396.569,359.5334;Inherit;False;72;SquareMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;370;-4899.373,536.4875;Inherit;False;2509.556;558.0143;#no blue;21;324;355;354;369;356;351;357;348;341;353;360;342;359;321;362;344;325;327;322;343;366;Autumn;1,0.5019608,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-4394.112,276.0626;Inherit;False;67;ColorMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-4399.487,51.77722;Inherit;False;16;ALBEDO_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-4029.759,279.7266;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;324;-4876.834,581.5218;Inherit;False;Property;_ColorMaskAutumnShift;Color Mask Autumn Shift;48;0;Create;True;0;0;False;1;Header(COLOR MASK SEASONS);0;0.259;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;355;-4569.807,586.4875;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;333;-3881.153,281.0453;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;354;-4387.752,587.8068;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;365;-3732.819,279.3979;Inherit;False;autumn;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;369;-4318.29,765.6353;Inherit;False;365;autumn;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;356;-4258.467,587.8068;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;357;-4067.504,588.7529;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.3;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;351;-4125.719,771.1697;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;1108;-5636.169,-992.6971;Inherit;False;1106;celldetail;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;348;-3869.877,772.7209;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;111;-5116.278,-1466.344;Inherit;False;2711.317;455.8878;;22;159;782;783;785;611;614;617;501;563;609;616;658;405;689;672;127;685;596;686;659;597;1242;NORMAL;0,0,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;669;-5439.06,-988.7162;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;360;-3551.013,970.8004;Inherit;False;Constant;_Int7;Int 7;41;0;Create;True;0;0;False;0;40;0;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;659;-5044.238,-1122.603;Inherit;False;656;detaildistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;353;-3711.319,770.5219;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;597;-5053.146,-1220.437;Float;False;Property;_NormalDetailStrength;Normal Detail Strength;17;0;Create;True;0;0;False;0;0.25;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1119;-5276.814,-989.1423;Inherit;False;PreparePerturbNormalHQ;-1;;1696;ce0790c3228f3654b818a19dd51453a4;0;1;1;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT3;4;FLOAT3;6;FLOAT;9
Node;AmplifyShaderEditor.IntNode;341;-4721.08,677.2591;Inherit;False;Constant;_Int6;Int 6;40;0;Create;True;0;0;False;0;3;0;0;1;INT;0
Node;AmplifyShaderEditor.RGBToHSVNode;321;-3541.102,752.404;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;1242;-5062.959,-1375.889;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;342;-4557.236,658.675;Inherit;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;596;-4819.316,-1395.919;Float;False;Property;_NormalStrength;Normal Strength;16;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;49;-4315.263,1155.981;Inherit;False;1905.541;350.5731;;14;507;156;109;506;189;225;221;217;1008;1009;212;213;233;192;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1007;-3508.813,-619.1028;Inherit;False;maintexraw;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;685;-4738.427,-1296.302;Inherit;False;Constant;_Vector0;Vector 0;49;0;Create;True;0;0;False;0;0.5,0.5,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;362;-3543.842,892.6512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1124;-4894.282,-989.5283;Inherit;False;PerturbNormalHQ;-1;;1697;45dff16e78a0685469fed8b5b46e4d96;0;4;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;686;-4689.458,-1141.326;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;595;-2349.176,-795.3361;Inherit;False;626.769;300.1909;;4;931;547;561;1316;PWSF Surface Map;0,0,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;359;-3403.343,953.0732;Inherit;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;344;-3259.896,829.1956;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1316;-2328.016,-703.1041;Inherit;False;PWSF Surface Map v1.0;26;;1698;ed7b7bb82b906954582fefabfe1172f9;0;0;3;FLOAT3;0;FLOAT3;44;FLOAT;41
Node;AmplifyShaderEditor.SimpleSubtractOpNode;325;-3259.691,730.5699;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;127;-4499.922,-1398.914;Inherit;True;Property;_BumpMap;Normal;15;3;[HDR];[NoScaleOffset];[Normal];Create;False;0;0;False;1;Header(NORMAL MAP);-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1008;-4285.349,1341.925;Inherit;False;1007;maintexraw;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;672;-4425.136,-1197.34;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;327;-3257.632,926.0892;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;405;-4178.835,-1249.394;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;689;-4179.08,-1144.072;Inherit;False;656;detaildistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;322;-3067.538,779.1349;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;60;-2349.13,-1180.487;Inherit;False;617.1411;314.8235;;4;158;618;610;1298;PWSF Cover Map v1.0;0,0,1,1;0;0
Node;AmplifyShaderEditor.BreakToComponentsNode;1009;-4078.347,1347.925;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;931;-2016.194,-646.5523;Inherit;False;surface_map_normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1298;-2300.285,-1054.867;Inherit;False;PWSF Cover Map v1.0;37;;1699;788eee1b2bbf0174bb805e3af535cba6;0;0;3;FLOAT3;0;FLOAT3;44;FLOAT;41
Node;AmplifyShaderEditor.RegisterLocalVarNode;561;-2015.835,-572.9171;Inherit;False;surface_map_alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;616;-4024.903,-1377.92;Inherit;False;931;surface_map_normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;212;-3822.348,1347.925;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;343;-2849.329,776.3534;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;658;-3937.157,-1269.241;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;366;-2632.817,776.4207;Inherit;False;AUTUMN_MOD;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;610;-1986.395,-962.5181;Inherit;False;cover_map_alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;233;-3726.348,1203.925;Float;False;Property;_RenderMode;Render Mode;2;1;[Enum];Create;True;3;Opaque;0;Cutout Alpha;1;Cutout Black;2;0;False;0;0;1;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;192;-3694.348,1283.925;Inherit;False;Constant;_Int3;Int 3;34;0;Create;True;0;0;False;0;1;0;0;1;INT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;618;-1996.351,-1047.543;Inherit;False;cover_map_normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;213;-3678.348,1363.925;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;609;-3754.24,-1372.612;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;563;-3856.161,-1127.929;Inherit;False;561;surface_map_alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;217;-3550.348,1411.925;Float;False;Property;_AlphaCutoffBias;Alpha Cutoff Bias;3;0;Create;True;0;0;False;0;0.49;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;368;-4403.671,130.9809;Inherit;False;366;AUTUMN_MOD;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;617;-3521.77,-1365.31;Inherit;False;618;cover_map_normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;501;-3475.127,-1270.711;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;547;-2012.932,-759.0607;Inherit;False;surface_map;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;614;-3563.025,-1127.517;Inherit;False;610;cover_map_alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1283;-3513.66,1260.931;Inherit;False;PWSF Enum Switch;-1;;1700;3e9cc2aefa7c15f46bb8ad0edeb00b30;1,34,1;9;134;FLOAT;0;False;146;INT;0;False;133;FLOAT;0;False;144;FLOAT;0;False;143;FLOAT;0;False;155;FLOAT;0;False;156;FLOAT;0;False;150;FLOAT;0;False;158;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;337;-4102.295,51.90866;Inherit;False;Screen;True;3;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-1959.308,-1134.708;Float;False;cover_map;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;611;-3263.092,-1270.045;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;572;-3859.694,87.71906;Inherit;False;547;surface_map;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;785;-3242.002,-1141.712;Inherit;False;FLOAT3;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;-1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;221;-3262.348,1315.925;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;562;-3891.354,171.1239;Inherit;False;561;surface_map_alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-3619.812,190.2615;Inherit;False;158;cover_map;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;783;-3067.972,-1199.979;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;497;-3616.679,70.99963;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-3182.348,1235.925;Inherit;False;72;SquareMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;225;-3118.348,1315.925;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;184;-3394.465,140.9555;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwitchByFaceNode;782;-2897.816,-1269.04;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FloorOpNode;506;-2942.348,1395.925;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;622;-3511.394,280.5636;Inherit;False;610;cover_map_alpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-2958.348,1299.925;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;764;-2271.465,605.8248;Inherit;False;971.1494;396.675;;4;748;749;755;1244;Smoothness;0,0.8758622,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;507;-2798.348,1299.925;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;826;-4936.474,1151.544;Inherit;False;529.8325;330.5134;;4;161;973;155;1300;PWSF Wind v1.0;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;620;-3238.54,119.2341;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-2691.649,-1273.829;Float;False;NORMAL_MOD;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1244;-2235.011,678.157;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;1300;-4910.61,1267.688;Inherit;False;PWSF Wind v1.0;57;;1701;d81bd1e3218b8f445976d025bdadac4f;0;0;3;FLOAT3;0;FLOAT;89;FLOAT;88
Node;AmplifyShaderEditor.SaturateNode;145;-3075.094,120.2372;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1238;-3172.678,249.1641;Inherit;False;159;NORMAL_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;769;-2186.237,1051.561;Inherit;False;875.438;413.129;;5;756;747;762;757;763;Ambient Occlusion;0,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-2636.928,1294.244;Float;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;739;-2369.51,1849.108;Inherit;False;678.9863;137.2309;;3;740;737;1286;PWSF Crossfade;0,0.9172413,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;1284;-2912.705,120.0556;Inherit;False;PWSF Subsurface Scattering;51;;1703;1def9532c018b8d4ebbd5288949bcd3c;0;2;94;FLOAT3;1,1,1;False;45;FLOAT3;0.5,0.5,1;False;1;FLOAT3;100
Node;AmplifyShaderEditor.GetLocalVarNode;737;-2336.378,1895.073;Inherit;False;156;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-4632.229,1291.269;Float;False;VerticalMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;114;-2317.896,63.7055;Inherit;False;769.2375;474.2277;;8;144;164;150;132;976;1101;1248;1250;Debug;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;973;-4632.229,1371.269;Float;False;DistanceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;747;-2135.236,1101.561;Inherit;True;Property;_OcclusionMap;Occlusion Map;24;1;[NoScaleOffset];Create;False;0;0;False;1;Header(AMBIENT OCCLUSION MAP);-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;756;-1837.553,1192.175;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;1272;-2360.263,1518.033;Inherit;False;890.2716;270.4436;http://graphics.pixar.com/library/BumpRoughness/paper.pdf - equation (3);2;1276;1318;PWSF Smoothness Geometric Specular Anti-aliasing v3.0;0,0.4024811,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-2268.808,281.9183;Inherit;False;67;ColorMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;762;-1653.566,1128.079;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;1276;-2057.998,1570.681;Float;False;Property;_SmoothnessMode;Smoothness Mode;19;1;[Enum];Create;True;3;Smoothness Cull Off;0;Smoothness Cull Front;1;Smoothness Cull Back;2;0;False;1;Header (SMOOTHNESS);0;0;0;1;INT;0
Node;AmplifyShaderEditor.SamplerNode;755;-1964.361,655.3339;Inherit;True;Property;_RoughnessTexture;Roughness Texture;18;1;[NoScaleOffset];Create;True;0;0;False;1;Header(ROUGHNESS MAP);-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;757;-1919.597,1363.69;Float;False;Property;_OcclusionStrength;Occlusion Strength;25;0;Create;False;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1318;-2288.827,1651.964;Inherit;False;PWSF Smoothness Geometric Specular v3.0;20;;1717;87fb58cae055bea429a44169a3c936fd;0;1;25;FLOAT;0;False;3;FLOAT;17;FLOAT;20;FLOAT;21
Node;AmplifyShaderEditor.GetLocalVarNode;976;-2286.675,435.9748;Inherit;False;973;DistanceMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;164;-2282.479,359.9869;Inherit;False;155;VerticalMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1286;-2142.534,1897.973;Inherit;False;PWSF Crossfade;5;;1704;bafe90066fae854429c3522bf9848111;0;1;67;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-2281.016,201.4379;Inherit;False;72;SquareMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1093;-2610.804,115.3474;Inherit;False;mainmix;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;132;-2251.699,119.175;Float;False;Property;_DebugMode;Debug Mode;72;1;[Enum];Create;True;4;Square Mask;0;Color Mask;1;Vertical Mask;2;Distance Mask;3;0;False;0;0;3;0;1;INT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;740;-1908.79,1894.287;Inherit;False;Crossfade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;749;-1644.476,683.16;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1101;-2029.196,128.3626;Inherit;False;1093;mainmix;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;1250;-1990.057,375.5704;Inherit;False;Property;_EnableDebug;Enable Debug;71;0;Create;True;0;0;False;2;Toggle;Header(DEBUG);0;0;0;1;INT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;161;-4675.229,1211.269;Float;False;Wind_Vertex_Offset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;401;-2653.796,-255.7144;Inherit;False;231.5158;170.8735;experimental leave disease;1;395;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;1288;-2030.429,209.1758;Inherit;False;PWSF Enum Switch;-1;;1720;3e9cc2aefa7c15f46bb8ad0edeb00b30;1,34,2;9;134;FLOAT;0;False;146;FLOAT;0;False;133;FLOAT;0;False;144;FLOAT;0;False;143;FLOAT;0;False;155;FLOAT;0;False;156;FLOAT;0;False;150;FLOAT;0;False;158;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1287;-1804.13,1616.776;Inherit;False;PWSF Enum Switch;-1;;1719;3e9cc2aefa7c15f46bb8ad0edeb00b30;1,34,1;9;134;FLOAT;0;False;146;FLOAT;0;False;133;FLOAT;0;False;144;FLOAT;0;False;143;FLOAT;0;False;155;FLOAT;0;False;156;FLOAT;0;False;150;FLOAT;0;False;158;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;763;-1494.795,1104.743;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;765;-1482.127,434.5316;Inherit;False;740;Crossfade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;748;-1455.425,681.5847;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;254;-4190.613,247.1877;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;424;-1472,352;Inherit;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;0.1;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;202;-1152,64;Inherit;False;Property;_MaskClipValue;Mask Clip Value;0;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;172;-1472,272;Inherit;False;159;NORMAL_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1310;-1436.531,599.5502;Inherit;False;Constant;_AlphaClip1;Alpha Clip;31;0;Create;True;0;0;False;0;0.49;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;203;-1152,144;Inherit;False;Property;_CullMode;Cull Mode;1;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CullMode;True;1;Header(GLOBAL SETTINGS);0;0;0;1;INT;0
Node;AmplifyShaderEditor.LerpOp;1248;-1764.129,133.5704;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-1472,512;Inherit;False;161;Wind_Vertex_Offset;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendOpsNode;395;-2628.847,-214.8638;Inherit;False;Darken;True;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;1319;-1152,224;Float;False;True;-1;2;;0;0;Standard;PWS/Procedural/Vegetation SP 2019_02 v3.1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;203;-1;0;True;202;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;376;0;375;0
WireConnection;376;1;377;0
WireConnection;636;0;1018;0
WireConnection;636;2;376;0
WireConnection;394;0;636;0
WireConnection;409;0;394;0
WireConnection;409;1;410;0
WireConnection;461;0;5;0
WireConnection;1278;25;647;0
WireConnection;1278;42;404;0
WireConnection;1106;0;409;0
WireConnection;458;0;8;0
WireConnection;11;0;458;0
WireConnection;11;1;461;0
WireConnection;656;0;1278;0
WireConnection;378;0;1107;0
WireConnection;378;1;11;0
WireConnection;661;0;11;0
WireConnection;661;1;378;0
WireConnection;661;2;660;0
WireConnection;16;0;661;0
WireConnection;1280;25;4;0
WireConnection;1280;57;10;1
WireConnection;1280;60;10;2
WireConnection;1280;61;10;3
WireConnection;1280;62;10;4
WireConnection;1279;1;23;0
WireConnection;1279;3;21;0
WireConnection;1279;4;28;0
WireConnection;1279;5;27;0
WireConnection;1254;0;256;0
WireConnection;1254;1;1279;0
WireConnection;1255;0;258;0
WireConnection;1255;1;1280;0
WireConnection;1255;2;1256;0
WireConnection;67;0;1254;0
WireConnection;72;0;1255;0
WireConnection;107;0;94;0
WireConnection;107;1;93;0
WireConnection;355;0;324;0
WireConnection;333;0;107;0
WireConnection;333;1;108;0
WireConnection;354;0;355;0
WireConnection;365;0;333;0
WireConnection;356;0;354;0
WireConnection;357;0;356;0
WireConnection;351;0;369;0
WireConnection;348;0;351;0
WireConnection;348;1;357;0
WireConnection;669;0;1108;0
WireConnection;353;0;348;0
WireConnection;353;1;351;1
WireConnection;1119;1;669;0
WireConnection;321;0;353;0
WireConnection;342;0;324;0
WireConnection;342;1;341;0
WireConnection;1007;0;5;0
WireConnection;362;0;355;0
WireConnection;1124;1;1119;0
WireConnection;1124;2;1119;4
WireConnection;1124;3;1119;6
WireConnection;686;0;597;0
WireConnection;686;1;659;0
WireConnection;359;0;354;0
WireConnection;359;1;360;0
WireConnection;344;0;321;2
WireConnection;344;1;362;0
WireConnection;325;0;321;1
WireConnection;325;1;342;0
WireConnection;127;1;1242;0
WireConnection;127;5;596;0
WireConnection;672;0;685;0
WireConnection;672;1;1124;0
WireConnection;672;2;686;0
WireConnection;327;0;321;3
WireConnection;327;1;359;0
WireConnection;405;0;127;0
WireConnection;405;1;672;0
WireConnection;322;0;325;0
WireConnection;322;1;344;0
WireConnection;322;2;327;0
WireConnection;1009;0;1008;0
WireConnection;931;0;1316;44
WireConnection;561;0;1316;41
WireConnection;212;0;1009;0
WireConnection;212;1;1009;1
WireConnection;343;0;322;0
WireConnection;658;0;127;0
WireConnection;658;1;405;0
WireConnection;658;2;689;0
WireConnection;366;0;343;0
WireConnection;610;0;1298;41
WireConnection;618;0;1298;44
WireConnection;213;0;212;0
WireConnection;213;1;1009;2
WireConnection;609;0;616;0
WireConnection;609;1;658;0
WireConnection;501;0;658;0
WireConnection;501;1;609;0
WireConnection;501;2;563;0
WireConnection;547;0;1316;0
WireConnection;1283;134;233;0
WireConnection;1283;146;192;0
WireConnection;1283;133;1009;3
WireConnection;1283;144;213;0
WireConnection;337;0;108;0
WireConnection;337;1;368;0
WireConnection;158;0;1298;0
WireConnection;611;0;501;0
WireConnection;611;1;617;0
WireConnection;611;2;614;0
WireConnection;221;0;1283;0
WireConnection;221;1;217;0
WireConnection;783;0;611;0
WireConnection;783;1;785;0
WireConnection;497;0;337;0
WireConnection;497;1;572;0
WireConnection;497;2;562;0
WireConnection;225;0;221;0
WireConnection;184;0;497;0
WireConnection;184;1;173;0
WireConnection;782;0;611;0
WireConnection;782;1;783;0
WireConnection;506;0;217;0
WireConnection;189;0;109;0
WireConnection;189;1;225;0
WireConnection;507;0;189;0
WireConnection;507;1;506;0
WireConnection;507;2;506;0
WireConnection;620;0;497;0
WireConnection;620;1;184;0
WireConnection;620;2;622;0
WireConnection;159;0;782;0
WireConnection;145;0;620;0
WireConnection;156;0;507;0
WireConnection;1284;94;145;0
WireConnection;1284;45;1238;0
WireConnection;155;0;1300;89
WireConnection;973;0;1300;88
WireConnection;747;1;1244;0
WireConnection;762;0;747;1
WireConnection;762;1;756;1
WireConnection;755;1;1244;0
WireConnection;1286;67;737;0
WireConnection;1093;0;1284;100
WireConnection;740;0;1286;0
WireConnection;749;0;755;1
WireConnection;161;0;1300;0
WireConnection;1288;134;132;0
WireConnection;1288;146;144;0
WireConnection;1288;133;150;0
WireConnection;1288;144;164;0
WireConnection;1288;143;976;0
WireConnection;1287;134;1276;0
WireConnection;1287;146;1318;17
WireConnection;1287;133;1318;20
WireConnection;1287;144;1318;21
WireConnection;763;1;762;0
WireConnection;763;2;757;0
WireConnection;748;0;749;0
WireConnection;748;1;763;0
WireConnection;748;2;1287;0
WireConnection;254;0;94;0
WireConnection;1248;0;1101;0
WireConnection;1248;1;1288;0
WireConnection;1248;2;1250;0
WireConnection;395;0;11;0
WireConnection;395;1;409;0
WireConnection;1319;0;1248;0
WireConnection;1319;1;172;0
WireConnection;1319;3;424;0
WireConnection;1319;4;748;0
WireConnection;1319;5;763;0
WireConnection;1319;10;765;0
WireConnection;1319;11;170;0
ASEEND*/
//CHKSM=355C5E0A10FB1366AC47ECFC70EC067859107D32