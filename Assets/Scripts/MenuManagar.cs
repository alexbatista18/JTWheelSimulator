using UnityEngine;
using UnityEngine.UI;  // Necessário para trabalhar com UI

public class MenuManagar : MonoBehaviour
{
    public GameObject panel; // O Panel que será mostrado ou escondido
    public Button showButton; // Botão para mostrar o Panel
    public Button hideButton; // Botão para esconder o Panel

    void Start()
    {
        // Certifica-se de que o painel está oculto no início do jogo
        panel.SetActive(false);

        // Associa o método ShowPanel ao botão de mostrar
        if (showButton != null)
        {
            showButton.onClick.AddListener(ShowPanel);
        }

        // Associa o método HidePanel ao botão de esconder
        if (hideButton != null)
        {
            hideButton.onClick.AddListener(HidePanel);
        }
    }

    // Método para mostrar o Panel
    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    // Método para esconder o Panel
    public void HidePanel()
    {
        panel.SetActive(false);
    }
}
