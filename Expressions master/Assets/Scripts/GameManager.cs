using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    public bool IsGameActive = false;   

    public void ActivateGame()
    {
        IsGameActive = true;
        Cursor.visible = false;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}