using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GenerateContent : MonoBehaviour
{
    public List<GameObject> availibleGameObjects;
    public List<Vector3> takenSpots;
    public Text text;
    private Grid grid;
    public int amountOfItems;
    public int SEED;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        
    }

    void Start()
    {
        if (SEED == 0)
        {
            SEED = Random.seed;
            Random.seed = SEED;
        }
        else
        {
            Random.seed = SEED;
        }

        if (text == null)
        {
            text = GameObject.FindGameObjectWithTag("Text").GetComponent<Text>();
        }
        if (text != null)
        {
            text.text = "" + SEED;
        }
        
        
        Debug.Log("LEVEL SEED: " + SEED);

        Generate();
    }

    private void Generate()
    {
        if (amountOfItems > 0)
        {
            GameObject temp = Instantiate(availibleGameObjects[Random.Range(0, availibleGameObjects.Count)], Vector3.zero, Quaternion.identity) as GameObject;
            
            temp.transform.position = new Vector3(Mathf.Floor(Random.Range(1, grid.xSize - 1)) + 0.5f, temp.GetComponent<MeshRenderer>().bounds.size.y / 2.0f, Mathf.Floor(Random.Range(1, grid.zSize - 1)) + 0.5f);

            for (int i = 0; i < takenSpots.Count; i++)
            {
                if (takenSpots[i].Equals(temp.transform.position))
                {
                    temp.transform.position = new Vector3(Mathf.Floor(Random.Range(1, grid.xSize - 1)) + 0.5f, temp.GetComponent<MeshRenderer>().bounds.size.y / 2.0f, Mathf.Floor(Random.Range(1, grid.zSize - 1)) + 0.5f);
                    break;
                }
            }

            takenSpots.Add(temp.transform.position);
            temp.transform.parent = gameObject.transform;
            temp.name = "" + amountOfItems;
            temp.isStatic = true;
            amountOfItems--;
            Generate();
        }
    }

    void Update()
    {
        
    }
}
