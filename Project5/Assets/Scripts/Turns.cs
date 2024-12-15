using UnityEngine;

public class TurnManager : MonoBehaviour {
    private bool isPlayerTurn = true;

    public bool IsPlayerTurn() {
        return isPlayerTurn;
    }

    public bool IsEnemyTurn() {
        return !isPlayerTurn;
    }

    public void EndPlayerTurn() {
        isPlayerTurn = false;
    }

    public void EndEnemyTurn() {
        isPlayerTurn = true;
    }
}
