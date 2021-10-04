Shader "Unlit/PlanetShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTop ("Top Color", Color) = (1,1,1,1)
        //_ColorMid ("Mid Color", Color) = (1,1,1,1)
        _ColorBot ("Bot Color", Color) = (1,1,1,1)
        //_Middle ("Middle", Range(0.001, 0.999)) = 1
        _PlanetRadius("Planet Radius", Float) = 1
        _PlanetCentre("Planet Centre", Vector) = (0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _ColorTop;
            //fixed4 _ColorMid;
            fixed4 _ColorBot;
            //float  _Middle;
            float _PlanetRadius;
            float3 _PlanetCenter;

            struct v2f
            {
                float4 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            float4 frag (v2f i) : COLOR
            {
                fixed4 c = lerp(_ColorBot, _ColorTop, 0.5);
                //c += lerp(_ColorMid, _ColorTop, (i.texcoord.y - _Middle) / (1 - _Middle)) * step(_Middle, i.texcoord.y);
                c.a = 1;
             return c;
            }
            ENDCG
        }
    }
}
