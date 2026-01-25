// Assets/Editor/ExportSpritesToPNGsAndRemap.cs
// Unity 2020+ (tested newer too). Place in an Editor/ folder.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class ExportSpritesToPNGsAndRemap : EditorWindow
{
    private Texture2D spritesheet;         // Source texture (Sprite 2D, Multiple, sliced)
    private DefaultAsset sheetsFolder;
    private DefaultAsset outputFolder;      // Destination folder for .png files
    private bool createOutputFolder = false;
    private bool remapReferences = true;    // Update all serialized references across project
    private bool remapAllAssets = true;
    private DefaultAsset remapFolder;
    private bool skipExisting = true;       // Skip if a .png with same name already exists
    private bool createAtlas = false;
    private bool deleteOldSheet = false;

    // NEW: flipping options
    private bool flipX = false;
    private bool flipY = false;

    [MenuItem("Tools/Sprites/Export Sub-Sprites to PNGs (+ Remap)")]
    public static void Open()
    {
        GetWindow<ExportSpritesToPNGsAndRemap>("Export Sprites to PNGs");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Source spritesheet (Sprite Mode: Multiple)", EditorStyles.boldLabel);
        using (new EditorGUI.DisabledScope(sheetsFolder != null))
        {
            spritesheet = (Texture2D)EditorGUILayout.ObjectField("Spritesheet", spritesheet, typeof(Texture2D), false);
        }
        sheetsFolder = (DefaultAsset)EditorGUILayout.ObjectField("Or folder with sheets", sheetsFolder, typeof(DefaultAsset), false);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        createOutputFolder = EditorGUILayout.Toggle("Create output folder", createOutputFolder);
        using (new EditorGUI.DisabledScope(createOutputFolder))
        {
            outputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Folder", outputFolder, typeof(DefaultAsset), false);
        }

        EditorGUILayout.Space(8);
        remapReferences = EditorGUILayout.Toggle("Remap references in project", remapReferences);
        using (new EditorGUI.DisabledScope(!remapReferences))
        {
            remapAllAssets = EditorGUILayout.Toggle("Remap every asset in project", remapAllAssets);
        }
        using (new EditorGUI.DisabledScope(!remapReferences || remapAllAssets))
        {
            remapFolder = (DefaultAsset)EditorGUILayout.ObjectField("Remap Folder", remapFolder, typeof(DefaultAsset), false);
        }
        skipExisting = EditorGUILayout.Toggle("Skip existing PNGs", skipExisting);

        EditorGUILayout.Space(8);
        createAtlas = EditorGUILayout.Toggle("Create sprite atlas", createAtlas);

        // NEW: flip options
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);
        flipX = EditorGUILayout.Toggle("Flip X (mirror horizontally)", flipX);
        flipY = EditorGUILayout.Toggle("Flip Y (mirror vertically)", flipY);

        EditorGUILayout.Space(8);
        deleteOldSheet = EditorGUILayout.Toggle("Delete old sheet", deleteOldSheet);

        EditorGUILayout.Space(12);
        using (new EditorGUI.DisabledScope((spritesheet == null && sheetsFolder == null) || (outputFolder == null && !createOutputFolder) || (remapReferences && !remapAllAssets && remapFolder == null)))
        {
            if (GUILayout.Button("Export PNGs & Remap"))
            {
                if (sheetsFolder != null)
                {
                    string folderPath = AssetDatabase.GetAssetPath(sheetsFolder);
                    string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });

                    foreach (string guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);

                        // keep only assets directly in folderPath
                        if (System.IO.Path.GetDirectoryName(path).Replace("\\", "/") == folderPath)
                        {
                            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                            if (asset is Texture2D tex)
                            {
                                ExportAndRemapPNGs(tex, outputFolder, createOutputFolder, remapReferences, createAtlas, skipExisting, remapAllAssets, remapFolder, deleteOldSheet, flipX, flipY, true);
                            }
                        }
                    }

                }
                else
                {
                    ExportAndRemapPNGs(spritesheet, outputFolder, createOutputFolder, remapReferences, createAtlas, skipExisting, remapAllAssets, remapFolder, deleteOldSheet, flipX, flipY);
                }
            }
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "• The source texture must be Sprite (2D and UI) with Sprite Mode = Multiple and sliced.\n" +
            "• This creates one .png per sub-sprite and imports each as a Single-sprite texture.\n" +
            "• If 'Remap references' is enabled, any references to the old sub-sprites are replaced with the new .png sprites.\n" +
            "• If Flip X/Y is enabled, exported PNGs are mirrored and pivots/borders are mirrored too.",
            MessageType.Info
        );
    }

    private static void ExportAndRemapPNGs(
        Texture2D spritesheet,
        Object outputFolder,
        bool createOutputFolder,
        bool remapReferences,
        bool createAtlas,
        bool skipExisting,
        bool remapAllAssets,
        Object remapFolder,
        bool deleteOldSheet,
        bool flipX,
        bool flipY,
        bool skipMessage = false)
    {
        string srcPath = AssetDatabase.GetAssetPath(spritesheet);
        if (string.IsNullOrEmpty(srcPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid spritesheet asset path.", "OK");
            return;
        }

        // Collect sub-sprites from the sliced texture
        Object[] subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(srcPath);
        List<Sprite> oldSubSprites = new List<Sprite>();
        foreach (var o in subs)
            if (o is Sprite s) oldSubSprites.Add(s);

        if (oldSubSprites.Count == 0)
        {
            EditorUtility.DisplayDialog("No sub-sprites found",
                "The texture doesn’t appear to be sliced (Sprite Mode = Multiple).", "OK");
            return;
        }

        string outFolderPath = AssetDatabase.GetAssetPath(outputFolder);
        if (!createOutputFolder && !AssetDatabase.IsValidFolder(outFolderPath))
        {
            EditorUtility.DisplayDialog("Error", "Select a valid output folder.", "OK");
            return;
        }
        if (createOutputFolder)
        {
            var folderPath = Path.GetDirectoryName(srcPath).Replace("\\", "/");
            AssetDatabase.CreateFolder(folderPath, spritesheet.name);
            outFolderPath = folderPath + "/" + spritesheet.name;
        }
        // Ensure source is temporarily readable (so we can copy pixels)
        var srcImporter = (TextureImporter)AssetImporter.GetAtPath(srcPath);
        bool originalReadable = srcImporter.isReadable;
        var originalCompression = srcImporter.textureCompression;

        SpriteAtlas newAtlas = null;
        if (createAtlas)
        {
            newAtlas = new SpriteAtlas();

            SpriteAtlasPackingSettings packSettings = new SpriteAtlasPackingSettings()
            {
                enableRotation = true,
                enableTightPacking = true,
                padding = 4
            };
            newAtlas.SetPackingSettings(packSettings);

            var atlasPath = Path.Combine(outFolderPath, spritesheet.name + ".spriteatlas");
            AssetDatabase.CreateAsset(newAtlas, atlasPath);
        }

        try
        {
            if (!originalReadable)
            {
                srcImporter.isReadable = true;
                // Lossless path helps avoid artifacts when copying
                srcImporter.textureCompression = TextureImporterCompression.Uncompressed;
                AssetDatabase.ImportAsset(srcPath, ImportAssetOptions.ForceUpdate);
            }

            // Read all pixels from the source into a readable Texture2D copy (handles compressed formats)
            Texture2D readableSource = GetReadableCopy(spritesheet);

            // Create map: old sub-sprite -> new imported .png sprite
            var remapMap = new Dictionary<Sprite, Sprite>();

            for (int i = 0; i < oldSubSprites.Count; i++)
            {
                var oldS = oldSubSprites[i];
                EditorUtility.DisplayProgressBar("Exporting PNGs", oldS.name, (float)i / oldSubSprites.Count);

                string safeName = SanitizeFileName(oldS.name);
                string pngPath = Path.Combine(outFolderPath, safeName + ".png").Replace("\\", "/");

                if (skipExisting && File.Exists(pngPath))
                {
                    // Load the sprite created by importer
                    var existingSprite = LoadSingleSpriteAtPath(pngPath);
                    if (existingSprite != null)
                    {
                        remapMap[oldS] = existingSprite;
                        continue;
                    }
                }

                // AFTER you set srcImporter.isReadable = true and reimport:
                Texture2D sourceReadable = spritesheet; // it is now readable with exact texels

                // After cropping from the sheet:
                Rect r = oldS.rect;
                int w = Mathf.RoundToInt(r.width);
                int h = Mathf.RoundToInt(r.height);
                int x = Mathf.RoundToInt(r.x);
                int y = Mathf.RoundToInt(r.y);

                // exact texels, no blit:
                Color[] pixels = sourceReadable.GetPixels(x, y, w, h);

                // Optional padding (currently 0)
                int pad = 0;
                Color[] masked = ApplyTightAlphaMask(pixels, w, h, oldS, pad);

                // NEW: flip pixels if requested
                if (flipX || flipY)
                    masked = FlipPixels(masked, w, h, flipX, flipY);

                var newTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                newTex.SetPixels(masked);
                newTex.Apply(false, false);

                byte[] png = newTex.EncodeToPNG();
                File.WriteAllBytes(pngPath, png);

                // Import the PNG
                AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceSynchronousImport);
                var newImp = (TextureImporter)AssetImporter.GetAtPath(pngPath);
                newImp.textureType = TextureImporterType.Sprite;
                newImp.spriteImportMode = SpriteImportMode.Single;
                newImp.spritePixelsPerUnit = oldS.pixelsPerUnit;

                // NEW: mirror 9-slice borders when flipped
                Vector4 border = oldS.border; // L, B, R, T
                if (flipX) (border.x, border.z) = (border.z, border.x);
                if (flipY) (border.y, border.w) = (border.w, border.y);
                newImp.spriteBorder = border;

                newImp.textureCompression = TextureImporterCompression.Uncompressed;
                newImp.isReadable = false;

                TextureImporterSettings s = new TextureImporterSettings();
                newImp.ReadTextureSettings(s);
                s.spriteMeshType = SpriteMeshType.Tight;
                s.readable = true;

                // NEW: mirror pivot when flipped (importer expects normalized [0..1])
                float pivX_px = oldS.pivot.x;
                float pivY_px = oldS.pivot.y;
                if (flipX) pivX_px = r.width - pivX_px;
                if (flipY) pivY_px = r.height - pivY_px;

                s.spritePivot = new Vector2(pivX_px / r.width, pivY_px / r.height);
                newImp.SetTextureSettings(s);

                AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

                // Load the resulting Sprite sub-asset produced by importer
                var newSprite = LoadSingleSpriteAtPath(pngPath);
                if (newSprite == null)
                {
                    Debug.LogWarning($"Could not load Sprite from {pngPath}");
                }
                else
                {
                    // Keep the same logical name for convenience
                    newSprite.name = oldS.name;
                    remapMap[oldS] = newSprite;
                    if (createAtlas)
                    {
                        // Prefer adding the Texture2D asset
                        var tex2D = AssetDatabase.LoadAssetAtPath<Texture2D>(pngPath);
                        if (tex2D != null)
                            newAtlas.Add(new Object[] { tex2D });
                    }

                }
            }

            if (createAtlas && newAtlas != null)
            {
                // Packs just this atlas for the current build target
                SpriteAtlasUtility.PackAtlases(
                    new SpriteAtlas[] { newAtlas },
                    EditorUserBuildSettings.activeBuildTarget);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (remapReferences)
                RemapAllReferences(remapMap, remapAllAssets, AssetDatabase.GetAssetPath(remapFolder));
        }
        finally
        {
            // Restore importer settings
            var srcPathLocal = AssetDatabase.GetAssetPath(spritesheet);
            var srcImporterLocal = (TextureImporter)AssetImporter.GetAtPath(srcPathLocal);
            srcImporterLocal.isReadable = originalReadable;
            srcImporterLocal.textureCompression = originalCompression;
            AssetDatabase.ImportAsset(srcPathLocal, ImportAssetOptions.ForceUpdate);

            EditorUtility.ClearProgressBar();

            if (deleteOldSheet)
            {
                AssetDatabase.DeleteAsset(srcPathLocal);
            }
        }
        if (!skipMessage)
        {
            EditorUtility.DisplayDialog(
                "Done",
                $"Exported sprites to PNGs in:\n{outFolderPath}\n" +
                (remapReferences ? "and remapped references across the project." : " (no remap)."),
                "OK"
            );
        }
    }

    // ---- Helpers ----

    // Builds an alpha mask from oldS.vertices/triangles and applies it to the cropped pixels.
    // If you add 'pad' pixels of gutter later, pass the same pad to shift the outline.
    private static Color[] ApplyTightAlphaMask(Color[] src, int w, int h, Sprite oldS, int pad)
    {
        var dst = new Color[src.Length];
        System.Array.Copy(src, dst, src.Length);

        int W = w + pad * 2;
        int H = h + pad * 2;

        bool[] keep = new bool[W * H];

        float ppu = oldS.pixelsPerUnit;
        var verts = oldS.vertices;
        var tris = oldS.triangles;

        var vpx = new Vector2[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            Vector2 vp = verts[i] * ppu + oldS.pivot; // pixels from rect bottom-left
            vp += new Vector2(pad, pad);              // shift if padding around crop
            vpx[i] = vp;
        }

        for (int t = 0; t < tris.Length; t += 3)
        {
            Vector2 p0 = vpx[tris[t + 0]];
            Vector2 p1 = vpx[tris[t + 1]];
            Vector2 p2 = vpx[tris[t + 2]];

            int minX = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(p0.x, Mathf.Min(p1.x, p2.x))), 0, W - 1);
            int maxX = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(p0.x, Mathf.Max(p1.x, p2.x))), 0, W - 1);
            int minY = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(p0.y, Mathf.Min(p1.y, p2.y))), 0, H - 1);
            int maxY = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(p0.y, Mathf.Max(p1.y, p2.y))), 0, H - 1);

            float Edge(Vector2 a, Vector2 b, Vector2 c) => (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
            float area = Edge(p0, p1, p2);
            if (Mathf.Approximately(area, 0f)) continue;

            for (int yy = minY; yy <= maxY; yy++)
            {
                for (int xx = minX; xx <= maxX; xx++)
                {
                    Vector2 p = new Vector2(xx + 0.5f, yy + 0.5f);
                    float w0 = Edge(p1, p2, p);
                    float w1 = Edge(p2, p0, p);
                    float w2 = Edge(p0, p1, p);

                    bool inside = (w0 >= 0 && w1 >= 0 && w2 >= 0) || (w0 <= 0 && w1 <= 0 && w2 <= 0);
                    if (inside) keep[yy * W + xx] = true;
                }
            }
        }

        for (int yy = 0; yy < h; yy++)
        {
            for (int xx = 0; xx < w; xx++)
            {
                int si = yy * w + xx;
                int mi = (yy + pad) * W + (xx + pad);
                if (!keep[mi]) { var c = dst[si]; c.a = 0f; dst[si] = c; }
            }
        }

        return dst;
    }

    // NEW: flip pixels in-place copy (returns a new array)
    private static Color[] FlipPixels(Color[] src, int w, int h, bool flipX, bool flipY)
    {
        if (!flipX && !flipY) return src;

        Color[] dst = new Color[src.Length];

        for (int y = 0; y < h; y++)
        {
            int srcY = flipY ? (h - 1 - y) : y;
            for (int x = 0; x < w; x++)
            {
                int srcX = flipX ? (w - 1 - x) : x;
                dst[y * w + x] = src[srcY * w + srcX];
            }
        }
        return dst;
    }

    private static Texture2D CreatePaddedTexture(int w, int h, Color[] src, int pad)
    {
        int W = w + 2 * pad, H = h + 2 * pad;
        var tex = new Texture2D(W, H, TextureFormat.RGBA32, false);

        tex.SetPixels(pad, pad, w, h, src);

        for (int yy = 0; yy < h; yy++)
        {
            Color left = src[yy * w + 0];
            Color right = src[yy * w + (w - 1)];

            for (int p = 0; p < pad; p++)
            {
                tex.SetPixel(p, pad + yy, left);
                tex.SetPixel(pad + w + p, pad + yy, right);
            }
        }

        for (int xx = 0; xx < w; xx++)
        {
            Color bottom = src[0 * w + xx];
            Color top = src[(h - 1) * w + xx];

            for (int p = 0; p < pad; p++)
            {
                tex.SetPixel(pad + xx, p, bottom);
                tex.SetPixel(pad + xx, pad + h + p, top);
            }
        }

        Color bl = src[0];
        Color br = src[w - 1];
        Color tl = src[(h - 1) * w + 0];
        Color tr = src[(h - 1) * w + (w - 1)];
        for (int py = 0; py < pad; py++)
        {
            for (int px = 0; px < pad; px++)
            {
                tex.SetPixel(px, py, bl);
                tex.SetPixel(px, pad + h + py, tl);
                tex.SetPixel(pad + w + px, py, br);
                tex.SetPixel(pad + w + px, pad + h + py, tr);
            }
        }

        tex.Apply(false, false);
        return tex;
    }

    private static Texture2D GetReadableCopy(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(source, rt);
        var prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0, false);
        tex.Apply(false, false);

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        return tex;
    }

    private static Sprite LoadSingleSpriteAtPath(string texturePath)
    {
        var s = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
        if (s != null) return s;

        var subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(texturePath);
        foreach (var o in subs)
            if (o is Sprite sp) return sp;

        return null;
    }

    private static string SanitizeFileName(string n)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            n = n.Replace(c, '_');
        return n;
    }

    private static void RemapAllReferences(Dictionary<Sprite, Sprite> map, bool remapAll, string remapFolder)
    {
        if (map == null || map.Count == 0) return;
        if (remapAll) { remapFolder = "Assets"; }

        // ---------- PASS 1: Non-scene/prefab assets ----------
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { remapFolder });
        int changedAssetFiles = 0;
        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = 0; i < assetGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                string ext = System.IO.Path.GetExtension(path).ToLowerInvariant();

                if (ext == ".prefab" || ext == ".unity") continue;

                EditorUtility.DisplayProgressBar("Remapping (assets)", path, (float)i / assetGuids.Length);

                bool changed = false;
                var main = AssetDatabase.LoadMainAssetAtPath(path);
                if (main != null && RemapObjectReferences(main, map))
                    changed = true;

                var subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                foreach (var o in subs)
                    if (RemapObjectReferences(o, map))
                        changed = true;

                if (changed && main != null)
                {
                    EditorUtility.SetDirty(main);
                    changedAssetFiles++;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        // ---------- PASS 2: Prefabs ----------
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { remapFolder });
        int changedPrefabs = 0;

        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            EditorUtility.DisplayProgressBar("Remapping (prefabs)", path, (float)i / prefabGuids.Length);

            GameObject root = PrefabUtility.LoadPrefabContents(path);
            if (root == null) continue;

            bool changed = RemapInGameObjectHierarchy(root, map);

            if (changed)
            {
                changedPrefabs++;
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }

            PrefabUtility.UnloadPrefabContents(root);
        }
        EditorUtility.ClearProgressBar();

        // ---------- PASS 3: Scenes ----------
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { remapFolder });
        int changedScenes = 0;

        for (int i = 0; i < sceneGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            EditorUtility.DisplayProgressBar("Remapping (scenes)", path, (float)i / sceneGuids.Length);

            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            bool changed = false;

            foreach (var root in scene.GetRootGameObjects())
                if (RemapInGameObjectHierarchy(root, map))
                    changed = true;

            if (changed)
            {
                changedScenes++;
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            EditorSceneManager.CloseScene(scene, true);
        }
        EditorUtility.ClearProgressBar();

        Debug.Log($"Sprite PNG Remap: assets changed={changedAssetFiles}, prefabs changed={changedPrefabs}, scenes changed={changedScenes}");
    }

    private static bool RemapInGameObjectHierarchy(GameObject root, Dictionary<Sprite, Sprite> map)
    {
        bool changed = false;
        var comps = root.GetComponentsInChildren<Component>(true);

        foreach (var c in comps)
        {
            if (!c) continue;
            var so = new SerializedObject(c);
            var it = so.GetIterator();
            bool enter = true;

            while (it.Next(enter))
            {
                enter = true;
                if (it.propertyType == SerializedPropertyType.ObjectReference)
                {
                    var curr = it.objectReferenceValue;
                    if (curr is Sprite s && s != null && map.TryGetValue(s, out var replacement))
                    {
                        Undo.RecordObject(c, "Remap Sprite");
                        it.objectReferenceValue = replacement;
                        changed = true;
                    }
                }
            }

            if (changed) so.ApplyModifiedProperties();
            if (changed) EditorUtility.SetDirty(c);
        }

        return changed;
    }

    private static bool RemapObjectReferences(Object obj, Dictionary<Sprite, Sprite> map)
    {
        if (obj == null) return false;

        bool changed = false;
        var so = new SerializedObject(obj);
        var prop = so.GetIterator();
        bool enterChildren = true;

        while (prop.Next(enterChildren))
        {
            enterChildren = true;
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                var curr = prop.objectReferenceValue;
                if (curr is Sprite s && s != null && map.TryGetValue(s, out var replacement))
                {
                    prop.objectReferenceValue = replacement;
                    changed = true;
                }
            }
        }

        if (changed) so.ApplyModifiedProperties();
        return changed;
    }
}
