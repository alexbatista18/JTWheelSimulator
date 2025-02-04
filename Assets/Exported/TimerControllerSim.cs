using UnityEngine;
using UnityEngine.UI; // Se você quiser exibir o tempo na UI
using TMPro; // Importa o namespace do TextMeshPro

public class TimerController : MonoBehaviour
{
    private float startTime; // Armazena o tempo quando o player começa
    private bool isTiming = false; // Verifica se o cronômetro está ativo
    public GameObject startLine; // O objeto que representa a linha de início
    public GameObject endLine; // O objeto que representa a linha de fim
    public Text timerText; // Campo de texto para exibir o tempo (opcional)
    public TextMeshPro timerTextMeshPro; // Campo de texto para exibir o tempo usando TextMeshPro

    void Update()
    {
        // Atualiza o cronômetro enquanto está ativo
        if (isTiming)
        {
            float currentTime = Time.time - startTime;
            DisplayTime(currentTime);
        }
    }

    // Método para iniciar o cronômetro
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == startLine && !isTiming)
        {
            StartTimer();
        }
        else if (other.gameObject == endLine && isTiming)
        {
            StopTimer();
        }
    }

    // Inicia o cronômetro
    void StartTimer()
    {
        startTime = Time.time;
        isTiming = true;
        Debug.Log("Cronômetro iniciado!");
    }

    // Para o cronômetro
    void StopTimer()
    {
        isTiming = false;
        float finalTime = Time.time - startTime;
        Debug.Log("Cronômetro parado! Tempo final: " + finalTime.ToString("F2") + " segundos.");
    }

    // Exibe o tempo na UI, se houver
    void DisplayTime(float time)
    {
        if (timerText != null)
        {
            timerText.text = time.ToString("F2") + "s";
        }
        if (timerTextMeshPro != null)
        {
            timerTextMeshPro.text = time.ToString("F2") + "s"; // Atualiza o TextMeshPro também
        }
    }
}
