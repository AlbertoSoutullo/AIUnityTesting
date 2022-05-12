// Unity Imports
using UnityEngine;

namespace PCG.Data
{
    [CreateAssetMenu()]
    public class MeshSettings : UpdatableData
    {
        public const int NumSupportedLODs = 5;

        public static int numVertsPerLine => 240 + 1;
    }
}
