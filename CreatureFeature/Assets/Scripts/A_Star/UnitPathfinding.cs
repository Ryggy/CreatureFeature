using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPathfinding : MonoBehaviour
{
    public Transform target;
    private Pathfinding pathfinding;
    private GridGraph gridGraph;
    private List<GridNode> path;
    private int currentPathIndex;
    public float pathUpdateDistance = 0.5f; // Distance threshold to skip pathfinding

    void Start()
    {
        gridGraph = FindObjectOfType<GridGraph>();
        pathfinding = new Pathfinding(gridGraph);
        InvokeRepeating(nameof(FindPath), 0f, 1f); // Find path every 2 seconds
    }

    void FindPath()
    {
        // Check if the unit is within the update distance to the target
        if (Vector3.Distance(transform.position, target.position) > pathUpdateDistance)
        {
            path = pathfinding.FindPath(transform.position, target.position);
            currentPathIndex = 0;
        }
    }

    void Update()
    {
        if (path != null && currentPathIndex < path.Count)
        {
            Vector3 targetPosition = gridGraph.GetWorldPositionFromNode(path[currentPathIndex]);
            targetPosition.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);
            }
            else
            {
                currentPathIndex++;
            }
        }
    }
    
}
