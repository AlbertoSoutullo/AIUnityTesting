// Unity Imports
using System.Linq;
using UnityEngine; 

namespace PCG.Data
{
    [CreateAssetMenu()]
    public class TextureData : UpdatableData
    {
        public Layer[] layers;
        
        private const int TextureSize = 512;
        private const TextureFormat TextureFormat = UnityEngine.TextureFormat.RGB565;

        private float _savedMinHeight;
        private float _savedMaxHeight;
        
        private static readonly int LayerCount = Shader.PropertyToID("layerCount");
        private static readonly int BaseColours = Shader.PropertyToID("baseColours");
        private static readonly int BaseStartHeights = Shader.PropertyToID("baseStartHeights");
        private static readonly int BaseBlends = Shader.PropertyToID("baseBlends");
        private static readonly int BaseColourStrength = Shader.PropertyToID("baseColourStrength");
        private static readonly int BaseTextureScales = Shader.PropertyToID("baseTextureScales");
        private static readonly int BaseTextures = Shader.PropertyToID("baseTextures");
        private static readonly int MINHeight = Shader.PropertyToID("minHeight");
        private static readonly int MAXHeight = Shader.PropertyToID("maxHeight");

        public void ApplyToMaterial(Material material) 
        {
            material.SetInt(LayerCount, layers.Length);
            material.SetColorArray(BaseColours, layers.Select(x => x.tint).ToArray());
            material.SetFloatArray(BaseStartHeights, layers.Select(x => x.startHeight).ToArray());
            material.SetFloatArray(BaseBlends, layers.Select(x => x.blendStrength).ToArray());
            material.SetFloatArray(BaseColourStrength, layers.Select(x => x.tintStrength).ToArray());
            material.SetFloatArray(BaseTextureScales, layers.Select(x => x.textureScale).ToArray());
            Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray ());
            material.SetTexture(BaseTextures, texturesArray);

            UpdateMeshHeights(material, _savedMinHeight, _savedMaxHeight);
        }

        public void UpdateMeshHeights(Material material, float minHeight, float maxHeight) {
            _savedMinHeight = minHeight;
            _savedMaxHeight = maxHeight;

            material.SetFloat (MINHeight, minHeight);
            material.SetFloat (MAXHeight, maxHeight);
        }

        Texture2DArray GenerateTextureArray(Texture2D[] textures) {
            Texture2DArray textureArray = new Texture2DArray(TextureSize, TextureSize, textures.Length, 
                TextureFormat, true);
            
            for (int i = 0; i < textures.Length; i++) {
                textureArray.SetPixels (textures [i].GetPixels (), i);
            }
            textureArray.Apply ();
            return textureArray;
        }

        [System.Serializable]
        public class Layer {
            public Texture2D texture;
            public Color tint;
            [Range(0,1)]
            public float tintStrength;
            [Range(0,1)]
            public float startHeight;
            [Range(0,1)]
            public float blendStrength;
            public float textureScale;
        }
    }
}
