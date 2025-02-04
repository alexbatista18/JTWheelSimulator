using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName = "Menu"; // Permite definir o nome da cena no Inspector

    public void LoadScene()
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log($"Carregando cena: {sceneName}");
        }
        else
        {
            Debug.LogError($"A cena '{sceneName}' não foi encontrada. Verifique o nome e Build Settings.");
        }
    }
}
