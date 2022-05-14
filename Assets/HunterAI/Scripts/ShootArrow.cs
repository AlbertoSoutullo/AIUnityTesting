// Unity Imports
using UnityEngine;

namespace HunterAI.Scripts
{
    public class ShootArrow : MonoBehaviour
    {
        public GameObject projectile;
        public Transform bowPosition;
        public float launchVelocity = 700f;

        private Companion _companion;
    
        public void Shoot()
        {
            GameObject arrow = Instantiate(projectile, bowPosition.position,  
                transform.rotation);
        
            _companion = GetComponent<Companion>();
            Vector3 target = _companion.GetCurrentTarget().transform.position - transform.position;
        
            arrow.GetComponent<Rigidbody>().AddForce(target * launchVelocity);
            gameObject.transform.LookAt(target);
        }
    }
}
