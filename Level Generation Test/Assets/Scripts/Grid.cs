﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer), typeof (BoxCollider))]
public class Grid : MonoBehaviour {

    private Material floorMaterial;
    public int xSize, zSize;

    public Vector3[] floorVerts;

    private Mesh meshFloor, meshWallsBack, meshWallsFront;

    public List<GameObject> doors;

    private void Awake()
    {
        gameObject.isStatic = true;
        Generate();
    }

    private void Generate()
    {
        floorVerts = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uvFloor = new Vector2[floorVerts.Length];

        floorMaterial = Resources.Load<Material>("floor");

        for (int i = 0, y = 0; y < zSize + 1; y++)
        {
            for (int x = 0; x < xSize + 1; x++, i++)
            {
                floorVerts[i] = new Vector3(x, 0, y);
                uvFloor[i] = new Vector2((float)x / xSize, (float)y / zSize);
            }
        }

        GetComponent<MeshFilter>().mesh = meshFloor = new Mesh();
        meshFloor.name = "Generated Floor Mesh";

        meshFloor.vertices = floorVerts;
        meshFloor.uv = uvFloor;
        meshFloor.uv2 = uvFloor;

        int[] TrianglesFloor = new int[xSize * zSize * 6];

        for (int ti = 0, vi = 0, y = 0; y < zSize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                TrianglesFloor[ti] = vi;
                TrianglesFloor[ti + 3] = TrianglesFloor[ti + 2] = vi + 1;
                TrianglesFloor[ti + 4] = TrianglesFloor[ti + 1] = vi + xSize + 1;
                TrianglesFloor[ti + 5] = vi + xSize + 2;
                meshFloor.triangles = TrianglesFloor;
            }
        }

        meshFloor.triangles = TrianglesFloor;

        Vector3 colliderSize = new Vector3((float)xSize, 0.01f, (float)zSize);

        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = colliderSize;
        collider.center = floorVerts[floorVerts.Length/2];

        meshFloor.RecalculateBounds();
        meshFloor.RecalculateNormals();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = floorMaterial;
        meshRenderer.material.mainTextureScale = new Vector2(xSize, zSize);

        gameObject.tag = "floor";

        GameObject front = new GameObject("front");
        front.transform.parent = gameObject.transform;
        front.layer = 2;
        front.tag = "front";
        front.AddComponent<Wall>();

        GameObject back = new GameObject("back");
        back.transform.parent = gameObject.transform;
        back.layer = 2;
        back.tag = "back";
        back.AddComponent<Wall>();

        GameObject down = new GameObject("down");
        down.transform.parent = gameObject.transform;
        down.layer = 2;
        down.tag = "down";
        down.AddComponent<Wall>();

        GameObject left = new GameObject("left");
        left.transform.parent = gameObject.transform;
        left.layer = 2;
        left.tag = "left";
        left.AddComponent<Wall>();
    }

    /*private void OnDrawGizmos()
    {
        if (floorVerts == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < floorVerts.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(floorVerts[i]), 0.1f);
        }
    }*/
}
