Shader "MyShader/SH_Model"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
        _ShadeColor ("ShadeColor", Color) = (1,1,1,1)
        _ShadeRange ("ShadeRange", float) = 1.0 //影色にする範囲
        _ColorIntense ("ColorIntense", Range(0,1)) = 1.0 //影色の彩度みたいなイメージ
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Name "ColorShade"
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            // 影を受ける
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            /////

            //デフォ
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                half3 normal : NORMAL; //法線
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                half3 normal : TEXCOORD2; //法線
                //float4 vertex : SV_POSITION;
                float4 pos : SV_POSITION; //影を受ける
                SHADOW_COORDS(1) // 影を受ける
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex); //影を受ける時posじゃないとだめ
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);//法線
                UNITY_TRANSFER_FOG(o,o.vertex);
                TRANSFER_SHADOW(o) //影を受ける
                return o;
            }

            fixed4 _ShadeColor;
            fixed4 _BaseColor;
            fixed _ShadeRange;
            fixed _ColorIntense;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //影色の処理追加
                _ShadeRange = max(0, _ShadeRange);
                float diff = dot(normalize(i.normal), normalize(_WorldSpaceLightPos0.xyz));
                diff *= _ShadeRange + (1 - _ShadeRange);
                diff *= SHADOW_ATTENUATION(i); //影を受ける
                diff = smoothstep(0,1,diff);
                
                fixed4 shade = lerp((0,0,0,0), _ShadeColor, _ColorIntense);
                col *= lerp(shade, _BaseColor, diff);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

        //影を落とすパス
        Name "ShadowCast"
        Pass
        {
            Tags{ "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        /////
    }
}