using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings noiseSettings)
    {
        switch(noiseSettings.filterType)
        {
            case NoiseSettings.FilterType.Basic:
                return new BasicNoiseFilter(noiseSettings.basicNoiseSettings);
            case NoiseSettings.FilterType.Rigid:
                return new RigidNoiseFilter(noiseSettings.rigidNoiseSettings);
        }
        return null;
    }
}
