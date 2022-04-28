using UnityEngine;
using System.Collections;

namespace PCG
{
    public class HideOnPlay : MonoBehaviour {
        
        void Start () {
            gameObject.SetActive (false);
        }

    }
}