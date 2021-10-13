using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Is it other ways to implement this noise? Other settings etc...

[System.Serializable]
public class NoiseSettings
{

    public enum FilterType { Basic, Rigid };
    public FilterType filterType;

    [ConditionalHideAttribute("filterType", 0)]
    public BasicNoiseSettings basicNoiseSettings;
    [ConditionalHideAttribute("filterType", 1)]
    public RigidNoiseSettings rigidNoiseSettings;

    [System.Serializable]
    public class BasicNoiseSettings
    {
        // Strength of the noise
        public float noiseStrength = 1;

        // How many layers of noise we want
        [Range(1, 8)]
        public int nLayers = 1;

        public float baseRoughness = 1;


        public float roughness = 2;

        // Determines how fast the amplitude diminish for each layer
        public float persistence = 0.5f;

        // Controls where the centre of the noise is
        public Vector3 centre;

        public float minValue;
    }

    [System.Serializable]
    public class RigidNoiseSettings : BasicNoiseSettings
    {
        public float weightMultiplier = 0.8f;

    }
}
