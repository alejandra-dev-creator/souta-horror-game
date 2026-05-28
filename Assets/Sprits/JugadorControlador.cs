using UnityEngine;
using UnityEngine.UI; // Necesario para el tipo Button (aunque no se asignen directamente aquí)
using TMPro; // Necesario si usas TextMeshProUGUI en tus botones o UI

public class Jugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 10f;
    public string botonSalto = "Jump"; // Mantener para compatibilidad con PC si quieres
    private bool saltoActivado = false; // Indica si un salto está en progreso

    [Header("Detección de Suelo")]
    public string tagSuelo = "Suelo"; // Tag para el suelo estático
    private bool estaEnElSuelo = false;
    private bool puedeSaltar = true; // Permite un solo salto por vez

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float moveInputX;

    // --- NUEVAS VARIABLES PARA CONTROL MÓVIL ---
    // Estas variables serán actualizadas por el script ButtonPressHandler
    private int mobileMoveDirection = 0; // -1: izquierda, 0: parado, 1: derecha
    private bool mobileJumpPressed = false; // True cuando el botón de salto móvil está presionado

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animator.SetFloat("movement", 0f);
        estaEnElSuelo = false;
        puedeSaltar = true;
        saltoActivado = false;
    }

    void Update()
    {
        // 1. Movimiento Horizontal (Combina entrada de PC y Móvil)
        float inputHorizontalPC = Input.GetAxisRaw("Horizontal"); // Entrada de teclado/joystick

        // Decide la entrada final de movimiento horizontal
        float finalMoveInput = inputHorizontalPC;
        if (mobileMoveDirection != 0) // Si se está presionando un botón móvil, priorizar
        {
            finalMoveInput = mobileMoveDirection;
        }
        // Si no hay botón móvil presionado, pero hay input de PC, usar input de PC
        else if (inputHorizontalPC != 0)
        {
            finalMoveInput = inputHorizontalPC;
        }

        moveInputX = finalMoveInput; // Asignar al moveInputX que usas para velocidad y animación
        rb.linearVelocity = new Vector2(moveInputX * velocidad, rb.linearVelocity.y);

        // 2. Animación de Movimiento
        animator.SetFloat("movement", Mathf.Abs(moveInputX));

        // 3. Actualizar parámetro enSuelo en el Animator
        animator.SetBool("ensuelo", estaEnElSuelo);

        // 4. Voltear Sprite
        if (moveInputX > 0) spriteRenderer.flipX = false;
        else if (moveInputX < 0) spriteRenderer.flipX = true;

        // 5. Detección y Ejecución del Salto (Combina entrada de PC y Móvil)
        bool pcJumpInput = Input.GetButtonDown(botonSalto); // Entrada de salto de PC
        bool finalJumpInput = pcJumpInput || mobileJumpPressed; // ¿Salto de PC O botón móvil presionado?

        if (finalJumpInput && estaEnElSuelo && puedeSaltar && !saltoActivado)
        {
            animator.SetTrigger("saltar");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            estaEnElSuelo = false;
            puedeSaltar = false;
            saltoActivado = true;
            mobileJumpPressed = false; // Resetear la variable de salto móvil después de un salto
        }

        // Resetear la variable de salto cuando toca el suelo de nuevo
        if (estaEnElSuelo)
        {
            saltoActivado = false;
            puedeSaltar = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Asegúrate de que tanto el suelo estático como las plataformas móviles tienen el tag "Suelo".
        // O si no, podrías usar Layers para la detección del suelo.
        if (collision.gameObject.CompareTag(tagSuelo))
        {
            estaEnElSuelo = true;
        }
        // Además, si estás usando el sistema de parentesco de la plataforma móvil,
        // esto asegurará que el jugador sigue "en el suelo" al tocar la plataforma.
        // Asegúrate de que tu plataforma móvil también tenga el tag "Suelo".
        // Debug.Log("OnCollisionEnter2D con: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");

        // Para que el jugador se mueva con la plataforma
        if (collision.gameObject.CompareTag("PlataformaMovil")) // Asegúrate de que tu plataforma móvil tenga este Tag
        {
            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagSuelo))
        {
            estaEnElSuelo = false;
        }
        // Quita al jugador como hijo de la plataforma cuando deja de colisionar
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug para ver qué se está tocando
        Debug.Log("Jugador colisionó con Trigger: " + other.gameObject.name + " (Tag: " + other.gameObject.tag + ")");

        if (other.gameObject.CompareTag("FinalJuego"))
        {
            Debug.Log("<color=green>¡El jugador llegó a la cabaña final! Llamando a GameManager para terminar el juego.</color>");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.MostrarPanelFinJuego(true);
            }
            else
            {
                Debug.LogError("GameManager.Instance no encontrado. Asegúrate de que tu GameManager esté en la escena.");
            }
        }
    }

    // --- NUEVOS MÉTODOS PÚBLICOS PARA EL CONTROL MÓVIL ---
    // Llamados por el ButtonPressHandler
    public void SetMobileMoveDirection(int direction)
    {
        mobileMoveDirection = direction; // -1 para izquierda, 0 para parado, 1 para derecha
    }

    public void SetMobileJumpPressed(bool pressed)
    {
        mobileJumpPressed = pressed;
    }
}