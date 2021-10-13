using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    NoiseSettings.RigidNoiseSettings noiseSettings;
    Noise noise = new Noise();

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings noiseSettings)
    {
        this.noiseSettings = noiseSettings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        
        // Controls how many changes occur along a unit length. Increasing the frequency
        // will increase the number of terrain features
        float frequency = noiseSettings.baseRoughness;
        
        // Controls the maximum absolute value that the noise value can have. 
        // Amplitude decreases for each layer that is added with a factor equal
        // to the persistence variable in NoiseSettings.
        float amplitude = 1;

        float weight = 1;

        for(int i = 0; i < noiseSettings.nLayers; i ++)
        {
            float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + noiseSettings.centre));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * noiseSettings.weightMultiplier); 

            noiseValue += v * amplitude;
            frequency *= noiseSettings.roughness;
            amplitude *= noiseSettings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue-noiseSettings.minValue);

        return noiseValue * noiseSettings.noiseStrength;
    }
}
