Shader "MyShader/SH_SetArea"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0,0.1)) = 0.02
    }
    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "Queue" = "Transparent+1"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float edgeX = min(i.uv.x, 1.0-i.uv.x)*2;
                float edgeY = min(i.uv.y, 1.0-i.uv.y);
                float edgeDistance = min(edgeX-_OutlineThickness, edgeY-_OutlineThickness);
                if (edgeDistance < 0){
                    return col*_OutlineColor;
                }else{
                    return float4(0,0,0,0);
                }
            }
            ENDCG
        }
    }
}
