// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "CustomStandard"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_BumpAmt("Distortion", Range(0,128)) = 10
		_Size("Size", Range(0, 20)) = 1

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

		CGINCLUDE
#define UNITY_SETUP_BRDF_INPUT MetallicSetup
		ENDCG

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Sprite" "AlphaDepth" = "False" "CanUseSpriteAtlas" = "True" "IgnoreProjector" = "True" }
		LOD 300

		// Horizontal blur
		GrabPass{
			Tags { "LightMode" = "Always" }
		}

		Pass
		{
			Tags { "LightMode" = "Always" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 uvgrab : TEXCOORD0;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
#else
				float scale = 1.0;
#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				return o;
			}

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Size;

			half4 frag(v2f i) : COLOR {
				//                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				//                  return col;

				half4 sum = half4(0,0,0,0);

				#define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight

				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18,  0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);

				return sum;
			}

			ENDCG
		}

		// Vertical blur
		GrabPass
		{
			Tags { "LightMode" = "Always" }
		}

		Pass
		{
			Tags { "LightMode" = "Always" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 uvgrab : TEXCOORD0;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				return o;
			}

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _Size;

			half4 frag(v2f i) : COLOR {
				//                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				//                  return col;

				half4 sum = half4(0,0,0,0);

				#define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight

				//G(X) = (1/(sqrt(2*PI*deviation*deviation))) * exp(-(x*x / (2*deviation*deviation)))

				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18,  0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);

				return sum;
			}

			ENDCG
		}

		// Distortion
		GrabPass
		{
			Tags { "LightMode" = "Always" }
		}

		Pass
		{
			Tags { "LightMode" = "Always" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 uvgrab : TEXCOORD0;
				float2 uvbump : TEXCOORD1;
				float2 uvmain : TEXCOORD2;
			};

			float _BumpAmt;
			float4 _BumpMap_ST;
			float4 _MainTex_ST;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
#else
				float scale = 1.0;
#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);
				o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 _Color;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			sampler2D _BumpMap;
			sampler2D _MainTex;

			half4 frag(v2f i) : COLOR{
				// calculate perturbed coordinates
				half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump)).rg; // we could optimize this by just reading the x  y without reconstructing the Z
				float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
				i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

				half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				half4 tint = tex2D(_MainTex, i.uvmain) * _Color;

				return col * tint;
			}

			ENDCG
		}

			// ------------------------------------------------------------------
			//  Base forward pass (directional light, emission, lightmaps, ...)
			Pass
			{
				Name "FORWARD"
				Tags { "LightMode" = "ForwardBase" }

				Blend[_SrcBlend][_DstBlend]
				ZWrite[_ZWrite]

				CGPROGRAM
				#pragma target 3.0

				// -------------------------------------

				#pragma shader_feature _NORMALMAP
				#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature _EMISSION
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _DETAIL_MULX2
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
				#pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
				#pragma shader_feature_local _PARALLAXMAP

				#pragma multi_compile_fwdbase
				#pragma multi_compile_fog
				#pragma multi_compile_instancing
				// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
				//#pragma multi_compile _ LOD_FADE_CROSSFADE

				#pragma vertex vertBase
				#pragma fragment fragBase
				#include "UnityStandardCoreForward.cginc"

				ENDCG
			}
				// ------------------------------------------------------------------
				//  Additive forward pass (one light per pass)
				Pass
				{
					Name "FORWARD_DELTA"
					Tags { "LightMode" = "ForwardAdd" }
					Blend[_SrcBlend] One
					Fog { Color(0,0,0,0) } // in additive pass fog should be black
					ZWrite Off
					ZTest LEqual

					CGPROGRAM
					#pragma target 3.0

				// -------------------------------------


				#pragma shader_feature _NORMALMAP
				#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
				#pragma shader_feature_local _DETAIL_MULX2
				#pragma shader_feature_local _PARALLAXMAP

				#pragma multi_compile_fwdadd_fullshadows
				#pragma multi_compile_fog
				// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
				//#pragma multi_compile _ LOD_FADE_CROSSFADE

				#pragma vertex vertAdd
				#pragma fragment fragAdd
				#include "UnityStandardCoreForward.cginc"

				ENDCG
			}
				// ------------------------------------------------------------------
				//  Shadow rendering pass
				Pass {
					Name "ShadowCaster"
					Tags { "LightMode" = "ShadowCaster" }

					ZWrite On ZTest LEqual

					CGPROGRAM
					#pragma target 3.0

				// -------------------------------------


				#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _PARALLAXMAP
				#pragma multi_compile_shadowcaster
				#pragma multi_compile_instancing
				// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
				//#pragma multi_compile _ LOD_FADE_CROSSFADE

				#pragma vertex vertShadowCaster
				#pragma fragment fragShadowCaster

				#include "UnityStandardShadow.cginc"

				ENDCG
			}
				// ------------------------------------------------------------------
				//  Deferred pass
				Pass
				{
					Name "DEFERRED"
					Tags { "LightMode" = "Deferred" }

					CGPROGRAM
					#pragma target 3.0
					#pragma exclude_renderers nomrt


				// -------------------------------------

				#pragma shader_feature _NORMALMAP
				#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature _EMISSION
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
				#pragma shader_feature_local _DETAIL_MULX2
				#pragma shader_feature_local _PARALLAXMAP

				#pragma multi_compile_prepassfinal
				#pragma multi_compile_instancing
				// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
				//#pragma multi_compile _ LOD_FADE_CROSSFADE

				#pragma vertex vertDeferred
				#pragma fragment fragDeferred

				#include "UnityStandardCore.cginc"

				ENDCG
				}

				// ------------------------------------------------------------------
				// Extracts information for lightmapping, GI (emission, albedo, ...)
				// This pass it not used during regular rendering.
				Pass
				{
					Name "META"
					Tags { "LightMode" = "Meta" }

					Cull Off

					CGPROGRAM
					#pragma vertex vert_meta
					#pragma fragment frag_meta

					#pragma shader_feature _EMISSION
					#pragma shader_feature_local _METALLICGLOSSMAP
					#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
					#pragma shader_feature_local _DETAIL_MULX2
					#pragma shader_feature EDITOR_VISUALIZATION

					#include "UnityStandardMeta.cginc"
					ENDCG
				}


		}

			FallBack "VertexLit"
				CustomEditor "CustomStandardGUI"
}
