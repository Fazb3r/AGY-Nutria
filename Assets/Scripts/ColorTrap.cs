using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorTrap : MonoBehaviour
{
    [Header("Índice requerido para cruzar (Ej: Amarillo = 2 o el que corresponda)")]
    public int requiredSignalIndex; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si la que entró es la nutria
        OtterController otter = collision.GetComponent<OtterController>();
        if (otter != null)
        {
            // Buscamos la ruleta en la escena para ver qué color/índice está activo ahora mismo
            RouletteSignals ruleta = FindObjectOfType<RouletteSignals>();
            
            if (ruleta != null)
            {
                // Comprobamos si el  índice activo en la ruleta coincide con el requerido
                if (ruleta.GetCurrentSelectedIndex() == requiredSignalIndex)
                {
                    Debug.Log("¡Color correcto! La nutria cruza a salvo.");
                }
                else
                {
                    Debug.Log("¡Color incorrecto! Reiniciando nivel...");
                    // Recarga la escena actual desde el inicio
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }
}