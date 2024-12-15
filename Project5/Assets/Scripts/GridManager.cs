using UnityEngine;

public class GridManager : MonoBehaviour {
    public GameObject tilePrefab; // Prefab used for the grid tiles
    public int gridWidth = 10; // Width of the grid (in tiles)
    public int gridHeight = 10; // Height of the grid (in tiles)
    public float tileSize = 1f; // The size of each tile

    public Node[,] gridNodes; // Array holding the grid nodes (tiles)

    private void Start() {
        GenerateGrid(); // Generate the grid when the game starts
    }

    // Generates the grid by creating tiles and associating them with nodes
    void GenerateGrid() {
        gridNodes = new Node[gridWidth, gridHeight]; // Initialize the grid node array

        // Loop through the grid and create the tiles
        for (int x = 0; x < gridWidth; x++) {
            for (int z = 0; z < gridHeight; z++) {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x * tileSize, 0, z * tileSize), Quaternion.identity);
                tile.transform.parent = transform; // Make the tile a child of this GameObject

                // Create a node for each tile, assuming all tiles are walkable by default
                Node node = new Node(x, z, true);
                gridNodes[x, z] = node;

                // Set up additional logic for tile properties (like walkability)
                TileInfo tileInfo = tile.GetComponent<TileInfo>();
                if (tileInfo != null) {
                    tileInfo.SetPosition(x, z); // Set the tile's position
                    tileInfo.SetNode(node); // Link the tile with the node
                    tileInfo.SetWalkable(true); // Set the tile as walkable
                }
            }
        }

        // Optionally place obstacles after the grid is generated
        FindObjectOfType<ObstacleManager>()?.PlaceObstacles(gridWidth, gridHeight);
    }

    // Get the node at a specific world position (not grid position)
    public Node GetNodeFromWorldPosition(Vector3 worldPosition) {
        int x = Mathf.FloorToInt(worldPosition.x / tileSize); // Convert world position to grid x-coordinate
        int z = Mathf.FloorToInt(worldPosition.z / tileSize); // Convert world position to grid z-coordinate

        // Ensure the position is within grid bounds
        if (x >= 0 && x < gridWidth && z >= 0 && z < gridHeight) {
            return gridNodes[x, z];
        }
        return null; // Return null if the position is out of bounds
    }

    // Get the node at a specific grid position (x, z)
    public Node GetNodeFromGridPosition(Vector2Int gridPosition) {
        if (gridPosition.x >= 0 && gridPosition.x < gridWidth &&
            gridPosition.y >= 0 && gridPosition.y < gridHeight) {
            return gridNodes[gridPosition.x, gridPosition.y];
        }
        return null; // Return null if the position is invalid
    }

    // Update the walkability of a node at a given grid position
    public void SetNodeWalkability(Vector2Int gridPosition, bool isWalkable) {
        Node node = GetNodeFromGridPosition(gridPosition);
        if (node != null) {
            node.isWalkable = isWalkable;
        }
    }

    // Nested Node class to represent each tile in the grid
    public class Node {
        public int gridX, gridZ;
        public bool isWalkable;
        public int gCost, hCost;
        public int fCost => gCost + hCost; 
        public Node parent;

        // Constructor to initialize a node
        public Node(int gridX, int gridZ, bool isWalkable) {
            this.gridX = gridX;
            this.gridZ = gridZ;
            this.isWalkable = isWalkable;
        }
    }
}
