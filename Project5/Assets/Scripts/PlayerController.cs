using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f; 
    private List<GridManager.Node> currentPath; 
    private bool isMoving = false; 
    private Vector3 targetPosition; 

    public Pathfinding pathfinding;
    public Text warningText;
    private float warningTime = 2f; 

    public static event System.Action<int> OnPlayerMove; 
    public bool canMove = true; 

    void Start() {
        // Ensure Pathfinding is assigned and set up correctly
        if (pathfinding == null) {
            pathfinding = FindObjectOfType<Pathfinding>();
            if (pathfinding == null) {
                Debug.LogError("Pathfinding is missing! Make sure it's assigned.");
                enabled = false; // Disable the script if Pathfinding is not set up
                return;
            }
        }

        targetPosition = transform.position; // Set the initial target position as the player's starting position

        // Hide the warning text at the start
        if (warningText != null) {
            warningText.gameObject.SetActive(false);
        }
    }

    void Update() {
        // Allow movement only if canMove is true
        if (canMove) {
            HandleInput(); // Check for player input
            MovePlayer(); // Move the player towards the target position
        }
    }

    // Handle player input (clicking on the grid)
    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) { // Left mouse button clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) { 
                TileInfo tileInfo = hit.collider.GetComponent<TileInfo>();
                if (tileInfo != null) {
                    Vector3 clickedPosition = hit.collider.transform.position; 
                    GridManager.Node targetNode = pathfinding.gridManager.GetNodeFromWorldPosition(clickedPosition); 

                    // If the node is walkable, set the path; otherwise, show a warning
                    if (targetNode != null && targetNode.isWalkable) {
                        SetPath(clickedPosition); // Set the path to the target tile
                    } else {
                        ShowWarning("This tile is not walkable."); // Show warning if the tile is blocked
                    }
                }
            }
        }
    }

    // Set the path towards the target position
    private void SetPath(Vector3 targetPosition) {
        currentPath = pathfinding.FindPath(transform.position, targetPosition); // Find the path from the player's position to the target
        if (currentPath != null && currentPath.Count > 0) {
            // If a valid path is found, set the target position and start moving
            int tilesToMove = currentPath.Count;
            this.targetPosition = new Vector3(
                currentPath[0].gridX * pathfinding.gridManager.tileSize,
                1f, // Set the Y position to a constant value (adjust if necessary)
                currentPath[0].gridZ * pathfinding.gridManager.tileSize
            );

            isMoving = true; // Start moving
            OnPlayerMove?.Invoke(tilesToMove); // Trigger the movement event
        } else {
            ShowWarning("No valid path to the target."); // Show a warning if no valid path is found
        }
    }

    // Move the player along the path
    private void MovePlayer() {
        if (isMoving && currentPath != null && currentPath.Count > 0) {
            // Move the player towards the current target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if the player has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
                currentPath.RemoveAt(0); // Remove the first node from the path (since the player reached it)

                // If there are more nodes in the path, update the target position; otherwise, stop moving
                if (currentPath.Count > 0) {
                    targetPosition = new Vector3(
                        currentPath[0].gridX * pathfinding.gridManager.tileSize,
                        1f,
                        currentPath[0].gridZ * pathfinding.gridManager.tileSize
                    );
                } else {
                    isMoving = false; // Stop moving when the path is complete
                }
            }
        }
    }

    // Show a warning message in the UI
    private void ShowWarning(string message) {
        if (warningText != null) {
            warningText.text = message; // Set the warning text
            warningText.gameObject.SetActive(true); // Show the warning text
            Invoke(nameof(HideWarning), warningTime); // Hide the warning after a certain time
        }
    }

    // Hide the warning message
    private void HideWarning() {
        if (warningText != null) {
            warningText.gameObject.SetActive(false); // Hide the warning text
        }
    }
}
