using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class WheelchairControllerUSB : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float RotationSpeed = 50f;

    private Rigidbody rb;
    private Vector3 movementInput;
    private float rotationInput;

    public Animator animator1;
    public Animator animator2;

    private float baselineX = 1550f;
    private bool baselineSet = false;

    private SerialPort serialPort;
    public string portName = "COM6"; // Altere para a porta correta do seu dispositivo
    public int baudRate = 9600; // Deve ser o mesmo baud rate configurado no dispositivo

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // Configuração da porta serial
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 1000;

            if (serialPort.IsOpen)
            {
                Debug.Log($"Port {portName} opened successfully.");
            }
            else
            {
                Debug.LogError($"Failed to open port {portName}.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to open port {portName}: {ex.Message}");
        }
    }

    void Update()
{
    if (serialPort != null && serialPort.IsOpen)
    {
        try
        {
            if (serialPort.BytesToRead > 0) // Verifica se há dados disponíveis
            {
                string data = serialPort.ReadExisting(); // Lê todos os dados disponíveis
                Debug.Log($"Data received: {data}"); // Exibe os dados recebidos no console
                if (!string.IsNullOrEmpty(data))
                {
                    ProcessBluetoothData(data); // Processa os dados recebidos
                }
            }
        }
        catch (System.TimeoutException)
        {
            Debug.LogWarning($"Read timed out on port {portName}.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading from port {portName}: {ex.Message}");
        }
    }
    else
    {
        Debug.LogWarning($"Port {portName} is not open.");
    }
}

    public void ProcessBluetoothData(string data)
    {
        string[] values = data.Split(',');
        if (values.Length < 2)
        {
            Debug.LogWarning("Invalid data received: " + data);
            return;
        }

        try
        {
            int x = int.Parse(values[0].Trim());
            int y = int.Parse(values[1].Trim());

            Debug.Log($"Processed data: x={x}, y={y}"); // Print dos valores processados

            // Definir o baseline baseado no primeiro valor recebido
            if (!baselineSet)
            {
                baselineX = x;
                baselineSet = true;
                Debug.Log($"Baseline set to: {baselineX}");
            }

            // Resetar inputs
            movementInput = Vector3.zero;
            rotationInput = 0f;

            // Calcular limites usando o baseline
            float lowerLimit = baselineX - 300;
            float upperLimit = baselineX + 200;

            // Movimento para frente e para trás
            if (x > upperLimit)
            {
                movementInput = Vector3.forward * MovementSpeed;
                SetAnimatorBools("tras", true);
            }
            else
            {
                SetAnimatorBools("tras", false);
            }

            if (x < lowerLimit)
            {
                movementInput = -Vector3.forward * MovementSpeed;
                SetAnimatorBools("frente", true);
            }
            else
            {
                SetAnimatorBools("frente", false);
            }

            // Movimento de rotação
            if (y < lowerLimit)
            {
                rotationInput = -RotationSpeed;
                SetAnimatorBools("esquerda", true);
            }
            else
            {
                SetAnimatorBools("esquerda", false);
            }

            if (y > upperLimit)
            {
                rotationInput = RotationSpeed;
                SetAnimatorBools("direita", true);
            }
            else
            {
                SetAnimatorBools("direita", false);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse data: {data}. Error: {ex.Message}");
        }
    }

    void SetAnimatorBools(string boolName, bool value)
    {
        if (animator1 != null && animator1.GetBool(boolName) != value)
        {
            animator1.SetBool(boolName, value);
        }

        if (animator2 != null && animator2.GetBool(boolName) != value)
        {
            animator2.SetBool(boolName, value);
        }
    }

    void FixedUpdate()
    {
        if (movementInput != Vector3.zero)
        {
            Vector3 movement = transform.forward * movementInput.z * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }

        if (rotationInput != 0)
        {
            Quaternion rotation = Quaternion.Euler(0, rotationInput * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * rotation);
        }
    }

    void OnDestroy()
    {
        // Fecha a porta serial ao sair do jogo
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log($"Port {portName} closed.");
        }
    }
}
