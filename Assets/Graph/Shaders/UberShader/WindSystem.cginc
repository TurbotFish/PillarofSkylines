#ifndef WIND_SYSTEM_INCLUDED
#define WIND_SYSTEM_INCLUDED

float3 RotationMatXYZ(float3 _vertex, float3 rotation){
	float3x3 mat;
	mat[0] = float3(
		cos(rotation.y) * cos(rotation.z),
		-cos(rotation.y) * sin(rotation.z), 
		sin(rotation.y)
	);
	mat[1] = float3(
		cos(rotation.x) * sin(rotation.z) + sin(rotation.x) * sin(rotation.y) * cos(rotation.z),
		cos(rotation.x) * cos(rotation.z) - sin(rotation.x) * sin(rotation.y) * sin(rotation.z),
		-sin(rotation.x) * cos(rotation.y)
	);
	mat[2] = float3(
		sin(rotation.x) * sin(rotation.z) - cos(rotation.x) * sin(rotation.y) * cos(rotation.z),
		sin(rotation.x) * cos(rotation.z) + cos(rotation.x) * sin(rotation.y) * sin(rotation.z),
		cos(rotation.x) * cos(rotation.y)
	);
	return mul(mat, _vertex);
}

float3 ApplyWind(float3 _vec, float3 _rotation){
	_rotation *= 3.1416 / 180;
	_vec = RotationMatXYZ(_vec, _rotation);
	return _vec;
}



#endif