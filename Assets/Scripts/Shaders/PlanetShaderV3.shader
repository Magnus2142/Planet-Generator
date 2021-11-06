Shader "Custom/PlanetShaderV3" 
{
	Properties 
	{
		_DiffuseMap ("Diffuse Map ", 2D)  = "white" {}
		_NormalMap ("Normal Map ", 2D)  = "white" {}
        _DiffuseMap2 ("Diffuse Map 2 ", 2D)  = "white" {}
        _NormalMap2 ("Normal Map 2", 2D)  = "white" {}
        _DiffuseMap3 ("Diffuse Map 3 ", 2D)  = "white" {}
        _NormalMap3 ("Normal Map 3", 2D)  = "white" {}
        _DiffuseMap4 ("Diffuse Map 4 ", 2D)  = "white" {}
        _NormalMap4 ("Normal Map 4", 2D)  = "white" {}
        _DiffuseMap5 ("Diffuse Map 5 ", 2D)  = "white" {}
        _NormalMap5 ("Normal Map 5", 2D)  = "white" {}
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
        _UpperValue("Upper Value", Range(0.001, 0.999)) = 0.9


	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert

		sampler2D _DiffuseMap;
        sampler2D _NormalMap;
        sampler2D _DiffuseMap2;
        sampler2D _NormalMap2;
		sampler2D _DiffuseMap3;
        sampler2D _NormalMap3;
        sampler2D _DiffuseMap4;
        sampler2D _NormalMap4;
        sampler2D _DiffuseMap5;
        sampler2D _NormalMap5;


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
        float _UpperValue;

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

        // Reoriented Normal Mapping
        // http://blog.selfshadow.com/publications/blending-in-detail/
        // Altered to take normals (-1 to 1 ranges) rather than unsigned normal maps (0 to 1 ranges)
        float3 blend_rnm(float3 n1, float3 n2)
        {
            n1.z += 1;
            n2.xy = -n2.xy;

            return n1 * dot(n1, n2) / n1.z - n2;
        }

        // Sample normal map with triplan"Ocean mesh"ar coordinates
        // Returned normal will be in obj/world space (depending whether pos/normal are given in obj or world space)
        // Based on: medium.com/@bgolus/normal-mapping-for-a-triplanar-shader-10bf39dca05a
        float3 triplanarNormal(float3 vertPos, float3 normal, float3 scale, float2 offset, sampler2D normalMap) {
            float3 absNormal = abs(normal);

            // Calculate triplanar blend
            float3 blendWeight = saturate(pow(normal, 4));
            // Divide blend weight by the sum of its components. This will make x + y + z = 1
            blendWeight /= dot(blendWeight, 1);

            // Calculate triplanar coordinates
            float2 uvX = vertPos.zy * scale + offset;
            float2 uvY = vertPos.xz * scale + offset;
            float2 uvZ = vertPos.xy * scale + offset;

            // Sample tangent space normal maps
            // UnpackNormal puts values in range [-1, 1] (and accounts for DXT5nm compression)
            float3 tangentNormalX = UnpackNormal(tex2D(normalMap, uvX));
            float3 tangentNormalY = UnpackNormal(tex2D(normalMap, uvY));
            float3 tangentNormalZ = UnpackNormal(tex2D(normalMap, uvZ));

            // Swizzle normals to match tangent space and apply reoriented normal mapping blend
            tangentNormalX = blend_rnm(half3(normal.zy, absNormal.x), tangentNormalX);
            tangentNormalY = blend_rnm(half3(normal.xz, absNormal.y), tangentNormalY);
            tangentNormalZ = blend_rnm(half3(normal.xy, absNormal.z), tangentNormalZ);

            // Apply input normal sign to tangent space Z
            float3 axisSign = sign(normal);
            tangentNormalX.z *= axisSign.x;
            tangentNormalY.z *= axisSign.y;
            tangentNormalZ.z *= axisSign.z;

            // Swizzle tangent normals to match input normal and blend together
            float3 outputNormal = normalize(
                tangentNormalX.zyx * blendWeight.x +
                tangentNormalY.xzy * blendWeight.y +
                tangentNormalZ.xyz * blendWeight.z
            );

            return outputNormal;
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
            float3 lightingNormal;

            if(dist_normalized < _MiddleValue)
            {
                yDiff = tex2D (_DiffuseMap2, yUV);
                xDiff = tex2D (_DiffuseMap2, xUV);
                zDiff = tex2D (_DiffuseMap2, zUV);   
                lightingNormal = triplanarNormal(IN.worldPos, IN.worldNormal, _texScale, 0, _NormalMap2);
            }
            else if(dist_normalized > _MiddleValue && dist_normalized < _UpperMiddleValue)
            {
                yDiff = tex2D (_DiffuseMap3, yUV);
                xDiff = tex2D (_DiffuseMap3, xUV);
                zDiff = tex2D (_DiffuseMap3, zUV);
                lightingNormal = triplanarNormal(IN.worldPos, IN.worldNormal, _texScale, 0, _NormalMap3);
            
            }
            else if(dist_normalized > _UpperMiddleValue && dist_normalized < _UpperValue)
            {
                yDiff = tex2D (_DiffuseMap4, yUV);
                xDiff = tex2D (_DiffuseMap4, xUV);
                zDiff = tex2D (_DiffuseMap4, zUV);
                lightingNormal = triplanarNormal(IN.worldPos, IN.worldNormal, _texScale, 0, _NormalMap4);

            }
            else if(dist_normalized > _UpperValue)
            {
                yDiff = tex2D (_DiffuseMap5, yUV);
                xDiff = tex2D (_DiffuseMap5, xUV);
                zDiff = tex2D (_DiffuseMap5, zUV);
                lightingNormal = triplanarNormal(IN.worldPos, IN.worldNormal, _texScale, 0, _NormalMap5);
            }
			// Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			half3 blendWeights = pow (abs(IN.worldNormal), _TriplanarBlendSharpness);
			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
			// Finally, blend together all three samples based on the blend mask.
            
			o.Albedo = xDiff * blendWeights.x + yDiff * blendWeights.y + zDiff * blendWeights.z;

            float lightShading = saturate(dot(lightingNormal, _WorldSpaceLightPos0.xyz));
            o.Albedo = o.Albedo * lightShading;
		}
		ENDCG
	}
}