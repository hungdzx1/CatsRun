using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class BgMapRounder
{
    private const string MaskSpritePath = "Assets/Sprits/SelectedMap/UIMask_Rounded.png";

    static BgMapRounder()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                return;

            RunAutoRound(false);
        };
    }

    [MenuItem("Tools/Bo góc cho BgMap (Tự động) %#r", false, 0)]
    [MenuItem("GameObject/UI/Bo góc BgMap tự động", false, 0)]
    public static void ManualRoundMenu()
    {
        RunAutoRound(true);
    }

    public static void RunAutoRound(bool isManual)
    {
        // 1. Try to find BgMap in active scenes
        GameObject target = null;

        // Check if user currently has BgMap selected
        if (Selection.activeGameObject != null && Selection.activeGameObject.name.Equals("BgMap", System.StringComparison.OrdinalIgnoreCase))
        {
            target = Selection.activeGameObject;
        }

        // Otherwise find all GameObjects named BgMap in loaded scenes
        if (target == null)
        {
            var images = Resources.FindObjectsOfTypeAll<Image>();
            foreach (var img in images)
            {
                if (img.gameObject.scene.isLoaded && img.gameObject.name.Equals("BgMap", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Pick the one that is not yet masked
                    if (img.transform.parent != null && img.transform.parent.name != "BgMap_Mask")
                    {
                        target = img.gameObject;
                        break;
                    }
                    else if (target == null)
                    {
                        target = img.gameObject;
                    }
                }
            }
        }

        if (target == null)
        {
            if (isManual)
            {
                EditorUtility.DisplayDialog("Thông báo", "Không tìm thấy GameObject tên là 'BgMap' trong Scene đang mở.\nHãy chọn đối tượng BgMap trong Hierarchy rồi nhấn lại menu này!", "OK");
            }
            return;
        }

        // Check if already masked
        if (target.transform.parent != null && target.transform.parent.name == "BgMap_Mask")
        {
            if (isManual)
            {
                EditorUtility.DisplayDialog("Thông báo", "BgMap đã được đặt trong Mask bo góc rồi!", "OK");
            }
            return;
        }

        RoundTarget(target, isManual);
    }

    private static void RoundTarget(GameObject bgMapObj, bool isManual)
    {
        Sprite maskSprite = GetOrCreateRoundedSprite();

        RectTransform bgRect = bgMapObj.GetComponent<RectTransform>();
        if (bgRect == null)
        {
            Debug.LogError("[BgMapRounder] BgMap không có RectTransform!");
            return;
        }

        Transform parentTransform = bgMapObj.transform.parent;
        int originalSiblingIndex = bgMapObj.transform.GetSiblingIndex();

        // 1. Create BgMap_Mask GameObject
        GameObject maskObj = new GameObject("BgMap_Mask", typeof(RectTransform), typeof(Image), typeof(Mask));
        Undo.RegisterCreatedObjectUndo(maskObj, "Create BgMap_Mask");

        RectTransform maskRect = maskObj.GetComponent<RectTransform>();
        maskRect.SetParent(parentTransform, false);
        maskRect.SetSiblingIndex(originalSiblingIndex);

        // Copy transform properties from BgMap
        maskRect.anchorMin = bgRect.anchorMin;
        maskRect.anchorMax = bgRect.anchorMax;
        maskRect.pivot = bgRect.pivot;
        maskRect.anchoredPosition = bgRect.anchoredPosition;
        maskRect.sizeDelta = bgRect.sizeDelta;
        maskRect.localScale = bgRect.localScale;
        maskRect.localRotation = bgRect.localRotation;

        // 2. Setup Image on Mask
        Image maskImage = maskObj.GetComponent<Image>();
        maskImage.sprite = maskSprite;
        maskImage.type = Image.Type.Sliced;
        maskImage.color = Color.white;

        // 3. Setup Mask component
        Mask maskComponent = maskObj.GetComponent<Mask>();
        maskComponent.showMaskGraphic = false;

        // 4. Reparent BgMap into BgMap_Mask
        Undo.SetTransformParent(bgMapObj.transform, maskObj.transform, "Reparent BgMap");
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.localScale = Vector3.one;
        bgRect.localRotation = Quaternion.identity;

        // 5. Select the new Mask and ping it
        Selection.activeGameObject = maskObj;
        EditorGUIUtility.PingObject(maskObj);

        // 6. Mark scene dirty so changes can be saved
        EditorSceneManager.MarkSceneDirty(bgMapObj.scene);

        Debug.Log("<color=#00FF7F><b>[CatsRun]</b></color> Đã tự động tạo Mask bo góc cho <b>" + bgMapObj.name + "</b> thành công!");

        if (isManual)
        {
            EditorUtility.DisplayDialog("Thành công!", "Đã tự động bo tròn góc cho BgMap!\n\nCấu trúc mới:\n" + (parentTransform != null ? parentTransform.name : "Parent") + " ➔ BgMap_Mask ➔ BgMap\n\nBạn có thể nhấn Ctrl + S để lưu Scene hoặc Ctrl + Z để hoàn tác.", "Tuyệt vời");
        }
    }

    private static Sprite GetOrCreateRoundedSprite()
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(MaskSpritePath);
        if (sprite != null) return sprite;

        try
        {
            string fullPath = Path.GetFullPath(MaskSpritePath);
            string dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            int size = 128;
            int radius = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float cx = -1f, cy = -1f;
                    if (x < radius && y < radius) { cx = radius; cy = radius; }
                    else if (x < radius && y >= size - radius) { cx = radius; cy = size - radius - 1; }
                    else if (x >= size - radius && y < radius) { cx = size - radius - 1; cy = radius; }
                    else if (x >= size - radius && y >= size - radius) { cx = size - radius - 1; cy = size - radius - 1; }

                    float alpha = 1f;
                    if (cx >= 0f)
                    {
                        float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                        float delta = dist - (radius - 0.5f);
                        if (delta <= -0.5f) alpha = 1f;
                        else if (delta >= 0.5f) alpha = 0f;
                        else alpha = 0.5f - delta;
                    }

                    colors[y * size + x] = new Color(1f, 1f, 1f, Mathf.Clamp01(alpha));
                }
            }

            tex.SetPixels(colors);
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);
            File.WriteAllBytes(fullPath, pngData);

            AssetDatabase.ImportAsset(MaskSpritePath, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(MaskSpritePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spriteBorder = new Vector4(radius, radius, radius, radius);
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }

            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(MaskSpritePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[BgMapRounder] Tạo mask sprite thất bại: " + ex.Message);
        }

        // Fallback to Unity's built-in UISprite if custom sprite could not be loaded
        if (sprite == null)
        {
            sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }

        return sprite;
    }
}
