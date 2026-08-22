using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RouletteSignals : MonoBehaviour
{
    [Header("Visual Feedback")]
    public Image recuadroEquipado; 
    public Color[] sectionColors;
    public Image[] slices; 

    private int _selectedSection = 0; 
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        ActualizarUI(); 
    }

    void Update()
    {
        // Evitar errores de compilación si aún no asignas las listas en el Inspector
        if (slices.Length == 0 || sectionColors.Length == 0) return;

        // 1. Cambiar selección con la rueda del mouse
        float scrollValue = Mouse.current.scroll.y.ReadValue();
        
        if (scrollValue > 0) 
        {
            _selectedSection++;
            if (_selectedSection >= slices.Length) _selectedSection = 0; // Volver al inicio
            ActualizarUI();
        }
        else if (scrollValue < 0) 
        {
            _selectedSection--;
            if (_selectedSection < 0) _selectedSection = slices.Length - 1; // Ir al final
            ActualizarUI();
        }

        // 2. Disparar con Clic Izquierdo
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DispararSenal();
        }
    }

    private void ActualizarUI()
    {
        // Actualizar el color del recuadro
        if (recuadroEquipado != null && _selectedSection < sectionColors.Length)
        {
            recuadroEquipado.color = sectionColors[_selectedSection];
        }

        // Recorrer dinámicamente las rebanadas para aplicar el Highlight
        for (int i = 0; i < slices.Length; i++)
        {
            if (slices[i] != null)
            {
                if (i == _selectedSection)
                {
                    // HIGHLIGHT: Más grande y 100% opaco
                    slices[i].rectTransform.localScale = new Vector3(1.15f, 1.15f, 1f);
                    Color c = slices[i].color;
                    c.a = 1f; 
                    slices[i].color = c;
                }
                else
                {
                    // APAGADO: Tamaño normal y semitransparente
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
        Debug.Log($"¡Pew! Disparando sonido Índice: {_selectedSection} en las coordenadas X:{worldPos.x}, Y:{worldPos.y}");
    }
}