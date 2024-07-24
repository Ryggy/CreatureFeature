using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

public class WorldGeneration : MonoBehaviour
{
    public int gridSizeX;
    public int gridSizeZ;
    public GameObject floor;
    public GameObject character;
    public GameObject[] prefabs;
    public List<GameObject> spawnedObstacles = new List<GameObject>();
    public float noiseScale = 1.0f;
    public int minWidth = 3;
    public int maxWidth = 10;
    public int minHeight = 3;
    public int maxHeight = 10;
    public int numberOfObjects = 5;

    public GridGraph gridGraph;
    public bool showOutline = true;
    public bool showConnections = true;
    
    public bool allowDiagonalMovement = true;
    
    void Start()
    {
        //Vector2Int rectanglePos = GenerateRectangleUsingPerlinNoise( noiseScale, minWidth, maxWidth, minHeight, maxHeight, gridSizeX, gridSizeZ);
        GenerateRectangle(noiseScale, minWidth, maxWidth, minHeight, maxHeight, out gridSizeX, out gridSizeZ);
        
        gridGraph = new GridGraph(gridSizeX, gridSizeZ);
        gridGraph.GenerateGrid(allowDiagonalMovement);
        
        SpawnGrid();
        PlaceObjectsInRectangle(maxWidth, maxHeight, prefabs, numberOfObjects);
        SpawnCharacter();
    }
    
    public void GenerateRectangle(float noiseScale, int minWidth, int maxWidth, int minHeight, int maxHeight, out int gridSizeX, out int gridSizeZ)
    {
        // float noiseX = Mathf.PerlinNoise(Time.time * noiseScale, 0);
        // float noiseZ = Mathf.PerlinNoise(0, Time.time * noiseScale);

        // int width = Mathf.RoundToInt(Mathf.Lerp(minWidth, maxWidth, noiseX));
        // int height = Mathf.RoundToInt(Mathf.Lerp(minHeight, maxHeight, noiseZ));
        
        // gridSizeX = Mathf.Max(width, height); // Ensure gridSizeX is at least as large as the width or height
        // gridSizeZ = Mathf.Max(width, height); // Same for gridSizeZ


        gridSizeX = Random.Range(minWidth, maxWidth + 1 );
        gridSizeZ = Random.Range(minHeight, maxHeight + 1 );
        
        // int x = Random.Range(0, gridSizeX - width + 1);
        // int z = Random.Range(0, gridSizeZ - height + 1);
        
        //return new Vector2Int(x, z);
    }

    public void SpawnGrid()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeZ; j++)
            {
                GridNode node = gridGraph.GetNodeFromIndex( i,  j);

                Vector3 position = gridGraph.GetWorldPositionFromNode(node);

                Instantiate(floor, position, Quaternion.identity);
            }
        }
    }
    public void PlaceObjectsInRectangle(int width, int height, GameObject[] prefabs, int numberOfObjects)
    {
        GridNode[,] grid = gridGraph.grid;
        List<GridNode> nodes = new List<GridNode>();
        
        for (int i = 0; i < grid.Length; i++)
        {
            // Convert 1D index to 2D coordinates
            int x = i % grid.GetLength(0); // Width of the grid (number of columns)
            int z = i / grid.GetLength(0); // Height of the grid (number of rows)
                
            // Ensure the indices are within the bounds of the grid
            if (x >= 0 && x < grid.GetLength(0) && z >= 0 && z < grid.GetLength(1))
            {
                nodes.Add(grid[x, z]);
            }
        }
        
        // Shuffle nodes to randomize placement
        nodes = nodes.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < numberOfObjects && i < nodes.Count; i++)
        {
            GameObject randomObject = prefabs[Random.Range(0, prefabs.Length)];
            
            GridNode node = nodes[i];
            Vector3 position = gridGraph.GetWorldPositionFromNode(node) + new Vector3(0, 1, 0);
            
            GameObject clone = Instantiate(randomObject, position, Quaternion.identity);
            spawnedObstacles.Add(clone);
            
            gridGraph.UpdateNode(node.Position, false);
        }
   
    }

    private void SpawnCharacter()
    {
        Instantiate(character, new Vector3(0.5f, 1.5f, 0.5f), Quaternion.identity);
    }
    
    void OnDrawGizmos()
    {
        if (gridGraph == null) return;

        if (showOutline)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    GridNode node = gridGraph.grid[x, z];
                    
                    // Set Gizmos color based on walkability
                    if (node.Walkable)
                    {
                        Gizmos.color = Color.green; // Color for walkable nodes
                    }
                    else
                    {
                        Gizmos.color = Color.red; // Color for non-walkable nodes
                    }
                    
                    Vector3 nodePos = (new Vector3(node.Position.x + gridGraph.offset, gridGraph.offset, node.Position.y + gridGraph.offset) * gridGraph.nodeSize);
                    DrawSquareOutline(nodePos, gridGraph.nodeSize);
                }
            }
        }

        if (showConnections)
        {
            Gizmos.color = Color.blue;
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    GridNode node = gridGraph.grid[x, z];
                    if (!node.Walkable) continue;

                    List<Connection> connections = gridGraph.GetConnections(node);
                    foreach (var connection in connections)
                    {
                        Vector3 fromPos = new Vector3(node.Position.x + gridGraph.offset, gridGraph.offset, node.Position.y + gridGraph.offset) * gridGraph.nodeSize;
                        Vector3 toPos = new Vector3(connection.ToNode.Position.x + gridGraph.offset, gridGraph.offset, connection.ToNode.Position.y + gridGraph.offset) * gridGraph.nodeSize;
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
