using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PCG.Data;

[CreateAssetMenu()]
public class PrefabsData : ScriptableObject
{
    public int seed;
    public AnimationCurve noiseImportance;
    public CabinPrefab cabinPrefab; 
    public Prefab[] prefabs;

    [System.Serializable]
    public class Prefab
    {
        public String name;
        public List<Transform> transforms;
        public HeightMapSettings noise;
        public AnimationCurve heightImportance;
    }

    [System.Serializable]
    public class CabinPrefab
    {
        public Transform Transform;
        public AnimationCurve heightImportance;
    }
}
