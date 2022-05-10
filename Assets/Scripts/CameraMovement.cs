using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    public float smoothSpeed;
    
    private Transform _targetTransform;
    private GameObject _backupGameObject;
    
    private void Start()
    {
        target = GameObject.FindWithTag("Player");
        
        if (target == null)
            PrepareActionWhenTargetSpawn();
        else
            AssignTargetToFollow();
    }

    private void PrepareActionWhenTargetSpawn()
    {
        Debug.Log("Player still not instantiated");
        Player.InstanceStarted += OnPlayerInstanceStarted;
        _backupGameObject = new GameObject();
        _targetTransform = _backupGameObject.transform;
    }

    private void AssignTargetToFollow()
    {
        _targetTransform = target.transform;
    }

    private void OnPlayerInstanceStarted(Player instance)
    {
        Player.InstanceStarted -= OnPlayerInstanceStarted;
        target = instance.gameObject;
        _targetTransform = target.transform;
        Destroy(_backupGameObject);
    }
    
    void LateUpdate()
    {
        Vector3 desiredPosition = _targetTransform.position + offset;
        transform.position = Vector3.Slerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(_targetTransform);
    }
}
