using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{

    public List<GameObject> floors;
    public List<GameObject> doors;
    private Camera mainCamera;
    public GameObject player, activeFloor, pointer, previousHit;
    public Vector3 target, destinationPosition;
    private Transform playerTransform;
    private float moveSpeed, previousCast, pastDistance, previousMove;
    public float remainingDistance;
    public float destinationDistance;
    public GameObject instantiatedPointer;
    public GameObject debugParticle;
    public Material seeThroughMat;
    private Material tempMat;
    public List<GameObject> obscureList;
    public List<Material> obscureMatList;
    private bool raycastObscure, checkForObject;

    void Start()
    {
        mainCamera = Camera.main;
        playerTransform = player.transform;
        destinationPosition = playerTransform.position;
        activeFloor = GameObject.FindGameObjectWithTag("floor");
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
                    destinationPosition = new Vector3(Mathf.Floor(targetPoint.x) + 0.5f, Mathf.Floor(targetPoint.y), Mathf.Floor(targetPoint.z) + 0.5f);
                    Quaternion targetRotation = Quaternion.LookRotation(targetPoint - playerTransform.position);
                    playerTransform.rotation = targetRotation;
                    if (instantiatedPointer != null)
                    {
                        DestroyObject(instantiatedPointer);
                    }
                    instantiatedPointer = Instantiate(pointer, destinationPosition, Quaternion.identity) as GameObject;
                }
            }

            previousMove = 0;   
        }

        if (destinationDistance > .01f)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, destinationPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void FollowPlayer()
    {
        previousCast += Time.deltaTime;
        //ok so i need to check my current destination with what it just was. If the destination is more than one it should to a check. Might be kind of tricky because the player never mooves one FULL unit but rather a tiny fraction less.

        if (destinationDistance < remainingDistance - 0.95f)
        {
            remainingDistance = destinationDistance;

            raycastObscure = true;
        }
        else
        {
            raycastObscure = false;
        }

        //slowed down the raycasting to save on some CPU. It only casts once every half second instead of as fast as it updates.

        if (raycastObscure)
        {
            player.GetComponent<ObjectCollision>().CollisionDetection();
        }
    }

    public void ClearRoomList()
    {
        doors.Clear();
        obscureList.Clear();
        obscureMatList.Clear();
    }

    public void ResetPosition(Vector3 position)
    {
        destinationPosition = new Vector3(Mathf.Floor(position.x) + 0.5f, 0.0f, Mathf.Floor(position.z) + 0.5f);
    }
}
