using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyArrow : MonoBehaviour
{
    private float _timeToAutoDestroy = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("AutoDestroyAfterTime", _timeToAutoDestroy);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Destroy(gameObject);
    }

    private void AutoDestroyAfterTime()
    {
        Destroy(gameObject);
    }
    
}