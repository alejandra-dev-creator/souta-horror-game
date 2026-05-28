using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    public int vidaMaxima = 3;
    private float vidaActual; // Ya está declarada como 'vidaActual'
    public Image barraDeVidaFill;
    private int pocionesRecolectadas = 0;
    public int pocionesNecesariasParaVida = 3;
    public int aumentoDeVidaPorGrupo = 1;

    private EnemigoIA villanoIA;

    void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarBarraDeVidaUI(); // Este es el nombre correcto del método
        Debug.Log("<color=green>VidaJugador:</color> Jugador inicializado con " + vidaActual + " de vida.");

        villanoIA = FindObjectOfType<EnemigoIA>();
        if (villanoIA == null)
        {
            Debug.LogWarning("<color=orange>VidaJugador:</color> No se encontró el script EnemigoIA en la escena. Asegúrate de que el villano tenga el script y esté activo.");
        }
    }

    public void RecibirDano(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("<color=red>VidaJugador:</color> Recibiendo " + cantidad + " de daño. Vida actual: " + vidaActual);

        if (vidaActual < 0)
        {
            vidaActual = 0;
        }

        ActualizarBarraDeVidaUI();

        if (vidaActual <= 0)
        {
            Debug.Log("<color=red>¡El jugador se quedó sin vida! ¡FIN DEL JUEGO!</color>");
            FinDeJuego();
        }
    }

    // En VidaJugador.cs
    public void ReiniciarVida()
    {
        // CORRECCIÓN: Usar 'vidaActual' y 'vidaMaxima' en lugar de 'saludActual' y 'saludMaxima'
        vidaActual = vidaMaxima; 
        // CORRECCIÓN: Usar 'ActualizarBarraDeVidaUI()' en lugar de 'ActualizarUIbarraDeVida()'
        ActualizarBarraDeVidaUI(); 
        // Cualquier otra cosa que necesites reiniciar en el jugador (posición, estado, etc.)
        Debug.Log("<color=cyan>VidaJugador:</color> Vida reiniciada.");
    }

    public void PocionRecolectada()
    {
        pocionesRecolectadas++;
        Debug.Log("<color=blue>VidaJugador:</color> Poción recolectada. Total: " + pocionesRecolectadas);
        if (pocionesRecolectadas >= pocionesNecesariasParaVida)
        {
            AumentarVida(aumentoDeVidaPorGrupo);
            pocionesRecolectadas = 0;
            Debug.Log("<color=blue>VidaJugador:</color> ¡Vida aumentada en " + aumentoDeVidaPorGrupo + "! Vida actual: " + vidaActual);
        }
    }

    public void AumentarVida(int cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima)
        {
            vidaActual = vidaMaxima;
        }
        Debug.Log("<color=blue>VidaJugador:</color> Vida aumentada. Nueva vida: " + vidaActual);
        ActualizarBarraDeVidaUI();
    }

    void ActualizarBarraDeVidaUI()
    {
        if (barraDeVidaFill != null)
        {
            barraDeVidaFill.fillAmount = vidaActual / vidaMaxima;
        }
        else
        {
            Debug.LogError("<color=red>VidaJugador:</color> ¡ERROR! No se ha asignado la imagen de relleno de la barra de vida en el Inspector.");
        }
    }

    public float GetVidaActual()
    {
        return vidaActual;
    }

    void FinDeJuego()
    {
        if (villanoIA != null)
        {
            villanoIA.ResetVillano();
            Debug.Log("<color=red>VidaJugador:</color> Solicitando reseteo del villano.");
        }

        if (GameManager.Instance != null)
        {
            // Ahora, GameManager.ultimaRetroalimentacionIncorrecta es pública, por lo que se puede acceder directamente.
            GameManager.Instance.MostrarPanelGameOverDerrota(GameManager.Instance.ultimaRetroalimentacionIncorrecta);
        }
        else
        {
            Debug.LogError("<color=red>VidaJugador:</color> GameManager.Instance no encontrado al intentar mostrar Game Over. Recargando escena como fallback.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        Debug.Log("<color=red>VidaJugador:</color> Juego terminado. Control de Game Over pasado a GameManager.");
    }
}