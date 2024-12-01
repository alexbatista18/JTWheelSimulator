using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelchairController : MonoBehaviour
{
    public float MovementSpeed = 5f; // Velocidade de movimento
    public float RotationSpeed = 100f; // Velocidade de rotação

    void FixedUpdate()
    {
        // Movimento para frente e para trás
        if(Input.GetKey(KeyCode.W)){
            transform.Translate(Vector3.forward * MovementSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S)){
            transform.Translate(-Vector3.forward * MovementSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D)){
            transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A)){
            transform.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
        }
    }
}