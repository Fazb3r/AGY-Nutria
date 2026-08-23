using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RouletteSignals : MonoBehaviour
{
    [Header("Visual Feedback")]
    public Image recuadroEquipado; 
    public Color[] sectionColors;
    public Image[] slices; 

    // AQUÍ ESTÁ LA VARIABLE QUE BUSCAMOS
    [Header("Game References")]
    public GameObject soundWavePrefab; 

    private int _selectedSection = 0; 
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        ActualizarUI(); 
    }

    void Update()
    {
        if (slices.Length == 0 || sectionColors.Length == 0) return;

        float scrollValue = Mouse.current.scroll.y.ReadValue();
        
        if (scrollValue > 0) 
        {
            _selectedSection++;
            if (_selectedSection >= slices.Length) _selectedSection = 0; 
            ActualizarUI();
        }
        else if (scrollValue < 0) 
        {
            _selectedSection--;
            if (_selectedSection < 0) _selectedSection = slices.Length - 1; 
            ActualizarUI();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DispararSenal();
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
        
        // Mantener la onda en el plano 2D
        worldPos.z = 0f; 

        // Generar la onda de sonido si el prefab está asignado
        if (soundWavePrefab != null)
        {
            GameObject newWave = Instantiate(soundWavePrefab, worldPos, Quaternion.identity);
            newWave.GetComponent<SoundWave>().signalIndex = _selectedSection;
        }
    }
    
    // Allows other scripts to know which section/index is currently selected
    public int GetCurrentSelectedIndex()
    {
        return _selectedSection;
    }
}