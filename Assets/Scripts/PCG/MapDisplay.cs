using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void DrawMesh(MeshData meshData) {
        Mesh mesh = meshData.CreateMesh ();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void InstantiatePrefabs(PrefabsInternalData prefabsInternalData)
    {
        Debug.Log("All prefabs to instantiate: " + prefabsInternalData.prefabsPositions.Count);
        List<Tuple<int, int>> prefabsPositions = prefabsInternalData.prefabsPositions;
        
        // create parent objects
        GameObject[] parents = new GameObject[prefabsInternalData.transformsNames.Length];
        for (int i = 0; i < prefabsInternalData.transformsNames.Length; ++i)
        {
            parents[i] = new GameObject(prefabsInternalData.transformsNames[i]);
        }
        
        // instantiate all prefabs
        var random = new System.Random();
        for (int i = 0; i < prefabsPositions.Count; ++i)
        {
            Vector3 position = prefabsInternalData.positions[prefabsPositions[i].Item1];
            List<Transform> prefabTransforms = prefabsInternalData.transforms[prefabsPositions[i].Item2];
            // sample a random item from the transforms list
            Transform prefabTransform = prefabTransforms[random.Next(prefabTransforms.Count)];
            
            var prefab = Instantiate(prefabTransform, parents[prefabsPositions[i].Item2].transform);
            prefab.position = position;

        }
    }

}