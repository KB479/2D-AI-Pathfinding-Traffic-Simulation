using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string GAME_SCENE = "GameScene"; 

    public void PlayGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyundan Çýkýldý!");
    }
}