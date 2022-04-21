using UnityEngine;

namespace PCG.Data
{
    [CreateAssetMenu()]
    public class TerrainData : ScriptableObject
    {
        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public float minHeight => meshHeightMultiplier * meshHeightCurve.Evaluate(0);

        public float maxHeight => meshHeightMultiplier * meshHeightCurve.Evaluate(1);
    }
}
