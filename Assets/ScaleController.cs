using UnityEngine;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{
    // Referências para os botões
    public Button buttonL;
    public Button buttonR;

    // Referências para os objetos (GameObjects)
    public GameObject chair;
    public GameObject model;

    // Componentes de escala
    private Vector3 chairScale;
    private Vector3 modelScale;

    // Referência ao WheelchairControllerBluetooth
    public WheelchairControllerBluetooth wheelchairController; // Adicione esta linha

    void Start()
    {
        // Inicializa o valor das escalas
        chairScale = chair.transform.localScale;
        modelScale = model.transform.localScale;

        // Adiciona listeners aos botões
        buttonL.onClick.AddListener(OnButtonLPressed);
        buttonR.onClick.AddListener(OnButtonRPressed);
    }

    // Quando o botão L é pressionado
    void OnButtonLPressed()
    {
        // Altera a escala do "chair"
        chairScale.z = -1;
        chair.transform.localScale = chairScale;

        // Altera a escala do "model"
        modelScale.x = -0.017f;
        model.transform.localScale = modelScale;

        // Define a lógica de movimentação normal
        wheelchairController.SetInvertMovement(false); // Adicione esta linha
    }

    // Quando o botão R é pressionado
    void OnButtonRPressed()
    {
        // Altera a escala do "chair"
        chairScale.z = 1;
        chair.transform.localScale = chairScale;

        // Altera a escala do "model"
        modelScale.x = 0.017f;
        model.transform.localScale = modelScale;

        // Inverte a lógica de movimentação
        wheelchairController.SetInvertMovement(true); // Adicione esta linha
    }
}
