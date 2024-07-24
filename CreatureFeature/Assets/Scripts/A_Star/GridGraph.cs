using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridGraph : MonoBehaviour
{
    public int gridSizeX;
    public int gridSizeZ;
    public float nodeSize = 1.0f;
    public float offset = 0.5f;
    
    public bool showOutline = true;
    public bool showConnections = true;
    public bool allowDiagonalMovement = true;
    
    public GridNode[,] grid;

    void Start()
    {
        GenerateGrid();
    }
    
    void GenerateGrid()
    {
        grid = new GridNode[gridSizeX, gridSizeZ];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector2Int position = new Vector2Int(x, z);
                bool walkable = true; // add logic to determine if a node is walkable
                grid[x, z] = new GridNode(position, walkable);
            }
        }
    }
    
    public List<Connection> GetConnections(GridNode node)
    {
        List<Connection> connections = new List<Connection>();

        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
        
        if (allowDiagonalMovement)
        {
            Vector2Int[] diagonalDirections = {
                new Vector2Int(1, 1),
                new Vector2Int(-1, -1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, 1)
            };
            directions = directions.Concat(diagonalDirections).ToArray();
        }
        
        foreach (var direction in directions)
        {
            Vector2Int neighborPos = node.Position + direction;
            if (IsValidPosition(neighborPos))
            {
                GridNode neighborNode = grid[neighborPos.x, neighborPos.y];
                if (neighborNode.Walkable)
                {
                    float cost = Vector2Int.Distance(node.Position, neighborPos);
                    connections.Add(new Connection(node, neighborNode, cost));
                }
            }
        }

        return connections;
    }

    bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridSizeX && position.y >= 0 && position.y < gridSizeZ;
    }
    
    public Vector3 GetWorldPositionFromNode(GridNode node) {
        // Add the offset to center the nodes 
        return new Vector3(node.Position.x + offset, 0, node.Position.y + offset) * nodeSize;
    }

    
    void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.green;

        if (showOutline)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    GridNode node = grid[x, z];
                    Vector3 nodePos = (new Vector3(node.Position.x + offset, 0, node.Position.y + offset) * nodeSize);
                    DrawSquareOutline(nodePos, nodeSize);
                }
            }
        }

        if (showConnections)
        {
            Gizmos.color = Color.red;
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    GridNode node = grid[x, z];
                    if (!node.Walkable) continue;

                    List<Connection> connections = GetConnections(node);
                    foreach (var connection in connections)
                    {
                        Vector3 fromPos = new Vector3(node.Position.x + offset, 0, node.Position.y + offset) * nodeSize;
                        Vector3 toPos = new Vector3(connection.ToNode.Position.x + offset, 0, connection.ToNode.Position.y + offset) * nodeSize;
                        Gizmos.DrawLine(fromPos, toPos);
                        Gizmos.DrawLine(fromPos, toPos);
                    }
                }
            }
        }
    }
    
    void DrawSquareOutline(Vector3 center, float size)
    {
        Vector3 halfSize = Vector3.one * (size / 2.0f);
        Vector3 topLeft = center + new Vector3(-halfSize.x, 0, halfSize.z);
        Vector3 topRight = center + new Vector3(halfSize.x, 0, halfSize.z);
        Vector3 bottomLeft = center + new Vector3(-halfSize.x, 0, -halfSize.z);
        Vector3 bottomRight = center + new Vector3(halfSize.x, 0, -halfSize.z);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
