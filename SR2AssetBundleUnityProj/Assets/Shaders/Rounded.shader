Shader "UI/SR2E/Rounded"
{
    Properties
    {
        [HideInInspector]_MainTex("Texture", 2D) = "white" {}
        _CornerRadius("Corner Radius", Float) = 20
        _HalfSize("Half Size", Vector) = (0,0,0,0)
        _OuterUV("Outer UV", Vector) = (0,0,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        ZTest [unity_GUIZTestMode]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float _CornerRadius;
            float4 _HalfSize;
            float4 _OuterUV;
            fixed4 _TextureSampleAdd;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // Perfect rounded corners with smooth anti-alias
            float RoundCornerAlpha(float2 p, float2 halfSize, float radius)
            {
                float2 d = abs(p) - (halfSize - radius);
                float dist = length(max(d, 0)) + min(max(d.x, d.y), 0.0) - radius;
                return saturate(1.0 - dist);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = (i.uv - _OuterUV.xy) / (_OuterUV.zw - _OuterUV.xy);
                float2 pos = uv * 2.0 - 1.0; // normalized -1..1
                pos *= _HalfSize.xy;

                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float alpha = RoundCornerAlpha(pos, _HalfSize.xy, _CornerRadius);
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}
