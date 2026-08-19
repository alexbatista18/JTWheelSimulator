using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class TrackTime : MonoBehaviour
{
    public GameObject otimerTextMesh; // Referência ao componente TextMesh para exibir o tempo
    public UnityEvent OnTimerStopped; // Evento para notificar quando o cronômetro parar
    public TextMesh folderPathTextMesh; // Referência ao componente TextMesh para mostrar o caminho da pasta ou erro
    public GameObject WheelChair; // Referência ao GameObject "Wheel Chair"
    
    public Text totalCollisionsText; 
    public TextMeshPro totalCollisionsText2;

    private TextMesh timerTextMesh;
    private int totalCollisions = 0;
    private int totalCollisionsOld = 0;
    private float startTime;
    private bool isTiming;
    private List<SegmentData> segmentTimes; 
    private string selectedDifficulty = "Hard"; 

    // GameObject que aparecerá no final
    public GameObject finishTextObject; 
    public TextMeshPro timing;
    private float finishTimerDuration = 10f; // Duração de 10 segundos
    private bool isShowingFinishText = false; // Controla a exibição do texto final
    private float finishTimerStart;

    private VrModeController vrModeController; // Referência ao controlador de VR
    
    // Variáveis para contagem regressiva sem Coroutine
    private float remainingTime;
    private bool isCountingDown = false;

    void Start()
    {
        timerTextMesh = otimerTextMesh.GetComponent<TextMesh>();
        isTiming = false;
        UpdateTimerText(0f);
        segmentTimes = new List<SegmentData>();

        // Obtenha a referência do controlador de VR
        vrModeController = FindObjectOfType<VrModeController>();

        if (OnTimerStopped == null)
        {
            OnTimerStopped = new UnityEvent();
        }

        if (finishTextObject != null)
        {
            finishTextObject.SetActive(false); // Certifica-se de que o GameObject está oculto no início
        }
    }

    void Update()
    {
        // Lógica para o cronômetro
        if (isTiming)
        {
            float currentTime = Time.time - startTime;
            UpdateTimerText(currentTime);
        }

        // Lógica para esconder o objeto após 10 segundos
        if (isShowingFinishText && Time.time - finishTimerStart >= finishTimerDuration)
        {
            finishTextObject.SetActive(false);
            isShowingFinishText = false;
        }

        // Contagem regressiva com Update()
        if (isCountingDown && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // Atualiza o tempo restante
            if (timing != null)
            {
                timing.text = $"{remainingTime:F1}";
            }

            // Quando o tempo acabar, o GameObject desaparecerá
            if (remainingTime <= 0)
            {
                isCountingDown = false;
                if (finishTextObject != null)
                {
                    finishTextObject.SetActive(false);
                }

                if (timing != null)
                {
                    timing.text = "";
                }
            }
        }
    }

    private void UpdateTimerText(float time)
    {
        if (timerTextMesh != null)
        {
            timerTextMesh.text = time.ToString("F2") + " s";
        }
    }

    public bool IsTimingActive => isTiming;

    public void StartTimer()
    {
        startTime = Time.time;
        isTiming = true;
        Debug.Log("Cronômetro iniciado.");
    }

    public void StopTimer()
{
    if (isTiming)
    {
        float finalTime = Time.time - startTime;
        isTiming = false;
        UpdateTimerText(finalTime);
        Debug.Log("Cronômetro parado. Tempo final: " + finalTime + " segundos.");

        SaveSegmentTimesAsCSV();

        // Calcula as colisões totais, mas agora com as colisões resetadas a cada novo segmento
        totalCollisions = 0; // Reseta o contador de colisões para o cálculo atual

        // Somar as colisões dos segmentos
        foreach (var segment in segmentTimes)
        {
            totalCollisions += segment.LeftCollisions + segment.RightCollisions; // Soma as colisões de todos os segmentos
        }

        // Atualiza o texto de colisões
        if (totalCollisionsText != null)
        {
            totalCollisionsText.text = $"Colisões totais: {totalCollisions}";
            totalCollisionsText2.text = $"Colisões totais: {totalCollisions}";
        }

        segmentTimes.Clear(); // Limpa os segmentos
        OnTimerStopped.Invoke();

        // Modifica a posição da Wheel Chair dependendo da dificuldade selecionada
        if (WheelChair != null)
        {
            switch (selectedDifficulty)
            {
                case "Easy":
                    WheelChair.transform.position = new Vector3(32f, 0f, -34f);
                    break;
                case "Medium":
                    WheelChair.transform.position = new Vector3(14.4f, 0f, -27f);
                    break;
                case "Hard":
                    WheelChair.transform.position = new Vector3(-3.16f, 0f, -16f);
                    break;
            }
            WheelChair.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Debug.Log($"Wheel Chair position updated for {selectedDifficulty} difficulty.");

            // Exibe o GameObject por 10 segundos quando a cadeira de rodas atinge o destino
            if (finishTextObject != null && vrModeController != null && vrModeController.IsVrModeEnabled())
            {
                finishTextObject.SetActive(true); // Mostra o objeto
                isShowingFinishText = true;
                finishTimerStart = Time.time; // Inicia o temporizador de 10 segundos

                // Começa a contagem regressiva de 10 segundos
                StartCountdown(finishTimerDuration);
            }
        }
    }
}

    // Método que gerencia a contagem regressiva sem Coroutine
    public void StartCountdown(float countdownTime)
    {
        remainingTime = countdownTime;
        isCountingDown = true;
    }

    public void AddSegmentTime(string segmentName, float segmentTime, int leftCollisions, int rightCollisions)
    {
        segmentTimes.Add(new SegmentData(segmentName, segmentTime, leftCollisions, rightCollisions));
        Debug.Log($"Segmento adicionado: {segmentName}, Tempo: {segmentTime}, Colisões Esquerda: {leftCollisions}, Colisões Direita: {rightCollisions}");
    }

    public void SetSelectedDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
    }

    private class SegmentData
    {
        public string Name { get; }
        public float Time { get; }
        public int LeftCollisions { get; }
        public int RightCollisions { get; }

        public SegmentData(string name, float time, int leftCollisions, int rightCollisions)
        {
            Name = name;
            Time = time;
            LeftCollisions = leftCollisions;
            RightCollisions = rightCollisions;
        }
    }

    public void SaveSegmentTimesAsCSV()
    {
        try
        {
            string difficulty = string.IsNullOrEmpty(selectedDifficulty) ? "default" : selectedDifficulty;
            string directoryPath = Application.persistentDataPath;
            int fileNumber = 1;
            string filePath;
            do
            {
                filePath = Path.Combine(directoryPath, $"{difficulty}_{fileNumber}.csv");
                fileNumber++;
            } while (File.Exists(filePath));

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("SegmentName, SegmentTime, LeftCollisions, RightCollisions");
                foreach (var segment in segmentTimes)
                {
                    writer.WriteLine($"{segment.Name}, {segment.Time}, {segment.LeftCollisions}, {segment.RightCollisions}");
                }
            }

            Debug.Log($"Dados dos segmentos salvos em: {filePath}");

            if (folderPathTextMesh != null)
            {
                folderPathTextMesh.text = $"Arquivo salvo em: {filePath}";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao salvar o arquivo: {e.Message}");
            if (folderPathTextMesh != null)
            {
                folderPathTextMesh.text = $"Erro ao salvar o arquivo: {e.Message}";
            }
        }
    }
}
