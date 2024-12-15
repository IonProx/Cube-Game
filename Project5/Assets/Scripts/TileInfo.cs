using UnityEngine;

public class TileInfo : MonoBehaviour {
    public Vector2Int tilePosition;
    public Color originalColor = Color.white;
    public string hoveredColorHex = "#FFFF00";
    public Color hoveredColor;

    private Renderer tileRenderer;
    private GridManager.Node node;

    private void Start() {
        tileRenderer = GetComponent<Renderer>();
        originalColor = tileRenderer.material.color;
        SetHoveredColor(hoveredColorHex);
    }

    private void SetHoveredColor(string hex) {
        if (hex.StartsWith("#")) {
            hex = hex.Substring(1);
        }

        if (hex.Length == 6) {
            float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

            hoveredColor = new Color(r, g, b);
        } else {
            hoveredColor = Color.yellow;
        }
    }

    public void SetPosition(int x, int y) {
        tilePosition = new Vector2Int(x, y);
    }

    public void SetWalkable(bool isWalkable) {
        if (node != null) {
            node.isWalkable = isWalkable;
        }
    }

    public void Highlight() {
        tileRenderer.material.color = hoveredColor;
        CancelInvoke(nameof(ResetColor));
        Invoke(nameof(ResetColor), 0.1f);
    }

    private void ResetColor() {
        tileRenderer.material.color = originalColor;
    }

    public void SetNode(GridManager.Node node) {
        this.node = node;
    }
}
