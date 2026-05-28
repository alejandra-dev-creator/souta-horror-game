using UnityEngine;
using UnityEngine.EventSystems; // Necesario para IPointerDownHandler y IPointerUpHandler
using UnityEngine.UI; // Para el tipo Button (opcional, pero útil si se adjunta a un Button)

// Este script debe ser añadido a los botones de UI (Izquierda, Derecha, Salto)
public class ButtonPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Queremos que el script Jugador sea el que escuche estos eventos.
    public Jugador jugadorController; // ASIGNAR TU GAMEOBJECT DE JUGADOR AQUÍ EN EL INSPECTOR

    // Variable para saber qué tipo de botón es este
    [Tooltip("1: Izquierda, 2: Derecha, 3: Salto")]
    public int buttonType = 0; // 0=none, 1=left, 2=right, 3=jump

    void Start()
    {
        if (jugadorController == null)
        {
            // Intenta encontrar el Jugador si no está asignado
            jugadorController = FindObjectOfType<Jugador>();
            if (jugadorController == null)
            {
                Debug.LogError("<color=red>ButtonPressHandler:</color> ¡ERROR! No se encontró ni se asignó un 'Jugador' para este botón. El botón no funcionará.");
                enabled = false; // Desactiva este script si no hay controlador
            }
        }
    }

    // Se llama cuando el puntero (dedo/ratón) presiona sobre el objeto
    public void OnPointerDown(PointerEventData eventData)
    {
        if (jugadorController == null) return;

        switch (buttonType)
        {
            case 1: // Botón Izquierda
                jugadorController.SetMobileMoveDirection(-1);
                break;
            case 2: // Botón Derecha
                jugadorController.SetMobileMoveDirection(1);
                break;
            case 3: // Botón Salto
                jugadorController.SetMobileJumpPressed(true);
                break;
        }
    }

    // Se llama cuando el puntero (dedo/ratón) suelta sobre el objeto (o fuera, después de presionar dentro)
    public void OnPointerUp(PointerEventData eventData)
    {
        if (jugadorController == null) return;

        switch (buttonType)
        {
            case 1: // Botón Izquierda
                jugadorController.SetMobileMoveDirection(0); // Detener movimiento
                break;
            case 2: // Botón Derecha
                jugadorController.SetMobileMoveDirection(0); // Detener movimiento
                break;
            case 3: // Botón Salto
                jugadorController.SetMobileJumpPressed(false); // Dejar de presionar salto
                break;
        }
    }
}