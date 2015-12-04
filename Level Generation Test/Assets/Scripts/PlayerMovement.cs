using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

    public List<GameObject> floors;
    public List<GameObject> doors;
    private Camera mainCamera;
    public GameObject player;
    public GameObject activeFloor;
    private bool traveling = false;
    private Vector3 target, destinationPosition;
    private Transform playerTransform;
    private float moveSpeed;
    public float destinationDistance;

    void Start()
    {
        mainCamera = Camera.main;
        playerTransform = player.transform;
        destinationPosition = playerTransform.position;
        activeFloor = GameObject.FindGameObjectWithTag("floor");
    }

	void Update ()
    {
        destinationDistance = Vector3.Distance(destinationPosition, playerTransform.position);

        Travel();

        FollowPlayer();

    }

    private void Travel()
    {

        if (destinationDistance < .5f)
        {       // To prevent shakin behavior when near destination
            moveSpeed = 0;
        }
        else if (destinationDistance > .5f)
        {           // To Reset Speed to default
            moveSpeed = 3;
        }

        // Moves the Player if the Left Mouse Button was clicked
        if (Input.GetMouseButtonDown(0))
        {
            Plane playerPlane = new Plane(Vector3.up, playerTransform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            float hitdist = 0.0f;

            if (playerPlane.Raycast(ray, out hitdist))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "floor")
                    {
                        Vector3 targetPoint = ray.GetPoint(hitdist);
                        destinationPosition = ray.GetPoint(hitdist);
                        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - playerTransform.position);
                        playerTransform.rotation = targetRotation;
                    }
                }
                
            }
        }

        if (destinationDistance > .5f)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, destinationPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void FollowPlayer()
    {
        Vector3 cameraPosition = new Vector3(player.transform.position.x + 8.5f, 8.5f, player.transform.position.z - 14.5f);
        mainCamera.transform.position = cameraPosition;
    }

    public void ClearRoomList()
    {
        doors.Clear();
    }

    public void ResetPosition(Vector3 position)
    {
        destinationPosition = position;
    }
}
