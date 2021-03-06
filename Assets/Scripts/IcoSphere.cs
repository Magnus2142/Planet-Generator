using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
*   This is the code for generating the vertices and triangles to create the planet mesh.
*   Code is fetched from this github: https://github.com/Pouchmouse/Procedural-Planet-Tutorial/blob/Tutorial1-IcosahedronGeneration/Assets/Scripts/Planet.cs
*   Most of the code is the same, except from some adjustments to fit my own project.
*/

public class IcoSphere
{
    /*

    ShapeGenerator shapeGenerator;
    List<Polygon> m_Polygons;
    List<Vector3> m_Vertices;

    float maxTerrainHeight;
    float minTerrainHeight;

    public void Generate(ShapeGenerator shapeGenerator, GameObject meshObj, int nRecursions)
    {
        this.shapeGenerator = shapeGenerator;
        InitAsIcosohedron();
        Subdivide(nRecursions);
        GenerateMesh(meshObj);
    }

    public void InitAsIcosohedron()
    {
        m_Polygons = new List<Polygon>();
        m_Vertices = new List<Vector3>();

        // An icosahedron has 12 vertices, and
        // since they're completely symmetrical the
        // formula for calculating them is kind of
        // symmetrical too:

        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(-1, t, 0).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(1, t, 0).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(-1, -t, 0).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(1, -t, 0).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(0, -1, t).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(0, 1, t).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(0, -1, -t).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(0, 1, -t).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(t, 0, -1).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(t, 0, 1).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(-t, 0, -1).normalized));
        m_Vertices.Add(shapeGenerator.CalculatePointOnPlanet(new Vector3(-t, 0, 1).normalized));

        for(int i = 0; i < 12; i ++)
        {
            float dist = Vector3.Distance(m_Vertices[i], new Vector3(0,0,0)) - shapeGenerator.GetPlanetRadius();
            if(dist > maxTerrainHeight)
            {
                maxTerrainHeight = dist;
            }
            if(dist < minTerrainHeight)
            {
                minTerrainHeight = dist;
            }
        }

        // And here's the formula for the 20 sides,
        // referencing the 12 vertices we just created.

        m_Polygons.Add(new Polygon( 0, 11,  5));
        m_Polygons.Add(new Polygon( 0,  5,  1));
        m_Polygons.Add(new Polygon( 0,  1,  7));
        m_Polygons.Add(new Polygon( 0,  7, 10));
        m_Polygons.Add(new Polygon( 0, 10, 11));
        m_Polygons.Add(new Polygon( 1,  5,  9));
        m_Polygons.Add(new Polygon( 5, 11,  4));
        m_Polygons.Add(new Polygon(11, 10,  2));
        m_Polygons.Add(new Polygon(10,  7,  6));
        m_Polygons.Add(new Polygon( 7,  1,  8));
        m_Polygons.Add(new Polygon( 3,  9,  4));
        m_Polygons.Add(new Polygon( 3,  4,  2));
        m_Polygons.Add(new Polygon( 3,  2,  6));
        m_Polygons.Add(new Polygon( 3,  6,  8));
        m_Polygons.Add(new Polygon( 3,  8,  9));
        m_Polygons.Add(new Polygon( 4,  9,  5));
        m_Polygons.Add(new Polygon( 2,  4, 11));
        m_Polygons.Add(new Polygon( 6,  2, 10));
        m_Polygons.Add(new Polygon( 8,  6,  7));
        m_Polygons.Add(new Polygon( 9,  8,  1));
    }

    public void Subdivide(int recursions)
    {
        var midPointCache = new Dictionary<int, int>();

        for (int i = 0; i < recursions; i++)
        {
            var newPolys = new List<Polygon>();
            foreach (var poly in m_Polygons)
            {
                int a = poly.m_Vertices[0];
                int b = poly.m_Vertices[1];
                int c = poly.m_Vertices[2];

                // Use GetMidPointIndex to either create a
                // new vertex between two old vertices, or
                // find the one that was already created.

                int ab = GetMidPointIndex(midPointCache, a, b);
                int bc = GetMidPointIndex(midPointCache, b, c);
                int ca = GetMidPointIndex(midPointCache, c, a);

                // Create the four new polygons using our original
                // three vertices, and the three new midpoints.
                newPolys.Add(new Polygon(a, ab, ca));
                newPolys.Add(new Polygon(b, bc, ab));
                newPolys.Add(new Polygon(c, ca, bc));
                newPolys.Add(new Polygon(ab, bc, ca));
            }
            // Replace all our old polygons with the new set of
            // subdivided ones.
            m_Polygons = newPolys;
        }
    }
    public int GetMidPointIndex(Dictionary<int, int> cache, int indexA, int indexB)
    {
        // We create a key out of the two original indices
        // by storing the smaller index in the upper two bytes
        // of an integer, and the larger index in the lower two
        // bytes. By sorting them according to whichever is smaller
        // we ensure that this function returns the same result
        // whether you call
        // GetMidPointIndex(cache, 5, 9)
        // or...
        // GetMidPointIndex(cache, 9, 5)

        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << 16) + greaterIndex;

        // If a midpoint is already defined, just return it.

        int ret;
        if (cache.TryGetValue(key, out ret))
            return ret;

        // If we're here, it's because a midpoint for these two
        // vertices hasn't been created yet. Let's do that now!

        Vector3 p1 = m_Vertices[indexA];
        Vector3 p2 = m_Vertices[indexB];
        Vector3 middle = shapeGenerator.CalculatePointOnPlanet(Vector3.Lerp(p1, p2, 0.5f).normalized);
        float dist = Vector3.Distance(middle, new Vector3(0,0,0)) - shapeGenerator.GetPlanetRadius();
            if(dist > maxTerrainHeight)
            {
                maxTerrainHeight = dist;
            }
            if(dist < minTerrainHeight)
            {
                minTerrainHeight = dist;
            }


        ret = m_Vertices.Count;
        m_Vertices.Add(middle);

        // Add our new midpoint to the cache so we don't have
        // to do this again. =)

        cache.Add(key, ret);
        return ret;
    }

    public void GenerateMesh(GameObject meshObject)
    {
        
        MeshRenderer surfaceRenderer = meshObject.GetComponent<MeshRenderer>();
        surfaceRenderer.sharedMaterial.SetFloat("_PlanetRadius", shapeGenerator.GetPlanetRadius());
        surfaceRenderer.sharedMaterial.SetFloat("_MaxTerrainDist", maxTerrainHeight);
        surfaceRenderer.sharedMaterial.SetFloat("_MinTerrainDist", minTerrainHeight);
        surfaceRenderer.sharedMaterial.SetVector("_PlanetCentre", meshObject.transform.position);

        Mesh terrainMesh = new Mesh();
        terrainMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int vertexCount = m_Polygons.Count * 3;

        int[] indices = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals  = new Vector3[vertexCount];
        
        for (int i = 0; i < m_Polygons.Count; i++)
        {
            var poly = m_Polygons[i];

            indices[i * 3 + 0] = i * 3 + 0;
            indices[i * 3 + 1] = i * 3 + 1;
            indices[i * 3 + 2] = i * 3 + 2;

            vertices[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]];
            vertices[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]];
            vertices[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]];

            normals[i * 3 + 0] = m_Vertices[poly.m_Vertices[0]];
            normals[i * 3 + 1] = m_Vertices[poly.m_Vertices[1]];
            normals[i * 3 + 2] = m_Vertices[poly.m_Vertices[2]];
        }

        terrainMesh.vertices = vertices;
        terrainMesh.normals  = normals;

        terrainMesh.SetTriangles(indices, 0);

        MeshFilter terrainFilter = meshObject.GetComponent<MeshFilter>();
        terrainFilter.mesh = terrainMesh;
    }*/
}
