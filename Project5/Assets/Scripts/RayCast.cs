using UnityEngine;

public class RayCast : MonoBehaviour {
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            TileInfo tile = hit.collider.GetComponent<TileInfo>();
            if (tile != null) {
                tile.Highlight();
                UIManager.Instance.UpdateTileInfo(tile.tilePosition);
            }
        }
    }
}
