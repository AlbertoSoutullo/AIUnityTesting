using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceHealthBar : MonoBehaviour
{

    public Camera main_camera;


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + main_camera.transform.rotation * Vector3.back,
            main_camera.transform.rotation * Vector3.down);
    }
}
