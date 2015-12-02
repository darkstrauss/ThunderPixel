using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

    public List<GameObject> floors;
    public List<GameObject> doors;
    public GameObject player;
    public GameObject activeFloor;

    void Start()
    {
        activeFloor = GameObject.FindGameObjectWithTag("floor");
    }

	void Update () {
        if (Input.GetMouseButton(0))
        {
            //refered to object that the raycast hits.
            RaycastHit hit;

            //how the ray casts. In this instance its from the middle of the main camera.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if the ray hits something
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
                if (hit.collider.tag == "floor")
                {
                    player.transform.position = Vector3.Lerp(player.transform.position, hit.point, 0.01f);
                }
            }
        }
    }

    public void ClearRoomList()
    {
        doors.Clear();
    }
}
