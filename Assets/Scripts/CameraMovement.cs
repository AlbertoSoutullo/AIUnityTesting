using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;

    private Transform _targetTransform;

    public float smoothSpeed;

    private void Start()
    {
        target = GameObject.FindWithTag("Player");
        if (target == null)
        {
            Debug.Log("Player still not instantiated");
            Player.InstanceStarted += OnPlayerInstanceStarted;
            GameObject emptyGO = new GameObject();
            _targetTransform = emptyGO.transform;
        }
        else
        {
            _targetTransform = target.transform;
        }
    }
    
    private void OnPlayerInstanceStarted(Player instance)
    {
        Player.InstanceStarted -= OnPlayerInstanceStarted;
        target = instance.gameObject;
        _targetTransform = target.transform;
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = _targetTransform.position + offset;
        transform.position = Vector3.Slerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(_targetTransform);
    }
}
