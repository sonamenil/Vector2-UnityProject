// Assets/Editor/ForcePackAtlases.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;

public static class ForcePackAtlases
{
    [MenuItem("Tools/Sprite Atlases/Pack All (Editor)")]
    public static void PackAll()
    {
        var atlases = AssetDatabase.FindAssets("t:SpriteAtlas");
        var atlasObjs = new SpriteAtlas[atlases.Length];
        for (int i = 0; i < atlases.Length; i++)
            atlasObjs[i] = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(atlases[i]));

        // Packs for the active build target; pass 'true' to clear cache.
        SpriteAtlasUtility.PackAtlases(atlasObjs, EditorUserBuildSettings.activeBuildTarget, false);
        UnityEngine.Debug.Log($"Packed {atlasObjs.Length} Sprite Atlases.");
    }
}
#endif
