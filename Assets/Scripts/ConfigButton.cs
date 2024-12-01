using UnityEngine;
using UnityEngine.UI;

public class ConfigButton : MonoBehaviour
{
    public GameObject configPanel;

    public void OpenConfig()
    {
        // Torna visível o painel de configurações quando o botão é clicado
        configPanel.SetActive(true);
    }
}
