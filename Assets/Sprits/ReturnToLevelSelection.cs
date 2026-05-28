using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLevelSelection : MonoBehaviour
{
    // Función para cargar la SampleScene y activar específicamente el Panel_Niveles al llegar
    public void LoadSampleSceneAndActivateLevelPanel()
    {
        Time.timeScale = 1f; // Asegura que el tiempo del juego se reanude

        // Guarda una preferencia para indicar que se debe activar el Panel_Niveles
        PlayerPrefs.SetInt("ActivateLevelPanelOnLoad", 1);
        PlayerPrefs.Save(); // Guarda los PlayerPrefs inmediatamente para que estén disponibles en la siguiente escena

        // Carga la SampleScene
        SceneManager.LoadScene("SampleScene");
        Debug.Log("<color=green>ReturnToLevelSelection:</color> Cargando SampleScene y marcando para activar Panel_Niveles.");
    }

    // Puedes mantener esta función si tienes otros botones que solo cargan la SampleScene
    // sin activar un panel específico (ej. si quieres regresar al Panel_MenuInicio).
    // Si no la usas, puedes eliminarla.
    public void LoadSampleSceneAndSetPanel()
    {
        Time.timeScale = 1f;
        // Reinicia la preferencia para que no siempre active el panel de niveles
        PlayerPrefs.SetInt("ActivateLevelPanelOnLoad", 0);
        PlayerPrefs.Save(); // Guarda el cambio
        SceneManager.LoadScene("SampleScene");
        Debug.Log("<color=green>ReturnToLevelSelection:</color> Cargando SampleScene sin activar un panel específico.");
    }
}
