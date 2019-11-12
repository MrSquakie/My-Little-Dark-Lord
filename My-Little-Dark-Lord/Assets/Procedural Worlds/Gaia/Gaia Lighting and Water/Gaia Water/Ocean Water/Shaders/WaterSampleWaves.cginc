//////////// WaterSampleWaves
#if !defined(WaterSampleWaves)
#define WaterSampleWaves


        float3 WaterSampleWaves (
        float4 Wave, float wspeed, float3 p, inout float3 tangent, inout float3 binormal
        ) 
        
        {

            float steepness = Wave.z;
            float wavelength = Wave.w;
            float k = 2 * UNITY_PI / wavelength;
            float c = sqrt(9.8 / k);
            float2 d = normalize(Wave.xy);
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
        
        
#endif