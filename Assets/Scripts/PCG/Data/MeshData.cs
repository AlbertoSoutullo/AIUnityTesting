using UnityEngine;

namespace PCG.Data
{
    public class MeshData {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        private int _triangleIndex;

        public MeshData(int meshWidth, int meshHeight) {
            vertices = new Vector3[meshWidth * meshHeight];
            triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
            uvs = new Vector2[meshWidth * meshHeight];
        }

        public void AddTriangle(int a, int b, int c) {
            triangles[_triangleIndex] = a;
            triangles[_triangleIndex + 1] = b;
            triangles[_triangleIndex + 2] = c;
            _triangleIndex += 3;
        }

        public Mesh CreateMesh() {
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs
            };
            
            mesh.RecalculateNormals();
            
            return mesh;
        }
    }
}