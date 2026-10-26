using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SelectedMap))]
public class SelectedMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space(6);
        
        GUI.backgroundColor = new Color(0.2f, 0.85f, 0.4f, 1f);
        if (GUILayout.Button("⚡ TỰ ĐỘNG GÁN TOÀN BỘ Ô (1-CLICK AUTO SETUP)", GUILayout.Height(40)))
        {
            MapButtonSetupHelper.Setup();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(6);
        EditorGUILayout.HelpBox("💡 HƯỚNG DẪN GẮN THỦ CÔNG:\n• Các ô (cardButton, lockObject...) nằm gom gọn bên trong danh sách 'Maps' bên dưới.\n• Bấm vào mũi tên ► bên cạnh chữ 'Maps' để mở danh sách.\n• Bấm tiếp vào ► Element 0 (Map 1), ► Element 1 (Map 2)... để thấy toàn bộ các ô để kéo thả!", MessageType.Info);
        EditorGUILayout.Space(6);

        DrawDefaultInspector();
    }
}
