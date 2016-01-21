using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour
{
    private float moveSpeed = 3.0f;

    public static int WALL = 0;
    public static int FLOOR = 1;
    public static int START = 2;
    public static int GOAL = 3;

    private GameObject player;
    private PlayerMovement playerMovement;
    private Grid grid;
    public bool process;
    private MapPosition playerPosition, enemyPosition;
    public AStarNode currentNode, goal;
    List<AStarNode> closed = new List<AStarNode>();
    List<AStarNode> open = new List<AStarNode>();
    List<MapPosition> path;
    public AStarNode[,] nodeMap;
    public int xSize, zSize;
    public int[,] map;

    public int pathCount;

    public void Initialize()
    {
        process = false;
        playerMovement = Camera.main.GetComponent<PlayerMovement>();
        player = playerMovement.player;
        grid = playerMovement.GetFloor();
        xSize = grid.xSize;
        zSize = grid.zSize;
        nodeMap = grid.nodeMap;
        map = grid.map;

        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        process = true;
        path = FindPath();
        yield return new WaitForSeconds(0.2f);

        while (process)
        {
            Vector3 movePosition = new Vector3((float)path[path.Count - 1].xPos + 0.5f, 1.0f, (float)path[path.Count - 1].yPos + 0.5f);
            Quaternion targetRotation = Quaternion.LookRotation(movePosition - gameObject.transform.position);
            gameObject.transform.rotation = targetRotation;

            while (!gameObject.transform.position.Equals(movePosition))
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, movePosition, moveSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            if (path.Count > 0)
            {
                path.RemoveAt(path.Count - 1);
            }

            if (path.Count <= 1)
            {
                process = false;
            }
        }
    }

    private List<MapPosition> FindPath(MapPosition start, MapPosition goal)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < zSize; y++)
            {
                nodeMap[x, y].parent = null;
            }
        }

        AStarNode startNode = nodeMap[start.xPos, start.yPos];
        startNode.g = 0;
        startNode.f = startNode.g + MapPosition.EucludianDistance(start, goal);
        open.Add(startNode);

        while (open.Count > 0)
        {
            AStarNode currentNode = open[0];

            if (currentNode.position == goal)
            {
                path = new List<MapPosition>();

                int count = 0;

                while (currentNode.parent != null)
                {
                    count++;
                    path.Add(currentNode.position);
                    currentNode = nodeMap[currentNode.parent.xPos, currentNode.parent.yPos];
                    if (count > xSize * zSize * 6)
                    {
                        Debug.LogError("out of range");
                        break;
                    }
                }
                path.Add(start);
                return path;
            }

            open.RemoveAt(0);
            closed.Add(currentNode);

            List<AStarNode> neighbours = GetNeighbours(currentNode.position);

            foreach (AStarNode neighbour in neighbours)
            {
                if (closed.Contains(neighbour))
                {
                    continue;
                }

                float g = currentNode.g + neighbour.f;

                bool inOpenList = open.Contains(neighbour);
                if (!inOpenList || g < neighbour.g)
                {
                    neighbour.parent = currentNode.position;
                    neighbour.g = g;
                    neighbour.f = g + MapPosition.EucludianDistance(neighbour.position, goal);

                    if (!inOpenList)
                    {
                        int index = 0;
                        while (index < open.Count && open[index].f < neighbour.f)
                        {
                            index++;
                        }
                        open.Insert(index, neighbour);
                    }
                }
            }
        }

        return null;
    }

    public List<AStarNode> GetNeighbours(MapPosition current)
    {
        List<AStarNode> neighbours = new List<AStarNode>();

        if (current.yPos > 0 && map[current.xPos, current.yPos - 1] == FLOOR && !closed.Contains(nodeMap[current.xPos, current.yPos - 1]))
        {
            neighbours.Add(nodeMap[current.xPos, current.yPos - 1]);
        }

        if (current.yPos < zSize - 1 && map[current.xPos, current.yPos + 1] == FLOOR && !closed.Contains(nodeMap[current.xPos, current.yPos + 1]))
        {
            neighbours.Add(nodeMap[current.xPos, current.yPos + 1]);
        }

        if (current.xPos > 0 && map[current.xPos - 1, current.yPos] == FLOOR && !closed.Contains(nodeMap[current.xPos - 1, current.yPos]))
        {
            neighbours.Add(nodeMap[current.xPos - 1, current.yPos]);
        }

        if (current.xPos < xSize - 1 && map[current.xPos + 1, current.yPos] == FLOOR && !closed.Contains(nodeMap[current.xPos + 1, current.yPos]))
        {
            neighbours.Add(nodeMap[current.xPos + 1, current.yPos]);
        }

        return neighbours;
    }

    public List<MapPosition> FindPath()
    {
        if (path != null && path.Count > 0)
        {
            path.Clear();
        }
        closed.Clear();
        open.Clear();

        currentNode = nodeMap[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z)];
        goal = nodeMap[Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z)];
        path = FindPath(currentNode.position, goal.position);

        return path;
    }
}
