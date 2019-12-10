//////////// PWSF Gerstner Three Waves 04 SRP
#if !defined(PWSFGerstnerThreeWaves04SRP)
#define PWSFGerstnerThreeWaves04SRP



float3 PWSFGerstnerWaveSRP(
	float4 wave, float wspeed, float3 p, inout float3 tangent, inout float3 binormal
) {

	float steepness = wave.z;
	float wavelength = wave.w;
	float k = 2 * PI / wavelength;
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
#endif