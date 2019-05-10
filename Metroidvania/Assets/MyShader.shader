Shader "MyShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		[Normal] _NormalMap("NormalMap", 2D) = "bump" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"

			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;			// ST is for Scale and Translation - Tiling and Offset of the texture
			sampler2D _NormalMap;
			float4 _NormalMap_ST;


			struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;			// TEXCOORD0 just because everyone uses it for interpolation
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct Interpolator {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float3 tbn[3] : TEXCOORD1;
			};


			Interpolator MyVertexProgram(VertexData v)
			{
				Interpolator i;
				i.position = UnityObjectToClipPos(v.position);		// model will render in it's actual position
				i.uv = TRANSFORM_TEX(v.uv, _NormalMap);

				float3 tangent = UnityObjectToWorldNormal(v.tangent);
				float3 normal = UnityObjectToWorldNormal(v.normal);
				float3 bitangent = cross(tangent, normal);

				i.tbn[0] = tangent;
				i.tbn[1] = bitangent;
				i.tbn[2] = normal;

				return i;
			}


			float4 MyFragmentProgram(Interpolator i) : SV_TARGET							// default shader target - frame buffer
			{
				float3 tangentNormal = tex2D(_NormalMap, i.uv) * 2 - 1;
				float3 worldNormal = float3(i.tbn[0] * tangentNormal.r + i.tbn[1] * tangentNormal.g + i.tbn[2] * tangentNormal.b);
				return saturate(dot(worldNormal, _WorldSpaceLightPos0));
			}

			ENDCG
		}
	}
}