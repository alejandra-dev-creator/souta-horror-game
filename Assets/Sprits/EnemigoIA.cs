using UnityEngine;

public class EnemigoIA : MonoBehaviour
{
    [Header("Persecución")]
    public float velocidadMovimiento = 5f; 
    public float rangoDePersecucion = 20f; 
    public float distanciaDeParada = 1f; 
    public Transform objetivoJugador; 
    public string tagJugador = "Player"; 

    [Header("Retraso de Inicio")]
    public float tiempoDeEsperaInicial = 2f; 
    private float tiempoInicioMovimiento; 
    private bool villanoActivo = false; 

    // ¡ELIMINADAS todas las variables de detección de terreno y salto!
    // public LayerMask capaSuelo; 
    // public Transform chequeoSueloOrigen; 
    // public Transform chequeoParedOrigen; 
    // public float longitudChequeoSuelo = 0.2f; 
    // public float longitudChequeoPared = 0.2f; 
    // public float fuerzaSalto = 8f; 
    // public float distanciaSaltoHaciaArriba = 0.5f; 

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // private bool estaEnElSuelo; // ¡ELIMINADA!
    private int direccionMovimientoX; // 1 para derecha, -1 para izquierda

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        tiempoInicioMovimiento = Time.time + tiempoDeEsperaInicial;
        villanoActivo = false;
        Debug.Log("<color=green>EnemigoIA:</color> [Awake] Villano inicializado, esperando " + tiempoDeEsperaInicial + " segundos.");
    }

    void Start()
    {
        if (objetivoJugador == null)
        {
            GameObject jugadorGO = GameObject.FindGameObjectWithTag(tagJugador);
            if (jugadorGO != null)
            {
                objetivoJugador = jugadorGO.transform;
                Debug.Log("<color=green>EnemigoIA:</color> [Start] Objetivo Jugador encontrado por Tag: " + tagJugador);
            }
            else
            {
                Debug.LogWarning("<color=red>EnemigoIA:</color> [Start] ¡ADVERTENCIA CRÍTICA! No se encontró un GameObject con el tag '" + tagJugador + "'. Asigna 'Souta' directamente en el Inspector del Enemigo.");
            }
        }
        else
        {
            Debug.Log("<color=green>EnemigoIA:</color> [Start] Objetivo Jugador ya asignado en el Inspector.");
        }
        
        // ¡VALIDACIÓN DE TRANSFORMS DE CHEQUEO ELIMINADA!
        Debug.Log("<color=green>EnemigoIA:</color> [Start] Script ejecutándose.");
    }

    void FixedUpdate() 
    {
        // ¡ELIMINADA toda la lógica de detección de suelo!
        // estaEnElSuelo = false; // Siempre se considera no en el suelo para evitar problemas si no hay chequeo
    }

    void Update()
    {
        // Activar el villano después del tiempo de espera
        if (!villanoActivo && Time.time >= tiempoInicioMovimiento)
        {
            villanoActivo = true;
            Debug.Log("<color=green>EnemigoIA:</color> Villano activado, ¡empezando a perseguir!");
        }

        // Lógica de movimiento solo si el villano está activo y tiene un objetivo
        if (villanoActivo && objetivoJugador != null)
        {
            float distanciaAlJugador = Vector2.Distance(transform.position, objetivoJugador.position);
            
            // Determinar la dirección horizontal hacia el jugador
            // El villano sólo se mueve si el jugador está fuera de la distancia de parada en X
            if (objetivoJugador.position.x > transform.position.x + distanciaDeParada)
            {
                direccionMovimientoX = 1; // Mover a la derecha
                spriteRenderer.flipX = false;
            }
            else if (objetivoJugador.position.x < transform.position.x - distanciaDeParada)
            {
                direccionMovimientoX = -1; // Mover a la izquierda
                spriteRenderer.flipX = true;
            }
            else
            {
                direccionMovimientoX = 0; // Detenerse si está cerca en X del jugador
            }

            // Mover el villano si está dentro del rango de persecución
            if (distanciaAlJugador <= rangoDePersecucion) 
            {
                MoverVillano();
            }
            else // Jugador fuera de rango de persecución, se detiene
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                if (animator != null) animator.SetFloat("Speed", 0f); // Detener animación de correr
            }
        }
        else // Villano no activo o sin objetivo, se detiene
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null) animator.SetFloat("Speed", 0f);
            if (objetivoJugador == null && villanoActivo) 
            {
                Debug.LogWarning("<color=red>EnemigoIA:</color> [Update] ¡ERROR! Objetivo Jugador es NULL. El villano no hará nada.");
            }
        }
    }

    void MoverVillano()
    {
        float targetVelX = direccionMovimientoX * velocidadMovimiento;

        // ¡ELIMINADAS las detecciones de pared y precipicio!
        // bool detectaPared = false; 
        // bool detectaPrecipicio = false; 

        // ¡ELIMINADA la lógica de Salto!
        // if (estaEnElSuelo) { ... }

        // Aplicar movimiento horizontal.
        if (direccionMovimientoX != 0) // Solo aplicar velocidad si hay una dirección
        {
            rb.linearVelocity = new Vector2(targetVelX, rb.linearVelocity.y);
        }
        else // Si la dirección es 0 (jugador a distancia de parada en X)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Control de animación de correr
        if (animator != null) 
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x)); 
        }
    }

    public void ResetVillano()
    {
        tiempoInicioMovimiento = Time.time + tiempoDeEsperaInicial;
        villanoActivo = false;
        rb.linearVelocity = Vector2.zero; // Detener completamente al villano al resetear
        if (animator != null) animator.SetFloat("Speed", 0f);
        Debug.Log("<color=green>EnemigoIA:</color> [ResetVillano] Villano reseteado, esperando " + tiempoDeEsperaInicial + " segundos antes de activarse de nuevo.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Esta parte es para el ataque, puedes ignorarla si solo quieres movimiento por ahora.
        Debug.Log($"<color=red>COLISIÓN DETECTADA:</color> El Villano ha tocado algo. Objeto: {other.gameObject.name}, Tag: {other.gameObject.tag}.");

        if (other.CompareTag(tagJugador)) 
        {
            Debug.Log("<color=red>¡TAG JUGADOR COINCIDE!</color> Intentando obtener el script 'VidaJugador' del objeto: " + other.gameObject.name);

            VidaJugador vidaJugador = other.GetComponent<VidaJugador>();

            if (vidaJugador != null)
            {
                Debug.Log("<color=red>¡SCRIPT VIDA JUGADOR ENCONTRADO!</color> Enviando daño total al jugador.");
                vidaJugador.RecibirDano(Mathf.RoundToInt(vidaJugador.GetVidaActual())); 
            }
            else
            {
                Debug.LogError("<color=red>¡ERROR CRÍTICO!</color> El objeto con el Tag '" + tagJugador + "' (nombre: " + other.gameObject.name + ") NO tiene el script 'VidaJugador' adjunto. ¡Asegúrate de que el script 'VidaJugador' esté en tu GameObject de jugador!");
            }
        }
        else
        {
            Debug.Log($"<color=yellow>COLISIÓN IGNORADA:</color> El Villano colisionó con un objeto con Tag '{other.gameObject.tag}' que no es el Jugador. Nombre objeto: {other.gameObject.name}.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDePersecucion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeParada);

        // ¡ELIMINADOS todos los Gizmos de detección de terreno y salto!
        // if (chequeoSueloOrigen != null) { ... }
        // if (chequeoParedOrigen != null && spriteRenderer != null) { ... }
        // Gizmos.DrawRay(transform.position + new Vector3(0, 0.1f, 0), Vector2.up * distanciaSaltoHaciaArriba); 
    }
}