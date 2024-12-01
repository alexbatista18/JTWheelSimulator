using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonMove : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference movementAction; // Referência ao InputAction para movimento e rotação
    
    [Header("Player Settings")]
    public float movementSpeed = 5f;  // Velocidade do movimento
    public float rotationSpeed = 100f; // Velocidade de rotação
    
    private Vector2 movementInput; // Entrada do movimento (para frente, para trás e rotação)
    private Vector3 moveDirection;  // Direção do movimento do player

    private Rigidbody rb; // Referência ao Rigidbody do jogador

    private void Awake()
    {
        // Obtém o Rigidbody anexado ao objeto
        rb = GetComponent<Rigidbody>();
        
        // Ativa a interpolação no Rigidbody para suavizar o movimento
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        // Ativa o Input Action ao habilitar o objeto
        movementAction.action.Enable();
    }

    private void OnDisable()
    {
        // Desativa o Input Action ao desabilitar o objeto
        movementAction.action.Disable();
    }

    void Update()
    {
        // Captura a entrada do movimento e rotação do Input Action
        movementInput = movementAction.action.ReadValue<Vector2>(); // Exemplo de entrada Vector2
        
        // Chama a função que processa o movimento e rotação
        ProcessMovementAndRotation();
    }

    private void ProcessMovementAndRotation()
    {
        // **Rotação**
        if (Mathf.Abs(movementInput.x) > 0.3f)  // Limite para evitar movimento pequeno
        {
            // Rotaciona à direita ou à esquerda baseado no valor do eixo X
            float rotationDirection = movementInput.x > 0 ? 1f : -1f;
            // Utiliza o Rigidbody para mover o jogador de forma física
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + rotationDirection * rotationSpeed * Time.deltaTime, 0);
            rb.MoveRotation(targetRotation);
        }

        // **Movimento (Para frente e para trás)**
        if (Mathf.Abs(movementInput.y) > 0.3f)  // Limite para evitar movimento pequeno
        {
            // Usa a direção local para o movimento
            moveDirection = movementInput.y > 0 ? transform.forward : -transform.forward;
            
            // Movimenta o player para frente ou para trás com uma velocidade suave
            Vector3 targetPosition = transform.position + moveDirection * movementSpeed * Time.deltaTime;
            rb.MovePosition(targetPosition);
        }
    }
}
