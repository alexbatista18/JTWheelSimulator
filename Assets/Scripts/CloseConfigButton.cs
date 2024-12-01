using UnityEngine;
using UnityEngine.UI;

public class CloseConfigButton : MonoBehaviour
{
    public GameObject configPanel;

    public void CloseConfig()
    {
        // Torna invisível o painel de configurações quando o botão é clicado
        configPanel.SetActive(false);
    }
}
