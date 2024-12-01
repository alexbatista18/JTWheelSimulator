using System.Collections;
using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class VrModeController : MonoBehaviour
{
    private const float _defaultFieldOfView = 60.0f;
    private Camera _mainCamera;
    public Button vrToggleButton; // Botão para ativar o VR

    private bool _isVrModeEnabled
    {
        get { return XRGeneralSettings.Instance.Manager.isInitializationComplete; }
    }

    public bool IsVrModeEnabled() // Função para verificar se o VR está ativo
    {
        return _isVrModeEnabled;
    }

    public void Start()
    {
        _mainCamera = Camera.main;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;

        // Garantir que o modo VR esteja desativado no início
        ExitVR();

        // Associa o método EnterVR ao botão
        if (vrToggleButton != null)
        {
            vrToggleButton.onClick.AddListener(EnterVR);
        }

        // Verifica se os parâmetros do dispositivo já estão salvos
        if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }
    }

    public void Update()
    {
        if (_isVrModeEnabled)
        {
            if (Api.IsCloseButtonPressed || IsScreenTouched())
            {
                ExitVR();
            }

            if (Api.IsGearButtonPressed)
            {
                Api.ScanDeviceParams();
            }

            Api.UpdateScreenParams();
        }
    }

    // Verifica se a tela foi tocada
    private bool IsScreenTouched()
    {
        Touchscreen touchScreen = Touchscreen.current;
        if (touchScreen != null && touchScreen.touches.Count > 0)
        {
            return touchScreen.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began;
        }
        return false;
    }

    private void EnterVR()
    {
        StartCoroutine(StartXR());
        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }
    }

    private void ExitVR()
    {
        StopXR();
    }

    private IEnumerator StartXR()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Debug.Log("XR started.");
        }
    }

    private void StopXR()
    {
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        _mainCamera.ResetAspect();
        _mainCamera.fieldOfView = _defaultFieldOfView;
    }
}
