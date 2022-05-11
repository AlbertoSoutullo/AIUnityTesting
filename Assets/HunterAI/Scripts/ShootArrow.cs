using UnityEngine;

namespace HunterAI.Scripts
{
    public class ShootArrow : MonoBehaviour
    {
        public GameObject projectile;
        public Transform bowPosition;
        public float launchVelocity = 700f;

        private Companion _fsm;
    
        public void Shoot()
        {
            GameObject arrow = Instantiate(projectile, bowPosition.position,  
                transform.rotation);
        
            _fsm = GetComponent<Companion>();
            Vector3 target = _fsm.GetCurrentTarget().transform.position - transform.position;
        
            arrow.GetComponent<Rigidbody>().AddForce(target * launchVelocity);
            gameObject.transform.LookAt(target);
        }
    }
}
