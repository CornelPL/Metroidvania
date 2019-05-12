Shader "MyShaders/Blur"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_BumpMap("Normalmap", 2D) = "bump" {}
		_BumpIntensity("NormalMap Intensity", Float) = 1
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		_BlurSize("Blur Size", Float) = 1
	}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}
			LOD 300
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting On
			ZWrite On
			Fog { Mode Off }
			CGPROGRAM
			#pragma target 3.0
			#pragma surface surf Lambert alpha:fade vertex:vert fullforwardshadows
			#pragma multi_compile DUMMY PIXELSNAP_ON 
			#pragma exclude_renderers flash
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _BumpMap;
			float4 _BumpMap_TexelSize;
			fixed _BumpIntensity;
			float _BlurSize;

			fixed4 _Color;
			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
				fixed4 color;
			};

			void vert(inout appdata_full v, out Input o)
			{
				#if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
				v.vertex = UnityPixelSnap(v.vertex);
				#endif
				v.normal = float3(0,0,-1);
				v.tangent = float4(1, 0, 0, 1);
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.color = _Color;
			}
			void surf(Input IN, inout SurfaceOutput o)
			{
				#define GRABPIXELY(weight, kernely) tex2D( _MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y + _MainTex_TexelSize.y * kernely * _BlurSize)) * weight
				#define GRABPIXELX(weight, kernelx) tex2D( _MainTex, float2(IN.uv_MainTex.x + _MainTex_TexelSize.x * kernelx * _BlurSize, IN.uv_MainTex.y)) * weight
				#define GRABNORMALPIXELX(weight, kernelx) UnpackNormal(tex2D( _BumpMap, float2(IN.uv_BumpMap.x + _BumpMap_TexelSize.x * kernelx * _BlurSize, IN.uv_BumpMap.y))) * weight
				#define GRABNORMALPIXELY(weight, kernely) UnpackNormal(tex2D( _BumpMap, float2(IN.uv_BumpMap.x, IN.uv_MainTex.y + _BumpMap_TexelSize.y * kernely * _BlurSize))) * weight
				
				half4 sum = half4(0, 0, 0, 0);

				sum += GRABPIXELY(0.05, -4.0);
				sum += GRABPIXELY(0.09, -3.0);
				sum += GRABPIXELY(0.12, -2.0);
				sum += GRABPIXELY(0.15, -1.0);
				sum += GRABPIXELY(0.18, 0.0);
				sum += GRABPIXELY(0.15, +1.0);
				sum += GRABPIXELY(0.12, +2.0);
				sum += GRABPIXELY(0.09, +3.0);
				sum += GRABPIXELY(0.05, +4.0);

				sum += GRABPIXELX(0.05, -4.0);
				sum += GRABPIXELX(0.09, -3.0);
				sum += GRABPIXELX(0.12, -2.0);
				sum += GRABPIXELX(0.15, -1.0);
				sum += GRABPIXELX(0.18, 0.0);
				sum += GRABPIXELX(0.15, +1.0);
				sum += GRABPIXELX(0.12, +2.0);
				sum += GRABPIXELX(0.09, +3.0);
				sum += GRABPIXELX(0.05, +4.0);

				//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
				o.Albedo = sum.rgb;
				o.Alpha = sum.a * IN.color.a;

				half3 suma = half3(0, 0, 0);

				suma += GRABNORMALPIXELX(0.05, -4.0);
				suma += GRABNORMALPIXELX(0.09, -3.0);
				suma += GRABNORMALPIXELX(0.12, -2.0);
				suma += GRABNORMALPIXELX(0.15, -1.0);
				suma += GRABNORMALPIXELX(0.18, 0.0);
				suma += GRABNORMALPIXELX(0.15, +1.0);
				suma += GRABNORMALPIXELX(0.12, +2.0);
				suma += GRABNORMALPIXELX(0.09, +3.0);
				suma += GRABNORMALPIXELX(0.05, +4.0);

				suma += GRABNORMALPIXELY(0.05, -4.0);
				suma += GRABNORMALPIXELY(0.09, -3.0);
				suma += GRABNORMALPIXELY(0.12, -2.0);
				suma += GRABNORMALPIXELY(0.15, -1.0);
				suma += GRABNORMALPIXELY(0.18, 0.0);
				suma += GRABNORMALPIXELY(0.15, +1.0);
				suma += GRABNORMALPIXELY(0.12, +2.0);
				suma += GRABNORMALPIXELY(0.09, +3.0);
				suma += GRABNORMALPIXELY(0.05, +4.0);

				//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				o.Normal = suma.xyz;
				_BumpIntensity = 1 / _BumpIntensity;
				o.Normal.z = o.Normal.z * _BumpIntensity;
				o.Normal = normalize((half3)o.Normal);
			}
			ENDCG
		}
			Fallback "Transparent/Cutout/Diffuse"
}