using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;

    void Awake()
    {
        // Si ya hay una música sonando y este objeto es el nuevo (el impostor)
        if (_instance != null && _instance != this)
        {
            // 1. Apagamos el componente de audio INMEDIATAMENTE para que no suene ni 1 frame
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.enabled = false;
            }

            // 2. Destruimos el objeto
            Destroy(this.gameObject);
            
            // 3. Detenemos la lectura del código aquí mismo
            return;
        }

        // Si es el original de la primera escena, lo guardamos y lo hacemos inmortal
        _instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(this.gameObject);
    }
}