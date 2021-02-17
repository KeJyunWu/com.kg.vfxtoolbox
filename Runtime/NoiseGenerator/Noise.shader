// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Noise"
{
	Properties
	{
		_MainTex ( "Screen", 2D ) = "black" {}
		_GlobalUV_ScrollSpeed("GlobalUV_ScrollSpeed", Float) = 0
		_Effect_Base_ResultMul("Effect_Base_ResultMul", Float) = 0
		_Effect_Base_NoiseScale("Effect_Base_NoiseScale", Float) = 0
		_Effect_Base_UVTwirl_ScrollSpeed("Effect_Base_UVTwirl_ScrollSpeed", Float) = 0
		_Effect_Base_UVTwirl_Intensity("Effect_Base_UVTwirl_Intensity", Float) = 0
		_Effect_Base_UVTwirl_Center("Effect_Base_UVTwirl_Center", Vector) = (0,0,0,0)
		_Effect_Base_UVWarp_ScrollSpeed("Effect_Base_UVWarp_ScrollSpeed", Float) = 0
		_Effect_Base_UVWarp_Intensity("Effect_Base_UVWarp_Intensity", Float) = 0
		_Effect_Base_UVWarp_NoiseScale("Effect_Base_UVWarp_NoiseScale", Float) = 0
		_Effect_Base_UVWarp2_NoiseScale("Effect_Base_UVWarp2_NoiseScale", Float) = 0
		_Effect_Wave_ResultMul("Effect_Wave_ResultMul", Float) = 0
		_Effect_Wave_Scale("Effect_Wave_Scale", Float) = 0
		_Effect_Wave_ScrollSpeed("Effect_Wave_ScrollSpeed", Float) = 0
		_Effect_Wave_ResultPower("Effect_Wave_ResultPower", Float) = 0
		_Effect_Scan_ResultMul("Effect_Scan_ResultMul", Float) = 0
		_Effect_Scan_ModeLerp("Effect_Scan_ModeLerp", Float) = 0
		_Effect_Scan_ResultPower("Effect_Scan_ResultPower", Float) = 0
		_Effect_Scan_UVWarpScale("Effect_Scan_UVWarpScale", Float) = 0
		_Effect_Scan_Scale("Effect_Scan_Scale", Float) = 0
		_Effect_Scan_ScrollSpeed("Effect_Scan_ScrollSpeed", Float) = 0
		_Effect_FBM_ResultMul("Effect_FBM_ResultMul", Float) = 0
		_Effect_FBM_Frequency("Effect_FBM_Frequency", Float) = 0
		_Effect_FBM_Octaves("Effect_FBM_Octaves", Float) = 0
		_Effect_FBM_Lacunarity("Effect_FBM_Lacunarity", Float) = 0
		_Effect_FBM_Gain("Effect_FBM_Gain", Float) = 0
		_Effect_Mask_ResultMul("Effect_Mask_ResultMul", Float) = 0
		_Effect_Mask_PixelsNum("Effect_Mask_PixelsNum", Float) = 0
		_Effect_Mask_NoiseScrollSpeed("Effect_Mask_NoiseScrollSpeed", Vector) = (0,0,0,0)
		_Effect_Mask_NoiseScale("Effect_Mask_NoiseScale", Float) = 0
		_Effect_Mask_ResultPower("Effect_Mask_ResultPower", Float) = 0
		_ScriptTime("ScriptTime", Float) = 0
		_Effect_Wave_PivotOffset("Effect_Wave_PivotOffset", Vector) = (0,0,0,0)
		_TextureRatio("TextureRatio", Float) = 0

	}

	SubShader
	{
		LOD 0

		
		
		ZTest Always
		Cull Off
		ZWrite Off

		
		Pass
		{ 
			CGPROGRAM 

			

			#pragma vertex vert_img_custom 
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
			#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
			#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex.SampleLevel(samplerTex,coord, lod)
			#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex.SampleBias(samplerTex,coord,bias)
			#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex.SampleGrad(samplerTex,coord,ddx,ddy)
			#else//ASE Sampling Macros
			#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
			#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex2Dlod(tex,float4(coord,0,lod))
			#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex2Dbias(tex,float4(coord,0,bias))
			#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex2Dgrad(tex,coord,ddx,ddy)
			#endif//ASE Sampling Macros
			


			struct appdata_img_custom
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				
			};

			struct v2f_img_custom
			{
				float4 pos : SV_POSITION;
				half2 uv   : TEXCOORD0;
				half2 stereoUV : TEXCOORD2;
		#if UNITY_UV_STARTS_AT_TOP
				half4 uv2 : TEXCOORD1;
				half4 stereoUV2 : TEXCOORD3;
		#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float _TextureRatio;
			uniform float _ScriptTime;
			uniform float _Effect_Base_UVWarp_ScrollSpeed;
			uniform float _Effect_Base_UVWarp_NoiseScale;
			uniform float _Effect_Base_UVWarp2_NoiseScale;
			uniform float _Effect_Base_UVWarp_Intensity;
			uniform float2 _Effect_Base_UVTwirl_Center;
			uniform float _Effect_Base_UVTwirl_Intensity;
			uniform float _Effect_Base_UVTwirl_ScrollSpeed;
			uniform float _Effect_Base_NoiseScale;
			uniform float _Effect_Base_ResultMul;
			uniform float _GlobalUV_ScrollSpeed;
			uniform float _Effect_FBM_Frequency;
			uniform float _Effect_FBM_Octaves;
			uniform float _Effect_FBM_Lacunarity;
			uniform float _Effect_FBM_Gain;
			uniform float _Effect_FBM_ResultMul;
			uniform float2 _Effect_Wave_PivotOffset;
			uniform float _Effect_Wave_ScrollSpeed;
			uniform float _Effect_Wave_Scale;
			uniform float _Effect_Wave_ResultPower;
			uniform float _Effect_Wave_ResultMul;
			uniform float _Effect_Scan_ResultMul;
			uniform float _Effect_Scan_ScrollSpeed;
			uniform float _Effect_Scan_UVWarpScale;
			uniform float _Effect_Scan_Scale;
			uniform float _Effect_Scan_ModeLerp;
			uniform float _Effect_Scan_ResultPower;
			uniform float _Effect_Mask_NoiseScale;
			uniform float2 _Effect_Mask_NoiseScrollSpeed;
			uniform float _Effect_Mask_PixelsNum;
			uniform float _Effect_Mask_ResultPower;
			uniform float _Effect_Mask_ResultMul;
			float random( float2 UV )
			{
				   return frac(sin(dot(UV, float2(12.9898,78.233)))*43758.5453123);
			}
			
			float valueNoise_KJ( float2 UV )
			{
				float2 i = floor(UV);
				float2 f = frac(UV);
				// Four corners in 2D of a tile
				float a = random(i);
				float b = random(i + float2(1.0, 0.0));
				float c = random(i + float2(0.0, 1.0));
				float d = random(i + float2(1.0, 1.0));
				float2 u = f * f * (3.0 - 2.0 * f);
				return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
			}
			
			float fbm( float2 UV, float octaves, float lacunarity, float gain )
			{
				// Initial values
				float result = 0.0;
				float amplitude = 0.5;
				float2 st = UV;
				//
				// Loop of octaves
				for (int i = 0; i < octaves; i++) {
				    result += amplitude * valueNoise_KJ(st);
				    st *= lacunarity;
				    amplitude *= gain;
				}
				return result;
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
					float2 voronoihash97( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi97( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -3; j <= 3; j++ )
						{
							for ( int i = -3; i <= 3; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash97( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						 		}
						 	}
						}
						return F1;
					}
			
					float2 voronoihash98( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi98( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -3; j <= 3; j++ )
						{
							for ( int i = -3; i <= 3; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash98( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						 		}
						 	}
						}
						return F1;
					}
			
			float4 NoiseGenerater_KJ3_g23( float2 UV, float Frequency, float octaves, float lacunarity, float gain )
			{
				float _fbmResult = fbm(UV*Frequency, octaves, lacunarity, gain);
				return float4(_fbmResult,_fbmResult,_fbmResult,1);
			}
			
			float4 NoiseGenerater_KJ3_g22( float2 UV, float Frequency, float octaves, float lacunarity, float gain )
			{
				float _fbmResult = fbm(UV*Frequency, octaves, lacunarity, gain);
				return float4(_fbmResult,_fbmResult,_fbmResult,1);
			}
			
			float4 NoiseGenerater_KJ3_g26( float2 UV, float Frequency, float octaves, float lacunarity, float gain )
			{
				float _fbmResult = fbm(UV*Frequency, octaves, lacunarity, gain);
				return float4(_fbmResult,_fbmResult,_fbmResult,1);
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
			
					float2 voronoihash195( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi195( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -3; j <= 3; j++ )
						{
							for ( int i = -3; i <= 3; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash195( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = max(abs(r.x), abs(r.y));
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						 		}
						 	}
						}
						return F2 - F1;
					}
			
					float2 voronoihash239( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi239( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -3; j <= 3; j++ )
						{
							for ( int i = -3; i <= 3; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash239( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.707 * sqrt(dot( r, r ));
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						 		}
						 	}
						}
						return F2;
					}
			


			v2f_img_custom vert_img_custom ( appdata_img_custom v  )
			{
				v2f_img_custom o;
				
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = float4( v.texcoord.xy, 1, 1 );

				#if UNITY_UV_STARTS_AT_TOP
					o.uv2 = float4( v.texcoord.xy, 1, 1 );
					o.stereoUV2 = UnityStereoScreenSpaceUVAdjust ( o.uv2, _MainTex_ST );

					if ( _MainTex_TexelSize.y < 0.0 )
						o.uv.y = 1.0 - o.uv.y;
				#endif
				o.stereoUV = UnityStereoScreenSpaceUVAdjust ( o.uv, _MainTex_ST );
				return o;
			}

			half4 frag ( v2f_img_custom i ) : SV_Target
			{
				#ifdef UNITY_UV_STARTS_AT_TOP
					half2 uv = i.uv2;
					half2 stereoUV = i.stereoUV2;
				#else
					half2 uv = i.uv;
					half2 stereoUV = i.stereoUV;
				#endif	
				
				half4 finalColor;

				// ase common template code
				float2 appendResult295 = (float2(( i.uv.xy.x * _TextureRatio ) , i.uv.xy.y));
				float2 appendResult294 = (float2(i.uv.xy.x , ( i.uv.xy.y * ( 1.0 / _TextureRatio ) )));
				float2 Local_BaseUV282 = ( _TextureRatio >= 1.0 ? appendResult295 : appendResult294 );
				float ScriptTime257 = _ScriptTime;
				float2 appendResult35 = (float2(0.0 , ( ScriptTime257 * _Effect_Base_UVWarp_ScrollSpeed )));
				float2 WarpUV83 = ( Local_BaseUV282 + appendResult35 );
				float simpleNoise25 = SimpleNoise( WarpUV83*_Effect_Base_UVWarp_NoiseScale );
				float simpleNoise26 = SimpleNoise( ( WarpUV83 + float2( 0.454,0.17785 ) )*_Effect_Base_UVWarp_NoiseScale );
				float2 appendResult28 = (float2(simpleNoise25 , simpleNoise26));
				float time97 = 0.0;
				float2 coords97 = ( WarpUV83 + float2( 1.258626,2.2585 ) ) * _Effect_Base_UVWarp2_NoiseScale;
				float2 id97 = 0;
				float2 uv97 = 0;
				float fade97 = 0.5;
				float voroi97 = 0;
				float rest97 = 0;
				for( int it97 = 0; it97 <2; it97++ ){
				voroi97 += fade97 * voronoi97( coords97, time97, id97, uv97, 0 );
				rest97 += fade97;
				coords97 *= 2;
				fade97 *= 0.5;
				}//Voronoi97
				voroi97 /= rest97;
				float time98 = 0.0;
				float2 coords98 = ( WarpUV83 + float2( 0.25196,0.2585 ) ) * _Effect_Base_UVWarp2_NoiseScale;
				float2 id98 = 0;
				float2 uv98 = 0;
				float fade98 = 0.5;
				float voroi98 = 0;
				float rest98 = 0;
				for( int it98 = 0; it98 <2; it98++ ){
				voroi98 += fade98 * voronoi98( coords98, time98, id98, uv98, 0 );
				rest98 += fade98;
				coords98 *= 2;
				fade98 *= 0.5;
				}//Voronoi98
				voroi98 /= rest98;
				float2 appendResult88 = (float2(voroi97 , voroi98));
				float2 lerpResult30 = lerp( WarpUV83 , ( ( appendResult28 + appendResult88 ) + Local_BaseUV282 ) , _Effect_Base_UVWarp_Intensity);
				float2 center45_g25 = _Effect_Base_UVTwirl_Center;
				float2 delta6_g25 = ( lerpResult30 - center45_g25 );
				float angle10_g25 = ( length( delta6_g25 ) * _Effect_Base_UVTwirl_Intensity );
				float x23_g25 = ( ( cos( angle10_g25 ) * delta6_g25.x ) - ( sin( angle10_g25 ) * delta6_g25.y ) );
				float2 break40_g25 = center45_g25;
				float2 appendResult24 = (float2(0.0 , ( ScriptTime257 * _Effect_Base_UVTwirl_ScrollSpeed )));
				float2 break41_g25 = appendResult24;
				float y35_g25 = ( ( sin( angle10_g25 ) * delta6_g25.x ) + ( cos( angle10_g25 ) * delta6_g25.y ) );
				float2 appendResult44_g25 = (float2(( x23_g25 + break40_g25.x + break41_g25.x ) , ( break40_g25.y + break41_g25.y + y35_g25 )));
				float simpleNoise38 = SimpleNoise( appendResult44_g25*_Effect_Base_NoiseScale );
				float2 appendResult61 = (float2(0.0 , ( ScriptTime257 * _GlobalUV_ScrollSpeed )));
				float2 GlobalUV63 = ( Local_BaseUV282 + appendResult61 );
				float2 UV3_g23 = GlobalUV63;
				float Frequency3_g23 = _Effect_FBM_Frequency;
				float octaves3_g23 = (float)(int)_Effect_FBM_Octaves;
				float lacunarity3_g23 = _Effect_FBM_Lacunarity;
				float gain3_g23 = _Effect_FBM_Gain;
				float4 localNoiseGenerater_KJ3_g23 = NoiseGenerater_KJ3_g23( UV3_g23 , Frequency3_g23 , octaves3_g23 , lacunarity3_g23 , gain3_g23 );
				float2 UV3_g22 = ( GlobalUV63 + float2( 0.454,0.157 ) );
				float Frequency3_g22 = _Effect_FBM_Frequency;
				float octaves3_g22 = (float)(int)_Effect_FBM_Octaves;
				float lacunarity3_g22 = _Effect_FBM_Lacunarity;
				float gain3_g22 = _Effect_FBM_Gain;
				float4 localNoiseGenerater_KJ3_g22 = NoiseGenerater_KJ3_g22( UV3_g22 , Frequency3_g22 , octaves3_g22 , lacunarity3_g22 , gain3_g22 );
				float2 appendResult140 = (float2(localNoiseGenerater_KJ3_g23.x , localNoiseGenerater_KJ3_g22.x));
				float2 UV3_g26 = ( GlobalUV63 + appendResult140 );
				float Frequency3_g26 = _Effect_FBM_Frequency;
				float octaves3_g26 = (float)(int)_Effect_FBM_Octaves;
				float lacunarity3_g26 = _Effect_FBM_Lacunarity;
				float gain3_g26 = _Effect_FBM_Gain;
				float4 localNoiseGenerater_KJ3_g26 = NoiseGenerater_KJ3_g26( UV3_g26 , Frequency3_g26 , octaves3_g26 , lacunarity3_g26 , gain3_g26 );
				float2 break48 = abs( (float2( -1,-1 ) + (( Local_BaseUV282 + _Effect_Wave_PivotOffset ) - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) );
				float2 temp_cast_6 = (( sqrt( ( pow( break48.x , 2.0 ) + pow( break48.y , 2.0 ) ) ) - ( ScriptTime257 * _Effect_Wave_ScrollSpeed ) )).xx;
				float simplePerlin2D54 = snoise( temp_cast_6*_Effect_Wave_Scale );
				simplePerlin2D54 = simplePerlin2D54*0.5 + 0.5;
				float saferPower78 = max( simplePerlin2D54 , 0.0001 );
				float2 temp_output_178_0 = abs( (float2( -1,-1 ) + (Local_BaseUV282 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) );
				float temp_output_265_0 = ( ScriptTime257 * _Effect_Scan_ScrollSpeed );
				float2 EffectScanUVScrollResult205 = ( temp_output_178_0 + temp_output_265_0 );
				float simpleNoise202 = SimpleNoise( ( EffectScanUVScrollResult205 + float2( 0.454,0.17785 ) )*_Effect_Scan_UVWarpScale );
				float EffectScanWarpUV210 = simpleNoise202;
				float2 temp_cast_7 = (( ( temp_output_178_0.x + temp_output_265_0 ) + EffectScanWarpUV210 )).xx;
				float simplePerlin2D184 = snoise( temp_cast_7*_Effect_Scan_Scale );
				simplePerlin2D184 = simplePerlin2D184*0.5 + 0.5;
				float time195 = 0.0;
				float2 coords195 = ( EffectScanUVScrollResult205 + EffectScanWarpUV210 ) * _Effect_Scan_Scale;
				float2 id195 = 0;
				float2 uv195 = 0;
				float voroi195 = voronoi195( coords195, time195, id195, uv195, 0 );
				float lerpResult193 = lerp( simplePerlin2D184 , voroi195 , _Effect_Scan_ModeLerp);
				float2 temp_output_269_0 = ( ScriptTime257 * _Effect_Mask_NoiseScrollSpeed );
				float time239 = temp_output_269_0.x;
				float pixelWidth235 =  1.0f / _Effect_Mask_PixelsNum;
				float pixelHeight235 = 1.0f / _Effect_Mask_PixelsNum;
				half2 pixelateduv235 = half2((int)(( Local_BaseUV282 + temp_output_269_0 ).x / pixelWidth235) * pixelWidth235, (int)(( Local_BaseUV282 + temp_output_269_0 ).y / pixelHeight235) * pixelHeight235);
				float2 coords239 = pixelateduv235 * _Effect_Mask_NoiseScale;
				float2 id239 = 0;
				float2 uv239 = 0;
				float voroi239 = voronoi239( coords239, time239, id239, uv239, 0 );
				float MaskResult240 = pow( voroi239 , _Effect_Mask_ResultPower );
				float lerpResult245 = lerp( 1.0 , MaskResult240 , _Effect_Mask_ResultMul);
				float4 temp_cast_9 = (( ( ( simpleNoise38 * _Effect_Base_ResultMul ) + ( localNoiseGenerater_KJ3_g26.x * _Effect_FBM_ResultMul ) + ( pow( saferPower78 , _Effect_Wave_ResultPower ) * _Effect_Wave_ResultMul ) + ( _Effect_Scan_ResultMul * pow( lerpResult193 , _Effect_Scan_ResultPower ) ) ) * lerpResult245 )).xxxx;
				

				finalColor = temp_cast_9;

				return finalColor;
			} 
			ENDCG 
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18703
2048;0;2048;1091;401.873;-260.2647;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;280;-7251.506,-131.7171;Inherit;False;Property;_TextureRatio;TextureRatio;32;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;290;-7038.111,157.253;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;283;-7041.96,-34.48953;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;293;-6790.96,264.5105;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;-6616.111,242.253;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;286;-6689.96,-59.48953;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;295;-6454.96,-14.4895;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;294;-6454.96,180.5105;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Compare;289;-6130.96,-131.4895;Inherit;False;3;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;256;-4763.242,790.1754;Inherit;False;Property;_ScriptTime;ScriptTime;30;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;257;-4544.128,790.079;Inherit;False;ScriptTime;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;282;-5941.96,-131.4895;Inherit;False;Local_BaseUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;258;-5151.621,753.6971;Inherit;False;257;ScriptTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;299;-3455.38,1976.541;Inherit;False;282;Local_BaseUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-5387.52,648.7907;Inherit;False;Property;_Effect_Base_UVWarp_ScrollSpeed;Effect_Base_UVWarp_ScrollSpeed;6;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-3125.277,2190.984;Inherit;False;Property;_Effect_Scan_ScrollSpeed;Effect_Scan_ScrollSpeed;19;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;177;-3182.484,1970.4;Inherit;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;-4965.621,774.6972;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;264;-2965.241,2274.171;Inherit;False;257;ScriptTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;260;-5219.63,153.814;Inherit;False;257;ScriptTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-5381.064,72.0863;Inherit;False;Property;_GlobalUV_ScrollSpeed;GlobalUV_ScrollSpeed;0;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;-5033.63,174.8141;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-4873.189,633.1498;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;265;-2779.241,2295.171;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;296;-4930.96,532.5105;Inherit;False;282;Local_BaseUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;178;-2990.382,1969.771;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-4681.908,609.4108;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;196;-2531.923,2170.948;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;271;-4133.421,929.2969;Inherit;False;Property;_Effect_Wave_PivotOffset;Effect_Wave_PivotOffset;31;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;301;-4948.495,-32.66043;Inherit;False;282;Local_BaseUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;61;-4890.737,55.4463;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;297;-4064.96,645.5105;Inherit;False;282;Local_BaseUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-4545.405,604.3779;Inherit;False;WarpUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-4699.454,31.7063;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;-2419.255,2163.32;Float;False;EffectScanUVScrollResult;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;270;-3724.038,744.7679;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-3231.869,-1456.494;Inherit;False;83;WarpUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-4563.705,23.4563;Inherit;False;GlobalUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;206;-3157.101,2476.245;Inherit;False;205;EffectScanUVScrollResult;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-3234.225,-1342.276;Inherit;False;83;WarpUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;46;-3445.205,649.6852;Inherit;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-3233.725,-1171.776;Inherit;False;83;WarpUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-3462.007,-1544.513;Inherit;False;Property;_Effect_Base_UVWarp_NoiseScale;Effect_Base_UVWarp_NoiseScale;8;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;51;-3381.205,825.6849;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-3014.28,-1165.262;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.25196,0.2585;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;201;-2852.254,2472.735;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.454,0.17785;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-3459.863,-1262.795;Inherit;False;Property;_Effect_Base_UVWarp2_NoiseScale;Effect_Base_UVWarp2_NoiseScale;9;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-3012.424,-1449.98;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.454,0.17785;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;214;-3000.842,2589.266;Inherit;False;Property;_Effect_Scan_UVWarpScale;Effect_Scan_UVWarpScale;17;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-3018.782,-1307.199;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;1.258626,2.2585;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-3069.369,-1566.994;Inherit;False;83;WarpUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;141;-2818.202,161.4688;Inherit;False;63;GlobalUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;202;-2705.867,2469.235;Inherit;False;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;48;-3477.205,889.6847;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NoiseGeneratorNode;25;-2861.037,-1567.48;Inherit;False;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;268;287.1152,621.3314;Inherit;False;257;ScriptTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;26;-2866.037,-1453.48;Inherit;False;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-2901.862,433.0799;Inherit;False;Property;_Effect_FBM_Octaves;Effect_FBM_Octaves;22;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;97;-2864.782,-1317.199;Inherit;True;2;0;1;0;2;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.Vector2Node;308;176.3903,697.4211;Inherit;False;Property;_Effect_Mask_NoiseScrollSpeed;Effect_Mask_NoiseScrollSpeed;27;0;Create;False;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;133;-2493.029,551.0295;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.454,0.157;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-2899.864,589.1237;Inherit;False;Property;_Effect_FBM_Gain;Effect_FBM_Gain;24;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-2899.514,357.8191;Inherit;False;Property;_Effect_FBM_Frequency;Effect_FBM_Frequency;21;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;98;-2860.782,-1043.199;Inherit;True;2;0;1;0;2;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;166;-2899.689,507.9498;Inherit;False;Property;_Effect_FBM_Lacunarity;Effect_FBM_Lacunarity;23;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;28;-2626.736,-1503.48;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;179;-2825.38,1968.868;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;269;533.1152,639.3315;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;-2628.592,-1218.762;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;168;-2351.982,189.0237;Inherit;True;NoiseGenerator_KJ;-1;;23;6234d8eaf02fdb445abd4d6c48da6d97;0;5;18;FLOAT2;0,0;False;22;FLOAT;0;False;19;INT;1;False;20;FLOAT;0;False;21;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;300;692.355,452.8303;Inherit;False;282;Local_BaseUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;210;-2520.101,2468.245;Float;False;EffectScanWarpUV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;43;-3237.206,761.6851;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;169;-2351.61,450.2768;Inherit;True;NoiseGenerator_KJ;-1;;22;6234d8eaf02fdb445abd4d6c48da6d97;0;5;18;FLOAT2;0,0;False;22;FLOAT;0;False;19;INT;1;False;20;FLOAT;0;False;21;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;42;-3237.206,633.6852;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;262;-2428.621,-136.3947;Inherit;False;257;ScriptTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;211;-2269.355,2543.72;Inherit;False;205;EffectScanUVScrollResult;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2586.04,-223.3909;Inherit;False;Property;_Effect_Base_UVTwirl_ScrollSpeed;Effect_Base_UVTwirl_ScrollSpeed;3;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;266;-3300.761,1554.972;Inherit;False;257;ScriptTime;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;136;-2072.567,258.757;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;252;976.6125,562.272;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-3093.205,681.6852;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;-2074.52,2092.069;Inherit;False;210;EffectScanWarpUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;298;-2513.695,-1067.838;Inherit;False;282;Local_BaseUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-3463.238,1462.603;Inherit;False;Property;_Effect_Wave_ScrollSpeed;Effect_Wave_ScrollSpeed;12;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-2351.782,-1244.199;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;246;803.6125,843.272;Inherit;False;Property;_Effect_Mask_PixelsNum;Effect_Mask_PixelsNum;26;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;212;-2265.355,2620.72;Inherit;False;210;EffectScanWarpUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-2531.896,2057.123;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;138;-2069.22,449.9732;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;204;-1817.855,2059.72;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-2181.051,-839.2973;Inherit;False;83;WarpUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;191;-1939.365,2248.93;Inherit;False;Property;_Effect_Scan_Scale;Effect_Scan_Scale;18;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;203;-1804.861,2540.872;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;-2242.621,-115.3946;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;267;-3114.761,1575.972;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;45;-3092.205,898.6847;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-2158.144,-1014.14;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCPixelate;235;1133.812,813.8276;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT;50;False;2;FLOAT;50;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;247;1117.613,1034.272;Inherit;False;Property;_Effect_Mask_NoiseScale;Effect_Mask_NoiseScale;28;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2550.469,-749.1421;Inherit;False;Property;_Effect_Base_UVWarp_Intensity;Effect_Base_UVWarp_Intensity;7;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;140;-1819.438,350.975;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;30;-1955.851,-833.5442;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;184;-1599.485,2053.27;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;137;-1622.874,171.7552;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VoronoiNode;239;1566.147,944.0306;Inherit;True;2;1;1;1;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;2;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.VoronoiNode;195;-1604.012,2387.894;Inherit;True;2;3;1;2;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;250;1464.613,1201.472;Inherit;False;Property;_Effect_Mask_ResultPower;Effect_Mask_ResultPower;29;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;192;-1322.909,2513.307;Inherit;False;Property;_Effect_Scan_ModeLerp;Effect_Scan_ModeLerp;15;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;18;-2216.452,-602.8822;Inherit;False;Property;_Effect_Base_UVTwirl_Center;Effect_Base_UVTwirl_Center;5;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;73;-2508.669,1050.913;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-2095.709,-241.0313;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2300.43,-472.9271;Inherit;False;Property;_Effect_Base_UVTwirl_Intensity;Effect_Base_UVTwirl_Intensity;4;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-2624.923,1244.62;Inherit;False;Property;_Effect_Wave_Scale;Effect_Wave_Scale;11;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-1637.252,968.8969;Inherit;False;Property;_Effect_Wave_ResultPower;Effect_Wave_ResultPower;13;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;193;-1287.909,2293.307;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1587.014,-392.1959;Inherit;False;Property;_Effect_Base_NoiseScale;Effect_Base_NoiseScale;2;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;54;-2157.987,1089.423;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-911.6371,1427.36;Inherit;False;Property;_Effect_Scan_ResultPower;Effect_Scan_ResultPower;16;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;16;-1573.632,-624.6327;Inherit;True;Twirl;-1;;25;90936742ac32db8449cd21ab6dd337c8;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;170;-1467.474,242.6993;Inherit;True;NoiseGenerator_KJ;-1;;26;6234d8eaf02fdb445abd4d6c48da6d97;0;5;18;FLOAT2;0,0;False;22;FLOAT;0;False;19;INT;1;False;20;FLOAT;0;False;21;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;249;1772.612,1049.472;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;186;-596.9417,1443.075;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;78;-1258.762,970.6948;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;173;-929.9067,-73.17261;Inherit;False;Property;_Effect_Base_ResultMul;Effect_Base_ResultMul;1;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;240;1967.228,1052.333;Inherit;False;MaskResult;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;38;-892.3016,-359.7709;Inherit;True;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;154;-1164.516,234.5128;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;82;-1372.88,1082.485;Inherit;False;Property;_Effect_Wave_ResultMul;Effect_Wave_ResultMul;10;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-1203.563,394.4741;Inherit;False;Property;_Effect_FBM_ResultMul;Effect_FBM_ResultMul;20;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;-916.6371,1329.36;Inherit;False;Property;_Effect_Scan_ResultMul;Effect_Scan_ResultMul;14;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;241;-470.7724,926.7325;Inherit;False;240;MaskResult;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-1056.452,971.228;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;244;-566.0029,1032.072;Inherit;False;Property;_Effect_Mask_ResultMul;Effect_Mask_ResultMul;25;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;-884.7763,186.4282;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-573.9067,-180.1726;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-349.6371,1333.36;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;245;-214.3876,906.8719;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-170.9917,716.9609;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;303;-3535.594,1777.353;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;181;-2787.854,2195.837;Inherit;False;1;0;FLOAT;-0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;77;-3145.788,1488.033;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;275;-3415.462,1132.619;Inherit;False;63;GlobalUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;305;630.8047,230.965;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;304;-4119.437,449.434;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;8;-2281.54,-215.8909;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;-9.7724,844.7325;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-5059.019,658.2907;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;307;-5015.203,329.2451;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;306;-2580.175,-977.4524;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;279;-1792.284,1790.709;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;302;-5112.025,-224.7373;Inherit;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;60;-5076.564,80.5863;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;278;-3156.01,1187.652;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.43;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;147.1021,844.4354;Float;False;True;-1;2;ASEMaterialInspector;0;3;Noise;c71b220b631b6344493ea3cf87110c93;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;True;2;False;-1;True;7;False;-1;False;True;0;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;True;0
WireConnection;293;1;280;0
WireConnection;291;0;290;2
WireConnection;291;1;293;0
WireConnection;286;0;283;1
WireConnection;286;1;280;0
WireConnection;295;0;286;0
WireConnection;295;1;283;2
WireConnection;294;0;290;1
WireConnection;294;1;291;0
WireConnection;289;0;280;0
WireConnection;289;2;295;0
WireConnection;289;3;294;0
WireConnection;257;0;256;0
WireConnection;282;0;289;0
WireConnection;177;0;299;0
WireConnection;259;0;258;0
WireConnection;259;1;33;0
WireConnection;261;0;260;0
WireConnection;261;1;59;0
WireConnection;35;1;259;0
WireConnection;265;0;264;0
WireConnection;265;1;190;0
WireConnection;178;0;177;0
WireConnection;36;0;296;0
WireConnection;36;1;35;0
WireConnection;196;0;178;0
WireConnection;196;1;265;0
WireConnection;61;1;261;0
WireConnection;83;0;36;0
WireConnection;62;0;301;0
WireConnection;62;1;61;0
WireConnection;205;0;196;0
WireConnection;270;0;297;0
WireConnection;270;1;271;0
WireConnection;63;0;62;0
WireConnection;46;0;270;0
WireConnection;51;0;46;0
WireConnection;92;0;89;0
WireConnection;201;0;206;0
WireConnection;27;0;85;0
WireConnection;95;0;90;0
WireConnection;202;0;201;0
WireConnection;202;1;214;0
WireConnection;48;0;51;0
WireConnection;25;0;84;0
WireConnection;25;1;32;0
WireConnection;26;0;27;0
WireConnection;26;1;32;0
WireConnection;97;0;95;0
WireConnection;97;2;91;0
WireConnection;133;0;141;0
WireConnection;98;0;92;0
WireConnection;98;2;91;0
WireConnection;28;0;25;0
WireConnection;28;1;26;0
WireConnection;179;0;178;0
WireConnection;269;0;268;0
WireConnection;269;1;308;0
WireConnection;88;0;97;0
WireConnection;88;1;98;0
WireConnection;168;18;141;0
WireConnection;168;22;164;0
WireConnection;168;19;165;0
WireConnection;168;20;166;0
WireConnection;168;21;167;0
WireConnection;210;0;202;0
WireConnection;43;0;48;1
WireConnection;169;18;133;0
WireConnection;169;22;164;0
WireConnection;169;19;165;0
WireConnection;169;20;166;0
WireConnection;169;21;167;0
WireConnection;42;0;48;0
WireConnection;136;0;168;0
WireConnection;252;0;300;0
WireConnection;252;1;269;0
WireConnection;44;0;42;0
WireConnection;44;1;43;0
WireConnection;96;0;28;0
WireConnection;96;1;88;0
WireConnection;185;0;179;0
WireConnection;185;1;265;0
WireConnection;138;0;169;0
WireConnection;204;0;185;0
WireConnection;204;1;213;0
WireConnection;203;0;211;0
WireConnection;203;1;212;0
WireConnection;263;0;262;0
WireConnection;263;1;19;0
WireConnection;267;0;266;0
WireConnection;267;1;76;0
WireConnection;45;0;44;0
WireConnection;22;0;96;0
WireConnection;22;1;298;0
WireConnection;235;0;252;0
WireConnection;235;1;246;0
WireConnection;235;2;246;0
WireConnection;140;0;136;0
WireConnection;140;1;138;0
WireConnection;30;0;86;0
WireConnection;30;1;22;0
WireConnection;30;2;31;0
WireConnection;184;0;204;0
WireConnection;184;1;191;0
WireConnection;137;0;141;0
WireConnection;137;1;140;0
WireConnection;239;0;235;0
WireConnection;239;1;269;0
WireConnection;239;2;247;0
WireConnection;195;0;203;0
WireConnection;195;2;191;0
WireConnection;73;0;45;0
WireConnection;73;1;267;0
WireConnection;24;1;263;0
WireConnection;193;0;184;0
WireConnection;193;1;195;0
WireConnection;193;2;192;0
WireConnection;54;0;73;0
WireConnection;54;1;75;0
WireConnection;16;1;30;0
WireConnection;16;2;18;0
WireConnection;16;3;17;0
WireConnection;16;4;24;0
WireConnection;170;18;137;0
WireConnection;170;22;164;0
WireConnection;170;19;165;0
WireConnection;170;20;166;0
WireConnection;170;21;167;0
WireConnection;249;0;239;0
WireConnection;249;1;250;0
WireConnection;186;0;193;0
WireConnection;186;1;187;0
WireConnection;78;0;54;0
WireConnection;78;1;79;0
WireConnection;240;0;249;0
WireConnection;38;0;16;0
WireConnection;38;1;4;0
WireConnection;154;0;170;0
WireConnection;81;0;78;0
WireConnection;81;1;82;0
WireConnection;171;0;154;0
WireConnection;171;1;172;0
WireConnection;174;0;38;0
WireConnection;174;1;173;0
WireConnection;189;0;188;0
WireConnection;189;1;186;0
WireConnection;245;1;241;0
WireConnection;245;2;244;0
WireConnection;80;0;174;0
WireConnection;80;1;171;0
WireConnection;80;2;81;0
WireConnection;80;3;189;0
WireConnection;181;0;190;0
WireConnection;77;0;76;0
WireConnection;8;0;19;0
WireConnection;242;0;80;0
WireConnection;242;1;245;0
WireConnection;34;0;33;0
WireConnection;60;0;59;0
WireConnection;278;0;275;0
WireConnection;0;0;242;0
ASEEND*/
//CHKSM=9A14089243F3B49CC86D4515F1535D0E6A21EE92