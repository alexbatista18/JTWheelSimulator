using System.Collections;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Material InactiveMaterial;
    public Material GazedAtMaterial;
    public GameObject ConfirmationObject;
    public float ConfirmationHideDelay = 1.0f; // Tempo de atraso para esconder o objeto de confirmação

    private Renderer _myRenderer;
    private Coroutine _confirmationHideCoroutine;
    private bool _hasConfirmed = false; // Controla se a confirmação já foi feita

    private void Start()
    {
        _myRenderer = GetComponent<Renderer>();
        SetMaterial(false);
    }

    public void OnPointerEnter()
    {
        if (!_hasConfirmed && ConfirmationObject != null)
        {
            ConfirmationController confirmationController = ConfirmationObject.GetComponent<ConfirmationController>();

            if (confirmationController != null)
            {
                confirmationController.OnConfirmButtonClick();
                _hasConfirmed = true; // Garante que só seja chamado uma vez
            }
        }
        SetMaterial(true);
        ShowConfirmationObject(true);
    }

    public void OnPointerExit()
    {
        SetMaterial(false);
        if (ConfirmationObject != null)
        {
            if (_confirmationHideCoroutine != null)
            {
                StopCoroutine(_confirmationHideCoroutine);
            }
            _confirmationHideCoroutine = StartCoroutine(HideConfirmationAfterDelay());
        }
        _hasConfirmed = false;
    }

    public void OnPointerClick()
    {
        if (ConfirmationObject != null && ConfirmationObject.activeSelf)
        {
            ConfirmSelection();
        }
    }

    private void SetMaterial(bool gazedAt)
    {
        if (InactiveMaterial != null && GazedAtMaterial != null)
        {
            Debug.Log("Entrou");
            _myRenderer.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
        }
    }

    private void ShowConfirmationObject(bool show)
    {
        if (ConfirmationObject != null)
        {
            ConfirmationObject.SetActive(show);
        }
    }

    private IEnumerator HideConfirmationAfterDelay()
    {
        yield return new WaitForSeconds(ConfirmationHideDelay);
    }

    private void ConfirmSelection()
    {
        Debug.Log("Dificuldade confirmada: " + gameObject.name);
        // Aqui você pode adicionar qualquer outra lógica necessária após a confirmação
    }
}
