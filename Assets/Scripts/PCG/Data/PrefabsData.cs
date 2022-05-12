// Unity Imports
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PCG.Data
{
    [CreateAssetMenu()]
    public class PrefabsData : UpdatableData
    {
        public int seed;
        public AnimationCurve noiseImportance;
        public CabinPrefab cabinPrefab; 
        public Prefab[] prefabs;

        [Serializable]
        public class Prefab
        {
            public String name;
            public List<Transform> transforms;
            public HeightMapSettings noise;
            public AnimationCurve heightImportance;
        }

        [Serializable]
        public class CabinPrefab
        {
            public Transform transform;
            public AnimationCurve heightImportance;
        }
    }
}
