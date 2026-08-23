using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string introSceneName = "Intro";
    public string gameSceneName = "OtterVsRats";

    public void StartGame()
    {
        SceneManager.LoadScene(introSceneName);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");

        Application.Quit();
    }
}