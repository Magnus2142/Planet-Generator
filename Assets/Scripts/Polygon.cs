using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*   A class that holds the vertices numbers of the 
*   triangle. This is to easier keep track of which vertices
*   used to draw the triangle
*/

public class Polygon
{
    public List<int> m_Vertices;

    public Polygon(int a, int b, int c)
    {
        // Vertice 1 = a, vertice 2 = b, vertice 3 = c
        m_Vertices = new List<int>() {a, b, c};
    }
}