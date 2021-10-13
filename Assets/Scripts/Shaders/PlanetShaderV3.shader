Shader "Custom/PlanetShaderV3" 
{
	Properties 
	{
		_DiffuseMap ("Diffuse Map ", 2D)  = "white" {}
        _DiffuseMap2 ("Diffuse Map 2 ", 2D)  = "white" {}
        _DiffuseMap3 ("Diffuse Map 3 ", 2D)  = "white" {}
        _DiffuseMap4 ("Diffuse Map 4 ", 2D)  = "white" {}
        _OceanColor("Ocean Color", Color) = (1, 1, 1, 1)
		_texScale ("tex Scale",float) = 1
		_TriplanarBlendSharpness ("Blend Sharpness",float) = 1
        _PlanetRadius("Planet Radius", Float) = 1
        _PlanetCentre("Planet Centre", Vector) = (0, 0, 0)
        _MaxTerrainDist("Max dist", Float) = 1
        _MinTerrainDist("Min dist", Float) = 1
        _LowerMiddleValue("Lower Middle Value", Range(0.001, 0.999)) = 0.2
        _MiddleValue("Middle Value", Range(0.001, 0.999)) = 0.25
        _UpperMiddleValue("Upper Middle Value", Range(0.001, 0.999)) = 0.8

	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert

		sampler2D _DiffuseMap;
        sampler2D _DiffuseMap2;
		sampler2D _DiffuseMap3;
        sampler2D _DiffuseMap4;

        fixed4 _OceanColor;

		float _texScale;
		float _TriplanarBlendSharpness;
        float _PlanetRadius;
        float3 _PlanetCentre;
        float _MaxTerrainDist;
        float _MinTerrainDist;
        float _LowerMiddleValue;
        float _MiddleValue;
        float _UpperMiddleValue;

		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		}; 

        float calculate_vertex_dist_normalized(float3 worldPos)
        {
            float distance = abs(pow((worldPos.x - _PlanetCentre.x), 2) + pow((worldPos.y - _PlanetCentre.y), 2) + pow((worldPos.z - _PlanetCentre.z), 2));
            distance = sqrt(distance);
            distance -= _PlanetRadius;
            float inverse_lerped = (distance - _MinTerrainDist) / (_MaxTerrainDist - _MinTerrainDist);
            return inverse_lerped;
        }

		void surf (Input IN, inout SurfaceOutput o) 
		{
            float dist_normalized = calculate_vertex_dist_normalized(IN.worldPos);

			// Find our UVs for each axis based on world position of the fragment.
			half2 yUV = IN.worldPos.xz / _texScale;
			half2 xUV = IN.worldPos.zy / _texScale;
			half2 zUV = IN.worldPos.xy / _texScale;
			// Now do tex samples from our diffuse map with each of the 3 UV set's we've just made.

            half3 yDiff;
            half3 xDiff;
            half3 zDiff; 

            if(dist_normalized > _LowerMiddleValue && dist_normalized < _MiddleValue)
            {
                yDiff = tex2D (_DiffuseMap2, yUV);
                xDiff = tex2D (_DiffuseMap2, xUV);
                zDiff = tex2D (_DiffuseMap2, zUV);            
            }
            else if(dist_normalized > _MiddleValue && dist_normalized < _UpperMiddleValue)
            {
                yDiff = tex2D (_DiffuseMap3, yUV);
                xDiff = tex2D (_DiffuseMap3, xUV);
                zDiff = tex2D (_DiffuseMap3, zUV);            
            }
            else if(dist_normalized > _UpperMiddleValue)
            {
                yDiff = tex2D (_DiffuseMap4, yUV);
                xDiff = tex2D (_DiffuseMap4, xUV);
                zDiff = tex2D (_DiffuseMap4, zUV);
            }
			// Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			half3 blendWeights = pow (abs(IN.worldNormal), _TriplanarBlendSharpness);
			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
			// Finally, blend together all three samples based on the blend mask.
            if(dist_normalized < _LowerMiddleValue)
            {
                o.Albedo = _OceanColor;
            }else
            {
			    o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;
            }
		}
		ENDCG
	}
}