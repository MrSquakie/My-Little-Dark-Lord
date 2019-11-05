// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PWS/Procedural/Vegetation SP v1.3 2019_01_14"
{
	Properties
	{
		[HideInInspector]_MaskClipValue("Mask Clip Value", Range( 0 , 1)) = 1
		[Enum(UnityEngine.Rendering.CullMode)][Header(GLOBAL SETTINGS)]_CullMode("Cull Mode", Int) = 0
		[Enum(Opaque,0,Cutout Alpha,1,Cutout Black,2)]_RenderMode("Render Mode", Int) = 0
		_AlphaCutoffBias("Alpha Cutoff Bias", Range( 0 , 1)) = 0.49
		_Metallic("Metallic", Range( 0 , 1)) = 0.1
		[Enum(OFF,0,Distance,1,Distance Dither,2,Distance Dither LOD,3)][Header(CROSSFADE)]_CrossfadeMode("Crossfade Mode", Int) = 0
		_CrossfadeDistance("Crossfade Distance", Range( 0 , 128)) = 64
		_CrossfadeDistanceExponent("Crossfade Distance Exponent", Range( 0 , 16)) = 15.74425
		_CrossfadeScale("Crossfade Scale", Range( 0 , 1)) = 0.8
		[HDR][Header(ALBEDO MAP)]_MainTex("MainTex", 2D) = "white" {}
		[HDR]_Color("Albedo Tint", Color) = (1,1,1,1)
		_AlbedoDetailIntensity("Albedo Detail Intensity", Range( 0 , 1)) = 0
		_AlbedoDetailScale("Albedo Detail Scale", Float) = 0
		_AlbedoDetailDistance("Albedo Detail Distance", Range( 0 , 4)) = 4
		[HDR][NoScaleOffset][Normal][Header(NORMAL MAP)]_NormalTexture("Normal Texture", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range( 0 , 1)) = 1
		_NormalDetailStrength("Normal Detail Strength", Range( 0 , 1)) = 0.25
		[NoScaleOffset][Header(ROUGHNESS MAP)]_RoughnessTexture("Roughness Texture", 2D) = "white" {}
		_Smoothness1("Smoothness", Range( 0 , 1)) = 1
		_SmoothnessVariance1("Smoothness Variance", Range( 0 , 1)) = 0.5
		_SmoothnessThreshold1("Smoothness Threshold", Range( 0 , 1)) = 0
		[NoScaleOffset][Header(AMBIENT OCCLUSION MAP)]_AmbientOcclusionTexture("Ambient Occlusion Texture", 2D) = "white" {}
		_AmbientOcclusion("Ambient Occlusion", Range( 0 , 1)) = 0.5
		[Header(COLOR MASK)][Toggle]_EnableColorMask("Enable Color Mask", Float) = 1
		[HDR]_ColorMaskTint("Color Mask Tint", Color) = (0,0,0,0)
		_ColorMaskFuzziness("Color Mask Fuzziness", Float) = 0.03
		_ColorMaskRange("Color Mask Range", Range( 0 , 1)) = 0
		[Header(COLOR MASK SEASONS)]_ColorMaskAutumnShift("Color Mask Autumn Shift", Range( 0 , 1)) = 0
		[Header(SQUARE MASK)][Toggle]_EnableSquareMask("Enable Square Mask", Float) = 0
		[Toggle]_SquareMaskInvert("Square Mask Invert", Float) = 0
		_SquareMaskTopXRightYBottomZLeftW("Square Mask Top X, Right Y, Bottom Z, Left W", Vector) = (0,0,0,0)
		[Header(SURFACE MAP)][Toggle]_EnableSurfaceMap("Enable Surface Map", Float) = 1
		_SurfaceTexture("Surface Texture", 2D) = "white" {}
		_SurfaceMapTilling("Surface Map Tilling", Float) = 1
		_SurfaceTint("Surface Tint", Color) = (1,1,1,0)
		_SurfaceSaturation("Surface Saturation", Range( 0 , 1)) = 0.5
		_SurfaceScatterScale("Surface Scatter Scale", Range( 0 , 2)) = 1.5
		_SurfaceBlendStrength("Surface Blend Strength", Range( 0 , 16)) = 0
		_SurfaceBias("Surface Bias", Range( -1 , 1)) = -0.25
		_SurfaceDistanceLength("Surface Distance Length", Range( 0 , 5)) = 0.75
		[HDR][NoScaleOffset][Normal]_SurfaceNormalTexture("Surface Normal Texture", 2D) = "bump" {}
		_SurfaceNormalStrength("Surface Normal Strength", Range( 0 , 1)) = 1
		[Header(COVER MAP)][Toggle]_EnableCoverMap("Enable Cover Map", Float) = 0
		[Toggle]_PWSFGlobalControllerSnow1("PWSF Global Controller Snow", Float) = 0
		[HDR]_CoverTexture("Cover Texture", 2D) = "white" {}
		_CoverMapTilling("Cover Map Tilling", Float) = 1
		_CoverIntensity("Cover Intensity", Range( 0 , 10)) = 2
		_CoverBias("Cover Bias", Range( -1 , 1)) = 0
		[HDR][NoScaleOffset][Normal]_CoverNormalTexture("Cover Normal Texture", 2D) = "bump" {}
		_CoverNormalStrength("Cover Normal Strength", Range( 0 , 1)) = 1
		[Header(WIND)][Toggle]_EnableWind("Enable Wind", Float) = 0
		[Toggle]_PWSFGlobalControllerWind("PWSF Global Controller Wind ", Float) = 0
		_WindIntensity("Wind Intensity", Range( 0 , 0.1)) = 0.0094
		_WindPulseFrequency("Wind Pulse Frequency", Range( 0 , 10)) = 8
		_WindPulseMagnitude("Wind Pulse Magnitude", Range( 0 , 1)) = 1
		_WindPulseFrequencyDetail("Wind Pulse Frequency Detail", Range( 0 , 5)) = 0.5
		_WindPulseMagnitudeDetail("Wind Pulse Magnitude Detail", Range( 0 , 1)) = 1
		_WindDirection("Wind Direction", Range( 0 , 2)) = 0.062
		_WindBlendRange("Wind Blend Range", Range( 0 , 32)) = 6.6
		[PowerSlider(3)]_WindBoxFalloff("Wind Box Falloff", Range( 0.001 , 10)) = 0.05
		_WindBoxPosition("Wind Box Position", Vector) = (0,0,0,0)
		_WindBoxSize("Wind Box Size", Vector) = (0,0,0,0)
		[Header(DEBUG)][Toggle]_EnableDebug("Enable Debug", Float) = 0
		[Enum(Square Mask,0,Color Mask,1,Box Mask,2)]_DebugMode("Debug Mode", Int) = 0
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
		uniform float _EnableWind;
		uniform float _WindDirection;
		uniform float _WindPulseFrequency;
		uniform float _WindPulseMagnitude;
		uniform float _WindPulseFrequencyDetail;
		uniform float _WindPulseMagnitudeDetail;
		uniform float _PWSFGlobalControllerWind;
		uniform float _WindIntensity;
		uniform half PWSF_GlobalWindIntensity;
		uniform float3 _WindBoxPosition;
		uniform float3 _WindBoxSize;
		uniform float _WindBoxFalloff;
		uniform float _WindBlendRange;
		uniform float _NormalStrength;
		uniform sampler2D _NormalTexture;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _AlbedoDetailScale;
		uniform float _NormalDetailStrength;
		uniform float _AlbedoDetailDistance;
		uniform float _AlbedoDetailIntensity;
		uniform float _SurfaceNormalStrength;
		uniform sampler2D _SurfaceNormalTexture;
		uniform float _SurfaceMapTilling;
		uniform float _EnableSurfaceMap;
		uniform sampler2D _SurfaceTexture;
		uniform float _SurfaceScatterScale;
		uniform float _SurfaceBias;
		uniform float _SurfaceDistanceLength;
		uniform float _SurfaceBlendStrength;
		uniform float _CoverNormalStrength;
		uniform sampler2D _CoverNormalTexture;
		uniform float _CoverMapTilling;
		uniform float _EnableCoverMap;
		uniform sampler2D _CoverTexture;
		uniform float _CoverBias;
		uniform float _PWSFGlobalControllerSnow1;
		uniform float _CoverIntensity;
		uniform half PWSF_GlobalSnowIntensity1;
		uniform float _EnableDebug;
		uniform float4 _Color;
		uniform float _EnableColorMask;
		uniform float4 _ColorMaskTint;
		uniform float _ColorMaskRange;
		uniform float _ColorMaskFuzziness;
		uniform float _EnableSquareMask;
		uniform float _SquareMaskInvert;
		uniform float4 _SquareMaskTopXRightYBottomZLeftW;
		uniform float _ColorMaskAutumnShift;
		uniform float4 _SurfaceTint;
		uniform float _SurfaceSaturation;
		uniform int _DebugMode;
		uniform float _Metallic;
		uniform sampler2D _RoughnessTexture;
		uniform sampler2D _AmbientOcclusionTexture;
		uniform float _AmbientOcclusion;
		uniform float _Smoothness1;
		uniform float _SmoothnessVariance1;
		uniform float _SmoothnessThreshold1;
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
			float3 temp_cast_0 = 0;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime31 = _Time.y * _WindPulseMagnitude;
			float2 appendResult26 = (float2(ase_worldPos.x , ase_worldPos.z));
			float mulTime819 = _Time.y * _WindPulseMagnitudeDetail;
			float simplePerlin2D48 = snoise( ( ( appendResult26 * _WindPulseFrequencyDetail ) + mulTime819 ) );
			float temp_output_71_0 = ( ( ase_vertex3Pos.y * cos( ( ( ( ase_worldPos.x + ase_worldPos.z ) * _WindPulseFrequency ) + mulTime31 ) ) * simplePerlin2D48 ) * lerp(_WindIntensity,( PWSF_GlobalWindIntensity * _WindIntensity ),_PWSFGlobalControllerWind) );
			float3 appendResult86 = (float3(temp_output_71_0 , 0.0 , temp_output_71_0));
			float3 rotatedValue141 = RotateAroundAxis( float3( 0,0,0 ), mul( float4( appendResult86 , 0.0 ), unity_ObjectToWorld ).xyz, float3( 0,1,0 ), ( _WindDirection * UNITY_PI ) );
			float temp_output_125_0 = saturate( ( distance( max( ( abs( ( ase_vertex3Pos + _WindBoxPosition ) ) - ( _WindBoxSize * 0.5 ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / _WindBoxFalloff ) );
			float3 Wind_Vertex_Offset161 = lerp(temp_cast_0,( rotatedValue141 * temp_output_125_0 * (( _WindBlendRange == 0.0 ) ? (float)0 :  saturate( ( 1.0 - ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _WindBlendRange ) ) ) ) ),_EnableWind);
			v.vertex.xyz += Wind_Vertex_Offset161;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float3 tex2DNode127 = UnpackScaleNormal( tex2D( _NormalTexture, uv0_MainTex ), _NormalStrength );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 temp_output_4_0_g78 = ase_worldNormal;
			float3 temp_output_14_0_g78 = cross( ddy( ase_worldPos ) , temp_output_4_0_g78 );
			float3 temp_output_9_0_g78 = ddx( ase_worldPos );
			float dotResult21_g78 = dot( temp_output_14_0_g78 , temp_output_9_0_g78 );
			float time636 = 0.0;
			float2 coords636 = uv0_MainTex * ( _AlbedoDetailScale * 50 );
			float2 id636 = 0;
			float voroi636 = voronoi636( coords636, time636,id636 );
			float temp_output_409_0 = ( ( 1.0 - voroi636 ) - 0.3 );
			float3 temp_cast_0 = (saturate( temp_output_409_0 )).xxx;
			float3 temp_output_1_0_g77 = temp_cast_0;
			float3 temp_output_2_0_g77 = ddx( temp_output_1_0_g77 );
			float temp_output_2_0_g78 = temp_output_1_0_g77.x;
			float3 temp_output_7_0_g77 = ddy( temp_output_1_0_g77 );
			float ifLocalVar17_g78 = 0;
			if( dotResult21_g78 > 0.0 )
				ifLocalVar17_g78 = 1.0;
			else if( dotResult21_g78 == 0.0 )
				ifLocalVar17_g78 = 0.0;
			else if( dotResult21_g78 < 0.0 )
				ifLocalVar17_g78 = -1.0;
			float3 normalizeResult23_g78 = normalize( ( ( abs( dotResult21_g78 ) * temp_output_4_0_g78 ) - ( ( ( ( ( temp_output_1_0_g77 + temp_output_2_0_g77 ).x - temp_output_2_0_g78 ) * temp_output_14_0_g78 ) + ( ( ( temp_output_1_0_g77 + temp_output_7_0_g77 ).x - temp_output_2_0_g78 ) * cross( temp_output_4_0_g78 , temp_output_9_0_g78 ) ) ) * ifLocalVar17_g78 ) ) );
			float detaildistance656 = ( saturate( ( 1.0 - ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _AlbedoDetailDistance ) ) ) * _AlbedoDetailIntensity );
			float3 lerpResult672 = lerp( float3(0.5,0.5,1) , normalizeResult23_g78 , ( _NormalDetailStrength * detaildistance656 ));
			float3 lerpResult658 = lerp( tex2DNode127 , BlendNormals( tex2DNode127 , lerpResult672 ) , detaildistance656);
			float2 temp_cast_4 = (_SurfaceMapTilling).xx;
			float2 temp_cast_5 = (1.0).xx;
			float2 uv_TexCoord914 = i.uv_texcoord * temp_cast_4 + temp_cast_5;
			float3 surface_normal615 = UnpackScaleNormal( tex2D( _SurfaceNormalTexture, uv_TexCoord914 ), _SurfaceNormalStrength );
			float4 tex2DNode472 = tex2D( _SurfaceTexture, uv_TexCoord914 );
			float simplePerlin3D530 = snoise( ase_worldPos*_SurfaceScatterScale );
			simplePerlin3D530 = simplePerlin3D530*0.5 + 0.5;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float HeightMask480 = saturate(pow(((( 1.0 - tex2DNode472.a )*saturate( ( simplePerlin3D530 - ( ( ase_vertex3Pos.y - _SurfaceBias ) / _SurfaceDistanceLength ) ) ))*4)+(saturate( ( simplePerlin3D530 - ( ( ase_vertex3Pos.y - _SurfaceBias ) / _SurfaceDistanceLength ) ) )*2),_SurfaceBlendStrength));
			float SURFACE_BLEND_MOD561 = lerp(0.0,saturate( HeightMask480 ),_EnableSurfaceMap);
			float3 lerpResult501 = lerp( lerpResult658 , BlendNormals( surface_normal615 , lerpResult658 ) , SURFACE_BLEND_MOD561);
			float2 temp_cast_6 = (_CoverMapTilling).xx;
			float2 temp_cast_7 = (1.0).xx;
			float2 uv_TexCoord917 = i.uv_texcoord * temp_cast_6 + temp_cast_7;
			float3 tex2DNode601 = UnpackScaleNormal( tex2D( _CoverNormalTexture, uv_TexCoord917 ), _CoverNormalStrength );
			float3 cover_normal618 = tex2DNode601;
			float4 tex2DNode118 = tex2D( _CoverTexture, uv_TexCoord917 );
			float switchResult910 = (((i.ASEVFace>0)?(1.0):(tex2DNode118.a)));
			float temp_output_185_0 = saturate( ( switchResult910 * ( (WorldNormalVector( i , tex2DNode601 )).y + _CoverBias ) * lerp(_CoverIntensity,( PWSF_GlobalSnowIntensity1 * _CoverIntensity ),_PWSFGlobalControllerSnow1) ) );
			float COVER_BLEND_MOD610 = lerp((float)0,temp_output_185_0,_EnableCoverMap);
			float3 lerpResult611 = lerp( lerpResult501 , cover_normal618 , COVER_BLEND_MOD610);
			float3 appendResult785 = (float3(1.0 , 1.0 , -1.0));
			float3 switchResult782 = (((i.ASEVFace>0)?(lerpResult611):(( lerpResult611 * appendResult785 ))));
			float3 NORMAL_MOD159 = switchResult782;
			o.Normal = NORMAL_MOD159;
			float3 temp_output_11_0 = ( (_Color).rgb * (tex2D( _MainTex, uv0_MainTex )).rgb );
			float3 temp_cast_9 = (temp_output_409_0).xxx;
			float3 blendOpSrc378 = temp_cast_9;
			float3 blendOpDest378 = temp_output_11_0;
			float3 lerpResult661 = lerp( temp_output_11_0 , ( saturate( (( blendOpDest378 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest378 ) * ( 1.0 - blendOpSrc378 ) ) : ( 2.0 * blendOpDest378 * blendOpSrc378 ) ) )) , detaildistance656);
			float3 ALBEDO_MOD16 = lerpResult661;
			float ColorMask67 = lerp((float)0,saturate( ( 1.0 - ( ( distance( _ColorMaskTint.rgb , ALBEDO_MOD16 ) - _ColorMaskRange ) / max( _ColorMaskFuzziness , 1E-05 ) ) ) ),_EnableColorMask);
			float temp_output_6_0 = frac( uv0_MainTex.y );
			float temp_output_7_0 = frac( uv0_MainTex.x );
			float temp_output_35_0 = saturate( ( step( ( 1.0 - temp_output_6_0 ) , _SquareMaskTopXRightYBottomZLeftW.x ) + step( ( 1.0 - temp_output_7_0 ) , _SquareMaskTopXRightYBottomZLeftW.y ) + step( temp_output_6_0 , _SquareMaskTopXRightYBottomZLeftW.z ) + step( temp_output_7_0 , _SquareMaskTopXRightYBottomZLeftW.w ) ) );
			float SquareMask72 = lerp((float)1,lerp(temp_output_35_0,( 1.0 - temp_output_35_0 ),_SquareMaskInvert),_EnableSquareMask);
			float3 autumn365 = ( ( ColorMask67 * SquareMask72 ) * ALBEDO_MOD16 );
			float3 break351 = autumn365;
			float temp_output_354_0 = sin( ( _ColorMaskAutumnShift * UNITY_PI ) );
			float2 appendResult353 = (float2(( break351.x * (0.3 + (( 1.0 - temp_output_354_0 ) - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) ) , break351.y));
			float3 hsvTorgb321 = RGBToHSV( float3( appendResult353 ,  0.0 ) );
			float3 hsvTorgb322 = HSVToRGB( float3(( hsvTorgb321.x - ( _ColorMaskAutumnShift / 3 ) ),( hsvTorgb321.y + saturate( ( _ColorMaskAutumnShift * UNITY_PI ) ) ),( hsvTorgb321.z + ( temp_output_354_0 / 40 ) )) );
			float2 AUTUMN_MOD366 = (hsvTorgb322).xy;
			float3 blendOpSrc337 = ALBEDO_MOD16;
			float3 blendOpDest337 = float3( AUTUMN_MOD366 ,  0.0 );
			float3 hsvTorgb478 = RGBToHSV( tex2DNode472.rgb );
			float3 hsvTorgb492 = HSVToRGB( float3(hsvTorgb478.x,( hsvTorgb478.y * _SurfaceSaturation ),hsvTorgb478.z) );
			float3 SURFACE_MOD547 = ( (_SurfaceTint).rgb * hsvTorgb492 );
			float3 lerpResult497 = lerp( ( saturate( ( 1.0 - ( 1.0 - blendOpSrc337 ) * ( 1.0 - blendOpDest337 ) ) )) , SURFACE_MOD547 , SURFACE_BLEND_MOD561);
			float3 COVER_MOD158 = ( (tex2DNode118).rgb * temp_output_185_0 * lerp(_CoverIntensity,( PWSF_GlobalSnowIntensity1 * _CoverIntensity ),_PWSFGlobalControllerSnow1) );
			float3 lerpResult620 = lerp( lerpResult497 , ( lerpResult497 + COVER_MOD158 ) , COVER_BLEND_MOD610);
			float lerpResult151 = lerp( SquareMask72 , ColorMask67 , (float)_DebugMode);
			float temp_output_125_0 = saturate( ( distance( max( ( abs( ( ase_vertex3Pos + _WindBoxPosition ) ) - ( _WindBoxSize * 0.5 ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / _WindBoxFalloff ) );
			float DebugWindBoxMask155 = ( 1.0 - temp_output_125_0 );
			int clampResult152 = clamp( ( _DebugMode - 1 ) , 0 , 1 );
			float lerpResult160 = lerp( lerpResult151 , DebugWindBoxMask155 , (float)clampResult152);
			float3 temp_cast_18 = (lerpResult160).xxx;
			o.Albedo = lerp(saturate( lerpResult620 ),temp_cast_18,_EnableDebug);
			o.Metallic = _Metallic;
			float lerpResult763 = lerp( 1.0 , ( tex2D( _AmbientOcclusionTexture, uv0_MainTex ).r * i.vertexColor.r ) , _AmbientOcclusion);
			float perceptualSmoothness9_g81 = _Smoothness1;
			float3 geometricNormalWS9_g81 = ase_worldNormal;
			float screenSpaceVariance9_g81 = _SmoothnessVariance1;
			float threshold9_g81 = _SmoothnessThreshold1;
			float localGetGeometricNormalVariance9_g81 = GetGeometricNormalVariance( perceptualSmoothness9_g81 , geometricNormalWS9_g81 , screenSpaceVariance9_g81 , threshold9_g81 );
			o.Smoothness = ( ( 1.0 - tex2D( _RoughnessTexture, uv0_MainTex ).r ) * lerpResult763 * localGetGeometricNormalVariance9_g81 );
			o.Occlusion = lerpResult763;
			o.Alpha = 1;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode229 = tex2D( _MainTex, uv_MainTex );
			float lerpResult238 = lerp( (float)1 , tex2DNode229.a , (float)_RenderMode);
			int clampResult237 = clamp( ( _RenderMode - 1 ) , 0 , 1 );
			float lerpResult239 = lerp( lerpResult238 , max( max( tex2DNode229.r , tex2DNode229.g ) , tex2DNode229.b ) , (float)clampResult237);
			float temp_output_506_0 = floor( _AlphaCutoffBias );
			float lerpResult507 = lerp( ( SquareMask72 * saturate( ( lerpResult239 + _AlphaCutoffBias ) ) ) , temp_output_506_0 , temp_output_506_0);
			float Opacity156 = lerpResult507;
			float temp_output_729_0 = saturate( pow( ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _CrossfadeDistance ) , _CrossfadeDistanceExponent ) );
			float lerpResult700 = lerp( Opacity156 , ( temp_output_729_0 + _CrossfadeScale ) , temp_output_729_0);
			float lerpResult732 = lerp( Opacity156 , lerpResult700 , (float)_CrossfadeMode);
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen695 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither695 = Dither8x8Bayer( fmod(clipScreen695.x, 8), fmod(clipScreen695.y, 8) );
			dither695 = step( dither695, saturate( (0.0 + (( 1.0 - temp_output_729_0 ) - 0.15) * (1.0 - 0.0) / (0.95 - 0.15)) ) );
			int temp_output_735_0 = ( _CrossfadeMode - 1 );
			int clampResult736 = clamp( temp_output_735_0 , 0 , 1 );
			float lerpResult733 = lerp( lerpResult732 , ( Opacity156 * dither695 ) , (float)clampResult736);
			float2 clipScreen706 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither706 = Dither8x8Bayer( fmod(clipScreen706.x, 8), fmod(clipScreen706.y, 8) );
			float smoothstepResult704 = smoothstep( 0.0 , 1.0 , unity_LODFade.x);
			dither706 = step( dither706, ( smoothstepResult704 * smoothstepResult704 ) );
			float temp_output_697_0 = ( 1.0 - dither706 );
			int clampResult741 = clamp( ( temp_output_735_0 - 1 ) , 0 , 1 );
			float lerpResult743 = lerp( lerpResult733 , ( lerpResult700 * saturate( ( temp_output_697_0 + temp_output_697_0 ) ) ) , (float)clampResult741);
			float Crossfade740 = lerpResult743;
			clip( Crossfade740 - _MaskClipValue );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows noshadow vertex:vertexDataFunc 

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
	Fallback "Bakery/Standard"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17106
149;17.5;1664;1002;10532.94;1922.989;4.05154;True;False
Node;AmplifyShaderEditor.CommentaryNode;655;-6146.493,-578.4186;Inherit;False;1402.528;496.2534;;10;656;662;404;653;654;650;648;647;646;645;PWSF Detail Distance;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;646;-6023.973,-245.001;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;645;-6096.645,-391.0239;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;400;-4549.896,-585.846;Inherit;False;1140.834;376.8854;;9;376;377;375;401;378;409;410;394;636;PWSF Cell Detail;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;647;-5948.504,-489.2092;Float;False;Property;_AlbedoDetailDistance;Albedo Detail Distance;13;0;Create;True;0;0;False;0;4;0.82;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;648;-5805.855,-388.1629;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;650;-5631.608,-383.5217;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;375;-4592.478,-498.1235;Inherit;False;Property;_AlbedoDetailScale;Albedo Detail Scale;12;0;Create;True;0;0;False;0;0;4.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1;-5943.843,26.17647;Inherit;False;1968.277;460.4759;;17;72;257;258;53;39;35;22;17;14;15;18;10;12;9;6;7;4;Square Mask;0.9705882,1,0,1;0;0
Node;AmplifyShaderEditor.IntNode;377;-4494.82,-411.7858;Inherit;False;Constant;_Int9;Int 9;40;0;Create;True;0;0;False;0;50;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;376;-4320.033,-485.7859;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3;-4732.225,-1194.799;Inherit;False;1370.224;462.5761;;8;16;660;661;11;461;458;5;8;ALBEDO_MOD;0,0.2467189,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;654;-5496.104,-380.6481;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-5894.587,87.15694;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-4621.779,-1144.37;Float;False;Property;_Color;Albedo Tint;10;1;[HDR];Create;False;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;404;-5435.596,-474.8058;Inherit;False;Property;_AlbedoDetailIntensity;Albedo Detail Intensity;11;0;Create;True;0;0;False;0;0;0.308;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-4658.734,-956.41;Inherit;True;Property;_MainTex;MainTex;9;1;[HDR];Create;False;0;0;False;1;Header(ALBEDO MAP);-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;653;-5312.767,-379.7262;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;636;-4156.379,-531.667;Inherit;False;0;0;1;0;1;False;1;False;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.FractNode;7;-5624.387,163.3454;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;662;-5135.812,-377.5738;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;458;-4340.779,-1133.253;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;394;-3980.596,-530.3102;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;410;-3978.413,-454.02;Inherit;False;Constant;_Deviant;Deviant;38;0;Create;True;0;0;False;0;0.3;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;6;-5623.732,73.25143;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;461;-4330.609,-955.9757;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;656;-4979.845,-380.2369;Inherit;False;detaildistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;10;-5683.681,277.9933;Float;False;Property;_SquareMaskTopXRightYBottomZLeftW;Square Mask Top X, Right Y, Bottom Z, Left W;33;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-4042.389,-1067.684;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;12;-5492.608,165.9975;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;9;-5491.184,75.09347;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;409;-3807.817,-532.2141;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;378;-3642.477,-536.1733;Inherit;False;Overlay;True;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StepOpNode;17;-5333.635,71.46653;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;18;-5332.866,166.2404;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;15;-5334.557,352.1055;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;14;-5335.115,259.2854;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;660;-4107.126,-833.0792;Inherit;False;656;detaildistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-5173.26,186.2324;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;661;-3786.246,-1067.44;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-3599.979,-1076.152;Float;False;ALBEDO_MOD;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;35;-5046.243,188.0854;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;19;-6249.218,537.9836;Inherit;False;1171.949;509.848;;8;67;255;256;21;23;28;27;811;Color Mask;0.972549,1,0,1;0;0
Node;AmplifyShaderEditor.ColorNode;21;-6134.695,684.0361;Float;False;Property;_ColorMaskTint;Color Mask Tint;26;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;27;-6149.244,945.2161;Float;False;Property;_ColorMaskFuzziness;Color Mask Fuzziness;28;0;Create;True;0;0;False;0;0.03;-0.81;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-6189.045,862.3762;Float;False;Property;_ColorMaskRange;Color Mask Range;29;0;Create;True;0;0;False;0;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;39;-4890.363,188.3759;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-6137.681,592.5225;Inherit;False;16;ALBEDO_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;53;-4721.803,238.1155;Float;False;Property;_SquareMaskInvert;Square Mask Invert;32;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;811;-5834.087,700.0439;Inherit;False;PWSF Color Mask v1.0;-1;;73;f9add065de4b3234b889fa0338f80562;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;256;-5732.635,590.7792;Inherit;False;Constant;_Int0;Int 0;37;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;258;-4618.202,157.0129;Inherit;False;Constant;_Int1;Int 1;38;0;Create;True;0;0;False;0;1;0;0;1;INT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;257;-4450.643,213.0147;Inherit;False;Property;_EnableSquareMask;Enable Square Mask;31;0;Create;True;0;0;False;1;Header(SQUARE MASK);0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;255;-5543.27,647.0518;Inherit;False;Property;_EnableColorMask;Enable Color Mask;25;0;Create;True;0;0;False;1;Header(COLOR MASK);1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-5316.229,649.7518;Float;False;ColorMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;79;-3916.62,-36.17989;Inherit;False;1540.945;488.2466;;17;184;620;497;254;145;173;572;562;337;368;365;333;108;107;94;93;622;Main;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;-4184.378,213.9785;Float;False;SquareMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-3849.699,253.0221;Inherit;False;67;ColorMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;370;-4899.373,536.4875;Inherit;False;2509.556;558.0143;#no blue;21;324;355;354;369;356;351;357;348;341;353;360;342;359;321;362;344;325;327;322;343;366;Autumn;1,0.5019608,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-3852.156,336.4929;Inherit;False;72;SquareMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;324;-4876.834,581.5218;Inherit;False;Property;_ColorMaskAutumnShift;Color Mask Autumn Shift;30;0;Create;True;0;0;False;1;Header(COLOR MASK SEASONS);0;0.064;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-3855.074,28.73666;Inherit;False;16;ALBEDO_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-3485.345,256.686;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;355;-4550.81,586.4875;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;333;-3336.74,258.0048;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;49;-3338.917,-665.1354;Inherit;False;1779.699;456.0286;;17;506;507;156;189;225;109;221;239;217;237;213;238;212;192;234;233;229;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;365;-3188.406,256.3574;Inherit;False;autumn;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;354;-4368.756,587.8068;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;595;-6271.083,-2500.044;Inherit;False;2761.342;1138.85;;32;916;915;914;605;472;69;772;547;496;546;489;492;479;530;561;550;480;488;534;583;469;528;606;467;540;514;513;485;478;615;430;476;PWSF Surface Map;0,0.2857146,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;229;-3308.388,-601.5748;Inherit;True;Property;_TextureSample0;Texture Sample 0;9;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;5;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;356;-4239.47,587.8068;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;233;-3223.601,-408.5635;Float;False;Property;_RenderMode;Render Mode;2;1;[Enum];Create;True;3;Opaque;0;Cutout Alpha;1;Cutout Black;2;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.PosVertexDataNode;514;-5870.414,-1717.175;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;60;-6372.998,-3819.512;Inherit;False;2847.163;1186.928;;25;911;773;158;610;618;186;187;439;185;440;124;434;435;910;436;118;101;601;917;600;919;918;921;922;923;PWSF Cover Map;0,0.2876301,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;513;-5947.414,-1571.175;Inherit;False;Property;_SurfaceBias;Surface Bias;41;0;Create;True;0;0;False;0;-0.25;-0.58;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;369;-4315.21,767.175;Inherit;False;365;autumn;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;351;-4127.291,771.1589;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;528;-6179.614,-1674.375;Inherit;False;Property;_SurfaceScatterScale;Surface Scatter Scale;39;0;Create;True;0;0;False;0;1.5;1.14;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;234;-2978.13,-352.0079;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;540;-6094.414,-1825.176;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;919;-6277.564,-3387.149;Inherit;False;Constant;_CoverMapOffset;Cover Map Offset;50;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;915;-6143.455,-2231.062;Inherit;False;Property;_SurfaceMapTilling;Surface Map Tilling;36;0;Create;True;0;0;False;0;1;1.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;918;-6277.218,-3489.818;Inherit;False;Property;_CoverMapTilling;Cover Map Tilling;48;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;467;-5851.077,-1487.506;Float;False;Property;_SurfaceDistanceLength;Surface Distance Length;42;0;Create;True;0;0;False;0;0.75;0.97;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;212;-2988.404,-614.3965;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;826;-5982.386,1244.834;Inherit;False;3496.133;1690.034;Comment;56;157;625;624;161;155;147;125;66;902;24;141;138;131;130;99;136;112;95;86;113;116;106;71;98;104;90;89;91;65;80;47;903;76;74;48;52;818;61;904;63;42;32;51;50;31;57;34;819;45;40;26;25;906;29;905;20;PWSF Wind Masked Box Setup;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;739;-3263.245,-1506.36;Inherit;False;2637.212;635.2985;;35;736;735;732;734;733;741;742;740;744;743;725;706;703;698;702;697;704;705;711;693;712;729;708;709;737;695;745;714;713;730;731;707;701;700;720;Crossfade;0,0.9172413,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;916;-6140.803,-2133.393;Inherit;False;Constant;_SurfaceMapOffset;Surface Map Offset;38;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;192;-2978.274,-477.3937;Inherit;False;Constant;_Int3;Int 3;34;0;Create;True;0;0;False;0;1;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;606;-5659.21,-1669.214;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;357;-4067.504,588.7529;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.3;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;238;-2824.787,-471.5109;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;917;-6065.617,-3466.311;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;914;-5862.307,-2197.05;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;469;-5514.115,-1668.275;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;348;-3869.877,772.7209;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;530;-5870.414,-1829.176;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;20;-5932.386,1927.449;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;701;-3244.437,-1438.329;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMaxOpNode;213;-2857.47,-589.1102;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;702;-3237.496,-1288.417;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;237;-2824.998,-350.9904;Inherit;False;3;0;INT;0;False;1;INT;0;False;2;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;600;-6126.755,-3236.429;Float;False;Property;_CoverNormalStrength;Cover Normal Strength;52;0;Create;True;0;0;False;0;1;0.425;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;711;-3021.142,-1243.925;Float;False;Property;_CrossfadeDistance;Crossfade Distance;6;0;Create;True;0;0;False;0;64;88.8;0;128;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;45;-5116.19,1450.697;Float;False;Property;_WindBoxPosition;Wind Box Position;63;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;601;-5782.721,-3378.683;Inherit;True;Property;_CoverNormalTexture;Cover Normal Texture;51;3;[HDR];[NoScaleOffset];[Normal];Create;True;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;353;-3711.319,770.5219;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;583;-5373.414,-1688.175;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-5649.718,1823.763;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;905;-5816.083,2339.093;Float;False;Property;_WindPulseMagnitudeDetail;Wind Pulse Magnitude Detail;59;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;921;-5640.469,-2879.316;Half;False;Global;PWSF_GlobalSnowIntensity1;PWSF_GlobalSnowIntensity;27;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;217;-2678.754,-293.9258;Float;False;Property;_AlphaCutoffBias;Alpha Cutoff Bias;3;0;Create;True;0;0;False;0;0.49;0.49;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;239;-2649.874,-470.2491;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;111;-7515.424,-1272.11;Inherit;False;2711.317;455.8878;;22;159;782;783;785;611;614;617;501;563;609;616;658;405;689;672;127;685;771;596;686;659;597;NORMAL_MOD;0.9989958,0,1,1;0;0
Node;AmplifyShaderEditor.IntNode;341;-4721.08,677.2591;Inherit;False;Constant;_Int6;Int 6;40;0;Create;True;0;0;False;0;3;0;0;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;26;-5652.554,2078.756;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-5790.715,1711.613;Float;False;Property;_WindPulseFrequency;Wind Pulse Frequency;56;0;Create;True;0;0;False;0;8;8;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;40;-5114.143,1294.834;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;360;-3551.013,970.8004;Inherit;False;Constant;_Int7;Int 7;41;0;Create;True;0;0;False;0;40;0;0;1;INT;0
Node;AmplifyShaderEditor.SamplerNode;472;-5578.534,-2118.018;Inherit;True;Property;_SurfaceTexture;Surface Texture;35;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;29;-5800.513,2206.095;Float;False;Property;_WindPulseFrequencyDetail;Wind Pulse Frequency Detail;58;0;Create;True;0;0;False;0;0.5;0.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;669;-7837.805,-557.2096;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;906;-5787.698,1609.786;Float;False;Property;_WindPulseMagnitude;Wind Pulse Magnitude;57;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;707;-2908.145,-1387.143;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;434;-5631.522,-2995.352;Inherit;False;Property;_CoverIntensity;Cover Intensity;49;0;Create;True;0;0;False;0;2;2.78;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;720;-2881.168,-1147.351;Float;False;Property;_CrossfadeDistanceExponent;Crossfade Distance Exponent;7;0;Create;True;0;0;False;0;15.74425;12.2;0;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-4874.896,1296.351;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;101;-5235.786,-3368.388;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-5473.73,1824.631;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;476;-5246.411,-1917.27;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;221;-2377.911,-446.8497;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;488;-5356.461,-1566.487;Float;False;Property;_SurfaceBlendStrength;Surface Blend Strength;40;0;Create;True;0;0;False;0;0;4.66;0;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;534;-5228.223,-1690.346;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;815;-7667.347,-792.4844;Inherit;False;PreparePerturbNormalHQ;-1;;77;ce0790c3228f3654b818a19dd51453a4;0;1;1;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT3;4;FLOAT3;6;FLOAT;9
Node;AmplifyShaderEditor.GetLocalVarNode;659;-7443.384,-928.37;Inherit;False;656;detaildistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;436;-5346.229,-3096.851;Inherit;False;Property;_CoverBias;Cover Bias;50;0;Create;True;0;0;False;0;0;-0.33;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;118;-5787.628,-3620.282;Inherit;True;Property;_CoverTexture;Cover Texture;47;1;[HDR];Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-5487.314,2075.25;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;819;-5462.134,2345.354;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;922;-5305.73,-2873.668;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-4842.837,1605.773;Float;False;Constant;_Float3;Float 3;13;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;-5454.318,1613.037;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;321;-3541.102,752.404;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;57;-4870.261,1441.851;Float;False;Property;_WindBoxSize;Wind Box Size;64;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;597;-7452.291,-1026.204;Float;False;Property;_NormalDetailStrength;Normal Detail Strength;16;0;Create;True;0;0;False;0;0.25;0.709;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;362;-3543.842,892.6512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;342;-4550.904,658.675;Inherit;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;708;-2738.945,-1387.943;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;359;-3403.343,953.0732;Inherit;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;430;-5918.077,-2389.861;Float;False;Property;_SurfaceNormalStrength;Surface Normal Strength;44;0;Create;True;0;0;False;0;1;0.964;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;479;-4995.194,-1911.706;Float;False;Property;_SurfaceSaturation;Surface Saturation;38;0;Create;True;0;0;False;0;0.5;0.738;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;904;-5125.367,2441.148;Half;False;Global;PWSF_GlobalWindIntensity;PWSF_GlobalWindIntensity;28;0;Create;True;0;0;False;0;0.1;0;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;709;-2592.846,-1386.743;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;3.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;225;-2237.101,-446.4496;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HeightMapBlendNode;480;-5048.545,-1701.893;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-2300.628,-535.6664;Inherit;False;72;SquareMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;686;-7088.601,-947.0927;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;478;-5106.448,-2115.251;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;814;-7260.605,-799.5311;Inherit;False;PerturbNormalHQ;-1;;78;45dff16e78a0685469fed8b5b46e4d96;0;4;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;325;-3259.691,730.5699;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;910;-5238.263,-3546.32;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;685;-7137.573,-1102.068;Inherit;False;Constant;_Vector0;Vector 0;49;0;Create;True;0;0;False;0;0.5,0.5,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;596;-7198.129,-1215.928;Float;False;Property;_NormalStrength;Normal Strength;15;0;Create;True;0;0;False;0;1;0.47;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;63;-4727.481,1296.268;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;923;-5140.7,-2991.646;Float;False;Property;_PWSFGlobalControllerSnow1;PWSF Global Controller Snow;46;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-5117.684,2347.511;Float;False;Property;_WindIntensity;Wind Intensity;55;0;Create;True;0;0;False;0;0.0094;0.0094;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-5266.021,1822.643;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;818;-5264.309,2076.391;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-4653.71,1459.993;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;327;-3257.632,926.0892;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;344;-3259.896,829.1956;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LODFadeNode;705;-3210.074,-1028.518;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;435;-5010.748,-3322.575;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;771;-7462.289,-1181.264;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;127;-6899.067,-1204.68;Inherit;True;Property;_NormalTexture;Normal Texture;14;3;[HDR];[NoScaleOffset];[Normal];Create;True;0;0;False;1;Header(NORMAL MAP);-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosOpNode;47;-5102.917,1824.429;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;322;-3067.538,779.1349;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;903;-4775.878,2434.475;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;74;-4566.284,2649.627;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;550;-4743.694,-1698.364;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;506;-2068.742,-375.0962;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-4845.291,-3343.923;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;489;-4691.303,-2242.882;Float;False;Property;_SurfaceTint;Surface Tint;37;0;Create;True;0;0;False;0;1,1,1,0;0.5957442,0.6132076,0.4425506,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-2084.105,-470.4947;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;485;-4611.433,-1939.821;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;672;-6824.28,-1003.106;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;52;-5034.092,1667.069;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;729;-2430.269,-1362.415;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;76;-4588.768,2495.446;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;69;-5585.125,-2335.094;Inherit;True;Property;_SurfaceNormalTexture;Surface Normal Texture;43;3;[HDR];[NoScaleOffset];[Normal];Create;True;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;80;-4488.066,1296.715;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;48;-5067.583,2072.92;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;704;-3010.509,-1025.757;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;507;-1935.512,-469.0518;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;440;-4326.385,-3266.932;Inherit;False;Constant;_Int8;Int 8;41;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;185;-4509.245,-3345.402;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;492;-4422.864,-2080.958;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMaxOpNode;90;-4319.259,1296.045;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;343;-2849.329,776.3534;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BlendNormalsNode;405;-6577.978,-1055.161;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;689;-6575.623,-949.8384;Inherit;False;656;detaildistance;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;902;-4621.037,2345.1;Float;False;Property;_PWSFGlobalControllerWind;PWSF Global Controller Wind ;54;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-4797.42,1801.516;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-4299.962,2645.986;Float;False;Property;_WindBlendRange;Wind Blend Range;61;0;Create;True;0;0;False;0;6.6;6.6;0;32;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;703;-2824.838,-1024.491;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;605;-4556.728,-1726.217;Inherit;False;Property;_EnableSurfaceMap;Enable Surface Map;34;0;Create;True;0;0;False;1;Header(SURFACE MAP);1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;615;-5235.837,-2334.446;Inherit;False;surface_normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DistanceOpNode;91;-4298.317,2539.63;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;546;-4425.547,-2243.467;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;730;-2275.078,-1235.057;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;98;-4171.196,1296.533;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;366;-2632.817,776.4207;Inherit;False;AUTUMN_MOD;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-4344.544,1441.306;Float;False;Property;_WindBoxFalloff;Wind Box Falloff;62;0;Create;True;0;0;False;1;PowerSlider(3);0.05;0.05;0.001;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;706;-2670.211,-1029.137;Inherit;False;1;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;616;-6399.02,-1184.878;Inherit;False;615;surface_normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-1777.925,-473.6764;Float;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;496;-4159.245,-2238.536;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;106;-4019.098,2540.454;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;658;-6336.301,-1075.008;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-4259.243,1802.539;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;561;-4267.987,-1728.394;Inherit;False;SURFACE_BLEND_MOD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;187;-5257.362,-3675.499;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;439;-4153.06,-3371.782;Inherit;False;Property;_EnableCoverMap;Enable Cover Map;45;0;Create;True;0;0;False;1;Header(COVER MAP);0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;693;-2100.09,-1234.876;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.15;False;2;FLOAT;0.95;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;712;-2592.538,-1283.443;Float;False;Property;_CrossfadeScale;Crossfade Scale;8;0;Create;True;0;0;False;0;0.8;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;116;-3885.836,2543.496;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;618;-5402.896,-3220.607;Inherit;False;cover_normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;697;-2457.094,-1029.451;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;713;-2271.499,-1359.612;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;734;-1776.763,-1142.588;Inherit;False;Property;_CrossfadeMode;Crossfade Mode;5;1;[Enum];Create;True;4;OFF;0;Distance;1;Distance Dither;2;Distance Dither LOD;3;0;False;1;Header(CROSSFADE);0;0;0;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;731;-1899.34,-1235.29;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;737;-2428.357,-1461.56;Inherit;False;156;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;368;-3859.257,107.9403;Inherit;False;366;AUTUMN_MOD;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;563;-6195.284,-935.9186;Inherit;False;561;SURFACE_BLEND_MOD;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;86;-4023.825,1783.913;Inherit;True;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;95;-4019.475,2054.292;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-4199.061,1647.967;Float;False;Property;_WindDirection;Wind Direction;60;0;Create;True;0;0;False;0;0.062;0.062;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-4193.062,-3671.405;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;609;-6153.384,-1178.378;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;610;-3885.202,-3371.4;Inherit;True;COVER_BLEND_MOD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;547;-3990.643,-2241.021;Inherit;False;SURFACE_MOD;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;-3996.994,1299.242;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-4022.277,-3676.77;Float;True;COVER_MOD;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PiNode;130;-3858.197,1653.846;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;617;-5920.912,-1171.076;Inherit;False;618;cover_normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DitheringNode;695;-1726.977,-1240.86;Inherit;False;1;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;114;-2317.896,158.3662;Inherit;False;914.8961;598.0989;;9;183;160;152;164;151;144;146;150;132;Debug;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;562;-3346.942,148.0833;Inherit;False;561;SURFACE_BLEND_MOD;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;700;-2111.755,-1374.388;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;125;-3412.361,1307.335;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;337;-3557.882,28.8681;Inherit;False;Screen;True;3;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-3783.557,1784.749;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;501;-5874.269,-1076.478;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;572;-3315.281,68.19775;Inherit;False;547;SURFACE_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;744;-1554.137,-1369.083;Inherit;False;156;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;131;-3727.227,2544.953;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;764;-2304.917,894.563;Inherit;False;1098.854;418.1606;;4;748;749;755;770;Smoothness;0,0.8758622,1,1;0;0
Node;AmplifyShaderEditor.IntNode;136;-3816.542,2706.56;Float;False;Constant;_Int10;Int 10;23;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;735;-1508.985,-1147.186;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;614;-5902.149,-935.5066;Inherit;False;610;COVER_BLEND_MOD;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;698;-2266.251,-1034.969;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;147;-3206.952,1304.528;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;769;-2130.102,1422.167;Inherit;False;875.438;413.129;;5;756;747;762;757;763;Ambient Occlusion;0,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;770;-2304.243,968.6722;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotateAboutAxisNode;141;-3619.664,1717.489;Inherit;False;False;4;0;FLOAT3;0,1,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;745;-2115.254,-1031.361;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;785;-5630.03,-943.0329;Inherit;False;FLOAT3;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;-1;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;736;-1348.974,-1147.693;Inherit;False;3;0;INT;0;False;1;INT;0;False;2;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.IntNode;132;-2267.974,499.4078;Float;False;Property;_DebugMode;Debug Mode;66;1;[Enum];Create;True;3;Square Mask;0;Color Mask;1;Box Mask;2;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-3075.399,167.2209;Inherit;False;158;COVER_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;742;-1343.462,-1027.061;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;497;-3072.266,47.95909;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCCompareEqual;138;-3566.571,2647.19;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0;False;2;INT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;732;-1364.749,-1362.319;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;611;-5662.237,-1075.812;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;725;-1508.917,-1258.573;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;747;-2080.102,1472.167;Inherit;True;Property;_AmbientOcclusionTexture;Ambient Occlusion Texture;23;1;[NoScaleOffset];Create;True;0;0;False;1;Header(AMBIENT OCCLUSION MAP);-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;622;-2966.981,257.523;Inherit;False;610;COVER_BLEND_MOD;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;625;-3210.517,1591.279;Inherit;False;Constant;_Int4;Int 4;51;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;714;-1939.425,-1053.134;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;155;-2997.352,1298.248;Float;False;DebugWindBoxMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;184;-2850.052,117.9149;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-2266.254,412.7428;Inherit;False;67;ColorMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;756;-1792.42,1567.781;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;144;-2277.462,333.2623;Inherit;False;72;SquareMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;733;-1183.119,-1285.717;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;741;-1187.352,-1023.668;Inherit;False;3;0;INT;0;False;1;INT;0;False;2;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;146;-2090.747,577.3938;Inherit;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;783;-5471.116,-1004.526;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-3239.871,1713.556;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;624;-3045.873,1689.036;Inherit;False;Property;_EnableWind;Enable Wind;53;0;Create;True;0;0;False;1;Header(WIND);0;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;620;-2716.369,96.19347;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;757;-1863.464,1734.296;Float;False;Property;_AmbientOcclusion;Ambient Occlusion;24;0;Create;True;0;0;False;0;0.5;0.978;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;151;-1972.91,366.1186;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwitchByFaceNode;782;-5294.96,-1073.807;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;152;-1936.196,575.4113;Inherit;False;3;0;INT;0;False;1;INT;0;False;2;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;164;-2057.859,490.1544;Inherit;False;155;DebugWindBoxMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;762;-1597.433,1498.685;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;743;-1025.398,-1159.092;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;755;-2030.633,944.5631;Inherit;True;Property;_RoughnessTexture;Roughness Texture;17;1;[NoScaleOffset];Create;True;0;0;False;1;Header(ROUGHNESS MAP);-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;160;-1754.436,462.206;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;401;-3751.88,-401.7918;Inherit;False;315.6667;172.253;experimental leave disease;1;395;;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;749;-1723.173,972.3892;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;763;-1438.662,1475.349;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;908;-1975.562,1149.205;Inherit;False;PWSF Smoothness Geometric Specular Anti-aliasing v1.0;18;;81;47958d34917c433438e13536e162627b;0;0;3;FLOAT;17;FLOAT;20;FLOAT;21
Node;AmplifyShaderEditor.RegisterLocalVarNode;740;-857.9052,-1164.608;Inherit;False;Crossfade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;161;-2802.112,1692.17;Float;False;Wind_Vertex_Offset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;145;-2533.412,227.3497;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-5087.132,-1090.572;Float;False;NORMAL_MOD;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;508;-1406.779,-319.1891;Inherit;False;574.1;350.5;Translucency;5;423;421;692;691;690;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;690;-1387.246,-263.1241;Float;False;Property;_SubSurfColor;SubSurfColor;27;0;Create;True;0;0;False;0;0.9339623,0.8733506,0.8238252,0;1,0.8995854,0.5801886,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;692;-1137.129,-257.3686;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;254;-3654.966,244.5983;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;691;-1334.557,-95.11623;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SwitchByFaceNode;911;-5020.426,-3737.837;Inherit;False;2;0;FLOAT3;1,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;172;-1291,162;Inherit;False;159;NORMAL_MOD;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;424;-1353,320;Inherit;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;0.1;0.047;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;202;-1024,64;Inherit;False;Property;_MaskClipValue;Mask Clip Value;0;1;[HideInInspector];Create;True;0;0;True;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;423;-1132.78,-159.7891;Inherit;False;Constant;_Int5;Int 5;39;0;Create;True;0;0;False;0;2;0;0;1;INT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;772;-5546.419,-2455.455;Inherit;False;0;472;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;748;-1386.812,974.2672;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;421;-968.8787,-255.0891;Inherit;False;2;0;COLOR;0,0,0,0;False;1;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-1350,533;Inherit;False;161;Wind_Vertex_Offset;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;203;-1024,144;Inherit;False;Property;_CullMode;Cull Mode;1;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CullMode;True;1;Header(GLOBAL SETTINGS);0;0;0;1;INT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;773;-5779.851,-3754.789;Inherit;False;0;118;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;395;-3738.477,-360.9413;Inherit;False;Darken;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;183;-1598.297,219.6645;Inherit;False;Property;_EnableDebug;Enable Debug;65;0;Create;True;0;0;False;1;Header(DEBUG);0;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;765;-1342.5,419.0746;Inherit;False;740;Crossfade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1024,224;Float;False;True;2;ASEMaterialInspector;0;0;Standard;PWS/Procedural/Vegetation SP v1.3 2019_01_14;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;Bakery/Standard;-1;-1;-1;-1;0;False;0;0;True;203;-1;0;True;202;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;648;0;645;0
WireConnection;648;1;646;0
WireConnection;650;0;648;0
WireConnection;650;1;647;0
WireConnection;376;0;375;0
WireConnection;376;1;377;0
WireConnection;654;0;650;0
WireConnection;5;1;4;0
WireConnection;653;0;654;0
WireConnection;636;0;4;0
WireConnection;636;2;376;0
WireConnection;7;0;4;1
WireConnection;662;0;653;0
WireConnection;662;1;404;0
WireConnection;458;0;8;0
WireConnection;394;0;636;0
WireConnection;6;0;4;2
WireConnection;461;0;5;0
WireConnection;656;0;662;0
WireConnection;11;0;458;0
WireConnection;11;1;461;0
WireConnection;12;0;7;0
WireConnection;9;0;6;0
WireConnection;409;0;394;0
WireConnection;409;1;410;0
WireConnection;378;0;409;0
WireConnection;378;1;11;0
WireConnection;17;0;9;0
WireConnection;17;1;10;1
WireConnection;18;0;12;0
WireConnection;18;1;10;2
WireConnection;15;0;7;0
WireConnection;15;1;10;4
WireConnection;14;0;6;0
WireConnection;14;1;10;3
WireConnection;22;0;17;0
WireConnection;22;1;18;0
WireConnection;22;2;14;0
WireConnection;22;3;15;0
WireConnection;661;0;11;0
WireConnection;661;1;378;0
WireConnection;661;2;660;0
WireConnection;16;0;661;0
WireConnection;35;0;22;0
WireConnection;39;0;35;0
WireConnection;53;0;35;0
WireConnection;53;1;39;0
WireConnection;811;1;23;0
WireConnection;811;3;21;0
WireConnection;811;4;28;0
WireConnection;811;5;27;0
WireConnection;257;0;258;0
WireConnection;257;1;53;0
WireConnection;255;0;256;0
WireConnection;255;1;811;0
WireConnection;67;0;255;0
WireConnection;72;0;257;0
WireConnection;107;0;94;0
WireConnection;107;1;93;0
WireConnection;355;0;324;0
WireConnection;333;0;107;0
WireConnection;333;1;108;0
WireConnection;365;0;333;0
WireConnection;354;0;355;0
WireConnection;356;0;354;0
WireConnection;351;0;369;0
WireConnection;234;0;233;0
WireConnection;212;0;229;1
WireConnection;212;1;229;2
WireConnection;606;0;514;2
WireConnection;606;1;513;0
WireConnection;357;0;356;0
WireConnection;238;0;192;0
WireConnection;238;1;229;4
WireConnection;238;2;233;0
WireConnection;917;0;918;0
WireConnection;917;1;919;0
WireConnection;914;0;915;0
WireConnection;914;1;916;0
WireConnection;469;0;606;0
WireConnection;469;1;467;0
WireConnection;348;0;351;0
WireConnection;348;1;357;0
WireConnection;530;0;540;0
WireConnection;530;1;528;0
WireConnection;213;0;212;0
WireConnection;213;1;229;3
WireConnection;237;0;234;0
WireConnection;601;1;917;0
WireConnection;601;5;600;0
WireConnection;353;0;348;0
WireConnection;353;1;351;1
WireConnection;583;0;530;0
WireConnection;583;1;469;0
WireConnection;25;0;20;1
WireConnection;25;1;20;3
WireConnection;239;0;238;0
WireConnection;239;1;213;0
WireConnection;239;2;237;0
WireConnection;26;0;20;1
WireConnection;26;1;20;3
WireConnection;472;1;914;0
WireConnection;669;0;409;0
WireConnection;707;0;701;0
WireConnection;707;1;702;0
WireConnection;50;0;40;0
WireConnection;50;1;45;0
WireConnection;101;0;601;0
WireConnection;32;0;25;0
WireConnection;32;1;24;0
WireConnection;476;0;472;4
WireConnection;221;0;239;0
WireConnection;221;1;217;0
WireConnection;534;0;583;0
WireConnection;815;1;669;0
WireConnection;118;1;917;0
WireConnection;34;0;26;0
WireConnection;34;1;29;0
WireConnection;819;0;905;0
WireConnection;922;0;921;0
WireConnection;922;1;434;0
WireConnection;31;0;906;0
WireConnection;321;0;353;0
WireConnection;362;0;355;0
WireConnection;342;0;324;0
WireConnection;342;1;341;0
WireConnection;708;0;707;0
WireConnection;708;1;711;0
WireConnection;359;0;354;0
WireConnection;359;1;360;0
WireConnection;709;0;708;0
WireConnection;709;1;720;0
WireConnection;225;0;221;0
WireConnection;480;0;476;0
WireConnection;480;1;534;0
WireConnection;480;2;488;0
WireConnection;686;0;597;0
WireConnection;686;1;659;0
WireConnection;478;0;472;0
WireConnection;814;1;815;0
WireConnection;814;2;815;4
WireConnection;814;3;815;6
WireConnection;325;0;321;1
WireConnection;325;1;342;0
WireConnection;910;1;118;4
WireConnection;63;0;50;0
WireConnection;923;0;434;0
WireConnection;923;1;922;0
WireConnection;42;0;32;0
WireConnection;42;1;31;0
WireConnection;818;0;34;0
WireConnection;818;1;819;0
WireConnection;61;0;57;0
WireConnection;61;1;51;0
WireConnection;327;0;321;3
WireConnection;327;1;359;0
WireConnection;344;0;321;2
WireConnection;344;1;362;0
WireConnection;435;0;101;2
WireConnection;435;1;436;0
WireConnection;127;1;771;0
WireConnection;127;5;596;0
WireConnection;47;0;42;0
WireConnection;322;0;325;0
WireConnection;322;1;344;0
WireConnection;322;2;327;0
WireConnection;903;0;904;0
WireConnection;903;1;66;0
WireConnection;550;0;480;0
WireConnection;506;0;217;0
WireConnection;124;0;910;0
WireConnection;124;1;435;0
WireConnection;124;2;923;0
WireConnection;189;0;109;0
WireConnection;189;1;225;0
WireConnection;485;0;478;2
WireConnection;485;1;479;0
WireConnection;672;0;685;0
WireConnection;672;1;814;0
WireConnection;672;2;686;0
WireConnection;729;0;709;0
WireConnection;69;1;914;0
WireConnection;69;5;430;0
WireConnection;80;0;63;0
WireConnection;80;1;61;0
WireConnection;48;0;818;0
WireConnection;704;0;705;1
WireConnection;507;0;189;0
WireConnection;507;1;506;0
WireConnection;507;2;506;0
WireConnection;185;0;124;0
WireConnection;492;0;478;1
WireConnection;492;1;485;0
WireConnection;492;2;478;3
WireConnection;90;0;80;0
WireConnection;343;0;322;0
WireConnection;405;0;127;0
WireConnection;405;1;672;0
WireConnection;902;0;66;0
WireConnection;902;1;903;0
WireConnection;65;0;52;2
WireConnection;65;1;47;0
WireConnection;65;2;48;0
WireConnection;703;0;704;0
WireConnection;703;1;704;0
WireConnection;605;1;550;0
WireConnection;615;0;69;0
WireConnection;91;0;76;0
WireConnection;91;1;74;0
WireConnection;546;0;489;0
WireConnection;730;0;729;0
WireConnection;98;0;90;0
WireConnection;366;0;343;0
WireConnection;706;0;703;0
WireConnection;156;0;507;0
WireConnection;496;0;546;0
WireConnection;496;1;492;0
WireConnection;106;0;91;0
WireConnection;106;1;89;0
WireConnection;658;0;127;0
WireConnection;658;1;405;0
WireConnection;658;2;689;0
WireConnection;71;0;65;0
WireConnection;71;1;902;0
WireConnection;561;0;605;0
WireConnection;187;0;118;0
WireConnection;439;0;440;0
WireConnection;439;1;185;0
WireConnection;693;0;730;0
WireConnection;116;0;106;0
WireConnection;618;0;601;0
WireConnection;697;0;706;0
WireConnection;713;0;729;0
WireConnection;713;1;712;0
WireConnection;731;0;693;0
WireConnection;86;0;71;0
WireConnection;86;2;71;0
WireConnection;186;0;187;0
WireConnection;186;1;185;0
WireConnection;186;2;923;0
WireConnection;609;0;616;0
WireConnection;609;1;658;0
WireConnection;610;0;439;0
WireConnection;547;0;496;0
WireConnection;112;0;98;0
WireConnection;112;1;104;0
WireConnection;158;0;186;0
WireConnection;130;0;113;0
WireConnection;695;0;731;0
WireConnection;700;0;737;0
WireConnection;700;1;713;0
WireConnection;700;2;729;0
WireConnection;125;0;112;0
WireConnection;337;0;108;0
WireConnection;337;1;368;0
WireConnection;99;0;86;0
WireConnection;99;1;95;0
WireConnection;501;0;658;0
WireConnection;501;1;609;0
WireConnection;501;2;563;0
WireConnection;131;0;116;0
WireConnection;735;0;734;0
WireConnection;698;0;697;0
WireConnection;698;1;697;0
WireConnection;147;0;125;0
WireConnection;141;1;130;0
WireConnection;141;3;99;0
WireConnection;745;0;698;0
WireConnection;736;0;735;0
WireConnection;742;0;735;0
WireConnection;497;0;337;0
WireConnection;497;1;572;0
WireConnection;497;2;562;0
WireConnection;138;0;89;0
WireConnection;138;2;136;0
WireConnection;138;3;131;0
WireConnection;732;0;744;0
WireConnection;732;1;700;0
WireConnection;732;2;734;0
WireConnection;611;0;501;0
WireConnection;611;1;617;0
WireConnection;611;2;614;0
WireConnection;725;0;737;0
WireConnection;725;1;695;0
WireConnection;747;1;770;0
WireConnection;714;0;700;0
WireConnection;714;1;745;0
WireConnection;155;0;147;0
WireConnection;184;0;497;0
WireConnection;184;1;173;0
WireConnection;733;0;732;0
WireConnection;733;1;725;0
WireConnection;733;2;736;0
WireConnection;741;0;742;0
WireConnection;146;0;132;0
WireConnection;783;0;611;0
WireConnection;783;1;785;0
WireConnection;157;0;141;0
WireConnection;157;1;125;0
WireConnection;157;2;138;0
WireConnection;624;0;625;0
WireConnection;624;1;157;0
WireConnection;620;0;497;0
WireConnection;620;1;184;0
WireConnection;620;2;622;0
WireConnection;151;0;144;0
WireConnection;151;1;150;0
WireConnection;151;2;132;0
WireConnection;782;0;611;0
WireConnection;782;1;783;0
WireConnection;152;0;146;0
WireConnection;762;0;747;1
WireConnection;762;1;756;1
WireConnection;743;0;733;0
WireConnection;743;1;714;0
WireConnection;743;2;741;0
WireConnection;755;1;770;0
WireConnection;160;0;151;0
WireConnection;160;1;164;0
WireConnection;160;2;152;0
WireConnection;749;0;755;1
WireConnection;763;1;762;0
WireConnection;763;2;757;0
WireConnection;740;0;743;0
WireConnection;161;0;624;0
WireConnection;145;0;620;0
WireConnection;159;0;782;0
WireConnection;692;0;690;0
WireConnection;692;1;691;0
WireConnection;254;0;94;0
WireConnection;748;0;749;0
WireConnection;748;1;763;0
WireConnection;748;2;908;17
WireConnection;421;0;692;0
WireConnection;421;1;423;0
WireConnection;395;0;11;0
WireConnection;183;0;145;0
WireConnection;183;1;160;0
WireConnection;0;0;183;0
WireConnection;0;1;172;0
WireConnection;0;3;424;0
WireConnection;0;4;748;0
WireConnection;0;5;763;0
WireConnection;0;10;765;0
WireConnection;0;11;170;0
ASEEND*/
//CHKSM=21340CA31C6B017603261F95AA94676285E0B9A9