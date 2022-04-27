using System;
using UnityEngine;

namespace PCG
{
    public class PutPlayerInMap : MonoBehaviour
    {
        public static GameObject Player;
        public static GameObject Hunter;
        
        public static LayerMask TerrainLayer;
        
        private void Start()
        {
            float positionYPlayer = 9999;
            float positionYHunter = 9999;
            RaycastHit hit;
            
            if (Physics.Raycast(new Vector3(0, 9999f, 0), Vector3.down,
                    out hit, Mathf.Infinity, TerrainLayer))
            {
                positionYPlayer = hit.point.y;
                positionYHunter = hit.point.y;
            }

            positionYPlayer += 1.5f / 2;
            positionYHunter += Hunter.GetComponent<CapsuleCollider>().height / 2;
            Vector3 positionPlayer = new Vector3(0, positionYPlayer, 0);
            Vector3 positionHunter = new Vector3(3, positionYHunter, 0);

            var playerObject = Instantiate(Player, positionPlayer, Quaternion.identity);
            Instantiate(Hunter, positionHunter, Quaternion.identity);
        }
    }
}