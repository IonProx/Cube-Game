using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    public GridManager gridManager;

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
                return RetracePath(startNode, targetNode);
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
        GridManager.Node bestNode = nodes[0];
        foreach (var node in nodes) {
            if (node.fCost < bestNode.fCost ||
                (node.fCost == bestNode.fCost && node.hCost < bestNode.hCost)) {
                bestNode = node;
            }
        }
        return bestNode;
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
            for (int z = -1; z <= 1; z++) {
                if (Mathf.Abs(x) == Mathf.Abs(z)) continue;

                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;

                if (checkX >= 0 && checkX < gridManager.gridWidth && checkZ >= 0 && checkZ < gridManager.gridHeight) {
                    neighbors.Add(gridManager.gridNodes[checkX, checkZ]);
                }
            }
        }

        return neighbors;
    }

    private int GetDistance(GridManager.Node a, GridManager.Node b) {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridZ - b.gridZ);
    }
}
