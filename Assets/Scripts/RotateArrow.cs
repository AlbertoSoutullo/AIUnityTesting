using UnityEngine;

public class RotateArrow : MonoBehaviour
{
    public float rotationSpeed = 50.0f;
    
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
