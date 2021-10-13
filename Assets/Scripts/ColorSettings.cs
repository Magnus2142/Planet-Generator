using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*   A class to hold the settings to control
*   the color settings/theme of the planets
*/

[CreateAssetMenu]
public class ColorSettings : ScriptableObject
{
    public Color color;
    public Material planetMaterial;
}
