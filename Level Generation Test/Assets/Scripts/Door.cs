using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
public class Door : MonoBehaviour {

    private static float TIMEWAIT = 3.0f;
    private int xSize, ySize, wallSize, gridX;
    private Wall wall;
    private Mesh mesh;
    public Vector3[] doorVerts;
    public GameObject floor;
    private Grid grid;
    public Material doorMaterial;
    private Vector2 materialScale;
    private Vector2[] uv;
    private PlayerMovement playerInteraction;

    private float justTraveled;

    void Awake()
    {
        justTraveled = 0;
        //Debug.Log("RUNNING START() IN DOOR.CS" + justTraveled);
        gameObject.isStatic = true;
        gameObject.transform.localPosition = Vector3.zero;

        GetComponent<BoxCollider>().isTrigger = true;

        wall = transform.parent.GetComponent<Wall>();
        floor = transform.parent.transform.parent.gameObject;
        grid = floor.GetComponent<Grid>();
            
        playerInteraction = Camera.main.GetComponent<PlayerMovement>();
        
        if (tag == "down" || tag == "back")
        {
            wallSize = wall.xSize;
        }
        else
        {
            wallSize = wall.zSize;
        }

        xSize = 1;
        ySize = 2;
        gridX = wall.xSize;

        Generate();
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

                if (tag == "down")
                {
                    doorVerts[i] = new Vector3(wallSize / 2 + (x * xSize), y * ySize, -0.01f); 
                }
                else if (tag == "front")
                {
                    doorVerts[i] = new Vector3(gridX + 0.01f, y * ySize, wallSize / 2 + (x * xSize));
                }
                else if (tag == "left")
                {
                    doorVerts[i] = new Vector3(0.01f, y * ySize, wallSize / 2 + (x * xSize));
                }
                else if (tag == "back")
                {
                    doorVerts[i] = new Vector3(wallSize / 2 + (x * xSize), y * ySize, wall.zSize - 0.01f);
                }
                else
                {
                    Debug.LogError("NO DIRECTION SET/INPROPPER");
                }

                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }

        Vector3 colliderSize = new Vector3((float)xSize / 2.0f, ySize, 0.5f);
        SetBoxCollider(colliderSize);

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

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //playerInteraction.doors.Add(gameObject);

        //grid.doors.Add(gameObject);
    }

    private void SetBoxCollider(Vector3 colliderSize)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = colliderSize;
        if (tag == "front")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2 - 1.01f, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2));
        }
        else if (tag == "back")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2) - 0.99f);
        }
        else if (tag == "down")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2) + 0.01f);
        }
        else if (tag == "left")
        {
            collider.center = new Vector3(doorVerts[0].x + (float)xSize / 2 - 0.01f, doorVerts[0].y + (float)ySize / 2, doorVerts[0].z + ((float)xSize / 2));
        }
        //generator.takenSpots.Add(collider.center);
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
        if (justTraveled > TIMEWAIT)
        {
            Debug.Log("TOUCHED DOOR: " + tag);
            Destroy(playerInteraction.activeFloor);
            playerInteraction.player.GetComponent<ObjectCollision>().ForceClearList();
            playerInteraction.ClearRoomList();
            GameObject newFloor = Instantiate(floorGet(), Vector3.zero, Quaternion.identity) as GameObject;
            List<GameObject> newDoorList = newFloor.GetComponent<Grid>().doors;
            playerInteraction.activeFloor = newFloor;

            Debug.Log("CURRENT DOOR COUNT: " + newDoorList.Count);

            if (tag == "front")
            {
                Debug.Log("ATTEMPTING TO LOOK THROUGH DOORS LIST FOR DIRECTION: " + tag);
                for (int i = 0; i < newDoorList.Count - 1; i++)
                {
                    if (grid.doors[i].tag == "left")
                    {
                        GameObject teleportDoor = grid.doors[i];
                        Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(0.5f, 0.0f, 0.5f);
                        playerInteraction.ResetPosition(teleportToDoor);
                        
                        break;
                    }
                }
            }
            else if (tag == "back")
            {
                Debug.Log("ATTEMPTING TO LOOK THROUGH DOORS LIST FOR DIRECTION: " + tag);
                for (int i = 0; i < grid.doors.Count - 1; i++)
                {
                    if (grid.doors[i].tag == "down")
                    {
                        GameObject teleportDoor = grid.doors[i];
                        Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(0.5f, 0.0f, 0.5f);
                        playerInteraction.ResetPosition(teleportToDoor);
                        
                        break;
                    }
                }
            }
            else if (tag == "down")
            {
                Debug.Log("ATTEMPTING TO LOOK THROUGH DOORS LIST FOR DIRECTION: " + tag);
                for (int i = 0; i < grid.doors.Count - 1; i++)
                {
                    if (grid.doors[i].tag == "back")
                    {
                        GameObject teleportDoor = grid.doors[i];
                        Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(-0.5f, 0.0f, -0.5f);
                        playerInteraction.ResetPosition(teleportToDoor);

                        break;
                    }
                }
            }
            else if (tag == "left")
            {
                Debug.Log("ATTEMPTING TO LOOK THROUGH DOORS LIST FOR DIRECTION: " + tag);
                for (int i = 0; i < grid.doors.Count - 1; i++)
                {
                    Debug.Log("CHECKING FOR NEW DOOR: " + i);
                    if (grid.doors[i].tag == "front")
                    {
                        Debug.Log("ATTEMPTING TO SET TELEPORT DOOR :" + i);
                        GameObject teleportDoor = grid.doors[i];
                        Vector3 teleportToDoor = teleportDoor.GetComponent<Door>().doorVerts[0] + new Vector3(-0.5f, 0.0f, 0.5f);
                        playerInteraction.ResetPosition(teleportToDoor);
                        
                        break;
                    }
                }
            }
            justTraveled = 0;
        }
    }

    void Update()
    {
        justTraveled += Time.deltaTime;
    }

    private GameObject floorGet()
    {
        GameObject pickedFloor = playerInteraction.floors[Random.Range(0, playerInteraction.floors.Count - 1)];

        return pickedFloor;
    }
    
}
