using UnityEngine;

public class SimpleSmoothParallax : MonoBehaviour
{
    [Tooltip("Arrastra aquí tu Virtual Camera de Cinemachine")]
    public Transform cameraTransform;
    [Range(0f, 1f)]
    [Tooltip("Factor de paralaje: 0=sin paralaje, valores > 0 para paralaje")]
    public float parallaxFactor = 0.1f;
    public float smoothing = 5f;
    private Vector3 startPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
            {
                Debug.LogError("¡No se encontró la Cámara Principal! Asigna la cámara en el Inspector.");
                enabled = false;
                return;
            }
        }
        startPosition = transform.position;
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            float targetX = startPosition.x + cameraTransform.position.x * parallaxFactor;
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothing);
        }
    }
}