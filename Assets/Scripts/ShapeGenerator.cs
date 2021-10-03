using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings shapeSettings;
    NoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings shapeSettings)
    {
        this.shapeSettings = shapeSettings;
        noiseFilters = new NoiseFilter[shapeSettings.noiseLayers.Length];
        for(int i = 0; i < noiseFilters.Length; i ++)
        {
            noiseFilters[i] = new NoiseFilter(shapeSettings.noiseLayers[i].noiseSettings);
        }
    }

    

    /**
    *   Here we take in the points (Vector3 coordinates) that forms
    *   the unity sphere which means a sphere of a radius one around the
    *   given center. Then we use the information from the ShapeSettings
    *   class to adjust the radius, noise to generate terrain etc.
    *   Calculates this here to avoid making the IcoSphere class very complicated.
    */
    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {

        float firstLayerValue = 0;
        float elevation = 0;

        if(noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if(shapeSettings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        for(int i = 1; i < noiseFilters.Length; i ++)
        {   
            if(shapeSettings.noiseLayers[i].enabled)
            {
                float mask = (shapeSettings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask; 
            }
        }

        return pointOnUnitSphere * shapeSettings.radius * (1 + elevation);
    }
}