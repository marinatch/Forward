using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public float speed;

    void Update()
    {
        //Solamente va a trabajar si el Target existe
        if(target != null)
        {
            Vector3 finalPos = transform.position;//de este manera el finalPos tiene las coordinadas de la camera
            finalPos.x = target.position.x;//y despues solo actualiza la posicion del x, para que sea igual al x del target
            transform.position = Vector3.Lerp(transform.position, finalPos, speed * Time.deltaTime);
        }
    }
}
