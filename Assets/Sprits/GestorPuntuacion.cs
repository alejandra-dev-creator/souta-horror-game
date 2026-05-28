using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración del Juego y Puntuación")]
    private int puntuacionActual = 0;
    public TextMeshProUGUI textoPuntuacion; // Referencia al TextMeshProUGUI de la UI
    [Tooltip("Puntaje total que el jugador debe alcanzar para que se dispare una pregunta.")]
    public int puntajeUmbralParaPregunta = 60;
    private bool preguntaActiva = false;

    [Header("Configuración de Preguntas")]
    [Tooltip("Lista de todas las preguntas que pueden aparecer. Rellena esto en el Inspector.")]
    public List<PreguntaData> listaDePreguntas;
    private PreguntaData preguntaActualData;
    private int indicePreguntaActual = 0;
    public string ultimaRetroalimentacionIncorrecta = "";

    [Header("Elementos de UI - Panel de Pregunta")]
    public GameObject panelPregunta;
    public TextMeshProUGUI textoPregunta;
    public Button botonSi;
    public Button botonNo;

    [Header("Elementos de UI - Panel de Retroalimentación")]
    public GameObject panelRetroalimentacion;
    public TextMeshProUGUI textoResultado;
    [TextArea(3, 5)]
    public TextMeshProUGUI textoExplicacion;
    public Button botonContinuar;
    public TextMeshProUGUI textoBotonRetroalimentacion;

    [Header("Elementos de UI - Paneles de Navegación")]
    [Tooltip("El GameObject del panel del menú de inicio.")]
    public GameObject panelMenuInicio; // Ya existía
    [Tooltip("El GameObject del panel donde están los botones de selección de niveles.")]
    public GameObject panelNiveles; // MODIFICACIÓN: Nuevo
    [Tooltip("El GameObject del panel o contenedor principal de la UI/elementos de tu juego Nivel 2.")]
    public GameObject panelJuegoPrincipal; // MODIFICACIÓN: Nuevo (Este es el que activa tu juego en SampleScene)

    [Header("Elementos de UI - Panel de Fin de Juego (Victoria)")]
    public GameObject panelFinJuego;
    public TextMeshProUGUI textoMensajeFin;
    public TextMeshProUGUI textoPuntuacionFinal;
    public Button botonReiniciar; // Este botón reinicia el juego desde el panel de victoria

    [Header("Elementos de UI - Panel de Game Over (Derrota)")]
    public GameObject panelGameOverDerrota;
    public TextMeshProUGUI textoMensajeGameOverDerrota;
    [TextArea(3, 5)]
    public TextMeshProUGUI textoExplicacionGameOverDerrota;
    public Button botonReiniciarGameOverDerrota; // Este botón reinicia el juego desde el panel de derrota

    [Header("Referencias del Jugador")]
    public VidaJugador vidaJugador;

    private Coroutine corrutinaEsperaRetroalimentacion;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Si el GameManager persiste (DontDestroyOnLoad), la lógica de PlayerPrefs en Start() no funcionará correctamente
            // Asegúrate de que DontDestroyOnLoad esté COMENTADO o ELIMINADO para este GameManager.
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        // Desactivar todos los paneles de UI al inicio (Awake)
        panelPregunta.SetActive(false);
        panelRetroalimentacion.SetActive(false);
        panelFinJuego.SetActive(false);
        panelGameOverDerrota.SetActive(false);
        if (panelNiveles != null) panelNiveles.SetActive(false);
        if (panelJuegoPrincipal != null) panelJuegoPrincipal.SetActive(false);

        if (botonContinuar != null)
        {
            botonContinuar.gameObject.SetActive(false);
        }

        // Asignar listeners a los botones
        // MODIFICACIÓN: El botón Jugar ya no llama a IniciarJuego directamente.
        // Ahora será manejado por el SceneLoader para alternar paneles.
        // botonJugar.onClick.AddListener(IniciarJuego); // COMENTADO O ELIMINADO
        
        // REMOVIDO: botonReiniciar.onClick.AddListener(ReiniciarJuego);
        // La asignación de ReiniciarJuego a los botones Reiniciar se hace en el Inspector ahora,
        // apuntando a MostrarPanelNiveles().

        botonSi.onClick.AddListener(() => ResponderPregunta(true));
        botonNo.onClick.AddListener(() => ResponderPregunta(false));
        botonContinuar.onClick.AddListener(ContinuarJuego);

        // REMOVIDO: if (botonReiniciarGameOverDerrota != null)
        // {
        //     botonReiniciarGameOverDerrota.onClick.AddListener(ReiniciarJuego);
        // }
        // La asignación de ReiniciarJuego a los botones Reiniciar se hace en el Inspector ahora,
        // apuntando a MostrarPanelNiveles().

        ActualizarTextoPuntuacion();
    }

    void Start()
    {
        // Lógica de inicialización de paneles al inicio de SampleScene
        // Por defecto, NO se activa el menú de inicio aquí, la lógica de PlayerPrefs lo manejará.
        // if (panelMenuInicio != null) panelMenuInicio.SetActive(true); // <--- COMENTADA/ELIMINADA ESTA LÍNEA

        if (panelNiveles != null) panelNiveles.SetActive(false);
        if (panelJuegoPrincipal != null) panelJuegoPrincipal.SetActive(false); // Asegura que el juego esté oculto al inicio

        // NUEVA LÓGICA: Comprobar si debemos ir al panel de niveles al cargar SampleScene
        // Esto ocurre cuando se regresa de Nivel1, Nivel3, Nivel4, Nivel5
        if (PlayerPrefs.GetInt("ReturnToLevelPanel", 0) == 1) // Si el indicador está activado
        {
            Debug.Log("<color=green>GameManager:</color> Detectado regreso desde un nivel. Mostrando Panel_Niveles.");
            if (panelMenuInicio != null) panelMenuInicio.SetActive(false); // Ocultar menú principal
            if (panelNiveles != null) panelNiveles.SetActive(true);       // Mostrar panel de niveles
            if (panelJuegoPrincipal != null) panelJuegoPrincipal.SetActive(false); // Asegurar que el juego esté oculto

            PlayerPrefs.SetInt("ReturnToLevelPanel", 0); // Resetear la preferencia después de usarla
            PlayerPrefs.Save(); // Guardar el cambio
        }
        else
        {
            // Si no se detectó el retorno a panel de niveles, se mostrará el panelMenuInicio por defecto.
            if (panelMenuInicio != null) panelMenuInicio.SetActive(true);
        }
    }

    public void IniciarJuego()
    {
        Time.timeScale = 1f; // Reanudar el juego
        puntuacionActual = 0;
        ActualizarTextoPuntuacion();
        preguntaActiva = false;
        indicePreguntaActual = 0;
        puntajeUmbralParaPregunta = 60; // Reiniciar el umbral para la primera pregunta
        Debug.Log("<color=lime>GameManager:</color> Iniciando juego (Nivel 2). Tiempo reanudado.");
    }

    public void OcultarTodosLosPanelesDeJuego()
    {
        panelPregunta.SetActive(false);
        panelRetroalimentacion.SetActive(false);
        panelFinJuego.SetActive(false);
        panelGameOverDerrota.SetActive(false);
        if (panelJuegoPrincipal != null) panelJuegoPrincipal.SetActive(false);
        Debug.Log("<color=lime>GameManager:</color> Todos los paneles de juego ocultos.");
    }

    public void MostrarPanelFinJuego(bool haGanado)
    {
        if (corrutinaEsperaRetroalimentacion != null)
        {
            StopCoroutine(corrutinaEsperaRetroalimentacion);
            corrutinaEsperaRetroalimentacion = null;
        }

        Time.timeScale = 0f;
        preguntaActiva = true;

        panelMenuInicio.SetActive(false);
        panelPregunta.SetActive(false);
        panelRetroalimentacion.SetActive(false);
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
        panelGameOverDerrota.SetActive(false);
        if (panelNiveles != null) panelNiveles.SetActive(false);
        if (panelJuegoPrincipal != null) panelJuegoPrincipal.SetActive(false);

        panelFinJuego.SetActive(true);

        if (haGanado)
        {
            textoMensajeFin.text = "¡FELICIDADES! ¡HAS ESCAPADO!";
            Debug.Log("<color=yellow>GameManager:</color> ¡Juego Terminado! El jugador ha ganado.");
        }
        else
        {
            textoMensajeFin.text = "¡JUEGO TERMINADO!"; // Mensaje genérico de derrota (no por pregunta)
            Debug.Log("<color=yellow>GameManager:</color> ¡Juego Terminado por derrota (general)!");
        }

        if (textoPuntuacionFinal != null)
        {
            textoPuntuacionFinal.text = "Puntuación Final: " + puntuacionActual.ToString();
        }
    }

    public void MostrarPanelGameOverDerrota(string explicacionIncorrecta = "")
    {
        if (corrutinaEsperaRetroalimentacion != null)
        {
            StopCoroutine(corrutinaEsperaRetroalimentacion);
            corrutinaEsperaRetroalimentacion = null;
        }

        Time.timeScale = 0f; // Pausar el juego
        preguntaActiva = true;

        panelMenuInicio.SetActive(false);
        panelPregunta.SetActive(false);
        panelRetroalimentacion.SetActive(false);
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
        panelFinJuego.SetActive(false);
        if (panelNiveles != null) panelNiveles.SetActive(false);
        if (panelJuegoPrincipal != null) panelJuegoPrincipal.SetActive(false);

        panelGameOverDerrota.SetActive(true);

        if (textoMensajeGameOverDerrota != null)
        {
            textoMensajeGameOverDerrota.text = "¡HAS PERDIDO LA VIDA! ¡GAME OVER!\n\n" + explicacionIncorrecta;
        }
        else
        {
            Debug.LogError("<color=red>GameManager:</color> ¡ERROR! No se ha asignado 'textoMensajeGameOverDerrota' en el Inspector.");
        }

        if (textoPuntuacionFinal != null)
        {
            textoPuntuacionFinal.text = "Puntuación Final: " + puntuacionActual.ToString();
        }
        else
        {
            Debug.LogWarning("<color=orange>GameManager:</color> 'textoPuntuacionFinal' no asignado para mostrar en Game Over por derrota.");
        }

        Debug.Log("<color=red>GameManager:</color> Mostrando Panel de Game Over por derrota.");
    }

    // Este método ya no es llamado por los botones de reiniciar de los paneles de fin de juego/game over.
    // Solo recarga la escena actual.
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("<color=lime>GameManager:</color> Reiniciando juego (recargando escena).");
    }

    public void AumentarPuntuacion(int puntos)
    {
        puntuacionActual += puntos;
        ActualizarTextoPuntuacion();
        Debug.Log($"<color=cyan>GameManager:</color> Puntuación actual: {puntuacionActual}");

        if (!preguntaActiva && puntuacionActual >= puntajeUmbralParaPregunta)
        {
            MostrarPregunta();
            puntajeUmbralParaPregunta += 60; // Aumentar el umbral para la próxima pregunta
        }
    }

    public int GetPuntuacion()
    {
        return puntuacionActual;
    }

    void ActualizarTextoPuntuacion()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = "Puntuación: " + puntuacionActual.ToString();
        }
        else
        {
            Debug.LogError("No se ha asignado el TextMeshProUGUI de la puntuación en el Inspector del GameManager.");
        }
    }

    void MostrarPregunta()
    {
        if (corrutinaEsperaRetroalimentacion != null)
        {
            StopCoroutine(corrutinaEsperaRetroalimentacion);
            corrutinaEsperaRetroalimentacion = null;
        }

        preguntaActiva = true;
        Time.timeScale = 0f; // Pausar el juego
        panelRetroalimentacion.SetActive(false);
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
        panelPregunta.SetActive(true);

        if (listaDePreguntas != null && listaDePreguntas.Count > 0)
        {
            preguntaActualData = listaDePreguntas[indicePreguntaActual % listaDePreguntas.Count];
            textoPregunta.text = preguntaActualData.preguntaTexto;
            Debug.Log($"<color=blue>GameManager:</color> Texto de Pregunta asignado: '{textoPregunta.text}'");
            indicePreguntaActual++;
        }
        else
        {
            Debug.LogWarning("<color=red>GameManager:</color> La lista de preguntas está vacía. ¡Añade preguntas en el Inspector!");
            ContinuarJuego(); // Si no hay preguntas, simplemente continúa
        }
    }

    void ResponderPregunta(bool respuestaJugadorEsSi)
    {
        if (corrutinaEsperaRetroalimentacion != null)
        {
            StopCoroutine(corrutinaEsperaRetroalimentacion);
            corrutinaEsperaRetroalimentacion = null;
        }

        panelPregunta.SetActive(false);
        Time.timeScale = 0f; // Pausar el juego
        preguntaActiva = true;

        if (respuestaJugadorEsSi == preguntaActualData.respuestaCorrectaEsSi)
        {
            panelRetroalimentacion.SetActive(true);
            textoResultado.text = "¡Respuesta Correcta!";
            textoExplicacion.text = preguntaActualData.retroalimentacionCorrecta;

            if (textoBotonRetroalimentacion != null)
            {
                textoBotonRetroalimentacion.text = "Continuar";
            }
            if (botonContinuar != null)
            {
                botonContinuar.gameObject.SetActive(true);
            }

            if (vidaJugador != null)
            {
                vidaJugador.PocionRecolectada();
                Debug.Log("<color=green>GameManager:</color> ¡Respuesta Correcta! El jugador ha sido recompensado con vida.");
            }
            else
            {
                Debug.LogWarning("<color=orange>GameManager:</color> VidaJugador no asignado. No se pudo recompensar al jugador.");
            }
            Debug.Log("<color=green>GameManager:</color> Retroalimentación Correcta mostrada. Esperando clic en Continuar.");

        }
        else // Si la respuesta es INCORRECTA
        {
            ultimaRetroalimentacionIncorrecta = preguntaActualData.retroalimentacionIncorrecta;

            Debug.Log("<color=red>GameManager:</color> ¡Respuesta incorrecta! Aplicando daño y esperando que VidaJugador active Game Over.");

            if (vidaJugador != null)
            {
                vidaJugador.RecibirDano(20);
            }
            else
            {
                Debug.LogWarning("<color=orange>GameManager:</color> VidaJugador no asignado. No se pudo aplicar daño letal al jugador. Mostrando Game Over directamente.");
                MostrarPanelGameOverDerrota(ultimaRetroalimentacionIncorrecta);
            }
        }
    }

    void ContinuarJuego()
    {
        if (corrutinaEsperaRetroalimentacion != null)
        {
            StopCoroutine(corrutinaEsperaRetroalimentacion);
            corrutinaEsperaRetroalimentacion = null;
        }

        panelRetroalimentacion.SetActive(false);
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
        Time.timeScale = 1f;
        preguntaActiva = false;
        Debug.Log("<color=cyan>GameManager:</color> Botón 'Continuar' presionado. Juego reanudado.");
    }

    // Nuevo método para mostrar el Panel de Niveles y reiniciar el estado del juego del Nivel 2
    public void MostrarPanelNiveles()
    {
        if (corrutinaEsperaRetroalimentacion != null)
        {
            StopCoroutine(corrutinaEsperaRetroalimentacion);
            corrutinaEsperaRetroalimentacion = null;
        }

        Time.timeScale = 0f; // Pausar el juego
        // Ocultar todos los paneles del juego y el menú de inicio
        panelMenuInicio.SetActive(false);
        panelPregunta.SetActive(false);
        panelRetroalimentacion.SetActive(false);
        panelFinJuego.SetActive(false);
        panelGameOverDerrota.SetActive(false);
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
        
        // CAMBIO CLAVE: DESACTIVAR EL PANEL DEL JUEGO PRINCIPAL PARA ASEGURAR QUE SE REINICIE
        if (panelJuegoPrincipal != null) panelJuegoPrincipal.SetActive(false); 

        // Mostrar el panel de selección de niveles
        if (panelNiveles != null)
        {
            panelNiveles.SetActive(true);
            Debug.Log("<color=lime>GameManager:</color> Mostrando Panel de Niveles. Juego pausado.");
        }
        else
        {
            Debug.LogError("<color=red>GameManager:</color> ¡ERROR! 'Panel_Niveles' no asignado en el GameManager.");
        }

        // LÓGICA PARA REINICIAR EL ESTADO DEL JUEGO (Nivel 2) cuando se regresa a la selección de niveles
        puntuacionActual = 0;
        ActualizarTextoPuntuacion();
        preguntaActiva = false;
        indicePreguntaActual = 0;
        puntajeUmbralParaPregunta = 60; // Reiniciar el umbral para la primera pregunta
        ultimaRetroalimentacionIncorrecta = ""; // Limpiar cualquier retroalimentación anterior
        
        if (vidaJugador != null)
        {
            // Asumiendo que tienes un método para reiniciar la vida del jugador
            vidaJugador.ReiniciarVida(); 
        }
        else
        {
            Debug.LogWarning("<color=orange>GameManager:</color> VidaJugador no asignado. No se pudo reiniciar la vida del jugador.");
        }
        Debug.Log("<color=lime>GameManager:</color> Estado del Nivel 2 reiniciado para la próxima partida.");
    }
}

