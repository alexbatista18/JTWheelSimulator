using UnityEngine;

public class BarrierColorChange : MonoBehaviour
{
    public GameObject greenObject; // Referência ao objeto chamado "Green"
    private Material greenMaterial; // Material do objeto "Green"
    public bool isLeftBarrier; // Indica se esta é a barreira esquerda

    private Color originalColor = new Color(0.31f, 0.82f, 0.34f, 0.47f); // Cor original (4FD157 com opacidade 47)
    private Color collisionColor = new Color(1.0f, 0.04f, 0.0f, 0.47f); // Cor ao colidir (FF0900 com opacidade 47)

    public static int leftBarrierCollisions; // Contador de colisões na barreira esquerda (static para ser acessível globalmente)
    public static int rightBarrierCollisions; // Contador de colisões na barreira direita (static para ser acessível globalmente)

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
        if (other.CompareTag("Player"))
        {
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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

    public static void ResetCollisionCounts()
{
    leftBarrierCollisions = 0;
    rightBarrierCollisions = 0;
}
}
