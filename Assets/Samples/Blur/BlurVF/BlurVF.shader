Shader "Custom/BlurVF" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Diff("Diff", Range(0, 1)) = 0.1
    }

    SubShader{
        Pass{
            CGPROGRAM
            #pragma fragment frag
            #pragma vertex vert_img
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Diff;

            float4 frag(v2f_img i) :COLOR{
                float4 col = tex2D(_MainTex, i.uv - _Diff) + tex2D(_MainTex, i.uv + _Diff);
                return col / 2;
            }
            ENDCG
        }
    }
}