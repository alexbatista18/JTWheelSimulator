using UnityEngine;
using UnityEngine.UI; // Para usar o Text da UI

public class CollisionCounter : MonoBehaviour
{
    public GameObject timeTrechoObject; // Objeto que contém o Text com o texto dos tempos
    public GameObject resultadoObject; // Objeto que vai exibir o resultado das colisões

    private Text timeTrechoText; // Text do objeto "Time Trecho"
    private Text resultadoText; // Text do objeto "Resultado"

    private void Start()
    {
        // Obtendo os componentes Text dos objetos
        timeTrechoText = timeTrechoObject.GetComponent<Text>();
        resultadoText = resultadoObject.GetComponent<Text>();

        // Processando o texto para contar as colisões
        ProcessarColisoes();
    }

    private void ProcessarColisoes()
    {
        string text = timeTrechoText.text; // Texto do Time Trecho

        // Variáveis para somar colisões Left e Right
        int totalLeft = 0;
        int totalRight = 0;

        // Expressões regulares para capturar "Left" e "Right"
        System.Text.RegularExpressions.Regex leftPattern = new System.Text.RegularExpressions.Regex(@"Left:\s*(\d+)");
        System.Text.RegularExpressions.Regex rightPattern = new System.Text.RegularExpressions.Regex(@"Right:\s*(\d+)");

        // Encontrando e somando todas as ocorrências de "Left"
        foreach (System.Text.RegularExpressions.Match match in leftPattern.Matches(text))
        {
            totalLeft += int.Parse(match.Groups[1].Value);
        }

        // Encontrando e somando todas as ocorrências de "Right"
        foreach (System.Text.RegularExpressions.Match match in rightPattern.Matches(text))
        {
            totalRight += int.Parse(match.Groups[1].Value);
        }

        // Exibindo o resultado no Text de "Resultado"
        resultadoText.text = $"Total Left: {totalLeft}, Total Right: {totalRight}";
    }
}
