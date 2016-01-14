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
    private int lastSelection;
    public int amountOfItems;
    public int SEED;

    void Start()
    {
        grid = GetComponent<Grid>();

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
            int selection = Random.Range(0, availibleGameObjects.Count - 1);

            GameObject temp = Instantiate(availibleGameObjects[selection], Vector3.zero, Quaternion.identity) as GameObject;
            takenSpots.Add(temp.transform.position);
            temp.transform.parent = gameObject.transform;
            temp.name = "" + amountOfItems;
            temp.isStatic = true;

            float heightGet = temp.GetComponent<MeshRenderer>().bounds.size.y / 2;

            if (lastSelection == selection)
            {
                temp.transform.position = takenSpots[takenSpots.Count - 1];
                temp.transform.position += new Vector3((int)Random.Range(-1, 1), 0.0f, (int)Random.Range(-1, 1));

                foreach (Vector3 item in takenSpots)
                {
                    if (item.x == temp.transform.position.x && item.z == temp.transform.position.z)
                    {
                        temp.transform.position += new Vector3((int)Random.Range(-1, 1), 0.0f, (int)Random.Range(-1, 1));
                    }
                }

                if (temp.transform.position.x > grid.xSize || temp.transform.position.x < 0)
                {
                    Destroy(temp);
                    //Debug.Log("REMOVING OBJECT OUT OF BOUNDS");
                }
                else if (temp.transform.position.z > grid.zSize || temp.transform.position.z < 0)
                {
                    Destroy(temp);
                    //Debug.Log("REMOVING OBJECT OUT OF BOUNDS");
                }
            }
            else
            {
                foreach (Vector3 item in takenSpots)
                {
                    while (item.x == temp.transform.position.x && item.z == temp.transform.position.z)
                    {
                        //Debug.Log("SWAPPING VECTORS FOR OBJECT: " + item);
                        temp.transform.position = new Vector3(Mathf.Floor(Random.Range(1, grid.xSize - 1)) + 0.5f, heightGet, Mathf.Floor(Random.Range(1, grid.zSize - 1)) + 0.5f);
                    }
                }
            }
            
            lastSelection = selection;
            amountOfItems--;

            if (amountOfItems == 0)
            {
                grid.OccupySpots(takenSpots);
            }

            Generate();
        }
    }
}
