// Unity Imports
using UnityEngine;

//Project Imports
using PCG.Data;

namespace PCG
{
    public static class MeshGenerator {

        public static MeshData GenerateTerrainMesh(float[,] heightMap, int levelOfDetail)
        {
            int width = heightMap.GetLength (0);
            int height = heightMap.GetLength (1);
            
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 1) / 2f;

            int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
            int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            
            int vertexIndex = 0;

            for (int y = 0; y < height; y += meshSimplificationIncrement) {
                for (int x = 0; x < width; x += meshSimplificationIncrement) {

                    meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y], topLeftZ - y);
                    meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                    
                    if (NotEdgePoints(x, y, width, height)) {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, 
                            vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }
                    vertexIndex++;
                }
            }
            return meshData;
        }

        private static bool NotEdgePoints(int x, int y, int width, int height)
        {
            return (x < width - 1 && y < height - 1);
        }
    }
}