using UnityEngine;

public class TrampaDeDano : MonoBehaviour
{
    public int dano = 1; // Cantidad de daño que inflige la trampa

    private void OnTriggerEnter2D(Collider2D other)
    {
        Jugador jugador = other.GetComponent<Jugador>();
        if (jugador != null)
        {
            // Aquí llamaremos a la función para reducir la vida del jugador
            VidaJugador vidaJugador = jugador.GetComponent<VidaJugador>();
            if (vidaJugador != null)
            {
                vidaJugador.RecibirDano(dano);
                Debug.Log("¡El jugador tocó la trampa y recibió " + dano + " de daño!");
            }
        }
    }
}
