using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Planet : MonoBehaviour
{

    // Controls how much detail we want on our planet
    [Range(1, 256)]
    public int resolution = 90;

    /**
    *   Settings classes so we can easily 
    *   edit the planet in the inspector
    */
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    public ShapeGenerator shapeGenerator;

    // Planet mesh object
    [SerializeField, HideInInspector]
    GameObject meshObj;

    /**
    *   Calls all the generate methods to create the planet
    */
    public void GeneratePlanet()
    {
        GenerateMesh();
        GenerateColors();
    }

    /**
    *   This method can be called if only the mesh needs to be updated, not the color
    */
    public void OnShapeSettingsUpdated()
    {
        GenerateMesh();
        GenerateColors();

    }

    /**
    *   This method can be called if only the colors need to be updated
    */
    public void OnColorSettingsUpdated()
    {
        GenerateColors();
    }

    /**
    *   This methods initializes the game object, adds the MeshRenderer and MeshFilter
    *   component, creates an IcoSphere object and uses the IcoSphere.Generate() method
    *   where we send in the planet object mesh.
    */
    void GenerateMesh()
    {   
        shapeGenerator = new ShapeGenerator(shapeSettings);

        if(meshObj == null)
        {
            meshObj = new GameObject("Planet Mesh");
            meshObj.AddComponent<MeshRenderer>();
            meshObj.AddComponent<MeshFilter>();
            meshObj.transform.parent = transform;
        }

        meshObj.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

        //IcoSphere icoSphere = new IcoSphere();
        //icoSphere.Generate(shapeGenerator, meshObj, recursions);
        CubeSphere cubeSphere = new CubeSphere();
        cubeSphere.Generate(shapeGenerator, meshObj, resolution);
    }

    /**
    *   Adjusts the color of the planet mesh object.
    */
    void GenerateColors()
    {
        meshObj.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.color;
    }
}
