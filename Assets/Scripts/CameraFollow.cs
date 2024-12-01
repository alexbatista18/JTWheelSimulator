using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // O objeto que a câmera seguirá
    public Vector3 offset; // Deslocamento da câmera em relação ao objeto
    public float smoothSpeed = 0.125f; // Velocidade de suavização do movimento da câmera

    // Para garantir que a câmera gire ao redor do jogador
    public float rotationSpeed = 100f;  // Velocidade de rotação da câmera

    private void LateUpdate()  // Use LateUpdate para garantir que a posição seja ajustada após o movimento do jogador
    {
        // Cálculo da posição desejada da câmera com base no deslocamento
        Vector3 desiredPosition = target.position + offset;

        // Suaviza o movimento da câmera entre a posição atual e a desejada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Atualiza a posição da câmera
        transform.position = smoothedPosition;

        // Gira a câmera ao redor do jogador (sem se afastar do eixo central)
        RotateCamera();

        // Faz com que a rotação da câmera seja a mesma do jogador (opcional)
        transform.rotation = target.rotation;
    }

    private void RotateCamera()
    {
        // Gira a câmera em torno do jogador usando o eixo Y (rotação horizontal)
        float horizontalInput = Input.GetAxis("Horizontal");  // Pode ser ajustado conforme o seu sistema de controle
        if (Mathf.Abs(horizontalInput) > 0.3f)  // A rotação só ocorre se houver uma entrada significativa
        {
            // Rotaciona a câmera ao redor do jogador (eixo Y)
            transform.RotateAround(target.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
        }
    }
}
