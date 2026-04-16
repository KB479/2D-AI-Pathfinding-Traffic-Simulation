using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Ayarlarý")]
    public Vector2 gridWorldSize = new Vector2(24, 24); 
    public float nodeRadius = 0.5f; 
    public LayerMask unwalkableMask; 

    private Node[,] grid;
    private float nodeDiameter; 
    private int gridSizeX;
    private int gridSizeY; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position
                                - Vector3.right * gridWorldSize.x / 2
                                - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft
                                   + Vector3.right * (x * nodeDiameter + nodeRadius)
                                   + Vector3.forward * (y * nodeDiameter + nodeRadius);

                worldPoint.y = 0f;

                Vector3 checkPoint = worldPoint + Vector3.up * 0.5f;
                bool walkable = !(Physics.CheckSphere(checkPoint, nodeRadius - 0.1f, unwalkableMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }

        ApplyTrafficDirections();
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                if (Mathf.Abs(x) + Mathf.Abs(y) != 1) continue; 


                if (node.trafficDirection != Vector2.zero)
                {
                    if (x != (int)node.trafficDirection.x || y != (int)node.trafficDirection.y)
                    {
                        continue;
                    }
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }


    void ApplyTrafficDirections()
    {
        RoadDirection[] roads = FindObjectsOfType<RoadDirection>();

        foreach (RoadDirection road in roads)
        {
            Bounds bounds = road.GetComponent<BoxCollider>().bounds;

            Node bottomLeft = NodeFromWorldPoint(bounds.min);
            Node topRight = NodeFromWorldPoint(bounds.max);

            int startX = Mathf.Clamp(bottomLeft.gridX, 0, gridSizeX - 1);
            int endX = Mathf.Clamp(topRight.gridX, 0, gridSizeX - 1);
            int startY = Mathf.Clamp(bottomLeft.gridY, 0, gridSizeY - 1);
            int endY = Mathf.Clamp(topRight.gridY, 0, gridSizeY - 1);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (bounds.Contains(grid[x, y].worldPosition))
                    {
                        grid[x, y].trafficDirection = road.direction;
                    }
                }
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0.1f, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n.walkable)
                {
                    Gizmos.color = new Color(1, 1, 1, 0.2f);
                    Vector3 gizmoPos = n.worldPosition + Vector3.up * 0.05f;
                    Gizmos.DrawCube(gizmoPos, new Vector3(nodeDiameter - 0.1f, 0.1f, nodeDiameter - 0.1f));
                }
                else
                {
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    Vector3 gizmoPos = n.worldPosition + Vector3.up * 1.05f;
                    Gizmos.DrawCube(gizmoPos, new Vector3(nodeDiameter - 0.1f, 0.1f, nodeDiameter - 0.1f));
                }
            }
        }
    }
}