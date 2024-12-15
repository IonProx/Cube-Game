using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour {
    public GridManager gridManager;
    private ObstacleManager obstacleManager;

    void Start() {
        obstacleManager = FindObjectOfType<ObstacleManager>();
    }

    public List<GridManager.Node> FindPath(Vector3 startPos, Vector3 targetPos) {
        var startNode = gridManager.GetNodeFromWorldPosition(startPos);
        var targetNode = gridManager.GetNodeFromWorldPosition(targetPos);

        if (startNode == null || targetNode == null || !targetNode.isWalkable) {
            Debug.LogError("Invalid path or the target node is blocked.");
            return null;
        }

        var openSet = new List<GridManager.Node> { startNode };
        var closedSet = new HashSet<GridManager.Node>();

        while (openSet.Count > 0) {
            var currentNode = GetLowestFCostNode(openSet);
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                return RetracePath(startNode, currentNode);
            }

            foreach (var neighbor in GetNeighbors(currentNode)) {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private GridManager.Node GetLowestFCostNode(List<GridManager.Node> nodes) {
        GridManager.Node lowestFCostNode = nodes[0];
        foreach (var node in nodes) {
            if (node.fCost < lowestFCostNode.fCost ||
                (node.fCost == lowestFCostNode.fCost && node.hCost < lowestFCostNode.hCost)) {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }

    private List<GridManager.Node> RetracePath(GridManager.Node startNode, GridManager.Node endNode) {
        var path = new List<GridManager.Node>();
        var currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private List<GridManager.Node> GetNeighbors(GridManager.Node node) {
        var neighbors = new List<GridManager.Node>();

        for (int x = -1; x <= 1; x++) {
            AddNeighborIfValid(node.gridX + x, node.gridZ, neighbors);
        }
        for (int z = -1; z <= 1; z++) {
            AddNeighborIfValid(node.gridX, node.gridZ + z, neighbors);
        }

        return neighbors;
    }

    private void AddNeighborIfValid(int x, int z, List<GridManager.Node> neighbors) {
        if (x < 0 || x >= gridManager.gridWidth || z < 0 || z >= gridManager.gridHeight) return;

        GridManager.Node neighborNode = gridManager.gridNodes[x, z];
        if (neighborNode != null && neighborNode.isWalkable) {
            neighbors.Add(neighborNode);
        }
    }

    private int GetDistance(GridManager.Node a, GridManager.Node b) {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridZ - b.gridZ);
    }
}
