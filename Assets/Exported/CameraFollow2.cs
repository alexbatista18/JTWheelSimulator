using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    public Transform target; // O objeto que a câmera seguirá
    public Vector3 offset; // Deslocamento da câmera em relação ao objeto
    public float smoothSpeed = 0.125f; // Velocidade de suavização do movimento da câmera

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Opcional: Fazer a câmera sempre olhar para o alvo
        // transform.LookAt(target);
        transform.rotation = target.rotation;
    }
}
