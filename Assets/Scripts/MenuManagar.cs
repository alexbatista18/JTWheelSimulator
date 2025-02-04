using UnityEngine;
using UnityEngine.UI;  // Necessário para trabalhar com UI

public class MenuManager : MonoBehaviour
{
    public GameObject panel; // O Panel que será mostrado ou escondido
    public Button showButton; // Botão para mostrar o Panel
    public Button hideButton; // Botão para esconder o Panel
    public GameObject targetComponent; // Componente que será escondido ou mostrado

    void Start()
    {
        // Certifica-se de que o painel está oculto no início do jogo
        panel.SetActive(false);

        // Certifica-se de que o componente está ativo no início
        if (targetComponent != null)
        {
            targetComponent.SetActive(true);
        }

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

    // Método para mostrar o Panel e esconder o componente
    public void ShowPanel()
    {
        panel.SetActive(true);
        if (targetComponent != null)
        {
            targetComponent.SetActive(false);
        }
    }

    // Método para esconder o Panel e mostrar o componente
    public void HidePanel()
    {
        panel.SetActive(false);
        if (targetComponent != null)
        {
            targetComponent.SetActive(true);
        }
    }
}
