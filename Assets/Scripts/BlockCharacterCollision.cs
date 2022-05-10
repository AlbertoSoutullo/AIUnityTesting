using UnityEngine;

public class BlockCharacterCollision : MonoBehaviour
{
    public CapsuleCollider characterCollider;

    public CapsuleCollider characterBlockerCollider;
    
    void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterBlockerCollider, true);
    }
}
