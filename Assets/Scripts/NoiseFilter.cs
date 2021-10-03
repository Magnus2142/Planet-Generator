using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Find other ways to implement noise with these or other settings?
public class NoiseFilter
{
    NoiseSettings noiseSettings;
    Noise noise = new Noise(Random.Range(0, 100));

    public NoiseFilter(NoiseSettings noiseSettings)
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

        for(int i = 0; i < noiseSettings.nLayers; i ++)
        {
            float v = noise.Evaluate(point * frequency + noiseSettings.centre);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= noiseSettings.roughness;
            amplitude *= noiseSettings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue-noiseSettings.minValue);

        return noiseValue * noiseSettings.noiseStrength;
    }
}
