using UnityEngine;

namespace HunterAI.Scripts
{
    public class ShootArrow : MonoBehaviour
    {
        public GameObject projectile;
        public Transform bow_position;
        public float launchVelocity = 700f;

        private CompanionMovement fsm;
    
        public void Shoot()
        {
            GameObject arrow = Instantiate(projectile, bow_position.position,  
                transform.rotation);
        
            fsm = GetComponent<CompanionMovement>();
            Vector3 target = fsm.GetCurrentTarget().transform.position - transform.position;
        
            arrow.GetComponent<Rigidbody>().AddForce(target * launchVelocity);

            // Physics.IgnoreCollision(arrow.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
