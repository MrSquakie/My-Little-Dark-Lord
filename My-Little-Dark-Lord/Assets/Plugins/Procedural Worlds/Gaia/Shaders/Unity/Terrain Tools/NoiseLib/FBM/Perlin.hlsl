//////////////////////////////////////////////////////////////////////////
//
//      DO NOT EDIT THIS FILE!! THIS IS AUTOMATICALLY GENERATED!!
//      DO NOT EDIT THIS FILE!! THIS IS AUTOMATICALLY GENERATED!!
//      DO NOT EDIT THIS FILE!! THIS IS AUTOMATICALLY GENERATED!!
//
//////////////////////////////////////////////////////////////////////////

#ifndef UNITY_TERRAIN_TOOL_NOISE_FbmPerlin_INC
#define UNITY_TERRAIN_TOOL_NOISE_FbmPerlin_INC

/*=========================================================================

    Includes

=========================================================================*/

#include "Assets/Procedural Worlds/Gaia/Shaders/Unity/Terrain Tools/NoiseLib/Implementation/PerlinImpl.hlsl"
#include "Assets/Procedural Worlds/Gaia/Shaders/Unity/Terrain Tools/NoiseLib/NoiseCommon.hlsl"





#ifndef FBMFRACTALINPUT_DEF // [ FBMFRACTALINPUT_DEF
#define FBMFRACTALINPUT_DEF

struct FbmFractalInput
{
	float octaves;
	float amplitude;
	float persistence;
	float frequency;
	float lacunarity;
	float warpIterations;
	float warpStrength;
	float4 warpOffsets;
};



FbmFractalInput GetDefaultFbmFractalInput()
{
	FbmFractalInput ret;

	ret.octaves = 8;
	ret.amplitude = 0.5;
	ret.persistence = 0.5;
	ret.frequency = 1;
	ret.lacunarity = 2;
	ret.warpIterations = 0;
	ret.warpStrength = 0.5;
	ret.warpOffsets = float4(2.5, 1.4, 3.2, 2.7);

	return ret;
}




float _FbmOctaves;
float _FbmAmplitude;
float _FbmPersistence;
float _FbmFrequency;
float _FbmLacunarity;
float _FbmWarpIterations;
float _FbmWarpStrength;
float4 _FbmWarpOffsets;

FbmFractalInput GetFbmFractalInput()
{
	FbmFractalInput ret;

	ret.octaves = _FbmOctaves;
	ret.amplitude = _FbmAmplitude;
	ret.persistence = _FbmPersistence;
	ret.frequency = _FbmFrequency;
	ret.lacunarity = _FbmLacunarity;
	ret.warpIterations = _FbmWarpIterations;
	ret.warpStrength = _FbmWarpStrength;
	ret.warpOffsets = _FbmWarpOffsets;

	return ret;
}



#endif // ] FBMFRACTALINPUT_DEF



/*=========================================================================

    Fractal Functions

=========================================================================*/

float noise_FbmPerlin_Raw( float pos, FbmFractalInput fractalInput )
{
    float prev = 0;
    float n = 0;

    float octaves = ceil(fractalInput.octaves) + (1 - sign(frac(fractalInput.octaves)));

    for( float i = 0; i < octaves; ++i )
    {
        prev = n;
        n += fractalInput.amplitude * get_noise_Perlin( pos * fractalInput.frequency );
        fractalInput.frequency *= fractalInput.lacunarity;
        fractalInput.amplitude *= fractalInput.persistence;
    }

    n = lerp(prev, n, frac(fractalInput.octaves));

    return n;
}

float noise_FbmPerlin_Raw( float2 pos, FbmFractalInput fractalInput )
{
    float prev = 0;
    float n = 0;

    float octaves = ceil(fractalInput.octaves) + (1 - sign(frac(fractalInput.octaves)));

    for( float i = 0; i < octaves; ++i )
    {
        prev = n;
        n += fractalInput.amplitude * get_noise_Perlin( pos * fractalInput.frequency );
        fractalInput.frequency *= fractalInput.lacunarity;
        fractalInput.amplitude *= fractalInput.persistence;
    }

    n = lerp(prev, n, frac(fractalInput.octaves));

    return n;
}

float noise_FbmPerlin_Raw( float3 pos, FbmFractalInput fractalInput )
{
    float prev = 0;
    float n = 0;

    float octaves = ceil(fractalInput.octaves) + (1 - sign(frac(fractalInput.octaves)));

    for( float i = 0; i < octaves; ++i )
    {
        prev = n;
        n += fractalInput.amplitude * get_noise_Perlin( pos * fractalInput.frequency );
        fractalInput.frequency *= fractalInput.lacunarity;
        fractalInput.amplitude *= fractalInput.persistence;
    }

    n = lerp(prev, n, frac(fractalInput.octaves));

    return n;
}

float noise_FbmPerlin_Raw( float4 pos, FbmFractalInput fractalInput )
{
    float prev = 0;
    float n = 0;

    float octaves = ceil(fractalInput.octaves) + (1 - sign(frac(fractalInput.octaves)));

    for( float i = 0; i < octaves; ++i )
    {
        prev = n;
        n += fractalInput.amplitude * get_noise_Perlin( pos * fractalInput.frequency );
        fractalInput.frequency *= fractalInput.lacunarity;
        fractalInput.amplitude *= fractalInput.persistence;
    }

    n = lerp(prev, n, frac(fractalInput.octaves));

    return n;
}

/*=========================================================================

    FbmPerlin Noise Functions - Fractal, Warped

=========================================================================*/

float noise_FbmPerlin( float pos, FbmFractalInput fractalInput )
{
    float prev = 0;

    float warpIterations = ceil(fractalInput.warpIterations) + (1 - sign(frac(fractalInput.warpIterations)));

    // do warping
    for ( float i = 0; i < warpIterations; ++i )
    {
        float q = noise_FbmPerlin_Raw( pos + fractalInput.warpOffsets.x, fractalInput );
        prev = pos;
        pos = pos + fractalInput.warpStrength * q;
    }

    pos = lerp(prev, pos, frac(fractalInput.warpIterations));

    float f = noise_FbmPerlin_Raw( pos, fractalInput );

    return f;
}

float noise_FbmPerlin( float2 pos, FbmFractalInput fractalInput )
{
    float2 prev = 0;

    float warpIterations = ceil(fractalInput.warpIterations) + (1 - sign(frac(fractalInput.warpIterations)));

    // do warping
    for ( float i = 0; i < warpIterations; ++i )
    {
        float2 q = float2( noise_FbmPerlin_Raw( pos, fractalInput ),
                           noise_FbmPerlin_Raw( pos + fractalInput.warpOffsets.xy, fractalInput ) );
        prev = pos;

        pos = pos + fractalInput.warpStrength * q;
    }
    
    pos = lerp(prev, pos, frac(fractalInput.warpIterations));

    float f = noise_FbmPerlin_Raw( pos, fractalInput );

    return f;
}

float noise_FbmPerlin( float3 pos, FbmFractalInput fractalInput )
{
    float3 prev = 0;

    float warpIterations = ceil(fractalInput.warpIterations) + (1 - sign(frac(fractalInput.warpIterations)));

    // do warping
    for ( float i = 0; i < warpIterations; ++i )
    {
        float3 q = float3( noise_FbmPerlin_Raw( pos.xyz, fractalInput ),
                    noise_FbmPerlin_Raw( pos.xyz + fractalInput.warpOffsets.xyz, fractalInput ),
                    noise_FbmPerlin_Raw( pos.xyz + float3( fractalInput.warpOffsets.x, fractalInput.warpOffsets.y, 0 ), fractalInput ) );
        prev = pos;
        pos = pos + fractalInput.warpStrength * q;
    }
    
    pos = lerp(prev, pos, frac(fractalInput.warpIterations));
    
    float f = noise_FbmPerlin_Raw( pos, fractalInput );

    return f;
}

float noise_FbmPerlin( float4 pos, FbmFractalInput fractalInput )
{
    float f = noise_FbmPerlin_Raw( pos, fractalInput );

    return f;
}

#endif // UNITY_TERRAIN_TOOL_NOISE_FbmPerlin_INC