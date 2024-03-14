using UnityEngine;
using TMPro;

public class TMPAssetLoader
{
    public const string Tmp_Font_Asset = "PuHuiTi-Medium";
    public static void Initialize(IResourceManager resource)
    {
        TMP_Text.OnFontAssetRequest += (hashcode, asset) =>
        {
            return resource.LoadAssetSync<TMP_FontAsset>(asset);
        };

        TMP_Text.OnMaterialAssetRequest += (asset) =>
        {
            Log.Info(asset);
            return resource.LoadAssetSync<Material>(asset);
          
        };
        TMP_Text.OnSpriteAssetRequest += (spriteAssetHashCode, asset) =>
        {
            return resource.LoadAssetSync<TMP_SpriteAsset>( asset);
        };
    }
}
