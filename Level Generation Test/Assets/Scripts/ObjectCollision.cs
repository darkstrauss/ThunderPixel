using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectCollision : MonoBehaviour {

    private PlayerMovement movement;
    private GameObject mainCamera;
    public Material seeThroughMat;
    public GameObject previousHit;
    public bool isTouchingDown, isTouchingRight;
    public List<GameObject> obscureListDown, obscureListRight;
    public List<Material> obscureMatListDown, obscureMatListRight;
    private float destinationDistance, remainingDistance;

    void Start()
    {
        mainCamera = Camera.main.gameObject;
        movement = mainCamera.GetComponent<PlayerMovement>();
        previousHit = gameObject;
        isTouchingDown = false;
        isTouchingRight = false;
        destinationDistance = 0;
        remainingDistance = 0;
    }

	private void Update()
    {
        
    }

    public void CollisionDetection()
    {
        RaycastHit hit;
        Ray rayForward = new Ray(transform.position, Vector3.forward);
        Ray rayBack = new Ray(transform.position, Vector3.back);
        Ray rayLeft = new Ray(transform.position, Vector3.left);
        Ray rayRight = new Ray(transform.position, Vector3.right);

        if (Physics.Raycast(rayForward, out hit))
        {
            if (hit.distance < 0.51f && hit.collider.name != "ThirdPersonController" && hit.collider.tag != "floor")
            {
                //movement.ResetPosition(transform.position);
                Destroy(Camera.main.GetComponent<PlayerMovement>().instantiatedPointer);
                Debug.Log("OBJECT IN CLOSE VACINITY FRONT: " + hit.distance + ", " + hit.collider.name);
            }

            Debug.DrawRay(rayForward.origin, rayForward.direction);
        }

        if (Physics.Raycast(rayBack, out hit))
        {
            if (hit.distance < 0.51f && hit.collider.name != "ThirdPersonController" && hit.collider.tag != "floor")
            {
                isTouchingDown = true;
                //movement.ResetPosition(transform.position);
                Destroy(Camera.main.GetComponent<PlayerMovement>().instantiatedPointer);
                Debug.Log("OBJECT IN CLOSE VACINITY BACK: " + hit.distance + ", " + hit.collider.name);
                //need to cast from this hit object to the left and right and add those hit objects to the list as well
                RayCastFromHitObjectDown(hit.collider.gameObject);
                AddToList(hit.collider.gameObject, "Down");
            }
            else
            {
                isTouchingDown = false;
                ClearList();
            }

            Debug.DrawRay(rayBack.origin, rayBack.direction);
        }
        else
        {
            isTouchingDown = false;
            ClearList();
        }

        if (Physics.Raycast(rayLeft, out hit))
        {
            if (hit.distance < 0.51f && hit.collider.name != "ThirdPersonController" && hit.collider.tag != "floor")
            {
                //movement.ResetPosition(transform.position);
                Destroy(Camera.main.GetComponent<PlayerMovement>().instantiatedPointer);
                Debug.Log("OBJECT IN CLOSE VACINITY LEFT: " + hit.distance + ", " + hit.collider.name);
            }

            Debug.DrawRay(rayLeft.origin, rayLeft.direction);
        }

        if (Physics.Raycast(rayRight, out hit))
        {
            if (hit.distance < 0.51f && hit.collider.name != "ThirdPersonController" && hit.collider.tag != "floor")
            {
                isTouchingRight = true;
                //movement.ResetPosition(transform.position);
                Destroy(Camera.main.GetComponent<PlayerMovement>().instantiatedPointer);
                Debug.Log("OBJECT IN CLOSE VACINITY RIGHT: " + hit.distance + ", " + hit.collider.name);
                //same as earlier only the axies change. Need to cast from the hit object up and down.
                RayCastFromHitObjectRight(hit.collider.gameObject);
                AddToList(hit.collider.gameObject, "Right");
            }
            else
            {
                isTouchingRight = false;
                ClearList();
            }

            Debug.DrawRay(rayRight.origin, rayRight.direction);
        }
        else
        {
            isTouchingRight = false;
            ClearList();
        }
    }

    private void AddToList(GameObject addThis, string direction)
    {
        if (direction == "Down")
        {
            if (!checkContents(addThis))
            {
                obscureListDown.Add(addThis);
                obscureMatListDown.Add(addThis.GetComponent<MeshRenderer>().material);
                addThis.GetComponent<MeshRenderer>().material = seeThroughMat;
            }
            else
            {
                //nothing
                Debug.Log("ALREADY IN LIST");
            }
        }

        if (direction == "Right")
        {
            if (!checkContents(addThis))
            {
                obscureListRight.Add(addThis);
                obscureMatListRight.Add(addThis.GetComponent<MeshRenderer>().material);
                addThis.GetComponent<MeshRenderer>().material = seeThroughMat;
            }
            else
            {
                //nothing
                Debug.Log("ALREADY IN LIST");
            }
        }
    }

    private bool checkContents(GameObject checkThis)
    {
        if (obscureListDown.Contains(checkThis) || obscureListRight.Contains(checkThis))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RayCastFromHitObjectDown(GameObject castFromThis)
    {
        Debug.Log("OI, THIS WANKER ON THE BOTTOM IS IN THE WAY!");
        Vector3 castPoint = new Vector3(castFromThis.transform.position.x, 0.0f, castFromThis.transform.position.z);
        RaycastHit hit;
        Ray rayLeft = new Ray(castPoint, Vector3.left);
        Ray rayRight = new Ray(castPoint, Vector3.right);

        if (Physics.Raycast(rayLeft, out hit))
        {
            if (hit.distance < 0.51f)
            {
                AddToList(hit.collider.gameObject, "Down");
            } 
        }

        if (Physics.Raycast(rayRight, out hit))
        {
            if (hit.distance < 0.51f)
            {
                AddToList(hit.collider.gameObject, "Down");
            }
        }
    }

    private void RayCastFromHitObjectRight(GameObject castFromThis)
    {
        Debug.Log("OI, THIS WANKER TO THE RIGHT IS IN THE WAY!");
        Vector3 castPoint = new Vector3(castFromThis.transform.position.x, 0.0f, castFromThis.transform.position.z);
        RaycastHit hit;
        Ray rayUp = new Ray(castPoint, Vector3.forward);
        Ray rayDown = new Ray(castPoint, Vector3.back);

        if (Physics.Raycast(rayUp, out hit))
        {
            if (hit.distance < 0.51f)
            {
                AddToList(hit.collider.gameObject, "Right");
            }
        }

        if (Physics.Raycast(rayDown, out hit))
        {
            if (hit.distance < 0.51f)
            {
                AddToList(hit.collider.gameObject, "Right");
            }
        }
    }

    public void ClearList()
    {
        if (obscureListDown.Count > 0 && !isTouchingDown)
        {
            for (int i = 0; i < obscureListDown.Count; i++)
            {
                obscureListDown[i].GetComponent<Renderer>().material = obscureMatListDown[i];
                //obscureList[i].layer = 0;
            }
            obscureListDown.Clear();
            obscureMatListDown.Clear();
        }
        else if (obscureListRight.Count > 0 && !isTouchingRight)
        {
            for (int i = 0; i < obscureListRight.Count; i++)
            {
                obscureListRight[i].GetComponent<Renderer>().material = obscureMatListRight[i];
                //obscureList[i].layer = 0;
            }
            obscureListRight.Clear();
            obscureMatListRight.Clear();
        }
    }

    public void ForceClearList()
    {
        if (obscureListDown.Count > 0)
        {
            for (int i = 0; i < obscureListDown.Count; i++)
            {
                obscureListDown[i].GetComponent<Renderer>().material = obscureMatListDown[i];
                //obscureList[i].layer = 0;
            }
        }
        else if (obscureListRight.Count > 0)
        {
            for (int i = 0; i < obscureListRight.Count; i++)
            {
                obscureListRight[i].GetComponent<Renderer>().material = obscureMatListRight[i];
            }
        }

        isTouchingRight = false;
        isTouchingDown = false;
        previousHit = gameObject;
        obscureListDown.Clear();
        obscureMatListDown.Clear();
        obscureListRight.Clear();
        obscureMatListRight.Clear();
    }
}
