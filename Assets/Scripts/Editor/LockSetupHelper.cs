using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class LockSetupHelper
{
    private const string RoundedMaskSpritePath = "Assets/Sprits/SelectedMap/UIMask_Rounded.png";
    private const string LockSpritePath = "Assets/Sprits/SelectedMap/lock.png";

    // Không tự chạy khi load nữa để tránh ghi đè các chỉnh sửa tay của bạn.
    // Dùng menu Tools > Khóa Card để chạy.

    [MenuItem("Tools/Khóa Card/🔒 Cấu hình hiệu ứng Lock che mờ Card %#l", false, 1)]
    public static void ManualSetupLock()
    {
        SetupLock(true);
    }

    [MenuItem("Tools/Khóa Card/👁️ Bật Lock (Xem hiệu ứng khóa)", false, 2)]
    public static void ToggleLockOn()
    {
        SetLockActive(true);
    }

    [MenuItem("Tools/Khóa Card/✨ Tắt Lock (Mở khóa Card)", false, 3)]
    public static void ToggleLockOff()
    {
        SetLockActive(false);
    }

    public static void SetupLock(bool isManual)
    {
        // 1. Find Lock GameObject
        GameObject lockObj = FindLockObject();
        if (lockObj == null)
        {
            if (isManual)
            {
                EditorUtility.DisplayDialog("Thông báo", "Không tìm thấy GameObject 'Lock' trong Scene!\nHãy chọn GameObject Lock trong Hierarchy rồi thử lại.", "OK");
            }
            return;
        }

        // 2. Find Sibling Card
        Transform map1Transform = lockObj.transform.parent;
        Transform cardTransform = null;
        if (map1Transform != null)
        {
            cardTransform = map1Transform.Find("Card");
        }

        if (cardTransform == null)
        {
            // Try search in scene
            var allCards = Resources.FindObjectsOfTypeAll<RectTransform>();
            foreach (var r in allCards)
            {
                if (r.gameObject.scene.isLoaded && r.name == "Card")
                {
                    cardTransform = r;
                    break;
                }
            }
        }

        if (cardTransform == null)
        {
            if (isManual)
            {
                EditorUtility.DisplayDialog("Thông báo", "Không tìm thấy GameObject 'Card' để căn kích thước cho Lock!", "OK");
            }
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(lockObj, "Setup Lock Overlay");

        RectTransform lockRect = lockObj.GetComponent<RectTransform>();
        RectTransform cardRect = cardTransform.GetComponent<RectTransform>();

        // 3. Make Lock match Card's exact on-screen rect (handles Card scale, stretch anchors, different parents)
        Vector3[] corners = new Vector3[4];
        Image bgImage = FindVisibleBackground(cardTransform);
        RectTransform fitRect = bgImage != null ? bgImage.rectTransform : cardRect;
        fitRect.GetWorldCorners(corners);
        Transform lockParent = lockRect.parent;
        Vector3 bl = lockParent != null ? lockParent.InverseTransformPoint(corners[0]) : corners[0];
        Vector3 tr = lockParent != null ? lockParent.InverseTransformPoint(corners[2]) : corners[2];

        lockRect.anchorMin = new Vector2(0.5f, 0.5f);
        lockRect.anchorMax = new Vector2(0.5f, 0.5f);
        lockRect.pivot = new Vector2(0.5f, 0.5f);
        lockRect.localScale = Vector3.one;
        lockRect.localRotation = Quaternion.identity;
        lockRect.sizeDelta = new Vector2(Mathf.Abs(tr.x - bl.x), Mathf.Abs(tr.y - bl.y));
        Vector3 center = (bl + tr) * 0.5f;
        lockRect.localPosition = new Vector3(center.x, center.y, 0f);

        // Ensure Lock is rendered on top of Card and SelectionFrame
        lockObj.transform.SetAsLastSibling();

        // 4. Setup Dark Rounded Dim Overlay on Lock
        Sprite roundedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RoundedMaskSpritePath);
        if (roundedSprite == null)
        {
            roundedSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }

        Image lockImage = lockObj.GetComponent<Image>();
        if (lockImage == null)
        {
            lockImage = lockObj.AddComponent<Image>();
        }

        // Use the visible background's own sprite so the dim area matches the card silhouette exactly
        Image cardImage = bgImage;
        if (cardImage != null && cardImage.sprite != null)
        {
            lockImage.sprite = cardImage.sprite;
            lockImage.type = cardImage.type;
            lockImage.preserveAspect = cardImage.preserveAspect;
            lockImage.pixelsPerUnitMultiplier = cardImage.pixelsPerUnitMultiplier;
            Debug.Log("[LockSetupHelper] Khớp màn che theo Image: " + cardImage.gameObject.name);
        }
        else
        {
            lockImage.sprite = roundedSprite;
            lockImage.type = Image.Type.Sliced;
            lockImage.preserveAspect = false;
        }
        // Semi-transparent dark overlay (65% opacity black/dark slate)
        lockImage.color = new Color(0.04f, 0.07f, 0.12f, 0.68f);
        lockImage.raycastTarget = true; // Blocks interaction with card while locked

        // 5. Setup Lock Icon as child
        Transform iconTransform = lockObj.transform.Find("IconLock");
        GameObject iconObj = iconTransform != null ? iconTransform.gameObject : null;
        if (iconObj == null)
        {
            // Check if there is an existing child
            if (lockObj.transform.childCount > 0)
            {
                iconObj = lockObj.transform.GetChild(0).gameObject;
                iconObj.name = "IconLock";
            }
            else
            {
                iconObj = new GameObject("IconLock", typeof(RectTransform), typeof(Image));
                iconObj.transform.SetParent(lockObj.transform, false);
            }
        }

        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        float iconSize = Mathf.Min(lockRect.sizeDelta.x, lockRect.sizeDelta.y) * 0.4f;
        iconRect.anchoredPosition = new Vector2(0f, iconSize * 0.35f); // Lift icon so icon + text are centered together
        iconRect.sizeDelta = new Vector2(iconSize, iconSize);
        iconRect.localScale = Vector3.one;

        // Load lock sprite
        Sprite lockSprite = LoadLockSprite();
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage == null) iconImage = iconObj.AddComponent<Image>();
        if (lockSprite != null) iconImage.sprite = lockSprite;
        iconImage.preserveAspect = true;
        iconImage.color = Color.white;
        iconImage.raycastTarget = false;

        // Add Shadow to Lock Icon for punchy 3D feel
        Shadow iconShadow = iconObj.GetComponent<Shadow>();
        if (iconShadow == null) iconShadow = iconObj.AddComponent<Shadow>();
        iconShadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
        iconShadow.effectDistance = new Vector2(2f, -3f);

        // 5b. Unlock requirement text below the lock icon
        SetupUnlockText(lockObj, lockRect, iconRect, iconSize);

        // 6. Ensure Lock is Active
        lockObj.SetActive(true);

        // Mark scene dirty
        EditorUtility.SetDirty(lockObj);
        EditorSceneManager.MarkSceneDirty(lockObj.scene);

        Debug.Log("<color=#FF4500><b>[CatsRun]</b></color> Đã cấu hình hiệu ứng Lock che mờ Card thành công!");

        if (isManual)
        {
            EditorUtility.DisplayDialog("Thành công!", "Đã cấu hình hiệu ứng Lock che mờ Card:\n\n• Nền đen mờ 68% bo góc ôm khít theo Card.\n• Icon ổ khóa 3D nổi bật ở chính giữa.\n• Chặn click vào Card khi đang khóa (Raycast Target).\n\nNhấn Ctrl + S để lưu Scene!", "Tuyệt vời");
        }
    }

    private const string FontAssetPath = "Assets/Font/LilitaOne-Regular SDF.asset";
    private const string LockTextMaterialPath = "Assets/Font/Materials/LilitaOne_LockText.mat";

    private static void SetupUnlockText(GameObject lockObj, RectTransform lockRect, RectTransform iconRect, float iconSize)
    {
        Transform t = lockObj.transform.Find("UnlockText");
        bool isNew = t == null;
        GameObject textObj = isNew
            ? new GameObject("UnlockText", typeof(RectTransform), typeof(TextMeshProUGUI))
            : t.gameObject;
        if (isNew)
        {
            Undo.RegisterCreatedObjectUndo(textObj, "Create UnlockText");
            textObj.transform.SetParent(lockObj.transform, false);
        }
        textObj.transform.SetAsLastSibling();

        // Layout: directly below the icon
        float textWidth = lockRect.sizeDelta.x * 0.9f;
        float textHeight = iconSize * 0.75f;
        RectTransform r = textObj.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 1f); // top-center, so it hangs below the icon
        r.sizeDelta = new Vector2(textWidth, textHeight);
        r.anchoredPosition = new Vector2(0f, iconRect.anchoredPosition.y - iconSize * 0.5f - iconSize * 0.08f);
        r.localScale = Vector3.one;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        if (isNew)
        {
            // Only set default text on creation so your later edits are kept
            tmp.text = "Unlock:\nHigh score: ";
        }

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (font != null)
        {
            tmp.font = font;
            Material mat = GetOrCreateLockTextMaterial(font);
            if (mat != null) tmp.fontSharedMaterial = mat;
        }

        tmp.alignment = TextAlignmentOptions.Top;
        tmp.color = Color.white;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 14f;
        tmp.fontSizeMax = Mathf.Max(20f, iconSize * 0.3f);
        tmp.lineSpacing = -10f;
        tmp.raycastTarget = false;
        tmp.ForceMeshUpdate();
        EditorUtility.SetDirty(tmp);
    }

    private static Material GetOrCreateLockTextMaterial(TMP_FontAsset font)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(LockTextMaterialPath);
        if (mat == null)
        {
            string dir = System.IO.Path.GetDirectoryName(LockTextMaterialPath);
            if (!AssetDatabase.IsValidFolder(dir))
            {
                AssetDatabase.CreateFolder("Assets/Font", "Materials");
            }
            mat = new Material(font.material) { name = "LilitaOne_LockText" };
            AssetDatabase.CreateAsset(mat, LockTextMaterialPath);
        }

        // White text, dark navy outline + soft drop shadow -> readable on the dark overlay
        mat.SetColor(ShaderUtilities.ID_FaceColor, Color.white);
        mat.EnableKeyword(ShaderUtilities.Keyword_Outline);
        mat.SetColor(ShaderUtilities.ID_OutlineColor, new Color(0.05f, 0.08f, 0.16f, 1f));
        mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.2f);
        mat.EnableKeyword(ShaderUtilities.Keyword_Underlay);
        mat.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(0f, 0f, 0f, 0.6f));
        mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.4f);
        mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.7f);
        mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.2f);
        mat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.2f);
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        return mat;
    }

    // Finds the largest visible Image inside Card (including Card itself) = the card's actual background
    private static Image FindVisibleBackground(Transform card)
    {
        Image best = null;
        float bestArea = 0f;
        Vector3[] c = new Vector3[4];
        foreach (var img in card.GetComponentsInChildren<Image>(true))
        {
            if (!img.enabled || !img.gameObject.activeInHierarchy) continue;
            if (img.sprite == null || img.color.a < 0.05f) continue;
            Mask m = img.GetComponent<Mask>();
            if (m != null && !m.showMaskGraphic) continue; // invisible mask shapes

            img.rectTransform.GetWorldCorners(c);
            float area = Mathf.Abs((c[2].x - c[0].x) * (c[2].y - c[0].y));
            if (area > bestArea)
            {
                bestArea = area;
                best = img;
            }
        }
        return best;
    }

    private static Sprite LoadLockSprite()
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(LockSpritePath);
        foreach (var obj in assets)
        {
            if (obj is Sprite s && s.name == "lock_0")
            {
                return s;
            }
        }
        foreach (var obj in assets)
        {
            if (obj is Sprite s) return s;
        }
        return null;
    }

    private static GameObject FindLockObject()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.name.Equals("Lock", System.StringComparison.OrdinalIgnoreCase))
            return Selection.activeGameObject;

        var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (var t in allTransforms)
        {
            if (t.gameObject.scene.isLoaded && t.gameObject.name.Equals("Lock", System.StringComparison.OrdinalIgnoreCase))
            {
                return t.gameObject;
            }
        }
        return null;
    }

    private static void SetLockActive(bool active)
    {
        GameObject lockObj = FindLockObject();
        if (lockObj != null)
        {
            Undo.RecordObject(lockObj, "Toggle Lock");
            lockObj.SetActive(active);
            EditorUtility.SetDirty(lockObj);
            EditorSceneManager.MarkSceneDirty(lockObj.scene);
            Debug.Log("<color=#FFD700><b>[CatsRun]</b></color> Lock đang: " + (active ? "BẬT (Khóa)" : "TẮT (Mở)"));
        }
    }
}
