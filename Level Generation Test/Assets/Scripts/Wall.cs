using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
public class Wall : MonoBehaviour {
    public Grid floorGrid;

    public int xSize, ySize, zSize, gridX;

    public Vector3[] wallVerts;

    private Mesh mesh;

    public Material wallMat;

    private Vector2 materialScale;

    public string direction;

    void Awake()
    {

        xSize = GetComponentInParent<Grid>().xSize;
        zSize = GetComponentInParent<Grid>().zSize;
        gridX = xSize;

        if (direction == "front" || direction == "down")
        {
            ySize = 1;
        }
        else
        {
            ySize = 5;
        }
        NewWall();
    }

    public void NewWall()
    {
        wallVerts = new Vector3[4];
        Vector2[] uv = new Vector2[wallVerts.Length];

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Generated Wall Mesh";

        for (int i = 0, y = 0; y < 2; y++)
        {
            for (int x = 0, z = 0; x < wallVerts.Length / 2; x++, z++, i++)
            {
                if (direction == "left")
                {
                    wallVerts[i] = new Vector3(0, y * ySize, z * zSize);
                    Vector3 colliderSize = new Vector3(0.01f, ySize, zSize);
                    SetBoxCollider(colliderSize);
                }
                else if (direction == "down")
                {
                    wallVerts[i] = new Vector3(x * xSize, y * ySize);
                    Vector3 colliderSize = new Vector3(xSize, ySize, 0.01f);
                    SetBoxCollider(colliderSize);
                }
                else if (direction == "front")
                {
                    wallVerts[i] = new Vector3(gridX, y * ySize, z * zSize);
                    Vector3 colliderSize = new Vector3(0.01f, ySize, zSize);
                    SetBoxCollider(colliderSize);
                }
                else if (direction == "back")
                {
                    wallVerts[i] = new Vector3(x * xSize, y * ySize, zSize);
                    Vector3 colliderSize = new Vector3(xSize, ySize, 0.01f);
                    SetBoxCollider(colliderSize);
                }
                else
                {
                    Debug.LogError("NO DIRECTION SET/INPROPPER");
                }

                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }

        mesh.vertices = wallVerts;
        mesh.uv = uv;
        mesh.uv2 = uv;


        int[] TrianglesWall = new int[6];

        TrianglesWall[0] = 0;
        TrianglesWall[1] = 2;
        TrianglesWall[2] = 1;
        TrianglesWall[3] = TrianglesWall[1];
        TrianglesWall[4] = 3;
        TrianglesWall[5] = TrianglesWall[2];

        mesh.triangles = TrianglesWall;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = wallMat;

        if (direction == "down" || direction == "back")
        {
            materialScale = new Vector2(xSize, ySize);
        }
        else
        {
            materialScale = new Vector2(zSize, ySize);
        }

        meshRenderer.material.mainTextureScale = materialScale;

        mesh.Optimize();

        mesh.RecalculateNormals();
    }

    private void SetBoxCollider(Vector3 colliderSize)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = colliderSize;
        if (direction == "front")
        {
            collider.center = new Vector3((float)xSize, (float)ySize / 2, (float)zSize / 2);
        }
        else if (direction == "back")
        {
            collider.center = new Vector3((float)xSize / 2, (float)ySize / 2, (float)zSize);
        }
        else if (direction == "down")
        {
            collider.center = new Vector3((float)xSize / 2, (float)ySize / 2, 0.0f);
        }
        else if (direction == "left")
        {
            collider.center = new Vector3(0.0f, (float)ySize / 2, (float)zSize / 2);
        }
    }

    private void OnDrawGizmos()
    {
        if (wallVerts == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < wallVerts.Length; i++)
        {
            Gizmos.DrawSphere(wallVerts[i], 0.1f);
        }
    }
}
