// Unity Imports
using System;
using System.Collections.Generic;
using UnityEngine;

// Project Imports
using PCG.Data;

namespace PCG
{
    public class PutPrefabsInMap : MonoBehaviour
    {
        public PrefabsData prefabsData;
        public HeightMapSettings heightMapSettings;

        public void GeneratePrefabs(Mesh meshData, Vector2 position, Transform parent)
        {
            Vector3 chunkOffset = new Vector3(position.x, 0, position.y);
            
            Vector3[] positions = SelectPossiblePositions(meshData, chunkOffset);
            
            PrefabsInternalData prefabsInternalData = PrefabsGenerator.DeterminePrefabsPositions(MeshSettings.numVertsPerLine,
                heightMapSettings.heightMultiplier, positions, prefabsData);
            
            InstantiatePrefabs(prefabsInternalData, parent);
        }
        
        private Vector3[] SelectPossiblePositions(Mesh meshData, Vector3 chunkOffset)
        {
            int[] triangles = meshData.triangles;
            // if (triangles.Length <= 1) return; todo check
            
            Vector3[] positions = new Vector3[triangles.Length / 3];
            int count = 0;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                // Only selecting the first vertex for each triangle
                positions[count] = meshData.vertices[triangles[i]] + chunkOffset;
                ++count;
            }

            return positions;
        }

        private void InstantiatePrefabs(PrefabsInternalData prefabsInternalData, Transform parent)
        {
            Debug.Log("All prefabs to instantiate: " + prefabsInternalData.PrefabsPositions.Count);
            List<Tuple<int, int>> prefabsPositions = prefabsInternalData.PrefabsPositions;
            
            GameObject[] parents = CreateParents(prefabsInternalData, parent);
        
            InstantiateRandomPrefabs(prefabsPositions, prefabsInternalData, parents);
        }
        
        private void InstantiateRandomPrefabs(List<Tuple<int, int>> prefabsPositions, 
            PrefabsInternalData prefabsInternalData, GameObject[] parents)
        {
            var random = new System.Random();
            for (int i = 0; i < prefabsPositions.Count; ++i)
            {
                Vector3 position = prefabsInternalData.Positions[prefabsPositions[i].Item1];
                List<Transform> prefabTransforms = prefabsInternalData.Transforms[prefabsPositions[i].Item2];
                
                Transform prefabTransform = prefabTransforms[random.Next(prefabTransforms.Count)];
            
                var prefab = Instantiate(prefabTransform, parents[prefabsPositions[i].Item2].transform);
                prefab.position = position;
            }
        }

        private GameObject[] CreateParents(PrefabsInternalData prefabsInternalData, Transform parent)
        {
            GameObject[] parents = new GameObject[prefabsInternalData.TransformsNames.Length];
            for (int i = 0; i < prefabsInternalData.TransformsNames.Length; ++i)
            {
                parents[i] = new GameObject(prefabsInternalData.TransformsNames[i]);
                parents[i].transform.SetParent(parent);
            }

            return parents;
        }
    }
}