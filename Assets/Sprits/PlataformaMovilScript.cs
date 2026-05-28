using UnityEngine;
using System.Collections; // Necesario para Corrutinas

public class PlataformaMovilScript : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Punto inicial de la plataforma. Arrastra un GameObject vacío aquí o define las coordenadas.")]
    public Transform puntoA;
    [Tooltip("Punto final de la plataforma. Arrastra un GameObject vacío aquí o define las coordenadas.")]
    public Transform puntoB;
    [Tooltip("Velocidad de movimiento de la plataforma.")]
    public float velocidad = 2f;
    [Tooltip("Tiempo que la plataforma espera en cada punto antes de moverse al siguiente.")]
    public float tiempoEsperaEnPunto = 1f;

    private Transform siguientePunto; // El punto al que se moverá la plataforma actualmente

    void Start()
    {
        // Validar que los puntos de movimiento estén asignados
        if (puntoA == null || puntoB == null)
        {
            Debug.LogError("<color=red>PlataformaMovilScript:</color> ¡ERROR! Los puntos A y B no están asignados en el Inspector. La plataforma no se moverá.");
            enabled = false; // Desactiva el script si no hay puntos
            return;
        }

        // Inicialmente, se moverá hacia el punto B
        siguientePunto = puntoB;
        // Mover la plataforma a la posición del punto A al inicio para asegurar que comience allí
        transform.position = puntoA.position;

        StartCoroutine(MoverPlataforma()); // Inicia la corrutina de movimiento
    }

    IEnumerator MoverPlataforma()
    {
        while (true) // Bucle infinito para que la plataforma se mueva continuamente
        {
            // Mover hacia el siguiente punto
            while (Vector2.Distance(transform.position, siguientePunto.position) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, siguientePunto.position, velocidad * Time.deltaTime);
                yield return null; // Esperar al siguiente frame
            }

            // Asegurar que la plataforma esté exactamente en el punto
            transform.position = siguientePunto.position;

            yield return new WaitForSeconds(tiempoEsperaEnPunto); // Esperar en el punto

            // Cambiar el siguiente punto al opuesto
            if (siguientePunto == puntoA)
            {
                siguientePunto = puntoB;
            }
            else
            {
                siguientePunto = puntoA;
            }
        }
    }

    // Esto es importante para que el jugador se mueva con la plataforma
    // Se llama cuando otro collider entra en contacto con este
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el objeto que colisiona es el jugador
        if (collision.gameObject.CompareTag("Player")) // Asegúrate de que tu jugador tenga el Tag "Player"
        {
            // Hace que el jugador sea hijo de la plataforma, así se moverá con ella
            collision.gameObject.transform.SetParent(transform);
        }
    }

    // Se llama cuando otro collider deja de estar en contacto con este
    void OnCollisionExit2D(Collision2D collision)
    {
        // Si el objeto que deja de colisionar es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // El jugador ya no es hijo de la plataforma
            collision.gameObject.transform.SetParent(null);
        }
    }
}
