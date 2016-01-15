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
    private int lastSelection = -1;
    public int amountOfItems;
    public int SEED;
    private bool placedFountain = false;

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
    }

    public void OnRenderObject()
    {
        Generate();
    }

    private void Generate()
    {
        if (amountOfItems > 0)
        {
            int selection = Random.Range(0, availibleGameObjects.Count);

            GameObject temp = Instantiate(availibleGameObjects[selection], Vector3.zero, Quaternion.identity) as GameObject;
            temp.transform.parent = gameObject.transform;
            temp.name = "" + amountOfItems;
            temp.isStatic = true;

            float heightGet = 0;

            temp.transform.position = new Vector3(Mathf.Floor(Random.Range(1, grid.xSize - 1)) + 0.5f, heightGet, Mathf.Floor(Random.Range(1, grid.zSize - 1)) + 0.5f);

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

                if (temp.transform.position.x > grid.xSize || temp.transform.position.x < 0 || temp.transform.position.z > grid.zSize || temp.transform.position.z < 0)
                {
                    Destroy(temp);
                    //Debug.Log("REMOVING OBJECT OUT OF BOUNDS");
                }
            }
            else if (selection == availibleGameObjects.Count - 1 && placedFountain == false)
            {
                Debug.Log("PLACING FOUNTAIN");
                temp.transform.position = new Vector3(Mathf.Floor(grid.xSize / 2) + 0.5f, heightGet, Mathf.Floor(grid.zSize / 2) + 0.5f);
                List<Vector3> neighbours = getNeighbours(temp.transform.position);

                for (int i = 0; i < neighbours.Count; i++)
                {
                    takenSpots.Add(neighbours[i]);
                }

                availibleGameObjects.RemoveAt(selection);
                placedFountain = true;
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
            takenSpots.Add(temp.transform.position);

            if (amountOfItems == 0)
            {
                Debug.Log("adding " + "#" + takenSpots.Count +  " takenspots to the map");
                
                grid.OccupySpots(takenSpots);
            }
        }
    }

    private List<Vector3> getNeighbours(Vector3 currentPosition)
    {
        List<Vector3> neighbours = new List<Vector3>();

        Vector3 up = new Vector3(currentPosition.x, 0.0f, currentPosition.z + 1.0f);
        Vector3 down = new Vector3(currentPosition.x, 0.0f, currentPosition.z - 1.0f);
        Vector3 right = new Vector3(currentPosition.x + 1.0f, 0.0f, currentPosition.z);
        Vector3 left = new Vector3(currentPosition.x - 1.0f, 0.0f, currentPosition.z);

        neighbours.Add(up);
        neighbours.Add(down);
        neighbours.Add(right);
        neighbours.Add(left);

        return neighbours;
    }
}
