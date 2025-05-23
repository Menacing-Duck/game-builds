using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        AutoDetectGridSize();
        CreateGrid();
    }

    void AutoDetectGridSize()
    {
        Collider2D[] colliders = FindObjectsOfType<Collider2D>();

        if (colliders.Length == 0)
        {
            return;
        }

        Bounds bounds = colliders[0].bounds;

        foreach (Collider2D col in colliders)
        {
            bounds.Encapsulate(col.bounds);
        }

        gridWorldSize = new Vector2(bounds.size.x, bounds.size.y);
        transform.position = bounds.center;
        
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius)
                                                      + Vector2.up * (y * nodeDiameter + nodeRadius);

                Collider2D hit = Physics2D.OverlapCircle(worldPoint, nodeRadius, obstacleMask);
                bool walkable = (hit == null);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x - transform.position.x + gridWorldSize.x / 2f) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.y - transform.position.y + gridWorldSize.y / 2f) / gridWorldSize.y);

        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSizeY - 1) * percentY), 0, gridSizeY - 1);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
}
