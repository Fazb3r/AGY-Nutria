using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class LevelExit : MonoBehaviour
{
    [Header("Nombre de la siguiente escena")]
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si el que tocó la reja es la nutria
        if (collision.CompareTag("Player") || collision.GetComponent<OtterController>() != null)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("¡Falta escribir el nombre de la escena en el Inspector!");
            }
        }
    }
}