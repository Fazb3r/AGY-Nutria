using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RouletteSignals : MonoBehaviour
{
    [Header("Level Progression")]
    [Tooltip("Cuántas secciones están disponibles en esta escena (Ej: 1 = solo la primera)")]
    public int seccionesDesbloqueadas = 4;

    [Header("Visual Feedback")]
    public Image recuadroEquipado; 
    public Color[] sectionColors;
    public Image[] slices; 

    [Header("Game References")]
    public GameObject soundWavePrefab; 
    
    [Header("Audio & Cooldown Settings")]
    public float clickCooldown = 1.0f; 
    
    [Space(10)]
    public AudioClip sonidoAtraerNutria;   // Índice 0
    public AudioClip sonidoRepelerNutria;  // Índice 1
    public AudioClip sonidoRatasA;         // Índice 2 
    public AudioClip sonidoRatasB;         // Índice 2 
    public AudioClip sonidoIndex3;         // Índice 3 

    private int _selectedSection = 0; 
    private Camera _mainCamera;
    
    private float _lastClickTime = -999f;
    private AudioSource _audioSource;
    private bool _alternarSonidoRatas = true;

    void Start()
    {
        _mainCamera = Camera.main;
        
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Evita que inicie en un índice bloqueado
        if (_selectedSection >= seccionesDesbloqueadas) 
        {
            _selectedSection = 0;
        }

        ActualizarUI(); 
    }

    void Update()
    {
        if (slices.Length == 0 || sectionColors.Length == 0) return;
        
        // Limita la navegación solo a los índices disponibles
        int disponibles = Mathf.Clamp(seccionesDesbloqueadas, 1, slices.Length);

        float scrollValue = Mouse.current.scroll.y.ReadValue();
        
        if (scrollValue > 0) 
        {
            _selectedSection++;
            if (_selectedSection >= disponibles) _selectedSection = 0; 
            ActualizarUI();
        }
        else if (scrollValue < 0) 
        {
            _selectedSection--;
            if (_selectedSection < 0) _selectedSection = disponibles - 1; 
            ActualizarUI();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Time.time >= _lastClickTime + clickCooldown)
            {
                _lastClickTime = Time.time;
                DispararSenal();
            }
        }
    }

    private void ActualizarUI()
    {
        if (recuadroEquipado != null && _selectedSection < sectionColors.Length)
        {
            recuadroEquipado.color = sectionColors[_selectedSection];
        }

        for (int i = 0; i < slices.Length; i++)
        {
            if (slices[i] != null)
            {
                // Si el índice actual (i) es mayor o igual a lo que está desbloqueado, lo apaga y oculta
                if (i >= seccionesDesbloqueadas)
                {
                    slices[i].gameObject.SetActive(false);
                    continue; 
                }

                // Si está desbloqueado, se asegura de que esté visible
                slices[i].gameObject.SetActive(true);

                if (i == _selectedSection)
                {
                    slices[i].rectTransform.localScale = new Vector3(1.15f, 1.15f, 1f);
                    Color c = slices[i].color;
                    c.a = 1f; 
                    slices[i].color = c;
                }
                else
                {
                    slices[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
                    Color c = slices[i].color;
                    c.a = 0.4f; 
                    slices[i].color = c;
                }
            }
        }
    }

    private void DispararSenal()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, _mainCamera.nearClipPlane));
        
        worldPos.z = 0f; 

        if (soundWavePrefab != null)
        {
            GameObject newWave = Instantiate(soundWavePrefab, worldPos, Quaternion.identity);
            newWave.GetComponent<SoundWave>().signalIndex = _selectedSection;
        }
        
        ReproducirSonidoPorIndice(_selectedSection);
    }
    
    private void ReproducirSonidoPorIndice(int index)
    {
        if (index == 0 && sonidoAtraerNutria != null)
        {
            _audioSource.PlayOneShot(sonidoAtraerNutria);
        }
        else if (index == 1 && sonidoRepelerNutria != null)
        {
            _audioSource.PlayOneShot(sonidoRepelerNutria);
        }
        else if (index == 2)
        {
            if (_alternarSonidoRatas && sonidoRatasA != null)
            {
                _audioSource.PlayOneShot(sonidoRatasA);
            }
            else if (!_alternarSonidoRatas && sonidoRatasB != null)
            {
                _audioSource.PlayOneShot(sonidoRatasB);
            }
            _alternarSonidoRatas = !_alternarSonidoRatas; 
        }
        else if (index == 3 && sonidoIndex3 != null)
        {
            _audioSource.PlayOneShot(sonidoIndex3);
        }
    }
    
    public int GetCurrentSelectedIndex()
    {
        return _selectedSection;
    }
}