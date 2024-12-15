using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 5f;     
    public float moveDelay = 0f;   

    [Header("References")]
    private GridManager gridManager;             
    private EnemyPathfinding enemyPathfinding;    
    private PlayerController playerController;    

    private bool isMoving = false;     
    private Queue<GridManager.Node> currentPath;  
    private int tilesToMove = 0;     

    void Start() {
        // Get references to the required components
        gridManager = FindObjectOfType<GridManager>();
        enemyPathfinding = FindObjectOfType<EnemyPathfinding>();
        playerController = FindObjectOfType<PlayerController>();

        // Check that the required references are found
        if (gridManager == null) Debug.LogError("GridManager is missing! Please assign it in the Inspector.");
        if (enemyPathfinding == null) Debug.LogError("EnemyPathfinding is missing! Please assign it in the Inspector.");
        if (playerController == null) Debug.LogError("PlayerController is missing! Please assign it in the Inspector.");

        // Subscribe to the event when the player moves
        PlayerController.OnPlayerMove += HandlePlayerMove;
    }

    void OnDestroy() {
        
        PlayerController.OnPlayerMove -= HandlePlayerMove;
    }

    // This function gets called whenever the player moves
    private void HandlePlayerMove(int tilesMoved) {
        if (isMoving || tilesMoved <= 0) return;  // If the enemy is already moving, do nothing

        tilesToMove = tilesMoved;  // Set the number of tiles the enemy will move

        Vector3 playerPosition = playerController.transform.position;
        List<GridManager.Node> path = enemyPathfinding.FindPath(transform.position, playerPosition);

        if (path != null && path.Count > 0) {
            // Ensure the enemy moves only as many tiles as available
            int actualTilesToMove = Mathf.Min(tilesToMove, path.Count);
            currentPath = new Queue<GridManager.Node>(path.GetRange(0, actualTilesToMove));  // Create a queue of nodes

            // Start moving along the path
            StartCoroutine(MoveAlongPath());  
        } else {
            Debug.Log("EnemyAI: No path found to the player.");
        }
    }

    // The enemy moves step by step along the calculated path
    private IEnumerator MoveAlongPath() {
        isMoving = true;


        while (currentPath.Count > 0) {
            GridManager.Node nextNode = currentPath.Dequeue();
            Vector3 targetPosition = new Vector3(
                nextNode.gridX * gridManager.tileSize,
                1f,  // Assuming a fixed height for the enemy
                nextNode.gridZ * gridManager.tileSize
            );


            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // If the enemy gets too close to the player, stop moving to avoid collision
                if (Vector3.Distance(transform.position, playerController.transform.position) < 1.0f) {
                    Debug.Log("EnemyAI: Stopped to avoid collision with the player.");
                    isMoving = false;
                    yield break;  
                }

                yield return null;
            }

            // Wait before moving to the next tile
            yield return new WaitForSeconds(moveDelay);
        }

        Debug.Log("EnemyAI: Finished moving " + tilesToMove + " tiles.");
        isMoving = false;  // Movement is done
    }
}
