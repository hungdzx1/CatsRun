using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Tools > Chọn Map > Tạo nút cho các Map + gán vào SelectedMap
public static class MapButtonSetupHelper
{
    [MenuItem("Tools/Chọn Map/🖱️ Tạo nút cho các Map + tự gán SelectedMap", false, 1)]
    public static void Setup()
    {
        SelectedMap selectedMap = Object.FindFirstObjectByType<SelectedMap>(FindObjectsInactive.Include);
        if (selectedMap == null)
        {
            GameObject panel = FindSelectedMapPanel();
            if (panel != null)
            {
                selectedMap = Undo.AddComponent<SelectedMap>(panel);
                Debug.Log("<color=green>[CatsRun]</color> Đã tự động gắn component SelectedMap vào SelectedMapPanel!");
            }
            else
            {
                EditorUtility.DisplayDialog("Thông báo", "Không tìm thấy GameObject 'SelectedMapPanel' trong Scene!\nHãy mở Scene Main Menu rồi thử lại.", "OK");
                return;
            }
        }

        // Tìm tất cả các Map (Map1, Map2, Map3...) có con tên "Card"
        List<Transform> mapRoots = new List<Transform>();
        foreach (Transform t in selectedMap.GetComponentsInChildren<Transform>(true))
        {
            if (Regex.IsMatch(t.name, @"^Map\s*\d+$", RegexOptions.IgnoreCase) && t.Find("Card") != null)
                mapRoots.Add(t);
        }
        mapRoots.Sort((a, b) => GetNumber(a.name).CompareTo(GetNumber(b.name)));

        if (mapRoots.Count == 0)
        {
            EditorUtility.DisplayDialog("Thông báo", "Không tìm thấy Map1/Map2/... có con 'Card' bên trong SelectedMapPanel.", "OK");
            return;
        }

        SerializedObject so = new SerializedObject(selectedMap);
        SerializedProperty maps = so.FindProperty("maps");
        if (maps.arraySize < mapRoots.Count) maps.arraySize = mapRoots.Count;

        string report = "";
        for (int i = 0; i < mapRoots.Count; i++)
        {
            Transform root = mapRoots[i];
            Transform card = root.Find("Card");

            Button btn = SetupCardButton(card.gameObject);

            Transform selection = FindDeep(root, "SelectionFrame");
            Transform lockT = FindDeep(root, "Lock");
            Transform scoreFrame = FindDeepStartsWith(root, "FrameScore");
            TextMeshProUGUI scoreText = scoreFrame != null ? scoreFrame.GetComponentInChildren<TextMeshProUGUI>(true) : null;
            Transform unlock = lockT != null ? FindDeep(lockT, "UnlockText") : null;

            SerializedProperty slot = maps.GetArrayElementAtIndex(i);
            slot.FindPropertyRelative("mapNumber").intValue = GetNumber(root.name);
            slot.FindPropertyRelative("cardButton").objectReferenceValue = btn;
            slot.FindPropertyRelative("selectionFrame").objectReferenceValue = selection != null ? selection.gameObject : null;
            slot.FindPropertyRelative("lockObject").objectReferenceValue = lockT != null ? lockT.gameObject : null;
            slot.FindPropertyRelative("scoreFrame").objectReferenceValue = scoreFrame != null ? scoreFrame.gameObject : null;
            slot.FindPropertyRelative("highScoreText").objectReferenceValue = scoreText;
            slot.FindPropertyRelative("unlockText").objectReferenceValue = unlock != null ? unlock.GetComponent<TextMeshProUGUI>() : null;

            report += string.Format("• {0}: Button ✔ | SelectionFrame {1} | Lock {2} | FrameScore {3} | HighScore {4}\n",
                root.name, Mark(selection), Mark(lockT), Mark(scoreFrame), Mark(scoreText));
        }

        // Tự gán nút Select nếu tìm thấy
        SerializedProperty selectBtn = so.FindProperty("selectButton");
        if (selectBtn.objectReferenceValue == null)
        {
            Transform sel = FindDeep(selectedMap.transform, "Select");
            if (sel != null && sel.GetComponent<Button>() != null)
                selectBtn.objectReferenceValue = sel.GetComponent<Button>();
        }

        // Tự gán nút Exit / Back nếu tìm thấy
        SerializedProperty exitBtn = so.FindProperty("exitButton");
        if (exitBtn.objectReferenceValue == null)
        {
            Transform header = selectedMap.transform.Find("Header");
            Button foundBtn = null;
            if (header != null)
            {
                foundBtn = header.GetComponentInChildren<Button>(true);
            }
            if (foundBtn == null)
            {
                foreach (var b in selectedMap.GetComponentsInChildren<Button>(true))
                {
                    if (b.name.IndexOf("back", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        b.name.IndexOf("exit", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        b.name.IndexOf("close", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foundBtn = b;
                        break;
                    }
                }
            }
            if (foundBtn != null)
            {
                exitBtn.objectReferenceValue = foundBtn;
                report += string.Format("• Nút Exit: {0} ✔\n", foundBtn.name);
            }
        }

        so.ApplyModifiedProperties();
        EditorSceneManager.MarkSceneDirty(selectedMap.gameObject.scene);

        EditorUtility.DisplayDialog("Xong!", "Đã tạo nút và gán vào SelectedMap:\n\n" + report +
            "\n(✘ = không tìm thấy, hãy kéo tay trong Inspector)\n\nNhấn Ctrl + S để lưu Scene!", "OK");
    }

    [MenuItem("Tools/Chọn Map/🔓 Mở khóa tất cả Map (Để Test)", false, 10)]
    public static void UnlockAllMapsForTesting()
    {
        SelectedMap sm = Object.FindFirstObjectByType<SelectedMap>(FindObjectsInactive.Include);
        if (sm != null)
        {
            SerializedObject so = new SerializedObject(sm);
            so.FindProperty("unlockAllForTesting").boolValue = true;
            so.ApplyModifiedProperties();
            sm.Refresh();
            EditorSceneManager.MarkSceneDirty(sm.gameObject.scene);
            EditorUtility.DisplayDialog("Thành công!", "Đã BẬT chế độ mở khóa tất cả các map!\nBây giờ bạn có thể chọn và chơi thử Map 2 và Map 3 ngay lập tức.", "Tuyệt vời");
        }
    }

    [MenuItem("Tools/Chọn Map/🔒 Khóa lại các Map theo điểm thực", false, 11)]
    public static void LockMapsNormal()
    {
        SelectedMap sm = Object.FindFirstObjectByType<SelectedMap>(FindObjectsInactive.Include);
        if (sm != null)
        {
            SerializedObject so = new SerializedObject(sm);
            so.FindProperty("unlockAllForTesting").boolValue = false;
            so.ApplyModifiedProperties();
            sm.Refresh();
            EditorSceneManager.MarkSceneDirty(sm.gameObject.scene);
            EditorUtility.DisplayDialog("Thành công!", "Đã TẮT chế độ test! Các map sẽ bị khóa theo đúng điểm số thực tế.", "OK");
        }
    }

    // Button đặt trên Card, có Image trong suốt phủ toàn bộ Card để bấm vào đâu cũng nhận
    private static Button SetupCardButton(GameObject card)
    {
        Image img = card.GetComponent<Image>();
        if (img == null)
        {
            img = Undo.AddComponent<Image>(card);
            img.color = new Color(1f, 1f, 1f, 0f); // trong suốt, chỉ để nhận click
        }
        Undo.RecordObject(img, "Card Raycast");
        img.raycastTarget = true;

        Button btn = card.GetComponent<Button>();
        if (btn == null) btn = Undo.AddComponent<Button>(card);
        Undo.RecordObject(btn, "Card Button");
        btn.targetGraphic = img;
        btn.transition = Selectable.Transition.None; // không đổi màu thẻ khi bấm
        Navigation nav = btn.navigation;
        nav.mode = Navigation.Mode.None;
        btn.navigation = nav;
        EditorUtility.SetDirty(btn);
        return btn;
    }

    private static int GetNumber(string s)
    {
        Match m = Regex.Match(s, @"\d+");
        return m.Success ? int.Parse(m.Value) : 0;
    }

    private static string Mark(Object o) => o != null ? "✔" : "✘";

    private static Transform FindDeep(Transform parent, string name)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
            if (t != parent && t.name == name) return t;
        return null;
    }

    private static Transform FindDeepStartsWith(Transform parent, string prefix)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
            if (t != parent && t.name.StartsWith(prefix)) return t;
        return null;
    }

    public static GameObject FindSelectedMapPanel()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.name.Equals("SelectedMapPanel", System.StringComparison.OrdinalIgnoreCase))
            return Selection.activeGameObject;

        var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (var t in allTransforms)
        {
            if (t.gameObject.scene.isLoaded && t.gameObject.name.Equals("SelectedMapPanel", System.StringComparison.OrdinalIgnoreCase))
            {
                return t.gameObject;
            }
        }
        return null;
    }

    [MenuItem("Tools/Chọn Map/➕ Thêm Map 1, 2, 3 vào Build Settings", false, 2)]
    public static void AddScenesToBuildSettings()
    {
        string[] scenePaths = new string[]
        {
            "Assets/Scenes/Main Menu.unity",
            "Assets/Scenes/Map 1.unity",
            "Assets/Scenes/Map2.unity",
            "Assets/Scenes/Map 3.unity"
        };

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        int added = 0;

        foreach (string path in scenePaths)
        {
            if (System.IO.File.Exists(path))
            {
                bool exists = buildScenes.Exists(s => s.path == path);
                if (!exists)
                {
                    buildScenes.Add(new EditorBuildSettingsScene(path, true));
                    added++;
                }
            }
        }

        if (added > 0)
        {
            EditorBuildSettings.scenes = buildScenes.ToArray();
            EditorUtility.DisplayDialog("Thành công!", $"Đã thêm {added} Scene vào Build Settings!\nHiện tại bạn có thể load Map 1, Map2, Map 3 mà không lo lỗi.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Thông báo", "Tất cả các Scene Map đã có sẵn trong Build Settings rồi!", "OK");
        }
    }
}
