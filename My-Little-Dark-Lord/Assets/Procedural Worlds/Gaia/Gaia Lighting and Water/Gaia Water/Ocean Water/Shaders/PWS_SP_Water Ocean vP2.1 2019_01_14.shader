// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PWS/SP/Water/Ocean vP2.1 2019_01_14"
{
	Properties
	{
		[Header (COLOR TINT)]_WaterTint("Water Tint", Color) = (0.08627451,0.2941177,0.3490196,1)
		_ShallowTint("Shallow Tint", Color) = (0.003921569,0.7529412,0.7137255,1)
		_ShallowDepth("Shallow Depth", Range( 0 , 100)) = 50
		_ShallowOffset("Shallow Offset", Range( -1 , 1)) = 0.2
		_DepthTint("Depth Tint", Color) = (0.08627451,0.2941177,0.3490196,1)
		_DepthOffset("Depth Offset", Range( 0 , 2)) = 0.5
		[Header (OPACITY)]_OpacityOcean("Opacity Ocean", Range( 0 , 1)) = 0.045
		_OpacityBeach("Opacity Beach", Range( 0 , 1)) = 0.15
		[Toggle]_IgnoreVertexColor("Ignore Vertex Color", Float) = 1
		[Header (GLOBAL SETTINGS)]_Occlusion("Occlusion", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0.25
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.05
		_SmoothnessVariance("Smoothness Variance", Range( 0 , 1)) = 0.05
		_SmoothnessThreshold("Smoothness Threshold", Range( 0 , 1)) = 0.05
		_Distortion("Distortion", Float) = 0
		[Header (REFRACTION)]_RefractedDepth("Refracted Depth", Range( 0 , 50)) = 30
		_RefractionScale("Refraction Scale", Range( 0 , 1)) = 0.5
		[Header (LIGHTING)]_LightIndirectStrengthSpecular("Light Indirect Strength Specular", Range( 0 , 1)) = 0.5
		_LightIndirectStrengthDiffuse("Light Indirect Strength Diffuse", Range( 0 , 1)) = 0.5
		[KeywordEnum(NdotL,N1,N1V,N1A)] _HighlightDirection("Highlight Direction", Float) = 0
		_HighlightTint("Highlight Tint", Color) = (0.003921569,0.3098039,0.3960784,0.9686275)
		_HighlightOffset("Highlight Offset", Range( -1 , -0.5)) = -1
		_HighlightSharpness("Highlight Sharpness", Range( 0.001 , 1)) = 0.3
		[Header (SHADOW)]_ShadowStrength("Shadow Strength", Range( 0 , 1)) = 0.35
		_ShadowSharpness("Shadow Sharpness", Range( 0.01 , 1)) = 0.01
		_ShadowOffset("Shadow Offset", Range( 0 , 1)) = 0.5
		[Header (REFLECTION)][Toggle]_EnableReflection("Enable Reflection", Float) = 0
		_ReflectionIntensity("Reflection Intensity", Range( 0 , 1)) = 0.3
		_ReflectionWobble("Reflection Wobble", Range( 0 , 1)) = 0.1
		_ReflectionFresnelPower("Reflection Fresnel Power", Range( 0 , 10)) = 1
		_ReflectionFresnelScale("Reflection Fresnel Scale", Range( 0 , 1)) = 0.4
		[Normal][Header (NORMAL MAP)]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalMapTiling("Normal Map Tiling", Float) = 80
		_NormalMapStrength("Normal Map Strength", Range( 0 , 1)) = 0.25
		_NormalMapSpeed("Normal Map Speed", Range( 0 , 0.1)) = 0.02
		_NormalMapTimescale("Normal Map Timescale", Range( 0 , 1)) = 0.25
		[Header (WAVE TESSELLATION)]_TessellationStrength("Tessellation Strength", Range( 0.01 , 1)) = 0.15
		_TessellationMaxDistance("Tessellation Max Distance", Float) = 3000
		[Header (X_Y DIRECTION Z_ STEEPNESS W_ WAVELENGTH)]_Wave001("Wave 001", Vector) = (1,0,0.21,50)
		_Wave002("Wave 002", Vector) = (-1,-0.5,0.21,49)
		_Wave003("Wave 003", Vector) = (0,1,0.21,45)
		_WaveSpeed("Wave Speed", Range( 0 , 5)) = 0.5
		[Header (OCEAN FOAM)][Toggle]_EnableOceanFoam("Enable Ocean Foam ", Float) = 0
		_OceanFoamMap("Ocean Foam Map", 2D) = "white" {}
		_OceanFoamTint("Ocean Foam Tint", Color) = (0.6039216,0.6039216,0.6039216,1)
		_OceanFoamTiling("Ocean Foam Tiling", Range( 0.01 , 100)) = 50
		[Gamma]_OceanFoamStrength("Ocean Foam Strength", Range( 0 , 1)) = 0.55
		_OceanFoamDistance("Ocean Foam Distance", Range( 0 , 1000)) = 40
		_OceanFoamSpeed("Ocean Foam Speed", Range( 0 , 0.5)) = 0.02
		[Header (BEACH FOAM)][Toggle]_EnableBeachFoam("Enable Beach Foam", Float) = 0
		_BeachFoamMap("Beach Foam Map", 2D) = "black" {}
		_BeachFoamTint("Beach Foam Tint", Color) = (0.8196079,0.8196079,0.8196079,1)
		_BeachFoamTiling("Beach Foam Tiling", Range( 0 , 1)) = 0.6
		_BeachFoamStrenght("Beach Foam Strenght", Range( 0.1 , 0.75)) = 0.35
		_foamMax("Beach Foam Distance", Range( 0.1 , 100)) = 18
		_BeachFoamSpeed("Beach Foam Speed", Range( 0 , 0.5)) = 0.09
		[Enum(UnityEngine.Rendering.CullMode)][Header (RENDERING OPTIONS)]_CullMode("Cull Mode", Int) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent-10" "IgnoreProjector" = "True" }
		Cull [_CullMode]
		GrabPass{ }
		GrabPass{ "_GrabTextureWater" }
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Tessellation.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma multi_compile _HIGHLIGHTDIRECTION_NDOTL _HIGHLIGHTDIRECTION_N1 _HIGHLIGHTDIRECTION_N1V _HIGHLIGHTDIRECTION_N1A
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
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
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldPos;
			half ASEVFace : VFACE;
			float4 vertexColor : COLOR;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _CameraDepthTexture_TexelSize;
		uniform int _CullMode;
		uniform float _EnableOceanFoam;
		uniform float4 _OceanFoamTint;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform sampler2D _OceanFoamMap;
		uniform float _OceanFoamSpeed;
		uniform float _OceanFoamTiling;
		uniform float _OceanFoamDistance;
		uniform float _OceanFoamStrength;
		uniform float4 _Wave001;
		uniform float _WaveSpeed;
		uniform float4 _Wave002;
		uniform float4 _Wave003;
		uniform float4 _DepthTint;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _RefractionScale;
		uniform float4 _ShallowTint;
		uniform float4 _WaterTint;
		uniform float _ShallowDepth;
		uniform float _ShallowOffset;
		uniform float _DepthOffset;
		uniform float _RefractedDepth;
		uniform float _EnableBeachFoam;
		uniform sampler2D _BeachFoamMap;
		uniform float _BeachFoamSpeed;
		uniform float _BeachFoamTiling;
		uniform float4 _BeachFoamTint;
		uniform float _foamMax;
		uniform float _BeachFoamStrenght;
		uniform float _NormalMapStrength;
		uniform float _NormalMapTimescale;
		uniform float _NormalMapSpeed;
		uniform float _NormalMapTiling;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _SmoothnessVariance;
		uniform float _SmoothnessThreshold;
		uniform float4 _HighlightTint;
		uniform float _Occlusion;
		uniform float _LightIndirectStrengthSpecular;
		uniform float _HighlightOffset;
		uniform float _HighlightSharpness;
		uniform float _LightIndirectStrengthDiffuse;
		uniform float _ShadowOffset;
		uniform float _ShadowSharpness;
		uniform float _ShadowStrength;
		uniform float _IgnoreVertexColor;
		uniform float _OpacityOcean;
		uniform float _EnableReflection;
		uniform sampler2D _ReflectionTex;
		uniform float _ReflectionWobble;
		uniform float _ReflectionFresnelScale;
		uniform float _ReflectionFresnelPower;
		uniform float _ReflectionIntensity;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTextureWater )
		uniform float _Distortion;
		uniform float _OpacityBeach;
		uniform float _TessellationMaxDistance;
		uniform float _TessellationStrength;


		float4 CalculateObliqueFrustumCorrection(  )
		{
			float x1 = -UNITY_MATRIX_P._31 / (UNITY_MATRIX_P._11 * UNITY_MATRIX_P._34);
			float x2 = -UNITY_MATRIX_P._32 / (UNITY_MATRIX_P._22 * UNITY_MATRIX_P._34);
			return float4(x1, x2, 0, UNITY_MATRIX_P._33 / UNITY_MATRIX_P._34 + x1 * UNITY_MATRIX_P._13 + x2 * UNITY_MATRIX_P._23);
		}


		float FROM( float3 screenPos )
		{
			return UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
		}


		float CorrectedLinearEyeDepth( float z , float correctionFactor )
		{
			return 1.f / (z / UNITY_MATRIX_P._34 + correctionFactor);
		}


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


		void ResetAlpha( Input SurfaceIn , SurfaceOutputStandard SurfaceOut , inout fixed4 FinalColor )
		{
			FinalColor.a = 1;
		}


		float2 AlignWithGrabTexel( float2 uv )
		{
			#if UNITY_UV_STARTS_AT_TOP
			if (_CameraDepthTexture_TexelSize.y < 0) {
				uv.y = 1 - uv.y;
			}
			#endif
			return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) * abs(_CameraDepthTexture_TexelSize.xy);
		}


		float3 WaterSampleWaves( float4 wave , float wspeed , float3 p , inout float3 tangent , inout float3 binormal )
		{
					    float steepness = wave.z;
					    float wavelength = wave.w;
					    float k = 2 * UNITY_PI / wavelength;
						float c = sqrt(9.8 / k);
						float2 d = normalize(wave.xy);
						float s = _Time.y * wspeed;
						float f = k * (dot(d, p.xz) - c *  s );
						float a = steepness / k;
						tangent += float3(
							-d.x * d.x * (steepness * sin(f)),
							d.x * (steepness * cos(f)),
							-d.x * d.y * (steepness * sin(f))
						);
						binormal += float3(
							-d.x * d.y * (steepness * sin(f)),
							d.y * (steepness * cos(f)),
							-d.y * d.y * (steepness * sin(f))
						);
						return float3(
							d.x * (a * cos(f)),
							a * sin(f),
							d.y * (a * cos(f))
						);
					
		}


		float GetRefractedDepth2814( float3 tangentSpaceNormal , float4 screenPos , inout float2 uv )
		{
			float2 uvOffset = tangentSpaceNormal.xy;
			uvOffset.y *= _CameraDepthTexture_TexelSize.z * abs(_CameraDepthTexture_TexelSize.y);
			uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);
			float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
			float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
			float depthDifference = backgroundDepth - surfaceDepth;
			uvOffset *= saturate(depthDifference);
			uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);
			backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
			return depthDifference = backgroundDepth - surfaceDepth;
		}


		float CallFresnelLerpFast( float3 specColor , float3 grazingTerm , float nv )
		{
			return FresnelLerpFast (specColor, grazingTerm, nv);
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float Wave001_W_WAVE_LENGHT2699 = _Wave001.w;
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, 0.0,_TessellationMaxDistance,( _TessellationStrength * Wave001_W_WAVE_LENGHT2699 ));
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float clampDepth2283 = SAMPLE_DEPTH_TEXTURE_LOD( _CameraDepthTexture, float4( ase_screenPosNorm.xy, 0, 0 ) );
			float z2286 = clampDepth2283;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 localCalculateObliqueFrustumCorrection2509 = CalculateObliqueFrustumCorrection();
			float dotResult2510 = dot( float4( ase_vertex3Pos , 0.0 ) , localCalculateObliqueFrustumCorrection2509 );
			float NEW_OBLIQUE_FRUSTUM_CORRECTION2512 = dotResult2510;
			float correctionFactor2286 = NEW_OBLIQUE_FRUSTUM_CORRECTION2512;
			float localCorrectedLinearEyeDepth2286 = CorrectedLinearEyeDepth( z2286 , correctionFactor2286 );
			float3 screenPos2287 = ase_screenPos.xyz;
			float localFROM2287 = FROM( screenPos2287 );
			float temp_output_2288_0 = ( localCorrectedLinearEyeDepth2286 - localFROM2287 );
			float NEW_BEHIND_DEPTH_032369 = saturate( temp_output_2288_0 );
			float2 temp_cast_2 = (_OceanFoamSpeed).xx;
			float2 temp_cast_3 = (_OceanFoamTiling).xx;
			float2 uv_TexCoord1726 = v.texcoord.xy * temp_cast_3;
			float2 _Vector0 = float2(2,4);
			float cos1730 = cos( _Vector0.x );
			float sin1730 = sin( _Vector0.x );
			float2 rotator1730 = mul( uv_TexCoord1726 - float2( 0,0 ) , float2x2( cos1730 , -sin1730 , sin1730 , cos1730 )) + float2( 0,0 );
			float2 panner1734 = ( 1.0 * _Time.y * temp_cast_2 + rotator1730);
			float2 temp_cast_4 = (_OceanFoamSpeed).xx;
			float cos1725 = cos( _Vector0.y );
			float sin1725 = sin( _Vector0.y );
			float2 rotator1725 = mul( uv_TexCoord1726 - float2( 0,0 ) , float2x2( cos1725 , -sin1725 , sin1725 , cos1725 )) + float2( 0,0 );
			float2 panner1728 = ( 1.0 * _Time.y * temp_cast_4 + rotator1725);
			float2 temp_cast_5 = (_OceanFoamSpeed).xx;
			float2 panner1729 = ( 1.0 * _Time.y * temp_cast_5 + uv_TexCoord1726);
			float screenDepth1713 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_LOD( _CameraDepthTexture, float4( ase_screenPosNorm.xy, 0, 0 ) ));
			float distanceDepth1713 = abs( ( screenDepth1713 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _OceanFoamDistance ) );
			float4 lerpResult1147 = lerp( float4( 0,0,0,0 ) , ( _OceanFoamTint * NEW_BEHIND_DEPTH_032369 * ( ( tex2Dlod( _OceanFoamMap, float4( panner1734, 0, 0.0) ) + tex2Dlod( _OceanFoamMap, float4( panner1728, 0, 0.0) ) + tex2Dlod( _OceanFoamMap, float4( panner1729, 0, 0.0) ) ) / 3.0 ) * saturate( distanceDepth1713 ) ) , _OceanFoamStrength);
			float4 OCAEN_FOAM302 = lerp(float4( 0,0,0,0 ),lerpResult1147,_EnableOceanFoam);
			float4 wave1383 = _Wave001;
			float wspeed1383 = _WaveSpeed;
			float3 p1383 = ase_vertex3Pos;
			float3 tangent1383 = float3(1,0,0);
			float3 binormal1383 = float3(0,0,1);
			float3 localWaterSampleWaves1383 = WaterSampleWaves( wave1383 , wspeed1383 , p1383 , tangent1383 , binormal1383 );
			float4 wave1605 = _Wave002;
			float wspeed1605 = _WaveSpeed;
			float3 p1605 = ase_vertex3Pos;
			float3 tangent1605 = tangent1383;
			float3 binormal1605 = binormal1383;
			float3 localWaterSampleWaves1605 = WaterSampleWaves( wave1605 , wspeed1605 , p1605 , tangent1605 , binormal1605 );
			float4 wave1608 = _Wave003;
			float wspeed1608 = _WaveSpeed;
			float3 p1608 = ase_vertex3Pos;
			float3 tangent1608 = tangent1605;
			float3 binormal1608 = binormal1605;
			float3 localWaterSampleWaves1608 = WaterSampleWaves( wave1608 , wspeed1608 , p1608 , tangent1608 , binormal1608 );
			float4 WAVE_GERSTNER_LOCAL_VERT_OFFSET1382 = ( OCAEN_FOAM302 + float4( localWaterSampleWaves1383 , 0.0 ) + float4( localWaterSampleWaves1605 , 0.0 ) + float4( localWaterSampleWaves1608 , 0.0 ) );
			v.vertex.xyz += WAVE_GERSTNER_LOCAL_VERT_OFFSET1382.rgb;
			float3 normalizeResult1381 = normalize( cross( binormal1608 , tangent1608 ) );
			float3 WAVE_GERSTNER_LOCAL_VERT_NORMAL1384 = normalizeResult1381;
			v.normal = WAVE_GERSTNER_LOCAL_VERT_NORMAL1384;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			SurfaceOutputStandard s1834 = (SurfaceOutputStandard ) 0;
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 normalizeResult2054 = normalize( (WorldNormalVector( i , UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) ) )) );
			float3 NORMAL_MAP_SAMPLE1987 = normalizeResult2054;
			float3 tangentSpaceNormal2814 = ( NORMAL_MAP_SAMPLE1987 * _RefractionScale );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 screenPos2814 = ase_screenPos;
			float2 uv2814 = float2( 0,0 );
			float localGetRefractedDepth2814 = GetRefractedDepth2814( tangentSpaceNormal2814 , screenPos2814 , uv2814 );
			float2 REFRACTED_UV2837 = uv2814;
			float4 screenColor2835 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,REFRACTED_UV2837);
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth2739 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth2739 = saturate( abs( ( screenDepth2739 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _ShallowDepth ) ) );
			float4 lerpResult2752 = lerp( _ShallowTint , _WaterTint , saturate( (distanceDepth2739*1.0 + _ShallowOffset) ));
			float4 lerpResult3015 = lerp( _DepthTint , ( screenColor2835 * lerpResult2752 ) , saturate( (distanceDepth2739*-1.0 + _DepthOffset) ));
			float4 temp_cast_0 = (0.0).xxxx;
			#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch2469 = temp_cast_0;
			#else
				float4 staticSwitch2469 = lerpResult3015;
			#endif
			float4 COLOR_TINT_FINAL444 = staticSwitch2469;
			#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch3190 = 0.0;
			#else
				float staticSwitch3190 = saturate( ( localGetRefractedDepth2814 / _RefractedDepth ) );
			#endif
			float REFRACTED_DEPTH2840 = staticSwitch3190;
			float4 lerpResult3173 = lerp( float4( 0,0,0,0 ) , COLOR_TINT_FINAL444 , REFRACTED_DEPTH2840);
			float clampDepth2283 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			float z2286 = clampDepth2283;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 localCalculateObliqueFrustumCorrection2509 = CalculateObliqueFrustumCorrection();
			float dotResult2510 = dot( float4( ase_vertex3Pos , 0.0 ) , localCalculateObliqueFrustumCorrection2509 );
			float NEW_OBLIQUE_FRUSTUM_CORRECTION2512 = dotResult2510;
			float correctionFactor2286 = NEW_OBLIQUE_FRUSTUM_CORRECTION2512;
			float localCorrectedLinearEyeDepth2286 = CorrectedLinearEyeDepth( z2286 , correctionFactor2286 );
			float3 screenPos2287 = ase_screenPos.xyz;
			float localFROM2287 = FROM( screenPos2287 );
			float temp_output_2288_0 = ( localCorrectedLinearEyeDepth2286 - localFROM2287 );
			float NEW_BEHIND_DEPTH_032369 = saturate( temp_output_2288_0 );
			float2 temp_cast_3 = (_OceanFoamSpeed).xx;
			float2 temp_cast_4 = (_OceanFoamTiling).xx;
			float2 uv_TexCoord1726 = i.uv_texcoord * temp_cast_4;
			float2 _Vector0 = float2(2,4);
			float cos1730 = cos( _Vector0.x );
			float sin1730 = sin( _Vector0.x );
			float2 rotator1730 = mul( uv_TexCoord1726 - float2( 0,0 ) , float2x2( cos1730 , -sin1730 , sin1730 , cos1730 )) + float2( 0,0 );
			float2 panner1734 = ( 1.0 * _Time.y * temp_cast_3 + rotator1730);
			float2 temp_cast_5 = (_OceanFoamSpeed).xx;
			float cos1725 = cos( _Vector0.y );
			float sin1725 = sin( _Vector0.y );
			float2 rotator1725 = mul( uv_TexCoord1726 - float2( 0,0 ) , float2x2( cos1725 , -sin1725 , sin1725 , cos1725 )) + float2( 0,0 );
			float2 panner1728 = ( 1.0 * _Time.y * temp_cast_5 + rotator1725);
			float2 temp_cast_6 = (_OceanFoamSpeed).xx;
			float2 panner1729 = ( 1.0 * _Time.y * temp_cast_6 + uv_TexCoord1726);
			float screenDepth1713 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth1713 = abs( ( screenDepth1713 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _OceanFoamDistance ) );
			float4 lerpResult1147 = lerp( float4( 0,0,0,0 ) , ( _OceanFoamTint * NEW_BEHIND_DEPTH_032369 * ( ( tex2D( _OceanFoamMap, panner1734 ) + tex2D( _OceanFoamMap, panner1728 ) + tex2D( _OceanFoamMap, panner1729 ) ) / 3.0 ) * saturate( distanceDepth1713 ) ) , _OceanFoamStrength);
			float4 OCAEN_FOAM302 = lerp(float4( 0,0,0,0 ),lerpResult1147,_EnableOceanFoam);
			float3 ase_worldPos = i.worldPos;
			float4 appendResult2675 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 temp_output_2647_0 = ( ( appendResult2675 / 10.0 ) * _BeachFoamTiling );
			float2 _Vector3 = float2(2,1);
			float cos1786 = cos( _Vector3.x );
			float sin1786 = sin( _Vector3.x );
			float2 rotator1786 = mul( temp_output_2647_0.xy - float2( 0,0 ) , float2x2( cos1786 , -sin1786 , sin1786 , cos1786 )) + float2( 0,0 );
			float2 panner2654 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator1786);
			float cos1787 = cos( _Vector3.y );
			float sin1787 = sin( _Vector3.y );
			float2 rotator1787 = mul( ( temp_output_2647_0 * ( _BeachFoamTiling * 5.0 ) ).xy - float2( 0,0 ) , float2x2( cos1787 , -sin1787 , sin1787 , cos1787 )) + float2( 0,0 );
			float2 panner2652 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator1787);
			float4 tex2DNode1763 = tex2D( _BeachFoamMap, panner2652 );
			float4 lerpResult2707 = lerp( float4( 0,0,0,0 ) , ( tex2D( _BeachFoamMap, panner2654 ) * tex2DNode1763 * tex2DNode1763.a * NEW_BEHIND_DEPTH_032369 ) , _BeachFoamTint);
			float3 unityObjectToViewPos2395 = UnityObjectToViewPos( ase_vertex3Pos );
			float NEW_SCREEN_DEPTH2306 = localCorrectedLinearEyeDepth2286;
			float temp_output_2409_0 = ( unityObjectToViewPos2395.z + NEW_SCREEN_DEPTH2306 );
			float NEW_CLOSENESS2373 = saturate( ( 1.0 / distance( _WorldSpaceCameraPos , ase_worldPos ) ) );
			float4 lerpResult1768 = lerp( abs( ( lerpResult2707 - float4( 0,0,0,0 ) ) ) , float4( 0,0,0,0 ) , saturate( ( ( ( temp_output_2409_0 - 0.001 ) * NEW_CLOSENESS2373 ) / ( ( _foamMax - 0.001 ) * NEW_CLOSENESS2373 ) ) ));
			float4 clampResult1775 = clamp( lerpResult1768 , float4( 0,0,0,0 ) , float4( 0.8602941,0.8602941,0.8602941,0 ) );
			float4 temp_cast_9 = (( 1.0 - _BeachFoamStrenght )).xxxx;
			float4 BEACH_FOAM834 = lerp(float4( 0,0,0,0 ),pow( clampResult1775 , temp_cast_9 ),_EnableBeachFoam);
			s1834.Albedo = ( lerpResult3173 + OCAEN_FOAM302 + BEACH_FOAM834 ).rgb;
			float mulTime1936 = _Time.y * _NormalMapTimescale;
			float2 temp_cast_11 = (_NormalMapSpeed).xx;
			float2 temp_cast_12 = (_NormalMapTiling).xx;
			float2 uv_TexCoord1821 = i.uv_texcoord * temp_cast_12;
			float2 _Vector1 = float2(2,4);
			float cos1825 = cos( _Vector1.x );
			float sin1825 = sin( _Vector1.x );
			float2 rotator1825 = mul( uv_TexCoord1821 - float2( 0,0 ) , float2x2( cos1825 , -sin1825 , sin1825 , cos1825 )) + float2( 0,0 );
			float2 panner1827 = ( mulTime1936 * temp_cast_11 + rotator1825);
			float2 temp_cast_13 = (_NormalMapSpeed).xx;
			float cos1823 = cos( _Vector1.y );
			float sin1823 = sin( _Vector1.y );
			float2 rotator1823 = mul( uv_TexCoord1821 - float2( 0,0 ) , float2x2( cos1823 , -sin1823 , sin1823 , cos1823 )) + float2( 0,0 );
			float2 panner1828 = ( mulTime1936 * temp_cast_13 + rotator1823);
			float2 temp_cast_14 = (_NormalMapSpeed).xx;
			float2 panner1826 = ( mulTime1936 * temp_cast_14 + uv_TexCoord1821);
			float3 lerpResult1831 = lerp( BlendNormals( BlendNormals( UnpackScaleNormal( tex2D( _NormalMap, panner1827 ), _NormalMapStrength ) , UnpackScaleNormal( tex2D( _NormalMap, panner1828 ), _NormalMapStrength ) ) , UnpackScaleNormal( tex2D( _NormalMap, panner1826 ), _NormalMapStrength ) ) , float3( 0,0,0 ) , float4(0,0,0,0.5607843).rgb);
			float3 normalizeResult2155 = normalize( (WorldNormalVector( i , lerpResult1831 )) );
			float3 clampResult2253 = clamp( float3( 0,0,0 ) , normalizeResult2155 , float3( 1,1,1 ) );
			float3 NORMAL_MAP982 = clampResult2253;
			s1834.Normal = NORMAL_MAP982;
			s1834.Emission = float3( 0,0,0 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult2043 = dot( NORMAL_MAP_SAMPLE1987 , ase_worldlightDir );
			float NdotL2044 = dotResult2043;
			s1834.Metallic = pow( max( NdotL2044 , 0.0 ) , exp2( _Metallic ) );
			float perceptualSmoothness2410 = _Smoothness;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 geometricNormalWS2410 = ase_worldNormal;
			float screenSpaceVariance2410 = _SmoothnessVariance;
			float threshold2410 = _SmoothnessThreshold;
			float localGetGeometricNormalVariance2410 = GetGeometricNormalVariance( perceptualSmoothness2410 , geometricNormalWS2410 , screenSpaceVariance2410 , threshold2410 );
			float switchResult2427 = (((i.ASEVFace>0)?(localGetGeometricNormalVariance2410):(0.0)));
			float NEW_SMOOTHNESS2432 = switchResult2427;
			s1834.Smoothness = NEW_SMOOTHNESS2432;
			s1834.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi1834 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g1834 = UnityGlossyEnvironmentSetup( s1834.Smoothness, data.worldViewDir, s1834.Normal, float3(0,0,0));
			gi1834 = UnityGlobalIllumination( data, s1834.Occlusion, s1834.Normal, g1834 );
			#endif

			float3 surfResult1834 = LightingStandard ( s1834, viewDir, gi1834 ).rgb;
			surfResult1834 += s1834.Emission;

			#ifdef UNITY_PASS_FORWARDADD//1834
			surfResult1834 -= s1834.Emission;
			#endif//1834
			float3 temp_cast_16 = (1.0).xxx;
			float3 indirectNormal1991 = WorldNormalVector( i , NORMAL_MAP_SAMPLE1987 );
			float4 HIGHLIGHT_TINT3077 = _HighlightTint;
			float temp_output_3041_0 = (HIGHLIGHT_TINT3077).a;
			Unity_GlossyEnvironmentData g1991 = UnityGlossyEnvironmentSetup( temp_output_3041_0, data.worldViewDir, indirectNormal1991, float3(0,0,0));
			float3 indirectSpecular1991 = UnityGI_IndirectSpecular( data, _Occlusion, indirectNormal1991, g1991 );
			float3 lerpResult2027 = lerp( temp_cast_16 , indirectSpecular1991 , ( 1.0 - _LightIndirectStrengthSpecular ));
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LIGHT_COLOR_FALLOFF3037 = ( ase_lightColor.rgb * ase_lightAtten );
			float SPECULAR_TINT_A1_53060 = pow( temp_output_3041_0 , 1.5 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult3048 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float dotResult3049 = dot( normalizeResult3048 , NORMAL_MAP_SAMPLE1987 );
			float4 wave1608 = _Wave003;
			float wspeed1608 = _WaveSpeed;
			float3 p1608 = ase_vertex3Pos;
			float4 wave1605 = _Wave002;
			float wspeed1605 = _WaveSpeed;
			float3 p1605 = ase_vertex3Pos;
			float4 wave1383 = _Wave001;
			float wspeed1383 = _WaveSpeed;
			float3 p1383 = ase_vertex3Pos;
			float3 tangent1383 = float3(1,0,0);
			float3 binormal1383 = float3(0,0,1);
			float3 localWaterSampleWaves1383 = WaterSampleWaves( wave1383 , wspeed1383 , p1383 , tangent1383 , binormal1383 );
			float3 tangent1605 = tangent1383;
			float3 binormal1605 = binormal1383;
			float3 localWaterSampleWaves1605 = WaterSampleWaves( wave1605 , wspeed1605 , p1605 , tangent1605 , binormal1605 );
			float3 tangent1608 = tangent1605;
			float3 binormal1608 = binormal1605;
			float3 localWaterSampleWaves1608 = WaterSampleWaves( wave1608 , wspeed1608 , p1608 , tangent1608 , binormal1608 );
			float3 normalizeResult1381 = normalize( cross( binormal1608 , tangent1608 ) );
			float3 WAVE_GERSTNER_LOCAL_VERT_NORMAL1384 = normalizeResult1381;
			float dotResult3064 = dot( normalizeResult3048 , WAVE_GERSTNER_LOCAL_VERT_NORMAL1384 );
			float dotResult3066 = dot( normalizeResult3048 , NORMAL_MAP982 );
			#if defined(_HIGHLIGHTDIRECTION_NDOTL)
				float staticSwitch3063 = NdotL2044;
			#elif defined(_HIGHLIGHTDIRECTION_N1)
				float staticSwitch3063 = dotResult3049;
			#elif defined(_HIGHLIGHTDIRECTION_N1V)
				float staticSwitch3063 = dotResult3064;
			#elif defined(_HIGHLIGHTDIRECTION_N1A)
				float staticSwitch3063 = dotResult3066;
			#else
				float staticSwitch3063 = NdotL2044;
			#endif
			float HIGHLIGHTS3070 = saturate( ( ( staticSwitch3063 + _HighlightOffset ) / ( ( 1.0 - _HighlightTint.a ) * ( 1.0 - _HighlightSharpness ) ) ) );
			float3 INDIRECT_SPECULAR532 = ( lerpResult2027 * LIGHT_COLOR_FALLOFF3037 * SPECULAR_TINT_A1_53060 * HIGHLIGHTS3070 * (HIGHLIGHT_TINT3077).rgb );
			float3 temp_cast_17 = (1.0).xxx;
			float3 temp_cast_18 = (NdotL2044).xxx;
			UnityGI gi1997 = gi;
			float3 diffNorm1997 = temp_cast_18;
			gi1997 = UnityGI_Base( data, 1, diffNorm1997 );
			float3 indirectDiffuse1997 = gi1997.indirect.diffuse + diffNorm1997 * 0.0001;
			float3 lerpResult1999 = lerp( temp_cast_17 , indirectDiffuse1997 , ( 1.0 - _LightIndirectStrengthDiffuse ));
			float temp_output_2008_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			float lerpResult2015 = lerp( temp_output_2008_0 , ( saturate( ( ( NdotL2044 + _ShadowOffset ) / _ShadowSharpness ) ) * ase_lightAtten ) , _ShadowStrength);
			float3 LIGHT_SHADOWS2113 = saturate( ( ( lerpResult1999 * ase_lightColor.a * temp_output_2008_0 ) + ( ase_lightColor.rgb * lerpResult2015 ) ) );
			float3 specColor2452 = float3( 0,0,0 );
			float3 temp_cast_19 = (NEW_SMOOTHNESS2432).xxx;
			float3 grazingTerm2452 = temp_cast_19;
			float dotResult2444 = dot( ase_worldNormal , ase_worldViewDir );
			float nv2452 = dotResult2444;
			float localCallFresnelLerpFast2452 = CallFresnelLerpFast( specColor2452 , grazingTerm2452 , nv2452 );
			float NEW_FINAL_OPACITY2371 = ( lerp(1.0,i.vertexColor.a,_IgnoreVertexColor) * ( 1.0 - _OpacityOcean ) );
			float NEW_REDUCE_AT_HORIZON2527 = max( 0.0 , ( 1.0 - ( localCallFresnelLerpFast2452 * NEW_FINAL_OPACITY2371 ) ) );
			float HIGHLIGHT_DIRECTION3182 = staticSwitch3063;
			float fresnelNdotV3186 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode3186 = ( 0.0 + _ReflectionFresnelScale * pow( 1.0 - fresnelNdotV3186, _ReflectionFresnelPower ) );
			float4 lerpResult2823 = lerp( float4( 0,0,0,0 ) , tex2D( _ReflectionTex, ( ase_screenPosNorm + ( HIGHLIGHT_DIRECTION3182 * _ReflectionWobble ) ).xy ) , ( fresnelNode3186 * _ReflectionIntensity ));
			float4 temp_cast_22 = (0.0).xxxx;
			#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch3174 = temp_cast_22;
			#else
				float4 staticSwitch3174 = lerp(float4( 0,0,0,0 ),lerpResult2823,_EnableReflection);
			#endif
			float4 PLANAR_REFLECTION2841 = staticSwitch3174;
			float fresnelNdotV2366 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode2366 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV2366, 5.0 ) );
			float3 lerpResult2381 = lerp( float3( 0,0,0 ) , NORMAL_MAP982 , ( NEW_FINAL_OPACITY2371 * _Distortion * NEW_CLOSENESS2373 * ( 1.0 - fresnelNode2366 ) * NEW_BEHIND_DEPTH_032369 ));
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 temp_output_2382_0 = ( float4( lerpResult2381 , 0.0 ) + ase_grabScreenPosNorm );
			float4 screenColor2416 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTextureWater,temp_output_2382_0.xy/temp_output_2382_0.w);
			float4 temp_cast_24 = (0.0).xxxx;
			#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch2422 = temp_cast_24;
			#else
				float4 staticSwitch2422 = screenColor2416;
			#endif
			float4 New_GRAB_SCREEN_COLOR2433 = staticSwitch2422;
			float DEPTH_TINT_A3003 = ( saturate( (distanceDepth2739*-5.0 + 1.0) ) * _OpacityBeach );
			#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch3180 = 0.0;
			#else
				float staticSwitch3180 = ( 1.0 - ( ( 1.0 - DEPTH_TINT_A3003 ) * NEW_FINAL_OPACITY2371 ) );
			#endif
			float4 lerpResult3006 = lerp( ( float4( ( ( surfResult1834 + INDIRECT_SPECULAR532 ) * LIGHT_SHADOWS2113 * NEW_REDUCE_AT_HORIZON2527 ) , 0.0 ) + PLANAR_REFLECTION2841 ) , New_GRAB_SCREEN_COLOR2433 , staticSwitch3180);
			c.rgb = lerpResult3006.rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows noinstancing exclude_path:deferred nodirlightmap vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
				float4 screenPos : TEXCOORD2;
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
				vertexDataFunc( v );
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
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
96;51.5;1664;953;9454.002;3116.144;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;2119;-8711.637,-807.0154;Inherit;False;3170.137;1932.604;Comment;6;1384;1382;1987;982;1387;1342;WAVES;0.7372549,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1342;-8674.525,-694.3645;Inherit;False;2696.459;748.5309;Comment;25;1193;2053;2054;2253;2155;2156;1831;1350;1938;1821;1935;1936;1833;1832;1988;1829;1830;1826;1827;1828;1369;1825;1823;1822;1349;NORMAL_MAP;0.7379308,0,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1349;-8642.154,-171.2029;Float;False;Property;_NormalMapTiling;Normal Map Tiling;33;0;Create;True;0;0;False;0;80;80;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1935;-8333.448,-14.50844;Float;False;Property;_NormalMapTimescale;Normal Map Timescale;36;0;Create;True;0;0;False;0;0.25;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1821;-8341.376,-196.2878;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;2281;-7435.345,-4767.571;Inherit;False;1878.871;304.1857;Oblique Frustum Correction Factor;4;2512;2510;2509;2508;OBLIQUE_FRUSTUM_CORRECTION;0,1,0.1391485,1;0;0
Node;AmplifyShaderEditor.Vector2Node;1822;-8246.063,-474.2522;Float;False;Constant;_Vector1;Vector 1;7;0;Create;True;0;0;False;0;2,4;2,4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;2120;-9349.076,-3149.296;Inherit;False;3822.243;2172.563;Comment;4;302;834;1743;1797;FOAM;1,0.654902,0.3019608,1;0;0
Node;AmplifyShaderEditor.CustomExpressionNode;2509;-7415.786,-4558.847;Float;False;float x1 = -UNITY_MATRIX_P._31 / (UNITY_MATRIX_P._11 * UNITY_MATRIX_P._34)@$float x2 = -UNITY_MATRIX_P._32 / (UNITY_MATRIX_P._22 * UNITY_MATRIX_P._34)@$return float4(x1, x2, 0, UNITY_MATRIX_P._33 / UNITY_MATRIX_P._34 + x1 * UNITY_MATRIX_P._13 + x2 * UNITY_MATRIX_P._23)@;4;False;0;CalculateObliqueFrustumCorrection;False;True;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;2508;-7409.399,-4709.119;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;1797;-9322.209,-2088.156;Inherit;False;3507.596;1071.366;Comment;34;2649;1783;2646;2647;1781;2641;2640;2675;2674;541;1776;1775;2057;1768;1774;1771;1770;2707;1766;1765;1779;1764;1763;2652;2672;2654;2651;2650;1787;1786;1785;2644;2645;2391;BEACH_FOAM;1,0.654902,0.3019608,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;1936;-8014.979,-11.81642;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1369;-8114.753,-97.80213;Float;False;Property;_NormalMapSpeed;Normal Map Speed;35;0;Create;True;0;0;False;0;0.02;0.02;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;1825;-8021.738,-573.2198;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;1823;-8016.699,-383.2285;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;2510;-7072.701,-4704.895;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1938;-7810.644,-20.25026;Float;False;Property;_NormalMapStrength;Normal Map Strength;34;0;Create;True;0;0;False;0;0.25;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;1827;-7765.548,-573.8427;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;1828;-7763.486,-383.1416;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2674;-9306.152,-2038.263;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;2512;-6205.531,-4707.495;Float;False;NEW_OBLIQUE_FRUSTUM_CORRECTION;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2282;-7432.715,-4339.828;Inherit;False;1901.699;425.0579;Clip BG from Grab;12;2369;2291;2362;2288;2287;2384;2285;2306;2284;2286;2496;2283;BEHIND_DEPTH_03;0,1,0.1568628,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1387;-8624.912,158.3432;Inherit;False;2647.598;939.8988;https://catlikecoding.com/unity/tutorials/flow/waves/;19;1263;1340;1381;1380;1608;1610;1605;1607;1383;1378;1379;1385;1377;2702;2701;2699;1609;1606;1386;GERSTNER WAVE;0.7372549,0,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;1988;-7498.554,-624.1205;Inherit;True;Property;_TextureSample6;Texture Sample 6;32;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Instance;1193;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;1826;-7764.816,-194.5213;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1829;-7501.525,-404.3293;Inherit;True;Property;_TextureSample3;Texture Sample 3;32;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Instance;1193;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;2675;-9104.498,-2036.451;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;2640;-9121.654,-1879.138;Float;False;Constant;_Float3;Float 3;63;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;1832;-7150.043,-517.7675;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1830;-7496.039,-204.3661;Inherit;True;Property;_TextureSample5;Texture Sample 5;32;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Instance;1193;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;1377;-7988.897,494.9915;Float;False;Constant;_WaveABINormal;Wave A BI Normal;24;0;Create;True;0;0;False;0;0,0,1;0,0,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;1379;-7970.83,727.8132;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1781;-9112.9,-1556.309;Float;False;Property;_BeachFoamTiling;Beach Foam Tiling;55;0;Create;True;0;0;False;1;;0.6;0.169;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2284;-7270.652,-4097.571;Inherit;False;2512;NEW_OBLIQUE_FRUSTUM_CORRECTION;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;2283;-7410.465,-4275.284;Inherit;False;1;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2337;-8663.614,-3815.121;Inherit;False;1085.674;600.1067;;6;2373;2368;2361;2356;2349;2352;CLOSENESS;0,1,0.1568628,1;0;0
Node;AmplifyShaderEditor.Vector4Node;1386;-8555.345,285.5197;Float;False;Property;_Wave001;Wave 001;39;0;Create;True;0;0;False;1;Header (X_Y DIRECTION Z_ STEEPNESS W_ WAVELENGTH);1,0,0.21,50;2.8,-0.5,0.25,11.7;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;2641;-8953.135,-2037.628;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;1385;-7989.67,653.1044;Float;False;Property;_WaveSpeed;Wave Speed;42;0;Create;True;0;0;False;0;0.5;0.4;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2285;-6701.503,-4084.308;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;1378;-7984.877,351.0539;Float;False;Constant;_WaveATangent;Wave A Tangent;23;0;Create;True;0;0;False;0;1,0,0;1,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;1193;-6762.724,-613.0348;Inherit;True;Property;_NormalMap;Normal Map;32;1;[Normal];Create;True;0;0;False;1;Header (NORMAL MAP);None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;1833;-6924.709,-224.1106;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;1606;-8574.604,590.4536;Float;False;Property;_Wave002;Wave 002;40;0;Create;True;0;0;False;0;-1,-0.5,0.21,49;-1.52,3.31,0.28,42;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;2286;-6865.877,-4270.077;Float;False;return 1.f / (z / UNITY_MATRIX_P._34 + correctionFactor)@;1;False;2;True;z;FLOAT;0;In;;Float;False;True;correctionFactor;FLOAT;0;In;;Float;False;CorrectedLinearEyeDepth;False;True;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;2349;-8616.48,-3512.332;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;2352;-8546.063,-3358.209;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;1350;-6918.232,-125.3984;Float;False;Constant;_Color2;Color 2;7;0;Create;True;0;0;False;0;0,0,0,0.5607843;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;1383;-7738.356,288.997;Float;False;$		    float steepness = wave.z@$		    float wavelength = wave.w@$		    float k = 2 * UNITY_PI / wavelength@$			float c = sqrt(9.8 / k)@$			float2 d = normalize(wave.xy)@$			float s = _Time.y * wspeed@$			float f = k * (dot(d, p.xz) - c *  s )@$			float a = steepness / k@$			tangent += float3($				-d.x * d.x * (steepness * sin(f)),$				d.x * (steepness * cos(f)),$				-d.x * d.y * (steepness * sin(f))$			)@$$			binormal += float3($				-d.x * d.y * (steepness * sin(f)),$				d.y * (steepness * cos(f)),$				-d.y * d.y * (steepness * sin(f))$			)@$$			return float3($				d.x * (a * cos(f)),$				a * sin(f),$				d.y * (a * cos(f))$			)@$		;3;False;5;True;wave;FLOAT4;0,0,0,0;In;;Float;False;True;wspeed;FLOAT;0;In;;Float;False;True;p;FLOAT3;0,0,0;In;;Float;False;True;tangent;FLOAT3;0,0,0;InOut;;Float;False;True;binormal;FLOAT3;0,0,0;InOut;;Float;False;WaterSampleWaves;False;False;0;5;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;3;FLOAT3;0;FLOAT3;4;FLOAT3;5
Node;AmplifyShaderEditor.CustomExpressionNode;2287;-6498.384,-4081.398;Float;False;return UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z)@;1;False;1;True;screenPos;FLOAT3;0,0,0;In;;Float;False;FROM;False;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2647;-8816.239,-2037.166;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldNormalVector;2053;-6452.992,-606.0179;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2646;-8801.813,-1553.751;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;2288;-6249.357,-4264.694;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;1605;-7420.789,593.3489;Float;False;$		    float steepness = wave.z@$		    float wavelength = wave.w@$		    float k = 2 * UNITY_PI / wavelength@$			float c = sqrt(9.8 / k)@$			float2 d = normalize(wave.xy)@$			float s = _Time.y * wspeed@$			float f = k * (dot(d, p.xz) - c *  s )@$			float a = steepness / k@$			tangent += float3($				-d.x * d.x * (steepness * sin(f)),$				d.x * (steepness * cos(f)),$				-d.x * d.y * (steepness * sin(f))$			)@$$			binormal += float3($				-d.x * d.y * (steepness * sin(f)),$				d.y * (steepness * cos(f)),$				-d.y * d.y * (steepness * sin(f))$			)@$$			return float3($				d.x * (a * cos(f)),$				a * sin(f),$				d.y * (a * cos(f))$			)@$		;3;False;5;True;wave;FLOAT4;0,0,0,0;In;;Float;False;True;wspeed;FLOAT;0;In;;Float;False;True;p;FLOAT3;0,0,0;In;;Float;False;True;tangent;FLOAT3;0,0,0;InOut;;Float;False;True;binormal;FLOAT3;0,0,0;InOut;;Float;False;WaterSampleWaves;False;False;0;5;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;3;FLOAT3;0;FLOAT3;4;FLOAT3;5
Node;AmplifyShaderEditor.Vector2Node;2644;-8425.567,-1924.772;Float;False;Constant;_BeachFoamPainDirection001;Beach Foam Pain Direction 001;45;0;Create;True;0;0;False;0;1,0;-4,-4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;3043;-5478.416,-141.9776;Inherit;False;2226.983;1512.149;Comment;4;2841;2840;2843;2808;;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.Vector2Node;1783;-8641.248,-1840.34;Float;False;Constant;_Vector3;Vector 3;53;0;Create;True;0;0;False;0;2,1;1,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector4Node;1609;-8574.545,877.152;Float;False;Property;_Wave003;Wave 003;41;0;Create;True;0;0;False;0;0,1,0.21,45;2.3,3.94,0.2,22.3;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;2645;-8429.022,-1706.237;Float;False;Constant;_BeachFoamPainDirection002;Beach Foam Pain Direction 002;46;0;Create;True;0;0;False;0;1,0;-3,-5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.NormalizeNode;2054;-6249.098,-605.2166;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2649;-8627.357,-1576.009;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;1831;-6690.361,-224.474;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1785;-8409.613,-1788.254;Float;False;Property;_BeachFoamSpeed;Beach Foam Speed;58;0;Create;True;0;0;False;0;0.09;0.06;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2391;-8230.119,-1396.536;Inherit;False;1492.065;359.8533;;14;2457;2455;2448;2447;2494;2435;2429;2430;2428;2424;2409;2400;2395;2507;FOAM_FADE;0,1,0.05740881,1;0;0
Node;AmplifyShaderEditor.DistanceOpNode;2356;-8297.93,-3423.056;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2306;-6582.172,-4182.337;Float;False;NEW_SCREEN_DEPTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;1786;-8421.459,-2041.702;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;2362;-6005.591,-4257.858;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;1787;-8419.871,-1577.257;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2361;-8135.937,-3438.948;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1879;-5483.442,-2938.374;Inherit;False;2178.435;2508.837;Comment;8;3070;532;3036;2111;2110;3069;2040;2113;LIGHTING;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;2507;-8221.346,-1356.984;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1987;-5928.588,-619.0297;Float;True;NORMAL_MAP_SAMPLE;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2651;-8100.203,-1920.067;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2650;-8100.358,-1695.457;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;2808;-5440.365,930.1021;Inherit;False;1868.514;356.2461;https://catlikecoding.com/unity/tutorials/flow/looking-through-water/;13;2821;2825;2837;2816;2815;2814;2813;2812;2811;2809;2810;3191;3190;REFRACTED_DEPTH;0.945098,0.9386187,0.02352941,1;0;0
Node;AmplifyShaderEditor.CustomExpressionNode;1608;-7133.278,890.7126;Float;False;$		    float steepness = wave.z@$		    float wavelength = wave.w@$		    float k = 2 * UNITY_PI / wavelength@$			float c = sqrt(9.8 / k)@$			float2 d = normalize(wave.xy)@$			float s = _Time.y * wspeed@$			float f = k * (dot(d, p.xz) - c *  s )@$			float a = steepness / k@$			tangent += float3($				-d.x * d.x * (steepness * sin(f)),$				d.x * (steepness * cos(f)),$				-d.x * d.y * (steepness * sin(f))$			)@$$			binormal += float3($				-d.x * d.y * (steepness * sin(f)),$				d.y * (steepness * cos(f)),$				-d.y * d.y * (steepness * sin(f))$			)@$$			return float3($				d.x * (a * cos(f)),$				a * sin(f),$				d.y * (a * cos(f))$			)@$		;3;False;5;True;wave;FLOAT4;0,0,0,0;In;;Float;False;True;wspeed;FLOAT;0;In;;Float;False;True;p;FLOAT3;0,0,0;In;;Float;False;True;tangent;FLOAT3;0,0,0;InOut;;Float;False;True;binormal;FLOAT3;0,0,0;InOut;;Float;False;WaterSampleWaves;False;False;0;5;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;3;FLOAT3;0;FLOAT3;4;FLOAT3;5
Node;AmplifyShaderEditor.WorldNormalVector;2156;-6522.359,-227.0932;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;1743;-8796.363,-2998.165;Inherit;False;2951.165;838.6112;Comment;23;1148;1147;1146;1720;1717;2610;1738;1737;1739;1716;1732;1736;1729;1728;1734;1730;1731;1725;1733;1727;1726;2148;2122;OCAEN_FOAM;1,0.654902,0.3019608,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;2400;-8091.641,-1160.208;Inherit;False;2306;NEW_SCREEN_DEPTH;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2810;-5409.704,982.753;Inherit;False;1987;NORMAL_MAP_SAMPLE;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;2809;-5416.724,1067.024;Float;False;Property;_RefractionScale;Refraction Scale;17;0;Create;True;0;0;False;0;0.5;0.091;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.UnityObjToViewPosHlpNode;2395;-7989.4,-1356.546;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;2368;-7991.071,-3438.099;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CrossProductOpNode;1380;-6800.69,908.8627;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;2040;-4466.762,-1667.484;Inherit;False;882.827;293.7588;Comment;5;2859;2042;2043;2041;2044;N dot L;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1733;-8774.63,-2583.244;Float;False;Property;_OceanFoamTiling;Ocean Foam Tiling;48;0;Create;True;0;0;False;1;;50;50;0.01;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2525;-5481.984,-5880.257;Inherit;False;2606.732;1281.2;hex code 4E83A9FF;28;2835;2836;3195;3015;2752;2746;954;345;2743;2744;2745;3003;3157;2999;444;2548;2469;3000;3001;2467;3002;442;3169;3171;3170;3168;2739;3193;COLOR_TINT;0,0.3020356,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;2654;-7943.947,-2044.246;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;2652;-7930.526,-1577.726;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2369;-5816.738,-4261.753;Float;False;NEW_BEHIND_DEPTH_03;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;2155;-6322.144,-226.9856;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;2672;-7973.422,-1820.059;Float;True;Property;_BeachFoamMap;Beach Foam Map;53;0;Create;True;0;0;False;0;None;None;False;black;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.CommentaryNode;3069;-5437.261,-2264.218;Inherit;False;1852.987;549.937;Comment;23;3058;3057;3056;3077;3076;3055;3059;3084;3063;3053;3064;3049;3066;3054;3052;3048;3050;3065;3067;3047;3046;3045;3182;HIGHLIGHTS;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2811;-5134.769,1106.927;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1763;-7643.917,-1602.54;Inherit;True;Property;_TextureSample0;Texture Sample 0;41;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;1381;-6619.871,909.4147;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2409;-7774.397,-1350.78;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1726;-8482.628,-2542.321;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;2041;-4450.213,-1617.226;Inherit;False;1987;NORMAL_MAP_SAMPLE;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2812;-5119.534,1000.903;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2373;-7825.355,-3442.176;Float;False;NEW_CLOSENESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2424;-7632.789,-1198.884;Float;False;Property;_foamMax;Beach Foam Distance;57;0;Create;False;0;0;False;0;18;18;0.1;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1779;-7646.207,-1811.416;Inherit;True;2369;NEW_BEHIND_DEPTH_03;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;3046;-5417.163,-2218.57;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;1731;-8408.325,-2825.275;Float;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;False;0;2,4;2,4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ClampOpNode;2253;-6156.74,-249.9267;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;2042;-4425.587,-1519.766;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;2428;-7636.765,-1290.865;Float;False;Constant;_foamMin;Beach Foam Edge;58;0;Create;False;0;0;False;0;0.001;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3193;-5441.165,-5438.112;Inherit;False;Property;_ShallowDepth;Shallow Depth;2;0;Create;True;0;0;False;0;50;50;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3045;-5427.677,-2070.46;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;1764;-7646.816,-2003.658;Inherit;True;Property;_TextureSample1;Texture Sample 1;40;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;2429;-7351.735,-1211.874;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2745;-5147.361,-4924.22;Float;False;Property;_ShallowOffset;Shallow Offset;3;0;Create;True;0;0;False;0;0.2;0.2;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3047;-5187.733,-2210.52;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1384;-5943.563,911.3991;Float;True;WAVE_GERSTNER_LOCAL_VERT_NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;982;-5907.618,-252.4872;Float;True;NORMAL_MAP;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1727;-8221.947,-2475.652;Float;False;Property;_OceanFoamSpeed;Ocean Foam Speed;51;0;Create;True;0;0;False;0;0.02;0.02;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;2043;-4132.591,-1617.939;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;2814;-4905.103,1003.582;Float;False;float2 uvOffset = tangentSpaceNormal.xy@$uvOffset.y *= _CameraDepthTexture_TexelSize.z * abs(_CameraDepthTexture_TexelSize.y)@$uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w)@$$float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv))@$float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z)@$float depthDifference = backgroundDepth - surfaceDepth@$$uvOffset *= saturate(depthDifference)@$uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w)@$backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv))@$return depthDifference = backgroundDepth - surfaceDepth@;1;False;3;True;tangentSpaceNormal;FLOAT3;0,0,0;In;;Float;False;True;screenPos;FLOAT4;0,0,0,0;In;;Float;False;True;uv;FLOAT2;0,0;InOut;;Float;False;GetRefractedDepth;True;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT2;0,0;False;2;FLOAT;0;FLOAT2;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1765;-7274.323,-1800.318;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2435;-7455.771,-1088.739;Inherit;False;2373;NEW_CLOSENESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;1730;-8160.178,-2927.095;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DepthFade;2739;-5130.136,-5470.061;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2744;-5146.767,-5004.415;Float;False;Constant;_WaveColorScale;Wave Color Scale;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;2430;-7351.927,-1345.858;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1766;-7307.66,-1567.691;Float;False;Property;_BeachFoamTint;Beach Foam Tint;54;0;Create;True;0;0;False;0;0.8196079,0.8196079,0.8196079,1;0.8207547,0.8207547,0.8207547,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;1725;-8157.285,-2738.178;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;2122;-7694.508,-2320.785;Inherit;False;746.2388;140.9981;Comment;3;1746;1713;1712;DEPTH_FADE;0.05248642,1,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;3065;-5227.976,-1788.493;Inherit;False;982;NORMAL_MAP;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2448;-7164.021,-1219.209;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2837;-4564.594,1148.521;Float;False;REFRACTED_UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2044;-3882.205,-1617.807;Float;False;NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;1728;-7889.043,-2738.09;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;3067;-5289.647,-1933.558;Inherit;False;1987;NORMAL_MAP_SAMPLE;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;2396;-5485.774,-3508.482;Inherit;False;1905.956;503.9011;Geometric Roughness Factor For specular AARef: Geometry into Shading - http://graphics.pixar.com/library/BumpRoughness/paper.pdf - equation (3);8;2432;2128;2427;2595;2596;2597;2407;2405;SMOOTHNESS;0,1,0.09278011,1;0;0
Node;AmplifyShaderEditor.PannerNode;1734;-7886.81,-2928.793;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalizeNode;3048;-5055.714,-2133.189;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;2110;-5449.291,-1337.188;Inherit;False;1871.18;875.2432;Comment;20;2012;2010;2011;2014;2015;2008;2006;2018;2005;2016;2017;2050;2049;2048;2045;2047;2046;2056;2109;3132;LIGHT_SHADOWS;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2447;-7162.584,-1347.008;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3050;-5400.827,-1861.922;Inherit;False;1384;WAVE_GERSTNER_LOCAL_VERT_NORMAL;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;2743;-4879.305,-5015.043;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2707;-7102.214,-1800.975;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;1729;-7889.059,-2544.693;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;2056;-5423.883,-1286.722;Inherit;False;2044;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1712;-7685.332,-2279.137;Float;False;Property;_OceanFoamDistance;Ocean Foam Distance;50;0;Create;True;0;0;False;0;40;40;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1736;-7680.11,-2954.319;Inherit;True;Property;_TextureSample9;Texture Sample 9;46;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1195;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;2595;-4977.689,-3412.026;Inherit;False;385;227;https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@6.7/manual/Geometric-Specular-Anti-Aliasing.html;1;2410;Geometric Specular Anti-aliasing;0,1,0.1795304,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;3049;-4882.694,-2136.361;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;3064;-4880.606,-2041.285;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2596;-5413.158,-3385.136;Float;False;Property;_SmoothnessVariance;Smoothness Variance;13;0;Create;True;0;0;False;0;0.05;0.432;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1770;-6939.208,-1803.264;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1732;-7682.408,-2760.87;Inherit;True;Property;_TextureSample4;Texture Sample 4;46;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1195;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;2455;-7011.568,-1286.414;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;2407;-5373.13,-3233.11;Inherit;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;1716;-7676.218,-2570.459;Inherit;True;Property;_FOAMTEXTURE;FOAM TEXTURE;46;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;1195;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;2405;-5419.66,-3459.353;Float;False;Property;_Smoothness;Smoothness;12;0;Create;True;0;0;False;0;0.05;0.097;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3168;-4876.733,-5539.463;Float;False;Property;_DepthOffset;Depth Offset;5;0;Create;True;0;0;False;0;0.5;0.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3054;-4433.075,-2066.636;Float;False;Property;_HighlightSharpness;Highlight Sharpness;23;0;Create;True;0;0;False;0;0.3;0.437;0.001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2046;-5427.371,-1195.305;Float;False;Property;_ShadowOffset;Shadow Offset;26;0;Create;True;0;0;False;0;0.5;0.295;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;3066;-4883.552,-1946.332;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3076;-4736.512,-1969.438;Float;False;Property;_HighlightTint;Highlight Tint;21;0;Create;True;0;0;False;0;0.003921569,0.3098039,0.3960784,0.9686275;0,0.6392157,0.8,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;2597;-5415.549,-3310.725;Float;False;Property;_SmoothnessThreshold;Smoothness Threshold;14;0;Create;True;0;0;False;0;0.05;0.135;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;954;-4879.305,-4897.043;Float;False;Property;_WaterTint;Water Tint;0;0;Create;True;0;0;False;1;Header (COLOR TINT);0.08627451,0.2941177,0.3490196,1;0,0.6381524,0.8018868,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;2325;-5473.567,-4545.63;Inherit;False;1914.155;350.8884;;9;2371;3159;3172;2359;2549;2870;3177;3176;3192;FINAL_OPACITY;0,0.9529412,0.05987054,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3170;-4771.776,-5620.234;Float;False;Constant;_DepthScale;Depth Scale;3;0;Create;True;0;0;False;0;-1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3052;-4988.118,-2224.302;Inherit;False;2044;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2836;-4596.612,-5345.803;Inherit;False;2837;REFRACTED_UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;2746;-4607.56,-5016.234;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;345;-4880.487,-5175.999;Float;False;Property;_ShallowTint;Shallow Tint;1;0;Create;True;0;0;False;0;0.003921569,0.7529412,0.7137255,1;0,0.6392157,0.8,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;1713;-7385.812,-2281.739;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;3171;-4581.791,-5471.058;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;3084;-4163.027,-2063.156;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2752;-4417.308,-5173.457;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2813;-4558.608,1069.558;Float;False;Property;_RefractedDepth;Refracted Depth;16;0;Create;True;0;0;False;1;Header (REFRACTION);30;8.6;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2047;-5129.11,-1281.313;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;3056;-4363.118,-1872.935;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3053;-4733.944,-2066.783;Float;False;Property;_HighlightOffset;Highlight Offset;22;0;Create;True;0;0;False;0;-1;-1;-1;-0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3077;-4435.531,-1969.302;Float;False;HIGHLIGHT_TINT;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;2835;-4351.011,-5352.176;Float;False;Global;_GrabScreen1;Grab Screen 1;5;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1738;-7283.769,-2479.81;Float;False;Constant;_Float9;Float 9;31;0;Create;True;0;0;False;0;3;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;2016;-5362.998,-943.3666;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3063;-4732.516,-2224.428;Float;False;Property;_HighlightDirection;Highlight Direction;20;0;Create;True;0;0;False;0;1;0;3;True;;KeywordEnum;4;NdotL;N1;N1V;N1A;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;2410;-4931.689,-3362.026;Float;False;#define PerceptualSmoothnessToRoughness(perceptualSmoothness) (1.0 - perceptualSmoothness) * (1.0 - perceptualSmoothness)$#define RoughnessToPerceptualSmoothness(roughness) 1.0 - sqrt(roughness)$float3 deltaU = ddx(geometricNormalWS)@$float3 deltaV = ddy(geometricNormalWS)@$float variance = screenSpaceVariance * (dot(deltaU, deltaU) + dot(deltaV, deltaV))@$float roughness = PerceptualSmoothnessToRoughness(perceptualSmoothness)@$// Ref: Geometry into Shading - http://graphics.pixar.com/library/BumpRoughness/paper.pdf - equation (3)$float squaredRoughness = saturate(roughness * roughness + min(2.0 * variance, threshold * threshold))@ // threshold can be really low, square the value for easier$return RoughnessToPerceptualSmoothness(sqrt(squaredRoughness))@;1;False;4;True;perceptualSmoothness;FLOAT;0;In;;Float;False;True;geometricNormalWS;FLOAT3;0,0,0;In;;Float;False;True;screenSpaceVariance;FLOAT;0.5;In;;Float;False;True;threshold;FLOAT;0.5;In;;Float;False;GetGeometricNormalVariance;False;True;0;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.5;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;1771;-6797.746,-1799.207;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;2457;-6884.146,-1284.425;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2111;-5432.776,-2844.807;Inherit;False;1843.569;542.3054;retrieves the reflections from reflection probes;17;2026;1991;2037;2029;2035;3073;3074;3038;3039;3040;1994;2027;3060;3044;3041;1992;3042;INDIRECT SPECULAR LIGHTING;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1739;-7269.714,-2612.307;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;2870;-5430.279,-4479.729;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;2843;-5437.158,-45.8869;Inherit;False;1843.053;753.5786;http://wiki.unity3d.com/index.php/MirrorReflection4;14;2822;3186;3184;2920;3174;2850;3175;2823;2818;2824;2921;2848;3187;3188;PLANAR_REFLECTION;1,0.9568607,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;2549;-5407.952,-4285.33;Float;False;Property;_OpacityOcean;Opacity Ocean;7;0;Create;True;0;0;False;1;Header (OPACITY);0.045;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2045;-5431.28,-1113.916;Float;False;Property;_ShadowSharpness;Shadow Sharpness;25;0;Create;True;0;0;False;0;0.01;0.426;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3055;-4007.999,-2082.916;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;3172;-5105.959,-4312.471;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3042;-5414.17,-2613.859;Inherit;False;3077;HIGHLIGHT_TINT;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;3036;-5449.31,-1664.185;Inherit;False;959.707;285.7245;Comment;4;3037;3034;3035;3033;LIGHT_COLOR_FALLOFF;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2815;-4276.045,1002.96;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3182;-4436.033,-2234.399;Float;False;HIGHLIGHT_DIRECTION;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2109;-5371.297,-722.7735;Inherit;False;983.9504;229.3156;Retrieves the information of surrounding light probes;6;1999;1996;3028;1998;2000;1997;INDIRECT DIFFUSE LIGHTING;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.SaturateNode;3169;-4364.076,-5472.638;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;2005;-5052.334,-945.5352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3059;-4415.706,-2157.901;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1717;-7206.545,-2929.086;Float;False;Property;_OceanFoamTint;Ocean Foam Tint;47;0;Create;True;0;0;False;0;0.6039216,0.6039216,0.6039216,1;0.6037736,0.6037736,0.6037736,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3195;-4137.143,-5349.07;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1768;-6665.551,-1800.732;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;1737;-7108.303,-2610.815;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;2848;-5410.061,175.5347;Inherit;False;485.3346;209.6514;Surface normals wobbling the reflection;3;2827;2828;2832;;1,0.9864793,0,1;0;0
Node;AmplifyShaderEditor.SwitchByFaceNode;2427;-4522.771,-3361.886;Inherit;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1746;-7131.439,-2280.025;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2425;-5475.126,-4133.245;Inherit;False;1910.637;534.2797;;12;2527;2446;2493;2490;3025;2466;2458;2450;2697;2444;2436;2439;REDUCE_AT_HORIZON;0,1,0.007030964,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;2018;-5143.072,-867.0861;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ToggleSwitchNode;2359;-5211.669,-4469.415;Float;False;Property;_IgnoreVertexColor;Ignore Vertex Color;9;0;Create;True;0;0;False;0;1;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1774;-6677.931,-1639.918;Float;False;Property;_BeachFoamStrenght;Beach Foam Strenght;56;0;Create;True;0;0;False;0;0.35;0.528;0.1;0.75;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;442;-4395.099,-5649.542;Float;False;Property;_DepthTint;Depth Tint;4;0;Create;True;0;0;False;0;0.08627451,0.2941177,0.3490196,1;0,0.6392157,0.8,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;2355;-7567.026,-3817.829;Inherit;False;2065.123;602.2773;For underwater;17;2433;2422;2413;2386;2383;2416;2382;2380;2381;2379;2366;2374;2376;2367;2372;2378;3194;GRAB_PASS;0,1,0.1558626,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;2610;-7260.705,-2744.517;Inherit;False;2369;NEW_BEHIND_DEPTH_03;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2048;-4988.963,-1282.262;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;2049;-4854.254,-1280.715;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2832;-5399.953,214.903;Inherit;False;3182;HIGHLIGHT_DIRECTION;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3002;-5123.136,-5718.061;Float;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1998;-5085.09,-576.5001;Float;False;Property;_LightIndirectStrengthDiffuse;Light Indirect Strength Diffuse;19;0;Create;True;0;0;False;0;0.5;0.285;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;1775;-6498.509,-1804.095;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.8602941,0.8602941,0.8602941,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;3057;-3861.536,-2156.159;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3015;-3973.955,-5519.217;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2827;-5401.05,307.8857;Float;False;Property;_ReflectionWobble;Reflection Wobble;29;0;Create;True;0;0;False;0;0.1;0.01;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2000;-5360.521,-660.4479;Inherit;False;2044;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;2439;-5395.801,-3811.712;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3159;-4889.493,-4460.469;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2432;-3965.338,-3352.329;Float;True;NEW_SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;2057;-6404.087,-1635.994;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2467;-3840.79,-5239.824;Float;False;Constant;_Float11;Float 11;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;3041;-5165.83,-2614.719;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;2366;-7537.994,-3754.955;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;2436;-5406.477,-3959.263;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;3001;-5124.136,-5796.061;Float;False;Constant;_Float10;Float 10;8;0;Create;True;0;0;False;0;-5;-5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;3033;-5428.352,-1616.549;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;2816;-4138.266,1005.595;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2006;-4874.481,-925.0764;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1720;-6923.167,-2764.279;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;3035;-5430.184,-1488.35;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3191;-4013.968,1092.377;Float;False;Constant;_Float6;Float 6;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1146;-6884.794,-2593.748;Float;False;Property;_OceanFoamStrength;Ocean Foam Strength;49;1;[Gamma];Create;True;0;0;False;0;0.55;0.5499999;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2050;-4702.189,-1281.252;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;2444;-5166.05,-3912.145;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2017;-5417.115,-1030.488;Float;False;Property;_ShadowStrength;Shadow Strength;24;0;Create;True;0;0;False;1;Header (SHADOW);0.35;0.18;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;3000;-4875.136,-5818.061;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;2367;-7266.201,-3662.153;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1776;-6263.991,-1800.671;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2446;-5447.731,-4061.851;Inherit;False;2432;NEW_SMOOTHNESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;2469;-3661.328,-5482.42;Float;False;Property;_Keyword1;Keyword 1;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2371;-3990.038,-4462.496;Float;True;NEW_FINAL_OPACITY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2029;-4994.746,-2439.67;Float;False;Property;_LightIndirectStrengthSpecular;Light Indirect Strength Specular;18;0;Create;True;0;0;False;1;Header (LIGHTING);0.5;0.512;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3187;-5227.685,524.1069;Float;False;Property;_ReflectionFresnelPower;Reflection Fresnel Power;30;0;Create;True;0;0;False;0;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;1997;-5081.162,-655.8745;Inherit;False;World;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;2008;-4728.149,-923.5424;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3190;-3845.527,994.0611;Float;False;Property;_Keyword0;Keyword 0;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1996;-4719.153,-684.1594;Float;False;Constant;_Float0;Float 0;20;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2697;-5043.495,-4025.763;Inherit;False;468.7852;187.2429;FresnelLerpFast is called in Unity GI to adjust the intensity of the reflection towards the horizon for the fresnel effect. Inverting it fixes the brightness at the horizon. ;1;2452;Fresnel Lerp Fast;0,1,0.007843138,1;0;0
Node;AmplifyShaderEditor.PowerNode;3044;-4940.971,-2797.955;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2921;-5410.773,-4.035202;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;3058;-3730.941,-2213.599;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2828;-5107.464,216.7899;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1994;-5227.831,-2531.354;Float;False;Property;_Occlusion;Occlusion;10;0;Create;True;0;0;False;1;Header (GLOBAL SETTINGS);0.5;0.225;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1992;-5406.283,-2780.594;Inherit;False;1987;NORMAL_MAP_SAMPLE;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;1147;-6593.786,-2791.462;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3188;-5225.125,437.0847;Float;False;Property;_ReflectionFresnelScale;Reflection Fresnel Scale;31;0;Create;True;0;0;False;0;0.4;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;3028;-4774.715,-570.4221;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3034;-5184.271,-1597.68;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;2026;-4678.527,-2729.771;Float;False;Constant;_Float5;Float 5;21;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectSpecularLight;1991;-4803.421,-2650.173;Inherit;False;Tangent;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;2761;-2102.23,-3826.495;Inherit;False;574.7582;207.8559;Comment;5;2929;2932;2930;2766;2933;METALLIC;0.9866247,1,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3060;-4787.949,-2804.922;Float;False;SPECULAR_TINT_A1_5;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;2999;-4666.136,-5819.061;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2822;-4811.582,537.5671;Float;False;Property;_ReflectionIntensity;Reflection Intensity;28;0;Create;True;0;0;False;0;0.3;0.169;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;2372;-7040.033,-3514.483;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;2452;-4946.037,-3973.607;Float;False;return FresnelLerpFast (specColor, grazingTerm, nv)@;1;False;3;True;specColor;FLOAT3;0,0,0;In;;Float;True;True;grazingTerm;FLOAT3;0,0,0;In;;Float;True;True;nv;FLOAT;0;In;;Float;True;CallFresnelLerpFast;False;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;541;-6068.847,-1837.278;Float;False;Property;_EnableBeachFoam;Enable Beach Foam;52;0;Create;True;0;0;False;1;Header (BEACH FOAM);0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;2014;-4472.947,-1254.406;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;3037;-5010.085,-1597.868;Float;False;LIGHT_COLOR_FALLOFF;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;3186;-4840.654,287.3525;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3070;-3553.514,-2220.574;Float;True;HIGHLIGHTS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2374;-7357.877,-3483.563;Inherit;False;2371;NEW_FINAL_OPACITY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3040;-4496.037,-2601.907;Inherit;False;3077;HIGHLIGHT_TINT;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2548;-4874.757,-5701.163;Float;False;Property;_OpacityBeach;Opacity Beach;8;0;Create;True;0;0;False;0;0.15;0.593;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3145;-2219.852,-4508.001;Inherit;False;520.7628;187.7532;Comment;3;2839;3079;3173;COLOR_TINT_FINAL;0,0.305243,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3194;-7265.057,-3566.606;Inherit;False;Property;_Distortion;Distortion;15;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;1999;-4567.218,-675.6276;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;444;-3338.242,-5497.189;Float;True;COLOR_TINT_FINAL;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2824;-4904.596,-1.354706;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;2037;-4691.465,-2435.852;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2450;-4832.229,-3770.568;Inherit;False;2371;NEW_FINAL_OPACITY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;1148;-6408.377,-2821.399;Float;False;Property;_EnableOceanFoam;Enable Ocean Foam ;45;0;Create;True;0;0;False;1;Header (OCEAN FOAM);0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2376;-7365.926,-3368.43;Inherit;False;2369;NEW_BEHIND_DEPTH_03;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2840;-3521.778,995.0491;Float;True;REFRACTED_DEPTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2015;-4475.47,-1063.728;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;834;-5784.343,-1828.321;Float;True;BEACH_FOAM;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3157;-4454.136,-5819.061;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2010;-4265.966,-1230.951;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;302;-5812.187,-2826.012;Float;True;OCAEN_FOAM;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;3074;-4239.019,-2480.766;Inherit;False;3060;SPECULAR_TINT_A1_5;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2378;-6975.742,-3487.418;Inherit;False;5;5;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2458;-4510.958,-3973.172;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;3039;-4239.428,-2654.69;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;2027;-4504.161,-2716.412;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;3079;-2198.977,-4474.404;Inherit;False;444;COLOR_TINT_FINAL;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;2766;-2093.094,-3697.948;Float;False;Property;_Metallic;Metallic;11;0;Create;True;0;0;False;0;0.25;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3038;-4242.139,-2556.863;Inherit;False;3037;LIGHT_COLOR_FALLOFF;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;3073;-4179.188,-2407.999;Inherit;False;3070;HIGHLIGHTS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2379;-6987.47,-3724.682;Inherit;True;982;NORMAL_MAP;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2933;-2000.334,-3780.571;Inherit;False;2044;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2736;-1909.623,-4290.032;Inherit;False;376.6147;194.5634;Comment;3;693;303;835;FOAM;0,0.2750301,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;2818;-4669.349,0.8964996;Inherit;True;Global;_ReflectionTex;_ReflectionTex;60;0;Create;True;0;0;False;0;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;2839;-2193.161,-4400.727;Inherit;False;2840;REFRACTED_DEPTH;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2011;-4274.224,-968.6924;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3184;-4474.958,264.3687;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2012;-3989.399,-1085.683;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;2738;-2687.413,-4093.542;Inherit;False;639.6603;217.5828;Comment;3;2731;727;2732;NORMAL;1,0,0.887598,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;835;-1912.275,-4174.084;Inherit;False;834;BEACH_FOAM;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;2823;-4327.998,89.4639;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;3146;-1110.86,-3897.914;Inherit;False;1038.177;250.42;Comment;8;3180;3005;3164;3166;3181;3165;3167;3004;;0,0.2599814,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2035;-3919.429,-2719.087;Inherit;False;5;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Exp2OpNode;2930;-1808.511,-3689.829;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3003;-3942.832,-5821.062;Float;False;DEPTH_TINT_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;2380;-6799.791,-3389.83;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;2932;-1807.186,-3781.005;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3173;-1875.036,-4462.318;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;3143;-1793.989,-3570.088;Inherit;False;262.3157;116.8459;Comment;1;2143;SMOOTHNESS;1,0.9622972,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;303;-1901.601,-4246.482;Inherit;False;302;OCAEN_FOAM;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;2466;-4344.793,-3973.649;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2381;-6742.672,-3529.559;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;693;-1657.863,-4247.634;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;727;-2511.838,-4055.968;Inherit;False;982;NORMAL_MAP;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;3142;-1290.008,-4299.643;Inherit;False;264.0483;118.5356;Comment;1;544;INDIRECT SPECULAR ;1,0.9214484,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3175;-4064.926,124.9742;Float;False;Constant;_Float2;Float 2;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3005;-1113.189,-3836.884;Inherit;False;3003;DEPTH_TINT_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;2850;-4153.413,18.8101;Float;False;Property;_EnableReflection;Enable Reflection;27;0;Create;True;0;0;False;1;Header (REFLECTION);0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2382;-6524.634,-3532.715;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;3025;-4158.656,-3996.238;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;2929;-1665.443,-3780.426;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;532;-3569.067,-2717.263;Float;True;INDIRECT_SPECULAR;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2143;-1776.93,-3528.569;Inherit;False;2432;NEW_SMOOTHNESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;3132;-3758.293,-1088.854;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;3164;-1007.675,-3685.888;Inherit;False;2371;NEW_FINAL_OPACITY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2113;-3552.869,-1099.594;Float;True;LIGHT_SHADOWS;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;3139;-842.9911,-4519.618;Inherit;False;248.8176;110.8628;Comment;1;3032;LIGHT_SHADOWS;1,0.9747956,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;544;-1275.461,-4258.381;Inherit;False;532;INDIRECT_SPECULAR;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;3135;-882.2737,-4290.927;Inherit;False;299.09;108.397;Comment;1;2528;REDUCE_AT_HORIZON;1,0.991496,0,1;0;0
Node;AmplifyShaderEditor.ScreenColorNode;2416;-6333.393,-3534.575;Float;False;Global;_GrabTextureWater;GrabTextureWater;5;0;Create;True;0;0;False;0;Object;-1;True;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;3174;-3896.485,26.6582;Float;False;Property;_Keyword3;Keyword 3;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2527;-3950.514,-4000.461;Float;True;NEW_REDUCE_AT_HORIZON;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2413;-6273.16,-3649.601;Float;False;Constant;_Float7;Float 7;29;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomStandardSurface;1834;-1453.614,-4043.019;Inherit;False;Metallic;World;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;3166;-876.7884,-3821.819;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2146;-1109.667,-3418.393;Inherit;False;1039.313;310.7102;Comment;8;1817;1816;2704;2703;2700;1818;2698;1371;TESSELLATION;1,0,0.889535,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3123;-895.0319,-4038.687;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;3032;-835.4291,-4484.618;Inherit;False;2113;LIGHT_SHADOWS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2528;-875.0026,-4253.924;Inherit;False;2527;NEW_REDUCE_AT_HORIZON;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1340;-7127.909,218.7617;Inherit;False;302;OCAEN_FOAM;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3165;-711.9116,-3806.203;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2841;-3537.807,69.0544;Float;True;PLANAR_REFLECTION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;3141;-467.3412,-4287.965;Inherit;False;255.6667;109.7778;Comment;1;3125;PLANAR_REFLECTION;1,0.9318118,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2699;-8315.547,378.3083;Float;False;Wave001_W_WAVE_LENGHT;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;2422;-6100.966,-3531.412;Float;False;Property;_Addpass;Add pass?;29;0;Fetch;True;0;0;False;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2700;-1077.259,-3351.812;Inherit;False;2699;Wave001_W_WAVE_LENGHT;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3125;-463.3412,-4252.965;Inherit;False;2841;PLANAR_REFLECTION;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3140;-386.2277,-4039.377;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;3167;-557.6074,-3802.427;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1263;-6866.157,265.3486;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3181;-554.6677,-3732.039;Float;False;Constant;_Float8;Float 8;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1371;-763.8928,-3373.862;Float;False;Property;_TessellationStrength;Tessellation Strength;37;0;Create;True;0;0;False;1;Header (WAVE TESSELLATION);0.15;0.01;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2433;-5789.07,-3532.213;Float;False;New_GRAB_SCREEN_COLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1817;-660.9109,-3185.967;Float;False;Property;_TessellationMaxDistance;Tessellation Max Distance;38;0;Create;True;0;0;False;0;3000;3000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2147;277.876,-4423.154;Inherit;False;169.4342;113.6689;Comment;1;1392;RENDER_OPTIONS;0,0.3987923,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3138;-159.3994,-4049.48;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;2289;1507.467,-4511.617;Inherit;False;1898.696;331.7527;;12;2318;2314;2301;2295;2293;2478;2495;2491;2497;2292;2922;2290;BEHIND_DEPTH_01;1,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1382;-5943.617,277.2064;Float;True;WAVE_GERSTNER_LOCAL_VERT_OFFSET;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2698;-473.4948,-3371.144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2148;-6530.938,-2512.053;Inherit;False;378;280;Comment;1;1195;OCAEN FOAM MAP;0.3019608,0.4445193,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2476;-706.5034,-4794.599;Inherit;False;651.5372;187.2608;;4;2988;2501;2499;2500;SHADOW_OPACITY;1,0,0.02967834,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1816;-663.1328,-3265.669;Float;False;Constant;_TessellationDistanceMin;Tessellation Distance Min;24;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3004;-377.7462,-3878.153;Inherit;False;2433;New_GRAB_SCREEN_COLOR;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;3180;-389.6738,-3776.242;Float;False;Property;_Keyword6;Keyword 6;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2920;-4230.263,226.989;Inherit;False;557.6765;208.831;Global Camera Depth Texture (Do not Remove);2;2820;2842;;0.9788539,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2385;1491.987,-4062.115;Inherit;False;1897.997;275.1177;Warped Depth Fade;10;2417;2488;2479;2403;2399;2393;2390;2389;2387;2388;BEHIND_DEPTH_02;1,0,0.03030539,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;1261;-444.2299,-3583.564;Inherit;False;1382;WAVE_GERSTNER_LOCAL_VERT_OFFSET;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2314;2703.573,-4299.233;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2293;2101.313,-4467.241;Inherit;False;2291;NEW_BASE_DEPTH_FADE;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2388;1517.21,-4005.604;Inherit;False;2386;NEW_SCREEN_DEPTH_WARPED;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;2494;-7643.025,-1116.033;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;2389;1879.213,-3998.4;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2128;-4966.488,-3160.092;Inherit;False;1384;WAVE_GERSTNER_LOCAL_VERT_NORMAL;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;2821;-4277.406,1109.008;Float;False;FinalColor.a = 1@;7;False;3;True;SurfaceIn;OBJECT;0;In;Input;Float;False;True;SurfaceOut;OBJECT;0;In;SurfaceOutputStandard;Float;False;True;FinalColor;OBJECT;0;InOut;fixed4;Float;False;ResetAlpha;False;True;0;4;0;FLOAT4;0,0,0,0;False;1;OBJECT;0;False;2;OBJECT;0;False;3;OBJECT;0;False;2;COLOR;0;OBJECT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;2491;1537.936,-4433.128;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;2478;2252.795,-4376.564;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2295;2413.234,-4297.417;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2292;1963.374,-4283.111;Float;False;NEW_DEPTH_DISTANCE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;2301;2559.518,-4298.112;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2488;2670.864,-3910.949;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;2922;1792.55,-4310.722;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2386;-6106.949,-3340.972;Float;False;NEW_SCREEN_DEPTH_WARPED;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;2495;1740.343,-4383.092;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2384;-6256.16,-4071.396;Float;False;NEW_DISTANE_DEPTH_01;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2390;1891.652,-3880.236;Inherit;False;2292;NEW_DEPTH_DISTANCE;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;1392;289.557,-4386.23;Float;False;Property;_CullMode;Cull Mode;59;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CullMode;True;1;Header (RENDERING OPTIONS);2;0;0;1;INT;0
Node;AmplifyShaderEditor.CustomExpressionNode;2825;-4896.088,1125.392;Float;False;#if UNITY_UV_STARTS_AT_TOP$if (_CameraDepthTexture_TexelSize.y < 0) {$	uv.y = 1 - uv.y@$}$#endif$return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) * abs(_CameraDepthTexture_TexelSize.xy)@;2;False;1;True;uv;FLOAT2;0,0;In;;Float;False;AlignWithGrabTexel;False;True;0;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;2387;1549.36,-3922.525;Inherit;False;2384;NEW_DISTANE_DEPTH_01;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1610;-6730.811,502.107;Float;False;Property;_Wave003Speed;Wave 003 Speed;44;0;Create;True;0;0;False;0;0.001;0.402;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;2479;2455.165,-3887.302;Float;False;return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f@;0;False;0;inInMirror;False;False;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2403;2521.921,-3994.416;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;2399;2359.688,-4000.906;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;2497;2443.978,-4463.742;Float;False;return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f@;0;False;0;inInMirror;False;False;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;2988;-171.5775,-4757.652;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2490;-4467.889,-3782.925;Float;False;surfaceColourUsedForFresnel;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2701;-8328.402,684.9341;Float;False;Wave002_W_WAVE_LENGHT;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2496;-7186.385,-4192.804;Float;False;NEW_ZERO_ONE_DEPTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2702;-8338.022,968.6394;Float;False;Wave003_W_WAVE_LENGHT;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1388;-447.5357,-3499.792;Inherit;False;1384;WAVE_GERSTNER_LOCAL_VERT_NORMAL;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2493;-4478.166,-3866.42;Inherit;False;2490;surfaceColourUsedForFresnel;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexToFragmentNode;2511;-6914.86,-4624.551;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2291;-6068.502,-4170.046;Float;False;NEW_BASE_DEPTH_FADE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2842;-3906.25,271.9899;Float;False;TexelSize;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenDepthNode;2383;-6341.596,-3341.268;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3177;-4318.39,-4339.6;Float;False;Property;_Keyword4;Keyword 4;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2499;-697.4253,-4679.098;Float;False;Constant;_Float13;Float 13;30;0;Create;True;0;0;False;0;0.125;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1607;-6728.674,599.0611;Float;False;Property;_Wave002Speed;Wave 002 Speed;43;0;Create;True;0;0;False;0;0.001;0.421;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexToFragmentNode;2859;-4147.48,-1487.253;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2704;-1078.794,-3204.155;Inherit;False;2702;Wave003_W_WAVE_LENGHT;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2731;-2674.01,-3980.753;Inherit;False;1384;WAVE_GERSTNER_LOCAL_VERT_NORMAL;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;3006;30.69359,-4051.146;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1195;-6490.715,-2453.537;Inherit;True;Property;_OceanFoamMap;Ocean Foam Map;46;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3176;-4583.193,-4332.802;Float;False;Constant;_Float4;Float 4;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceBasedTessNode;1818;-319.7706,-3370.933;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;2703;-1074.134,-3276.674;Inherit;False;2701;Wave002_W_WAVE_LENGHT;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;3192;-4677.992,-4485.409;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2290;1501.682,-4285.679;Float;False;Property;_DepthDistance_A;Depth Distance_A;6;0;Create;True;0;0;False;0;0;132;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2417;2842.443,-3993.958;Float;False;NEW_BEHIND_DEPTH_02;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2500;-691.3911,-4758.554;Float;False;Constant;_Float14;Float 14;30;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;2820;-4211.731,270.9847;Float;False;Global;_CameraDepthTexture_TexelSize;_CameraDepthTexture_TexelSize;2;0;Create;True;0;0;True;0;0,0,0,0;0.0008503401,0.00129199,1176,774;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;2501;-525.7224,-4756.622;Float;False;Property;_Shadow;Shadow?;28;0;Fetch;True;0;0;False;0;0;0;0;False;UNITY_PASS_SHADOWCASTER;Toggle;2;Key0;Key1;Fetch;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;2732;-2256.599,-3973.275;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2318;2849.93,-4293.366;Float;False;NEW_BEHIND_DEPTH_01;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;2393;2188.649,-3999.251;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;307;280.0145,-4279.978;Float;False;True;6;ASEMaterialInspector;0;0;CustomLighting;PWS/SP/Water/Ocean vP2.1 2019_01_14;False;False;False;False;False;False;False;False;True;False;False;False;False;False;True;False;False;True;True;True;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;-10;False;Opaque;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;18.9;10;25;False;0.5;True;0;5;False;-1;10;False;-1;1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;1392;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1821;0;1349;0
WireConnection;1936;0;1935;0
WireConnection;1825;0;1821;0
WireConnection;1825;2;1822;1
WireConnection;1823;0;1821;0
WireConnection;1823;2;1822;2
WireConnection;2510;0;2508;0
WireConnection;2510;1;2509;0
WireConnection;1827;0;1825;0
WireConnection;1827;2;1369;0
WireConnection;1827;1;1936;0
WireConnection;1828;0;1823;0
WireConnection;1828;2;1369;0
WireConnection;1828;1;1936;0
WireConnection;2512;0;2510;0
WireConnection;1988;1;1827;0
WireConnection;1988;5;1938;0
WireConnection;1826;0;1821;0
WireConnection;1826;2;1369;0
WireConnection;1826;1;1936;0
WireConnection;1829;1;1828;0
WireConnection;1829;5;1938;0
WireConnection;2675;0;2674;1
WireConnection;2675;1;2674;3
WireConnection;1832;0;1988;0
WireConnection;1832;1;1829;0
WireConnection;1830;1;1826;0
WireConnection;1830;5;1938;0
WireConnection;2641;0;2675;0
WireConnection;2641;1;2640;0
WireConnection;1833;0;1832;0
WireConnection;1833;1;1830;0
WireConnection;2286;0;2283;0
WireConnection;2286;1;2284;0
WireConnection;1383;0;1386;0
WireConnection;1383;1;1385;0
WireConnection;1383;2;1379;0
WireConnection;1383;3;1378;0
WireConnection;1383;4;1377;0
WireConnection;2287;0;2285;0
WireConnection;2647;0;2641;0
WireConnection;2647;1;1781;0
WireConnection;2053;0;1193;0
WireConnection;2646;0;1781;0
WireConnection;2288;0;2286;0
WireConnection;2288;1;2287;0
WireConnection;1605;0;1606;0
WireConnection;1605;1;1385;0
WireConnection;1605;2;1379;0
WireConnection;1605;3;1383;4
WireConnection;1605;4;1383;5
WireConnection;2054;0;2053;0
WireConnection;2649;0;2647;0
WireConnection;2649;1;2646;0
WireConnection;1831;0;1833;0
WireConnection;1831;2;1350;0
WireConnection;2356;0;2349;0
WireConnection;2356;1;2352;0
WireConnection;2306;0;2286;0
WireConnection;1786;0;2647;0
WireConnection;1786;2;1783;1
WireConnection;2362;0;2288;0
WireConnection;1787;0;2649;0
WireConnection;1787;2;1783;2
WireConnection;2361;1;2356;0
WireConnection;1987;0;2054;0
WireConnection;2651;0;2644;0
WireConnection;2651;1;1785;0
WireConnection;2650;0;2645;0
WireConnection;2650;1;1785;0
WireConnection;1608;0;1609;0
WireConnection;1608;1;1385;0
WireConnection;1608;2;1379;0
WireConnection;1608;3;1605;4
WireConnection;1608;4;1605;5
WireConnection;2156;0;1831;0
WireConnection;2395;0;2507;0
WireConnection;2368;0;2361;0
WireConnection;1380;0;1608;5
WireConnection;1380;1;1608;4
WireConnection;2654;0;1786;0
WireConnection;2654;2;2651;0
WireConnection;2652;0;1787;0
WireConnection;2652;2;2650;0
WireConnection;2369;0;2362;0
WireConnection;2155;0;2156;0
WireConnection;1763;0;2672;0
WireConnection;1763;1;2652;0
WireConnection;1381;0;1380;0
WireConnection;2409;0;2395;3
WireConnection;2409;1;2400;0
WireConnection;1726;0;1733;0
WireConnection;2812;0;2810;0
WireConnection;2812;1;2809;0
WireConnection;2373;0;2368;0
WireConnection;2253;1;2155;0
WireConnection;1764;0;2672;0
WireConnection;1764;1;2654;0
WireConnection;2429;0;2424;0
WireConnection;2429;1;2428;0
WireConnection;3047;0;3046;0
WireConnection;3047;1;3045;0
WireConnection;1384;0;1381;0
WireConnection;982;0;2253;0
WireConnection;2043;0;2041;0
WireConnection;2043;1;2042;0
WireConnection;2814;0;2812;0
WireConnection;2814;1;2811;0
WireConnection;1765;0;1764;0
WireConnection;1765;1;1763;0
WireConnection;1765;2;1763;4
WireConnection;1765;3;1779;0
WireConnection;1730;0;1726;0
WireConnection;1730;2;1731;1
WireConnection;2739;0;3193;0
WireConnection;2430;0;2409;0
WireConnection;2430;1;2428;0
WireConnection;1725;0;1726;0
WireConnection;1725;2;1731;2
WireConnection;2448;0;2429;0
WireConnection;2448;1;2435;0
WireConnection;2837;0;2814;3
WireConnection;2044;0;2043;0
WireConnection;1728;0;1725;0
WireConnection;1728;2;1727;0
WireConnection;1734;0;1730;0
WireConnection;1734;2;1727;0
WireConnection;3048;0;3047;0
WireConnection;2447;0;2430;0
WireConnection;2447;1;2435;0
WireConnection;2743;0;2739;0
WireConnection;2743;1;2744;0
WireConnection;2743;2;2745;0
WireConnection;2707;1;1765;0
WireConnection;2707;2;1766;0
WireConnection;1729;0;1726;0
WireConnection;1729;2;1727;0
WireConnection;1736;1;1734;0
WireConnection;3049;0;3048;0
WireConnection;3049;1;3067;0
WireConnection;3064;0;3048;0
WireConnection;3064;1;3050;0
WireConnection;1770;0;2707;0
WireConnection;1732;1;1728;0
WireConnection;2455;0;2447;0
WireConnection;2455;1;2448;0
WireConnection;1716;1;1729;0
WireConnection;3066;0;3048;0
WireConnection;3066;1;3065;0
WireConnection;2746;0;2743;0
WireConnection;1713;0;1712;0
WireConnection;3171;0;2739;0
WireConnection;3171;1;3170;0
WireConnection;3171;2;3168;0
WireConnection;3084;0;3054;0
WireConnection;2752;0;345;0
WireConnection;2752;1;954;0
WireConnection;2752;2;2746;0
WireConnection;2047;0;2056;0
WireConnection;2047;1;2046;0
WireConnection;3056;0;3076;4
WireConnection;3077;0;3076;0
WireConnection;2835;0;2836;0
WireConnection;3063;1;3052;0
WireConnection;3063;0;3049;0
WireConnection;3063;2;3064;0
WireConnection;3063;3;3066;0
WireConnection;2410;0;2405;0
WireConnection;2410;1;2407;0
WireConnection;2410;2;2596;0
WireConnection;2410;3;2597;0
WireConnection;1771;0;1770;0
WireConnection;2457;0;2455;0
WireConnection;1739;0;1736;0
WireConnection;1739;1;1732;0
WireConnection;1739;2;1716;0
WireConnection;3055;0;3056;0
WireConnection;3055;1;3084;0
WireConnection;3172;0;2549;0
WireConnection;2815;0;2814;0
WireConnection;2815;1;2813;0
WireConnection;3182;0;3063;0
WireConnection;3169;0;3171;0
WireConnection;2005;0;2016;0
WireConnection;3059;0;3063;0
WireConnection;3059;1;3053;0
WireConnection;3195;0;2835;0
WireConnection;3195;1;2752;0
WireConnection;1768;0;1771;0
WireConnection;1768;2;2457;0
WireConnection;1737;0;1739;0
WireConnection;1737;1;1738;0
WireConnection;2427;0;2410;0
WireConnection;1746;0;1713;0
WireConnection;2359;1;2870;4
WireConnection;2048;0;2047;0
WireConnection;2048;1;2045;0
WireConnection;2049;0;2048;0
WireConnection;1775;0;1768;0
WireConnection;3057;0;3059;0
WireConnection;3057;1;3055;0
WireConnection;3015;0;442;0
WireConnection;3015;1;3195;0
WireConnection;3015;2;3169;0
WireConnection;3159;0;2359;0
WireConnection;3159;1;3172;0
WireConnection;2432;0;2427;0
WireConnection;2057;0;1774;0
WireConnection;3041;0;3042;0
WireConnection;2816;0;2815;0
WireConnection;2006;0;2005;0
WireConnection;2006;1;2018;2
WireConnection;1720;0;1717;0
WireConnection;1720;1;2610;0
WireConnection;1720;2;1737;0
WireConnection;1720;3;1746;0
WireConnection;2050;0;2049;0
WireConnection;2050;1;2016;0
WireConnection;2444;0;2436;0
WireConnection;2444;1;2439;0
WireConnection;3000;0;2739;0
WireConnection;3000;1;3001;0
WireConnection;3000;2;3002;0
WireConnection;2367;0;2366;0
WireConnection;1776;0;1775;0
WireConnection;1776;1;2057;0
WireConnection;2469;1;3015;0
WireConnection;2469;0;2467;0
WireConnection;2371;0;3159;0
WireConnection;1997;0;2000;0
WireConnection;2008;0;2006;0
WireConnection;3190;1;2816;0
WireConnection;3190;0;3191;0
WireConnection;3044;0;3041;0
WireConnection;3058;0;3057;0
WireConnection;2828;0;2832;0
WireConnection;2828;1;2827;0
WireConnection;1147;1;1720;0
WireConnection;1147;2;1146;0
WireConnection;3028;0;1998;0
WireConnection;3034;0;3033;1
WireConnection;3034;1;3035;0
WireConnection;1991;0;1992;0
WireConnection;1991;1;3041;0
WireConnection;1991;2;1994;0
WireConnection;3060;0;3044;0
WireConnection;2999;0;3000;0
WireConnection;2372;0;2367;0
WireConnection;2452;1;2446;0
WireConnection;2452;2;2444;0
WireConnection;541;1;1776;0
WireConnection;3037;0;3034;0
WireConnection;3186;2;3188;0
WireConnection;3186;3;3187;0
WireConnection;3070;0;3058;0
WireConnection;1999;0;1996;0
WireConnection;1999;1;1997;0
WireConnection;1999;2;3028;0
WireConnection;444;0;2469;0
WireConnection;2824;0;2921;0
WireConnection;2824;1;2828;0
WireConnection;2037;0;2029;0
WireConnection;1148;1;1147;0
WireConnection;2840;0;3190;0
WireConnection;2015;0;2008;0
WireConnection;2015;1;2050;0
WireConnection;2015;2;2017;0
WireConnection;834;0;541;0
WireConnection;3157;0;2999;0
WireConnection;3157;1;2548;0
WireConnection;2010;0;2014;1
WireConnection;2010;1;2015;0
WireConnection;302;0;1148;0
WireConnection;2378;0;2374;0
WireConnection;2378;1;3194;0
WireConnection;2378;2;2373;0
WireConnection;2378;3;2372;0
WireConnection;2378;4;2376;0
WireConnection;2458;0;2452;0
WireConnection;2458;1;2450;0
WireConnection;3039;0;3040;0
WireConnection;2027;0;2026;0
WireConnection;2027;1;1991;0
WireConnection;2027;2;2037;0
WireConnection;2818;1;2824;0
WireConnection;2011;0;1999;0
WireConnection;2011;1;2014;2
WireConnection;2011;2;2008;0
WireConnection;3184;0;3186;0
WireConnection;3184;1;2822;0
WireConnection;2012;0;2011;0
WireConnection;2012;1;2010;0
WireConnection;2823;1;2818;0
WireConnection;2823;2;3184;0
WireConnection;2035;0;2027;0
WireConnection;2035;1;3038;0
WireConnection;2035;2;3074;0
WireConnection;2035;3;3073;0
WireConnection;2035;4;3039;0
WireConnection;2930;0;2766;0
WireConnection;3003;0;3157;0
WireConnection;2932;0;2933;0
WireConnection;3173;1;3079;0
WireConnection;3173;2;2839;0
WireConnection;2466;0;2458;0
WireConnection;2381;1;2379;0
WireConnection;2381;2;2378;0
WireConnection;693;0;3173;0
WireConnection;693;1;303;0
WireConnection;693;2;835;0
WireConnection;2850;1;2823;0
WireConnection;2382;0;2381;0
WireConnection;2382;1;2380;0
WireConnection;3025;1;2466;0
WireConnection;2929;0;2932;0
WireConnection;2929;1;2930;0
WireConnection;532;0;2035;0
WireConnection;3132;0;2012;0
WireConnection;2113;0;3132;0
WireConnection;2416;0;2382;0
WireConnection;3174;1;2850;0
WireConnection;3174;0;3175;0
WireConnection;2527;0;3025;0
WireConnection;1834;0;693;0
WireConnection;1834;1;727;0
WireConnection;1834;3;2929;0
WireConnection;1834;4;2143;0
WireConnection;3166;0;3005;0
WireConnection;3123;0;1834;0
WireConnection;3123;1;544;0
WireConnection;3165;0;3166;0
WireConnection;3165;1;3164;0
WireConnection;2841;0;3174;0
WireConnection;2699;0;1386;4
WireConnection;2422;1;2416;0
WireConnection;2422;0;2413;0
WireConnection;3140;0;3123;0
WireConnection;3140;1;3032;0
WireConnection;3140;2;2528;0
WireConnection;3167;0;3165;0
WireConnection;1263;0;1340;0
WireConnection;1263;1;1383;0
WireConnection;1263;2;1605;0
WireConnection;1263;3;1608;0
WireConnection;2433;0;2422;0
WireConnection;3138;0;3140;0
WireConnection;3138;1;3125;0
WireConnection;1382;0;1263;0
WireConnection;2698;0;1371;0
WireConnection;2698;1;2700;0
WireConnection;3180;1;3167;0
WireConnection;3180;0;3181;0
WireConnection;2314;1;2301;0
WireConnection;2494;0;2409;0
WireConnection;2389;0;2388;0
WireConnection;2389;1;2387;0
WireConnection;2478;0;2495;0
WireConnection;2478;1;2292;0
WireConnection;2295;0;2293;0
WireConnection;2295;1;2292;0
WireConnection;2292;0;2290;0
WireConnection;2301;0;2295;0
WireConnection;2301;1;2295;0
WireConnection;2488;0;2403;0
WireConnection;2488;1;2479;0
WireConnection;2386;0;2383;0
WireConnection;2495;0;2491;2
WireConnection;2384;0;2287;0
WireConnection;2403;1;2399;0
WireConnection;2399;0;2393;0
WireConnection;2399;1;2393;0
WireConnection;2988;1;2501;0
WireConnection;2701;0;1606;4
WireConnection;2496;0;2283;0
WireConnection;2702;0;1609;4
WireConnection;2291;0;2288;0
WireConnection;2842;0;2820;0
WireConnection;2383;0;2382;0
WireConnection;3177;0;3176;0
WireConnection;2859;0;2042;0
WireConnection;3006;0;3138;0
WireConnection;3006;1;3004;0
WireConnection;3006;2;3180;0
WireConnection;1818;0;2698;0
WireConnection;1818;1;1816;0
WireConnection;1818;2;1817;0
WireConnection;2417;0;2403;0
WireConnection;2501;1;2500;0
WireConnection;2501;0;2499;0
WireConnection;2732;0;727;0
WireConnection;2732;1;2731;0
WireConnection;2318;0;2314;0
WireConnection;2393;0;2389;0
WireConnection;2393;1;2390;0
WireConnection;307;13;3006;0
WireConnection;307;11;1261;0
WireConnection;307;12;1388;0
WireConnection;307;14;1818;0
ASEEND*/
//CHKSM=A253B14A556AF73C2D10C978D90178FC5BF23D35