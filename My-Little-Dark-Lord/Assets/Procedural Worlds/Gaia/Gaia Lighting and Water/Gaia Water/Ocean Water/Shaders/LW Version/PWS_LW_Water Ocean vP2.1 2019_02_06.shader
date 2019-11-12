// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PWS/LW/Water/Ocean vP2.1 2019_02_06"
{
    Properties
    {
		[Header (COLOR TINT)]_WaterTint("Water Tint", Color) = (0,0.7294118,1,1)
		_DepthTint("Depth Tint", Color) = (0.08627451,0.2941177,0.3490196,1)
		_DepthDistance("Depth Distance", Range( 0 , 500)) = 0
		[Header (OPACITY)]_OpacityOcean("Opacity Ocean", Range( 0 , 0.5)) = 0
		[Toggle]_IgnoreVertexColor("Ignore Vertex Color", Float) = 1
		[Header (GLOBAL SETTINGS)]_Occlusion("Occlusion", Range( 0 , 1)) = 0.9
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.95
		_SmoothnessVariance("Smoothness Variance", Range( 0 , 1)) = 0
		_SmoothnessThreshold("Smoothness Threshold", Range( 0 , 1)) = 0
		[Header (LIGHTING)]_LightIndirectStrengthSpecular("Light Indirect Strength Specular", Range( 0 , 1)) = 1
		_ShadowIntensity("Shadow Intensity", Range( 0 , 1)) = 0
		_ShadowDepth("Shadow Depth", Range( 0.9 , 1)) = 0.6532351
		_ShadowFresnelPower("Shadow Fresnel Power", Range( 0 , 1)) = 0
		_ShadowFrenelScale("Shadow Frenel Scale", Range( 0 , 1)) = 0
		[Header (REFLECTION)][Toggle]_EnableReflection("Enable Reflection", Float) = 1
		_ReflectionCubeMap("Reflection Cube Map", CUBE) = "black" {}
		_ReflectionIntensity("Reflection Intensity", Range( 0 , 1)) = 0
		_ReflectionWobble("Reflection Wobble", Range( 0 , 1)) = 0.6218035
		_ReflectionFresnelPower("Reflection Fresnel Power", Range( 0 , 10)) = 2.66
		_ReflectionFrenelScale("Reflection Frenel Scale", Range( 0 , 1)) = 0.372
		[Normal][Header (NORMAL MAP)]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalMapTiling("Normal Map Tiling", Range( 0 , 100)) = 1
		_NormalMapStrength("Normal Map Strength", Range( 0 , 1)) = 1
		_NormalMapSpeed("Normal Map Speed", Range( 0 , 0.1)) = 0
		_NormalMapTimescale("Normal Map Timescale", Range( 0 , 1)) = 1
		[Header (WAVE)]_WaveTileUV("Wave Tile UV", Range( 0 , 100)) = 0
		_WaveSpeed("Wave Speed", Range( 0 , 1)) = 0.5560129
		_WaveHeight("Wave Height", Range( 0 , 1.5)) = 1
		_WaveNoiseShape("Wave Noise Shape", Vector) = (0.5,0,0,0)
		_WaveUp("Wave Up", Vector) = (0,1,0,0)
		_WaveDirection("Wave Direction", Vector) = (1,0,0,0)
		[Header (OCEAN FOAM)][Toggle]_EnableOceanFoam("Enable Ocean Foam ", Float) = 1
		_OceanFoamMap("Ocean Foam Map", 2D) = "white" {}
		_OceanFoamTint("Ocean Foam Tint", Color) = (1,1,1,1)
		_OceanFoamTiling("Ocean Foam Tiling", Range( 0.01 , 100)) = 2.032399
		[Gamma]_OacenFoamStrenght("Oacen Foam Strenght", Range( 0 , 1)) = 0.28031
		_OceanFoamSpeed("Ocean Foam Speed", Range( 0 , 0.5)) = 0.06184824
		_OceanFoamDistance("Ocean Foam Distance", Range( 0 , 1000)) = 1
		[Header (BEACH FOAM)][Toggle]_EnableBeachFoam("Enable Beach Foam", Float) = 1
		_BeachFoamMap("Beach Foam Map", 2D) = "black" {}
		_BeachFoamTint("Beach Foam Tint", Color) = (1,1,1,1)
		_BeachFoamTiling("Beach Foam Tiling", Range( 0 , 1)) = 100
		_BeachFoamStrenght("Beach Foam Strenght", Range( 0.1 , 0.75)) = 0.75
		_BeachFoamSpeed("Beach Foam Speed", Range( 0 , 0.5)) = 0
		_foamMax("Beach Foam Distance", Range( 0.001 , 25)) = 0.7027565
		[Enum(UnityEngine.Rendering.CullMode)][Header (RENDERING OPTIONS)]_CullMode("Cull Mode", Int) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
    }


    SubShader
    {
		
        Tags { "RenderPipeline"="LightweightPipeline" "RenderType"="Opaque" "Queue"="Transparent-10" }

		Cull [_CullMode]
		HLSLINCLUDE
		#pragma target 3.5
		ENDHLSL
		
        Pass
        {
			
        	Tags { "LightMode"="LightweightForward" }

        	Name "Base"
			Blend One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
            
        	HLSLPROGRAM
            #define _RECEIVE_SHADOWS_OFF 1
            #define ASE_SRP_VERSION 60901
            #define REQUIRE_DEPTH_TEXTURE 1
            #define REQUIRE_OPAQUE_TEXTURE 1
            #define _NORMALMAP 1

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            

        	// -------------------------------------
            // Lightweight Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            
        	// -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
        	#pragma fragment frag

        	#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
        	#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
        	#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
		
			

			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _OceanFoamMap;
			sampler2D _BeachFoamMap;
			sampler2D _NormalMap;
			samplerCUBE _ReflectionCubeMap;
			CBUFFER_START( UnityPerMaterial )
			int _CullMode;
			float3 _WaveUp;
			float _WaveHeight;
			float _WaveSpeed;
			float2 _WaveDirection;
			float2 _WaveNoiseShape;
			float _WaveTileUV;
			float _EnableOceanFoam;
			float4 _OceanFoamTint;
			float _OceanFoamSpeed;
			float _OceanFoamTiling;
			float _OceanFoamDistance;
			float _OacenFoamStrenght;
			float _EnableBeachFoam;
			float _BeachFoamSpeed;
			float _BeachFoamTiling;
			float4 _BeachFoamTint;
			float _foamMax;
			float _BeachFoamStrenght;
			float4 _DepthTint;
			float4 _WaterTint;
			float _DepthDistance;
			float _IgnoreVertexColor;
			float _OpacityOcean;
			float _NormalMapStrength;
			float _NormalMapTimescale;
			float _NormalMapSpeed;
			float _NormalMapTiling;
			float4 _NormalMap_ST;
			float _Occlusion;
			float _LightIndirectStrengthSpecular;
			float _EnableReflection;
			float _ReflectionWobble;
			float _ReflectionFrenelScale;
			float _ReflectionFresnelPower;
			float _ReflectionIntensity;
			float _ShadowFrenelScale;
			float _ShadowFresnelPower;
			float _ShadowIntensity;
			float _ShadowDepth;
			float _Metallic;
			float _Smoothness;
			float _SmoothnessVariance;
			float _SmoothnessThreshold;
			CBUFFER_END

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;
                float4 ase_tangent : TANGENT;
                float4 texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        	struct GraphVertexOutput
            {
                float4 clipPos                : SV_POSITION;
                float4 lightmapUVOrVertexSH	  : TEXCOORD0;
        		half4 fogFactorAndVertexLight : TEXCOORD1; // x: fogFactor, yzw: vertex light
            	float4 shadowCoord            : TEXCOORD2;
				float4 tSpace0					: TEXCOORD3;
				float4 tSpace1					: TEXCOORD4;
				float4 tSpace2					: TEXCOORD5;
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            	UNITY_VERTEX_OUTPUT_STEREO
            };

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
			

            GraphVertexOutput vert (GraphVertexInput v  )
        	{
        		GraphVertexOutput o = (GraphVertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
            	UNITY_TRANSFER_INSTANCE_ID(v, o);
        		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float temp_output_1255_0 = ( _Time.y * _WaveSpeed );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float4 appendResult1245 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float4 WORLD_SPACE_UVs1272 = appendResult1245;
				float4 WaveTileUV1269 = ( ( WORLD_SPACE_UVs1272 * float4( _WaveNoiseShape, 0.0 , 0.0 ) ) * _WaveTileUV );
				float2 panner1250 = ( temp_output_1255_0 * _WaveDirection + WaveTileUV1269.xy);
				float simplePerlin2D1260 = snoise( panner1250 );
				float2 panner1259 = ( temp_output_1255_0 * _WaveDirection + ( WaveTileUV1269 * float4( 0.1,0.1,0,0 ) ).xy);
				float simplePerlin2D1261 = snoise( panner1259 );
				float temp_output_1262_0 = ( simplePerlin2D1260 + simplePerlin2D1261 );
				float3 MOD_WAVE_021271 = ( ( _WaveUp * _WaveHeight ) * temp_output_1262_0 );
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord7 = screenPos;
				
				o.ase_texcoord8 = v.vertex;
				o.ase_texcoord9.xyz = v.ase_texcoord.xyz;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord9.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = MOD_WAVE_021271;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal =  v.ase_normal ;

        		// Vertex shader outputs defined by graph
                float3 lwWNormal = TransformObjectToWorldNormal(v.ase_normal);
				float3 lwWorldPos = TransformObjectToWorld(v.vertex.xyz);
				float3 lwWTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				float3 lwWBinormal = normalize(cross(lwWNormal, lwWTangent) * v.ase_tangent.w);
				o.tSpace0 = float4(lwWTangent.x, lwWBinormal.x, lwWNormal.x, lwWorldPos.x);
				o.tSpace1 = float4(lwWTangent.y, lwWBinormal.y, lwWNormal.y, lwWorldPos.y);
				o.tSpace2 = float4(lwWTangent.z, lwWBinormal.z, lwWNormal.z, lwWorldPos.z);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                
         		// We either sample GI from lightmap or SH.
        	    // Lightmap UV and vertex SH coefficients use the same interpolator ("float2 lightmapUV" for lightmap or "half3 vertexSH" for SH)
                // see DECLARE_LIGHTMAP_OR_SH macro.
        	    // The following funcions initialize the correct variable with correct data
        	    OUTPUT_LIGHTMAP_UV(v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy);
        	    OUTPUT_SH(lwWNormal, o.lightmapUVOrVertexSH.xyz);

        	    half3 vertexLight = VertexLighting(vertexInput.positionWS, lwWNormal);
        	    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
        	    o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
        	    o.clipPos = vertexInput.positionCS;

        	#ifdef _MAIN_LIGHT_SHADOWS
        		o.shadowCoord = GetShadowCoord(vertexInput);
        	#endif
        		return o;
        	}

        	half4 frag (GraphVertexOutput IN , half ase_vface : VFACE ) : SV_Target
            {
            	UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

        		float3 WorldSpaceNormal = normalize(float3(IN.tSpace0.z,IN.tSpace1.z,IN.tSpace2.z));
				float3 WorldSpaceTangent = float3(IN.tSpace0.x,IN.tSpace1.x,IN.tSpace2.x);
				float3 WorldSpaceBiTangent = float3(IN.tSpace0.y,IN.tSpace1.y,IN.tSpace2.y);
				float3 WorldSpacePosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldSpaceViewDirection = SafeNormalize( _WorldSpaceCameraPos.xyz  - WorldSpacePosition );
    
				float4 screenPos = IN.ase_texcoord7;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float clampDepth933 = SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy );
				float z932 = clampDepth933;
				float4 localCalculateObliqueFrustumCorrection890 = CalculateObliqueFrustumCorrection();
				float dotResult891 = dot( float4( IN.ase_texcoord8.xyz , 0.0 ) , localCalculateObliqueFrustumCorrection890 );
				float NEW_OBLIQUE_FRUSTUM_CORRECTION892 = dotResult891;
				float correctionFactor932 = NEW_OBLIQUE_FRUSTUM_CORRECTION892;
				float localCorrectedLinearEyeDepth932 = CorrectedLinearEyeDepth( z932 , correctionFactor932 );
				float3 screenPos896 = screenPos.xyz;
				float localFROM896 = FROM( screenPos896 );
				float temp_output_897_0 = ( localCorrectedLinearEyeDepth932 - localFROM896 );
				float NEW_BEHIND_DEPTH_03900 = saturate( temp_output_897_0 );
				float2 temp_cast_2 = (_OceanFoamSpeed).xx;
				float2 temp_cast_3 = (_OceanFoamTiling).xx;
				float2 uv0735 = IN.ase_texcoord9.xyz * temp_cast_3 + float2( 0,0 );
				float2 _Vector0 = float2(2,4);
				float cos730 = cos( _Vector0.x );
				float sin730 = sin( _Vector0.x );
				float2 rotator730 = mul( uv0735 - float2( 0,0 ) , float2x2( cos730 , -sin730 , sin730 , cos730 )) + float2( 0,0 );
				float2 panner747 = ( 1.0 * _Time.y * temp_cast_2 + rotator730);
				float2 temp_cast_4 = (_OceanFoamSpeed).xx;
				float cos737 = cos( _Vector0.y );
				float sin737 = sin( _Vector0.y );
				float2 rotator737 = mul( uv0735 - float2( 0,0 ) , float2x2( cos737 , -sin737 , sin737 , cos737 )) + float2( 0,0 );
				float2 panner740 = ( 1.0 * _Time.y * temp_cast_4 + rotator737);
				float2 temp_cast_5 = (_OceanFoamSpeed).xx;
				float2 panner733 = ( 1.0 * _Time.y * temp_cast_5 + uv0735);
				float screenDepth741 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth741 = abs( ( screenDepth741 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _OceanFoamDistance ) );
				float4 lerpResult748 = lerp( float4( 0,0,0,0 ) , ( _OceanFoamTint * NEW_BEHIND_DEPTH_03900 * ( ( tex2D( _OceanFoamMap, panner747 ) + tex2D( _OceanFoamMap, panner740 ) + tex2D( _OceanFoamMap, panner733 ) ) / 3.0 ) * saturate( distanceDepth741 ) ) , _OacenFoamStrenght);
				float4 OCAEN_FOAM754 = lerp(float4( 0,0,0,0 ),lerpResult748,_EnableOceanFoam);
				float4 appendResult841 = (float4(WorldSpacePosition.x , WorldSpacePosition.z , 0.0 , 0.0));
				float4 temp_output_845_0 = ( ( appendResult841 / 10.0 ) * _BeachFoamTiling );
				float2 _Vector3 = float2(2,1);
				float cos853 = cos( _Vector3.x );
				float sin853 = sin( _Vector3.x );
				float2 rotator853 = mul( temp_output_845_0.xy - float2( 0,0 ) , float2x2( cos853 , -sin853 , sin853 , cos853 )) + float2( 0,0 );
				float2 panner861 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator853);
				float cos852 = cos( _Vector3.y );
				float sin852 = sin( _Vector3.y );
				float2 rotator852 = mul( ( temp_output_845_0 * ( _BeachFoamTiling * 5.0 ) ).xy - float2( 0,0 ) , float2x2( cos852 , -sin852 , sin852 , cos852 )) + float2( 0,0 );
				float2 panner860 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator852);
				float4 tex2DNode862 = tex2D( _BeachFoamMap, panner860 );
				float4 lerpResult875 = lerp( float4( 0,0,0,0 ) , ( tex2D( _BeachFoamMap, panner861 ) * tex2DNode862 * tex2DNode862.a * NEW_BEHIND_DEPTH_03900 ) , _BeachFoamTint);
				float3 unityObjectToViewPos859 = TransformWorldToView( TransformObjectToWorld( IN.ase_texcoord8.xyz) );
				float NEW_SCREEN_DEPTH899 = localCorrectedLinearEyeDepth932;
				float temp_output_863_0 = ( unityObjectToViewPos859.z + NEW_SCREEN_DEPTH899 );
				float NEW_CLOSENESS949 = saturate( ( 1.0 / distance( _WorldSpaceCameraPos , WorldSpacePosition ) ) );
				float4 lerpResult881 = lerp( abs( ( lerpResult875 - float4( 0,0,0,0 ) ) ) , float4( 0,0,0,0 ) , saturate( ( ( ( temp_output_863_0 - 0.001 ) * NEW_CLOSENESS949 ) / ( ( _foamMax - 0.001 ) * NEW_CLOSENESS949 ) ) ));
				float4 clampResult883 = clamp( lerpResult881 , float4( 0,0,0,0 ) , float4( 0.8602941,0.8602941,0.8602941,0 ) );
				float4 temp_cast_8 = (( 1.0 - _BeachFoamStrenght )).xxxx;
				float4 temp_cast_9 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch1289 = temp_cast_9;
				#else
				float4 staticSwitch1289 = lerp(float4( 0,0,0,0 ),pow( clampResult883 , temp_cast_8 ),_EnableBeachFoam);
				#endif
				float4 BEACH_FOAM886 = staticSwitch1289;
				float screenDepth983 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth983 = saturate( abs( ( screenDepth983 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthDistance ) ) );
				float4 lerpResult996 = lerp( _DepthTint , _WaterTint , saturate( (distanceDepth983*-0.1 + 1.0) ));
				float4 temp_cast_10 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch1000 = temp_cast_10;
				#else
				float4 staticSwitch1000 = lerpResult996;
				#endif
				float4 COLOR_TINT_FINAL1003 = staticSwitch1000;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
				float4 fetchOpaqueVal951 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( ase_grabScreenPosNorm ), 1.0 );
				float4 GRAB_SCREEN_COLOR952 = fetchOpaqueVal951;
				float DEPTH_TINT_A1006 = ( saturate( (distanceDepth983*-5.0 + 5.0) ) * 0.07898042 );
				float NEW_FINAL_OPACITY969 = ( lerp(1.0,IN.ase_color.a,_IgnoreVertexColor) * ( 1.0 - _OpacityOcean ) );
				#ifdef UNITY_PASS_FORWARDADD
				float staticSwitch1023 = 0.0;
				#else
				float staticSwitch1023 = ( 1.0 - ( ( 1.0 - DEPTH_TINT_A1006 ) * NEW_FINAL_OPACITY969 ) );
				#endif
				float4 lerpResult1025 = lerp( ( ( OCAEN_FOAM754 + BEACH_FOAM886 ) + COLOR_TINT_FINAL1003 ) , GRAB_SCREEN_COLOR952 , staticSwitch1023);
				
				float mulTime785 = _Time.y * _NormalMapTimescale;
				float2 temp_cast_12 = (_NormalMapSpeed).xx;
				float2 temp_cast_13 = (_NormalMapTiling).xx;
				float2 uv0779 = IN.ase_texcoord9.xyz.xy * temp_cast_13 + float2( 0,0 );
				float2 _Vector1 = float2(2,4);
				float cos786 = cos( _Vector1.x );
				float sin786 = sin( _Vector1.x );
				float2 rotator786 = mul( uv0779 - float2( 0,0 ) , float2x2( cos786 , -sin786 , sin786 , cos786 )) + float2( 0,0 );
				float2 panner802 = ( mulTime785 * temp_cast_12 + rotator786);
				float2 temp_cast_14 = (_NormalMapSpeed).xx;
				float cos781 = cos( _Vector1.y );
				float sin781 = sin( _Vector1.y );
				float2 rotator781 = mul( uv0779 - float2( 0,0 ) , float2x2( cos781 , -sin781 , sin781 , cos781 )) + float2( 0,0 );
				float2 panner794 = ( mulTime785 * temp_cast_14 + rotator781);
				float2 temp_cast_15 = (_NormalMapSpeed).xx;
				float2 panner795 = ( mulTime785 * temp_cast_15 + uv0779);
				float3 lerpResult800 = lerp( BlendNormal( BlendNormal( UnpackNormalScale( tex2D( _NormalMap, panner802 ), _NormalMapStrength ) , UnpackNormalScale( tex2D( _NormalMap, panner794 ), _NormalMapStrength ) ) , UnpackNormalScale( tex2D( _NormalMap, panner795 ), _NormalMapStrength ) ) , float3( 0,0,0 ) , float4(0,0,0,0.428).rgb);
				float3 tanToWorld0 = float3( WorldSpaceTangent.x, WorldSpaceBiTangent.x, WorldSpaceNormal.x );
				float3 tanToWorld1 = float3( WorldSpaceTangent.y, WorldSpaceBiTangent.y, WorldSpaceNormal.y );
				float3 tanToWorld2 = float3( WorldSpaceTangent.z, WorldSpaceBiTangent.z, WorldSpaceNormal.z );
				float3 tanNormal796 = lerpResult800;
				float3 worldNormal796 = float3(dot(tanToWorld0,tanNormal796), dot(tanToWorld1,tanNormal796), dot(tanToWorld2,tanNormal796));
				float3 normalizeResult793 = normalize( worldNormal796 );
				float3 clampResult787 = clamp( float3( 0,0,0 ) , normalizeResult793 , float3( 1,1,1 ) );
				float3 NORMAL_MAP797 = clampResult787;
				
				float3 temp_cast_17 = (1.0).xxx;
				float2 uv_NormalMap = IN.ase_texcoord9.xyz.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;
				float3 tanNormal790 = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap ), 1.0f );
				float3 worldNormal790 = float3(dot(tanToWorld0,tanNormal790), dot(tanToWorld1,tanNormal790), dot(tanToWorld2,tanNormal790));
				float3 normalizeResult789 = normalize( worldNormal790 );
				float3 NORMAL_MAP_SAMPLE788 = normalizeResult789;
				float3 tanNormal1148 = NORMAL_MAP_SAMPLE788;
				float temp_output_1129_0 = (COLOR_TINT_FINAL1003).a;
				half3 reflectVector1148 = reflect( -WorldSpaceViewDirection, float3(dot(tanToWorld0,tanNormal1148), dot(tanToWorld1,tanNormal1148), dot(tanToWorld2,tanNormal1148)) );
				float3 indirectSpecular1148 = GlossyEnvironmentReflection( reflectVector1148, 1.0 - temp_output_1129_0, _Occlusion );
				float3 lerpResult1156 = lerp( temp_cast_17 , indirectSpecular1148 , ( 1.0 - _LightIndirectStrengthSpecular ));
				float SPECULAR_TINT_A1_51143 = pow( temp_output_1129_0 , 1.5 );
				float4 INDIRECT_SPECULAR1162 = ( float4( lerpResult1156 , 0.0 ) * SPECULAR_TINT_A1_51143 * float4( (COLOR_TINT_FINAL1003).rgb , 0.0 ) * _MainLightColor );
				float3 ase_worldReflection = reflect(-WorldSpaceViewDirection, WorldSpaceNormal);
				float dotResult1099 = dot( NORMAL_MAP_SAMPLE788 , _MainLightPosition.xyz );
				float NdotL1103 = dotResult1099;
				float fresnelNdotV1198 = dot( WorldSpaceNormal, WorldSpaceViewDirection );
				float fresnelNode1198 = ( 0.0 + _ReflectionFrenelScale * pow( 1.0 - fresnelNdotV1198, _ReflectionFresnelPower ) );
				float4 lerpResult1203 = lerp( float4( 0,0,0,0 ) , texCUBE( _ReflectionCubeMap, ( ase_worldReflection + ( NdotL1103 * _ReflectionWobble ) ) ) , ( fresnelNode1198 * _ReflectionIntensity ));
				float4 temp_cast_20 = (0.0).xxxx;
				#ifdef UNITY_PASS_FORWARDADD
				float4 staticSwitch1206 = temp_cast_20;
				#else
				float4 staticSwitch1206 = lerp(float4( 0,0,0,0 ),lerpResult1203,_EnableReflection);
				#endif
				float4 REFLECTION_CUBEMAP1232 = staticSwitch1206;
				float3 temp_cast_21 = (NdotL1103).xxx;
				float3 tanNormal1181 = temp_cast_21;
				float3 worldNormal1181 = float3(dot(tanToWorld0,tanNormal1181), dot(tanToWorld1,tanNormal1181), dot(tanToWorld2,tanNormal1181));
				float3 normalizeResult1286 = normalize( ( _MainLightPosition.xyz + WorldSpaceViewDirection ) );
				float dotResult1183 = dot( worldNormal1181 , normalizeResult1286 );
				float fresnelNdotV1297 = dot( WorldSpaceNormal, WorldSpaceViewDirection );
				float fresnelNode1297 = ( 0.0 + _ShadowFrenelScale * pow( 1.0 - fresnelNdotV1297, _ShadowFresnelPower ) );
				float lerpResult1300 = lerp( dotResult1183 , 0.0 , ( fresnelNode1297 * ( 1.0 - _ShadowIntensity ) ));
				float _FakeShadow1179 = ( 1.0 - saturate( (lerpResult1300*_ShadowDepth + _ShadowDepth) ) );
				
				float perceptualSmoothness1048 = _Smoothness;
				float3 geometricNormalWS1048 = WorldSpaceNormal;
				float screenSpaceVariance1048 = _SmoothnessVariance;
				float threshold1048 = _SmoothnessThreshold;
				float localGetGeometricNormalVariance1048 = GetGeometricNormalVariance( perceptualSmoothness1048 , geometricNormalWS1048 , screenSpaceVariance1048 , threshold1048 );
				float switchResult1049 = (((ase_vface>0)?(localGetGeometricNormalVariance1048):(0.0)));
				float NEW_SMOOTHNESS1050 = switchResult1049;
				
				
		        float3 Albedo = lerpResult1025.rgb;
				float3 Normal = NORMAL_MAP797;
				float3 Emission = ( INDIRECT_SPECULAR1162 + REFLECTION_CUBEMAP1232 + _FakeShadow1179 ).rgb;
				float3 Specular = float3(0.5, 0.5, 0.5);
				float Metallic = pow( max( NdotL1103 , 0.0 ) , exp2( _Metallic ) );
				float Smoothness = NEW_SMOOTHNESS1050;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0;

        		InputData inputData;
        		inputData.positionWS = WorldSpacePosition;

        #ifdef _NORMALMAP
        	    inputData.normalWS = normalize(TransformTangentToWorld(Normal, half3x3(WorldSpaceTangent, WorldSpaceBiTangent, WorldSpaceNormal)));
        #else
            #if !SHADER_HINT_NICE_QUALITY
                inputData.normalWS = WorldSpaceNormal;
            #else
        	    inputData.normalWS = normalize(WorldSpaceNormal);
            #endif
        #endif

        #if !SHADER_HINT_NICE_QUALITY
        	    // viewDirection should be normalized here, but we avoid doing it as it's close enough and we save some ALU.
        	    inputData.viewDirectionWS = WorldSpaceViewDirection;
        #else
        	    inputData.viewDirectionWS = normalize(WorldSpaceViewDirection);
        #endif

        	    inputData.shadowCoord = IN.shadowCoord;

        	    inputData.fogCoord = IN.fogFactorAndVertexLight.x;
        	    inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
        	    inputData.bakedGI = SAMPLE_GI(IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, inputData.normalWS);

        		half4 color = LightweightFragmentPBR(
        			inputData, 
        			Albedo, 
        			Metallic, 
        			Specular, 
        			Smoothness, 
        			Occlusion, 
        			Emission, 
        			Alpha);

			#ifdef TERRAIN_SPLAT_ADDPASS
				color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
			#else
				color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
			#endif

        #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif

		#if ASE_LW_FINAL_COLOR_ALPHA_MULTIPLY
				color.rgb *= color.a;
		#endif
		
		#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition (IN.clipPos.xyz, unity_LODFade.x);
		#endif
        		return color;
            }

        	ENDHLSL
        }

		
        Pass
        {
			
        	Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

            HLSLPROGRAM
            #define _RECEIVE_SHADOWS_OFF 1
            #define ASE_SRP_VERSION 60901

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
                float3 ase_normal : NORMAL;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			CBUFFER_START( UnityPerMaterial )
			int _CullMode;
			float3 _WaveUp;
			float _WaveHeight;
			float _WaveSpeed;
			float2 _WaveDirection;
			float2 _WaveNoiseShape;
			float _WaveTileUV;
			float _EnableOceanFoam;
			float4 _OceanFoamTint;
			float _OceanFoamSpeed;
			float _OceanFoamTiling;
			float _OceanFoamDistance;
			float _OacenFoamStrenght;
			float _EnableBeachFoam;
			float _BeachFoamSpeed;
			float _BeachFoamTiling;
			float4 _BeachFoamTint;
			float _foamMax;
			float _BeachFoamStrenght;
			float4 _DepthTint;
			float4 _WaterTint;
			float _DepthDistance;
			float _IgnoreVertexColor;
			float _OpacityOcean;
			float _NormalMapStrength;
			float _NormalMapTimescale;
			float _NormalMapSpeed;
			float _NormalMapTiling;
			float4 _NormalMap_ST;
			float _Occlusion;
			float _LightIndirectStrengthSpecular;
			float _EnableReflection;
			float _ReflectionWobble;
			float _ReflectionFrenelScale;
			float _ReflectionFresnelPower;
			float _ReflectionIntensity;
			float _ShadowFrenelScale;
			float _ShadowFresnelPower;
			float _ShadowIntensity;
			float _ShadowDepth;
			float _Metallic;
			float _Smoothness;
			float _SmoothnessVariance;
			float _SmoothnessThreshold;
			CBUFFER_END

        	struct VertexOutput
        	{
        	    float4 clipPos      : SV_POSITION;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
        	};

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
			

            // x: global clip space bias, y: normal world space bias
            float3 _LightDirection;

            VertexOutput ShadowPassVertex(GraphVertexInput v )
        	{
        	    VertexOutput o;
        	    UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO (o);

				float temp_output_1255_0 = ( _Time.y * _WaveSpeed );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float4 appendResult1245 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float4 WORLD_SPACE_UVs1272 = appendResult1245;
				float4 WaveTileUV1269 = ( ( WORLD_SPACE_UVs1272 * float4( _WaveNoiseShape, 0.0 , 0.0 ) ) * _WaveTileUV );
				float2 panner1250 = ( temp_output_1255_0 * _WaveDirection + WaveTileUV1269.xy);
				float simplePerlin2D1260 = snoise( panner1250 );
				float2 panner1259 = ( temp_output_1255_0 * _WaveDirection + ( WaveTileUV1269 * float4( 0.1,0.1,0,0 ) ).xy);
				float simplePerlin2D1261 = snoise( panner1259 );
				float temp_output_1262_0 = ( simplePerlin2D1260 + simplePerlin2D1261 );
				float3 MOD_WAVE_021271 = ( ( _WaveUp * _WaveHeight ) * temp_output_1262_0 );
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = MOD_WAVE_021271;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;

        	    float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

                float invNdotL = 1.0 - saturate(dot(_LightDirection, normalWS));
                float scale = invNdotL * _ShadowBias.y;

                // normal bias is negative since we want to apply an inset normal offset
                positionWS = _LightDirection * _ShadowBias.xxx + positionWS;
				positionWS = normalWS * scale.xxx + positionWS;
                float4 clipPos = TransformWorldToHClip(positionWS);

                // _ShadowBias.x sign depens on if platform has reversed z buffer
                //clipPos.z += _ShadowBias.x;

        	#if UNITY_REVERSED_Z
        	    clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
        	#else
        	    clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
        	#endif
                o.clipPos = clipPos;

        	    return o;
        	}

            half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

               

				float Alpha = 1;
				float AlphaClipThreshold = AlphaClipThreshold;

         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif

		#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition (IN.clipPos.xyz, unity_LODFade.x);
		#endif
				return 0;
            }

            ENDHLSL
        }

		
        Pass
        {
			
        	Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }

            ZWrite On
			ColorMask 0

            HLSLPROGRAM
            #define _RECEIVE_SHADOWS_OFF 1
            #define ASE_SRP_VERSION 60901

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            

			CBUFFER_START( UnityPerMaterial )
			int _CullMode;
			float3 _WaveUp;
			float _WaveHeight;
			float _WaveSpeed;
			float2 _WaveDirection;
			float2 _WaveNoiseShape;
			float _WaveTileUV;
			float _EnableOceanFoam;
			float4 _OceanFoamTint;
			float _OceanFoamSpeed;
			float _OceanFoamTiling;
			float _OceanFoamDistance;
			float _OacenFoamStrenght;
			float _EnableBeachFoam;
			float _BeachFoamSpeed;
			float _BeachFoamTiling;
			float4 _BeachFoamTint;
			float _foamMax;
			float _BeachFoamStrenght;
			float4 _DepthTint;
			float4 _WaterTint;
			float _DepthDistance;
			float _IgnoreVertexColor;
			float _OpacityOcean;
			float _NormalMapStrength;
			float _NormalMapTimescale;
			float _NormalMapSpeed;
			float _NormalMapTiling;
			float4 _NormalMap_ST;
			float _Occlusion;
			float _LightIndirectStrengthSpecular;
			float _EnableReflection;
			float _ReflectionWobble;
			float _ReflectionFrenelScale;
			float _ReflectionFresnelPower;
			float _ReflectionIntensity;
			float _ShadowFrenelScale;
			float _ShadowFresnelPower;
			float _ShadowIntensity;
			float _ShadowDepth;
			float _Metallic;
			float _Smoothness;
			float _SmoothnessVariance;
			float _SmoothnessThreshold;
			CBUFFER_END

            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        	struct VertexOutput
        	{
        	    float4 clipPos      : SV_POSITION;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
        	};

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
			
           

            VertexOutput vert(GraphVertexInput v  )
            {
                VertexOutput o = (VertexOutput)0;
        	    UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float temp_output_1255_0 = ( _Time.y * _WaveSpeed );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float4 appendResult1245 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float4 WORLD_SPACE_UVs1272 = appendResult1245;
				float4 WaveTileUV1269 = ( ( WORLD_SPACE_UVs1272 * float4( _WaveNoiseShape, 0.0 , 0.0 ) ) * _WaveTileUV );
				float2 panner1250 = ( temp_output_1255_0 * _WaveDirection + WaveTileUV1269.xy);
				float simplePerlin2D1260 = snoise( panner1250 );
				float2 panner1259 = ( temp_output_1255_0 * _WaveDirection + ( WaveTileUV1269 * float4( 0.1,0.1,0,0 ) ).xy);
				float simplePerlin2D1261 = snoise( panner1259 );
				float temp_output_1262_0 = ( simplePerlin2D1260 + simplePerlin2D1261 );
				float3 MOD_WAVE_021271 = ( ( _WaveUp * _WaveHeight ) * temp_output_1262_0 );
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = MOD_WAVE_021271;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;

        	    o.clipPos = TransformObjectToHClip(v.vertex.xyz);
        	    return o;
            }

            half4 frag(VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);

				

				float Alpha = 1;
				float AlphaClipThreshold = AlphaClipThreshold;

         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif
		#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition (IN.clipPos.xyz, unity_LODFade.x);
		#endif
				return 0;
            }
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
		
        Pass
        {
			
        	Name "Meta"
            Tags { "LightMode"="Meta" }

            Cull Off

            HLSLPROGRAM
            #define _RECEIVE_SHADOWS_OFF 1
            #define ASE_SRP_VERSION 60901
            #define REQUIRE_DEPTH_TEXTURE 1
            #define REQUIRE_OPAQUE_TEXTURE 1

            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag

			
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/MetaInput.hlsl"
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            

			uniform float4 _CameraDepthTexture_TexelSize;
			sampler2D _OceanFoamMap;
			sampler2D _BeachFoamMap;
			sampler2D _NormalMap;
			samplerCUBE _ReflectionCubeMap;
			CBUFFER_START( UnityPerMaterial )
			int _CullMode;
			float3 _WaveUp;
			float _WaveHeight;
			float _WaveSpeed;
			float2 _WaveDirection;
			float2 _WaveNoiseShape;
			float _WaveTileUV;
			float _EnableOceanFoam;
			float4 _OceanFoamTint;
			float _OceanFoamSpeed;
			float _OceanFoamTiling;
			float _OceanFoamDistance;
			float _OacenFoamStrenght;
			float _EnableBeachFoam;
			float _BeachFoamSpeed;
			float _BeachFoamTiling;
			float4 _BeachFoamTint;
			float _foamMax;
			float _BeachFoamStrenght;
			float4 _DepthTint;
			float4 _WaterTint;
			float _DepthDistance;
			float _IgnoreVertexColor;
			float _OpacityOcean;
			float _NormalMapStrength;
			float _NormalMapTimescale;
			float _NormalMapSpeed;
			float _NormalMapTiling;
			float4 _NormalMap_ST;
			float _Occlusion;
			float _LightIndirectStrengthSpecular;
			float _EnableReflection;
			float _ReflectionWobble;
			float _ReflectionFrenelScale;
			float _ReflectionFresnelPower;
			float _ReflectionIntensity;
			float _ShadowFrenelScale;
			float _ShadowFresnelPower;
			float _ShadowIntensity;
			float _ShadowDepth;
			float _Metallic;
			float _Smoothness;
			float _SmoothnessVariance;
			float _SmoothnessThreshold;
			CBUFFER_END

            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature EDITOR_VISUALIZATION


            struct GraphVertexInput
            {
                float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        	struct VertexOutput
        	{
        	    float4 clipPos      : SV_POSITION;
                float4 ase_texcoord : TEXCOORD0;
                float4 ase_texcoord1 : TEXCOORD1;
                float4 ase_texcoord2 : TEXCOORD2;
                float4 ase_texcoord3 : TEXCOORD3;
                float4 ase_color : COLOR;
                float4 ase_texcoord4 : TEXCOORD4;
                float4 ase_texcoord5 : TEXCOORD5;
                float4 ase_texcoord6 : TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
        	};

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
			

            VertexOutput vert(GraphVertexInput v  )
            {
                VertexOutput o = (VertexOutput)0;
        	    UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float temp_output_1255_0 = ( _Time.y * _WaveSpeed );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float4 appendResult1245 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
				float4 WORLD_SPACE_UVs1272 = appendResult1245;
				float4 WaveTileUV1269 = ( ( WORLD_SPACE_UVs1272 * float4( _WaveNoiseShape, 0.0 , 0.0 ) ) * _WaveTileUV );
				float2 panner1250 = ( temp_output_1255_0 * _WaveDirection + WaveTileUV1269.xy);
				float simplePerlin2D1260 = snoise( panner1250 );
				float2 panner1259 = ( temp_output_1255_0 * _WaveDirection + ( WaveTileUV1269 * float4( 0.1,0.1,0,0 ) ).xy);
				float simplePerlin2D1261 = snoise( panner1259 );
				float temp_output_1262_0 = ( simplePerlin2D1260 + simplePerlin2D1261 );
				float3 MOD_WAVE_021271 = ( ( _WaveUp * _WaveHeight ) * temp_output_1262_0 );
				
				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				o.ase_texcoord3.xyz = ase_worldPos;
				
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord4.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord5.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord6.xyz = ase_worldBitangent;
				
				o.ase_texcoord1 = v.vertex;
				o.ase_texcoord2.xyz = v.ase_texcoord.xyz;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;
				o.ase_texcoord6.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = v.vertex.xyz;
				#else
				float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = MOD_WAVE_021271;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal =  v.ase_normal ;
#if !defined( ASE_SRP_VERSION ) || ASE_SRP_VERSION  > 51300				
                o.clipPos = MetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST);
#else
				o.clipPos = MetaVertexPosition (v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST);
#endif
        	    return o;
            }

            half4 frag(VertexOutput IN  ) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(IN);

           		float4 screenPos = IN.ase_texcoord;
           		float4 ase_screenPosNorm = screenPos / screenPos.w;
           		ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
           		float clampDepth933 = SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy );
           		float z932 = clampDepth933;
           		float4 localCalculateObliqueFrustumCorrection890 = CalculateObliqueFrustumCorrection();
           		float dotResult891 = dot( float4( IN.ase_texcoord1.xyz , 0.0 ) , localCalculateObliqueFrustumCorrection890 );
           		float NEW_OBLIQUE_FRUSTUM_CORRECTION892 = dotResult891;
           		float correctionFactor932 = NEW_OBLIQUE_FRUSTUM_CORRECTION892;
           		float localCorrectedLinearEyeDepth932 = CorrectedLinearEyeDepth( z932 , correctionFactor932 );
           		float3 screenPos896 = screenPos.xyz;
           		float localFROM896 = FROM( screenPos896 );
           		float temp_output_897_0 = ( localCorrectedLinearEyeDepth932 - localFROM896 );
           		float NEW_BEHIND_DEPTH_03900 = saturate( temp_output_897_0 );
           		float2 temp_cast_2 = (_OceanFoamSpeed).xx;
           		float2 temp_cast_3 = (_OceanFoamTiling).xx;
           		float2 uv0735 = IN.ase_texcoord2.xyz * temp_cast_3 + float2( 0,0 );
           		float2 _Vector0 = float2(2,4);
           		float cos730 = cos( _Vector0.x );
           		float sin730 = sin( _Vector0.x );
           		float2 rotator730 = mul( uv0735 - float2( 0,0 ) , float2x2( cos730 , -sin730 , sin730 , cos730 )) + float2( 0,0 );
           		float2 panner747 = ( 1.0 * _Time.y * temp_cast_2 + rotator730);
           		float2 temp_cast_4 = (_OceanFoamSpeed).xx;
           		float cos737 = cos( _Vector0.y );
           		float sin737 = sin( _Vector0.y );
           		float2 rotator737 = mul( uv0735 - float2( 0,0 ) , float2x2( cos737 , -sin737 , sin737 , cos737 )) + float2( 0,0 );
           		float2 panner740 = ( 1.0 * _Time.y * temp_cast_4 + rotator737);
           		float2 temp_cast_5 = (_OceanFoamSpeed).xx;
           		float2 panner733 = ( 1.0 * _Time.y * temp_cast_5 + uv0735);
           		float screenDepth741 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
           		float distanceDepth741 = abs( ( screenDepth741 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _OceanFoamDistance ) );
           		float4 lerpResult748 = lerp( float4( 0,0,0,0 ) , ( _OceanFoamTint * NEW_BEHIND_DEPTH_03900 * ( ( tex2D( _OceanFoamMap, panner747 ) + tex2D( _OceanFoamMap, panner740 ) + tex2D( _OceanFoamMap, panner733 ) ) / 3.0 ) * saturate( distanceDepth741 ) ) , _OacenFoamStrenght);
           		float4 OCAEN_FOAM754 = lerp(float4( 0,0,0,0 ),lerpResult748,_EnableOceanFoam);
           		float3 ase_worldPos = IN.ase_texcoord3.xyz;
           		float4 appendResult841 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
           		float4 temp_output_845_0 = ( ( appendResult841 / 10.0 ) * _BeachFoamTiling );
           		float2 _Vector3 = float2(2,1);
           		float cos853 = cos( _Vector3.x );
           		float sin853 = sin( _Vector3.x );
           		float2 rotator853 = mul( temp_output_845_0.xy - float2( 0,0 ) , float2x2( cos853 , -sin853 , sin853 , cos853 )) + float2( 0,0 );
           		float2 panner861 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator853);
           		float cos852 = cos( _Vector3.y );
           		float sin852 = sin( _Vector3.y );
           		float2 rotator852 = mul( ( temp_output_845_0 * ( _BeachFoamTiling * 5.0 ) ).xy - float2( 0,0 ) , float2x2( cos852 , -sin852 , sin852 , cos852 )) + float2( 0,0 );
           		float2 panner860 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _BeachFoamSpeed ) + rotator852);
           		float4 tex2DNode862 = tex2D( _BeachFoamMap, panner860 );
           		float4 lerpResult875 = lerp( float4( 0,0,0,0 ) , ( tex2D( _BeachFoamMap, panner861 ) * tex2DNode862 * tex2DNode862.a * NEW_BEHIND_DEPTH_03900 ) , _BeachFoamTint);
           		float3 unityObjectToViewPos859 = TransformWorldToView( TransformObjectToWorld( IN.ase_texcoord1.xyz) );
           		float NEW_SCREEN_DEPTH899 = localCorrectedLinearEyeDepth932;
           		float temp_output_863_0 = ( unityObjectToViewPos859.z + NEW_SCREEN_DEPTH899 );
           		float NEW_CLOSENESS949 = saturate( ( 1.0 / distance( _WorldSpaceCameraPos , ase_worldPos ) ) );
           		float4 lerpResult881 = lerp( abs( ( lerpResult875 - float4( 0,0,0,0 ) ) ) , float4( 0,0,0,0 ) , saturate( ( ( ( temp_output_863_0 - 0.001 ) * NEW_CLOSENESS949 ) / ( ( _foamMax - 0.001 ) * NEW_CLOSENESS949 ) ) ));
           		float4 clampResult883 = clamp( lerpResult881 , float4( 0,0,0,0 ) , float4( 0.8602941,0.8602941,0.8602941,0 ) );
           		float4 temp_cast_8 = (( 1.0 - _BeachFoamStrenght )).xxxx;
           		float4 temp_cast_9 = (0.0).xxxx;
           		#ifdef UNITY_PASS_FORWARDADD
           		float4 staticSwitch1289 = temp_cast_9;
           		#else
           		float4 staticSwitch1289 = lerp(float4( 0,0,0,0 ),pow( clampResult883 , temp_cast_8 ),_EnableBeachFoam);
           		#endif
           		float4 BEACH_FOAM886 = staticSwitch1289;
           		float screenDepth983 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
           		float distanceDepth983 = saturate( abs( ( screenDepth983 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _DepthDistance ) ) );
           		float4 lerpResult996 = lerp( _DepthTint , _WaterTint , saturate( (distanceDepth983*-0.1 + 1.0) ));
           		float4 temp_cast_10 = (0.0).xxxx;
           		#ifdef UNITY_PASS_FORWARDADD
           		float4 staticSwitch1000 = temp_cast_10;
           		#else
           		float4 staticSwitch1000 = lerpResult996;
           		#endif
           		float4 COLOR_TINT_FINAL1003 = staticSwitch1000;
           		float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
           		float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
           		float4 fetchOpaqueVal951 = float4( SHADERGRAPH_SAMPLE_SCENE_COLOR( ase_grabScreenPosNorm ), 1.0 );
           		float4 GRAB_SCREEN_COLOR952 = fetchOpaqueVal951;
           		float DEPTH_TINT_A1006 = ( saturate( (distanceDepth983*-5.0 + 5.0) ) * 0.07898042 );
           		float NEW_FINAL_OPACITY969 = ( lerp(1.0,IN.ase_color.a,_IgnoreVertexColor) * ( 1.0 - _OpacityOcean ) );
           		#ifdef UNITY_PASS_FORWARDADD
           		float staticSwitch1023 = 0.0;
           		#else
           		float staticSwitch1023 = ( 1.0 - ( ( 1.0 - DEPTH_TINT_A1006 ) * NEW_FINAL_OPACITY969 ) );
           		#endif
           		float4 lerpResult1025 = lerp( ( ( OCAEN_FOAM754 + BEACH_FOAM886 ) + COLOR_TINT_FINAL1003 ) , GRAB_SCREEN_COLOR952 , staticSwitch1023);
           		
           		float3 temp_cast_12 = (1.0).xxx;
           		float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - ase_worldPos );
           		ase_worldViewDir = normalize(ase_worldViewDir);
           		float2 uv_NormalMap = IN.ase_texcoord2.xyz.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;
           		float3 ase_worldTangent = IN.ase_texcoord4.xyz;
           		float3 ase_worldNormal = IN.ase_texcoord5.xyz;
           		float3 ase_worldBitangent = IN.ase_texcoord6.xyz;
           		float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
           		float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
           		float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
           		float3 tanNormal790 = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap ), 1.0f );
           		float3 worldNormal790 = float3(dot(tanToWorld0,tanNormal790), dot(tanToWorld1,tanNormal790), dot(tanToWorld2,tanNormal790));
           		float3 normalizeResult789 = normalize( worldNormal790 );
           		float3 NORMAL_MAP_SAMPLE788 = normalizeResult789;
           		float3 tanNormal1148 = NORMAL_MAP_SAMPLE788;
           		float temp_output_1129_0 = (COLOR_TINT_FINAL1003).a;
           		half3 reflectVector1148 = reflect( -ase_worldViewDir, float3(dot(tanToWorld0,tanNormal1148), dot(tanToWorld1,tanNormal1148), dot(tanToWorld2,tanNormal1148)) );
           		float3 indirectSpecular1148 = GlossyEnvironmentReflection( reflectVector1148, 1.0 - temp_output_1129_0, _Occlusion );
           		float3 lerpResult1156 = lerp( temp_cast_12 , indirectSpecular1148 , ( 1.0 - _LightIndirectStrengthSpecular ));
           		float SPECULAR_TINT_A1_51143 = pow( temp_output_1129_0 , 1.5 );
           		float4 INDIRECT_SPECULAR1162 = ( float4( lerpResult1156 , 0.0 ) * SPECULAR_TINT_A1_51143 * float4( (COLOR_TINT_FINAL1003).rgb , 0.0 ) * _MainLightColor );
           		float3 ase_worldReflection = reflect(-ase_worldViewDir, ase_worldNormal);
           		float dotResult1099 = dot( NORMAL_MAP_SAMPLE788 , _MainLightPosition.xyz );
           		float NdotL1103 = dotResult1099;
           		float fresnelNdotV1198 = dot( ase_worldNormal, ase_worldViewDir );
           		float fresnelNode1198 = ( 0.0 + _ReflectionFrenelScale * pow( 1.0 - fresnelNdotV1198, _ReflectionFresnelPower ) );
           		float4 lerpResult1203 = lerp( float4( 0,0,0,0 ) , texCUBE( _ReflectionCubeMap, ( ase_worldReflection + ( NdotL1103 * _ReflectionWobble ) ) ) , ( fresnelNode1198 * _ReflectionIntensity ));
           		float4 temp_cast_15 = (0.0).xxxx;
           		#ifdef UNITY_PASS_FORWARDADD
           		float4 staticSwitch1206 = temp_cast_15;
           		#else
           		float4 staticSwitch1206 = lerp(float4( 0,0,0,0 ),lerpResult1203,_EnableReflection);
           		#endif
           		float4 REFLECTION_CUBEMAP1232 = staticSwitch1206;
           		float3 temp_cast_16 = (NdotL1103).xxx;
           		float3 tanNormal1181 = temp_cast_16;
           		float3 worldNormal1181 = float3(dot(tanToWorld0,tanNormal1181), dot(tanToWorld1,tanNormal1181), dot(tanToWorld2,tanNormal1181));
           		float3 normalizeResult1286 = normalize( ( _MainLightPosition.xyz + ase_worldViewDir ) );
           		float dotResult1183 = dot( worldNormal1181 , normalizeResult1286 );
           		float fresnelNdotV1297 = dot( ase_worldNormal, ase_worldViewDir );
           		float fresnelNode1297 = ( 0.0 + _ShadowFrenelScale * pow( 1.0 - fresnelNdotV1297, _ShadowFresnelPower ) );
           		float lerpResult1300 = lerp( dotResult1183 , 0.0 , ( fresnelNode1297 * ( 1.0 - _ShadowIntensity ) ));
           		float _FakeShadow1179 = ( 1.0 - saturate( (lerpResult1300*_ShadowDepth + _ShadowDepth) ) );
           		
				
		        float3 Albedo = lerpResult1025.rgb;
				float3 Emission = ( INDIRECT_SPECULAR1162 + REFLECTION_CUBEMAP1232 + _FakeShadow1179 ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0;

         #if _AlphaClip
        		clip(Alpha - AlphaClipThreshold);
        #endif

                MetaInput metaInput = (MetaInput)0;
                metaInput.Albedo = Albedo;
                metaInput.Emission = Emission;
                
                return MetaFragment(metaInput);
            }
            ENDHLSL
        }
		
    }
    Fallback "Hidden/InternalErrorShader"
	CustomEditor "ASEMaterialInspector"
	
}
/*ASEBEGIN
Version=17101
2139;32.5;1664;987;8989.02;10367.32;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;888;-6214.383,-13638.87;Inherit;False;1181.449;291.5627;Oblique Frustum Correction Factor;5;892;909;891;889;890;OBLIQUE_FRUSTUM_CORRECTION;0,1,0.1391485,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;838;-8871.104,-11522.64;Inherit;False;3849.003;1100.833;Comment;37;886;1289;1290;885;884;883;882;880;881;878;877;875;872;868;865;864;862;858;861;860;856;854;853;852;849;851;847;850;846;848;844;845;842;843;840;841;839;BEACH_FOAM;1,0.417028,0.3019608,1;0;0
Node;AmplifyShaderEditor.CustomExpressionNode;890;-6194.824,-13430.15;Float;False;float x1 = -UNITY_MATRIX_P._31 / (UNITY_MATRIX_P._11 * UNITY_MATRIX_P._34)@$float x2 = -UNITY_MATRIX_P._32 / (UNITY_MATRIX_P._22 * UNITY_MATRIX_P._34)@$return float4(x1, x2, 0, UNITY_MATRIX_P._33 / UNITY_MATRIX_P._34 + x1 * UNITY_MATRIX_P._13 + x2 * UNITY_MATRIX_P._23)@;4;False;0;CalculateObliqueFrustumCorrection;False;True;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;889;-6188.438,-13580.42;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;891;-5863.721,-13579.2;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;839;-8835.174,-11472.74;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;892;-5433.886,-13580.3;Float;False;NEW_OBLIQUE_FRUSTUM_CORRECTION;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;840;-8650.675,-11313.62;Float;False;Constant;_Float3;Float 3;63;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;893;-6690.136,-12565.64;Inherit;False;1658.599;348.3577;Clip BG from Grab;12;900;934;896;928;898;911;897;899;894;922;932;933;BEHIND_DEPTH_03;0,1,0.1568628,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;841;-8633.52,-11470.93;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;934;-6127.677,-12383.05;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;843;-8482.157,-11472.11;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;894;-6574.016,-12355.44;Inherit;False;892;NEW_OBLIQUE_FRUSTUM_CORRECTION;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;895;-8098.543,-12178.46;Inherit;False;1024.421;476.6597;;6;944;945;946;947;949;948;CLOSENESS;0,1,0.1568628,1;0;0
Node;AmplifyShaderEditor.ScreenDepthNode;933;-6692.968,-12520.92;Inherit;False;1;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;842;-8641.921,-10990.79;Float;False;Property;_BeachFoamTiling;Beach Foam Tiling;43;0;Create;True;0;0;False;1;;100;0.632;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;945;-8009.346,-11973.61;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CustomExpressionNode;896;-5873.278,-12323;Float;False;return UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z)@;1;False;1;True;screenPos;FLOAT3;0,0,0;In;;Float;False;FROM;False;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;932;-6169.241,-12516.74;Float;False;return 1.f / (z / UNITY_MATRIX_P._34 + correctionFactor)@;1;False;2;True;z;FLOAT;0;In;;Float;False;True;correctionFactor;FLOAT;0;In;;Float;False;CorrectedLinearEyeDepth;False;True;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;845;-8345.261,-11471.65;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;944;-8078.039,-12122.55;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;844;-8330.836,-10988.23;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;851;-7958.043,-11140.71;Float;False;Constant;_BeachFoamPainDirection002;Beach Foam Pain Direction 002;46;0;Create;True;0;0;False;0;1,0;-3,-5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;848;-7734.294,-10832.55;Inherit;False;1369.163;367.1912;;14;887;879;876;874;873;871;870;869;867;866;863;857;855;859;FOAM_FADE;0,1,0.05740881,1;0;0
Node;AmplifyShaderEditor.Vector2Node;846;-8170.269,-11274.82;Float;False;Constant;_Vector3;Vector 3;53;0;Create;True;0;0;False;0;2,1;1,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;847;-8156.377,-11010.49;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;775;-8783.496,-9162.553;Inherit;False;3103.881;801.673;Comment;27;802;801;800;799;798;797;796;795;794;793;792;791;790;789;788;787;786;785;784;783;782;781;780;779;778;777;776;NORMAL_MAP;0.7379308,0,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;850;-7938.634,-11222.73;Float;False;Property;_BeachFoamSpeed;Beach Foam Speed;45;0;Create;True;0;0;False;0;0;0.061;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;849;-7954.587,-11359.25;Float;False;Constant;_BeachFoamPainDirection001;Beach Foam Pain Direction 001;45;0;Create;True;0;0;False;0;1,0;-4,-4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;897;-5630.238,-12513.52;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;946;-7775.01,-12028.11;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;791;-6870.172,-9070.283;Inherit;True;Property;_NormalMap;Normal Map;22;1;[Normal];Create;True;0;0;False;1;Header (NORMAL MAP);None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;898;-5474.746,-12514.29;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;899;-5913.919,-12424.31;Float;False;NEW_SCREEN_DEPTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;853;-7950.481,-11476.18;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;854;-7629.227,-11354.55;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;856;-7629.381,-11129.93;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;855;-7720.27,-10780.98;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;982;-4975.099,-13623.72;Inherit;False;2062.545;776.8127;hex code 4E83A9FF;19;1295;997;998;1002;1001;983;1004;1005;1006;987;986;991;996;993;994;990;999;1000;1003;COLOR_TINT;0,0.3020356,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;947;-7633.72,-12037.1;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;852;-7948.893,-11011.73;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;1243;-8806.173,-8190.28;Inherit;False;769.783;224.9718;For worldSpace tilling;3;1272;1245;1244;WORLD_SPACE_UVs;0.9351528,0.08235294,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;861;-7472.972,-11478.73;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldNormalVector;790;-6566.172,-9054.283;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;860;-7459.551,-11012.2;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;900;-5315.949,-12519.41;Float;False;NEW_BEHIND_DEPTH_03;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;858;-7475.224,-11256.81;Float;True;Property;_BeachFoamMap;Beach Foam Map;41;0;Create;True;0;0;False;0;None;None;False;black;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.UnityObjToViewPosHlpNode;859;-7532.073,-10781.61;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;857;-7620.295,-10618.89;Inherit;False;899;NEW_SCREEN_DEPTH;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;948;-7503.538,-12037.35;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;726;-8837.032,-10324.06;Inherit;False;2944.038;874.245;Comment;24;728;754;749;748;753;750;553;739;731;743;742;745;738;744;747;727;740;733;734;737;730;735;736;746;OCAEN_FOAM;1,0.654902,0.3019608,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1295;-4958.601,-13214.67;Inherit;False;Property;_DepthDistance;Depth Distance;2;0;Create;True;0;0;False;0;0;176;0;500;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;862;-7172.941,-11037.02;Inherit;True;Property;_TextureSample0;Texture Sample 0;41;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;863;-7353.786,-10781.3;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1081;-4936.69,-11647.84;Inherit;False;2217.493;1126.181;Comment;4;1083;1162;1085;1306;LIGHTING;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;789;-6358.172,-9054.283;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;865;-7175.84,-11438.14;Inherit;True;Property;_TextureSample1;Texture Sample 1;40;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;949;-7316.858,-11923.9;Float;False;NEW_CLOSENESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;983;-4632.687,-13227.47;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1244;-8758.173,-8110.28;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;864;-7134.397,-11205.06;Inherit;False;900;NEW_BEHIND_DEPTH_03;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;987;-4553.602,-13114.91;Float;False;Constant;_DepthScale;Depth Scale;7;0;Create;True;0;0;False;0;-0.1;-0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;986;-4545.338,-13020.88;Float;False;Constant;_DepthOffset;Depth Offset;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;866;-7250.703,-10634.91;Float;False;Property;_foamMax;Beach Foam Distance;46;0;Create;False;0;0;False;0;0.7027565;17.4;0.001;25;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;867;-7217.244,-10717.2;Float;False;Constant;_foamMin;Foam Edge;56;0;Create;False;0;0;False;0;0.001;0.001;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;746;-8815.299,-9909.133;Float;False;Property;_OceanFoamTiling;Ocean Foam Tiling;36;0;Create;True;0;0;False;1;;2.032399;27.9;0.01;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;872;-6803.348,-11234.8;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;869;-7059.697,-10541.26;Inherit;False;949;NEW_CLOSENESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;991;-4335.624,-13203.17;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;736;-8448.994,-10151.17;Float;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;False;0;2,4;2,4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;868;-6829.213,-11052.97;Float;False;Property;_BeachFoamTint;Beach Foam Tint;42;0;Create;True;0;0;False;0;1,1,1,1;0.8962264,0.8962264,0.8962264,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;870;-6969.84,-10781.88;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;1245;-8518.173,-8126.28;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;788;-6118.172,-9054.283;Float;True;NORMAL_MAP_SAMPLE;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;1083;-4875.369,-10922.87;Inherit;False;882.827;293.7588;Comment;5;1164;1103;1099;1096;1095;N dot L;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;871;-6969.647,-10647.9;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;735;-8523.297,-9868.213;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;730;-8200.845,-10252.99;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;874;-6781.934,-10655.23;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;990;-4130.545,-13100.74;Float;False;Property;_WaterTint;Water Tint;0;0;Create;True;0;0;False;1;Header (COLOR TINT);0,0.7294118,1,1;0,0.418816,0.7075471,0.9921569;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;1095;-4846.193,-10787.15;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;873;-6780.498,-10783.03;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;993;-4043.296,-13204.21;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1246;-9062.176,-7902.28;Inherit;False;1039.283;407.6538;Wave Tile Shape;6;1269;1268;1267;1266;1248;1247;Wave Tile UV`s;0.8619477,0.08235294,1,1;0;0
Node;AmplifyShaderEditor.RotatorNode;737;-8197.953,-10064.07;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;994;-4090.367,-13373.11;Float;False;Property;_DepthTint;Depth Tint;1;0;Create;True;0;0;False;0;0.08627451,0.2941177,0.3490196,1;0,0.5943396,0.4922631,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;875;-6652.155,-11225;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1096;-4860.298,-10872.61;Inherit;False;788;NORMAL_MAP_SAMPLE;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;734;-8269.506,-9786.043;Float;False;Property;_OceanFoamSpeed;Ocean Foam Speed;38;0;Create;True;0;0;False;0;0.06184824;0.022;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1272;-8326.174,-8126.28;Float;False;WORLD_SPACE_UVs;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;1247;-9030.176,-7646.28;Float;False;Property;_WaveNoiseShape;Wave Noise Shape;30;0;Create;True;0;0;False;0;0.5,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;999;-3713.414,-13014.44;Float;False;Constant;_Float11;Float 11;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;740;-7929.713,-10063.98;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;747;-7927.479,-10254.69;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;1099;-4541.196,-10873.32;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1266;-9030.176,-7854.28;Inherit;True;1272;WORLD_SPACE_UVs;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;877;-6493.631,-11225.79;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;727;-7809.624,-9640.303;Inherit;False;746.2388;140.9981;Comment;3;752;741;732;DEPTH_FADE;0.05248642,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1178;-4929.536,-10448.83;Inherit;False;2201.101;726.7228;Create fake surface and depth shadowing;20;1304;1179;1182;1180;1186;1300;1187;1299;1183;1297;1286;1303;1181;1298;1280;1185;1301;1302;1277;1279;Fake Shadows;1,0.9427493,0,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;876;-6629.48,-10722.44;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;733;-7929.728,-9870.594;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;996;-3808.827,-13252.04;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;732;-7809.787,-9583.094;Float;False;Property;_OceanFoamDistance;Ocean Foam Distance;39;0;Create;True;0;0;False;0;1;168;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;1277;-4801.309,-10265.91;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;1268;-8742.173,-7742.28;Float;False;Property;_WaveTileUV;Wave Tile UV;27;0;Create;True;0;0;False;1;Header (WAVE);0;99.6;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;998;-4578.36,-13421.48;Float;False;Constant;_DPOffset;DP Offset;8;0;Create;True;0;0;False;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;738;-7723.077,-10086.76;Inherit;True;Property;_TextureSample4;Texture Sample 4;34;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;729;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;878;-6367.109,-11226.22;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1248;-8758.173,-7854.28;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;744;-7720.778,-10280.21;Inherit;True;Property;_TextureSample9;Texture Sample 9;34;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;729;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;879;-6502.059,-10720.45;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;997;-4567.232,-13528.85;Float;False;Constant;_DPScale;DP Scale;7;0;Create;True;0;0;False;0;-5;-5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1103;-4290.811,-10873.19;Float;True;NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;745;-7716.886,-9896.354;Inherit;True;Property;_FOAMTEXTURE;FOAM TEXTURE;34;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;729;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;1279;-4566.975,-10186.46;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;1000;-3535.88,-13255.22;Float;False;Property;_Keyword1;Keyword 1;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;1190;-4890.184,-9581.353;Inherit;False;2146.788;734.2653;Comment;17;1232;1204;1206;1205;1201;1203;1228;1242;1199;1195;1200;1198;1196;1197;1237;1194;1211;Reflection;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;1001;-4338.368,-13558.86;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1298;-4404.578,-9844.203;Float;False;Property;_ShadowIntensity;Shadow Intensity;11;0;Create;True;0;0;False;0;0;0.238;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1085;-4886.024,-11554.27;Inherit;False;1843.569;542.3054;retrieves the reflections from reflection probes;16;1165;1160;1156;1155;1152;1148;1147;1146;1144;1143;1141;1138;1137;1129;1127;1131;INDIRECT SPECULAR LIGHTING;0.9843137,1,0,1;0;0
Node;AmplifyShaderEditor.DepthFade;741;-7500.929,-9601.264;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1280;-4360.493,-10256.59;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;880;-6463.439,-11040.47;Float;False;Property;_BeachFoamStrenght;Beach Foam Strenght;44;0;Create;True;0;0;False;0;0.75;0.624;0.1;0.75;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1194;-4835.054,-9129.973;Float;False;Property;_ReflectionWobble;Reflection Wobble;19;0;Create;True;0;0;False;0;0.6218035;0.099;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;743;-7310.381,-9938.203;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1267;-8438.173,-7854.28;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;1211;-4797.707,-9210.194;Inherit;False;1103;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;963;-4901.409,-12627.37;Inherit;False;1914.155;350.8884;;6;969;968;967;966;965;964;FINAL_OPACITY;0,0.9529412,0.05987054,1;0;0
Node;AmplifyShaderEditor.LerpOp;881;-6247.48,-11222.25;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1301;-4740.19,-9867.478;Float;False;Property;_ShadowFresnelPower;Shadow Fresnel Power;13;0;Create;True;0;0;False;0;0;0.234;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1302;-4738.306,-9969.166;Float;False;Property;_ShadowFrenelScale;Shadow Frenel Scale;14;0;Create;True;0;0;False;0;0;0.803;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1185;-4793.012,-10392.58;Inherit;False;1103;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1003;-3212.901,-13255.28;Float;True;COLOR_TINT_FINAL;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;742;-7324.436,-9805.703;Float;False;Constant;_Float9;Float 9;31;0;Create;True;0;0;False;0;3;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;964;-4858.12,-12561.47;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;1269;-8278.174,-7854.28;Float;False;WaveTileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FresnelNode;1297;-4364.512,-10140.73;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;883;-6085.386,-11223.14;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.8602941,0.8602941,0.8602941,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;1181;-4556.248,-10384.19;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;1127;-4935.156,-11304.11;Inherit;False;1003;COLOR_TINT_FINAL;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;731;-7148.97,-9936.713;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;1249;-7974.172,-8254.282;Inherit;False;2292.233;761.1898;Comment;18;1270;1271;1265;1262;1264;1263;1251;1260;1261;1250;1259;1252;1257;1253;1255;1258;1254;1256;MOD_WAVE_02;0.9071532,0.08235294,1,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;1286;-4232.358,-10251.26;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;882;-6111.562,-11080.51;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;739;-7247.212,-10254.98;Float;False;Property;_OceanFoamTint;Ocean Foam Tint;35;0;Create;True;0;0;False;0;1,1,1,1;1,0.9669811,0.9669811,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;752;-7246.554,-9599.543;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1303;-4118.248,-9898.771;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;553;-7306.419,-10057.79;Inherit;False;900;NEW_BEHIND_DEPTH_03;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1196;-4818.225,-8952.65;Float;False;Property;_ReflectionFresnelPower;Reflection Fresnel Power;20;0;Create;True;0;0;False;0;2.66;6.37;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1002;-4099.999,-13559.86;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1195;-4531.402,-9202.084;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldReflectionVector;1242;-4805.243,-9453.061;Inherit;True;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;1197;-4823.654,-9040.275;Float;False;Property;_ReflectionFrenelScale;Reflection Frenel Scale;21;0;Create;True;0;0;False;0;0.372;0.581;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;965;-4857.749,-12372.56;Float;False;Property;_OpacityOcean;Opacity Ocean;3;0;Create;True;0;0;False;1;Header (OPACITY);0;0.097;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1004;-3793.972,-13465.44;Float;False;Constant;_OpacityBeach;Opacity Beach;8;0;Create;True;0;0;False;0;0.07898042;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;1254;-7814.172,-7854.28;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1200;-4319.84,-8956.852;Float;False;Property;_ReflectionIntensity;Reflection Intensity;18;0;Create;True;0;0;False;0;0;0.558;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;884;-5946.95,-11219.21;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;1129;-4619.08,-11324.18;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1199;-4351.829,-9326.684;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;1183;-4061.653,-10384.99;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;967;-4544.519,-12440.66;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;750;-6925.462,-9919.633;Float;False;Property;_OacenFoamStrenght;Oacen Foam Strenght;37;1;[Gamma];Create;True;0;0;False;0;0.28031;0.664;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1299;-3992.916,-10132.59;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1005;-3500.911,-13553.65;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;753;-6963.836,-10090.17;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1256;-7910.172,-7758.28;Float;False;Property;_WaveSpeed;Wave Speed;28;0;Create;True;0;0;False;0;0.5560129;0.573;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;966;-4637.725,-12567.24;Float;False;Property;_IgnoreVertexColor;Ignore Vertex Color;4;0;Create;True;0;0;False;0;1;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1258;-7846.172,-7646.28;Inherit;False;1269;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FresnelNode;1198;-4340.424,-9180.499;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1252;-7878.172,-8142.28;Inherit;False;1269;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;1228;-4170.582,-9397.505;Inherit;True;Property;_ReflectionCubeMap;Reflection Cube Map;17;0;Create;True;0;0;False;0;None;None;True;0;False;black;LockedToCube;False;Object;-1;Auto;Cube;6;0;SAMPLER2D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;748;-6634.454,-10117.35;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1300;-3855.949,-10372.57;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1137;-4859.533,-11490.06;Inherit;False;788;NORMAL_MAP_SAMPLE;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;885;-5793.882,-11248.09;Float;False;Property;_EnableBeachFoam;Enable Beach Foam;40;0;Create;True;0;0;False;1;Header (BEACH FOAM);1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1257;-7558.172,-7662.28;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.1,0.1,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;1253;-7862.172,-7998.28;Float;False;Property;_WaveDirection;Wave Direction;32;0;Create;True;0;0;False;0;1,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1255;-7494.172,-7918.28;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1141;-4394.221,-11507.42;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;968;-4331.626,-12554.71;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1138;-4504.625,-11136.58;Float;False;Property;_LightIndirectStrengthSpecular;Light Indirect Strength Specular;10;0;Create;True;0;0;False;1;Header (LIGHTING);1;0.288;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1006;-3327.77,-13556.52;Float;False;DEPTH_TINT_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1290;-5733.686,-11013.11;Float;False;Constant;_Float6;Float 6;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1201;-4002.181,-9179.234;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1165;-4677.641,-11235.66;Float;False;Property;_Occlusion;Occlusion;5;0;Create;True;0;0;False;1;Header (GLOBAL SETTINGS);0.9;0.283;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1187;-3923.775,-10237.3;Float;False;Property;_ShadowDepth;Shadow Depth;12;0;Create;True;0;0;False;0;0.6532351;0.999;0.9;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1144;-3938.559,-11357.64;Inherit;False;1003;COLOR_TINT_FINAL;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;969;-4153.557,-12544.39;Float;True;NEW_FINAL_OPACITY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1017;-2372.961,-13167.72;Inherit;False;1006;DEPTH_TINT_A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;1186;-3630.022,-10391.2;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectSpecularLight;1148;-4321.569,-11350.72;Inherit;False;Tangent;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1143;-4241.198,-11514.39;Float;False;SPECULAR_TINT_A1_5;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;749;-6439.986,-10135.11;Float;False;Property;_EnableOceanFoam;Enable Ocean Foam ;33;0;Create;True;0;0;False;1;Header (OCEAN FOAM);1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1146;-4152.994,-11147.39;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1289;-5552.559,-11242.2;Float;False;Property;_Keyword2;Keyword 2;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1203;-3826.214,-9414.215;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1147;-4202.366,-11437.51;Float;False;Constant;_Float5;Float 5;21;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;1259;-7270.172,-7774.28;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;1250;-7270.172,-7998.28;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;1156;-3928.142,-11508.52;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1204;-3585.504,-9313.989;Float;False;Constant;_Float2;Float 2;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1260;-6998.172,-8014.28;Inherit;False;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;1152;-3669.838,-11354.05;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;1155;-3758.778,-11437.08;Inherit;False;1143;SPECULAR_TINT_A1_5;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;1131;-3633.417,-11261.03;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;1018;-2246.746,-13065.17;Inherit;False;969;NEW_FINAL_OPACITY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;1205;-3646.634,-9443.211;Float;False;Property;_EnableReflection;Enable Reflection;15;0;Create;True;0;0;False;1;Header (REFLECTION);1;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;886;-5250.877,-11251.08;Float;True;BEACH_FOAM;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;901;-7324.939,-12477.05;Inherit;False;509.5791;207.0073;Comment;2;952;951;GRAB_SCREEN_COLOR;1,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;754;-6128.04,-10111.03;Float;True;OCAEN_FOAM;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1019;-2129.901,-13163.86;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1180;-3397.416,-10388.48;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;1251;-6678.172,-8174.28;Float;False;Property;_WaveUp;Wave Up;31;0;Create;True;0;0;False;0;0,1,0;-1,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;1263;-6678.172,-8014.28;Float;False;Property;_WaveHeight;Wave Height;29;0;Create;True;0;0;False;0;1;0.815;0;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1261;-6982.172,-7790.28;Inherit;False;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1182;-3205.9,-10376.35;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1264;-6358.172,-8174.28;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenColorNode;951;-7304.534,-12436.02;Float;False;Global;_GrabScreen1;Grab Screen 1;5;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1160;-3322.306,-11509.75;Inherit;True;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1262;-6774.172,-7918.28;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1020;-1965.025,-13148.24;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;755;-1960.431,-13585.32;Inherit;False;754;OCAEN_FOAM;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;1206;-3381.682,-9439.336;Float;False;Property;_Keyword3;Keyword 3;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1287;-1952.904,-13505.56;Inherit;False;886;BEACH_FOAM;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1179;-2983.835,-10374.12;Float;False;_FakeShadow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1265;-6150.172,-7934.28;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1162;-3011.986,-11521.42;Float;True;INDIRECT_SPECULAR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;1022;-1810.72,-13142.61;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1015;-1879.708,-13386.02;Inherit;False;1003;COLOR_TINT_FINAL;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1021;-1814.508,-13003.62;Float;False;Constant;_Float8;Float 8;31;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1232;-3062.461,-9435.97;Float;True;REFLECTION_CUBEMAP;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1288;-1690.904,-13569.56;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;952;-7121.216,-12438.34;Float;False;GRAB_SCREEN_COLOR;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;1042;-4910.616,-12213.54;Inherit;False;1905.956;503.9011;Geometric Roughness Factor For specular AARef: Geometry into Shading - http://graphics.pixar.com/library/BumpRoughness/paper.pdf - equation (3);7;1050;1049;1047;1046;1045;1044;1043;SMOOTHNESS;0,1,0.09278011,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;904;-6515.45,-12903.02;Inherit;False;1489.018;267.7906;Warped Depth Fade;10;926;910;914;915;921;925;919;908;906;923;BEHIND_DEPTH_02;0,1,0.0219624,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;728;-6573.641,-9873.994;Inherit;False;308.9639;240.5508;Comment;1;729;OCAEN FOAM MAP;0.3019608,0.4445193,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;902;-6497.336,-13285.37;Inherit;False;1482.119;323.1931;;11;918;907;912;927;916;930;929;913;917;924;920;BEHIND_DEPTH_01;1,0,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1271;-5974.172,-7934.28;Float;True;MOD_WAVE_02;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;1062;-1279.105,-13644.42;Inherit;False;169.4342;113.6689;Comment;1;1063;RENDER_OPTIONS;0,0.3987923,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;903;-7061.259,-12186.08;Inherit;False;2028.224;494.04;For underwater;16;935;937;936;938;943;939;940;942;941;957;954;960;958;961;955;959;GRAB_PASS;0,1,0.1558626,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1047;-4402.531,-12117.08;Inherit;False;385;227;https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@6.7/manual/Geometric-Specular-Anti-Aliasing.html;1;1048;Geometric Specular Anti-aliasing;0,1,0.1795304,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;1233;-1246.932,-13173.4;Inherit;False;1232;REFLECTION_CUBEMAP;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1024;-1638.303,-13256.06;Inherit;False;952;GRAB_SCREEN_COLOR;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;1169;-1631.684,-12780.92;Inherit;False;574.7582;207.8559;Comment;5;1174;1173;1172;1171;1170;METALLIC;0.9866247,1,0,1;0;0
Node;AmplifyShaderEditor.StaticSwitch;1023;-1627.406,-13144.74;Float;False;Property;_Keyword6;Keyword 6;31;0;Create;True;0;0;True;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1065;-1541.524,-13407.11;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;1188;-1203.636,-13070.86;Inherit;False;1179;_FakeShadow;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1175;-1248.117,-13259.11;Inherit;False;1162;INDIRECT_SPECULAR;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;921;-5716.678,-12854.06;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;908;-6169.581,-12851.56;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;802;-7878.172,-9022.283;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;957;-6909.137,-11855.48;Inherit;False;900;NEW_BEHIND_DEPTH_03;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;782;-8742.173,-8622.284;Float;False;Property;_NormalMapTiling;Normal Map Tiling;23;0;Create;True;0;0;False;0;1;77.1;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;938;-5831.427,-12032.87;Float;False;Global;_GrabTextureWater;GrabTextureWater;5;0;Create;True;0;0;False;0;Object;-1;True;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexToFragmentNode;1164;-4561.085,-10751.64;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;784;-7926.172,-8478.283;Float;False;Property;_NormalMapStrength;Normal Map Strength;24;0;Create;True;0;0;False;0;1;0.739;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1046;-4838,-12090.19;Float;False;Property;_SmoothnessVariance;Smoothness Variance;8;0;Create;True;0;0;False;0;0;0.184;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;795;-7878.172,-8654.283;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;776;-8358.174,-8926.283;Float;False;Constant;_Vector1;Vector 1;7;0;Create;True;0;0;False;0;2,4;2,4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RotatorNode;781;-8118.172,-8830.283;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;1274;-737.0482,-12828.63;Inherit;False;1271;MOD_WAVE_02;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;1025;-1280.401,-13404.44;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;801;-7030.172,-8670.283;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;958;-6920.162,-11961.01;Float;False;Constant;_ReflectionDistortionWaves;Reflection Distortion Waves;32;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;799;-7030.172,-8558.283;Float;False;Constant;_Color2;Color 2;7;0;Create;True;0;0;False;0;0,0,0,0.428;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;777;-7606.172,-8654.283;Inherit;True;Property;_TextureSample5;Texture Sample 5;22;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Instance;791;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;1176;-879.744,-13303.17;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1044;-4840.39,-12015.78;Float;False;Property;_SmoothnessThreshold;Smoothness Threshold;9;0;Create;True;0;0;False;0;0;0.115;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;792;-7606.172,-8862.283;Inherit;True;Property;_TextureSample3;Texture Sample 3;22;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Instance;791;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;887;-7260.938,-10552.06;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;906;-6482.584,-12771.09;Inherit;False;928;NEW_DISTANE_DEPTH_01;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;800;-6806.172,-8670.283;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;912;-5498.757,-13096.22;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;943;-5588.64,-12041.49;Float;False;Property;_Addpass;Add pass?;29;0;Fetch;True;0;0;False;0;0;0;0;False;UNITY_PASS_FORWARDADD;Toggle;2;Key0;Key1;Fetch;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwitchByFaceNode;1049;-3947.615,-12066.94;Inherit;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;922;-6494.661,-12450.03;Float;False;NEW_ZERO_ONE_DEPTH;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;778;-8230.174,-8542.283;Float;False;Property;_NormalMapSpeed;Normal Map Speed;25;0;Create;True;0;0;False;0;0;0.0704;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;907;-5365.677,-13090.35;Float;False;NEW_BEHIND_DEPTH_01;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;919;-6161.62,-12741.05;Inherit;False;913;NEW_DEPTH_DISTANCE;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;925;-5861.677,-12850.87;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1306;-4919.321,-11220.63;Inherit;False;-1;;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;942;-6055.958,-12023.53;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;787;-6262.172,-8702.283;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;961;-6491.379,-12126.8;Inherit;False;797;NORMAL_MAP;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexToFragmentNode;909;-5715.645,-13501.79;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1237;-4436.03,-9540.479;Float;True;Global;SkyboxReflection;Skybox Reflection;16;0;Create;True;0;0;False;0;None;None;False;white;LockedToCube;Cube;0;1;SAMPLERCUBE;0
Node;AmplifyShaderEditor.OneMinusNode;960;-6789.515,-12049.45;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;937;-5582.544,-11853.27;Float;False;NEW_SCREEN_DEPTH_WARPED;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;954;-6468.363,-11973.38;Inherit;False;5;5;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;786;-8134.172,-9022.283;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Exp2OpNode;1172;-1337.965,-12644.26;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;955;-6782.444,-12126.86;Inherit;False;969;NEW_FINAL_OPACITY;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;1270;-6582.172,-7790.28;Float;True;M_WAVE_PATTERN;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1173;-1194.897,-12734.85;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;1171;-1336.641,-12735.43;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;796;-6630.172,-8686.283;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;1050;-3390.181,-12057.39;Float;True;NEW_SMOOTHNESS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;926;-5342.682,-12850.18;Float;False;NEW_BEHIND_DEPTH_02;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;928;-5485.668,-12338.23;Float;False;NEW_DISTANE_DEPTH_01;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;913;-6195.074,-13069.56;Float;False;NEW_DEPTH_DISTANCE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;940;-6321.439,-11880.65;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;1063;-1267.424,-13607.49;Float;False;Property;_CullMode;Cull Mode;47;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CullMode;True;1;Header (RENDERING OPTIONS);2;0;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;1045;-4844.502,-12164.41;Float;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;False;0;0.95;0.127;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1170;-1596.799,-12735;Inherit;False;1103;NdotL;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;798;-7606.172,-9070.283;Inherit;True;Property;_TextureSample6;Texture Sample 6;22;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Instance;791;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;920;-6482.459,-13230.38;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;785;-8118.172,-8462.283;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;917;-6292.676,-13176.76;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;1043;-4667.14,-11907.3;Inherit;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;794;-7878.172,-8830.283;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;935;-5241.868,-12040.49;Float;False;NEW_GRABPASS;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;793;-6438.172,-8686.283;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;959;-7045.129,-12131.27;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;1048;-4356.531,-12067.08;Float;False;#define PerceptualSmoothnessToRoughness(perceptualSmoothness) (1.0 - perceptualSmoothness) * (1.0 - perceptualSmoothness)$#define RoughnessToPerceptualSmoothness(roughness) 1.0 - sqrt(roughness)$float3 deltaU = ddx(geometricNormalWS)@$float3 deltaV = ddy(geometricNormalWS)@$float variance = screenSpaceVariance * (dot(deltaU, deltaU) + dot(deltaV, deltaV))@$float roughness = PerceptualSmoothnessToRoughness(perceptualSmoothness)@$// Ref: Geometry into Shading - http://graphics.pixar.com/library/BumpRoughness/paper.pdf - equation (3)$float squaredRoughness = saturate(roughness * roughness + min(2.0 * variance, threshold * threshold))@ // threshold can be really low, square the value for easier$return RoughnessToPerceptualSmoothness(sqrt(squaredRoughness))@;1;False;4;True;perceptualSmoothness;FLOAT;0;In;;Float;False;True;geometricNormalWS;FLOAT3;0,0,0;In;;Float;False;True;screenSpaceVariance;FLOAT;0.5;In;;Float;False;True;threshold;FLOAT;0.5;In;;Float;False;GetGeometricNormalVariance;False;True;0;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.5;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;916;-5764.194,-13096.06;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;803;-725.3918,-13490.14;Inherit;False;797;NORMAL_MAP;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;939;-5765.781,-12121.06;Float;False;Constant;_Float7;Float 7;29;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;797;-6070.172,-8702.283;Float;True;NORMAL_MAP;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;915;-5580.487,-12852.17;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;779;-8454.173,-8654.283;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;923;-6504.013,-12852.64;Inherit;False;937;NEW_SCREEN_DEPTH_WARPED;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;927;-5624.551,-13096.75;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;1052;-766.4394,-12917.36;Inherit;False;1050;NEW_SMOOTHNESS;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;729;-6563.777,-9833.863;Inherit;True;Property;_OceanFoamMap;Ocean Foam Map;34;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1304;-4885.945,-10108.77;Inherit;False;-1;;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;936;-5839.053,-11857.88;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;911;-5479.963,-12412.52;Float;False;NEW_BASE_DEPTH_FADE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;910;-5436.14,-12759.51;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;783;-7254.172,-8974.283;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;941;-6251.272,-12024.77;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomExpressionNode;918;-5691.954,-13219.24;Float;False;return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f@;0;False;0;inInMirror;False;False;0;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;924;-6476.563,-13076.02;Float;False;Property;_DepthDistance_A;Depth Distance_A;48;0;Create;True;0;0;False;0;0;132;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;929;-5909.694,-13166.91;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;930;-6062.84,-13247.63;Inherit;False;911;NEW_BASE_DEPTH_FADE;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;914;-5684.005,-12734.34;Float;False;return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f@;0;False;0;inInMirror;False;False;0;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;1174;-1622.548,-12652.38;Float;False;Property;_Metallic;Metallic;6;0;Create;True;0;0;False;0;0;0.118;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;780;-8438.173,-8462.283;Float;False;Property;_NormalMapTimescale;Normal Map Timescale;26;0;Create;True;0;0;False;0;1;0.516;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1058;-303.1641,-13398.1;Float;False;True;2;ASEMaterialInspector;0;2;PWS/LW/Water/Ocean vP2.1 2019_02_06;1976390536c6c564abb90fe41f6ee334;True;Base;0;0;Base;11;False;False;False;True;2;True;1063;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Transparent=Queue=-10;True;3;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=LightweightForward;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position,InvertActionOnDeselection;1;Receive Shadows;0;LOD CrossFade;0;1;_FinalColorxAlpha;0;4;True;True;True;True;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1061;-424.7883,-13223.13;Float;False;False;2;ASEMaterialInspector;0;2;New Amplify Shader;1976390536c6c564abb90fe41f6ee334;True;Meta;0;3;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1060;-424.7883,-13223.13;Float;False;False;2;ASEMaterialInspector;0;2;New Amplify Shader;1976390536c6c564abb90fe41f6ee334;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1059;-424.7883,-13223.13;Float;False;False;2;ASEMaterialInspector;0;2;New Amplify Shader;1976390536c6c564abb90fe41f6ee334;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=LightweightPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;891;0;889;0
WireConnection;891;1;890;0
WireConnection;892;0;891;0
WireConnection;841;0;839;1
WireConnection;841;1;839;3
WireConnection;843;0;841;0
WireConnection;843;1;840;0
WireConnection;896;0;934;0
WireConnection;932;0;933;0
WireConnection;932;1;894;0
WireConnection;845;0;843;0
WireConnection;845;1;842;0
WireConnection;844;0;842;0
WireConnection;847;0;845;0
WireConnection;847;1;844;0
WireConnection;897;0;932;0
WireConnection;897;1;896;0
WireConnection;946;0;944;0
WireConnection;946;1;945;0
WireConnection;898;0;897;0
WireConnection;899;0;932;0
WireConnection;853;0;845;0
WireConnection;853;2;846;1
WireConnection;854;0;849;0
WireConnection;854;1;850;0
WireConnection;856;0;851;0
WireConnection;856;1;850;0
WireConnection;947;1;946;0
WireConnection;852;0;847;0
WireConnection;852;2;846;2
WireConnection;861;0;853;0
WireConnection;861;2;854;0
WireConnection;790;0;791;0
WireConnection;860;0;852;0
WireConnection;860;2;856;0
WireConnection;900;0;898;0
WireConnection;859;0;855;0
WireConnection;948;0;947;0
WireConnection;862;0;858;0
WireConnection;862;1;860;0
WireConnection;863;0;859;3
WireConnection;863;1;857;0
WireConnection;789;0;790;0
WireConnection;865;0;858;0
WireConnection;865;1;861;0
WireConnection;949;0;948;0
WireConnection;983;0;1295;0
WireConnection;872;0;865;0
WireConnection;872;1;862;0
WireConnection;872;2;862;4
WireConnection;872;3;864;0
WireConnection;991;0;983;0
WireConnection;991;1;987;0
WireConnection;991;2;986;0
WireConnection;870;0;863;0
WireConnection;870;1;867;0
WireConnection;1245;0;1244;1
WireConnection;1245;1;1244;3
WireConnection;788;0;789;0
WireConnection;871;0;866;0
WireConnection;871;1;867;0
WireConnection;735;0;746;0
WireConnection;730;0;735;0
WireConnection;730;2;736;1
WireConnection;874;0;871;0
WireConnection;874;1;869;0
WireConnection;873;0;870;0
WireConnection;873;1;869;0
WireConnection;993;0;991;0
WireConnection;737;0;735;0
WireConnection;737;2;736;2
WireConnection;875;1;872;0
WireConnection;875;2;868;0
WireConnection;1272;0;1245;0
WireConnection;740;0;737;0
WireConnection;740;2;734;0
WireConnection;747;0;730;0
WireConnection;747;2;734;0
WireConnection;1099;0;1096;0
WireConnection;1099;1;1095;0
WireConnection;877;0;875;0
WireConnection;876;0;873;0
WireConnection;876;1;874;0
WireConnection;733;0;735;0
WireConnection;733;2;734;0
WireConnection;996;0;994;0
WireConnection;996;1;990;0
WireConnection;996;2;993;0
WireConnection;738;1;740;0
WireConnection;878;0;877;0
WireConnection;1248;0;1266;0
WireConnection;1248;1;1247;0
WireConnection;744;1;747;0
WireConnection;879;0;876;0
WireConnection;1103;0;1099;0
WireConnection;745;1;733;0
WireConnection;1000;1;996;0
WireConnection;1000;0;999;0
WireConnection;1001;0;983;0
WireConnection;1001;1;997;0
WireConnection;1001;2;998;0
WireConnection;741;0;732;0
WireConnection;1280;0;1277;0
WireConnection;1280;1;1279;0
WireConnection;743;0;744;0
WireConnection;743;1;738;0
WireConnection;743;2;745;0
WireConnection;1267;0;1248;0
WireConnection;1267;1;1268;0
WireConnection;881;0;878;0
WireConnection;881;2;879;0
WireConnection;1003;0;1000;0
WireConnection;1269;0;1267;0
WireConnection;1297;2;1302;0
WireConnection;1297;3;1301;0
WireConnection;883;0;881;0
WireConnection;1181;0;1185;0
WireConnection;731;0;743;0
WireConnection;731;1;742;0
WireConnection;1286;0;1280;0
WireConnection;882;0;880;0
WireConnection;752;0;741;0
WireConnection;1303;0;1298;0
WireConnection;1002;0;1001;0
WireConnection;1195;0;1211;0
WireConnection;1195;1;1194;0
WireConnection;884;0;883;0
WireConnection;884;1;882;0
WireConnection;1129;0;1127;0
WireConnection;1199;0;1242;0
WireConnection;1199;1;1195;0
WireConnection;1183;0;1181;0
WireConnection;1183;1;1286;0
WireConnection;967;0;965;0
WireConnection;1299;0;1297;0
WireConnection;1299;1;1303;0
WireConnection;1005;0;1002;0
WireConnection;1005;1;1004;0
WireConnection;753;0;739;0
WireConnection;753;1;553;0
WireConnection;753;2;731;0
WireConnection;753;3;752;0
WireConnection;966;1;964;4
WireConnection;1198;2;1197;0
WireConnection;1198;3;1196;0
WireConnection;1228;1;1199;0
WireConnection;748;1;753;0
WireConnection;748;2;750;0
WireConnection;1300;0;1183;0
WireConnection;1300;2;1299;0
WireConnection;885;1;884;0
WireConnection;1257;0;1258;0
WireConnection;1255;0;1254;0
WireConnection;1255;1;1256;0
WireConnection;1141;0;1129;0
WireConnection;968;0;966;0
WireConnection;968;1;967;0
WireConnection;1006;0;1005;0
WireConnection;1201;0;1198;0
WireConnection;1201;1;1200;0
WireConnection;969;0;968;0
WireConnection;1186;0;1300;0
WireConnection;1186;1;1187;0
WireConnection;1186;2;1187;0
WireConnection;1148;0;1137;0
WireConnection;1148;1;1129;0
WireConnection;1148;2;1165;0
WireConnection;1143;0;1141;0
WireConnection;749;1;748;0
WireConnection;1146;0;1138;0
WireConnection;1289;1;885;0
WireConnection;1289;0;1290;0
WireConnection;1203;1;1228;0
WireConnection;1203;2;1201;0
WireConnection;1259;0;1257;0
WireConnection;1259;2;1253;0
WireConnection;1259;1;1255;0
WireConnection;1250;0;1252;0
WireConnection;1250;2;1253;0
WireConnection;1250;1;1255;0
WireConnection;1156;0;1147;0
WireConnection;1156;1;1148;0
WireConnection;1156;2;1146;0
WireConnection;1260;0;1250;0
WireConnection;1152;0;1144;0
WireConnection;1205;1;1203;0
WireConnection;886;0;1289;0
WireConnection;754;0;749;0
WireConnection;1019;0;1017;0
WireConnection;1180;0;1186;0
WireConnection;1261;0;1259;0
WireConnection;1182;0;1180;0
WireConnection;1264;0;1251;0
WireConnection;1264;1;1263;0
WireConnection;1160;0;1156;0
WireConnection;1160;1;1155;0
WireConnection;1160;2;1152;0
WireConnection;1160;3;1131;0
WireConnection;1262;0;1260;0
WireConnection;1262;1;1261;0
WireConnection;1020;0;1019;0
WireConnection;1020;1;1018;0
WireConnection;1206;1;1205;0
WireConnection;1206;0;1204;0
WireConnection;1179;0;1182;0
WireConnection;1265;0;1264;0
WireConnection;1265;1;1262;0
WireConnection;1162;0;1160;0
WireConnection;1022;0;1020;0
WireConnection;1232;0;1206;0
WireConnection;1288;0;755;0
WireConnection;1288;1;1287;0
WireConnection;952;0;951;0
WireConnection;1271;0;1265;0
WireConnection;1023;1;1022;0
WireConnection;1023;0;1021;0
WireConnection;1065;0;1288;0
WireConnection;1065;1;1015;0
WireConnection;921;0;925;0
WireConnection;921;1;925;0
WireConnection;908;0;923;0
WireConnection;908;1;906;0
WireConnection;802;0;786;0
WireConnection;802;2;778;0
WireConnection;802;1;785;0
WireConnection;938;0;942;0
WireConnection;1164;0;1095;0
WireConnection;795;0;779;0
WireConnection;795;2;778;0
WireConnection;795;1;785;0
WireConnection;781;0;779;0
WireConnection;781;2;776;2
WireConnection;1025;0;1065;0
WireConnection;1025;1;1024;0
WireConnection;1025;2;1023;0
WireConnection;801;0;783;0
WireConnection;801;1;777;0
WireConnection;777;1;795;0
WireConnection;777;5;784;0
WireConnection;1176;0;1175;0
WireConnection;1176;1;1233;0
WireConnection;1176;2;1188;0
WireConnection;792;1;794;0
WireConnection;792;5;784;0
WireConnection;887;0;863;0
WireConnection;800;0;801;0
WireConnection;800;2;799;0
WireConnection;912;1;927;0
WireConnection;943;1;938;0
WireConnection;943;0;939;0
WireConnection;1049;0;1048;0
WireConnection;922;0;933;0
WireConnection;907;0;912;0
WireConnection;925;0;908;0
WireConnection;925;1;919;0
WireConnection;942;0;941;0
WireConnection;942;1;940;0
WireConnection;787;1;793;0
WireConnection;960;0;959;0
WireConnection;937;0;936;0
WireConnection;954;0;955;0
WireConnection;954;1;958;0
WireConnection;954;2;949;0
WireConnection;954;3;960;0
WireConnection;954;4;957;0
WireConnection;786;0;779;0
WireConnection;786;2;776;1
WireConnection;1172;0;1174;0
WireConnection;1270;0;1262;0
WireConnection;1173;0;1171;0
WireConnection;1173;1;1172;0
WireConnection;1171;0;1170;0
WireConnection;796;0;800;0
WireConnection;1050;0;1049;0
WireConnection;926;0;915;0
WireConnection;928;0;896;0
WireConnection;913;0;924;0
WireConnection;798;1;802;0
WireConnection;798;5;784;0
WireConnection;785;0;780;0
WireConnection;917;0;920;2
WireConnection;794;0;781;0
WireConnection;794;2;778;0
WireConnection;794;1;785;0
WireConnection;935;0;943;0
WireConnection;793;0;796;0
WireConnection;1048;0;1045;0
WireConnection;1048;1;1043;0
WireConnection;1048;2;1046;0
WireConnection;1048;3;1044;0
WireConnection;916;0;930;0
WireConnection;916;1;913;0
WireConnection;797;0;787;0
WireConnection;915;1;921;0
WireConnection;779;0;782;0
WireConnection;927;0;916;0
WireConnection;927;1;916;0
WireConnection;936;0;942;0
WireConnection;911;0;897;0
WireConnection;910;0;915;0
WireConnection;910;1;914;0
WireConnection;783;0;798;0
WireConnection;783;1;792;0
WireConnection;941;1;961;0
WireConnection;941;2;954;0
WireConnection;929;0;917;0
WireConnection;929;1;913;0
WireConnection;1058;0;1025;0
WireConnection;1058;1;803;0
WireConnection;1058;2;1176;0
WireConnection;1058;3;1173;0
WireConnection;1058;4;1052;0
WireConnection;1058;8;1274;0
ASEEND*/
//CHKSM=2F2CB030677C98A2864F1DB42AF567476CDD1B32