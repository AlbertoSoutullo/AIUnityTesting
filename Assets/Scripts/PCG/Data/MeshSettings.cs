// Unity Imports
using UnityEngine;

namespace PCG.Data
{
    [CreateAssetMenu()]
    public class MeshSettings : UpdatableData
    {
        public static int numVertsPerLine => 240 + 1;
    }
}
