using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*   A class to hold the settings variables concerning the planets
*   shape.
*/

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject
{
    // Adjusts this to control the radius of the planet
    public float radius = 1;
    // Holds all the settings for the noise filters like noise strength, roughness etc
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }
}
