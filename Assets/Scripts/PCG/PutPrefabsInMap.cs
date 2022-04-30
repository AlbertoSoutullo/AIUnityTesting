using PCG.Data;
using UnityEngine;


namespace PCG
{
    public class PutPrefabsInMap : MonoBehaviour
    {
        //todo
        public PrefabsData _prefabsData;
        public HeightMapSettings heightMapSettings;
        
        public void GeneratePrefabs(MeshData meshData)
        {
            // loop through all triangles and select the positions to where instantiate prefabs
            int[] triangles = meshData.triangles;
            Vector3[] positions = new Vector3[triangles.Length / 3];
            int count = 0;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                // only selecting the first vertex for each triangle (greedy approach)
                positions[count] = meshData.vertices[triangles[i]];
                ++count;
            }

            int size = (int) Mathf.Sqrt(meshData.vertices.Length);
            // determine where to instantiate the prefabs
            PrefabsInternalData prefabsInternalData = PrefabsGenerator.DeterminePrefabsPositions(size,
                size, heightMapSettings.heightMultiplier, positions, _prefabsData);
        
            // instantiate the prefabs
            MapPreview display = FindObjectOfType<MapPreview>();
            display.InstantiatePrefabs(prefabsInternalData);
        }
        
    }
}