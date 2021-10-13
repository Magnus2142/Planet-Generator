Shader "Custom/PlanetShaderV2"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _ColorTop ("Top Color", Color) = (1, 1, 1, 1)
        _ColorMid ("Mid Color", Color) = (1, 1, 1, 1)
        _ColorBot ("Bot Color", Color) = (1, 1, 1, 1)
        _Middle ("Middle", Range(0.001, 0.999)) = 0.240
        _PlanetRadius("Planet Radius", Float) = 1
        _PlanetCentre("Planet Centre", Vector) = (0, 0, 0)
        _MaxTerrainDist("Max dist", Float) = 1
        _MinTerrainDist("Min dist", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _ColorTop;
        fixed4 _ColorMid;
        fixed4 _ColorBot;
        float  _Middle;
        float _PlanetRadius;
        float3 _PlanetCentre;
        float _MaxTerrainDist;
        float _MinTerrainDist;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float calculate_vertex_dist_normalized(float3 worldPos)
        {
            //float distance = sqrt(pow((worldPos.x - _PlanetCentre.x), 2) + pow(worldPos.y - _PlanetCentre.y), 2) + pow(worldPos.z - _PlanetCentre.z), 2));
            float distance = pow((worldPos.x - _PlanetCentre.x), 2) + pow((worldPos.y - _PlanetCentre.y), 2) + pow((worldPos.z - _PlanetCentre.z), 2);
            distance = sqrt(distance);
            distance -= _PlanetRadius;
            float inverse_lerped = (distance - _MinTerrainDist) / (_MaxTerrainDist - _MinTerrainDist);
            return inverse_lerped;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float dist = calculate_vertex_dist_normalized(IN.worldPos);
            fixed4 c = lerp(_ColorBot, _ColorMid, dist / _Middle)
                * step(dist, _Middle);
            c += lerp(_ColorMid, _ColorTop, (dist - _Middle) / (1 - _Middle)) * step(_Middle, dist);

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
