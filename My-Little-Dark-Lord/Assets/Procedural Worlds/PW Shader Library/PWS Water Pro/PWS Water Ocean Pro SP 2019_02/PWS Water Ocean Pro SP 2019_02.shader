// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PWS/Water/Ocean Pro SP 2019_02 v3.2"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)][Header (RENDERING OPTIONS)]_CullMode("Cull Mode", Int) = 2
		[Header (COLOR TINT)]_WaterTint("Water Tint", Color) = (0.08627451,0.2941177,0.3490196,1)
		_ShallowTint("Shallow Tint", Color) = (0.003921569,0.7529412,0.7137255,1)
		_ShallowDepth("Shallow Depth", Range( 0 , 100)) = 46.47059
		_ShallowOffset("Shallow Offset", Range( -1 , 1)) = 0.2
		_DepthTint("Depth Tint", Color) = (0.08627451,0.2941177,0.3490196,1)
		_DepthOffset("Depth Offset", Range( 0 , 2)) = 0.5
		[Header (OPACITY)]_OpacityBeach("Opacity Beach", Range( 0 , 1)) = 0
		_OpacityOcean("Opacity Ocean", Range( 0 , 1)) = 0
		[Header (LIGHTING)]_LightIndirectStrengthSpecular1("Light Indirect Strength Specular", Range( 0 , 1)) = 0
		_LightIndirectStrengthDiffuse1("Light Indirect Strength Diffuse", Range( 0 , 1)) = 0.5
		_HighlightTint1("Highlight Tint", Color) = (0.003921569,0.3098039,0.3960784,0.9686275)
		_HighlightOffset1("Highlight Offset", Range( -1 , -0.5)) = -1
		_HighlightSharpness1("Highlight Sharpness", Range( 0.001 , 1)) = 0.3
		[Header (SHADOW)]_ShadowStrength1("Shadow Strength", Range( 0 , 1)) = 0.35
		_ShadowSharpness1("Shadow Sharpness", Range( 0.01 , 1)) = 0.01
		_ShadowOffset1("Shadow Offset", Range( 0 , 1)) = 0.5
		[Enum(Smoothness Cull Off,0,Smoothness Cull Front,1,Smoothness Cull Back,2)][Header (SMOOTHNESS)]_SmoothnessMode("Smoothness Mode", Int) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_SmoothnessVariance("Smoothness Variance", Range( 0 , 1)) = 0.5
		_SmoothnessThreshold("Smoothness Threshold", Range( 0 , 1)) = 0
		_Occlusion("Occlusion", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", Range( 0 , 1)) = 0.25
		[Normal][Header (NORMAL MAP)]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalMapTiling("Normal Map Tiling", Float) = 80
		_NormalMapStrength("Normal Map Strength", Range( 0 , 1)) = 0.25
		_NormalMapSpeed("Normal Map Speed", Range( 0 , 0.1)) = 0.02
		_NormalMapTimescale("Normal Map Timescale", Range( 0 , 1)) = 0.25
		[Header (GERSTNER WAVES)][Header (X_Y DIRECTION Z_ STEEPNESS W_ WAVELENGTH)]_Wave01("Wave 01", Vector) = (-1,0,0.2,40)
		_Wave02("Wave 02", Vector) = (-1,1,0.25,35)
		_Wave03("Wave 03", Vector) = (2,1,0.2,25)
		_WaveHeight("Wave Height", Range( 0 , 1)) = 0.5
		_WaveHeightValueXYZ("Wave Height Value X Y Z", Vector) = (3,3,3,0)
		_WaveSpeed("Wave Speed", Range( 0 , 5)) = 1.1
		_TessellationStrength("Tessellation Strength", Range( 0.01 , 1)) = 0.1472393
		_TessellationMaxDistance("Tessellation Max Distance", Float) = 3000
		[Header (Ocean Wave Controls)]_OceanIntensity("Ocean Intensity ", Range( 0 , 1)) = 0.8
		_OceanRange("Ocean Range ", Range( 0 , 50)) = 5
		_OceanPower("Ocean Power ", Range( 0 , 1)) = 1
		_OceanOffset("Ocean Offset ", Range( 0 , 50)) = 3.3
		[Header (Beach Wave Controls)]_BeachIntensity("Beach Intensity ", Range( 0 , 1)) = 0.8
		_BeachRange("Beach Range ", Range( 0 , 50)) = 5
		_BeachPower("Beach Power ", Range( 0 , 1)) = 1
		_BeachOffset("Beach Offset ", Range( 0 , 50)) = 3.3
		[Header (REFLECTION)][KeywordEnum(on,off)] _EnableReflection("Enable Reflection", Float) = 1
		_ReflectionIntensity("Reflection Intensity", Range( 0 , 1)) = 0.7357025
		_ReflectionWobble("Reflection Wobble", Range( 0 , 1)) = 0
		_ReflectionFresnelScale("Reflection Fresnel Scale", Range( 0 , 1)) = 1
		_ReflectionFresnelPower("Reflection Fresnel Power", Range( 0 , 10)) = 0
		[Header (REFRACTION)]_RefractedDepth1("Refracted Depth", Range( 0 , 50)) = 30
		_RefractionScale1("Refraction Scale", Range( 0 , 1)) = 0.5
		[Header (OCEAN FOAM)][KeywordEnum(on,off)] _EnableOceanFoam1("Enable Ocean Foam ", Float) = 0
		_OceanFoamMap("Ocean Foam Map", 2D) = "white" {}
		_OceanFoamTint("Ocean Foam Tint", Color) = (0.6039216,0.6039216,0.6039216,1)
		_OceanFoamTiling("Ocean Foam Tiling", Float) = 50
		[Gamma]_OceanFoamStrength("Ocean Foam Strength", Range( 0 , 1)) = 0.3983873
		_foamMax2("Ocean Foam Distance", Range( 0.1 , 100)) = 18
		_OceanFoamSpeed("Ocean Foam Speed", Range( 0 , 0.5)) = 0.02
		[Header (BEACH FOAM)][KeywordEnum(on,off)] _EnableBeachFoam("Enable Beach Foam", Float) = 0
		_BeachFoamMap("Beach Foam Map", 2D) = "black" {}
		_BeachFoamTint("Beach Foam Tint", Color) = (0.8196079,0.8196079,0.8196079,1)
		_BeachFoamTiling("Beach Foam Tiling", Float) = 0.6
		_BeachFoamStrength("Beach Foam Strength", Range( 0.1 , 0.75)) = 0.35
		_foamMax1("Beach Foam Distance", Range( 0.1 , 100)) = 35.62941
		_BeachFoamSpeed("Beach Foam Speed", Range( 0 , 0.5)) = 0.09
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent-10" }
		Cull [_CullMode]
		GrabPass{ }
		GrabPass{ "_GrabTextureWater1" }
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma multi_compile_local _ENABLEREFLECTION_ON _ENABLEREFLECTION_OFF
		#pragma multi_compile_local _ENABLEOCEANFOAM1_ON _ENABLEOCEANFOAM1_OFF
		#pragma multi_compile_local _ENABLEBEACHFOAM_ON _ENABLEBEACHFOAM_OFF
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf StandardCustomLighting keepalpha noshadow noinstancing vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
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

		uniform int _CullMode;
		uniform float _BeachRange;
		uniform float _BeachPower;
		uniform float _BeachOffset;
		uniform float _BeachIntensity;
		uniform float _OceanRange;
		uniform float _OceanPower;
		uniform float _OceanOffset;
		uniform float _OceanIntensity;
		uniform float3 _WaveHeightValueXYZ;
		uniform float _WaveHeight;
		uniform float4 _Wave01;
		uniform float _WaveSpeed;
		uniform half PWSF_GlobalWindIntensityWater;
		uniform float4 _Wave02;
		uniform float4 _Wave03;
		uniform sampler2D _ReflectionTex;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _ReflectionWobble;
		uniform float _ReflectionFresnelScale;
		uniform float _ReflectionFresnelPower;
		uniform float _ReflectionIntensity;
		uniform float4 _DepthTint;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _RefractionScale1;
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float4 _ShallowTint;
		uniform float4 _WaterTint;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float _ShallowDepth;
		uniform float _ShallowOffset;
		uniform float _DepthOffset;
		uniform float _RefractedDepth1;
		uniform float4 _OceanFoamTint;
		uniform sampler2D _OceanFoamMap;
		uniform float _OceanFoamSpeed;
		uniform float _OceanFoamTiling;
		uniform float _foamMax2;
		uniform float _OceanFoamStrength;
		uniform sampler2D _BeachFoamMap;
		uniform float _BeachFoamSpeed;
		uniform float _BeachFoamTiling;
		uniform float4 _BeachFoamTint;
		uniform float _foamMax1;
		uniform float _BeachFoamStrength;
		uniform float _NormalMapStrength;
		uniform float _NormalMapTimescale;
		uniform float _NormalMapSpeed;
		uniform float _NormalMapTiling;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _SmoothnessVariance;
		uniform float _SmoothnessThreshold;
		uniform int _SmoothnessMode;
		uniform float4 _HighlightTint1;
		uniform float _Occlusion;
		uniform float _LightIndirectStrengthSpecular1;
		uniform float _HighlightOffset1;
		uniform float _HighlightSharpness1;
		uniform float _LightIndirectStrengthDiffuse1;
		uniform float _ShadowOffset1;
		uniform float _ShadowSharpness1;
		uniform float _ShadowStrength1;
		uniform float _OpacityOcean;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTextureWater1 )
		uniform float _OpacityBeach;
		uniform float _TessellationMaxDistance;
		uniform float _TessellationStrength;


		float2 AlignWithGrabTexel( float2 uv )
		{
			#if UNITY_UV_STARTS_AT_TOP
			if (_CameraDepthTexture_TexelSize.y < 0) {
				uv.y = 1 - uv.y;
			}
			#endif
			return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) * abs(_CameraDepthTexture_TexelSize.xy);
		}


		void ResetAlpha( Input SurfaceIn , SurfaceOutputStandard SurfaceOut , inout fixed4 FinalColor )
		{
			FinalColor.a = 1;
		}


		float CorrectedLinearEyeDepth( float z , float correctionFactor )
		{
			return 1.f / (z / UNITY_MATRIX_P._34 + correctionFactor);
		}


		float4 CalculateObliqueFrustumCorrection(  )
		{
			float x1 = -UNITY_MATRIX_P._31 / (UNITY_MATRIX_P._11 * UNITY_MATRIX_P._34);
			float x2 = -UNITY_MATRIX_P._32 / (UNITY_MATRIX_P._22 * UNITY_MATRIX_P._34);
			return float4(x1, x2, 0, UNITY_MATRIX_P._33 / UNITY_MATRIX_P._34 + x1 * UNITY_MATRIX_P._13 + x2 * UNITY_MATRIX_P._23);
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


		float3 PWSFGerstnerWaves( float4 wave , float wspeed , float3 p , inout float3 tangent , inout float3 binormal )
		{
				float steepness = wave.z;
				float wavelength = wave.w;
				float k = 2 * UNITY_PI / wavelength;
				float c = sqrt(9.8 / k);
				float2 d = normalize(wave.xy);
				float s = _Time.y * wspeed;
				float f = k * (dot(d, p.xz) - c * s);
				float a = steepness / k;
				float S, C;
				sincos(f, /*out*/ S, /*out*/ C);
				tangent += float3(
					-d.x * d.x * (steepness * S),
					d.x * (steepness * C),
					-d.x * d.y * (steepness * S)
					);
				binormal += float3(
					-d.x * d.y * (steepness * S),
					d.y * (steepness * C),
					-d.y * d.y * (steepness * S)
					);
				return float3(
					d.x * (a * C),
					a * S,
					d.y * (a * C)
					);
		}


		float GetRefractedDepth5_g2026( float3 tangentSpaceNormal , float4 screenPos , inout float2 uv )
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
			float GerstnerWavelength3987 = _Wave01.w;
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, 0.0,_TessellationMaxDistance,( _TessellationStrength * GerstnerWavelength3987 ));
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_66_0_g2042 = distance( _WorldSpaceCameraPos , ase_worldPos );
			float temp_output_82_0_g2042 = ( pow( ( temp_output_66_0_g2042 / _BeachRange ) , _BeachPower ) - _BeachOffset );
			float temp_output_85_0_g2042 = saturate( temp_output_82_0_g2042 );
			float temp_output_70_0_g2042 = ( pow( ( temp_output_66_0_g2042 / _OceanRange ) , _OceanPower ) - _OceanOffset );
			float temp_output_71_0_g2042 = saturate( temp_output_70_0_g2042 );
			float4 wave92_g2042 = _Wave01;
			float temp_output_41_0_g2042 = ( _WaveSpeed + PWSF_GlobalWindIntensityWater );
			float wspeed92_g2042 = temp_output_41_0_g2042;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 p92_g2042 = ase_vertex3Pos;
			float3 tangent92_g2042 = float3(1,0,0);
			float3 binormal92_g2042 = float3(0,0,1);
			float3 localPWSFGerstnerWaves92_g2042 = PWSFGerstnerWaves( wave92_g2042 , wspeed92_g2042 , p92_g2042 , tangent92_g2042 , binormal92_g2042 );
			float4 wave91_g2042 = _Wave02;
			float wspeed91_g2042 = temp_output_41_0_g2042;
			float3 p91_g2042 = ase_vertex3Pos;
			float3 tangent91_g2042 = tangent92_g2042;
			float3 binormal91_g2042 = binormal92_g2042;
			float3 localPWSFGerstnerWaves91_g2042 = PWSFGerstnerWaves( wave91_g2042 , wspeed91_g2042 , p91_g2042 , tangent91_g2042 , binormal91_g2042 );
			float4 wave93_g2042 = _Wave03;
			float wspeed93_g2042 = temp_output_41_0_g2042;
			float3 p93_g2042 = ase_vertex3Pos;
			float3 tangent93_g2042 = tangent91_g2042;
			float3 binormal93_g2042 = binormal91_g2042;
			float3 localPWSFGerstnerWaves93_g2042 = PWSFGerstnerWaves( wave93_g2042 , wspeed93_g2042 , p93_g2042 , tangent93_g2042 , binormal93_g2042 );
			float3 PWSFGerstnerLocalVertOffset1382 = ( ( temp_output_85_0_g2042 * _BeachIntensity ) * ( ( ( 1.0 - temp_output_71_0_g2042 ) * _OceanIntensity ) * ( ( _WaveHeightValueXYZ * _WaveHeight ) * ( localPWSFGerstnerWaves92_g2042 + localPWSFGerstnerWaves91_g2042 + localPWSFGerstnerWaves93_g2042 ) ) ) );
			v.vertex.xyz += PWSFGerstnerLocalVertOffset1382;
			float3 normalizeResult12_g2042 = normalize( cross( binormal93_g2042 , tangent93_g2042 ) );
			float3 PWSFGerstnerLocalVertNormal1384 = saturate( normalizeResult12_g2042 );
			v.normal = PWSFGerstnerLocalVertNormal1384;
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
			float4 temp_cast_0 = (0.0).xxxx;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 normalizeResult15_g2025 = normalize( (WorldNormalVector( i , UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) ) )) );
			float3 _NormalMapWorld1987 = normalizeResult15_g2025;
			float3 temp_cast_2 = (_NormalMapWorld1987.x).xxx;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult3_g2027 = dot( temp_cast_2 , ase_worldlightDir );
			float _PWSFNdotL2044 = dotResult3_g2027;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV3778 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode3778 = ( 0.0 + _ReflectionFresnelScale * pow( 1.0 - fresnelNdotV3778, _ReflectionFresnelPower ) );
			float4 lerpResult3783 = lerp( float4( 0,0,0,0 ) , tex2D( _ReflectionTex, ( ase_screenPosNorm + ( _PWSFNdotL2044 * _ReflectionWobble ) ).xy ) , ( fresnelNode3778 * _ReflectionIntensity ));
			float4 temp_cast_4 = (0.0).xxxx;
			#if defined(_ENABLEREFLECTION_ON)
				float4 staticSwitch3821 = lerpResult3783;
			#elif defined(_ENABLEREFLECTION_OFF)
				float4 staticSwitch3821 = temp_cast_0;
			#else
				float4 staticSwitch3821 = temp_cast_0;
			#endif
			float4 temp_cast_5 = (0.0).xxxx;
			#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch3787 = temp_cast_5;
			#else
				float4 staticSwitch3787 = staticSwitch3821;
			#endif
			float4 Reflection3788 = staticSwitch3787;
			SurfaceOutputStandard s3965 = (SurfaceOutputStandard ) 0;
			float3 temp_cast_7 = (( _NormalMapWorld1987.x * _RefractionScale1 )).xxx;
			float3 tangentSpaceNormal5_g2026 = temp_cast_7;
			float4 screenPos5_g2026 = ase_screenPos;
			float2 temp_cast_9 = (_CameraDepthTexture_TexelSize.x).xx;
			float2 uv5_g2026 = temp_cast_9;
			float localGetRefractedDepth5_g2026 = GetRefractedDepth5_g2026( tangentSpaceNormal5_g2026 , screenPos5_g2026 , uv5_g2026 );
			float2 REFRACTED_UV3970 = uv5_g2026;
			float4 screenColor3976 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,REFRACTED_UV3970);
			float screenDepth2739 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth2739 = saturate( abs( ( screenDepth2739 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _ShallowDepth ) ) );
			float4 lerpResult2752 = lerp( _ShallowTint , _WaterTint , saturate( (distanceDepth2739*1.0 + _ShallowOffset) ));
			float4 lerpResult3015 = lerp( _DepthTint , ( screenColor3976 * lerpResult2752 ) , saturate( (distanceDepth2739*-1.0 + _DepthOffset) ));
			float4 temp_cast_10 = (0.0).xxxx;
			#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch2469 = temp_cast_10;
			#else
				float4 staticSwitch2469 = lerpResult3015;
			#endif
			float4 ColorTintFinal444 = staticSwitch2469;
			#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch11_g2026 = 0.0;
			#else
				float staticSwitch11_g2026 = saturate( ( localGetRefractedDepth5_g2026 / _RefractedDepth1 ) );
			#endif
			float REFRACTED_DEPTH3971 = staticSwitch11_g2026;
			float4 lerpResult3980 = lerp( float4( 0,0,0,0 ) , ColorTintFinal444 , REFRACTED_DEPTH3971);
			float clampDepth3_g2028 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			float z6_g2028 = clampDepth3_g2028;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 localCalculateObliqueFrustumCorrection55_g2028 = CalculateObliqueFrustumCorrection();
			float dotResult53_g2028 = dot( float4( ase_vertex3Pos , 0.0 ) , localCalculateObliqueFrustumCorrection55_g2028 );
			float correctionFactor6_g2028 = dotResult53_g2028;
			float localCorrectedLinearEyeDepth6_g2028 = CorrectedLinearEyeDepth( z6_g2028 , correctionFactor6_g2028 );
			float eyeDepth50_g2028 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float temp_output_47_0_g2028 = ( eyeDepth50_g2028 - ase_screenPos.w );
			float temp_output_7_0_g2028 = ( localCorrectedLinearEyeDepth6_g2028 - abs( temp_output_47_0_g2028 ) );
			float temp_output_8_0_g2028 = saturate( temp_output_7_0_g2028 );
			float Screen_depth_behind2369 = temp_output_8_0_g2028;
			float2 temp_cast_12 = (_OceanFoamSpeed).xx;
			float2 temp_cast_13 = (_OceanFoamTiling).xx;
			float2 uv_TexCoord4_g2035 = i.uv_texcoord * temp_cast_13;
			float2 _Vector0 = float2(2,4);
			float cos5_g2035 = cos( _Vector0.x );
			float sin5_g2035 = sin( _Vector0.x );
			float2 rotator5_g2035 = mul( uv_TexCoord4_g2035 - float2( 0,0 ) , float2x2( cos5_g2035 , -sin5_g2035 , sin5_g2035 , cos5_g2035 )) + float2( 0,0 );
			float2 panner10_g2035 = ( 1.0 * _Time.y * temp_cast_12 + rotator5_g2035);
			float2 temp_cast_14 = (_OceanFoamSpeed).xx;
			float cos6_g2035 = cos( _Vector0.y );
			float sin6_g2035 = sin( _Vector0.y );
			float2 rotator6_g2035 = mul( uv_TexCoord4_g2035 - float2( 0,0 ) , float2x2( cos6_g2035 , -sin6_g2035 , sin6_g2035 , cos6_g2035 )) + float2( 0,0 );
			float2 panner11_g2035 = ( 1.0 * _Time.y * temp_cast_14 + rotator6_g2035);
			float2 temp_cast_15 = (_OceanFoamSpeed).xx;
			float2 panner8_g2035 = ( 1.0 * _Time.y * temp_cast_15 + uv_TexCoord4_g2035);
			float4 tex2DNode14_g2035 = tex2D( _OceanFoamMap, panner8_g2035 );
			float3 unityObjectToViewPos38_g2035 = UnityObjectToViewPos( ase_vertex3Pos );
			float Screen_depth2306 = localCorrectedLinearEyeDepth6_g2028;
			float temp_output_39_0_g2035 = ( unityObjectToViewPos38_g2035.z + Screen_depth2306 );
			float temp_output_16_0_g2028 = saturate( ( 1.0 / distance( _WorldSpaceCameraPos , ase_worldPos ) ) );
			float Screen_closeness2373 = temp_output_16_0_g2028;
			float temp_output_44_0_g2035 = Screen_closeness2373;
			float4 lerpResult49_g2035 = lerp( float4( 0,0,0,0 ) , ( _OceanFoamTint * Screen_depth_behind2369 * ( ( tex2D( _OceanFoamMap, panner10_g2035 ) + tex2D( _OceanFoamMap, panner11_g2035 ) + tex2DNode14_g2035 ) / 3.0 ) ) , saturate( ( ( ( temp_output_39_0_g2035 - 0.001 ) * temp_output_44_0_g2035 ) / ( ( _foamMax2 - 0.001 ) * temp_output_44_0_g2035 ) ) ));
			float4 lerpResult25_g2035 = lerp( float4( 0,0,0,0 ) , lerpResult49_g2035 , _OceanFoamStrength);
			float4 temp_cast_16 = (0.0).xxxx;
			#if defined(_ENABLEOCEANFOAM1_ON)
				float4 staticSwitch33_g2035 = lerpResult25_g2035;
			#elif defined(_ENABLEOCEANFOAM1_OFF)
				float4 staticSwitch33_g2035 = temp_cast_16;
			#else
				float4 staticSwitch33_g2035 = lerpResult25_g2035;
			#endif
			float4 PWSFOceanFoam302 = staticSwitch33_g2035;
			float4 appendResult3_g2037 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 temp_output_8_0_g2037 = ( ( appendResult3_g2037 / 10.0 ) * _BeachFoamTiling );
			float2 _Vector3 = float2(2,1);
			float cos18_g2037 = cos( _Vector3.x );
			float sin18_g2037 = sin( _Vector3.x );
			float2 rotator18_g2037 = mul( temp_output_8_0_g2037.xy - float2( 0,0 ) , float2x2( cos18_g2037 , -sin18_g2037 , sin18_g2037 , cos18_g2037 )) + float2( 0,0 );
			float2 panner24_g2037 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator18_g2037);
			float cos17_g2037 = cos( _Vector3.y );
			float sin17_g2037 = sin( _Vector3.y );
			float2 rotator17_g2037 = mul( ( temp_output_8_0_g2037 * ( _BeachFoamTiling * 5.0 ) ).xy - float2( 0,0 ) , float2x2( cos17_g2037 , -sin17_g2037 , sin17_g2037 , cos17_g2037 )) + float2( 0,0 );
			float2 panner22_g2037 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator17_g2037);
			float4 tex2DNode28_g2037 = tex2D( _BeachFoamMap, panner22_g2037 );
			float4 lerpResult36_g2037 = lerp( float4( 0,0,0,0 ) , ( tex2D( _BeachFoamMap, panner24_g2037 ) * tex2DNode28_g2037 * tex2DNode28_g2037.a * Screen_depth_behind2369 ) , _BeachFoamTint);
			float3 unityObjectToViewPos21_g2037 = UnityObjectToViewPos( ase_vertex3Pos );
			float temp_output_25_0_g2037 = ( unityObjectToViewPos21_g2037.z + Screen_depth2306 );
			float temp_output_51_0_g2037 = Screen_closeness2373;
			float4 lerpResult43_g2037 = lerp( abs( ( lerpResult36_g2037 - float4( 0,0,0,0 ) ) ) , float4( 0,0,0,0 ) , saturate( ( ( ( temp_output_25_0_g2037 - 0.001 ) * temp_output_51_0_g2037 ) / ( ( _foamMax1 - 0.001 ) * temp_output_51_0_g2037 ) ) ));
			float4 clampResult44_g2037 = clamp( lerpResult43_g2037 , float4( 0,0,0,0 ) , float4( 0.8602941,0.8602941,0.8602941,0 ) );
			float4 temp_cast_19 = (( 1.0 - _BeachFoamStrength )).xxxx;
			float4 temp_cast_20 = (0.0).xxxx;
			#if defined(_ENABLEBEACHFOAM_ON)
				float4 staticSwitch59_g2037 = pow( clampResult44_g2037 , temp_cast_19 );
			#elif defined(_ENABLEBEACHFOAM_OFF)
				float4 staticSwitch59_g2037 = temp_cast_20;
			#else
				float4 staticSwitch59_g2037 = pow( clampResult44_g2037 , temp_cast_19 );
			#endif
			float4 PWSFBeachFoam834 = staticSwitch59_g2037;
			s3965.Albedo = ( lerpResult3980 + PWSFOceanFoam302 + PWSFBeachFoam834 ).rgb;
			float mulTime6_g2025 = _Time.y * _NormalMapTimescale;
			float2 temp_cast_22 = (_NormalMapSpeed).xx;
			float2 temp_cast_23 = (_NormalMapTiling).xx;
			float2 uv_TexCoord1_g2025 = i.uv_texcoord * temp_cast_23;
			float2 _Vector1 = float2(2,4);
			float cos4_g2025 = cos( _Vector1.x );
			float sin4_g2025 = sin( _Vector1.x );
			float2 rotator4_g2025 = mul( uv_TexCoord1_g2025 - float2( 0,0 ) , float2x2( cos4_g2025 , -sin4_g2025 , sin4_g2025 , cos4_g2025 )) + float2( 0,0 );
			float2 panner7_g2025 = ( mulTime6_g2025 * temp_cast_22 + rotator4_g2025);
			float2 temp_cast_24 = (_NormalMapSpeed).xx;
			float cos3_g2025 = cos( _Vector1.y );
			float sin3_g2025 = sin( _Vector1.y );
			float2 rotator3_g2025 = mul( uv_TexCoord1_g2025 - float2( 0,0 ) , float2x2( cos3_g2025 , -sin3_g2025 , sin3_g2025 , cos3_g2025 )) + float2( 0,0 );
			float2 panner8_g2025 = ( mulTime6_g2025 * temp_cast_24 + rotator3_g2025);
			float2 temp_cast_25 = (_NormalMapSpeed).xx;
			float2 panner11_g2025 = ( mulTime6_g2025 * temp_cast_25 + uv_TexCoord1_g2025);
			float3 lerpResult23_g2025 = lerp( BlendNormals( BlendNormals( UnpackScaleNormal( tex2D( _NormalMap, panner7_g2025 ), _NormalMapStrength ) , UnpackScaleNormal( tex2D( _NormalMap, panner8_g2025 ), _NormalMapStrength ) ) , UnpackScaleNormal( tex2D( _NormalMap, panner11_g2025 ), _NormalMapStrength ) ) , float3( 0,0,0 ) , float4(0,0,0,0.5607843).rgb);
			float3 normalizeResult25_g2025 = normalize( (WorldNormalVector( i , lerpResult23_g2025 )) );
			float3 clampResult26_g2025 = clamp( float3( 0,0,0 ) , normalizeResult25_g2025 , float3( 1,1,1 ) );
			float3 _NormalMapAnimated982 = clampResult26_g2025;
			s3965.Normal = _NormalMapAnimated982;
			s3965.Emission = float3( 0,0,0 );
			s3965.Metallic = _Metallic;
			float perceptualSmoothness9_g2038 = _Smoothness;
			float3 geometricNormalWS9_g2038 = ase_worldNormal;
			float screenSpaceVariance9_g2038 = _SmoothnessVariance;
			float threshold9_g2038 = _SmoothnessThreshold;
			float localGetGeometricNormalVariance9_g2038 = GetGeometricNormalVariance( perceptualSmoothness9_g2038 , geometricNormalWS9_g2038 , screenSpaceVariance9_g2038 , threshold9_g2038 );
			float _176_g2039 = localGetGeometricNormalVariance9_g2038;
			float switchResult16_g2038 = (((i.ASEVFace>0)?(1.0):(localGetGeometricNormalVariance9_g2038)));
			float _1171_g2039 = switchResult16_g2038;
			float ENUM173_g2039 = (float)_SmoothnessMode;
			float lerpResult275_g2039 = lerp( _176_g2039 , _1171_g2039 , saturate( ENUM173_g2039 ));
			float switchResult7_g2038 = (((i.ASEVFace>0)?(localGetGeometricNormalVariance9_g2038):(0.0)));
			float _2170_g2039 = switchResult7_g2038;
			float lerpResult271_g2039 = lerp( lerpResult275_g2039 , _2170_g2039 , saturate( ( ENUM173_g2039 - 1.0 ) ));
			half THREE27_g2039 = lerpResult271_g2039;
			float PWSFSmoothnessGeometricSpecular2432 = THREE27_g2039;
			s3965.Smoothness = PWSFSmoothnessGeometricSpecular2432;
			s3965.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi3965 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g3965 = UnityGlossyEnvironmentSetup( s3965.Smoothness, data.worldViewDir, s3965.Normal, float3(0,0,0));
			gi3965 = UnityGlobalIllumination( data, s3965.Occlusion, s3965.Normal, g3965 );
			#endif

			float3 surfResult3965 = LightingStandard ( s3965, viewDir, gi3965 ).rgb;
			surfResult3965 += s3965.Emission;

			#ifdef UNITY_PASS_FORWARDADD//3965
			surfResult3965 -= s3965.Emission;
			#endif//3965
			float3 temp_cast_28 = (1.0).xxx;
			float temp_output_79_0_g2040 = _NormalMapWorld1987.x;
			float3 temp_cast_30 = (temp_output_79_0_g2040).xxx;
			float3 indirectNormal76_g2040 = WorldNormalVector( i , temp_cast_30 );
			float temp_output_29_0_g2040 = (_HighlightTint1).a;
			Unity_GlossyEnvironmentData g76_g2040 = UnityGlossyEnvironmentSetup( temp_output_29_0_g2040, data.worldViewDir, indirectNormal76_g2040, float3(0,0,0));
			float3 indirectSpecular76_g2040 = UnityGI_IndirectSpecular( data, _Occlusion, indirectNormal76_g2040, g76_g2040 );
			float3 lerpResult53_g2040 = lerp( temp_cast_28 , indirectSpecular76_g2040 , ( 1.0 - _LightIndirectStrengthSpecular1 ));
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 temp_output_35_0_g2040 = ( ase_lightColor.rgb * float3( 0,0,0 ) );
			float3 INDIRECT_SPECULAR4004 = ( lerpResult53_g2040 * temp_output_35_0_g2040 * pow( temp_output_29_0_g2040 , 1.5 ) * saturate( ( ( 0.0 + _HighlightOffset1 ) / ( ( 1.0 - _HighlightTint1.a ) * ( 1.0 - _HighlightSharpness1 ) ) ) ) * (_HighlightTint1).rgb );
			float3 temp_cast_31 = (1.0).xxx;
			float temp_output_78_0_g2040 = _PWSFNdotL2044;
			float3 temp_cast_32 = (temp_output_78_0_g2040).xxx;
			UnityGI gi83_g2040 = gi;
			float3 diffNorm83_g2040 = WorldNormalVector( i , temp_cast_32 );
			gi83_g2040 = UnityGI_Base( data, 1, diffNorm83_g2040 );
			float3 indirectDiffuse83_g2040 = gi83_g2040.indirect.diffuse + diffNorm83_g2040 * 0.0001;
			float3 lerpResult45_g2040 = lerp( temp_cast_31 , indirectDiffuse83_g2040 , ( 1.0 - _LightIndirectStrengthDiffuse1 ));
			float temp_output_33_0_g2040 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			float lerpResult48_g2040 = lerp( temp_output_33_0_g2040 , ( saturate( ( ( temp_output_78_0_g2040 + _ShadowOffset1 ) / _ShadowSharpness1 ) ) * ase_lightAtten ) , _ShadowStrength1);
			float3 LIGHT_SHADOWS4005 = saturate( ( ( lerpResult45_g2040 * ase_lightColor.a * temp_output_33_0_g2040 ) + ( ase_lightColor.rgb * lerpResult48_g2040 ) ) );
			float3 temp_cast_33 = (0.0).xxx;
			float3 specColor14_g2041 = temp_cast_33;
			float3 temp_cast_35 = (ColorTintFinal444.r).xxx;
			float3 grazingTerm14_g2041 = temp_cast_35;
			float dotResult5_g2041 = dot( ase_worldNormal , ase_worldViewDir );
			float nv14_g2041 = dotResult5_g2041;
			float localCallFresnelLerpFast14_g2041 = CallFresnelLerpFast( specColor14_g2041 , grazingTerm14_g2041 , nv14_g2041 );
			float Opacity2371 = ( i.vertexColor.a * ( 1.0 - _OpacityOcean ) );
			float PWSFLightingReduceatHorizon3992 = max( 0.0 , ( 1.0 - ( localCallFresnelLerpFast14_g2041 * Opacity2371 ) ) );
			float fresnelNdotV19_g2028 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode19_g2028 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV19_g2028, 5.0 ) );
			float lerpResult26_g2028 = lerp( 0.0 , _NormalMapAnimated982.x , ( Opacity2371 * 0.0 * temp_output_16_0_g2028 * ( 1.0 - fresnelNode19_g2028 ) * temp_output_8_0_g2028 ));
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 temp_output_28_0_g2028 = ( lerpResult26_g2028 + ase_grabScreenPosNorm );
			float4 screenColor30_g2028 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTextureWater1,temp_output_28_0_g2028.xy/temp_output_28_0_g2028.w);
			float4 temp_cast_38 = (0.0).xxxx;
			#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch31_g2028 = temp_cast_38;
			#else
				float4 staticSwitch31_g2028 = screenColor30_g2028;
			#endif
			float4 Grab_screen_color2433 = staticSwitch31_g2028;
			float DepthTintAlpha3003 = ( saturate( (distanceDepth2739*-5.0 + 1.0) ) * _OpacityBeach );
			#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch3180 = 0.0;
			#else
				float staticSwitch3180 = ( 1.0 - ( ( 1.0 - DepthTintAlpha3003 ) * Opacity2371 ) );
			#endif
			float4 lerpResult3006 = lerp( ( Reflection3788 + float4( ( ( surfResult3965 + INDIRECT_SPECULAR4004 ) * LIGHT_SHADOWS4005 * PWSFLightingReduceatHorizon3992 ) , 0.0 ) ) , Grab_screen_color2433 , staticSwitch3180);
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
	}
}
/*ASEBEGIN
Version=17400
194;143;1664;826;6582.932;5897.396;2.057374;True;True
Node;AmplifyShaderEditor.CommentaryNode;3392;-5507.21,-4134.791;Inherit;False;1079.105;251.1047;Comment;3;982;1987;4017;PWSF Water Normal Map Swirling SP v3.0;0.7372549,0,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;4017;-5426.029,-4067.124;Inherit;False;PWSF Water Normal Map Swirling SP v3.0;26;;2025;c1b39032cbbd97a4da4a74689588ba08;0;0;2;FLOAT3;29;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;2525;-5499.566,-5789.413;Inherit;False;2346.933;1077.252;hex code 4E83A9FF;29;444;2469;3015;2467;3169;442;2752;3003;2746;3171;3157;954;345;3168;3170;2548;2999;2743;2744;3000;2745;2739;3002;3001;3193;2325;3976;3977;3978;Color Tint;0,0.3020356,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;3966;-3015.187,-3466.745;Inherit;False;1601.324;336.4244;https://catlikecoding.com/unity/tutorials/flow/looking-through-water/;6;3972;3971;3970;3968;3967;3996;PWSF Water Refracted Depth v3.0;0.945098,0.9386187,0.02352941,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;2325;-4162.377,-5048.362;Inherit;False;901.978;311.5114;;5;3172;2549;2371;3159;2870;Opacity;0,0.9529412,0.05987054,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1987;-4979.753,-3999.952;Float;False;_NormalMapWorld;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;3967;-2964.737,-3372.167;Float;False;Global;_CameraDepthTexture_TexelSize;_CameraDepthTexture_TexelSize;2;0;Create;True;0;0;True;0;0,0,0,0;0.0005302227,0.001068376,1886,936;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;2549;-4122.245,-4829.731;Float;False;Property;_OpacityOcean;Opacity Ocean;8;0;Create;True;0;0;False;0;0;0.148;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3968;-2367.811,-3418.826;Inherit;False;1987;_NormalMapWorld;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;3193;-5474.215,-5327.39;Inherit;False;Property;_ShallowDepth;Shallow Depth;3;0;Create;True;0;0;False;0;46.47059;55.8;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;3172;-3837.69,-4826.873;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2744;-5143.938,-4891.588;Float;False;Constant;_WaveColorScale;Wave Color Scale;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2745;-5207.785,-4798.557;Float;False;Property;_ShallowOffset;Shallow Offset;4;0;Create;True;0;0;False;0;0.2;0.195;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;3996;-2111.984,-3394.635;Inherit;False;PWSF Water Refracted Depth v3.0;55;;2026;b055c91979830a5458db81f3e6e93c44;0;3;22;FLOAT4;0,0,0,0;False;15;FLOAT;0;False;16;FLOAT;0;False;2;FLOAT2;14;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;2739;-5187.934,-5379.033;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;2870;-4084.228,-5004.24;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;2743;-4891.833,-4857.273;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;2040;-4324.202,-3162.987;Inherit;False;1121.825;144.6242;Comment;3;2044;3614;3942;PWSF NdotL v3.0;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3159;-3657.012,-4912.387;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3970;-1693.946,-3417.018;Float;False;REFRACTED_UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;3168;-4864.482,-5417.216;Float;False;Property;_DepthOffset;Depth Offset;6;0;Create;True;0;0;False;0;0.5;0.495;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3978;-4617.276,-5231.554;Inherit;False;3970;REFRACTED_UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;982;-4986.015,-4084.444;Float;False;_NormalMapAnimated;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2371;-3483.169,-4917.875;Float;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;954;-4874.826,-5035.71;Float;False;Property;_WaterTint;Water Tint;1;0;Create;True;0;0;False;1;Header (COLOR TINT);0.08627451,0.2941177,0.3490196,1;0.1882345,0.6287979,0.8196079,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;2746;-4630.055,-4858.715;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3170;-4765.805,-5496.417;Float;False;Constant;_DepthScale;Depth Scale;3;0;Create;True;0;0;False;0;-1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3396;-4324.944,-3668;Inherit;False;1161.378;399.116;;7;2306;2369;2373;2433;2374;2379;3941;PWSF Water Custom Grab Pass v3.0;0,0.9529412,0.05882353,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;3614;-4289.74,-3105.49;Inherit;False;1987;_NormalMapWorld;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;345;-4858.662,-5209.795;Float;False;Property;_ShallowTint;Shallow Tint;2;0;Create;True;0;0;False;0;0.003921569,0.7529412,0.7137255,1;0.2463947,0.4562678,0.462264,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;3171;-4510.18,-5378.989;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;3942;-4011.715,-3115.688;Inherit;False;PWSF NdotL v3.0;-1;;2027;6f4e02b9e119b5744971c0b68e8eed5b;0;1;7;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;3976;-4401.294,-5230.903;Float;False;Global;_GrabScreen1;Grab Screen 1;5;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;2752;-4438.166,-5052.768;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;2374;-4247.979,-3498.935;Inherit;False;2371;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2379;-4302.287,-3588.275;Inherit;False;982;_NormalMapAnimated;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2044;-3589.439,-3110.64;Float;False;_PWSFNdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3977;-4191.218,-5178.514;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;442;-4385.24,-5561.782;Float;False;Property;_DepthTint;Depth Tint;5;0;Create;True;0;0;False;0;0.08627451,0.2941177,0.3490196,1;0.18009,0.3868915,0.518,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;3941;-4010.809,-3596.862;Inherit;False;PWSF Water Custom Grab Pass v3.0;-1;;2028;84db7f407c2735b4986e5ebfc2f85480;0;2;45;FLOAT;0;False;46;FLOAT;0;False;8;FLOAT;0;COLOR;38;FLOAT;39;FLOAT;40;FLOAT;41;FLOAT;42;FLOAT;43;FLOAT;44
Node;AmplifyShaderEditor.SaturateNode;3169;-4289.019,-5381.794;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3808;-5488.234,-2952.09;Inherit;False;2390.976;726.7626;Comment;18;3824;3911;3788;3787;3821;3785;3783;3822;3782;3781;3778;3779;3776;3774;3809;3775;3773;3772;Reflection;0.9837489,1,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3773;-5404.179,-2514.022;Float;False;Property;_ReflectionWobble;Reflection Wobble;52;0;Create;True;0;0;False;0;0;0.536;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3387;-5503.359,-4668.273;Inherit;False;1102.011;254.1616;Comment;5;834;2435;2400;3870;3943;PWSF Water Foam Beach v3.0;1,0.654902,0.3019608,1;0;0
Node;AmplifyShaderEditor.LerpOp;3015;-3991.538,-5428.373;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;3772;-5332.172,-2616.861;Inherit;False;2044;_PWSFNdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2373;-3507.932,-3530.687;Float;False;Screen_closeness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2306;-3507.302,-3368.473;Float;False;Screen_depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2369;-3498.478,-3453.186;Float;False;Screen_depth_behind;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1743;-5502.708,-4370.462;Inherit;False;1107.159;194.9096;Comment;4;302;1779;3869;3944;PWSF Water Foam Ocean v3.0;1,0.654902,0.3019608,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;2467;-3888.534,-5187.184;Float;False;Constant;_Float11;Float 11;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3002;-5131.296,-5625.646;Float;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3001;-5135.437,-5728.77;Float;False;Constant;_Float10;Float 10;8;0;Create;True;0;0;False;0;-5;-5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;3000;-4930.401,-5728.787;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;2469;-3719.125,-5427.77;Float;False;Property;_Keyword1;Keyword 1;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3776;-5116.59,-2608.231;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;3809;-5305.048,-2813.545;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1779;-5576.317,-4318.9;Inherit;False;2369;Screen_depth_behind;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3774;-5148.685,-2421.93;Float;False;Property;_ReflectionFresnelScale;Reflection Fresnel Scale;53;0;Create;True;0;0;False;0;1;0.352;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3775;-5137.903,-2340.076;Float;False;Property;_ReflectionFresnelPower;Reflection Fresnel Power;54;0;Create;True;0;0;False;0;0;0.78;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2435;-5512.175,-4592.038;Inherit;False;2373;Screen_closeness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2400;-5505.792,-4505.889;Inherit;False;2306;Screen_depth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3359;-4335.179,-4637.568;Inherit;False;1332.293;330.1035;http://graphics.pixar.com/library/BumpRoughness/paper.pdf - equation (3);4;3538;2432;3896;4016;PWSF Smoothness Geometric Specular v3.0;0,0.4024811,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3824;-4966.868,-2703.606;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;2999;-4707.271,-5731.357;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;4016;-4182.36,-4496.381;Inherit;False;PWSF Smoothness Geometric Specular v3.0;20;;2038;87fb58cae055bea429a44169a3c936fd;0;1;25;FLOAT;0;False;3;FLOAT;17;FLOAT;20;FLOAT;21
Node;AmplifyShaderEditor.FunctionNode;3943;-5205.442,-4587.129;Inherit;False;PWSF Water Foam Beach v3.0;66;;2037;4ddef9c0ccfa2604e9a2a38dcaf12896;0;3;51;FLOAT;0;False;52;FLOAT;0;False;53;FLOAT;0;False;2;COLOR;0;FLOAT;60
Node;AmplifyShaderEditor.IntNode;3538;-4098.705,-4582.465;Float;False;Property;_SmoothnessMode;Smoothness Mode;19;1;[Enum];Create;True;3;Smoothness Cull Off;0;Smoothness Cull Front;1;Smoothness Cull Back;2;0;False;1;Header (SMOOTHNESS);0;0;0;1;INT;0
Node;AmplifyShaderEditor.CommentaryNode;3998;-3013.629,-4137.579;Inherit;False;1250.437;566.8701;Comment;9;4006;4005;4004;4014;4000;3999;4002;4015;4001;PWSF Water Custom Lighting v3.0;0.945098,0.9372549,0.02352941,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3971;-1698.109,-3325.074;Float;False;REFRACTED_DEPTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2548;-4777.718,-5608.749;Float;False;Property;_OpacityBeach;Opacity Beach;7;0;Create;True;0;0;False;1;Header (OPACITY);0;0.393;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;3911;-4970.158,-2901.642;Inherit;True;Global;_ReflectionTex;_ReflectionTex;38;0;Create;True;0;0;False;0;None;;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;3944;-5206.646,-4302.642;Inherit;False;PWSF Water Foam Ocean v3.0;58;;2035;3bf6834ee5afdac4b9545ba41b4dcdad;0;3;30;FLOAT;0;False;44;FLOAT;0;False;36;FLOAT;0;False;2;COLOR;0;FLOAT;50
Node;AmplifyShaderEditor.RangedFloatNode;3779;-4793.684,-2341.986;Float;False;Property;_ReflectionIntensity;Reflection Intensity;51;0;Create;True;0;0;False;0;0.7357025;0.214;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;3778;-4777.131,-2518.093;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;444;-3379.952,-5428.463;Float;False;ColorTintFinal;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;3979;-2523.344,-5206.616;Inherit;False;3971;REFRACTED_DEPTH;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3079;-2497.248,-5287.106;Inherit;False;444;ColorTintFinal;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3157;-4481.139,-5734.498;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3782;-4516.829,-2524.979;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;3896;-3704.902,-4540.917;Inherit;False;PWSF Enum Switch;-1;;2039;3e9cc2aefa7c15f46bb8ad0edeb00b30;1,34,1;9;134;FLOAT;0;False;146;FLOAT;0;False;133;FLOAT;0;False;144;FLOAT;0;False;143;FLOAT;0;False;155;FLOAT;0;False;156;FLOAT;0;False;150;FLOAT;0;False;158;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;834;-4697.318,-4618.486;Float;False;PWSFBeachFoam;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3781;-4707.472,-2740.521;Inherit;True;Property;;;36;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;302;-4727.311,-4330.375;Float;False;PWSFOceanFoam;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4015;-2972.034,-4075.645;Float;False;Property;_Occlusion;Occlusion;24;0;Create;True;0;0;False;0;0.5;0.344;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;4002;-2934.552,-3887.632;Inherit;False;1987;_NormalMapWorld;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;3999;-2895.302,-3972.425;Inherit;False;2044;_PWSFNdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3988;-4298.418,-4152.348;Inherit;False;1011.476;252.2012;max(0, 1-(FresnelLerpFast(...)*Opacity));4;3992;3990;3989;3997;PWSF Lighting Reduce at Horizon v3.0;0,0.334579,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2432;-3424.154,-4564.507;Float;False;PWSFSmoothnessGeometricSpecular;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3989;-4271.138,-4106.814;Inherit;False;444;ColorTintFinal;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;835;-2259.382,-5066.817;Inherit;False;834;PWSFBeachFoam;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;3990;-4246.917,-4022.947;Inherit;False;2371;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3003;-4243.042,-5739.639;Float;False;DepthTintAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;303;-2259.304,-5137.464;Inherit;False;302;PWSFOceanFoam;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;3783;-4361.849,-2757.721;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3822;-4238.013,-2875.529;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;4014;-2613.817,-4053.054;Inherit;False;PWSF Water Custom Lighting v3.0;9;;2040;7d24d5506c1d9d048995cd814d89c1df;0;5;88;FLOAT;0;False;78;FLOAT;0;False;79;FLOAT;0;False;81;FLOAT;0;False;82;FLOAT;0;False;3;FLOAT3;74;FLOAT3;84;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;3980;-2189.353,-5259.14;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;1387;-5553.954,-3831.753;Inherit;False;1182.586;833.1803;https://catlikecoding.com/unity/tutorials/flow/waves/;14;1384;1382;3751;3753;3524;3919;3754;3921;3918;3285;3752;3920;3964;3987;PWSF Water Gerstner Waves SP v3.0;0.7372549,0,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3921;-5506.845,-3092.803;Inherit;False;Property;_BeachIntensity;Beach Intensity ;46;0;Create;True;0;0;False;1;Header (Beach Wave Controls);0.8;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3752;-5503.33,-3327.405;Float;False;Property;_OceanOffset;Ocean Offset ;45;0;Create;True;0;0;False;0;3.3;1.8;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3524;-5517.183,-3713.998;Half;False;Global;PWSF_GlobalWindIntensityWater;PWSF_GlobalWindIntensityWater;27;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3919;-5505.776,-3406.079;Float;False;Property;_BeachPower;Beach Power ;48;0;Create;True;0;0;False;0;1;0.136;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3754;-5521.424,-3637.474;Float;False;Property;_OceanRange;Ocean Range ;43;0;Create;True;0;0;False;0;5;13.3;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3751;-5517.553,-3485.653;Float;False;Property;_OceanPower;Ocean Power ;44;0;Create;True;0;0;False;0;1;0.399;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3753;-5509.886,-3175.826;Inherit;False;Property;_OceanIntensity;Ocean Intensity ;42;0;Create;True;0;0;False;1;Header (Ocean Wave Controls);0.8;0.947;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3920;-5500.431,-3249.975;Float;False;Property;_BeachOffset;Beach Offset ;49;0;Create;True;0;0;False;0;3.3;36.3;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3285;-5521.374,-3790.048;Float;False;Property;_WaveSpeed;Wave Speed;39;0;Create;True;0;0;True;0;1.1;3.9;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;4004;-2048.615,-3924.056;Float;False;INDIRECT_SPECULAR;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;2143;-2262.327,-4696.848;Inherit;False;2432;PWSFSmoothnessGeometricSpecular;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;3997;-4038.394,-4076.227;Inherit;False;PWSF Lighting Reduce at Horizon v3.0;-1;;2041;58046f1a7a923914eb9db82c67a2b04d;0;3;15;FLOAT;0;False;16;FLOAT;0;False;17;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3785;-3906.61,-2564.356;Float;False;Constant;_Float2;Float 2;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3005;-1742.02,-4768.018;Inherit;False;3003;DepthTintAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3560;-1955.07,-5112.781;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;727;-2464.422,-4989.289;Inherit;False;982;_NormalMapAnimated;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;3962;-2180.509,-4780.512;Float;False;Property;_Metallic;Metallic;25;0;Create;True;0;0;False;0;0.25;0.082;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3821;-4040.936,-2761.771;Float;False;Property;_EnableReflection;Enable Reflection;50;0;Create;True;0;0;False;1;Header (REFLECTION);1;1;0;True;;KeywordEnum;2;on;off;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3918;-5523.276,-3561.621;Float;False;Property;_BeachRange;Beach Range ;47;0;Create;True;0;0;False;0;5;8.5;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;4005;-2065.029,-4005.017;Float;False;LIGHT_SHADOWS;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;3964;-5193.869,-3773.933;Inherit;False;PWSF Water Gerstner Waves SP v3.0;32;;2042;24e3d03e41f06ab438dd9cfa4b80e497;2,73,2,88,1;10;26;FLOAT;0;False;42;FLOAT;0;False;60;FLOAT;32;False;76;FLOAT;32;False;61;FLOAT;1;False;89;FLOAT;1;False;63;FLOAT;0;False;81;FLOAT;0;False;67;FLOAT;1;False;83;FLOAT;1;False;3;FLOAT3;23;FLOAT;24;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;3166;-1513.807,-4762.646;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;4007;-1705.615,-5136.357;Inherit;False;4004;INDIRECT_SPECULAR;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;3787;-3749.357,-2768.127;Float;False;Property;_Keyword3;Keyword 3;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;3164;-1549.854,-4647.383;Inherit;False;2371;Opacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3992;-3662.184,-4107.993;Float;False;PWSFLightingReduceatHorizon;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomStandardSurface;3965;-1715.563,-5010.501;Inherit;False;Metallic;World;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;3993;-1632.255,-5232.038;Inherit;False;3992;PWSFLightingReduceatHorizon;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3788;-3453.37,-2770.115;Float;False;Reflection;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;4009;-1371.585,-5007.435;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;4008;-1548.697,-5317.155;Inherit;False;4005;LIGHT_SHADOWS;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3165;-1352.831,-4762.631;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3987;-4693.351,-3705.163;Float;False;GerstnerWavelength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3181;-1204.487,-4645.066;Float;False;Constant;_Float8;Float 8;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3982;-1274.004,-4344.171;Inherit;False;3987;GerstnerWavelength;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;3167;-1215.927,-4761.954;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2433;-3506.677,-3613.981;Float;False;Grab_screen_color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3981;-1291.638,-4427.22;Float;False;Property;_TessellationStrength;Tessellation Strength;40;0;Create;True;0;0;False;0;0.1472393;0.046;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3806;-1116.214,-5120.295;Inherit;False;3788;Reflection;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3994;-1192.589,-5007.981;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1382;-4703.089,-3617.743;Float;False;PWSFGerstnerLocalVertOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3984;-949.2398,-4392.503;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3985;-1124.656,-4141.327;Float;False;Property;_TessellationMaxDistance;Tessellation Max Distance;41;0;Create;True;0;0;False;0;3000;300;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;3004;-1024.94,-4859.577;Inherit;False;2433;Grab_screen_color;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3823;-922.2465,-5033.926;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1384;-4702.211,-3782.554;Float;False;PWSFGerstnerLocalVertNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;3983;-1126.878,-4221.028;Float;False;Constant;_TessellationDistanceMin;Tessellation Distance Min;24;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;3180;-1049.125,-4766.651;Float;False;Property;_Keyword6;Keyword 6;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3006;-721.1799,-5032.16;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;4001;-3008.421,-3809.317;Inherit;False;1384;PWSFGerstnerLocalVertNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1261;-972.592,-4598.369;Inherit;False;1382;PWSFGerstnerLocalVertOffset;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;4000;-2947.602,-3730.781;Inherit;False;982;_NormalMapAnimated;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;2732;-2109.682,-4886.918;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3869;-4729.9,-4249.287;Inherit;False;PWSFOceanFoamAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;2731;-2489.589,-4883.896;Inherit;False;1384;PWSFGerstnerLocalVertNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;4006;-2069.613,-4075.85;Float;False;LIGHT_COLOR_FALLOFF;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1388;-982.6292,-4518.116;Inherit;False;1384;PWSFGerstnerLocalVertNormal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3870;-4704.241,-4532.67;Inherit;False;PWSFBeachFoamAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3972;-2642.497,-3412.569;Float;False;TexelSize;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DistanceBasedTessNode;3986;-783.5158,-4326.293;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.IntNode;3958;-489.1665,-5350.348;Float;False;Property;_CullMode;Cull Mode;0;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CullMode;True;1;Header (RENDERING OPTIONS);2;0;0;1;INT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3963;-478.4442,-5263.554;Float;False;True;-1;6;;0;0;CustomLighting;PWS/Water/Ocean Pro SP 2019_02 v3.2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;False;-10;False;Opaque;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;18.9;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;3958;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1987;0;4017;0
WireConnection;3172;0;2549;0
WireConnection;3996;15;3968;0
WireConnection;3996;16;3967;0
WireConnection;2739;0;3193;0
WireConnection;2743;0;2739;0
WireConnection;2743;1;2744;0
WireConnection;2743;2;2745;0
WireConnection;3159;0;2870;4
WireConnection;3159;1;3172;0
WireConnection;3970;0;3996;14
WireConnection;982;0;4017;29
WireConnection;2371;0;3159;0
WireConnection;2746;0;2743;0
WireConnection;3171;0;2739;0
WireConnection;3171;1;3170;0
WireConnection;3171;2;3168;0
WireConnection;3942;7;3614;0
WireConnection;3976;0;3978;0
WireConnection;2752;0;345;0
WireConnection;2752;1;954;0
WireConnection;2752;2;2746;0
WireConnection;2044;0;3942;0
WireConnection;3977;0;3976;0
WireConnection;3977;1;2752;0
WireConnection;3941;45;2379;0
WireConnection;3941;46;2374;0
WireConnection;3169;0;3171;0
WireConnection;3015;0;442;0
WireConnection;3015;1;3977;0
WireConnection;3015;2;3169;0
WireConnection;2373;0;3941;39
WireConnection;2306;0;3941;43
WireConnection;2369;0;3941;40
WireConnection;3000;0;2739;0
WireConnection;3000;1;3001;0
WireConnection;3000;2;3002;0
WireConnection;2469;1;3015;0
WireConnection;2469;0;2467;0
WireConnection;3776;0;3772;0
WireConnection;3776;1;3773;0
WireConnection;3824;0;3809;0
WireConnection;3824;1;3776;0
WireConnection;2999;0;3000;0
WireConnection;3943;51;2435;0
WireConnection;3943;52;2400;0
WireConnection;3943;53;1779;0
WireConnection;3971;0;3996;0
WireConnection;3944;30;1779;0
WireConnection;3944;44;2435;0
WireConnection;3944;36;2400;0
WireConnection;3778;2;3774;0
WireConnection;3778;3;3775;0
WireConnection;444;0;2469;0
WireConnection;3157;0;2999;0
WireConnection;3157;1;2548;0
WireConnection;3782;0;3778;0
WireConnection;3782;1;3779;0
WireConnection;3896;134;3538;0
WireConnection;3896;146;4016;17
WireConnection;3896;133;4016;20
WireConnection;3896;144;4016;21
WireConnection;834;0;3943;0
WireConnection;3781;0;3911;0
WireConnection;3781;1;3824;0
WireConnection;302;0;3944;0
WireConnection;2432;0;3896;0
WireConnection;3003;0;3157;0
WireConnection;3783;1;3781;0
WireConnection;3783;2;3782;0
WireConnection;4014;88;4015;0
WireConnection;4014;78;3999;0
WireConnection;4014;79;4002;0
WireConnection;3980;1;3079;0
WireConnection;3980;2;3979;0
WireConnection;4004;0;4014;0
WireConnection;3997;15;3989;0
WireConnection;3997;16;3990;0
WireConnection;3560;0;3980;0
WireConnection;3560;1;303;0
WireConnection;3560;2;835;0
WireConnection;3821;1;3783;0
WireConnection;3821;0;3822;0
WireConnection;4005;0;4014;84
WireConnection;3964;26;3285;0
WireConnection;3964;42;3524;0
WireConnection;3964;60;3754;0
WireConnection;3964;76;3918;0
WireConnection;3964;61;3751;0
WireConnection;3964;89;3919;0
WireConnection;3964;63;3752;0
WireConnection;3964;81;3920;0
WireConnection;3964;67;3753;0
WireConnection;3964;83;3921;0
WireConnection;3166;0;3005;0
WireConnection;3787;1;3821;0
WireConnection;3787;0;3785;0
WireConnection;3992;0;3997;0
WireConnection;3965;0;3560;0
WireConnection;3965;1;727;0
WireConnection;3965;3;3962;0
WireConnection;3965;4;2143;0
WireConnection;3788;0;3787;0
WireConnection;4009;0;3965;0
WireConnection;4009;1;4007;0
WireConnection;3165;0;3166;0
WireConnection;3165;1;3164;0
WireConnection;3987;0;3964;24
WireConnection;3167;0;3165;0
WireConnection;2433;0;3941;38
WireConnection;3994;0;4009;0
WireConnection;3994;1;4008;0
WireConnection;3994;2;3993;0
WireConnection;1382;0;3964;0
WireConnection;3984;0;3981;0
WireConnection;3984;1;3982;0
WireConnection;3823;0;3806;0
WireConnection;3823;1;3994;0
WireConnection;1384;0;3964;23
WireConnection;3180;1;3167;0
WireConnection;3180;0;3181;0
WireConnection;3006;0;3823;0
WireConnection;3006;1;3004;0
WireConnection;3006;2;3180;0
WireConnection;2732;0;727;0
WireConnection;2732;1;2731;0
WireConnection;3869;0;3944;50
WireConnection;4006;0;4014;74
WireConnection;3870;0;3943;60
WireConnection;3972;0;3967;0
WireConnection;3986;0;3984;0
WireConnection;3986;1;3983;0
WireConnection;3986;2;3985;0
WireConnection;3963;13;3006;0
WireConnection;3963;11;1261;0
WireConnection;3963;12;1388;0
WireConnection;3963;14;3986;0
ASEEND*/
//CHKSM=B023793E864B7F51D7F409E9A55E5338EF3B274B