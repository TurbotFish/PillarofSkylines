//
// CloudLightingModel.cginc: Simple lighting model for clouds.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

inline float4 LightingCloud (SurfaceOutput s, float3 lightDir, float atten) {
	float4 c;
	c.rgb = s.Albedo.rgb + (_LightColor0.rgb);
	c.a = s.Alpha;
	return c;
}