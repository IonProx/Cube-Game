using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public Text tileInfoText; // Assign in Inspector

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void UpdateTileInfo(Vector2Int position) {
        tileInfoText.text = $"Tile Location: ({position.x}, {position.y})";
    }
}
