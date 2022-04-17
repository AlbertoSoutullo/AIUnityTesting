using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float smoothSpeed;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Slerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(target);
    }
}
