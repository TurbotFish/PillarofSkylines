//
// MobileCloudLightingModel.cginc: Simple lighting model for clouds.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

inline fixed4 LightingMobileCloud (SurfaceOutput s, fixed3 lightDir, fixed atten) {
	fixed4 c;
	c.rgb = s.Albedo.rgb + (_LightColor0.rgb);
	c.a = s.Alpha;
	return c;
}