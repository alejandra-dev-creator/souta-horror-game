using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager

public class SceneLoader : MonoBehaviour
{
    // Asigna estos paneles en el Inspector en el GameObject SceneButtonsManager en SampleScene
    // Asegúrate de arrastrar Panel_MenuInicio y Panel_Niveles a estos slots.
    public GameObject panelMenuInicio;
    public GameObject panelNiveles;

    void Start()
    {
        // Comprueba si se ha establecido la preferencia para activar el Panel_Niveles
        if (PlayerPrefs.GetInt("ActivateLevelPanelOnLoad", 0) == 1) // 0 es el valor por defecto si no existe
        {
            // Activa el Panel_Niveles
            if (panelNiveles != null)
            {
                panelNiveles.SetActive(true);
                Debug.Log("<color=blue>SceneLoader:</color> Panel_Niveles activado al cargar escena.");
            }
            // Desactiva el Panel_MenuInicio (si está activo)
            if (panelMenuInicio != null)
            {
                panelMenuInicio.SetActive(false);
                Debug.Log("<color=blue>SceneLoader:</color> Panel_MenuInicio desactivado al cargar escena.");
            }

            // Reinicia la preferencia para que no se active de nuevo la próxima vez que cargues SampleScene
            PlayerPrefs.SetInt("ActivateLevelPanelOnLoad", 0);
            PlayerPrefs.Save(); // Guarda el cambio inmediatamente
        }
        else
        {
            // Si no hay una preferencia para activar Panel_Niveles,
            // asegúrate de que Panel_MenuInicio esté activo y Panel_Niveles inactivo por defecto
            if (panelMenuInicio != null)
            {
                panelMenuInicio.SetActive(true);
            }
            if (panelNiveles != null)
            {
                panelNiveles.SetActive(false);
            }
            Debug.Log("<color=blue>SceneLoader:</color> Mostrando Panel_MenuInicio por defecto.");
        }
    }

    // Función para cargar una escena por su nombre (para Nivel 1, 3, 4, 5)
    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f; // Asegura que el tiempo del juego se reanude
        SceneManager.LoadScene(sceneName);
        Debug.Log($"<color=cyan>SceneLoader:</color> Cargando escena: {sceneName}");
    }

    // Función para reiniciar la escena actual (útil para botones de Reiniciar)
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f; // Asegura que el tiempo del juego se reanude
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("<color=cyan>SceneLoader:</color> Reiniciando la escena actual.");
    }

    // ******************************************************************
    // * FUNCIONES ESPECÍFICAS PARA ACTIVAR/DESACTIVAR GameObjects individualmente *
    // * (Diseñadas como workaround porque las funciones de dos parámetros no aparecían en el Inspector) *
    // ******************************************************************

    // Función para activar un GameObject específico
    // Usar para el primer paso de activar paneles
    public void ActivateGameObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            Debug.Log($"<color=blue>SceneLoader:</color> GameObject '{obj.name}' ACTIVADO.");
        }
        else
        {
            Debug.LogWarning("<color=orange>SceneLoader:</color> Intento de activar un GameObject nulo.");
        }
    }

    // Función para desactivar un GameObject específico
    // Usar para el segundo paso de desactivar paneles
    public void DeactivateGameObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            Debug.Log($"<color=blue>SceneLoader:</color> GameObject '{obj.name}' DESACTIVADO.");
        }
        else
        {
            Debug.LogWarning("<color=orange>SceneLoader:</color> Intento de desactivar un GameObject nulo.");
        }
    }

    // ******************************************************************
    // * FUNCIONES ANTERIORES (Mantenidas en el código por referencia) *
    // * Puede que no las uses si no aparecen en el Inspector.         *
    // ******************************************************************

    // Función original para cambiar el estado activo/inactivo de un GameObject (toggle)
    public void SetGameObjectActive(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(!obj.activeSelf);
            Debug.Log($"<color=blue>SceneLoader:</color> GameObject '{obj.name}' activado/desactivado. Ahora está: {obj.activeSelf}");
        }
        else
        {
            Debug.LogWarning("<color=orange>SceneLoader:</color> Intento de activar/desactivar un GameObject nulo.");
        }
    }

    // Función original para activar/desactivar dos paneles (no apareció en tu Inspector)
    public void ActivateAndDeactivatePanels(GameObject panelToActivate, GameObject panelToDeactivate)
    {
        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);
            Debug.Log($"<color=blue>SceneLoader:</color> Panel '{panelToActivate.name}' ACTIVADO.");
        }
        else
        {
            Debug.LogWarning("<color=orange>SceneLoader:</color> Intento de activar un panel nulo.");
        }

        if (panelToDeactivate != null)
        {
            panelToDeactivate.SetActive(false);
            Debug.Log($"<color=blue>SceneLoader:</color> Panel '{panelToDeactivate.name}' DESACTIVADO.");
        }
        else
        {
            Debug.LogWarning("<color=orange>SceneLoader:</color> Intento de desactivar un panel nulo.");
        }
    }

    // Función original para activar/desactivar un GameObject con un bool explícito (no apareció en tu Inspector)
    public void SetGameObjectActivation(GameObject obj, bool activate)
    {
        if (obj != null)
        {
            obj.SetActive(activate);
            Debug.Log($"<color=blue>SceneLoader:</color> GameObject '{obj.name}' {(activate ? "ACTIVADO" : "DESACTIVADO")}.");
        }
        else
        {
            Debug.LogWarning("<color=orange>SceneLoader:</color> Intento de cambiar estado de un GameObject nulo.");
        }
    }
}