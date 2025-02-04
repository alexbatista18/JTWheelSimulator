using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectFase: MonoBehaviour
{
    // Referências aos botões
    public Button botaoFase1;
    public Button botaoFase2;
    public Button botaoVoltarMenu;

    // Start é chamado antes do primeiro frame de atualização
    void Start()
    {
        // Adicionando listeners aos botões para chamar as funções ao serem clicados
        botaoFase1.onClick.AddListener(StartFase1);
        botaoFase2.onClick.AddListener(StartFase2);
        botaoFase2.onClick.AddListener(VoltarMenu);
    }

    // Função para iniciar a Fase 1
    void StartFase1()
    {
        SceneManager.LoadScene("Fase1"); // Substitua com o nome da sua cena da fase 1
    }

    // Função para iniciar a Fase 2
    void StartFase2()
    {
        SceneManager.LoadScene("Fase2"); // Substitua com o nome da sua cena da fase 2
    }

    void VoltarMenu()
    {
        SceneManager.LoadScene("Menu"); // Substitua com o nome da sua cena da fase 2
    }
}