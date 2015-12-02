using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
public class Door : MonoBehaviour {

    private int xSize, ySize, wallSize, gridX;
    private Wall wall;
    private Mesh mesh;
    public Vector3[] doorVerts;
    public Material doorMaterial;
    private Vector2 materialScale;
    private Vector2[] uv;
    private PlayerMovement playerInteraction;

    private float justTraveled;

    void Awake()
    {
        wall = GetComponentInParent<Wall>();

        playerInteraction = Camera.main.GetComponent<PlayerMovement>();
        
        if (wall.direction == "down" || wall.direction == "back")
        {
            wallSize = wall.floorGrid.xSize;
        }
        else
        {
            wallSize = wall.floorGrid.zSize;
        }

        xSize = 1;
        ySize = 2;
        gridX = wall.floorGrid.xSize;

        Generate();

        justTraveled = 0;
    }

    private void Generate()
    {

        doorVerts = new Vector3[4];
        uv = new Vector2[doorVerts.Length];

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Generated Door";

        for (int i = 0, y = 0; y < 2; y++)
        {
            for (int x = 0; x < doorVerts.Length / 2; x++, i++)
            {

                if (wall.direction == "down")
                {
                    doorVerts[i] = new Vector3(wallSize / 2 + (x * xSize), y * ySize, -0.01f);
                    Vector3 colliderSize = new Vector3(xSize, ySize, 1);
                    SetBoxCollider(colliderSize);
                }
                else if (wall.direction == "front")
                {
                    doorVerts[i] = new Vector3(gridX + 0.01f, y * ySize, wallSize / 2 + (x * xSize));
                    Vector3 colliderSize = new Vector3(xSize, ySize, 1);
                    SetBoxCollider(colliderSize);
                }
                else if (wall.direction == "left")
                {
                    doorVerts[i] = new Vector3(0.01f, y * ySize, wallSize / 2 + (x * xSize));
                    Vector3 colliderSize = new Vector3(xSize, ySize, 1);
                    SetBoxCollider(colliderSize);
                }
                else if (wall.direction == "back")
                {
                    doorVerts[i] = new Vector3(wallSize / 2 + (x * xSize), y * ySize, wall.floorGrid.zSize - 0.01f);
                    Vector3 colliderSize = new Vector3(xSize, ySize, 1);
                    SetBoxCollider(colliderSize);
                }
                else
                {
                    Debug.LogError("NO DIRECTION SET/INPROPPER");
                }

                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }

        mesh.vertices = doorVerts;
        mesh.uv = uv;
        mesh.uv2 = uv;

        int[] trianglesDoor = new int[6];

        trianglesDoor[0] = 0;
        trianglesDoor[1] = 2;
        trianglesDoor[2] = 1;
        trianglesDoor[3] = trianglesDoor[1];
        trianglesDoor[4] = 3;
        trianglesDoor[5] = trianglesDoor[2];

        mesh.triangles = trianglesDoor;

        mesh.RecalculateNormals();

        tag = wall.direction;

        playerInteraction.doors.Add(gameObject);

    }

    private void SetBoxCollider(Vector3 colliderSize)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = colliderSize;
        if (wall.direction == "front")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2 - 1, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2));
        }
        else if (wall.direction == "back")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2) - 1);
        }
        else if (wall.direction == "down")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2));
        }
        else if (wall.direction == "left")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2));
        }
    }

    private void OnDrawGizmos()
    {
        if (doorVerts == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < doorVerts.Length; i++)
        {
            Gizmos.DrawSphere(doorVerts[i], 0.1f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (tag == "front" && justTraveled > 5)
        {
            Debug.Log("TOUCHED DOOR: " + tag);

            Destroy(playerInteraction.activeFloor);
            playerInteraction.activeFloor = null;
            playerInteraction.ClearRoomList();
            GameObject newFloor = Instantiate(playerInteraction.floors[Random.Range(0, playerInteraction.floors.Count)], Vector3.zero, Quaternion.identity) as GameObject;
            playerInteraction.activeFloor = newFloor;

            GameObject teleportDoor = null;

            for (int i = 0; i < playerInteraction.doors.Count; i++)
            {
                if (playerInteraction.doors[i].tag == "left")
                {
                    teleportDoor = playerInteraction.doors[i];
                    break;
                }
            }

            Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(0.5f, 0.0f, 0.5f);
            playerInteraction.player.transform.position = teleportToDoor;

            justTraveled = 0;
        }
        else if (tag == "back" && justTraveled > 5)
        {
            Debug.Log("TOUCHED DOOR: " + tag);

            Destroy(playerInteraction.activeFloor);
            playerInteraction.activeFloor = null;
            playerInteraction.ClearRoomList();
            GameObject newFloor = Instantiate(playerInteraction.floors[Random.Range(0, playerInteraction.floors.Count)], Vector3.zero, Quaternion.identity) as GameObject;
            playerInteraction.activeFloor = newFloor;

            GameObject teleportDoor = null;

            for (int i = 0; i < playerInteraction.doors.Count; i++)
            {
                if (playerInteraction.doors[i].tag == "down")
                {
                    teleportDoor = playerInteraction.doors[i];
                    break;
                }
            }

            Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(0.5f, 0.0f, 0.5f);
            playerInteraction.player.transform.position = teleportToDoor;

            justTraveled = 0;
        }
        else if (tag == "down" && justTraveled > 5)
        {
            Debug.Log("TOUCHED DOOR: " + tag);

            Destroy(playerInteraction.activeFloor);
            playerInteraction.activeFloor = null;
            playerInteraction.ClearRoomList();
            GameObject newFloor = Instantiate(playerInteraction.floors[Random.Range(0, playerInteraction.floors.Count)], Vector3.zero, Quaternion.identity) as GameObject;
            playerInteraction.activeFloor = newFloor;

            GameObject teleportDoor = null;

            for (int i = 0; i < playerInteraction.doors.Count; i++)
            {
                if (playerInteraction.doors[i].tag == "back")
                {
                    teleportDoor = playerInteraction.doors[i];
                    break;
                }
            }

            Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(0.5f, 0.0f, -0.5f);
            playerInteraction.player.transform.position = teleportToDoor;

            justTraveled = 0;
        }
        else if (tag == "left" && justTraveled > 5)
        {
            Debug.Log("TOUCHED DOOR: " + tag);

            Destroy(playerInteraction.activeFloor);
            playerInteraction.activeFloor = null;
            playerInteraction.ClearRoomList();
            GameObject newFloor = Instantiate(playerInteraction.floors[Random.Range(0, playerInteraction.floors.Count)], Vector3.zero, Quaternion.identity) as GameObject;
            playerInteraction.activeFloor = newFloor;

            justTraveled = 0;

            GameObject teleportDoor = null;

            for (int i = 0; i < playerInteraction.doors.Count; i++)
            {
                if (playerInteraction.doors[i].tag == "front")
                {
                    teleportDoor = playerInteraction.doors[i];
                    break;
                }
            }

            Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(-0.5f, 0.0f, 0.5f);
            playerInteraction.player.transform.position = teleportToDoor;
        }
    }

    void Update()
    {
        justTraveled += Time.deltaTime;
    }
    
}
