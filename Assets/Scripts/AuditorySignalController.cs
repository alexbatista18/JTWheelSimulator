using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AuditorySignalController : MonoBehaviour
{
    [Header("References")]
    public GameObject signalObject;       // Painel/imagem vermelha no Canvas
    public GameObject wheelchairObject;   // Objeto da cadeira de rodas
    public TrackTime trackTime;           // Script do temporizador principal

    [Header("Timing")]
    public float minInterval = 8f;
    public float maxInterval = 12f;
    public float signalDuration = 2f;
    public float movementThreshold = 0.02f; // metros por frame para considerar em movimento

    // --- estado do ciclo ---
    private bool isRunning = false;
    private float trackStartTime;

    private float nextStimulusIn;   // tempo restante até próximo estímulo
    private bool signalVisible;
    private float signalTimer;

    // --- rastreio por estímulo ---
    private bool trackingResponse;
    private float stimulusTimeInTrack;
    private float stimulusWallTime;
    private bool playerStoppedThisStimulus;
    private float timeToStop;
    private float stopWallTime;
    private bool trackingResume;

    // --- detecção de movimento ---
    private Vector3 lastPosition;
    private bool wasMoving;

    // --- registros ---
    private List<StimulusRecord> records = new List<StimulusRecord>();

    private UnityEngine.UI.Image signalImage;

    void Start()
    {
        Debug.Log("[Signal] Start() chamado.");

        if (signalObject == null)
        {
            Debug.LogError("[Signal] ERRO: signalObject nao atribuido no Inspector!");
        }
        else
        {
            signalImage = signalObject.GetComponent<UnityEngine.UI.Image>();
            SetSignalVisible(false);
            Debug.Log("[Signal] signalObject encontrado: " + signalObject.name +
                      " | usa Image.enabled: " + (signalImage != null));

            if (signalObject == gameObject)
                Debug.LogWarning("[Signal] ATENCAO: signalObject eh o mesmo GameObject do script. Usando Image.enabled para nao desativar o Update.");
        }

        if (wheelchairObject == null)
            Debug.LogError("[Signal] ERRO: wheelchairObject nao atribuido no Inspector!");
        else
            Debug.Log("[Signal] wheelchairObject encontrado: " + wheelchairObject.name);

        if (trackTime == null)
            Debug.LogError("[Signal] ERRO: trackTime nao atribuido no Inspector!");
        else
        {
            trackTime.OnTimerStopped.AddListener(OnTrackFinished);
            Debug.Log("[Signal] trackTime encontrado e listener registrado.");
        }
    }

    public void StartCycle()
    {
        if (isRunning)
        {
            Debug.Log("[Signal] StartCycle ignorado - ja esta rodando.");
            return;
        }
        if (wheelchairObject == null)
        {
            Debug.LogError("[Signal] StartCycle abortado - wheelchairObject eh null.");
            return;
        }

        isRunning = true;
        trackStartTime = Time.time;
        lastPosition = wheelchairObject.transform.position;
        wasMoving = false;
        records.Clear();

        ScheduleNextStimulus();
        Debug.Log($"[Signal] Ciclo iniciado. Proximo estimulo em {nextStimulusIn:F1}s");
    }

    private bool _loggedWaitingForTimer = false;

    void Update()
    {
        if (!isRunning && trackTime != null)
        {
            if (!trackTime.IsTimingActive && !_loggedWaitingForTimer)
            {
                Debug.Log("[Signal] Aguardando TrackTime iniciar...");
                _loggedWaitingForTimer = true;
            }

            if (trackTime.IsTimingActive)
            {
                Debug.Log("[Signal] TrackTime ativo detectado! Iniciando ciclo.");
                _loggedWaitingForTimer = false;
                StartCycle();
            }
        }

        if (!isRunning) return;

        UpdateMovementState();
        UpdateStimulusTimer();
        UpdateResponseTracking();
    }

    private void UpdateMovementState()
    {
        Vector3 currentPos = wheelchairObject.transform.position;
        float delta = Vector3.Distance(currentPos, lastPosition);
        wasMoving = delta > movementThreshold;
        lastPosition = currentPos;
    }

    private void UpdateStimulusTimer()
    {
        nextStimulusIn -= Time.deltaTime;

        if (nextStimulusIn <= 0f && !signalVisible)
            TriggerStimulus();

        if (signalVisible)
        {
            signalTimer -= Time.deltaTime;
            if (signalTimer <= 0f)
                HideSignal();
        }
    }

    private void TriggerStimulus()
    {
        stimulusTimeInTrack = Time.time - trackStartTime;
        stimulusWallTime = Time.time;
        playerStoppedThisStimulus = false;
        timeToStop = -1f;
        trackingResponse = true;
        trackingResume = false;

        signalVisible = true;
        signalTimer = signalDuration;

        if (signalObject != null)
        {
            SetSignalVisible(true);
            Debug.Log($"[Signal] Estimulo disparado! Tempo na pista: {stimulusTimeInTrack:F1}s");
        }
        else
            Debug.LogError("[Signal] TriggerStimulus: signalObject eh null!");
    }

    private void HideSignal()
    {
        signalVisible = false;
        if (signalObject != null)
        {
            SetSignalVisible(false);
            Debug.Log("[Signal] Sinal escondido.");
        }

        ScheduleNextStimulus();
        Debug.Log($"[Signal] Proximo estimulo em {nextStimulusIn:F1}s");
    }

    private void UpdateResponseTracking()
    {
        if (!trackingResponse) return;

        // Aguarda o player parar
        if (!playerStoppedThisStimulus && !wasMoving)
        {
            playerStoppedThisStimulus = true;
            timeToStop = Time.time - stimulusWallTime;
            stopWallTime = Time.time;
            trackingResume = true;
            return;
        }

        // Aguarda o player voltar a se mover após parar
        if (trackingResume && wasMoving)
        {
            float durationStopped = Time.time - stopWallTime;
            records.Add(new StimulusRecord(
                stimulusTimeInTrack,
                playerStoppedThisStimulus,
                timeToStop,
                durationStopped
            ));
            trackingResponse = false;
            trackingResume = false;
        }
    }

    private void SetSignalVisible(bool visible)
    {
        if (signalImage != null)
            signalImage.enabled = visible;
        else if (signalObject != null && signalObject != gameObject)
            signalObject.SetActive(visible);
    }

    private void ScheduleNextStimulus()
    {
        nextStimulusIn = Random.Range(minInterval, maxInterval);
    }

    private void OnTrackFinished()
    {
        isRunning = false;

        // Salva qualquer estímulo pendente sem retomada
        if (trackingResponse && playerStoppedThisStimulus)
        {
            records.Add(new StimulusRecord(
                stimulusTimeInTrack,
                true,
                timeToStop,
                -1f // não retomou antes do fim
            ));
        }
        else if (trackingResponse)
        {
            records.Add(new StimulusRecord(stimulusTimeInTrack, false, -1f, -1f));
        }

        SaveCSV();
    }

    private void SaveCSV()
    {
        if (records.Count == 0) return;

        try
        {
            string difficulty = trackTime != null ? GetDifficultyFromTrackTime() : "default";
            string directoryPath = Application.persistentDataPath;
            int fileNumber = 1;
            string filePath;
            do
            {
                filePath = Path.Combine(directoryPath, $"{difficulty}_{fileNumber}_estimulos.csv");
                fileNumber++;
            } while (File.Exists(filePath));

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("TempoDoEstimulo_s, PlayerParou, TempoAteParar_s, DuracaoParado_s");
                foreach (var r in records)
                {
                    string stopped = r.PlayerStopped ? "sim" : "nao";
                    string timeToStopStr = r.TimeToStop >= 0 ? r.TimeToStop.ToString("F2") : "N/A";
                    string durationStr = r.DurationStopped >= 0 ? r.DurationStopped.ToString("F2") : "N/A";
                    writer.WriteLine($"{r.StimulusTime:F2}, {stopped}, {timeToStopStr}, {durationStr}");
                }
            }

            Debug.Log($"Dados de estímulos salvos em: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao salvar estímulos: {e.Message}");
        }
    }

    private string GetDifficultyFromTrackTime()
    {
        // TrackTime.selectedDifficulty é privado; usamos reflection apenas para leitura
        var field = typeof(TrackTime).GetField("selectedDifficulty",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (string)field.GetValue(trackTime) : "default";
    }

    private class StimulusRecord
    {
        public float StimulusTime;
        public bool PlayerStopped;
        public float TimeToStop;
        public float DurationStopped;

        public StimulusRecord(float t, bool stopped, float toStop, float duration)
        {
            StimulusTime = t;
            PlayerStopped = stopped;
            TimeToStop = toStop;
            DurationStopped = duration;
        }
    }
}
