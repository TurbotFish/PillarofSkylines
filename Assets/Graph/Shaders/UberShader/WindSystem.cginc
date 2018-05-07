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

float3 RotationAroundAxis(float3 _axis, float _angle, float4 _position){
	_angle *= 3.1416 / 180;
	float4x4 _matrix;
	_matrix[0] = float4(
					(1 - cos(_angle)) * _axis.x * _axis.x + cos(_angle),
					(1 - cos(_angle)) * _axis.x * _axis.y - _axis.z * sin(_angle),
					(1 - cos(_angle)) * _axis.z * _axis.x + _axis.y * sin(_angle),
					0
				);
	_matrix[1] = float4(
					(1 - cos(_angle)) * _axis.x * _axis.y + _axis.z * sin(_angle),
					(1 - cos(_angle)) * _axis.y * _axis.y + cos(_angle),
					(1 - cos(_angle)) * _axis.y * _axis.z - _axis.x * sin(_angle),
					0
				);

	_matrix[2] = float4(
					(1 - cos(_angle)) * _axis.z * _axis.x - _axis.y * sin(_angle),
					(1 - cos(_angle)) * _axis.y * _axis.z + _axis.x * sin(_angle),
					(1 - cos(_angle)) * _axis.z * _axis.z + cos(_angle),
					0
				);

	_matrix[3] = float4(
					0,
					0,
					0,
					1
				);

	return mul(_position, _matrix);
}

float3 ApplyWind(float3 _vec, float3 _rotation){
	_rotation *= 3.1416 / 180;
	_vec = RotationMatXYZ(_vec, _rotation);
	return _vec;
}

float3 NewWind(float3 _axis, float4 _position, float _angle){
	return RotationAroundAxis(_axis, _angle, _position).xyz;
}


#endif