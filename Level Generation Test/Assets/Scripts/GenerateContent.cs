using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateContent : MonoBehaviour
{
    public List<GameObject> availibleGameObjects;
    private Grid grid;
    public int amountOfItems;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Start()
    {
        if (amountOfItems > 0)
        {
            GameObject temp = Instantiate(availibleGameObjects[Random.Range(0, availibleGameObjects.Count)], Vector3.zero, Quaternion.identity) as GameObject;
            temp.transform.position = new Vector3(Mathf.Floor(Random.Range(1, grid.xSize - 1)) + 0.5f, temp.GetComponent<MeshRenderer>().bounds.size.y / 2.0f, Mathf.Floor(Random.Range(1, grid.zSize - 1)) + 0.5f);
            temp.transform.parent = gameObject.transform;
            temp.isStatic = true;
            amountOfItems--;
            Start();
        }
    }

    void Update()
    {
        
    }
}
