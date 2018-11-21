// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SSR"
{
	Properties
	{
		_MainTex ("Base(RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always Blend Off

		CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _CameraDepthTexture;
		sampler2D _CameraGBufferTexture2;
		float4x4 _InvViewProj;
		float4x4 _ViewProj;

		float4 _Params1;
		#define _RaytraceMaxLength        _Params1.x
		#define _RaytraceMaxThickness     _Params1.y
		#define _ReflectionEnhancer       _Params1.z
		#define _AccumulationBlendRatio   _Params1.w

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float4 screenPos : TEXCOORD0;
		};


		float ComputeDepth(float4 clippos)
		{
		#if defined(SHADER_TARGET_GLSL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
			return (clippos.z / clippos.w) * 0.5 + 0.5;
		#else
			return clippos.z / clippos.w;
		#endif
		}


		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			//座標位置をウィンドウの座標位置に変換
			o.screenPos = ComputeScreenPos(o.vertex);
			return o;
		}



		float4 frag(v2f i):SV_Target
		{
			float2 uv = i.screenPos.xy / i.screenPos.w;
			float4 col = tex2D(_MainTex, uv);

			//デプスバッファから深度値を取得
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
			//深度値が1以上ならテクスチャの色を返す
			if (depth >= 1.0) return col;

			//正規デバイス座標系に変換(-1～1)
			float2 spos = 2.0 * uv - 1.0;

			//スクリーン座標からビュー・プロジェクションに変換
			float4 pos = mul(_InvViewProj, float4(spos, depth, 1.0));

			//スクリーン座標に変換
			pos = pos / pos.w;

			//カメラの入射ベクトル
			float3 camDir = normalize(pos - _WorldSpaceCameraPos);

			//法線
			float3 normal = tex2D(_CameraGBufferTexture2, uv) * 2.0 - 1.0;

			//カメラの反射ベクトル
			float3 refDir =normalize(camDir - 2.0 * dot(camDir,normal) * normal);

			int maxRayNum = 100;
			//１ループで移動する距離
			float3 step = _RaytraceMaxLength / maxRayNum * refDir;
			for(int i = 1; i <= maxRayNum; ++i)
			{
				//少しレイを伸ばす
				float3 rayPos = pos + step * i;
				//飛ばした先の座標をクリップ空間に変換
				float4 vpPos = mul(_ViewProj, float4(rayPos, 1.0));
				//スクリーン座標に変換
				float2 rayuv = vpPos.xy / vpPos.w * 0.5 + 0.5;
				//正規デバイス座標系からUvに変換
			//	rayuv = rayuv * 0.5 + 0.5;

				//レイのuvの深度値
				float rayDepth = ComputeDepth(vpPos);
				//g-bufferの深度値
				float gbufferDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,rayuv);

				float _MaxThickness = _RaytraceMaxThickness / maxRayNum;
				if(rayDepth - gbufferDepth > 0 && rayDepth - gbufferDepth < _MaxThickness)
				{
					col += tex2D(_MainTex,rayuv) * 0.2;
					break;
				}
			}

			return col;
		}

		ENDCG


		Pass
		{
			CGPROGRAM
		    #pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
