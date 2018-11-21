Shader "DoF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

		// メインテクスチャ
        sampler2D _MainTex;
		// ぼかしたテクスチャ
		sampler2D _BlurTex;
		// デプスバッファ
        sampler2D _CameraDepthTexture;
        float4 _MainTex_ST;
		// 正規化した1ピクセルのサイズ
        float4 _MainTex_TexelSize;
		// カメラからのピントを合わせる距離
        float _Foucus;


        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        // メインテクスチャからサンプリングしてRGBのみ返す
        half3 sampleMain(float2 uv){
            return tex2D(_MainTex, uv).rgb;
        }

        // 対角線上の4点からサンプリングした色の平均値を返す
        half3 sampleBox (float2 uv, float delta) {
            float4 offset = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
            half3 sum = sampleMain(uv + offset.xy) + sampleMain(uv + offset.zy) + sampleMain(uv + offset.xw) + sampleMain(uv + offset.zw);
            return sum * 0.25;
        }

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv,_MainTex);
            return o;
        }

        ENDCG


        Cull Off
        ZTest Always
        ZWrite Off

        Tags { "RenderType"="Opaque" }

        // 0: ダウンサンプリング用のパス
        Pass
        {
            CGPROGRAM

            // ダウンサンプリング時には1ピクセル分ずらした対角線上の4点からサンプリング
            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = 1;
                col.rgb = sampleBox(i.uv, 1.0);
                return col;
            }

            ENDCG
        }

        // 1:アップサンプリング用のパス
        Pass
        {

            CGPROGRAM
            // 1: アップサンプリング時には0.5ピクセル分ずらした対角線上の4点からサンプリング
            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = 1;
                col.rgb = sampleBox(i.uv, 0.5);
                return col;
            }
            ENDCG
        }

        // 2:被写界深度
        Pass
        {
            CGPROGRAM
            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 outColor;
				// メインテクスチャのカラー
				fixed4 mainColor = tex2D(_MainTex, i.uv);
				// ぼかしたテクスチャのカラー
				fixed4 blurColor = tex2D(_BlurTex, i.uv);
                fixed depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,i.uv));

				//ピントが合っているか
                if(depth <= _Foucus)
                {
					outColor  = mainColor;
                }
				else
				{
					float ratio = saturate((depth - _Foucus) * 0.5);
					outColor = lerp(mainColor , blurColor , ratio);
				}

				//outColor.a = 1;

				return outColor;
            }
            ENDCG
        }

    }
}
