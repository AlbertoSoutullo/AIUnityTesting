using System;
using System.Collections.Generic;
using PCG.Data;
using UnityEngine;


namespace PCG
{
    public class PutPrefabsInMap : MonoBehaviour
    {
        //todo
        public PrefabsData _prefabsData;
        public HeightMapSettings heightMapSettings;
        
        public void GeneratePrefabs(Mesh meshData, Vector2 position, Transform parent)
        {
            Vector3 chunkOffset = new Vector3(position.x, 0, position.y);
            // loop through all triangles and select the positions to where instantiate prefabs
            int[] triangles = meshData.triangles;
            if (triangles.Length <= 1) return;
            Vector3[] positions = new Vector3[triangles.Length / 3];
            int count = 0;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                // only selecting the first vertex for each triangle (greedy approach)
                positions[count] = meshData.vertices[triangles[i]] + chunkOffset;
                ++count;
            }
            
            // determine where to instantiate the prefabs
            PrefabsInternalData prefabsInternalData = PrefabsGenerator.DeterminePrefabsPositions(241,
                241, heightMapSettings.heightMultiplier, positions, _prefabsData);
        
            // instantiate the prefabs
            InstantiatePrefabs(prefabsInternalData, parent);
        }
        
        // todo : move this out
        public void InstantiatePrefabs(PrefabsInternalData prefabsInternalData, Transform parent)
        {
            Debug.Log("All prefabs to instantiate: " + prefabsInternalData.prefabsPositions.Count);
            List<Tuple<int, int>> prefabsPositions = prefabsInternalData.prefabsPositions;
        
            // create parent objects
            GameObject[] parents = new GameObject[prefabsInternalData.transformsNames.Length];
            for (int i = 0; i < prefabsInternalData.transformsNames.Length; ++i)
            {
                parents[i] = new GameObject(prefabsInternalData.transformsNames[i]);
                parents[i].transform.SetParent(parent);
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
}