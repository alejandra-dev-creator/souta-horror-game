// PocionVida.cs
// Maneja el comportamiento de la poción cuando es recogida por el jugador.

using UnityEngine;

public class PocionVida : MonoBehaviour
{
    public int puntosPorPocion = 20; // Puntos que otorga cada poción
    public AudioClip sonidoRecoger; // Asigna aquí el sonido desde el Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Intenta obtener el componente 'Jugador' del objeto que colisionó
        Jugador jugador = other.GetComponent<Jugador>();
        if (jugador != null) // Si el objeto es el jugador
        {
            // Reproducir el sonido de recolección si está asignado
            if (sonidoRecoger != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
            }
            else
            {
                Debug.LogWarning("No se ha asignado el sonido de recoger en la poción '" + gameObject.name + "'.");
            }

            // Llamar al GameManager para aumentar la puntuación
            // Ahora usa el Singleton GameManager.Instance
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AumentarPuntuacion(puntosPorPocion);
                Debug.Log("¡Poción recogida! +" + puntosPorPocion + " puntos añadidos a la puntuación.");
            }
            else
            {
                Debug.LogError("No se encontró la instancia de GameManager. Asegúrate de que tu GameManager esté en un GameObject activo en la escena.");
            }

            // Llamar a la lógica de aumento de vida directamente en el script VidaJugador del jugador
            VidaJugador vidaJugador = jugador.GetComponent<VidaJugador>();
            if (vidaJugador != null)
            {
                vidaJugador.PocionRecolectada(); // Esto debería aumentar la vida del jugador
            }
            else
            {
                Debug.LogWarning("El jugador no tiene un componente VidaJugador para llamar a PocionRecolectada.");
            }

            // Destruir la poción después de ser recogida
            Destroy(gameObject);
        }
    }
}