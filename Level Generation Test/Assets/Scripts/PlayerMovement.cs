using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{

    public List<GameObject> floors;
    public List<GameObject> doors;
    private Camera mainCamera;
    public GameObject player, activeFloor, pointer, previousHit;
    private Vector3 target, destinationPosition, currentPosition, lastPoistion;
    private Transform playerTransform;
    private float moveSpeed, previousCast;
    public float destinationDistance;
    private GameObject instantiatedPointer;
    public ParticleSystem debugParticle;
    public Material seeThroughMat;
    private Material tempMat;
    public List<GameObject> obscureList;
    public List<Material> obscureMatList;


    void Start()
    {
        mainCamera = Camera.main;
        playerTransform = player.transform;
        destinationPosition = playerTransform.position;
        activeFloor = GameObject.FindGameObjectWithTag("floor");
        previousCast = 0;
        previousHit = gameObject;
    }

	void Update ()
    {
        Travel();

        FollowPlayer();
    }

    private void Travel()
    {

        destinationDistance = Vector3.Distance(destinationPosition, playerTransform.position);

        if (destinationDistance < .1f)
        {       // To prevent shakin behavior when near destination
            moveSpeed = 0;

            DestroyObject(instantiatedPointer);
        }
        else if (destinationDistance > .1f)
        {           // To Reset Speed to default
            moveSpeed = 3;
        }

        // Moves the Player if the Left Mouse Button was clicked
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "floor")
                {
                    Vector3 targetPoint = hit.point;
                    destinationPosition = new Vector3(Mathf.Floor(targetPoint.x) + 0.5f, 0.0f, Mathf.Floor(targetPoint.z) + 0.5f);
                    Quaternion targetRotation = Quaternion.LookRotation(targetPoint - playerTransform.position);
                    playerTransform.rotation = targetRotation;
                    if (instantiatedPointer != null)
                    {
                        DestroyObject(instantiatedPointer);
                    }
                    instantiatedPointer = Instantiate(pointer, destinationPosition, Quaternion.identity) as GameObject;
                }
            }
                
        }

        if (destinationDistance > .1f)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, destinationPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void FollowPlayer()
    {
        Vector3 cameraPosition = new Vector3(player.transform.position.x + 8.4f, 8.6f, player.transform.position.z - 14.5f);
        mainCamera.transform.position = cameraPosition;

        previousCast += Time.deltaTime;
        //ok so i need to check my current destination with what it just was. If the destination is more than one it should to a check. Might be kind of tricky because the player never mooves one FULL unit but rather a tiny fraction less.

        //slowed down the raycasting to save on some CPU. It only casts once every second instead of as fast as it updates.

        if (previousCast > 0.45f)
        {

            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log(hit.collider.name);
                if (hit.collider.name != "ThirdPersonController" && hit.collider.name != previousHit.name)
                {
                    Debug.Log("HIT OBJECT IS NOT PLAYER");
                    obscureList.Add(hit.collider.gameObject);
                    obscureMatList.Add(hit.collider.gameObject.GetComponent<Renderer>().material);
                    
                    foreach (GameObject item in obscureList)
                    {
                        item.GetComponent<Renderer>().material = seeThroughMat;
                        //item.layer = 2;
                    }
                }
                else if (obscureList.Count > 0 && hit.collider.name == "ThirdPersonController")
                {
                    Debug.Log("HIT OBJECT IS PLAYER");
                    for (int i = 0; i < obscureList.Count; i++)
                    {
                        obscureList[i].GetComponent<Renderer>().material = obscureMatList[i];
                        //obscureList[i].layer = 0;
                    }
                    obscureList.Clear();
                    obscureMatList.Clear();
                }

                //for debugging purposes
                //ParticleSystem temp = (ParticleSystem)Instantiate(debugParticle, hit.point, Quaternion.identity);
                //DestroyObject(temp, 1);

                previousHit = hit.collider.gameObject;
                previousCast = 0;
            }
        }
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
