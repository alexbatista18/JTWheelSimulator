using System.Collections;
using UnityEngine.Android;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothManager : MonoBehaviour
{
    public Text receivedData;
    public GameObject deviceMACText;
    public GameObject wheelchair;
    private bool isConnected;
    private bool isSearching;

    private static AndroidJavaClass unity3dbluetoothplugin;
    private static AndroidJavaObject BluetoothConnector;

    private WheelchairControllerBluetooth wheelchairController;

    private readonly string deviceMAC = "10:52:1C:5D:F8:26";

    // Referência ao botão de busca
    public Button searchButton;

    // Variáveis para o tempo de conexão
    private float connectionTimeout = 5f; // Tempo de espera para tentativa de conexão
    private float connectionTimer; // Temporizador para contar os 5 segundos

    // Start is called before the first frame update
    void Start()
    {
        InitBluetooth();
        isConnected = false;
        isSearching = false;

        if (wheelchair != null)
        {
            wheelchairController = wheelchair.GetComponent<WheelchairControllerBluetooth>();
        }

        // Conecta a função do botão de busca
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    // Inicializa o Bluetooth
    public void InitBluetooth()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        // Verificação das permissões
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
            || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADMIN")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
        {
            Permission.RequestUserPermissions(new string[] {
                Permission.CoarseLocation,
                Permission.FineLocation,
                "android.permission.BLUETOOTH_ADMIN",
                "android.permission.BLUETOOTH",
                "android.permission.BLUETOOTH_SCAN",
                "android.permission.BLUETOOTH_ADVERTISE",
                "android.permission.BLUETOOTH_CONNECT"
            });
        }

        unity3dbluetoothplugin = new AndroidJavaClass("com.example.unity3dbluetoothplugin.BluetoothConnector");
        BluetoothConnector = unity3dbluetoothplugin.CallStatic<AndroidJavaObject>("getInstance");
    }

    // Inicia a conexão Bluetooth
    public void StartConnection()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        receivedData.text = "Tentando conectar...";
        BluetoothConnector.CallStatic("StartConnection", deviceMAC);
    }

    // Interrompe a conexão Bluetooth
    public void StopConnection()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        if (isConnected)
        {
            BluetoothConnector.CallStatic("StopConnection");
            isConnected = false;
            receivedData.text = "Desconectado.";
        }
    }

    // Atualiza o status da conexão
    public void ConnectionStatus(string status)
    {
        Toast("Connection Status: " + status);
        isConnected = status == "connected";

        if (isConnected)
        {
            receivedData.text = "Conectado com sucesso!";
        }
        else
        {
            receivedData.text = "Desconectado do dispositivo.";
        }
    }

    // Processa os dados recebidos via Bluetooth
    public void ReadData(string data)
    {
        receivedData.text = "Dados recebidos...";
        Debug.Log("BT Stream: " + data);
        string[] values = data.Split(',');
        if (values.Length >= 2)
        {
            int value1 = int.Parse(values[0].Trim());
            int value2 = int.Parse(values[1].Trim());
            receivedData.text = "Joystick Conectado";
        }
        else
        {
            receivedData.text = "Os dados recebidos não contêm valores suficientes.";
        }

        if (wheelchairController != null)
        {
            wheelchairController.ProcessBluetoothData(data);
        }
    }

    // Exibe uma mensagem de log (Toast)
    public void ReadLog(string data)
    {
        Debug.Log(data);
    }

    public void Toast(string data)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        BluetoothConnector.CallStatic("Toast", data);
    }

    // Função chamada quando o botão de busca é clicado
    private void OnSearchButtonClicked()
    {
        if (isSearching) 
            return; // Evita iniciar a busca se já estiver em processo de busca

        isSearching = true;
        receivedData.text = "Procurando dispositivo Bluetooth...";

        // Reseta o temporizador de conexão
        connectionTimer = 0f;
        
        // Inicia a tentativa de conexão
        StartConnection();

        // Inicia a verificação de conexão com timeout
        StartCoroutine(CheckBluetoothConnectionWithTimeout());
    }

    // Verifica a conexão com timeout de 5 segundos
    private IEnumerator CheckBluetoothConnectionWithTimeout()
    {
        while (isSearching && connectionTimer < connectionTimeout)
        {
            connectionTimer += Time.deltaTime; // Conta o tempo decorrido
            yield return null; // Espera um frame

            if (isConnected)
            {
                isSearching = false; // Para a busca quando estiver conectado
                yield break;
            }
        }

        // Se não conseguiu conectar dentro do tempo
        if (!isConnected)
        {
            receivedData.text = "Não foi possível se conectar.";
            isSearching = false;
        }
    }
}
