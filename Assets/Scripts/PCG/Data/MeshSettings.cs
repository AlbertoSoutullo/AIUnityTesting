using UnityEngine;

namespace PCG.Data
{
    [CreateAssetMenu()]
    public class MeshSettings : UpdatableData
    {
        public const int numSupportedLODs = 5;
        public const int numSupportedChunkSizes = 9;
        public static readonly int[] supportedChunkSizes = {48,72,96,120,144,168,192,216,240};
        
        [Range(0,numSupportedChunkSizes-1)]
        public int chunkSizeIndex;

        public int numVertsPerLine => supportedChunkSizes [chunkSizeIndex] + 1;

        /* todo
        public float meshWorldSize {
            get {
                return (numVertsPerLine - 3) * meshScale;
            }
        }*/
    }
}
