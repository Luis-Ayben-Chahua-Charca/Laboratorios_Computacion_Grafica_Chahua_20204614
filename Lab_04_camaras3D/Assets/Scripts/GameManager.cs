using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject victoryCanvas;

    private bool gameEnded = false;

    void Start()
    {
        victoryCanvas.SetActive(false);
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        victoryCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}