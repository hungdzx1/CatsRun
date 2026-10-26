using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using TMPro;

public static class TextStyleHelper
{
    private const string FontAssetPath = "Assets/Font/LilitaOne-Regular SDF.asset";
    private const string MaterialDir = "Assets/Font/Materials";

    [MenuItem("Tools/Style UI/🌟 1. Áp dụng Style Khuyên Dùng (Gold & Chocolate)", false, 1)]
    public static void ApplyStyleRecommended()
    {
        ApplyPreset(1);
    }

    [MenuItem("Tools/Style UI/💎 2. Áp dụng Style Xanh Navy & Vàng Tươi", false, 2)]
    public static void ApplyStyleNavy()
    {
        ApplyPreset(2);
    }

    [MenuItem("Tools/Style UI/🎮 3. Áp dụng Style Arcade Pop (Chữ Trắng Viền Đậm)", false, 3)]
    public static void ApplyStyleWhiteArcade()
    {
        ApplyPreset(3);
    }

    public static void ApplyPreset(int preset)
    {
        EnsureMaterialDir();

        // Find Name and HighScore GameObjects
        GameObject nameObj = FindUIObject("Name");
        GameObject scoreObj = FindUIObject("HighScore");

        if (nameObj == null && scoreObj == null)
        {
            EditorUtility.DisplayDialog("Thông báo", "Không tìm thấy GameObject 'Name' hoặc 'HighScore' trong Scene!\nHãy đảm bảo bạn đang mở Scene chứa các đối tượng này.", "OK");
            return;
        }

        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (fontAsset == null)
        {
            Debug.LogWarning("[TextStyleHelper] Không tìm thấy font asset tại: " + FontAssetPath);
        }

        if (preset == 1)
        {
            // Preset 1: Khuyên Dùng (Name: Nâu tím huyền bí; HighScore: Vàng Cúp Hoàng Gia)
            if (nameObj != null)
            {
                Color face = new Color(0.18f, 0.08f, 0.22f, 1f);      // #2E1437 Nâu tím đậm
                Color outline = new Color(0.1f, 0.04f, 0.12f, 1f);   // #1A0A1F
                Color shadow = new Color(0f, 0f, 0f, 0.45f);
                ApplyStyleToTarget(nameObj, fontAsset, "LilitaOne_Name_Choco.mat", face, outline, 0.08f, shadow, 0f, -0.65f, 0.1f, 0.2f);
            }

            if (scoreObj != null)
            {
                Color face = new Color(1f, 0.72f, 0.05f, 1f);        // #FFB80D Vàng kim rực rỡ
                Color outline = new Color(0.36f, 0.15f, 0f, 1f);     // #5C2600 Nâu cam hổ phách
                Color shadow = new Color(0.2f, 0.06f, 0f, 0.6f);     // Đổ bóng nâu đen
                ApplyStyleToTarget(scoreObj, fontAsset, "LilitaOne_Score_Gold.mat", face, outline, 0.18f, shadow, 0.5f, -0.7f, 0.15f, 0.2f);
            }

            Debug.Log("<color=#FFD700><b>[CatsRun]</b></color> Đã áp dụng <b>Style Khuyên Dùng (Gold & Chocolate)</b> thành công!");
            EditorUtility.DisplayDialog("Thành công!", "Đã áp dụng Style Khuyên Dùng:\n\n• Name: Màu Nâu Tím Huyền Bí + Drop Shadow mượt mà.\n• HighScore: Màu Vàng Cúp Hoàng Gia 🏆 + Viền hổ phách + Đổ bóng 3D.\n\nNhấn Ctrl + S để lưu Scene!", "Tuyệt vời");
        }
        else if (preset == 2)
        {
            // Preset 2: Xanh Navy & Vàng Tươi
            if (nameObj != null)
            {
                Color face = new Color(0.08f, 0.14f, 0.24f, 1f);     // #14243D Xanh Navy đậm
                Color outline = new Color(0.04f, 0.08f, 0.14f, 1f);
                Color shadow = new Color(0f, 0f, 0f, 0.4f);
                ApplyStyleToTarget(nameObj, fontAsset, "LilitaOne_Name_Navy.mat", face, outline, 0.06f, shadow, 0f, -0.6f, 0.08f, 0.15f);
            }

            if (scoreObj != null)
            {
                Color face = new Color(1f, 0.85f, 0.1f, 1f);         // #FFD91A Vàng tươi
                Color outline = new Color(0.42f, 0.18f, 0.02f, 1f);  // #6B2E05
                Color shadow = new Color(0f, 0f, 0f, 0.5f);
                ApplyStyleToTarget(scoreObj, fontAsset, "LilitaOne_Score_BrightGold.mat", face, outline, 0.16f, shadow, 0.5f, -0.65f, 0.12f, 0.2f);
            }

            Debug.Log("<color=#00BFFF><b>[CatsRun]</b></color> Đã áp dụng <b>Style Xanh Navy & Vàng Tươi</b> thành công!");
            EditorUtility.DisplayDialog("Thành công!", "Đã áp dụng Style Xanh Navy & Vàng Tươi!\n\nNhấn Ctrl + S để lưu Scene!", "OK");
        }
        else if (preset == 3)
        {
            // Preset 3: Arcade Pop (Chữ Trắng Viền Đậm)
            if (nameObj != null)
            {
                Color face = Color.white;
                Color outline = new Color(0.18f, 0.08f, 0.28f, 1f);  // Viền Tím đậm
                Color shadow = new Color(0f, 0f, 0f, 0.5f);
                ApplyStyleToTarget(nameObj, fontAsset, "LilitaOne_Name_WhiteArcade.mat", face, outline, 0.22f, shadow, 0.5f, -0.8f, 0.12f, 0.2f);
            }

            if (scoreObj != null)
            {
                Color face = Color.white;
                Color outline = new Color(0.02f, 0.38f, 0.62f, 1f);  // Viền Xanh dương đậm
                Color shadow = new Color(0f, 0.12f, 0.25f, 0.6f);
                ApplyStyleToTarget(scoreObj, fontAsset, "LilitaOne_Score_WhiteBlue.mat", face, outline, 0.24f, shadow, 0.5f, -0.8f, 0.15f, 0.2f);
            }

            Debug.Log("<color=#FF69B4><b>[CatsRun]</b></color> Đã áp dụng <b>Style Arcade Pop (Chữ Trắng Viền Đậm)</b> thành công!");
            EditorUtility.DisplayDialog("Thành công!", "Đã áp dụng Style Arcade Pop!\n\nNhấn Ctrl + S để lưu Scene!", "OK");
        }
    }

    private static void ApplyStyleToTarget(GameObject obj, TMP_FontAsset fontAsset, string matFileName,
        Color faceColor, Color outlineColor, float outlineWidth,
        Color shadowColor, float shadowOffsetX, float shadowOffsetY, float shadowDilate, float shadowSoftness)
    {
        Undo.RecordObject(obj, "Apply Text Style");

        // 1. TextMeshProUGUI
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            if (fontAsset != null && tmp.font != fontAsset)
            {
                tmp.font = fontAsset;
            }

            Material mat = GetOrCreateMaterial(fontAsset, matFileName);
            if (mat != null)
            {
                // Set Face Color
                mat.SetColor(ShaderUtilities.ID_FaceColor, faceColor);

                // Set Outline
                if (outlineWidth > 0.001f)
                {
                    mat.EnableKeyword(ShaderUtilities.Keyword_Outline);
                    mat.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);
                    mat.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);
                }
                else
                {
                    mat.DisableKeyword(ShaderUtilities.Keyword_Outline);
                    mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0);
                }

                // Set Underlay (Shadow)
                mat.EnableKeyword(ShaderUtilities.Keyword_Underlay);
                mat.SetColor(ShaderUtilities.ID_UnderlayColor, shadowColor);
                mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, shadowOffsetX);
                mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, shadowOffsetY);
                mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, shadowDilate);
                mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, shadowSoftness);

                EditorUtility.SetDirty(mat);
                AssetDatabase.SaveAssets();

                tmp.fontSharedMaterial = mat;
            }

            tmp.color = Color.white; // Avoid color multiplying with face color
            tmp.UpdateMeshPadding();
            tmp.ForceMeshUpdate();
            EditorUtility.SetDirty(tmp);
            EditorSceneManager.MarkSceneDirty(obj.scene);
            return;
        }

        // 2. Legacy UI Text
        Text legacyText = obj.GetComponent<Text>();
        if (legacyText != null)
        {
            legacyText.color = faceColor;

            // Shadow component
            Shadow shadow = obj.GetComponent<Shadow>();
            if (shadow == null) shadow = obj.AddComponent<Shadow>();
            shadow.effectColor = shadowColor;
            shadow.effectDistance = new Vector2(shadowOffsetX * 4f, shadowOffsetY * 4f);

            // Outline component
            Outline outline = obj.GetComponent<Outline>();
            if (outlineWidth > 0.001f)
            {
                if (outline == null) outline = obj.AddComponent<Outline>();
                outline.effectColor = outlineColor;
                outline.effectDistance = new Vector2(outlineWidth * 3f, -outlineWidth * 3f);
            }
            else if (outline != null)
            {
                Object.DestroyImmediate(outline);
            }

            EditorUtility.SetDirty(legacyText);
            EditorSceneManager.MarkSceneDirty(obj.scene);
        }
    }

    private static Material GetOrCreateMaterial(TMP_FontAsset fontAsset, string fileName)
    {
        string path = MaterialDir + "/" + fileName;
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        Material baseMat = fontAsset != null ? fontAsset.material : null;
        if (baseMat == null)
        {
            Shader shader = Shader.Find("TextMeshPro/Distance Field");
            if (shader == null) shader = Shader.Find("TextMeshPro/Mobile/Distance Field");
            mat = new Material(shader);
        }
        else
        {
            mat = new Material(baseMat);
        }

        mat.name = Path.GetFileNameWithoutExtension(fileName);
        AssetDatabase.CreateAsset(mat, path);
        AssetDatabase.SaveAssets();
        return mat;
    }

    private static GameObject FindUIObject(string name)
    {
        // 1. Check current selection
        if (Selection.activeGameObject != null && Selection.activeGameObject.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            return Selection.activeGameObject;

        // 2. Search scene objects
        var tmps = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var t in tmps)
        {
            if (t.gameObject.scene.isLoaded && t.gameObject.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return t.gameObject;
        }

        var texts = Resources.FindObjectsOfTypeAll<Text>();
        foreach (var t in texts)
        {
            if (t.gameObject.scene.isLoaded && t.gameObject.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return t.gameObject;
        }

        return null;
    }

    private static void EnsureMaterialDir()
    {
        if (!Directory.Exists(MaterialDir))
        {
            Directory.CreateDirectory(MaterialDir);
            AssetDatabase.Refresh();
        }
    }
}
