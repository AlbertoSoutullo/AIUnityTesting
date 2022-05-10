using UnityEngine;

public class DestroyArrow : MonoBehaviour
{
    private const float TimeToSelfDestroy = 3.0f;
    
    void Start()
    {
        Invoke(nameof(AutoDestroyAfterTime), TimeToSelfDestroy);
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