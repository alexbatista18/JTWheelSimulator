using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rodinha_L : MonoBehaviour
{
    
    public float RotationSpeed = 75f; // Velocidade de rotação

    void FixedUpdate()
    {
        // Movimento para frente e para trás
        if(Input.GetKey(KeyCode.W)){
            transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S)){
            transform.Rotate(Vector3.forward * -RotationSpeed * Time.deltaTime);
        }
    }
}
