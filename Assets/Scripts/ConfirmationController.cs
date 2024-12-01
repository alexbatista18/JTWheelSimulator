using UnityEngine;
using UnityEngine.UI; // Adicione esta linha para usar UI

public class ConfirmationController : MonoBehaviour
{
    public string difficultyLevel;

    public Vector3 newCircuitScale;
    public Vector3 newPlanePosition;
    public Vector3 newWheelChairPosition;

    public GameObject Circuito;
    public GameObject Plane;
    public GameObject WheelChair;

    public TrackTime trackTime;
    public Button confirmButton;

    private void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClick);
        }
    }

    public void OnConfirmButtonClick()
    {
        ConfirmSelection();
    }

    private void ConfirmSelection()
    {
        if (trackTime != null)
        {
            trackTime.SetSelectedDifficulty(difficultyLevel);
            Debug.Log("Dificuldade selecionada: " + difficultyLevel);
        }

        if (Circuito != null)
        {
            Circuito.transform.localScale = newCircuitScale;
        }

        if (Plane != null)
        {
            Plane.transform.position = newPlanePosition;
        }

        if (WheelChair != null)
        {
            WheelChair.transform.position = newWheelChairPosition;
        }
    }
}