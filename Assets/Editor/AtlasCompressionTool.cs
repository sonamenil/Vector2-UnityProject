// Assets/Editor/AtlasCompressionTool.cs
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasCompressionTool : EditorWindow
{
    // Which platforms to touch. Add/remove platform names if you want.
    // Valid names include: "DefaultTexturePlatform", "Standalone", "Android", "iPhone", "WebGL"
    private static readonly string[] kPlatforms = new[]
    {
        "DefaultTexturePlatform",
        "Standalone",
        "Android",
        "iPhone",
        "WebGL",
    };

    // UI state
    private TextureImporterCompression _compression = TextureImporterCompression.CompressedHQ;
    private bool _overrideForAllPlatforms = true;
    private string[] _platformsUi;
    private bool[] _platformToggles;

    [MenuItem("Tools/Sprite Atlases/Set Compression On All Atlases...")]
    private static void Open()
    {
        var win = GetWindow<AtlasCompressionTool>("Atlas Compression");
        win.minSize = new Vector2(420, 260);
        win.Show();
    }

    private void OnEnable()
    {
        _platformsUi = (string[])kPlatforms.Clone();
        _platformToggles = Enumerable.Repeat(true, _platformsUi.Length).ToArray();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Apply Compression To All Sprite Atlases", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _compression = (TextureImporterCompression)EditorGUILayout.EnumPopup(
            new GUIContent("Compression", "Texture compression level for the platform settings."),
            _compression
        );

        _overrideForAllPlatforms = EditorGUILayout.Toggle(
            new GUIContent("Override For Platforms", "Tick to force an override entry for each chosen platform."),
            _overrideForAllPlatforms
        );

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Platforms to Apply:", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        for (int i = 0; i < _platformsUi.Length; i++)
        {
            _platformToggles[i] = EditorGUILayout.ToggleLeft(_platformsUi[i], _platformToggles[i]);
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space(12);

        if (GUILayout.Button("Apply To All Sprite Atlases"))
        {
            ApplyToAllAtlases(_compression, _overrideForAllPlatforms, _platformsUi, _platformToggles);
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Notes:\n" +
            "• This sets TextureImporterPlatformSettings.textureCompression for the selected platforms.\n" +
            "• Leave the format as 'Automatic' (Unity picks the best per-platform format) and only change compression level.\n" +
            "• You can re-run anytime to change levels (e.g., CompressedHQ ? Uncompressed).",
            MessageType.Info
        );
    }

    private static void ApplyToAllAtlases(TextureImporterCompression compression, bool forceOverride, string[] platforms, bool[] enabledFlags)
    {
        var guids = AssetDatabase.FindAssets("t:SpriteAtlas");
        int modified = 0;

        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if (atlas == null) continue;

            bool changed = false;

            for (int p = 0; p < platforms.Length; p++)
            {
                if (!enabledFlags[p]) continue;
                var platformName = platforms[p];

                // Get current platform settings (creates a default copy internally if none)
                var s = SpriteAtlasExtensions.GetPlatformSettings(atlas, platformName);

                // We keep 'Automatic' format and only change compression level.
                // textureCompression controls Uncompressed/Compressed/CompressedHQ/CompressedLQ
                if (s.textureCompression != compression || s.overridden != forceOverride)
                {
                    s.overridden = forceOverride;              // create/force an override entry if requested
                    s.textureCompression = compression;        // the thing we care about
                    // Optional: keep automatic format (recommended)
                    // s.format = TextureImporterFormat.Automatic;

                    SpriteAtlasExtensions.SetPlatformSettings(atlas, s);
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(atlas);
                modified++;
            }
        }

        AssetDatabase.SaveAssets();
        // Optional: repack to regenerate cached textures for the active build target
        RepackModifiedAtlases();

        EditorUtility.DisplayDialog("Atlas Compression",
            $"Updated compression on {modified} Sprite Atlas asset(s).", "OK");
    }

    // If you want to repack immediately after changing settings, uncomment the calls.
    private static void RepackModifiedAtlases()
    {
        var guids = AssetDatabase.FindAssets("t:SpriteAtlas");
        var atlases = guids
            .Select(g => AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(a => a != null)
            .ToArray();

        if (atlases.Length > 0)
        {
            SpriteAtlasUtility.PackAtlases(atlases, EditorUserBuildSettings.activeBuildTarget, false);
        }
    }
}
#endif
