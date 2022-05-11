using UnityEngine;

namespace PCG
{
    public class HideOnPlay : MonoBehaviour {
        
        void Start () {
            gameObject.SetActive (false);
        }
    }
}