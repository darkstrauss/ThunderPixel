using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{

    public List<GameObject> floors;
    public List<GameObject> doors;
    private Camera mainCamera;
    public GameObject player, activeFloor, pointer, previousHit;
    private Grid activeGrid;
    public Vector3 target, destinationPosition;
    private Transform playerTransform;
    private float moveSpeed, previousCast, pastDistance, previousMove;
    public float remainingDistance;
    public float destinationDistance;
    public GameObject instantiatedPointer;
    public GameObject debugParticle;
    public Material seeThroughMat;
    private Material tempMat;
    private bool raycastObscure, checkForObject;

    void Start()
    {
        mainCamera = Camera.main;
        playerTransform = player.transform;
        destinationPosition = playerTransform.position;
        activeFloor = GameObject.FindGameObjectWithTag("floor");
        activeGrid = activeFloor.GetComponent<Grid>();
        previousCast = 0;
        raycastObscure = false;
        previousHit = gameObject;
        previousMove = 0;
        checkForObject = false;
    }

	void Update ()
    {
        Travel();

        FollowPlayer();
    }

    void LateUpdate()
    {
        Vector3 cameraPosition = new Vector3(player.transform.position.x + 4.3f, 9.0f, player.transform.position.z - 7.3f);
        mainCamera.transform.position = cameraPosition;
    }

    private void Travel()
    {
        destinationDistance = Vector3.Distance(destinationPosition, playerTransform.position);

        if (destinationDistance < .01f)
        {
            moveSpeed = 0;
            remainingDistance = 0;
            ResetPosition(player.transform.position);
            if (activeFloor.GetComponent<Grid>().path != null && activeFloor.GetComponent<Grid>().path.Count > 0)
            {
                activeFloor.GetComponent<Grid>().ClearPath();
            }
            
            Destroy(instantiatedPointer);
            if (checkForObject)
            {
                player.GetComponent<ObjectCollision>().CollisionDetection();
                checkForObject = false;
            }
        }
        else if (destinationDistance > .01f)
        {
            // To Reset Speed to default
            if (!checkForObject)
            {
                checkForObject = true;
            }
            moveSpeed = 3;
            if (remainingDistance == 0)
            {
                remainingDistance = destinationDistance;
            }
        }

        previousMove += Time.deltaTime;

        // Moves the Player if the Left Mouse Button was clicked
        if (Input.GetMouseButtonUp(0) && previousMove > 0.3f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "floor")
                {
                    player.GetComponent<ObjectCollision>().CollisionDetection();
                    Vector3 targetPoint = hit.point;
                    destinationPosition = new Vector3(Mathf.Floor(targetPoint.x) + 0.5f, 0, Mathf.Floor(targetPoint.z) + 0.5f);
                    if (instantiatedPointer != null)
                    {
                        DestroyObject(instantiatedPointer);
                    }
                    instantiatedPointer = Instantiate(pointer, destinationPosition, Quaternion.identity) as GameObject;

                    StartCoroutine(Move());
                }
            }

            previousMove = 0;
        }
    }

    private IEnumerator Move()
    {
        List<MapPosition> path = activeFloor.GetComponent<Grid>().FindPath(destinationPosition);
        
        while (path != null && path.Count > 0)
        {
            Vector3 movePosition = new Vector3(path[path.Count - 1].xPos + 0.5f, 0.0f, path[path.Count - 1].yPos + 0.5f);
            Debug.Log("Path list :" + path[path.Count - 1].ToString() + ", Index Count: " + path.Count);
            Quaternion targetRotation = Quaternion.LookRotation(movePosition - playerTransform.position);
            playerTransform.rotation = targetRotation;
            player.transform.position = movePosition;
            path.RemoveAt(path.Count - 1);
            yield return new WaitForSeconds(0.3f);
        }

        activeFloor.GetComponent<Grid>().ClearPath();
    }

    private void FollowPlayer()
    {
        previousCast += Time.deltaTime;

        if (destinationDistance < remainingDistance - 0.95f)
        {
            remainingDistance = destinationDistance;
            raycastObscure = true;
        }
        else
        {
            raycastObscure = false;
        }

        if (raycastObscure)
        {
            player.GetComponent<ObjectCollision>().CollisionDetection();
        }
    }

    public void ClearRoomList()
    {
        activeFloor.GetComponent<Grid>().ClearPath();
        //activeGrid.nodeMap = null;
        doors.RemoveRange(0, doors.Count);
    }

    public void ResetPosition(Vector3 position)
    {
        destinationPosition = new Vector3(Mathf.Floor(position.x) + 0.5f, 0.0f, Mathf.Floor(position.z) + 0.5f);
        player.transform.position = destinationPosition;
    }
}
