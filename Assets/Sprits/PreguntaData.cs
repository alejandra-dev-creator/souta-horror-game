// PreguntaData.cs
// Define la estructura de datos para cada pregunta.

using UnityEngine; // Necesario para [System.Serializable] si lo quieres ver en el Inspector

[System.Serializable] // IMPORTANTE: Asegúrate de que esta línea esté aquí
public class PreguntaData
{
    [Tooltip("El texto completo de la pregunta.")]
    public string preguntaTexto;

    [Tooltip("Marcar si 'Sí' es la respuesta correcta para esta pregunta, desmarcar si 'No' es la correcta.")]
    public bool respuestaCorrectaEsSi;

    [Tooltip("Mensaje de retroalimentación a mostrar si el jugador responde correctamente.")]
    [TextArea(3, 5)] // Hace el campo de texto más grande en el Inspector para facilitar la escritura
    public string retroalimentacionCorrecta;

    [Tooltip("Mensaje de retroalimentación a mostrar si el jugador responde incorrectamente.")]
    [TextArea(3, 5)] // Hace el campo de texto más grande en el Inspector
    public string retroalimentacionIncorrecta;
}
