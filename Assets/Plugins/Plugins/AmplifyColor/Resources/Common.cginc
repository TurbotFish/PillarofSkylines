// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFY_COLOR_COMMON_INCLUDED
#define AMPLIFY_COLOR_COMMON_INCLUDED

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform float4 _MainTex_TexelSize;
uniform sampler2D _RgbTex;
uniform sampler2D _LerpRgbTex;
uniform sampler2D _RgbBlendCacheTex;
uniform sampler2D _MaskTex;
uniform float4 _MaskTex_TexelSize;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _DepthCurveLut;
uniform float _LerpAmount;
uniform float _Exposure;

struct v2f
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
#if defined( AC_DITHERING )
	float4 screenPos : TEXCOORD2;
#endif
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos( v.vertex );
	o.uv = v.texcoord.xy;
	o.uv1 = v.texcoord.xy;
	#if defined( UNITY_UV_STARTS_AT_TOP )
		if ( _MainTex_TexelSize.y < 0 )
			o.uv1.y = 1 - o.uv1.y;
	#endif
	#if defined( UNITY_HALF_TEXEL_OFFSET )
		o.uv1.xy += _MaskTex_TexelSize.xy * float2( -0.5, 0.5 );
	#endif
	#if defined( AC_DITHERING )
		o.screenPos = ComputeScreenPos( o.pos );
	#endif
	return o;
}

inline float4 to_srgb( float4 c )
{
	c.rgb = max( 1.055 * pow( c.rgb, 0.416666667 ) - 0.055, 0 );
	return c;
}

inline float4 to_linear( float4 c )
{
	c.rgb = c.rgb * ( c.rgb * ( c.rgb * 0.305306011 + 0.682171111 ) + 0.012522878 );
	return c;
}

#define A ( 0.15 )
#define B ( 0.50 )
#define C ( 0.10 )
#define D ( 0.20 )
#define E ( 0.02 )
#define F ( 0.30 )
#define W ( 11.2 )

inline float3 uncharted2_tonemap( float3 x )
{
	return ( ( x * ( A * x + C * B ) + D *  E ) / ( x * ( A * x + B ) + D *  F ) ) - E / F;
}

inline float3 tonemap( float3 color )
{
	// Uncharted 2 tone mapping operator
	// http://filmicgames.com/archives/75

	const float ExposureBias = 2.0f;
	const float3 white_scale = 1 / uncharted2_tonemap( W );
	color = uncharted2_tonemap( color * _Exposure * ExposureBias ) * white_scale;
	return color;
}

inline float3 screen_space_dither( float4 screenPos )
{
	// Iestyn's RGB dither (7 asm instructions) from Portal 2 X360, slightly modified for VR
	// http://alex.vlachos.com/graphics/Alex_Vlachos_Advanced_VR_Rendering_GDC2015.pdf

	float3 d = dot( float2( 171.0, 231.0 ), UNITY_PROJ_COORD( screenPos ).xy * _MainTex_TexelSize.zw ).xxx;
	d.rgb = frac( d.rgb / float3( 103.0, 71.0, 97.0 ) ) - float3( 0.5, 0.5, 0.5 );
	return d.rgb / 255.0;
}

inline float4 fetch_process_ldr_gamma( v2f i )
{
	// fetch
	float4 color = tex2D( _MainTex, i.uv );

	// dither
	#if defined( AC_DITHERING )
		color.rgb += screen_space_dither( i.screenPos );
	#endif

	// clamp
	#if AC_QUALITY_MOBILE
		color.rgb = min( ( 0.999 ).xxx, color.rgb ); // dev/hw compatibility
	#else
	#endif

	return color;
}

inline float4 fetch_process_hdr_gamma( v2f i )
{
	// fetch
	float4 color = tex2D( _MainTex, i.uv );

	// tonemap
	#if defined( AC_TONEMAPPING )
		color.rgb = tonemap( color.rgb );
	#else
		color.rgb *= _Exposure;
	#endif

	// dither
	#if defined( AC_DITHERING )
		color.rgb += screen_space_dither( i.screenPos );
	#endif

	// clamp
	#if AC_QUALITY_MOBILE
		color.rgb = clamp( 0, ( 0.999 ).xxx, color.rgb ); // dev/hw compatibility
	#else
		color.rgb = saturate( color.rgb );
	#endif

	return color;
}

inline float4 fetch_process_ldr_linear( v2f i )
{
	// fetch
	float4 color = tex2D( _MainTex, i.uv );

	// convert to gamma
	color = to_srgb( color );

	// dither
	#if defined( AC_DITHERING )
		color.rgb += screen_space_dither( i.screenPos );
	#endif

	// clamp
	#if AC_QUALITY_MOBILE
		color.rgb = min( ( 0.999 ).xxx, color.rgb ); // dev/hw compatibility
	#else
	#endif

	return color;
}

inline float4 fetch_process_hdr_linear( v2f i )
{
	// fetch
	float4 color = tex2D( _MainTex, i.uv );

	// tonemap
	#if defined( AC_TONEMAPPING )
		color.rgb = tonemap( color.rgb );
	#else
		color.rgb *= _Exposure;
	#endif

	// convert to gamma
	color = to_srgb( color );

	// dither
	#if defined( AC_DITHERING )
		color.rgb += screen_space_dither( i.screenPos );
	#endif

	// clamp
	#if AC_QUALITY_MOBILE
		color.rgb = clamp( 0, ( 0.999 ).xxx, color.rgb ); // dev/hw compatibility
	#else
		color.rgb = saturate( color.rgb );
	#endif

	return color;
}

inline float4 output_ldr_gamma( float4 color )
{
	return color;
}

inline float4 output_hdr_gamma( float4 color )
{
	return color;
}

inline float4 output_ldr_linear( float4 color )
{
	return to_linear( color );
}

inline float4 output_hdr_linear( float4 color )
{
	return to_linear( color );
}

inline float3 apply_lut( float3 color, sampler2D lut )
{
	const float4 coord_scale = float4( 0.0302734375, 0.96875, 31.0, 0.0 );
	const float4 coord_offset = float4( 0.00048828125, 0.015625, 0.0, 0.0 );
	const float2 texel_height_X0 = float2( 0.03125, 0.0 );

	float3 coord = color * coord_scale + coord_offset;

	#if AC_QUALITY_MOBILE

		float3 coord_floor = floor( coord + 0.5 );
		float2 coord_bot = coord.xy + coord_floor.zz * texel_height_X0;

		color.rgb = tex2D( lut, coord_bot ).rgb;

	#else

		float3 coord_frac = frac( coord );
		float3 coord_floor = coord - coord_frac;

		float2 coord_bot = coord.xy + coord_floor.zz * texel_height_X0;
		float2 coord_top = coord_bot + texel_height_X0;

		float3 lutcol_bot = tex2D( lut, coord_bot ).rgb;
		float3 lutcol_top = tex2D( lut, coord_top ).rgb;

		color.rgb = lerp( lutcol_bot, lutcol_top, coord_frac.z );

	#endif

	return color;
}

inline float4 apply( float4 color )
{
	color.rgb = apply_lut( color.rgb, _RgbTex );
	return color;
}

inline float4 apply_blend( float4 color )
{
	color.rgb = apply_lut( color.rgb, _RgbBlendCacheTex );
	return color;
}

#endif // AMPLIFY_COLOR_COMMON_INCLUDED
