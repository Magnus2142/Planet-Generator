using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeSphere
{
    ShapeGenerator shapeGenerator;

    public int gridSize;

	private Mesh mesh;
	private Vector3[] vertices;
	private Vector3[] normals;

    float maxTerrainHeight;
    float minTerrainHeight = 100f;

	Vector3 minVertex;

	Vector3 planetCentre;

	public void Generate (ShapeGenerator shapeGenerator, GameObject meshObj, int resolution, float oceanLevel) 
    {
        this.shapeGenerator = shapeGenerator;
        this.gridSize = resolution;
		this.planetCentre = meshObj.transform.position;
        MeshRenderer meshRenderer = meshObj.GetComponent<MeshRenderer>();
        
		meshObj.GetComponent<MeshFilter>().sharedMesh = mesh = new Mesh();
		mesh.name = "Cube Sphere";
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        CreateVertices();
        CreateTriangles();
		Material mat = new Material(Shader.Find("Custom/PlanetShaderDesert"));
		meshRenderer.sharedMaterial.SetFloat("_PlanetRadius", shapeGenerator.GetPlanetRadius());
        meshRenderer.sharedMaterial.SetFloat("_MaxTerrainDist", maxTerrainHeight);
        meshRenderer.sharedMaterial.SetFloat("_MinTerrainDist", minTerrainHeight);
        meshRenderer.sharedMaterial.SetVector("_PlanetCentre", planetCentre);
		meshRenderer.sharedMaterial.SetFloat("_LowerMiddleValue", oceanLevel);
		meshRenderer.sharedMaterials[0] = mat;
		meshRenderer.sharedMaterials[1] = mat;
		meshRenderer.sharedMaterials[2] = mat;
	}

    private void CreateVertices()
    {
        int cornerVertices = 8;
		int edgeVertices = (gridSize + gridSize + gridSize - 3) * 4;
		int faceVertices = (
			(gridSize - 1) * (gridSize - 1) +
			(gridSize - 1) * (gridSize - 1) +
			(gridSize - 1) * (gridSize - 1)) * 2;
		vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
		normals = new Vector3[vertices.Length];

   		int v = 0;
		for (int y = 0; y <= gridSize; y++) {
			for (int x = 0; x <= gridSize; x++) {
				SetVertex(v++, x, y, 0);
			}
			for (int z = 1; z <= gridSize; z++) {
				SetVertex(v++, gridSize, y, z);
			}
			for (int x = gridSize - 1; x >= 0; x--) {
				SetVertex(v++, x, y, gridSize);
			}
			for (int z = gridSize - 1; z > 0; z--) {
				SetVertex(v++, 0, y, z);
			}
		}
		for (int z = 1; z < gridSize; z++) {
			for (int x = 1; x < gridSize; x++) {
				SetVertex(v++, x, gridSize, z);
			}
		}
		for (int z = 1; z < gridSize; z++) {
			for (int x = 1; x < gridSize; x++) {
				SetVertex(v++, x, 0, z);
			}
		}

        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    private void CreateTriangles()
    {
        int[] trianglesZ = new int[(gridSize * gridSize) * 12];
		int[] trianglesX = new int[(gridSize * gridSize) * 12];
		int[] trianglesY = new int[(gridSize * gridSize) * 12];
		int ring = (gridSize + gridSize) * 2;
		int tZ = 0, tX = 0, tY = 0, v = 0;

		for (int y = 0; y < gridSize; y++, v++) {
			for (int q = 0; q < gridSize; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < gridSize; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < gridSize; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < gridSize - 1; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
		}

        tY = CreateTopFace(trianglesY, tY, ring);
		tY = CreateBottomFace(trianglesY, tY, ring);
		
        mesh.subMeshCount = 3;
		mesh.SetTriangles(trianglesZ, 0);
		mesh.SetTriangles(trianglesX, 1);
		mesh.SetTriangles(trianglesY, 2);
    }

    private int CreateTopFace (int[] triangles, int t, int ring) {
		int v = ring * gridSize;
		for (int x = 0; x < gridSize - 1; x++, v++) {
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

		int vMin = ring * (gridSize + 1) - 1;
		int vMid = vMin + 1;
        int vMax = v + 2;
		for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + gridSize - 1);
            for (int x = 1; x < gridSize - 1; x++, vMid++) {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + gridSize - 1, vMid + gridSize);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + gridSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
		t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
		for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
		}
		t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);
		return t;
	}

    private int CreateBottomFace (int[] triangles, int t, int ring) {
		int v = 1;
		int vMid = vertices.Length - (gridSize - 1) * (gridSize - 1);
		t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
		for (int x = 1; x < gridSize - 1; x++, v++, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

		int vMin = ring - 2;
		vMid -= gridSize - 2;
		int vMax = v + 2;

		for (int z = 1; z < gridSize - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid + gridSize - 1, vMin + 1, vMid);
			for (int x = 1; x < gridSize - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid + gridSize - 1, vMid + gridSize, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vMid + gridSize - 1, vMax + 1, vMid, vMax);
		}

		int vTop = vMin - 1;
		t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
		for (int x = 1; x < gridSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
		}
		t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
		
		return t;
	}

    // Helper functions beneath:

    private void SetVertex (int i, int x, int y, int z) {
        Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;
        float x2 = v.x * v.x;
		float y2 = v.y * v.y;
		float z2 = v.z * v.z;
		Vector3 s;
		s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
		s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
		s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

		normals[i] = v.normalized;
		vertices[i] = shapeGenerator.CalculatePointOnPlanet(normals[i]);
        float dist = Math.Abs(Vector3.Distance(vertices[i], planetCentre) - shapeGenerator.GetPlanetRadius());
		
        if(dist > maxTerrainHeight)
        {
            maxTerrainHeight = dist;
        }
        if(dist < minTerrainHeight)
        {
            minTerrainHeight = dist;
			minVertex = vertices[i];
        }
    
	}

    private static int SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) 
    {
		triangles[i] = v00;
		triangles[i + 1] = triangles[i + 4] = v01;
		triangles[i + 2] = triangles[i + 3] = v10;
		triangles[i + 5] = v11;
		return i + 6;
	}

    
}
