using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementeTeclado : MonoBehaviour
{
    public float MovementSpeed = 5f; // Velocidade de movimento
    public float RotationSpeed = 100f; // Velocidade de rotação

    public Animator animator1; // Referência ao primeiro Animator
    public Animator animator2; // Referência ao segundo Animator

    void FixedUpdate()
    {
        bool isMovingForward = Input.GetKey(KeyCode.W);
        bool isMovingBackward = Input.GetKey(KeyCode.S);
        bool isRotatingRight = Input.GetKey(KeyCode.D);
        bool isRotatingLeft = Input.GetKey(KeyCode.A);

        // Movimento para frente e para trás
        if (isMovingForward)
        {
            transform.Translate(Vector3.forward * MovementSpeed * Time.deltaTime);
        }
        if (isMovingBackward)
        {
            transform.Translate(-Vector3.forward * MovementSpeed * Time.deltaTime);
        }

        // Rotação para a direita e para a esquerda
        if (isRotatingRight)
        {
            transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
        if (isRotatingLeft)
        {
            transform.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
        }

        // Atualiza os animadores
        SetAnimatorBools("frente", isMovingForward);
        SetAnimatorBools("tras", isMovingBackward);
        SetAnimatorBools("direita", isRotatingRight);
        SetAnimatorBools("esquerda", isRotatingLeft);
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
}
