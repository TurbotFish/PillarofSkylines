#ifndef WIND_SYSTEM_INCLUDED
#define WIND_SYSTEM_INCLUDED

float3 _WindDir;

float3 RotationMatX(float3 _vertex, float rotation){
	float3x3 mat;
	mat[0] = float3(1, 0, 0);
	mat[1] = float3(0, cos(rotation), -sin(rotation));
	mat[2] = float3(0, sin(rotation), cos(rotation));
	return mul(mat, _vertex);
}

float3 RotationMatY(float3 _vertex, float rotation){
	float3x3 mat;
	mat[0] = float3(cos(rotation), 0, sin(rotation));
	mat[1] = float3(0, 1, 0);
	mat[2] = float3(-sin(rotation), 0, cos(rotation));
	return mul(mat, _vertex);
}//shouldn't be necessary


float3 RotationMatZ(float3 _vertex, float rotation){
	float3x3 mat;
	mat[0] = float3(cos(rotation), -sin(rotation), 0);
	mat[1] = float3(sin(rotation), cos(rotation), 0);
	mat[2] = float3(0, 0, 1);
	return mul(mat, _vertex);
}

float3 Rad2Deg(float3 rad){
	return rad * 3.14 / 180;
}

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