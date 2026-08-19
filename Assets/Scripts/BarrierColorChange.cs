using UnityEngine;

public class BarrierColorChange : MonoBehaviour
{
    public GameObject greenObject; // Referência ao objeto chamado "Green"
    private Material greenMaterial; // Material do objeto "Green"
    public bool isLeftBarrier; // Indica se esta é a barreira esquerda
    public AudioSource warningAudioSource; // AudioSource para tocar o aviso
    public AudioClip warningClip; // Som de aviso da colisão
    public bool repeatWarningWhileTouching; // Se true, repete o som enquanto o player estiver encostado
    public float warningRepeatInterval = 0.75f; // Intervalo entre avisos quando repeatWarningWhileTouching estiver ativo

    private Color originalColor = new Color(0.31f, 0.82f, 0.34f, 0.47f); // Cor original (4FD157 com opacidade 47)
    private Color collisionColor = new Color(1.0f, 0.04f, 0.0f, 0.47f); // Cor ao colidir (FF0900 com opacidade 47)

    public static int leftBarrierCollisions; // Contador de colisões na barreira esquerda (static para ser acessível globalmente)
    public static int rightBarrierCollisions; // Contador de colisões na barreira direita (static para ser acessível globalmente)

    private bool playerTouchingBarrier;
    private float nextWarningTime;

    private void Start()
    {
        if (greenObject != null)
        {
            Renderer renderer = greenObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                greenMaterial = renderer.material;
                SetMaterialColor(originalColor); // Define a cor original na inicialização
            }
            else
            {
                Debug.LogError("O objeto 'Green' não possui um Renderer.");
            }
        }
        else
        {
            Debug.LogError("O objeto 'Green' não foi atribuído.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerTouchingBarrier)
        {
            playerTouchingBarrier = true;

            //Debug.Log("Tendo Colisão");
            
            if (isLeftBarrier)
            {
                
                leftBarrierCollisions++;
                //Debug.Log("Somando Left " + leftBarrierCollisions);
            }
            else
            {
                
                rightBarrierCollisions++;
                //Debug.Log("Somando Right " + rightBarrierCollisions);
            }

            if (greenMaterial != null)
            {
                SetMaterialColor(collisionColor);
            }

            PlayWarningSound();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || !repeatWarningWhileTouching)
        {
            return;
        }

        if (Time.time >= nextWarningTime)
        {
            PlayWarningSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTouchingBarrier = false;

            if (greenMaterial != null)
            {
                SetMaterialColor(originalColor);
            }
        }
    }

    private void SetMaterialColor(Color color)
    {
        if (greenMaterial != null)
        {
            greenMaterial.color = color;
        }
    }

    private void PlayWarningSound()
    {
        if (warningAudioSource == null)
        {
            return;
        }

        if (warningClip != null)
        {
            warningAudioSource.PlayOneShot(warningClip);
        }
        else
        {
            warningAudioSource.Play();
        }

        nextWarningTime = Time.time + warningRepeatInterval;
    }

    public static void ResetCollisionCounts()
{
    leftBarrierCollisions = 0;
    rightBarrierCollisions = 0;
}
}
