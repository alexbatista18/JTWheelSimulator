using UnityEngine;
using System;
using System.Collections.Generic;

public class SegmentTimer : MonoBehaviour
{
    private TextMesh resultTextMesh; // Referência ao TextMesh para exibir os resultados

    private float startTime; // Tempo de início do segmento atual
    private float segmentTime; // Tempo decorrido no segmento atual
    private string segmentName; // Nome do segmento (nome do GameObject)
    private TrackTime trackTime; // Referência ao script TrackTime

    void Start()
    {
        segmentName = gameObject.name; // Define o segmentName como o nome deste GameObject
        startTime = Time.time;
        segmentTime = 0f;
        

        // Encontra o script TrackTime
        trackTime = FindObjectOfType<TrackTime>();

        // Atribui o TextMesh do objeto "Time Trecho"
        GameObject timeObject = GameObject.Find("Time Trecho");
        if (timeObject != null)
        {
            resultTextMesh = timeObject.GetComponent<TextMesh>();
        }
        else
        {
            Debug.LogError("Objeto 'Time Trecho' não encontrado.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopTimer(); // Para o cronômetro do segmento anterior, se houver

            // Inicia o cronômetro para o novo segmento
            startTime = Time.time;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopTimer(); // Para o cronômetro do segmento atual
            if (trackTime != null)
            {
                trackTime.AddSegmentTime(segmentName, segmentTime, BarrierColorChange.leftBarrierCollisions, BarrierColorChange.rightBarrierCollisions);
            }
            DisplayResults(); // Exibe os resultados após sair do segmento
            //Debug.Log("Resetando");
            //BarrierColorChange.leftBarrierCollisions = 0;
            //BarrierColorChange.rightBarrierCollisions = 0;

            // Adiciona o tempo do segmento ao script TrackTime
            // Reseta os contadores de colisão para o próximo segmento
            BarrierColorChange.ResetCollisionCounts();
            
        }
    }

    void Update()
    {
        // Atualiza o tempo decorrido no segmento atual
        segmentTime = Time.time - startTime;
    }

    void StopTimer()
    {
        // Para o cronômetro do segmento atual
        segmentTime = Time.time - startTime;
    }

    void DisplayResults()
    {
        if (resultTextMesh != null)
        {
            resultTextMesh.text += $"\n{segmentName}: {segmentTime:F2} s (Left: {BarrierColorChange.leftBarrierCollisions}, Right: {BarrierColorChange.rightBarrierCollisions})";
        }
    }
}
