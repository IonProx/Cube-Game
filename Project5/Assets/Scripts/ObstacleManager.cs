using UnityEngine;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour {
    public GameObject obstaclePrefab; // The prefab for the obstacle object
    private HashSet<Vector2Int> blockedPositions = new HashSet<Vector2Int>(); // Keeps track of positions that are blocked by obstacles

    // Randomly places obstacles on the grid
    public void PlaceObstacles(int gridWidth, int gridHeight) {
        int obstaclesToPlace = 5; // Number of obstacles to place on the grid
        blockedPositions.Clear(); // Clear any previously placed obstacles

        for (int i = 0; i < obstaclesToPlace; i++) {
            Vector2Int position;

            // Keep picking random positions until we find one that's not already blocked
            do {
                int x = Random.Range(0, gridWidth); // Get a random x-coordinate
                int y = Random.Range(0, gridHeight); // Get a random y-coordinate
                position = new Vector2Int(x, y);
            }
            while (blockedPositions.Contains(position)); // Ensure no duplicate obstacle positions

            blockedPositions.Add(position);
            Vector3 worldPosition = new Vector3(position.x, 1, position.y); 
            Instantiate(obstaclePrefab, worldPosition, Quaternion.identity); 

            // Update the grid to mark this position as unwalkable
            var node = FindObjectOfType<GridManager>().GetNodeFromGridPosition(position);
            if (node != null) {
                node.isWalkable = false; // Mark the node as unwalkable
            }
        }
    }

    // Check if a specific grid tile is blocked
    public bool IsTileBlocked(Vector2Int position) {
        return blockedPositions.Contains(position); // Return true if the tile is blocked
    }

    // Clears all obstacles from the grid
    public void ClearObstacles() {
        blockedPositions.Clear(); // Clear all blocked positions
    }
}
